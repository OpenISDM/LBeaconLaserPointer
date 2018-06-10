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
        [Display(Name = "位置名稱")]
        public string Position { get; set; }
        [Display(Name = "緯度")]
        public double Latitude { get; set; }
        [Display(Name = "經度")]
        public double Longitude { get; set; }
        [Display(Name = "樓層")]
        public string Floor { get; set; }
    }

    public partial class LaserPointerInformation
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
        [Display(Name = "位置名稱")]
        public string Position { get; set; }
        [Display(Name = "經度")]
        public double Longitude { get; set; }
        [Display(Name = "緯度")]
        public double Latitude { get; set; }
        [Display(Name = "起始面向經度")]
        public double FaceLongitude { get; set; }
        [Display(Name = "起始面向緯度")]
        public double FaceLatitude { get; set; }
        [Display(Name = "樓層")]
        public string Floor { get; set; }
    }
}