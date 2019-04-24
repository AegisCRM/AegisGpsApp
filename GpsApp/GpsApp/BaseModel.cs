using System;
using System.Collections.Generic;
using System.Text;

namespace GpsApp
{
    public class BaseModel
    {
        public int ResponseCode { get; set; }
        public string Message { get; set; }
        public string DeviceId { get; set; }
    }
}
