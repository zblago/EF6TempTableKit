using EF6TempTableKit.Attributes;
using EF6TempTableKit.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF6TempTableKit.Test.TempTables
{
    [Table("#tempProductCategoryCount", Schema = "tempDb")]
    public class ProductCategoryCountTempTable : ITempTable
    {
        [Key]
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
