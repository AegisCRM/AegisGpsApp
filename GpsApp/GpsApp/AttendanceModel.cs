using System;
using System.Collections.Generic;
using System.Text;

namespace GpsApp
{
    public class AttendanceModel : BaseModel
    {
        public AttendanceModel() { }

        public string UserId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string IsMockedLocation { get; set; }
        public string AttendanceMode { get; set; }
        public string CurrentState { get; set; }
        public string Source { get; set; }
    }
}
