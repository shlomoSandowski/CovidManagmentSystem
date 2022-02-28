using System.ComponentModel.DataAnnotations;
namespace CovidManagementSystem.Entities
{
    public class Address
    {
        [Required]
        public string city { get; set; }
        [Required]
        public string street { get; set; }
        [Required]
        public int houseNumber { get; set; }
        public int apartmentNumber { get; set; }

        public Address(string city,string street, int houseNumber, int apartmentNumber=-1)
        {
            
            this.city = city;
            this.street = street;
            this.houseNumber = houseNumber;
            this.apartmentNumber = apartmentNumber;
        }

    }
}
