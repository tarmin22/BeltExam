using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace BeltExam.Models
{
    public class Activity
    {
        // auto-implemented properties need to match the columns in your table
        // the [Key] attribute is used to mark the Model property being used for your table's Primary Key
        [Key]
        public int ActivityId { get; set; }
        // MySQL VARCHAR and TEXT types can be represeted by a string
        public int UserId { get; set; }
        [Required]
        [MinLength(2)]
        public string Title { get; set; }
        public string Time { get; set; }
        public DateTime Date { get; set; }


        public int Duration { get; set; }
        public string DurUnit { get; set; }


        [Required]
        [MinLength(5)]
        public string Description { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public User Createdby { get; set; }
        public List<UserActivity> ActivityUser { get; set; }


    }
}