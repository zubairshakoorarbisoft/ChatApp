using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp_SignalR
{
    public class ChatUserViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime MessageTime { get; set; }
    }
}
