using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NBD_BID_SYSTEM.Models
{
    public class BidLabor
    {
        public int ID { get; set; }
        [Display(Name = "Working Hours")]
        [Required(ErrorMessage = "You cannot leave working hour field empty")]
        public double HoursWorked { get; set; }
        [Display(Name = "Labour Extended Price")]
        [Required(ErrorMessage = "You cannot leave Extended Price  field empty")]
        public double ExtPrice { get; set; }

        [Display(Name = "Labour")]
        [Required(ErrorMessage = "You cannot leave Labour  field empty")]
        public int LaborID { get; set; }
        public Labor Labor { get; set; }

        [Display(Name = "Bid")]
        [Required(ErrorMessage = "You cannot leave Bid empty")]
        public int BidID { get; set; }
        public Bid Bid { get; set; }
    }
}
