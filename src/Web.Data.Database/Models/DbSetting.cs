using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMTools.Web.Data.Database.Models
{
    public class DbSetting
    {
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public string Color { get; set; }

        public DateTime SysStampIn { get; set; }
    }
}
