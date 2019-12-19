using System.Collections.Generic;
using AMTools.Web.Core.ViewModels;

namespace AMTools.Web.Core.Services.Settings.Interfaces
{
    public interface IQualificationService
    {
        List<QualificationViewModel> GetAll();
        List<QualificationViewModel> GetByReferenceString(string referenceString);
    }
}