using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NBD_BID_SYSTEM.Models
{
    public class Labor
    {
        public Labor()
        {
            BidLabors = new HashSet<BidLabor>();
        }
        public int ID { get; set; }

        [Display(Name = "Labour Type")]
        [Required(ErrorMessage = "You cannot leave labour type field empty")]
        [MaxLength(50, ErrorMessage = "It can be upto 50 character long only")]
        public string Type { get; set; }
        [Display(Name = "Price")]
        [Required(ErrorMessage = "You cannot leave Price field empty")]
        public double Price { get; set; }

        [Display(Name = "Cost")]
        [Required(ErrorMessage = "You cannot leave Cost field empty")]
        public double Cost { get; set; }

        public ICollection<BidLabor> BidLabors { get; set; }
    }
}
