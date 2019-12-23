using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMTools.Shared.Core.Models
{
    public class EmailSender
    {
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Hostname { get; set; }

        [Required]
        public int? Port { get; set; }

        public bool UseSsl { get; set; }

        /// <summary>Soll der Absender die versendeten Emails in Kopie erhalten?</summary>
        public bool GetEmailsInCopy { get; set; }
    }
}
