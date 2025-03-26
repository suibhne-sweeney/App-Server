using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace App_Server.Controllers
{
    [ApiController]
    [Route("api/posts")]
    public class PostsController : ControllerBase
    {
        // Mongo DB Setup and Connection
        private readonly IMongoCollection<Post> postCollection;
        
        public PostsController() 
        {
            DotNetEnv.Env.Load();
            var connectionString = Environment.GetEnvironmentVariable("MONGO_URI");
            var databaseName = "test";
            var collectionName = "posts";

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            postCollection = database.GetCollection<Post>(collectionName);
        }
        
        [HttpGet("feed")]
        public async Task<IActionResult> GetFeedPosts()
        {
            try
            {
                var posts = await postCollection.Find(new BsonDocument()).ToListAsync();
                return Ok(posts);
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }
    }
}
