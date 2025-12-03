using EF6TempTableKit.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF6TempTableKitNET8.Test.TempTables.Dependencies
{
    [Table("#tempPartType")]
    public class PartTypeTempTable : ITempTable
    {
        [ClusteredIndex]
        [NonClusteredIndex("first")]
        [NonClusteredIndex("second")]
        [TempFieldTypeAttribute("int")]
        [Key]
        public int PartTypeId { get; set; }

        [NonClusteredIndex("third")]
        [NonClusteredIndex("second")]
        [TempFieldTypeAttribute("varchar(200)")]
        public string Name { get; set; }
    }

    [NotMapped]
    public class PartTypeTempTableDto : PartTypeTempTable
    {
    }
}
