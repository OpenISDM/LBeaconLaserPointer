using System;
using System.Collections.Generic;
using System.Text;

namespace LLP_API
{
    public partial class BeaconInformation
    {
        public Guid Id { get; set; }
        public string Position { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Floor { get; set; }
    }

    public partial class LaserPointerInformation
    {
        public Guid Id { get; set; }
        public string Position { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double FaceLongitude { get; set; }
        public double FaceLatitude { get; set; }
        public string Floor { get; set; }
    }
}
