using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cv_Management.ViewModel.User
{
    public class ResponsesLoginViewModel
    {
        public  string AccessToken { get; set; }
        public  int LifeTime { get; set; }
        public  string Type { get; set; }
    }
}