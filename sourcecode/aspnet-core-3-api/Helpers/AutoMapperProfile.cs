using AutoMapper;
using System;
using WebApi.Entities;
using WebApi.Models.Accounts;
using WebApi.Models.Chats;
using WebApi.Models.Comments;
using WebApi.Models.Follows;
using WebApi.Models.Notifications;
using WebApi.Models.Posts;
using WebApi.Models.Reactions;

namespace WebApi.Helpers
{
    public class AutoMapperProfile : Profile
    {
        // mappings between model and entity objects <source,des>
        public AutoMapperProfile()
        {

            //CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
            //Follow
            CreateMap<Message, MessageResponse>();

            CreateMap<CreateChatMessageRequest, Message>();

            //CreateMap<UpdateChatMessageRequest, ChatMessage>();

            //Follow
            CreateMap<Follow, FollowResponse>();

            CreateMap<CreateFollowRequest, Follow>();

            CreateMap<UpdateFollowRequest, Follow>();

            //Reaction
            CreateMap<Reaction, ReactionResponse>();

            CreateMap<CreateReactionRequest, Reaction>();

            CreateMap<UpdateReactionRequest, Reaction>();
            //Notification
            CreateMap<Notification, NotificationResponse>();

            CreateMap<CreateNotificationRequest, Notification>();

            CreateMap<UpdateNotificationRequest, Notification>();
            //Post
            CreateMap<Post, PostResponse>();
               
            CreateMap<PostResponse, Post>();

            CreateMap<CreatePostRequest, Post>();

            CreateMap<UpdatePostRequest, Post>();
            //Comment
            CreateMap<Comment, CommentResponse>();

            CreateMap<CommentResponse, Comment>();

            CreateMap<CreateCommentRequest, Comment>();

            CreateMap<UpdateCommentRequest, Comment>();
            //Account
            CreateMap<AppUser, AccountResponse>();

            CreateMap<AccountResponse, AppUser>();

            CreateMap<AppUser, AuthenticateResponse>();

            CreateMap<RegisterRequest, AppUser>();

            CreateMap<CreateAccountRequest, AppUser>();

            CreateMap<UpdateAccountRequest, AppUser>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                        // ignore null role
                        if (x.DestinationMember.Name == "Role" && src.Role == null) return false;

                        return true;
                    }
                ));
        }
    }
}