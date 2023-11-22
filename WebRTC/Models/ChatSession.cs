namespace WebRTC.Models
{
    public class ChatSession
    {
        public string GroupId { get; set; }

        public List<string> Users { get; set; } = new List<string>();

        public ChatSession(string groupId)
        {
            this.GroupId = groupId;
        }

        public void AddUser(string userId)
        {
            this.Users.Add(userId);
        }

        public void RemoveUser(string userId)
        {
            this.Users.Remove(userId);
        }
    }
}
