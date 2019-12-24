using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ChatApp_SignalR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ChatApp_SignalR.Data;

namespace ChatApp_SignalR.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ILogger<ChatController> _logger;
        public ApplicationDbContext dbContext = null;
        UserManager<ApplicationUser> _userManager;
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();



        public ChatController(ILogger<ChatController> logger, ApplicationDbContext _dbContext, 
            UserManager<ApplicationUser> userManager)
        {
            dbContext = _dbContext;
            _userManager = userManager;
            _logger = logger;
        }

        public IActionResult Index()
        {

            return View("Chat");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpGet]
        public JsonResult getLoggedInUserId()
        {
            string LoggedInUserId = _userManager.GetUserId(HttpContext.User);
            return Json(LoggedInUserId);
        }

        public JsonResult LoadAllUsers()
        {

            var LoggedInUserId = _userManager.GetUserId(HttpContext.User);
            var userList = dbContext.Users.Where(s => s.Id != LoggedInUserId).OrderBy(s => s.UserName);
            return Json(userList);
        }

        [HttpGet]
        public JsonResult getCountofUnreadMessages(string Id)
        {
            string LoggedInUserId = _userManager.GetUserId(HttpContext.User);
            var unreadMessageCount = dbContext.Chat.Where(s => s.isRead == false && s.SenderId == Id && s.ReceiverId == LoggedInUserId)
            .Count();
            return Json(unreadMessageCount);
        }
        public JsonResult LoadChatDetail(string Id)
        {
            string LoggedInUserId = _userManager.GetUserId(HttpContext.User);

            List<Chat> chatModel = (from p in dbContext.Chat
                                    where p.SenderId == Id && p.ReceiverId == LoggedInUserId
                                    select p).ToList();

            foreach (Chat p in chatModel)
            {
                p.isRead = true;
            }
            dbContext.SaveChanges();

            var chatDetail = dbContext.Chat.Select(s => new MessageViewModel
            {
                MessageId = s.ChatId,
                MessageBody = s.MessageBody,
                SenderId = s.SenderId,
                SenderName = s.SenderUser.UserName,
                ReceiverId = s.ReceiverId,
                ReceiverName = s.ReceiverUser.UserName,
                DateCreateed = s.DateCreateed,
                //messageFrom = LoggedInUserId == s.SenderId? "sender": "receiver"
                messageFrom = HttpContext.User.Identity.Name
            })
            .Where(s => s.SenderId == LoggedInUserId && s.ReceiverId == Id || s.ReceiverId == LoggedInUserId && s.SenderId == Id)
            .OrderBy(s => s.DateCreateed)
            .ToList();
            return Json(chatDetail);
        }
    }
}
