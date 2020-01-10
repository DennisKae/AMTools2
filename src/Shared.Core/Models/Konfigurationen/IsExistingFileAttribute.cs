using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMTools.Shared.Core.Models.Konfigurationen
{
    public class IsExistingFileAttribute : ValidationAttribute
    {
        public IsExistingFileAttribute() : base("Die Datei existiert nicht.")
        {

        }
        // TODO Prio 99: Configüberprüfung via custom validation attribute geht nicht
        public override bool IsValid(object value)
        {
            string stringValue = value as string;

            if (string.IsNullOrWhiteSpace(stringValue) || !File.Exists(stringValue))
            {
                Console.WriteLine("------ Die Datei existiert nicht: " + stringValue);
                return false;
            }

            return true;
        }
    }
}
