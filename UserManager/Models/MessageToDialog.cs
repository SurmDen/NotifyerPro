using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManager.Models
{
    public class MessageToDialog
    {
        public string Context { get; set; }

        public long CurrentUserId { get; set; }

        public long ReceiverId { get; set; }
    }
}
