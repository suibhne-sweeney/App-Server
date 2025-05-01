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

        [HttpPatch("user/{id}/friends/{friendId}")]
        public async Task<IActionResult> FriendManager(string id, string friendId)
        {
            try
            {
                var user = await userCollection.Find(u => u.Id.ToString() == id).FirstOrDefaultAsync();
                var friend = await userCollection.Find(u => u.Id.ToString() == friendId).FirstOrDefaultAsync();

                if (user == null || friend == null)
                {
                    return NotFound(new { error = "User or friend not found" });
                }

            var update = user.Friends.Contains(friendId)
                ? Builders<User>.Update.Pull(u => u.Friends, friendId)
                : Builders<User>.Update.Push(u => u.Friends, friendId);

            await userCollection.UpdateOneAsync(u => u.Id.ToString() == id, update);
            var updatedUser = await userCollection.Find(u => u.Id.ToString() == id).FirstOrDefaultAsync();
            var updatedUserFriends = await userCollection.Find(u => updatedUser.Friends.Contains(u.Id.ToString())).ToListAsync();
                
            return Ok(updatedUserFriends);
            }
            catch (Exception e)
            { 
                return BadRequest(new { error = e.Message });
            }
        }
    }
}