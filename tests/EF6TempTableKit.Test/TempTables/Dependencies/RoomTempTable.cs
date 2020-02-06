using EF6TempTableKit.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF6TempTableKit.Test.TempTables.Dependencies
{
    [Table("#tempRoom")]
    public class RoomTempTable : ITempTable
    {
        [TempFieldTypeAttribute("int")]
        [Key]
        public int RoomId { get; set; }

        [TempFieldTypeAttribute("varchar(200)")]
        public string Name { get; set; }

        [TempFieldTypeAttribute("int")]
        public int ChairId { get; set; }
    }

    [NotMapped]
    public class RoomTempTableDto : RoomTempTable
    {
    }
}
