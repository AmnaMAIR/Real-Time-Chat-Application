using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System;

namespace Whatsapp.Hubs // ⚠️ Note: YourProjectName ki jagah apne project ka naam likhein
{
    public class ChatHub : Hub
    {
        // Active connections ko user ke phone number ke sath map karne ke liye
        private static readonly ConcurrentDictionary<string, string> UserConnections = new ConcurrentDictionary<string, string>();

        public async Task RegisterUser(string phoneNumber)
        {
            UserConnections[phoneNumber] = Context.ConnectionId;
            await Clients.All.SendAsync("UserPresenceChanged", phoneNumber, true);
        }

        public async Task SendPrivateMessage(string senderNum, string receiverNum, string text)
        {
            var timeStr = DateTime.Now.ToString("hh:mm tt");

            // Check karein ke receiver online hai ya nahi (sent vs delivered tick)
            bool isReceiverOnline = UserConnections.ContainsKey(receiverNum);
            string initialStatus = isReceiverOnline ? "delivered" : "sent";

            // 1. Sender ki apni screen par message sync karein
            await Clients.Caller.SendAsync("ReceiveMessage", senderNum, receiverNum, text, timeStr, true, initialStatus);

            // 2. Agar receiver online hai, toh usko message send karein
            if (isReceiverOnline)
            {
                await Clients.Client(UserConnections[receiverNum]).SendAsync("ReceiveMessage", senderNum, receiverNum, text, timeStr, false, "delivered");
            }
        }

        // 🟢 NEW: Typing status ko target user tak pohanchane ke liye
        public async Task BroadcastTypingState(string senderNum, string receiverNum, bool isTyping)
        {
            if (UserConnections.TryGetValue(receiverNum, out string targetConnId))
            {
                await Clients.Client(targetConnId).SendAsync("ReceiveTypingState", senderNum, isTyping);
            }
        }

        // 🟢 NEW: Jab chat box khule toh Blue Ticks trigger karne ke liye
        public async Task MarkMessagesAsRead(string senderNum, string receiverNum)
        {
            if (UserConnections.TryGetValue(receiverNum, out string targetConnId))
            {
                await Clients.Client(targetConnId).SendAsync("MessagesMarkedAsRead", senderNum);
            }
        }

        public async Task SendGroupMessage(string senderNum, string senderName, string text)
        {
            var timeStr = DateTime.Now.ToString("hh:mm tt");
            await Clients.All.SendAsync("ReceiveGroupMessage", senderNum, senderName, text, timeStr);
        }

        public async Task UpdateIdentityState(string phone, string name, string about, string avatarBase64)
        {
            await Clients.All.SendAsync("IdentityChangedBroadcast", phone, name, about, avatarBase64);
        }
    }
}