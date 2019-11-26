using EF6TempTableKit.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF6TempTableKit.Test.TempTables
{
    [Table("#tempProductCategory")]
    public class ProductCategoryTempTable : ITempTable
    {
        //[Key] //As we have Id property, we DON'T have to mark primary key with [Key] attribute.
        [NonClusteredIndex("CategoryId_CategoryName")]
        [TempFieldTypeAttribute("int")]
        public int Id { get; set; }

        [NonClusteredIndex("CategoryId_CategoryName")]
        [TempFieldTypeAttribute("varchar(100)")]
        public string CategoryName { get; set; }
    }

    [NotMapped]
    public class ProductCategoryTempTableDto : ProductCategoryTempTable
    {
    }
}
