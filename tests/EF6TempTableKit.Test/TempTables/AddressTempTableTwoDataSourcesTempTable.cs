using EF6TempTableKit.Attributes;
using EF6TempTableKit.Test.CustomConverters;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF6TempTableKit.Test.TempTables
{
    [Table("#tempAddressTempTableTwoDataSources")]
    public class AddressTempTableTwoDataSourcesTempTable : ITempTable
    {
        [ClusteredIndex]
        [NonClusteredIndex("first")]
        [NonClusteredIndex("second")]
        [TempFieldTypeAttribute("int")]
        public int Id { get; set; }

        [TempFieldTypeAttribute("int")]
        public int Id2 { get; set; }

        [NonClusteredIndex("third")]
        [NonClusteredIndex("second")]
        [TempFieldTypeAttribute("varchar(200)")]
        [CustomConverterAttribute(typeof(StringCustomConverter))]
        public string Name { get; set; }
    }

    [NotMapped]
    public class AddressTempTableTwoDataSourcesDto : AddressTempTableTwoDataSourcesTempTable
    {
    }
}
