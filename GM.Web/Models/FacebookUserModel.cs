using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GM.Models
{
    public class FacebookUserModel
    {
        public long id { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public DateTime birthday { get; set; }
        public string gender { get; set; }
        public string username { get; set; }
        public string AccessToken { get; set; }
    }
}