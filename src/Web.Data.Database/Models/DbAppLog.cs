using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMTools.Web.Data.Database.Models
{
    public class DbAppLog
    {
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column(Order = 1)]
        public DateTime Timestamp { get; set; }

        [Column(Order = 2)]
        public AppLogSeverity Severity { get; set; }

        [Column(Order = 3)]
        public string Message { get; set; }

        public string ApplicationPart { get; set; }
        public string BatchCommand { get; set; }

        public bool SysDeleted { get; set; }
    }
}
