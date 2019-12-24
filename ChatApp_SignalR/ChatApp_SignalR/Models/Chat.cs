using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp_SignalR.Models
{
    public class Chat
    {
        public int ChatId { get; set; }
        public string MessageBody { get; set; }
        public string SenderId { get; set; }
        [ForeignKey("SenderId")]
        public virtual ApplicationUser SenderUser { get; set; }
        [Required]
        public string ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public virtual ApplicationUser ReceiverUser { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DateCreateed { get; set; }

        public bool? isRead { get; set; }

    }
}
