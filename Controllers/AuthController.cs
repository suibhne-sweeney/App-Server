using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Microsoft.AspNetCore.Identity.Data;

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
        
        [HttpPost("register")]
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
        
        // Using FromForm and not FromBody because it allows for file upload
        public async Task<IActionResult> Register([FromForm] User user)
        {
            try
            {
                // THIS CODE SEGMENT IS FOR FILE UPLOADING
                // it will upload to the ./Public folder 
                // the ./Public folder is where we will upload files. 
                // copy and paste this at the top of your function 
                // it works no edits needed
                string root = Directory.GetCurrentDirectory();
                
                var file = Request.Form.Files.FirstOrDefault();
                if(file != null)
                {
                    string pathToSave = Path.Combine(root, "Public");
                    if (!Directory.Exists(pathToSave)) Directory.CreateDirectory(pathToSave);

                    string fullPath = Path.Combine(pathToSave, file.FileName);
                    using FileStream stream = new FileStream(fullPath, FileMode.Create);
                    await file.CopyToAsync(stream);
                }
                
                // This is just me adding the user to the database nothing to do with the code above this
                var passwrodHash = PasswordHasher.HashPassword(user.Password!);
                user.Password = passwrodHash;
                user.CreatedAt = DateTime.Now;
                user.UpdatedAt = DateTime.Now;
                user.Friends = [];

                await userCollection.InsertOneAsync(user);
                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message }); 
            }
        }

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
                var userId = user.Id?.ToString() ?? "";

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