using EF6TempTableKit.Attributes;
using EF6TempTableKit.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF6TempTableKit.Test.TempTables
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
        [TempFieldTypeAttribute("varchar(200)")]
        public virtual string Name { get; set; }
    }

    [NotMapped]
    public class AddressTempTableDto : AddressTempTable
    {
    }
}
