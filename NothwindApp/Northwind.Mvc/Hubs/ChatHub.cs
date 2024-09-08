using Microsoft.AspNetCore.SignalR;
using Northwind.Chat.Models;
namespace Northwind.Mvc.Hubs
{
    public class ChatHub:Hub
    {
        private static Dictionary<string, string> users = new();

        public async Task Register(RegisterModel model)
        {
            users[model.Username] = Context.ConnectionId;

            foreach(string group in model.Groups.Split(','))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, group);
            }
        }

        public async Task SendMessage(MessageModel command)
        {
            MessageModel reply = new()
            {
                From = command.From,
                Body = command.Body
            };
            IClientProxy proxy;
            switch (command.ToType)
            {
                case "User":
                    string connectionId = users[command.To];
                    reply.To = $"{command.To} [{connectionId}]";
                    proxy = Clients.Client(connectionId);
                    break;
                case "Group":
                    reply.To = $"Group: {command.To}";
                    proxy = Clients.Group(command.To);
                    break;
                default:
                    reply.To = "Everyone";
                    proxy = Clients.All;
                    break;
            }
            await proxy.SendAsync(method: "ReceiveMessage", arg1: reply);
        }
    }
}
