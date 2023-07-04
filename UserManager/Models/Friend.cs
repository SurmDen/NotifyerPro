using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManager.Models
{
    public class Friend
    {
        public long Id { get; set; }

        public User User { get; set; }

        public long UserId { get; set; }

        public long AnotherUserId { get; set; }
    }
}
