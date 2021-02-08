using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PostsApi.Data;
using PostsApi.Models;

namespace PostsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParsingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ParsingController> _logger;


        readonly HttpClient client = new HttpClient();
        readonly string url = "https://habr.com/ru/";

        public ParsingController(ILogger<ParsingController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        //[HttpGet]
        //public IActionResult Index()
        //{
        //    return Ok("Welcome to Admin Page!");
        //}

        [HttpGet]
        public async Task<IActionResult> Parse(int pageCount)
        {
            try
            {

                for (var i = 1; i <= pageCount; i++)
                {
                    var response = await client.GetAsync(url + "page" + i.ToString() + "/");

                    string source = "";

                    if (response != null && response.StatusCode == HttpStatusCode.OK)
                    {
                        source = await response.Content.ReadAsStringAsync();
                    }

                    HtmlDocument doc = new HtmlDocument();

                    doc.LoadHtml(source);

                    var html = doc.DocumentNode.InnerHtml.ToString();

                    var Posts_id = doc.DocumentNode.SelectNodes("//li[contains(@class,'content-list__item_post')]");

                    foreach (var item in Posts_id)
                    {
                        if (item.Attributes.Contains("id") == true && item.Attributes["id"].Value.ToString().IndexOf("post") != -1)
                        {
                            string Post_id = item.Attributes["id"].Value.ToString();
                            var href = url + Post_id.Split('_')[0] + "/" + Post_id.Split('_')[1] + "/";

                            var Post = await GetData(href);
                            await _context.Posts.AddAsync(Post);
                        }
                    }

                    await _context.SaveChangesAsync();
                }

                return null;

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        protected async Task<Post> GetData(string url)
        {
            Post post = new Post();
            try
            {


                var response = await client.GetAsync(url);

                string source = "";

                if (response != null && response.StatusCode == HttpStatusCode.OK)
                {
                    source = await response.Content.ReadAsStringAsync();
                }

                HtmlDocument doc = new HtmlDocument();

                doc.LoadHtml(source);

                post.Title = doc.DocumentNode.SelectSingleNode("//span[@class='post__title-text']").InnerText.ToString();

                var DateValues = doc.DocumentNode.SelectSingleNode("//span[@class='post__time']").Attributes["data-time_published"].Value.Split('-');

                var DayValues = DateValues[2].Split('T');

                DateTime Date = new DateTime(Int16.Parse(DateValues[0]), Int16.Parse(DateValues[1]), Int16.Parse(DayValues[0]), Int16.Parse(DayValues[1].Split(':')[0]), Int16.Parse(DayValues[1].Split(':')[1].Replace("Z", "")), 00);

                post.Date = Date;

                var ListofNodes = doc.DocumentNode.SelectSingleNode("//div[@class='post__wrapper']").SelectNodes("//li/a[contains(@class,'inline-list__item-link')]");

                foreach (var item in ListofNodes)
                {
                    if (item.Attributes.Contains("title") == true && item.Attributes["title"].Value == "Вы не подписаны на этот хаб")
                    {
                        post.Theme += item.InnerText + ";";
                    }
                }

                var TextofArticle = doc.DocumentNode.SelectSingleNode("//div[contains(@class,'post__body_full')]");


                post.Text += TextofArticle.InnerText;
                return post;


            }
            catch (Exception e)
            {
                
            }
            return post;
        }
    }
}
