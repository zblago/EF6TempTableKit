using EF6TempTableKit.DbContext;
using EF6TempTableKit.Test.Web.Model.TempTables;
using System.Data.Entity;

namespace EF6TempTableKit.Test.Web.Models
{
    [DbConfigurationType(typeof(DbConfig))]
    public partial class AdventureWorks : IDbContextWithTempTable
    {
        public DbSet<AddressTempTable> AddressesTempTable { get; set; }

        public TempTableContainer TempTableContainer { get; set; } = new TempTableContainer();
    }
}