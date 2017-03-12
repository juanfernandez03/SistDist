using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwitterSD.Models
{
    public class TweeterData
    {
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string nameUser { get; set; }

    }

    public static class TweeterDataLogin
        
    {
        public static string authorizationKey { get; set; }
        public static string authorizationSecret { get; set; }

    }
}