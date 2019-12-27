using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Core.ViewModels;
using Microsoft.AspNetCore.SignalR;

namespace AMTools.Web.Hubs
{
    public class AvailabilityHub : Hub
    {
        private readonly ISubscriberService _subscriberService;
        private readonly ILogService _logService;

        public AvailabilityHub(
            ISubscriberService subscriberService,
            ILogService logService)
        {
            _subscriberService = subscriberService;
            _logService = logService;
        }

        public void SendToAll(List<SubscriberViewModel> subscriberViewModels)
        {
            Clients.All.SendAsync(nameof(SendToAll), subscriberViewModels);
        }

        public override async Task OnConnectedAsync()
        {
            _logService.Info($"Neuer {nameof(AvailabilityHub)} Benutzer verbunden!");
            await base.OnConnectedAsync();

            SendToAll(_subscriberService.GetAll());
        }
    }
}
