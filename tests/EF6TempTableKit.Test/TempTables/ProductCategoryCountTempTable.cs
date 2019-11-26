using EF6TempTableKit.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF6TempTableKit.Test.TempTables
{
    [Table("#tempProductCategoryCount")]
    public class ProductCategoryCountTempTable : ITempTable
    {
        [Key] //As we don't have Id property, we have to mark primary key with [Key] attribute.
        [ClusteredIndex]
        [NonClusteredIndex("CategoryId_CategoryName")]
        [TempFieldTypeAttribute("int")]
        public int CategoryId { get; set; }

        [NonClusteredIndex("CategoryId_CategoryName")]
        [TempFieldTypeAttribute("varchar(100)")]
        public string CategoryName { get; set; }

        [TempFieldTypeAttribute("int")]
        public int ProductCount { get; set; }
    }

    [NotMapped]
    public class ProductCategoryCountTempTableDto : ProductCategoryCountTempTable
    {
    }
}
