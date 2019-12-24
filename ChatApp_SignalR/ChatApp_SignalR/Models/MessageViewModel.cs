using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp_SignalR.Models
{
    public class MessageViewModel
    {

        public int MessageId { get; set; }

        public string MessageBody { get; set; }

        public string SenderId { get; set; }

        public string ReceiverId { get; set; }

        public DateTime DateCreateed { get; set; }

        public bool? isRead { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string messageFrom { get; set; }

    }
}
