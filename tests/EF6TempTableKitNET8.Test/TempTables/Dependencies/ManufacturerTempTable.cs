using EF6TempTableKit.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF6TempTableKitNET8.Test.TempTables.Dependencies
{
    [Table("#tempManufacturer")]
    public class ManufacturerTempTable : ITempTable
    {
        [ClusteredIndex]
        [TempFieldTypeAttribute("int")]
        public int Id { get; set; }

        [TempFieldTypeAttribute("varchar(200)")]
        public string Name { get; set; }

        [TempFieldTypeAttribute("int")]
        public int AddressId { get; set; }
    }

    [NotMapped]
    public class ManufacturerTempTableDto : ManufacturerTempTable
    {
    }
}
