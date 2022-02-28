using System.ComponentModel.DataAnnotations;
namespace CovidManagementSystem.Entities
{
    public class NewPatientVisitedSite
    {
        [Required]
        public DateTime dateOfVisit { get; set; }
        [Required]
        public string siteName { get; set; }
        
        public Address siteAddress { get; set; }
        public NewPatientVisitedSite() { }
        public NewPatientVisitedSite(DateTime dateOfVisit,string siteName,string city, string street,int houseNumber, int apartmentNumber)
        {
            this.dateOfVisit = dateOfVisit;
            this.siteName = siteName;
            this.siteAddress = new Address(city, street, houseNumber, apartmentNumber);

        }

    }
}
