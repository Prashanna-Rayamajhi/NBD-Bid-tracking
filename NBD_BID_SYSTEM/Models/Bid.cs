using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NBD_BID_SYSTEM.Models
{
    public class Bid : IValidatableObject
    {
        public Bid()
        {
            BidLabors = new HashSet<BidLabor>();
            BidStaffs = new HashSet<BidStaff>();
            Materials = new HashSet<Material>();
        }
        public int ID { get; set; }

        [Required(ErrorMessage = "You cannot leave date field empty")]
        [Display(Name = "Bid Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "You cannot leave amount field empty")]
        [Display(Name = "Bid Amount")]
        [DataType(DataType.Currency)]
        
        public double Amount { get; set; }

        [Required(ErrorMessage = "You cannot leave Project Field empty")]
        [Display(Name = "Project")]
        public int ProjectID { get; set; }

        public Project Project { get; set; }

        [Display(Name = "Approve Status")]
        public int ApproveBidID { get; set; }
        public ApproveBid ApproveBid { get; set; }

        [Display(Name = "Bid Labors")]
        public ICollection<BidLabor> BidLabors { get; set; }

        [Display (Name = "Staff for bid")]
        public ICollection<BidStaff> BidStaffs { get; set; }

        public ICollection<Material> Materials { get; set; }

        

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
           if(Amount < 100)
            {
                yield return new ValidationResult("Amonut must be minimum of $100", new[] { "Amount" });
            }
        }
    }
}
