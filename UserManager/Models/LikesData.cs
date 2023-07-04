using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManager.Models
{
    public class LikesData
    {
        public long LikedUserId { get; set; }

        public long CurrentUserId { get; set; }

        public string CurrentUserName { get; set; }
    }
}
