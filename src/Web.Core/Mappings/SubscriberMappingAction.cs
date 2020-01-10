using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Web.Core.Services.Interfaces;
using AMTools.Web.Core.Services.Settings.Interfaces;
using AMTools.Web.Core.ViewModels;
using AMTools.Web.Data.Database.Models;
using AutoMapper;

namespace AMTools.Web.Core.Mappings
{
    public class SubscriberMappingAction : IMappingAction<Subscriber, SubscriberViewModel>, IMappingAction<DbSubscriber, SubscriberViewModel>
    {
        private readonly IQualificationSettingService _qualificationService;
        private readonly IAvailabilityStatusService _availabilityStatusService;

        public SubscriberMappingAction(
            IQualificationSettingService qualificationService,
            IAvailabilityStatusService availabilityStatusService)
        {
            _qualificationService = qualificationService;
            _availabilityStatusService = availabilityStatusService;
        }

        public void Process(Subscriber source, SubscriberViewModel destination, ResolutionContext context)
        {
            destination.Qualifications = _qualificationService.GetByReferenceString(source.Qualification);
            destination.AvailabilityStatus = _availabilityStatusService.GetByIssi(source.Issi);
        }

        public void Process(DbSubscriber source, SubscriberViewModel destination, ResolutionContext context)
        {
            destination.Qualifications = _qualificationService.GetByReferenceString(source.Qualification);
            destination.AvailabilityStatus = _availabilityStatusService.GetByIssi(source.Issi);
        }
    }
}
