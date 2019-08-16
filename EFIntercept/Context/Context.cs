namespace EFIntercept.Context
{
    using SharDev.EFInterceptor.DbContext;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;


    public partial class AdventureWorksDW2008R2Entities : DbContextInterceptor
    {

        public AdventureWorksDW2008R2Entities() : base("name=AdventureWorksDW2008R2Entities")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //throw new UnintentionalCodeFirstException();
        }

        public virtual DbSet<DimReseller> DimReseller { get; set; }
        public virtual DbSet<TemporaryStudentIdentity> TemporaryStudents { get; set; }
    }

    [System.ComponentModel.DataAnnotations.Schema.Table("DimReseller")]
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

    [System.ComponentModel.DataAnnotations.Schema.Table("#tempStudent", Schema = "tempDb")]
    public class TemporaryStudentIdentity
    {
        public TemporaryStudentIdentity()
        { }

        [Key]
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
    }

}
