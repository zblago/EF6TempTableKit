using EF6TempTableKit.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF6TempTableKitNET8.Test.TempTables.Dependencies
{
    [Table("#tempOffice")]
    public class OfficeTempTable : ITempTable
    {
        [ClusteredIndex]
        [NonClusteredIndex("first")]
        [NonClusteredIndex("second")]
        [TempFieldTypeAttribute("int")]
        public int Id { get; set; }

        [NonClusteredIndex("third")]
        [NonClusteredIndex("second")]
        [TempFieldTypeAttribute("varchar(200)")]
        public string Name { get; set; }

        [TempFieldTypeAttribute("int")]
        public int DepartmentId { get; set; }

        [TempFieldTypeAttribute("int")]
        public int RoomId { get; set; }

        [TempFieldTypeAttribute("int")]
        public int OfficeTypeId { get; set; }
    }

    [NotMapped]
    public class OfficeTempTableDto : OfficeTempTable
    {
    }
}
