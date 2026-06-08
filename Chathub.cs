using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace WhatsAppClone.Hubs // Apne project ka naam check kar lein
{
    public class ChatHub : Hub
    {
        private readonly IMongoCollection<ChatMessage> _messages;

        // Dependency Injection ke zariye database access
        public ChatHub(IMongoDatabase database)
        {
            _messages = database.GetCollection<ChatMessage>("Messages");
        }

        // Message bhejne aur save karne ka method
        public async Task SendMessage(string user, string message)
        {
            var newMessage = new ChatMessage
            {
                User = user,
                Text = message,
                Timestamp = DateTime.Now
            };

            // MongoDB mein data save karna
            await _messages.InsertOneAsync(newMessage);

            // Client ko message broadcast karna
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }

    // Message ka format/Model
    public class ChatMessage
    {
        public MongoDB.Bson.ObjectId Id { get; set; }
        public string User { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
    }
}