using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Web.Data.Database.Models;
using AutoMapper;

namespace AMTools.Web.Data.Database
{
    public class DatabaseMapProfile : Profile
    {
        public DatabaseMapProfile()
        {
            CreateMap<DbAlert, Alert>().ReverseMap();

            // TODO: Anfrage-URL zum DbAppLog hinzufügen
            // https://docs.automapper.org/en/stable/Before-and-after-map-actions.html

            CreateMap<DbAppLog, AppLog>().ReverseMap();

            CreateMap<DbAvailabilityStatus, AvailabilityStatus>().ReverseMap();

            CreateMap<DbSetting, Setting>().ReverseMap();

            CreateMap<DbSubscriber, Subscriber>().ReverseMap();

            CreateMap<DbUserResponse, UserResponse>().ReverseMap();
        }
    }
}
