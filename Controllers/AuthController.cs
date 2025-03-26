using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity.Data;
using MongoDB.Bson;


namespace App_Server.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMongoCollection<User> userCollection;
        private readonly JwtService _jwtService;

        public AuthController(JwtService jwtService) 
        {
            DotNetEnv.Env.Load();
            var connectionString = Environment.GetEnvironmentVariable("MONGO_URI");
            var databaseName = "test";
            var collectionName = "users";
            _jwtService = jwtService;

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            userCollection = database.GetCollection<User>(collectionName);
        }
        
        // [HttpPost("register")]
        // public async Task<IActionResult> Register([FromBody] User user)
        // {
        //     try
        //     {
        //         await userCollection.InsertOneAsync(user);
        //         return Ok(user);
        //     }
        //     catch (Exception e)
        //     {
                
        //         return NotFound(e);
        //     }
        // }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var email = loginRequest.Email;
                var password = loginRequest.Password;

                var user = await userCollection.Find(user => user.Email == email).FirstOrDefaultAsync();
                if(user == null) return BadRequest("User does not exist");

                bool isMatch = PasswordHasher.VerifyPassword(password, user.Password ?? "");
                if(!isMatch) return Unauthorized("Password is not correct.");

                DotNetEnv.Env.Load();
                var secret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "";
                var userId = user.Id.ToString() ?? "";

                var token = _jwtService.GenerateToken(userId, secret);

                var response = new {
                    user,
                    token
                };

                return Ok(response);
            }
            catch (Exception e)
            {
                
                return BadRequest(new { error = e.Message }); 
            }
        }
    }
}