using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManager.Models;
using IdentityManager.Models;

namespace UserService.Models
{
    public interface IUserRepository
    {
        public IEnumerable<User> GetUsers();

        public Task CreateUserAsync(User user);

        public User GetUserByParameters(RequestUser user);

        public Task UpdateUserAsync(User user);

        public Task<User> GetUserAsync(long id);

        public Task DeleteUserAsync(long id);

        public Task AddLikeAsync(LikesData data);

        public Task AddFriendAsync(FriendData data);
    }
}
