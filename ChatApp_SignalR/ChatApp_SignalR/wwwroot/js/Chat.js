"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ReceiveMessage", function (messageViewModel) {
    debugger
    if (messageViewModel.senderId == chatViewM.SelectedUserId() && messageViewModel.receiverId == chatViewM.Loggedinuser() ||
        messageViewModel.senderId == chatViewM.Loggedinuser() && messageViewModel.receiverId == chatViewM.SelectedUserId()) {
        $(".messages").animate({ scrollTop: $('.messages').prop("scrollHeight") }, "fast");
        for (var j = 0; j < chatViewM.Availableusers().length; j++) {
            if (messageViewModel.senderId == chatViewM.Availableusers()[j].id()) {
                chatViewM.Availableusers()[j].unreadMessage(0);
            }
        }
        chatViewM.userMessage.push(new chatViewM.MessageDetailMapping(messageViewModel));
    
    }
    else {
        for (var j = 0; j < chatViewM.Availableusers().length; j++) {
            if (messageViewModel.senderId == chatViewM.Availableusers()[j].id()) {
                chatViewM.Availableusers()[j].unreadMessage(chatViewM.Availableusers()[j].unreadMessage() + 1);

            }
        }
    }
});


connection.on("GetAllConnections", function (connections) {
    for (var j = 0; j < chatViewM.Availableusers().length; j++) {
        chatViewM.Availableusers()[j].isOnline(false);
    }
    $.each(connections, function (key, value) {
        for (var j = 0; j < chatViewM.Availableusers().length; j++) {
            if (key == chatViewM.Availableusers()[j].id()) {
                chatViewM.Availableusers()[j].isOnline(true);
            }

        }
    });
});


$("#messageInput").on('keypress', function (e) {

    if (e.which == 13) {
        $("#sendButton").trigger('click');
    }
});







// KnockOut Code Start

var vm = function () {
    var self = this;
    self.bool = ko.observable(false);
    self.showContent = ko.observable(false);
    self.MessageBody = ko.observable("");
    self.hasMessageBody = ko.observable(false);
    self.Availableusers = ko.observableArray();
    self.userMessage = ko.observableArray();
    self.SelectedUser = ko.observable();
    self.Loggedinuser = ko.observable();
    self.Message = ko.observable("");
    self.loadUsers = function () {
        $.ajax({
            url: '/Chat/getLoggedInUserId',
            asyn: false,
            success: function (data) {

                self.Loggedinuser(data);
            }
        });
        $.ajax({
            url: '/Chat/LoadAllUsers',
            success: function (data) {
                $.each(data, function (ind, user) {
                    $.ajax({
                        url: '/Chat/getCountofUnreadMessages',
                        data:
                        {
                            Id: user.id,
                        },
                        success: function (data) {
                            self.Availableusers.push(new self.userMapping(user, data));
                        }
                    });
                });

            }
        });

    }
    self.userMapping = function (object, unreadCount) {
        var self = this;
        self.id = ko.observable(object.id);
        self.userName = ko.observable(object.userName);
        self.unreadMessage = ko.observable(unreadCount);
        self.isOnline = ko.observable();
    }
    self.SelectedUserId = ko.observable();

    self.LoadMessageDetail = function (object) {
        self.showContent(true);
        object.unreadMessage(0);
        self.SelectedUser(object);
        self.SelectedUserId(object.id());
        self.userMessage([]);
        $.ajax({
            url: '/Chat/LoadChatDetail',
            async: false,
            data:
                { Id: object.id() },
            success: function (data) {
                
                $.each(data, function (ind, detail) {
                    
                    self.userMessage.push(new self.MessageDetailMapping(detail));
                });
            }
        });
        $(".messages").animate({ scrollTop: $('.messages').prop("scrollHeight") }, "fast");     
    }
    self.SendMessage = function () {
        if (self.MessageBody() == "") {
            console.log(self.MessageBody());
            self.hasMessageBody(false);
            alert("Can't Send Empty Message !!!");

        }
        else {
            connection.invoke("SendMessage", new self.SendMessageMapping()).catch(function (err) {
                return console.error(err.toString());
            });
        }
        $(".messages").animate({ scrollTop: $('.messages').prop("scrollHeight") }, "fast");
        self.MessageBody("");
    }
    self.MessageDetailMapping = function (object) {
        debugger
        var self = this;
        self.Message = ko.observable(object.messageBody);
        self.ReceiverId = ko.observable(object.receiverId);
        self.SenderId = ko.observable(object.senderId);
        self.MessageFrom = ko.observable(object.messageFrom);
        //var date = moment(object.dateCreateed).format('MMMM Do YYYY, h:mm:ss a')
        var date = moment(object.dateCreateed).format('MMMM Do, h:mm a')
        self.TimeSent = ko.observable(date);

    }
    self.SendMessageMapping = function () {
        this.MessageBody = self.MessageBody();
        this.ReceiverId = self.SelectedUserId();
        this.SenderId = self.Loggedinuser();
    }


    this.selectItem = function (item) {
        console.log(item.userName());
        self.selectedItem(item);
    }

};
var chatViewM = new vm();
chatViewM.loadUsers();
ko.applyBindings(chatViewM);




// KnockOut Code End