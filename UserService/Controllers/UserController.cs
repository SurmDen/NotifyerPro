using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Models;
using IdentityManager.Models;
using IdentityManager.Infrastructure;
using UserManager.Models;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Newtonsoft.Json;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/userservice")]
    public class UserController : Controller
    {
        private IUserRepository userRepository;
        private IWebHostEnvironment hostEnvironment;

        public UserController(IUserRepository repository, IWebHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;
            userRepository = repository;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUserAsync([FromForm] FormUser user)
        {
            if (ModelState.IsValid)
            {
                HttpClient client = new HttpClient();

                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>
                {
                            {"Password", user.Password },
                            {"Role", "User" },
                            {"Name", user.Name },
                            {"Email" , user.Email}
                };

                var encodedUser = new FormUrlEncodedContent(keyValuePairs);

                Models.User rightUser = new Models.User()
                {
                    Password = user.Password,
                    Address = user.Address,
                    Name = user.Name,
                    Email = user.Email,
                    Telephone = user.Telephone
                };


                if (userRepository.GetUsers().Count() > 1)
                {
                    Models.User tempUser;

                    try
                    {
                        tempUser = userRepository.GetUsers().
                        Where(u => u.Password == PasswordHesher.HeshPassword(user.Password)).First();
                    }
                    catch
                    {
                        tempUser = null;
                    }

                    if (tempUser == null)
                    {
                        
                        await userRepository.CreateUserAsync(rightUser);

                        var innerresponce = await client.PostAsync("Http://localhost:5100/api/identityservice/login", encodedUser);

                        if (innerresponce.IsSuccessStatusCode)
                        {
                            string innertoken = await innerresponce.Content.ReadAsStringAsync();

                            return Ok(innertoken);
                        }
                    }
                    else
                    {
                        return BadRequest();
                    }
                }

                await userRepository.CreateUserAsync(rightUser);

                var responce = await client.PostAsync("Http://localhost:5100/api/identityservice/login", encodedUser);

                if (responce.IsSuccessStatusCode)
                {
                    string token =await responce.Content.ReadAsStringAsync();

                    return Ok(token);
                }
            }

            return BadRequest();
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateUserAsync([FromForm] Models.User user)
        {
            if (ModelState.IsValid)
            {
                Models.User tempUser = userRepository.GetUsers().
                       First(u => u.Password == PasswordHesher.HeshPassword(user.Password));

                if (userRepository.GetUsers().Where(u=>u.Id != user.Id).Count() != 0)
                {
                    if (tempUser == null)
                    {
                        user.Password = PasswordHesher.HeshPassword(user.Password);
                        await userRepository.UpdateUserAsync(user);
                        return Ok();
                    }
                    else
                    {
                        return BadRequest();
                    }
                }

                user.Password = PasswordHesher.HeshPassword(user.Password);
                await userRepository.UpdateUserAsync(user);

                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("remove")]
        public async Task DeleteUserAsync([FromForm]long id)
        {
            await userRepository.DeleteUserAsync(id);
        }

        [HttpGet("user/{id}")]
        public async Task<Models.User> GetUserAsync(long id)
        {
            return await userRepository.GetUserAsync(id) ?? null;
        }

        [HttpGet("users")]
        public IEnumerable<Models.User> GetAllUsers()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.
            Create("http://localhost:5100/api/identityservice/currentuser/email");

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string responseString;

            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    responseString = reader.ReadToEnd();
                }
            }

            IEnumerable<Models.User> exeptedUsers = userRepository.GetUsers().Where(u=>u.Email == responseString);

            IEnumerable<Models.User> users = userRepository.GetUsers().Except(exeptedUsers);

            if (users.Count() != 0)
            {
                return users;
            }

            return null;
        }

        [HttpPost("user")]
        public IActionResult GetUser([FromForm]RequestUser reqUser)
        {
            if (ModelState.IsValid)
            {
                Models.User user  = userRepository.GetUserByParameters(reqUser) ?? null;

                if (user != null)
                {
                    return Ok(user);
                }
            }

            return BadRequest();
        }

        [HttpPost("user/like")]
        public async Task<IActionResult> LikeUser([FromForm]LikesData data)
        {
            if (ModelState.IsValid)
            {
                await userRepository.AddLikeAsync(data);

                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("user/friendship")]
        public async Task<IActionResult> AddFriend([FromForm] FriendData data)
        {
            if (ModelState.IsValid)
            {
                await userRepository.AddFriendAsync(data);
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("user/addphoto")]
        public async Task<IActionResult> AddPhoto([FromForm]string path)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.
                Create("http://localhost:5100/api/identityservice/currentuser");

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string responseString;

            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    responseString = await reader.ReadToEndAsync();
                }
            }

            Models.User user = JsonConvert.DeserializeObject<Models.User>(responseString);

            if (!string.IsNullOrEmpty(path))
            {

                user.Photo = path;

                await userRepository.UpdateUserAsync(user);

                return Ok();

                
            }

            return BadRequest();
        }
    }
}
