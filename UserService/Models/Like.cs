using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Models
{
    public class Like 
    {
        public long Id { get; set; }

        public User User { get; set; }

        public long UserId { get; set; }

        public string CurrentUserName { get; set; }

        public long CurrentUserId { get; set; }
    }
}
