using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NBD_BID_SYSTEM.Models
{
    public class Position
    {
        public int ID { get; set; }

        [Display(Name = "Position Description")]
        [MaxLength(30, ErrorMessage = "Position description cannot be more than 30 characters")]
        public string Description { get; set; }

        [Display(Name = "Staff")]
        public ICollection<Staff> Staffs { get; set; }

    }
}
