using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NBD_BID_SYSTEM.Models
{
    public class InventoryType
    {
        public InventoryType()
        {
            Inventories = new HashSet<Inventory>();
        }
        public int ID { get; set; }

        [Required]
        [Display(Name ="Type of Item")]
        public string DescOfType { get; set; }

        public ICollection<Inventory> Inventories { get; set; }
    }
}
