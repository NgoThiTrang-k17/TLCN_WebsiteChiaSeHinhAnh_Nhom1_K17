﻿using WebApi.Entities;

namespace WebApi.Models.Follows
{
    public class CreateFollowRequest
    {
        public int AccountId { get; set; }
        public int FollowerId { get; set; }
        public Status Status { get; set; }
    }
}