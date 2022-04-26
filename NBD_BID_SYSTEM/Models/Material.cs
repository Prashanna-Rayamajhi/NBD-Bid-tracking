using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NBD_BID_SYSTEM.Models
{
    public class Material
    {
        public int ID { get; set; }

        [Display(Name = "Price")]
        [Required(ErrorMessage = "Price Cannot be empty")]
        public double Price { get; set; }

        [Display(Name = "Quantity")]
        [Required(ErrorMessage ="Quantity cannot be 0 or less than zero")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "You must select the bid")]
        [Display(Name = "Bid")]
        public int BidID { get; set; }
        public Bid Bid { get; set; }

        [Display(Name = "Inventory")]
        [Required(ErrorMessage ="You must select the inventory")]
        public int InventoryID { get; set; }
        public Inventory Inventory { get; set; }
    }
}
