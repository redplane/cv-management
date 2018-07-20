using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cv_Management.ViewModel.Responsibility
{
    public class ReadingResponsibilityViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double CreatedTime { get; set; }
        public double? LastModifiedTime { get; set; }
    }
}