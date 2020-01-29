using EF6TempTableKit.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF6TempTableKit.Test.TempTables.Dependencies
{
    [Table("#tempDepartment")]
    public class DepartmentTempTable : ITempTable
    {
        [Key]
        public int DepartmentId { get; set; }

        [TempFieldTypeAttribute("varchar(200)")]
        public string Name { get; set; }

        [TempFieldTypeAttribute("int")]
        public int LeadId { get; set; }
    }

    [NotMapped]
    public class DepartmentTempTableDto : DepartmentTempTable
    {
    }
}
