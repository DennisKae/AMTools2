using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMTools.Core.Services.Logging;
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Core.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMTools.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriberController : BaseController
    {
        private readonly ISubscriberService _subscriberService;

        public SubscriberController(
            ISubscriberService subscriberService,
            ILogFactory logFactory) : base(logFactory)
        {
            _subscriberService = subscriberService;
        }

        /// <summary>Liefert alle Subscriber</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<SubscriberViewModel>), StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            return Execute(() =>
            {
                return _subscriberService.GetAll();
            });
        }
    }
}