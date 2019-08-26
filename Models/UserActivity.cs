using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace BeltExam.Models
{
    public class UserActivity
    {
        // auto-implemented properties need to match the columns in your table
        // the [Key] attribute is used to mark the Model property being used for your table's Primary Key
        [Key]
        public int UserActivityId { get; set; }
        public int ActivityId { get; set; }
        // MySQL VARCHAR and TEXT types can be represeted by a string
        public int UserId { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public Activity AcitvityCreated { get; set; }
        public User ActivityParticipant { get; set; }



    }
}