using EF6TempTableKit.Attributes;
using EF6TempTableKit.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF6TempTableKit.Test.TempTables
{
    [Table("#tempAddressMultipleId", Schema = "tempDb")]
    public class AddressTempTableMultipleId : ITempTable
    {
        [ClusteredIndex]
        [NonClusteredIndex("first")]
        [NonClusteredIndex("second")]
        [TempFieldTypeAttribute("int")]
        public virtual int Id { get; set; }

        [TempFieldTypeAttribute("int")]
        public virtual int Id2 { get; set; }

        [NonClusteredIndex("third")]
        [NonClusteredIndex("second")]
        [TempFieldTypeAttribute("varchar(200)")]
        public virtual string Name { get; set; }
    }

    [NotMapped]
    public class AddressTempTableMultipleIdDto : AddressTempTableMultipleId
    {
    }
}
