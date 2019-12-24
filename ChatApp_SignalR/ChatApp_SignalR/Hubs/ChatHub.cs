using ChatApp_SignalR.Data;
using ChatApp_SignalR.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp_SignalR.Hubs
{
    public class ChatHub : Hub
    {
        private ApplicationDbContext _db;
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();
        UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChatHub(ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SendMessage(MessageViewModel messageViewModel)
        {
            //await Clients.All.SendAsync("ReceiveMessage", user, message);


            string LoggedInUserId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            messageViewModel.DateCreateed = DateTime.Now;
            messageViewModel.isRead = false;
            try
            {
                _db.Chat.Add(new Chat
                {
                    MessageBody = messageViewModel.MessageBody,
                    ReceiverId = messageViewModel.ReceiverId,
                    SenderId = messageViewModel.SenderId,
                    DateCreateed = messageViewModel.DateCreateed,
                    isRead = messageViewModel.isRead,
                });
                _db.SaveChanges();
                var receiverconnectedId = _connections.GetConnections(messageViewModel.ReceiverId).FirstOrDefault();
                var senderconnectedId = _connections.GetConnections(messageViewModel.SenderId).FirstOrDefault();

                var messageModel = new MessageViewModel
                {
                    MessageBody = messageViewModel.MessageBody,
                    SenderId = messageViewModel.SenderId,
                    ReceiverId = messageViewModel.ReceiverId,
                    DateCreateed = messageViewModel.DateCreateed,
                    messageFrom = _httpContextAccessor.HttpContext.User.Identity.Name
                };

                //if (LoggedInUserId == messageModel.ReceiverId)
                //    messageModel.messageFrom = "receiver";
                //if (LoggedInUserId == messageModel.SenderId)
                //    messageModel.messageFrom = "sender";






                await Clients.Clients(receiverconnectedId, senderconnectedId).SendAsync("ReceiveMessage", messageModel);
                //await Clients.All.SendAsync("ReceiveMessage", user, message);

            }
            catch (Exception e)
            {

            }
        }

        public override Task OnConnectedAsync()
        {
            string name = _db.Users.Where(s => s.UserName == Context.User.Identity.Name).Select(s => s.Id).FirstOrDefault();
            _connections.Add(name, Context.ConnectionId);
            var connectedUsers = _connections.GetAllActiveConnections();
            Clients.All.SendAsync("GetAllConnections", connectedUsers);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string name = _db.Users.Where(s => s.UserName == Context.User.Identity.Name).Select(s => s.Id).FirstOrDefault();
            _connections.Remove(name, Context.ConnectionId);
            var offlineUsers = _connections.GetAllActiveConnections();
            Clients.All.SendAsync("GetAllConnections", offlineUsers);
            return base.OnDisconnectedAsync(exception);
        }
    }
}