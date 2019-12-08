using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMTools.Web.Data.Database.Models
{
    public class DbAlert
    {
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Number { get; set; }

        public int Status { get; set; }

        public DateTime Timestamp { get; set; }

        public DateTime AlertTimestamp { get; set; }

        public string Text { get; set; }

        public string AlertedSubscribers { get; set; }

        public string Xml { get; set; }

        public DateTime SysStampIn { get; set; }
        public DateTime? SysStampUp { get; set; }
        public bool SysDeleted { get; set; }
    }
}
