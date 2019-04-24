using System;
using System.Collections.Generic;
using System.Text;

namespace GpsApp
{
    public class LoginModel : BaseModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
    }
}
