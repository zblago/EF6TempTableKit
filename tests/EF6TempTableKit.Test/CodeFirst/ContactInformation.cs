using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF6TempTableKit.Test.CodeFirst
{
    public class ContactInformation
    {
        [Key]
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JobTitle { get; set; }
        public string BusinessEntityType { get; set; }
    }
}
