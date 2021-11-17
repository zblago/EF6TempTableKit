using EF6TempTableKit.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF6TempTableKit.Test.TempTables
{
    [Table("#tempAddress")]
    public class AddressTempTable : ITempTable
    {
        public string retStr()
        {
            return "";
        }

        [ClusteredIndex]
        [NonClusteredIndex("first")]
        [NonClusteredIndex("second")]
        [TempFieldTypeAttribute("int")]
        public int Id { get; set; }

        [NonClusteredIndex("third")]
        [NonClusteredIndex("second")]
        [TempFieldTypeAttribute("varchar(200)")]
        public string Name { get; set; }


        [NonClusteredIndex("third")]
        [NonClusteredIndex("second")]
        [TempFieldTypeAttribute("datetime")]
        public DateTime DateOfBirth { get; set; }

        [TempFieldTypeAttribute("bit")]
        [StringFormat("sss")]
        public bool IsActive { get; set; }
    }

    [NotMapped]
    public class AddressTempTableDto : AddressTempTable
    {
    }


    public class T
    {
        public T(Func<object> t) { }
    }
}
