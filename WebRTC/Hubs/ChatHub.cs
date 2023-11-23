using Microsoft.AspNetCore.SignalR;
using WebRTC.Models;

namespace WebRTC.Hubs
{
    public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private static Dictionary<string, ChatSession> ChatSessions = new Dictionary<string, ChatSession>();

        public async void StartVideoChat(string userId, string otherUserId)
        {
            // Bir görüntülü sohbet oturumu oluşturun.
            var chatSession = new ChatSession(userId);

            // Oturuma katılan kullanıcıları ekleyin.
            chatSession.AddUser(userId);
            chatSession.AddUser(otherUserId);

            // Oturumu tüm kullanıcılara bildirin.
            await Clients.All.SendAsync("StartVideoChat", chatSession);
        }
        public void ProcessImage(string userId, byte[] imageData)
        {
            // Görüntüyü işleyin (örneğin, dosyaya kaydedin, ekranda gösterin, vs.)

            // Örneğin, alınan görüntüyü tüm kullanıcılara göndermek için
            Clients.All.SendAsync("ReceiveImage", userId, imageData);
        }
        public async void EndVideoChat(string userId, string otherUserId)
        {
            // Oturumu al
            var chatSession = GetChatSession(userId, otherUserId);

            // Oturum varsa işlemi gerçekleştir
            if (chatSession != null)
            {
                // Oturumdan kullanıcıları kaldırın.
                chatSession.RemoveUser(userId);
                chatSession.RemoveUser(otherUserId);

                // Oturumdan çıkın.
                await Clients.All.SendAsync("EndVideoChat", chatSession.GroupId);
            }
        }
        public async Task CreateVideoChatGroup()
        {
            var callerId = Context.ConnectionId;

            // Bir görüntülü sohbet grubu oluşturun.
            var chatSession = new ChatSession(callerId);

            // Gruba kullanıcıları ekleyin.
            chatSession.AddUser(callerId);

            // Grubu tüm kullanıcılara bildirin.
            await Clients.All.SendAsync("CreateVideoChatGroup", chatSession);

            // Grup içindeki kullanıcıları gönderin.
            await Clients.All.SendAsync("GroupUsers", chatSession.Users);

            // Grubu oluşturan kullanıcı hariç, diğer kullanıcıları ekleyin.
            var otherUsers = ChatSessions.Values
                .Where(session => session.Users.Contains(callerId) && session.GroupId != chatSession.GroupId)
                .SelectMany(session => session.Users)
                .Distinct()
                .ToList();

            // Gruba eklenen diğer kullanıcılara bildirin.
            await Clients.Clients(otherUsers).SendAsync("GroupUsers", chatSession.Users);
        }
        public ChatSession GetChatSession(string userId1, string userId2)
        {
            // Oturum kimliğini oluşturun.
            string chatSessionId = string.Format("{0}-{1}", userId1, userId2);

            // Oturumu dizinden alın.
            ChatSession chatSession = ChatSessions[chatSessionId];

            // Oturum yoksa oluşturun.
            if (chatSession == null)
            {
                chatSession = new ChatSession(userId1);
                ChatSessions[chatSessionId] = chatSession;
            }

            return chatSession;
        }
        public class ChatGroup
        {
            public string GroupId { get; set; }

            public List<string> Users { get; set; } = new List<string>();

            public ChatGroup(string groupId)
            {
                this.GroupId = groupId;
            }

            public void AddUser(string userId)
            {
                this.Users.Add(userId);
            }
        }
    }
}