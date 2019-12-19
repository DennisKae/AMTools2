using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Web.Core.Mappings;
using AMTools.Web.Core.ViewModels;
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

            CreateMap<Setting, QualificationViewModel>()
                .AfterMap<QualificationMappingAction>();

            CreateMap<Setting, SubGroupViewModel>()
                .AfterMap<SubGroupMappingAction>();

            CreateMap<Setting, SeverityLevelViewModel>()
                .AfterMap<SeverityLevelMappingAction>();
        }
    }
}
