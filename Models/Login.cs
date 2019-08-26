using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeltExam.Models
{
    public class Login
    {

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string LEmail { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string LPassword { get; set; }

    }
}