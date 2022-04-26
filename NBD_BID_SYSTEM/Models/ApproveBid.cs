using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NBD_BID_SYSTEM.Models
{
    public class ApproveBid
    {
        public int ID { get; set; } //Primary key

        [Display(Name = "Bid Status")]
        public string Status { get; set; } //Status can be either accepted/approved, pending, rejected

        [Display (Name = "Bids")]
        public ICollection<Bid> Bids { get; set; }
    }
}
