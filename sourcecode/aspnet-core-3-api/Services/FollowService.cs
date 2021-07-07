﻿using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Hubs;
using WebApi.Models.Follows;
using WebApi.Models.Notifications;

namespace WebApi.Services
{
    public interface IFollowService
    {
        Task<FollowResponse> CreateFollow(CreateFollowRequest model);
        Task<FollowResponse> UpdateFollow(int id, UpdateFollowRequest model);
        void DeleteFollow(int id);
        void DeleteFollowBySubjectId(int accountId, int followerId);

        Task<IEnumerable<FollowResponse>> GetAll();
        Task<IEnumerable<FollowResponse>> GetBySubjectId(int userId);
        Task<IEnumerable<FollowResponse>> GetByFollowerId(int userId);

        Task<FollowState> GetState(int accountId, int followerId);
    }
    public class FollowService : IFollowService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<PresenceHub> _presenceHub;
        private readonly PresenceTracker _tracker;

        public FollowService(DataContext context,
            IMapper mapper,
            IAccountService accountService,
            INotificationService notificationService,
            IHubContext<PresenceHub> presenceHub,
            PresenceTracker tracker)
        {
            _context = context;
            _mapper = mapper;
            _accountService = accountService;
            _notificationService = notificationService;
            _presenceHub = presenceHub;
            _tracker = tracker;
        }

        //Create
        public async Task<FollowResponse> CreateFollow(CreateFollowRequest model)
        {
            var follow = _mapper.Map<Follow>(model);
            if (follow == null) throw new AppException("Create follow failed");
            await _context.Follows.AddAsync(follow);
            await _context.SaveChangesAsync();
            SendNotification(follow);
            return _mapper.Map<FollowResponse>(follow);
        }

        //Update
        public async Task<FollowResponse> UpdateFollow(int id, UpdateFollowRequest model)
        {
            var follow = await getFollow(id);
            if (follow == null) throw new AppException("Update follow failed");
            _mapper.Map(model, follow);
            _context.Follows.Update(follow);
            await _context.SaveChangesAsync();

            return _mapper.Map<FollowResponse>(follow); ;
        }

        //Delete
        public async void DeleteFollow(int id)
        {
            var follow = getFollow(id);
            if (follow == null) throw new KeyNotFoundException("Follow not found");
            _context.Remove(follow);
            await _context.SaveChangesAsync();
        }

        public async void DeleteFollowBySubjectId(int accountId, int followerId)
        {
            var follow = _context.Follows.Where(follow => follow.SubjectId == accountId && follow.FollowerId == followerId).FirstOrDefault();
            if (follow == null) throw new KeyNotFoundException("Follow not found");
            _context.Remove(follow);
            await _context.SaveChangesAsync();
        }

        //Get all Follow
        public async Task<IEnumerable<FollowResponse>> GetAll()
        {
            var follows = await _context.Follows.ToListAsync();
            return _mapper.Map<List<FollowResponse>>(follows);
        }

        //Get all Follower of each user
        public async Task<IEnumerable<FollowResponse>> GetBySubjectId(int userId)
        {
            var follows = _context.Follows.Where(follow => follow.SubjectId == userId);
            var followers = _mapper.Map<List<FollowResponse>>(follows);
            foreach (FollowResponse follower in followers)
            {
                var account = await _accountService.GetById(follower.FollowerId);
                follower.FollowerName = account.Name;
                follower.FollowerAvatarPath = account.AvatarPath;
            }

            return followers;
        }

        //Get all Follow of each user
        public async Task<IEnumerable<FollowResponse>> GetByFollowerId(int userId)
        {
            var follows = _context.Follows.Where(follow => follow.FollowerId == userId);
            var subjects = _mapper.Map<List<FollowResponse>>(follows);
            foreach (FollowResponse subject in subjects)
            {
                var account = await _accountService.GetById(subject.SubjectId);
                subject.FollowerName = account.Name;
                subject.FollowerAvatarPath = account.AvatarPath;
            }
            return subjects;
        }

        public async Task<FollowState> GetState(int subjectId, int followerId)
        {

            var followCount = await _context.Follows.Where(follow => follow.SubjectId == subjectId && follow.FollowerId == followerId).CountAsync();
            var followState = new FollowState
            {
                IsCreated = false
            };
            if (followCount != 0)
            {
                followState.IsCreated = true;

            }
            return followState;
        }

        //Helper methods

        private async void SendNotification(Follow model)
        {
            var notification = new CreateNotificationRequest
            {
                ActionOwnerId = model.FollowerId,
                NotificationType = NotificationType.FollowRequest,
                PostId = 0,
                ReiceiverId = model.SubjectId,
                Created = DateTime.Now,
                Status = Status.Created
            };
           await _notificationService.CreateNotification(notification);
            var connections = await _tracker.GetConnectionForUser(notification.ReiceiverId);
            if (connections != null)
            {
                await _presenceHub.Clients.Clients(connections).SendAsync("NewNotification", notification);
            }
        }

        private async Task<Follow> getFollow(int id)
        {
            var follow = await _context.Follows.FindAsync(id);
            if (follow == null) throw new KeyNotFoundException("Follow not found");
            return follow;
        }
    }
}
