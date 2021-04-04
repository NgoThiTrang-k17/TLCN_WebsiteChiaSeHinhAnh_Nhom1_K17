﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Follows;
using WebApi.Models.Notifications;
using WebApi.Models.Posts;

namespace WebApi.Services
{

    public interface IPostService
    {
        PostResponse GetById(int postId);
        PostResponse Create(CreatePostRequest model);
        PostResponse UpdatePost(int id, UpdatePostRequest model);
        void DeletePost(int id);
        IEnumerable<PostResponse> GetAll();
        IEnumerable<PostResponse> GetByOwnerId(int ownerId);
    }
    public class PostService : IPostService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;
        private readonly INotificationService _notificationService;
        private readonly IFollowService _followService;
        public PostService(
            DataContext context,
            IMapper mapper,
            IAccountService accountService,
            INotificationService notificationService,
            IFollowService followService)
        {
            _context = context;
            _mapper = mapper;
            _accountService = accountService;
            _notificationService = notificationService;
            _followService = followService;

        }
        //Create
        public PostResponse Create(CreatePostRequest model)
        {
            var post = _mapper.Map<Post>(model);
            _context.Posts.Add(post);
            _context.SaveChanges();
            SendNotification(post);
            return _mapper.Map<PostResponse>(post);
        }

        //Update
        public PostResponse UpdatePost(int id, UpdatePostRequest model)
        {
            var post = GetPost(id);
            post.PostTitle = model.Title;
            _context.Posts.Update(post);
            _context.SaveChanges();

            return _mapper.Map<PostResponse>(post);
        }

        //Delete
        public void DeletePost(int id)
        {
            var post = GetPost(id);
            _context.RemoveRange(GetComments(id));
            _context.RemoveRange(GetReactions(id));
            _context.Remove(post);
            _context.SaveChanges();
        }

        //Get all posts
        public IEnumerable<PostResponse> GetAll()
        {
            var posts = _context.Posts;
            var postResponses = _mapper.Map<IList<PostResponse>>(posts);
            foreach (PostResponse postResponse in postResponses)
            {
                var owner = _accountService.GetById(postResponse.OwnerId);
                postResponse.OwnerId = owner.Id;
                postResponse.OwnerName = owner.Name;
                postResponse.OwnerName = owner.Name;
                postResponse.OwnerAvatar = owner.AvatarPath;

                postResponse.FollowerCount = _context.Follows.Count(f => f.SubjectId == owner.Id);

                (postResponse.CommentCount, postResponse.ReactionCount) = GetPostInfor(postResponse.Id);
            }
            return postResponses;
        }

        //Get specific post by its Id
        public PostResponse GetById(int postId)
        {
            var post = GetPost(postId);
            var owner = _accountService.GetById(post.OwnerId);
            var response = _mapper.Map<PostResponse>(post);
            bool IsOwnerNameNull = owner.Name == null;
            response.OwnerName = IsOwnerNameNull ? "" : owner.Name;

            bool IsAvatarPathNull = owner.AvatarPath == null;
            response.OwnerAvatar = IsAvatarPathNull ? "" : owner.AvatarPath;

            response.FollowerCount = owner.FollowerCount;


            (response.CommentCount, response.ReactionCount) = GetPostInfor(response.Id);
            return response;
        }

        //Get posts for each user
        public IEnumerable<PostResponse> GetByOwnerId(int ownerId)
        {
            var posts = _context.Posts.Where(post => post.OwnerId == ownerId);
            var postResponses = _mapper.Map<IList<PostResponse>>(posts);
            foreach (PostResponse postResponse in postResponses)
            {
                var owner = _accountService.GetById(postResponse.OwnerId);
                postResponse.OwnerId = owner.Id;

                bool IsOwnerNameNull = owner.Name == null;
                postResponse.OwnerName = IsOwnerNameNull ? "" : owner.Name;

                bool IsAvatarPathNull = owner.AvatarPath == null;
                postResponse.OwnerAvatar = IsAvatarPathNull ? "" : owner.AvatarPath;

                postResponse.FollowerCount = _context.Follows.Count(f => f.SubjectId == owner.Id);

                (postResponse.CommentCount, postResponse.ReactionCount) = GetPostInfor(postResponse.Id);
            }
            return postResponses;
        }

        //helper
        private Post GetPost(int id)
        {
            var post = _context.Posts.Find(id);
            if (post == null) throw new KeyNotFoundException("Post not found");
            return post;
        }

        //Create Notification for posting
        private void SendNotification(Post post)
        {
            var follows = _followService.GetAllByUserId(post.OwnerId);
            //Create notification for each follower of Post owner
            foreach (FollowResponse follow in follows)
            {
                var notification = new CreateNotificationRequest
                {
                    //Post Owner
                    ActionOwnerId = post.OwnerId,
                    NotificationType = NotificationType.Posted,
                    PostId = post.Id,
                    ReiceiverId = follow.FollowerId,
                    Created = DateTime.Now,
                    Status = Status.Created
                };
                _notificationService.CreateNotification(notification);
            }
        }

        private (int, int) GetPostInfor(int id)
        {
            var commentcount = _context.Comments.Count(c => c.PostId == id);
            var reactioncount = _context.Reactions.Count(r => r.PostId == id);
            return (commentcount, reactioncount);
        }

        private IEnumerable<Comment> GetComments(int id)
        {
            return _context.Comments.Where(c => c.OwnerId == id);           
        }

        private IEnumerable<Reaction> GetReactions(int id)
        {
            return _context.Reactions.Where(c => c.OwnerId == id);
        }
    }
}