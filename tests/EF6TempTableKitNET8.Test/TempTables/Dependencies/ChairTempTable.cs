using EF6TempTableKit.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF6TempTableKitNET8.Test.TempTables.Dependencies
{
    [Table("#tempChair")]
    public class ChairTempTable : ITempTable
    {
        [ClusteredIndex]
        [NonClusteredIndex("first")]
        [NonClusteredIndex("second")]
        [TempFieldTypeAttribute("int")]
        [Key]
        public int ChairId { get; set; }

        [NonClusteredIndex("third")]
        [NonClusteredIndex("second")]
        [TempFieldTypeAttribute("varchar(200)")]
        public string Name { get; set; }

        [TempFieldTypeAttribute("int")]
        public int PartId { get; set; }
    }

    [NotMapped]
    public class ChairTempTableDto : ChairTempTable
    {
    }
}
