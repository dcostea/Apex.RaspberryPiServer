using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apex.RaspberryPiServer.Models
{
    public class Reading
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReadingId { get; set; }
        public double Temperature { get; set; }
        public double Luminosity { get; set; }
        public double Infrared { get; set; }
        public double Distance { get; set; }
        public string Source { get; set; }
    }
}