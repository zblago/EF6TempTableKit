using EF6TempTableKit.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF6TempTableKit.Test.TempTables
{
    [Table("#tempCustomer")]
    public class CustomerTempTable : ITempTable
    {
        [ClusteredIndex]
        [NonClusteredIndex("first")]
        [NonClusteredIndex("second")]
        [TempFieldTypeAttribute("int")]
        public int Id { get; set; }
    }

    [NotMapped]
    public class CustomerTempTableDto : CustomerTempTable
    {
    }
}
