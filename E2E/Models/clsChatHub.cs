using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace E2E.Models
{
    public class ClsChatHub : Hub
    {
        public void JoinGroup(string groupName)
        {
            Groups.Add(Context.ConnectionId, groupName);
        }

        public Task SendMessageToGroup(string groupName, string message)
        {
            var res = Clients.Group(groupName).addChatMessage(message);
            return res;
        }
    }
}
