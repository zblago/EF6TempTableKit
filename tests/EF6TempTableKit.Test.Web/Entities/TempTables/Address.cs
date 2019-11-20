using EF6TempTableKit.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF6TempTableKit.Test.Web.Model.TempTables
{
    [Table("#tempAddress", Schema = "tempDb")]
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
        public string AddressLine1 { get; set; }
    }

    [NotMapped]
    public class AddressTempTableDto : AddressTempTable
    {
    }
}
