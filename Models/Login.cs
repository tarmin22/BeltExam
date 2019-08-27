using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeltExam.Models
{
    public class Login
    {

        [Required(ErrorMessage = "Username is required.")]
        [MinLength(3)]
        [MaxLength(15)]
        [Display(Name = "Username")]
        public string LUsername { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string LPassword { get; set; }

    }
}