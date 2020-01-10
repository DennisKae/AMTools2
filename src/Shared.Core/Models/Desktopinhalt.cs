namespace AMTools.Shared.Core.Models
{
    public class Desktopinhalt
    {

        public string Anzeigetext { get; set; }


        public string Befehl { get; set; }


        public string Prozessbezeichnung { get; set; }

        /// <summary>Nummer des Ziel-Desktops</summary>
        public int? Desktop { get; set; }
    }
}