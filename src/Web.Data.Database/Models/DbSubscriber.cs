using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMTools.Web.Data.Database.Models
{
    public class DbSubscriber
    {
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        public string Issi { get; set; }

        public string Name { get; set; }

        public string Qualification { get; set; }


        public DateTime SysStampIn { get; set; }

        public DateTime? SysStampUp { get; set; }

        public bool SysDeleted { get; set; }
    }
}
