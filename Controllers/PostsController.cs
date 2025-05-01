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

        [HttpGet("{user_id}")] 
        public async Task<IActionResult> GetUserPosts(string user_id)
        {
            try
            {
                var posts = await postCollection.Find(p => p.UserId.ToString() == user_id).ToListAsync();
                if (posts == null || posts.Count == 0)
                {
                    return NotFound(new { error = "No posts found for this user" });
                }
                return Ok(posts);
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpPatch("{post_id}/like")]
        public async Task<IActionResult> likeManager(string post_id, [FromQuery] string user_id)
        {
            try {
                var post = await postCollection.Find(p => p.Id == new ObjectId(post_id)).FirstOrDefaultAsync();
                if (post == null)
                {
                    return NotFound(new { error = "Post not found" });
                }

                if (post.Likes == null)
                    post.Likes = new Dictionary<string, bool>();

                if (post.Likes.ContainsKey(user_id) && post.Likes[user_id])
                    post.Likes[user_id] = false;
                else
                    post.Likes[user_id] = true;

                var update = Builders<Post>.Update.Set(p => p.Likes, post.Likes);
                await postCollection.UpdateOneAsync(p => p.Id == new ObjectId(post_id), update);

                var updatedPost = await postCollection.Find(p => p.Id == new ObjectId(post_id)).FirstOrDefaultAsync();

                return Ok(updatedPost);
            } catch (Exception e) {
                return BadRequest(new { error = e.Message });
            }
        }

        
    }
}
