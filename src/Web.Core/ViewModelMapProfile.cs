using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Web.Core.Mappings;
using AMTools.Web.Core.Mappings.Settings;
using AMTools.Web.Core.ViewModels;
using AMTools.Web.Core.ViewModels.Settings;
using AMTools.Web.Data.Database.Models;
using AutoMapper;

namespace AMTools.Web.Core
{
    public class ViewModelMapProfile : Profile
    {
        public ViewModelMapProfile()
        {
            CreateMap<DbAlert, AlertViewModel>()
                .AfterMap<AlertMappingAction>()
                .ReverseMap();
            CreateMap<Alert, AlertViewModel>()
                .AfterMap<AlertMappingAction>()
                .ReverseMap();

            CreateMap<DbUserResponse, UserResponseViewModel>()
                .AfterMap<UserResponseMappingAction>()
                .ReverseMap();
            CreateMap<UserResponse, UserResponseViewModel>()
                .AfterMap<UserResponseMappingAction>()
                .ReverseMap();

            CreateMap<DbSubscriber, SubscriberViewModel>()
                .AfterMap<SubscriberMappingAction>()
                .ReverseMap();
            CreateMap<Subscriber, SubscriberViewModel>()
                .AfterMap<SubscriberMappingAction>()
                .ReverseMap();

            CreateMap<Setting, QualificationSettingViewModel>()
                .AfterMap<QualificationSettingMappingAction>();

            CreateMap<Setting, SubGroupSettingViewModel>()
                .AfterMap<SubGroupSettingMappingAction>();

            CreateMap<Setting, SeverityLevelSettingViewModel>()
                .AfterMap<SeverityLevelSettingMappingAction>();

            CreateMap<Setting, AvailabilityStatusSettingViewModel>()
                .AfterMap<AvailabilityStatusSettingMappingAction>();

            CreateMap<DbAvailabilityStatus, AvailabilityStatusViewModel>()
                .AfterMap<AvailabilityStatusMappingAction>();

            CreateMap<AvailabilityStatus, AvailabilityStatusViewModel>()
                .AfterMap<AvailabilityStatusMappingAction>();
        }
    }
}
