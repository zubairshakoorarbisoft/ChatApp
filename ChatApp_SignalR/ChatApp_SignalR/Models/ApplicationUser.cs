using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp_SignalR.Models
{
    public class ApplicationUser : IdentityUser
    {  
        public string FullName { get; set; }       
        public bool? inOnline { get; set; }
        public string ConnectionId { get; set; }
        public string ProfilePicture { get; set; }
    }
}
