using EF6TempTableKit.DbContext;
using EF6TempTableKit.Edmx.Web.Models.TempTables;
using System.Data.Entity;

namespace EF6TempTableKit.Edmx.Web.Models
{
    [DbConfigurationType(typeof(DbConfig))]
    public partial class AdventureWorksEntities : IDbContextWithTempTable
    {
        public DbSet<AddressTempTable> AddressTempTable { get; set; }

        public TempTableContainer TempTableContainer { get; set; } = new TempTableContainer();
    }
}