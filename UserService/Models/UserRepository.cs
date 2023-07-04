using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using IdentityManager.Models;
using IdentityManager.Infrastructure;
using UserManager.Models;

namespace UserService.Models
{
    public class UserRepository : IUserRepository
    {
        private DataContext context;

        public UserRepository(DataContext context)
        {
            this.context = context;
        }

        public IEnumerable<User> GetUsers()
        {
            IEnumerable<User> users = context.Users.Include(u => u.Friends).Include(u => u.Likes);

            foreach (User user in users)
            {
                foreach (Like like in user.Likes)
                {
                    like.User = null;
                }

                foreach (Friend friend in user.Friends)
                {
                    friend.User = null;
                }
            }

            return users;
        }
            

        public async Task CreateUserAsync(User user)
        {
            user.Password = PasswordHesher.HeshPassword(user.Password);
            user.Role = "User";

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(long id)
        {
            User user = await context.Users.FindAsync(id);

            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }

        public async Task<User> GetUserAsync(long id)
        {
            User user =  await context.Users.
                Include(u=>u.Likes).
                Include(u=>u.Friends).
                FirstAsync(u=>u.Id == id);

            foreach (Friend friend in user.Friends)
            {
                friend.User = null;
            }

            foreach (Like like in user.Likes)
            {
                like.User = null;
            }

            return user;
        }

        public async Task AddLikeAsync(LikesData data)
        {
            User likedUser = await context.Users.Include(u=>u.Likes).FirstAsync(u=>u.Id == data.LikedUserId);

            bool exists = false;
            long likeId = 0;

            foreach (Like like in likedUser.Likes)
            {
                if (like.CurrentUserId == data.CurrentUserId)
                {
                    exists = true;
                    likeId = like.Id;
                }
            }

            if (exists)
            {
                context.Likes.Remove(context.Likes.Find(likeId));
            }
            else
            {
                likedUser.Likes.Add(new Like() {CurrentUserName = data.CurrentUserName, CurrentUserId = data.CurrentUserId });
            }

            await context.SaveChangesAsync();
        }

        public async Task AddFriendAsync(FriendData data)
        {
            User current = await context.Users.Include(u=>u.Friends).FirstAsync(u=>u.Id == data.CurrentUserId);

            bool exists = false;
            long friendId = 0;

            foreach (Friend friend in current.Friends)
            {
                if (friend.AnotherUserId == data.FutureFriendId)
                {
                    exists = true;
                    friendId = friend.Id;
                }
            }

            if (exists)
            {
                context.Friends.Remove(context.Friends.Find(friendId));
            }
            else
            {
                current.Friends.Add(new Friend() { AnotherUserId = data.FutureFriendId });

                context.Users.Update(current);
            }

            await context.SaveChangesAsync();
        }

        public User GetUserByParameters(RequestUser user)
        {
            try
            {
                User ansUser = context.Users.
                Where(u => u.Password == PasswordHesher.HeshPassword(user.Password) && u.Email == user.Email)
                .Include(u => u.Friends).Include(u => u.Likes)
                .First();

                foreach (Friend friend in ansUser.Friends)
                {
                    friend.User = null;
                }

                foreach (Like like in ansUser.Likes)
                {
                    like.User = null;
                }

                return ansUser;
            }
            catch
            {
                return null;
            }
        }
        
    }
}
