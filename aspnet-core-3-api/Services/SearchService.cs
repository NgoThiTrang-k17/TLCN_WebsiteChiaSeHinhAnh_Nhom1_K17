﻿using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Accounts;
using WebApi.Models.Posts;

namespace WebApi.Services
{
    public interface ISearchService
    {
        //SearchResult<T> Search(string query, int page = 1, int pageSize = 10);

        //IEnumerable<string> Autocomplete(string query);

        //IEnumerable<string> Suggest(string query);

        //SearchResult<T> FindMoreLikeThis(string query, int pageSize);

        //SearchResult<Post> SearchByCategory(string query, IEnumerable<string> tags, int page, int pageSize);

        //T Get(string id);
        IEnumerable<PostResponse> SearchForPosts(string query);
        IEnumerable<AccountResponse> SearchForAccounts(int id, string query);
    }
    public class SearchService : ISearchService
    {
        
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        public SearchService(DataContext context,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _context = context;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        public IEnumerable<PostResponse> SearchForPosts( string query)
        {
            var posts = _context.Posts.Where(p=>p.PostTitle.Contains(query));
            return _mapper.Map<IList<PostResponse>>(posts);
        }


        public IEnumerable<AccountResponse> SearchForAccounts(int id, string query)
        {
            var accounts = _context.Accounts.Where(p => p.Name.Contains(query));
            var accountResponses = _mapper.Map<IList<AccountResponse>>(accounts);
            foreach (AccountResponse accountResponse in accountResponses)
            {
                accountResponse.FollowerCount = _context.Follows.Count(f => f.AccountId == accountResponse.Id);
                accountResponse.FollowingCount = _context.Follows.Count(f => f.FollowerId == accountResponse.Id);

                if (_context.Follows.Count(f => f.AccountId == accountResponse.Id && f.FollowerId == id) == 1)
                    accountResponse.IsFollowedByCurrentUser = 1;
                else
                    accountResponse.IsFollowedByCurrentUser = 0;
            }    
            return accountResponses;
        }

    }
    //public class ElasticIndexService
    //{
    //    private readonly IElasticClient client;

    //    public ElasticIndexService()
    //    {
    //        client = ElasticConfig.GetClient();
    //    }


    //    public void CreateIndex(string fileName, int maxItems)
    //    {
    //        if (!client.IndexExists(ElasticConfig.IndexName).Exists)
    //        {
    //            var indexDescriptor = new CreateIndexDescriptor(ElasticConfig.IndexName)
    //                .Mappings(ms => ms
    //                    .Map<Post>(m => m.AutoMap()));

    //            client.CreateIndex(ElasticConfig.IndexName, i => indexDescriptor);
    //        }

    //        BulkIndex(HostingEnvironment.MapPath("~/data/" + fileName), maxItems);
    //    }

    //    private IEnumerable<Post> LoadPostsFromFile(string inputUrl)
    //    {
    //        using (XmlReader reader = XmlReader.Create(inputUrl))
    //        {
    //            reader.MoveToContent();
    //            while (reader.Read())
    //            {
    //                if (reader.NodeType == XmlNodeType.Element && reader.Name == "row")
    //                {
    //                    if (String.Equals(reader.GetAttribute("PostTypeId"), "1"))
    //                    {
    //                        XElement el = XNode.ReadFrom(reader) as XElement;

    //                        if (el != null)
    //                        {
    //                            Post post = new Post
    //                            {
    //                                Id = el.Attribute("Id").Value,
    //                                Title = el.Attribute("Title") != null ? el.Attribute("Title").Value : "",
    //                                CreationDate = DateTime.Parse(el.Attribute("CreationDate").Value),
    //                                Score = int.Parse(el.Attribute("Score").Value),
    //                                Body = HtmlRemoval.StripTagsRegex(el.Attribute("Body").Value),
    //                                Tags =
    //                                    el.Attribute("Tags") != null
    //                                        ? el.Attribute("Tags")
    //                                            .Value.Replace("><", "|")
    //                                            .Replace("<", "")
    //                                            .Replace(">", "")
    //                                            .Replace("&gt;&lt;", "|")
    //                                            .Replace("&lt;", "")
    //                                            .Replace("&gt;", "")
    //                                            .Split('|')
    //                                        : null,
    //                                AnswerCount =
    //                                    el.Attribute("AnswerCount") != null
    //                                        ? int.Parse(el.Attribute("AnswerCount").Value)
    //                                        : 0
    //                            };
    //                            post.Suggest = post.Tags;
    //                            yield return post;
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    private void BulkIndex(string path, int maxItems)
    //    {
    //        int i = 0;
    //        int take = maxItems;
    //        int batch = 1000;
    //        foreach (var batches in LoadPostsFromFile(path).Take(take).Batch(batch))
    //        {
    //            i++;
    //            var result = client.IndexMany<Post>(batches, ElasticConfig.IndexName);
    //        }
    //    }
    //}
    //public class ElasticSearchService : ISearchService<Post>
    //{
    //    private readonly IElasticClient client;

    //    public ElasticSearchService()
    //    {
    //        client = ElasticConfig.GetClient();
    //    }

    //    public SearchResult<Post> Search(string query, int page, int pageSize)
    //    {
    //        var result = client.Search<Post>(x => x.Query(q => q
    //                                                    .MultiMatch(mp => mp
    //                                                        .Query(query)
    //                                                            .Fields(f => f
    //                                                                .Fields(f1 => f1.Title, f2 => f2.Body, f3 => f3.Tags))))
    //                                                .Aggregations(a => a
    //                                                    .Terms("by_tags", t => t
    //                                                        .Field(f => f.Tags)
    //                                                        .Size(10))));

    //        return new SearchResult<Post>
    //        {
    //            Total = (int)result.Total,
    //            Page = page,
    //            Results = result.Documents,
    //            ElapsedMilliseconds = result.Took,
    //            AggregationsByTags = result.Aggs.Terms("by_tags").Buckets.ToDictionary(x => x.Key, y => y.DocCount.GetValueOrDefault(0))
    //        };
    //    }

    //    public SearchResult<Post> SearchByCategory(string query, IEnumerable<string> tags, int page = 1,
    //        int pageSize = 10)
    //    {

    //        var filters = tags.Select(c => new Func<QueryContainerDescriptor<Post>, QueryContainer>(x => x.Term(f => f.Tags, c))).ToArray();

    //        var result = client.Search<Post>(x => x
    //            .Query(q => q
    //                .Bool(b => b
    //                    .Must(m => m
    //                        .MultiMatch(mp => mp
    //                            .Query(query)
    //                                .Fields(f => f
    //                                    .Fields(f1 => f1.Title, f2 => f2.Body, f3 => f3.Tags))))
    //                    .Filter(f => f
    //                        .Bool(b1 => b1
    //                            .Must(filters)))))
    //            .Aggregations(a => a
    //                .Terms("by_tags", t => t
    //                    .Field(f => f.Tags)
    //                    .Size(10)
    //                )
    //            )
    //            .From(page - 1)
    //            .Size(pageSize));

    //        return new SearchResult<Post>
    //        {
    //            Total = (int)result.Total,
    //            Page = page,
    //            Results = result.Documents,
    //            ElapsedMilliseconds = result.Took,
    //            AggregationsByTags = result.Aggs.Terms("by_tags").Buckets.ToDictionary(x => x.Key, y => y.DocCount.GetValueOrDefault(0))
    //        };
    //    }

    //    public IEnumerable<string> Autocomplete(string query)
    //    {
    //        var result = client.Suggest<Post>(x => x.Completion("tag-suggestions", c => c.Text(query)
    //            .Field(f => f.Suggest)
    //            .Size(6)));

    //        return result.Suggestions["tag-suggestions"].SelectMany(x => x.Options).Select(y => y.Text);
    //    }

    //    public IEnumerable<string> Suggest(string query)
    //    {
    //        var result = client.Suggest<Post>(x => x.Term("post-suggestions", t => t.Text(query)
    //            .Field(f => f.Body)
    //            .Field(f => f.Title)
    //            .Field(f => f.Tags)
    //            .Size(6)));

    //        return result.Suggestions["post-suggestions"].SelectMany(x => x.Options).Select(y => y.Text);
    //    }

    //    public SearchResult<Post> FindMoreLikeThis(string id, int pageSize)
    //    {
    //        var result = client.Search<Post>(x => x
    //            .Query(y => y
    //                .MoreLikeThis(m => m
    //                    .Like(l => l.Document(d => d.Id(id)))
    //                    .Fields(new[] { "title", "body", "tags" })
    //                    .MinTermFrequency(1)
    //                    )).Size(pageSize));

    //        return new SearchResult<Post>
    //        {
    //            Total = (int)result.Total,
    //            Page = 1,
    //            Results = result.Documents
    //        };
    //    }

    //    public Post Get(string id)
    //    {
    //        var result = client.Get<Post>(new DocumentPath<Post>(id));
    //        return result.Source;
    //    }
    //}
}
