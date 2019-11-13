namespace EF6TempTableKit.Test.Web.Context
{
    using EF6TempTableKit.Attributes;
    using EF6TempTableKit.DbContext;
    using EF6TempTableKit.Model;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;

    public class DbConfig : DbConfiguration
    {
        public DbConfig()
        {
            AddInterceptor(new AdventureWorkQueryInterceptor());
            AddInterceptor(new EF6TempTableKitQueryInterceptor());
        }
    }

    [DbConfigurationType(typeof(DbConfig))]
    public partial class AdventureWorksDW2008R2Entities : DbContext, IDbContextWithTempTable
    {

        public AdventureWorksDW2008R2Entities() : base("name=AdventureWorksDW2008R2Entities")
        {
            this.TempTableContainer = new TempTableContainer();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Ignore<TemporaryStudentIdentity>();
        }

        public virtual DbSet<DimReseller> DimReseller { get; set; }
        public virtual DbSet<DimTest> DimTest { get; set; }
        public virtual DbSet<TemporaryStudentIdentity> TemporaryStudents { get; set; }

        public TempTableContainer TempTableContainer { get; set; }
    }

    [Table("DimReseller")]
    public partial class DimReseller
    {
        [Key]
        public int ResellerKey { get; set; }
        public Nullable<int> GeographyKey { get; set; }
        public string ResellerAlternateKey { get; set; }
        public string Phone { get; set; }
        public string BusinessType { get; set; }
        public string ResellerName { get; set; }
        public Nullable<int> NumberEmployees { get; set; }
        public string OrderFrequency { get; set; }
        public Nullable<byte> OrderMonth { get; set; }
        public Nullable<int> FirstOrderYear { get; set; }
        public Nullable<int> LastOrderYear { get; set; }
        public string ProductLine { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public Nullable<decimal> AnnualSales { get; set; }
        public string BankName { get; set; }
        public Nullable<byte> MinPaymentType { get; set; }
        public Nullable<decimal> MinPaymentAmount { get; set; }
        public Nullable<decimal> AnnualRevenue { get; set; }
        public Nullable<int> YearOpened { get; set; }
    }

    [Table("DimTest")]
    public partial class DimTest
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [Table("#tempStudent", Schema = "tempDb")]
    public class TemporaryStudentIdentity : ITempTable
    {
        public TemporaryStudentIdentity()
        { }

        [TempFieldTypeAttribute("int")]
        [ClusteredIndex]
        [NonClusteredIndex("first")]
        [NonClusteredIndex("second")]
        public virtual int Id { get; set; }

        [TempFieldTypeAttribute("varchar(20)")]
        [NonClusteredIndex("third")]
        [NonClusteredIndex("second")]
        public virtual string Name { get; set; }
    }

    [NotMapped]
    public class TemporaryStudentIdentityDto : TemporaryStudentIdentity
    {
    }
}
