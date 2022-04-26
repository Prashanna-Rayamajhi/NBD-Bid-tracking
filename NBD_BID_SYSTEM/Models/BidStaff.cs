using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NBD_BID_SYSTEM.Models
{
    public class BidStaff
    {
        public int ID { get; set; } //Primary key for m:m relationship for bid and the staff table
        [Required]
        public int BidID { get; set; }
        public Bid Bid { get; set; }
        [Required]
        public int StaffID { get; set; }
        public Staff Staff { get; set; }
    }
}
