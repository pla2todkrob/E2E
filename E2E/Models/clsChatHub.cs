using E2E.Models.Tables;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace E2E.Models
{
    public class clsChatHub : Hub
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