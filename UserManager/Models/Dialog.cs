using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManager.Models
{
    public class Dialog
    {
        public long Id { get; set; }

        public long FirstUserId { get; set; }

        public long SecondUserId { get; set; }

        public List<Message> Messages { get; set; }

    }
}
