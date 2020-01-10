using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMTools.Shared.Core.Models.Konfigurationen
{
    public class CacheKonfiguration
    {
        [Required]
        public double? DauerInMinuten { get; set; }
    }
}
