using EF6TempTableKit.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF6TempTableKitNET8.Test.TempTables
{
    [Table("#tempAddress")]
    public class AddressTempTable : ITempTable
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
    }

    [NotMapped]
    public class AddressTempTableDto : AddressTempTable
    {
    }
}
