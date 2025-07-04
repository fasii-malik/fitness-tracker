using FitnessTracker.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace FitnessTracker.Infrastructure
{
    public class UserStatusHub : Hub
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserStatusHub(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public override async Task OnConnectedAsync()
        {
            var user = await _userManager.GetUserAsync(Context.User);
            if (user != null)
            {
                user.LastActivityDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                // Broadcast the updated status to all clients
                await Clients.All.SendAsync("ReceiveStatusUpdate", user.Id, user.IsOnline);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = await _userManager.GetUserAsync(Context.User);
            if (user != null)
            {
                // Set the LastActivityDate to a time that will indicate "Offline" status
                user.LastActivityDate = DateTime.UtcNow.AddMinutes(-5);
                await _userManager.UpdateAsync(user);

                // Broadcast the updated status to all clients
                await Clients.All.SendAsync("ReceiveStatusUpdate", user.Id, user.IsOnline);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
