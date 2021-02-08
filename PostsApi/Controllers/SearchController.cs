using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PostsApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostsApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SearchController> _logger;
        public SearchController(ILogger<SearchController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public string Get(string text)
        {
            var answerString = "";
            try
            {

                var SerchingPosts = _context.Posts.Where(post =>
                       post.Text.IndexOf(text)!=-1);
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
