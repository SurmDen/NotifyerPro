using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManager.Models
{
    public class User
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Role { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Telephone { get; set; }

        public string Address { get; set; }

        public List<Like> Likes { get; set; }

        public List<Friend> Friends { get; set; }

        public string Photo { get; set; }
    }
}
