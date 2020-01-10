using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMTools.Web.Data.Database.Models
{
    public class DbUserResponse
    {
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int AlertId { get; set; }

        public string Issi { get; set; }

        public bool Accept { get; set; }

        public string Color { get; set; }

        public string Response { get; set; }

        public DateTime Timestamp { get; set; }

        public DateTime SysStampIn { get; set; }

        public bool SysDeleted { get; set; }
    }
}
