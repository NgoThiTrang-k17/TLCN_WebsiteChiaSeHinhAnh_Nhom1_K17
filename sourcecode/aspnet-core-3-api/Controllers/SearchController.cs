﻿
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.Models.Posts;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : BaseController
    {
        private readonly ISearchService _searchService;

        public SearchController(
            ISearchService searchService)
        {
            _searchService = searchService;
        }
        [HttpGet]
        public ActionResult<IEnumerable<PostResponse>> Search(string query)
        {

            if (query.StartsWith('@'))
            {
                query = query.Substring(1);
                var posts = _searchService.SearchForAccounts(Account.Id, query);
                return Ok(posts);
            }
            else
            {
                var posts = _searchService.SearchForPosts(query);
                return Ok(posts);
            }

        }
    }
}
