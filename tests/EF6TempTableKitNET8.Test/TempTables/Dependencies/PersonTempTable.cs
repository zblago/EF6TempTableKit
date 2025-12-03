using EF6TempTableKit.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF6TempTableKitNET8.Test.TempTables.Dependencies
{
    [Table("#tempPerson")]
    public class PersonTempTable : ITempTable
    {
        [Key]
        [TempFieldTypeAttribute("int")]
        public int PersonId { get; set; }

        [TempFieldTypeAttribute("varchar(200)")]
        public string Name { get; set; }

        [TempFieldTypeAttribute("int")]
        public int AddressId { get; set; }
    }

    [NotMapped]
    public class PersonTempTableDto : PersonTempTable
    {
    }
}
