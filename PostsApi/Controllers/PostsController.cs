using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PostsApi.Data;

namespace PostsApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class PostsController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PostsController> _logger;
        public PostsController(ILogger<PostsController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public string Get(string from, string to)
        {
            var answerString = "";
            try
            {
                DateTime fromDate = Convert.ToDateTime(from);
                DateTime toDate = Convert.ToDateTime(to);

                var SerchingPosts = _context.Posts.Where(post =>
                       post.Date >= fromDate && post.Date <= toDate)
                   .OrderBy(post => post.Date);
                answerString = JsonConvert.SerializeObject(SerchingPosts);
                return answerString;
            }
            catch (Exception e)
            {
                return ($"Not correct parameter format: {e.Message}");
            }
        }

    }
}
