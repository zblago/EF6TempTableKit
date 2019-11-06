using EF6TempTableKit.Attributes;
using EF6TempTableKit.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF6TempTableKit.Edmx.Web.Models.TempTables
{
    [Table("#tempAddress", Schema = "tempDb")]
    public class AddressTempTable : ITempTable
    {
        [ClusteredIndex]
        [NonClusteredIndex("first")]
        [NonClusteredIndex("second")]
        [TempFieldTypeAttribute("int")]
        public virtual int Id { get; set; }

        [NonClusteredIndex("third")]
        [NonClusteredIndex("second")]
        [TempFieldTypeAttribute("varchar(20)")]
        public virtual string Name { get; set; }
    }

    [NotMapped]
    public class AddressDto : AddressTempTable
    {
    }
}