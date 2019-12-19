using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Web.Core.Services.Settings.Interfaces;
using AMTools.Web.Core.ViewModels;
using AMTools.Web.Data.Database.Models;
using AutoMapper;

namespace AMTools.Web.Core.Mappings
{
    public class SubscriberMappingAction : IMappingAction<Subscriber, SubscriberViewModel>, IMappingAction<DbSubscriber, SubscriberViewModel>
    {
        private readonly IQualificationService _qualificationService;

        public SubscriberMappingAction(
            IQualificationService qualificationService)
        {
            _qualificationService = qualificationService;
        }

        public void Process(Subscriber source, SubscriberViewModel destination, ResolutionContext context)
        {
            if (!string.IsNullOrWhiteSpace(source?.Qualification))
            {
                destination.Qualifications = _qualificationService.GetByReferenceString(source.Qualification);
            }
        }

        public void Process(DbSubscriber source, SubscriberViewModel destination, ResolutionContext context)
        {
            if (!string.IsNullOrWhiteSpace(source?.Qualification))
            {
                destination.Qualifications = _qualificationService.GetByReferenceString(source.Qualification);
            }
        }
    }
}
