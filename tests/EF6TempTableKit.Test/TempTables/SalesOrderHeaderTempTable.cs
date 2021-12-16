using EF6TempTableKit.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF6TempTableKit.Test.TempTables
{
    [Table("#tempAddress")]
    public class SalesOrderHeaderTempTable : ITempTable
    {
        [TempFieldTypeAttribute("int")]
        public int SalesOrderID { get; set; }

        [TempFieldTypeAttribute("bigint")]
        public int SalesOrderBigIntID { get; set; }

        [TempFieldTypeAttribute("byte")]
        public byte RevisionNumber { get; set; }

        [TempFieldTypeAttribute("datetime")]
        public DateTime OrderDate { get; set; }

        [TempFieldTypeAttribute("datetime2")]
        public DateTime DueDate { get; set; }

        [TempFieldTypeAttribute("datetime2")]
        public DateTime? ShipDate { get; set; }

        [TempFieldTypeAttribute("byte")]
        public byte Status { get; set; }

        [TempFieldTypeAttribute("bit")]
        public bool OnlineOrderFlag { get; set; }

        [TempFieldTypeAttribute("varchar(100)")]
        public string SalesOrderNumber { get; set; }

        [TempFieldTypeAttribute("varchar(100)")]
        public string PurchaseOrderNumber { get; set; }

        [TempFieldTypeAttribute("varchar(100)")]
        public string AccountNumber { get; set; }

        [TempFieldTypeAttribute("int")]
        public int CustomerID { get; set; }

        [TempFieldTypeAttribute("int")]
        public int? SalesPersonID { get; set; }

        [TempFieldTypeAttribute("int")]
        public int? TerritoryID { get; set; }

        [TempFieldTypeAttribute("int")]
        public int BillToAddressID { get; set; }

        [TempFieldTypeAttribute("int")]
        public int ShipToAddressID { get; set; }

        [TempFieldTypeAttribute("int")]
        public int ShipMethodID { get; set; }

        [TempFieldTypeAttribute("int")]
        public int? CreditCardID { get; set; }

        [TempFieldTypeAttribute("string")]
        public string CreditCardApprovalCode { get; set; }

        [TempFieldTypeAttribute("int")]
        public int? CurrencyRateID { get; set; }

        [TempFieldTypeAttribute("money")]
        public decimal SubTotal { get; set; }

        [TempFieldTypeAttribute("money")]
        public decimal TaxAmt { get; set; }

        [TempFieldTypeAttribute("money")]
        public decimal Freight { get; set; }

        [TempFieldTypeAttribute("money")]
        public decimal TotalDue { get; set; }

        [TempFieldTypeAttribute("varchar(100)")]
        public string Comment { get; set; }

        [TempFieldTypeAttribute("varchar(100)")]
        public Guid rowguid { get; set; }

        [TempFieldTypeAttribute("datetime")]
        public DateTime ModifiedDate { get; set; }
    }

    [NotMapped]
    public class SalesOrderHeaderTempTableDto : SalesOrderHeaderTempTable
    {
    }
}
