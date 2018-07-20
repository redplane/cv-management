using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cv_Management.ViewModel.User;

namespace Cv_Management.ViewModel.UserDescription
{
    public class ReadingUserDescriptionViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public  ReadingUserViewModel User { get; set; }
        public string Description { get; set; }
    }
}