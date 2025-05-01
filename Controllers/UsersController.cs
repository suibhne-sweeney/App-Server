using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace App_Server.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        // Mongo DB Setup and Connection
        private readonly IMongoCollection<User> userCollection;
        
        public UsersController() 
        {
            DotNetEnv.Env.Load();
            var connectionString = Environment.GetEnvironmentVariable("MONGO_URI");
            var databaseName = "test";
            var collectionName = "users";

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            userCollection = database.GetCollection<User>(collectionName);
        }
        
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await userCollection.Find(new BsonDocument()).ToListAsync();
                return Ok(users);
            }
            catch (Exception e)
            { 
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpGet("getUser/{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            try
            {
                var user = await userCollection.Find(u => u.Id.ToString() == id).FirstOrDefaultAsync();
                if (user == null)
                {
                    return NotFound(new { error = "User not found" });
                }
                return Ok(user);
            }
            catch (Exception e)
            { 
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpGet("getUser/{id}/friends")]
        public async Task<IActionResult> GetUserFriends(string id)
        {
            try
            {
                var user = await userCollection.Find(u => u.Id.ToString() == id).FirstOrDefaultAsync();
                if (user == null)
                {
                    return NotFound(new { error = "User not found" });
                }
                return Ok(user.Friends);
            }
            catch (Exception e)
            { 
                return BadRequest(new { error = e.Message });
            }        
    }

        [HttpPatch("user/{id}/addFriend/{friendId}")]
        public async Task<IActionResult> AddFriend(string id, string friendId)
        {
            try
            {
                var user = await userCollection.Find(u => u.Id.ToString() == id).FirstOrDefaultAsync();
                if (user == null)
                {
                    return NotFound(new { error = "User not found" });
                }

                var friend = await userCollection.Find(u => u.Id.ToString() == friendId).FirstOrDefaultAsync();
                if (friend == null)
                {
                    return NotFound(new { error = "Friend not found" });
                }

                if (user.Friends.Contains(friendId))
                {
                    return BadRequest(new { error = "Already friends" });
                }

                user.Friends.Add(friendId);
                await userCollection.ReplaceOneAsync(u => u.Id.ToString() == id, user);
                
                return Ok(user);
            }
            catch (Exception e)
            { 
                return BadRequest(new { error = e.Message });
            }
        }
    }
}