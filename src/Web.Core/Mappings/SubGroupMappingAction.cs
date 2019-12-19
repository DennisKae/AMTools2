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
    public class SubGroupMappingAction : IMappingAction<Setting, SubGroupViewModel>
    {
        public void Process(Setting source, SubGroupViewModel destination, ResolutionContext context)
        {
            destination.Nummer = int.TryParse(source?.Key, out int parsedNumber) ? parsedNumber : default;
            destination.Bezeichnung = source?.Value;
        }
    }
}
