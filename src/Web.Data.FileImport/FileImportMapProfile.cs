using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Web.Data.Files.Models.Callout;
using AutoMapper;

namespace AMTools.Web.Data.Files
{
    public class FileImportMapProfile : Profile
    {
        public FileImportMapProfile()
        {
            CreateMap<AlertImportModel, Alert>();
            CreateMap<AlertUserResponseImportModel, UserResponse>();
        }
    }
}
