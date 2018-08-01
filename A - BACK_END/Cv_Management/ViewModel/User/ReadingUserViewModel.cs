using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cv_Management.ViewModel.User
{
    public class ReadingUserViewModel
    {
        public ReadingUserViewModel()
        {
            
        }
        public ReadingUserViewModel(Models.Entities.User e )
        {
            this.Id = e.Id;
            this.FirstName = e.FirstName;
            this.LastName = e.LastName;
            this.Photo = e.Photo;
            this.Birthday = e.Birthday;
            this.Email = e.Email;
            this.Password = e.Password;
        }
        public int Id { get; set; }
        public  string Email { get; set; }

        public  string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Photo { get; set; }

        public double Birthday { get; set; }

        public string Role { get; set; }
    }
}