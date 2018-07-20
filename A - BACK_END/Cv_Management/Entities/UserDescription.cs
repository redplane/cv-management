using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cv_Management.Entities
{
    public class UserDescription
    {
        [Key]
        public  int Id { get; set; }
        public  int UserId { get; set; }
        public User User { get; set; }
        public  string Description { get; set; }
    }
}