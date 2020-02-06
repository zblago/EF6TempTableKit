using EF6TempTableKit.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF6TempTableKit.Test.TempTables.Dependencies
{
    [Table("#tempOfficeType")]
    public class OfficeTypeTempTable : ITempTable
    {
        [TempFieldTypeAttribute("int")]
        public int Id { get; set; }

        [TempFieldTypeAttribute("varchar(200)")]
        public string Name { get; set; }
    }

    [NotMapped]
    public class OfficeTypeTempTableDto : OfficeTypeTempTable
    {
    }
}
