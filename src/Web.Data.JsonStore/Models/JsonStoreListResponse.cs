using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMTools.Web.Data.JsonStore.Models
{
    public class JsonStoreListResponse<T>
    {
        public List<T> Result { get; set; }
        public bool Ok { get; set; }
    }
}
