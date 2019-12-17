using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMTools.Shared.Core.Models;
using AMTools.Web.Core.ViewModels;
using AutoMapper;

namespace AMTools.Web.Core.Mappings
{
    public class QualificationMappingAction : IMappingAction<Setting, QualificationViewModel>
    {
        public void Process(Setting source, QualificationViewModel destination, ResolutionContext context)
        {
            destination.Referenz = source?.Key;
            destination.Bezeichnung = source?.Value;
        }
    }
}
