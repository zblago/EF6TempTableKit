using EF6TempTableKit.Attributes;
using EF6TempTableKit.Test.CustomFormatter;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF6TempTableKit.Test.TempTables
{
    [Table("#tempAddress")]
    public class AddressTempTable : ITempTable
    {
        [ClusteredIndex]
        [NonClusteredIndex("first")]
        [NonClusteredIndex("second")]
        [TempFieldTypeAttribute("int")]
        [FuncFormatAttribute(typeof(StringCustomFormatter))]
        public int Id { get; set; }

        [NonClusteredIndex("third")]
        [NonClusteredIndex("second")]
        [TempFieldTypeAttribute("varchar(200)")]
        [FuncFormatAttribute(typeof(StringCustomFormatter))]
        public string Name { get; set; }


        [NonClusteredIndex("third")]
        [NonClusteredIndex("second")]
        [TempFieldTypeAttribute("datetime")]
        public DateTime DateOfBirth { get; set; }

        [TempFieldTypeAttribute("bit")]
        public bool IsActive { get; set; }
    }

    [NotMapped]
    public class AddressTempTableDto : AddressTempTable
    {
    }
}
