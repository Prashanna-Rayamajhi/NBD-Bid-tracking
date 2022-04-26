using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NBD_BID_SYSTEM.Models
{
    public class Project : IValidatableObject
    {
        public Project()
        {
            Bids = new HashSet<Bid>();
        }
        public int ID { get; set; }
        [Required (ErrorMessage = "You cannot leave Site empty")]
        [Display(Name = "Project Site")]
        [MaxLength(50, ErrorMessage = "Project site cannot be longer than 50 characters")]
        public string Site { get; set; }

        [Required (ErrorMessage = "You cannot leave Begin Date empty")]
        [Display(Name = "Begin Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyy-MM-dd}")]
        public DateTime BeginDate { get; set; }

        [Required (ErrorMessage = "You cannot leave end date empty")]
        [Display(Name = "Estimated Completion Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyy-MM-dd}")]
        public DateTime CompletionDate { get; set; }

        public ICollection<Bid> Bids { get; set; }

        //relation
        [Display(Name = " Client Info: ")]
        public int ClientID { get; set; }
        public Client Client { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(CompletionDate < DateTime.Today.Date)
            {
                yield return new ValidationResult("Completion Date must be in future or today", new[] { "CompletionDate" });
            }
        }
    }
}