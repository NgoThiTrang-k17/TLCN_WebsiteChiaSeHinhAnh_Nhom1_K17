﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.Models.Comments;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController : BaseController
    {
        private readonly ICommentService _commentService;
        private readonly IMapper _mapper;

        public CommentController(
            ICommentService commentService,
            IMapper mapper)
        {
            _commentService = commentService;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommentResponse>> GetAll()
        {
            var comments = _commentService.GetAll();
            return Ok(comments);
        }

        [HttpGet("{id:int}")]
        public ActionResult<IEnumerable<CommentResponse>> GetAllByPostId(int id)
        {
            var comments = _commentService.GetAllByPostId(id);
            return Ok(comments);
        }

        [HttpPost]
        public ActionResult<CommentResponse> Create(CreateCommentRequest comment)
        {
            var model = new CreateCommentRequest
            {
                Content = comment.Content,
                DateCreated = DateTime.Now,
                OwnerId = //Account.Id,
                comment.OwnerId, 
                PostId = //Post.Id
                comment.PostId
            };
            _commentService.CreateComment(model);
            return Ok(new { message = "Adding comment succesful!" });
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public ActionResult<CommentResponse> Update(int id, CommentResponse model)
        {

            var post = _commentService.UpdateComment(id, model);

            return Ok(new { message = "Updating comment succesful!" });
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            // users can delete their own Comment and admins can delete any Comment
            if (id != Account.Id && Account.Role != Role.Admin)
                return Unauthorized(new { message = "Unauthorized" });

            _commentService.DeleteComment(id);
            return Ok(new { message = "Comment deleted successfully" });
        }
    }
}
