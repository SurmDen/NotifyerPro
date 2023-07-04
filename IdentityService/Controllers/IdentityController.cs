using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using IdentityManager.Models;
using System.Net;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using UserManager.Models;
using Microsoft.Extensions.Caching.Memory;

namespace IdentityService.Controllers
{
    [ApiController]
    [Route("api/identityservice")]
    public class IdentityController : Controller
    {
        private static int tempUserId = 10000000;
        private IMemoryCache cache;
        private ITokenService tokenService;

        public IdentityController(ITokenService tokenService, IMemoryCache cache)
        {
            this.tokenService = tokenService;
            this.cache = cache;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm]RequestUser requser)
        {
            if (ModelState.IsValid)
            {
                
                HttpClient client = new HttpClient();

                Dictionary<string, string> content = new Dictionary<string, string>()
                {
                    {"Password", $"{requser.Password}" },
                    { "Role", $"{requser.Role}"},
                    {"Name", $"{requser.Name}" },
                    {"Email", $"{requser.Email}" }
                };

                var encodedContent = new FormUrlEncodedContent(content);

                var response = await client.PostAsync("http://localhost:5200/api/userservice/user", encodedContent);

                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();

                    User user = JsonConvert.DeserializeObject<User>(responseString);

                    requser.Role = user.Role;

                    string Token = tokenService.GenerateToken(requser);

                    if (tokenService.ValidateToken(Token))
                    {
                        try
                        {
                            cache.Remove("current");
                        }
                        catch
                        {

                        }

                        cache.Set<User>("current", user);
                        cache.Set<string>("email", user.Email);

                        return Ok(Token);
                    }
                }
            }

            return BadRequest();
        }

        [HttpGet("currentuser")]
        public IActionResult GetCurrentUser()
        {
            User user = (User)cache.Get("current") ?? null ;

            if (user != null)
            {
                return Ok(user);
            }

            return BadRequest();
        }

        [HttpGet("currentuser/email")]
        public string GetCurrentUsersEmail()
        {
            string email = (string)cache.Get("email");
            return email;
        }

        [HttpPost("current/update/friends")]
        public IActionResult UpdateFriends([FromForm] FriendData data)
        {
            User user = (User)cache.Get("current");
            cache.Remove("current");

            bool exists = false;
            long friendId = 0;

            foreach (Friend friend in user.Friends)
            {
                if (friend.AnotherUserId == data.FutureFriendId)
                {
                    exists = true;
                    friendId = friend.Id;
                }
            }

            if (exists)
            {
                user.Friends.Remove(user.Friends.Where(f=>f.Id == friendId).First());
            }
            else
            {
                tempUserId++;

                user.Friends.Add(new Friend() { UserId = user.Id, AnotherUserId = data.FutureFriendId, Id = tempUserId });

            }

            cache.Set<User>("current", user);

            return Ok();
        }

        [HttpPost("current/update/photo")]
        public IActionResult UpdatePhoto([FromForm] string path)
        {
            User user = (User)cache.Get("current");

            cache.Remove("current");

            user.Photo = path;

            cache.Set<User>("current", user);

            return Ok();
        }
    }
}
