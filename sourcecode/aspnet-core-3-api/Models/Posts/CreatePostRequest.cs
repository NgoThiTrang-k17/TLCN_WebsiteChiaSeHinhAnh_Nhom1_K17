using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Posts
{
    public class CreatePostRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public string Categories { get; set; }
        public string Path { get; set; }

        public int OwnerId { get; set; }
    }
}