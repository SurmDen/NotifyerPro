using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManager.Models
{
    public class Message
    {
        public long Id { get; set; }

        public long UsersId { get; set; }

        public string Context { get; set; }

        public Dialog Dialog { get; set; }

        public long DialogId { get; set; }
    }
}
