using MaintenanceCallSystem6.Enums;
using System.ComponentModel.DataAnnotations;

namespace MaintenanceCallSystem6.Models
{
    public class MaintenanceCall
    {
        public int Id { get; set; }

        //[Required(ErrorMessage = "The Description field is required.")]
        public string? Description { get; set; }

        //[Required(ErrorMessage = "The FirstName field is required.")]
        public string? FirstName { get; set; }

        //[Required(ErrorMessage = "The LastName field is required.")]
        public string? LastName { get; set; }

        //[Required(ErrorMessage = "The DateLogged field is required.")]
        [DataType(DataType.Date)]
        public DateTime? DateLogged { get; set; }

        public string? Status { get; set; }
        public string? TechnicianAssigned { get; set; }
        public DateTime? DateAttended { get; set; }


        //me      now

        public string? Address { get; set; } 
        public string? City { get; set; } 
        public string? Province { get; set; }
        public string? PostalCode { get; set; }

        // Foreign key for User

        //public string UserId { get; set; }

        //// Navigation property
        //public User User { get; set; }

        public string? IssueTypeEnum  { get; set; } // Use the enum here
    }

}
