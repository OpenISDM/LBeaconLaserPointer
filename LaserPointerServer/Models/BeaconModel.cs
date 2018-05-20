using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LBeacon.Models
{
    public partial class BeaconInformation
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
        [Display(Name = "Beacon名稱")]
        public string Name { get; set; }
        [Display(Name = "緯度")]
        public double Latitude { get; set; }
        [Display(Name = "經度")]
        public double Longitude { get; set; }
        [Display(Name = "位置")]
        public string Place { get; set; }
    }
}