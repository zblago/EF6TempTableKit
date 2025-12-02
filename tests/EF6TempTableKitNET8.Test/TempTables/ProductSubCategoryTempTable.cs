using EF6TempTableKit.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF6TempTableKitNET8.Test.TempTables
{
    [Table("#tempProductSubCategory")]
    public class ProductSubCategoryTempTable : ITempTable
    {
        //[Key] //As we have Id property, we DON'T have to mark primary key with [Key] attribute.
        [TempFieldTypeAttribute("int")]
        public int Id { get; set; }

        [TempFieldTypeAttribute("int")]
        public int CategoryId { get; set; }

        [TempFieldTypeAttribute("varchar(200)")]
        public string CategoryName { get; set; }
    }

    [NotMapped]
    public class ProductSubCategoryTempTableDto : ProductSubCategoryTempTable
    {
    }
}
