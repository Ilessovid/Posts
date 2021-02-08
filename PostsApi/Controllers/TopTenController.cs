using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PostsApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PostsApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class TopTenController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TopTenController> _logger;
        public TopTenController(ILogger<TopTenController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        public string Get()
        {
            var allText = "";
            string answerString = "";
            try
            {
                var arrayOfPosts = _context.Posts.ToArray();

                foreach (var item in arrayOfPosts)
                {
                    allText += item.Text;
                }
                var a = Regex.Split(allText.ToLower(), @"\W+").Where(s => s.Length > 3).GroupBy(s => s).OrderByDescending(g => g.Count()).ToArray();

                for (int i = 0; i <= 10; i++)
                {
                    answerString += "WORD :" + a[i].Key.ToString() + " Count:" + a[i].Count().ToString() + "\n";
                }

                //answerString = JsonConvert.SerializeObject(SerchingPosts);
                return answerString;
            }
            catch (Exception e)
            {
                return ($"Not correct parameter format: {e.Message}");
            }
        }
    }
}
