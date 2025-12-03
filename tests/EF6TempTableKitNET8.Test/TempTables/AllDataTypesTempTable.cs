using EF6TempTableKit.Attributes;
using EF6TempTableKitNET8.Test.CustomConverters;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF6TempTableKitNET8.Test.TempTables
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
    /// </summary>
    [Table("#tempAllDataTypes")]
    public class AllDataTypesTempTable : ITempTable
    {
        [Key] //As we don't have Id property, we have to mark primary key with [Key] attribute.
        [TempFieldTypeAttribute("bigint")]
        public Int64 Bigint { get; set; }

        [TempFieldTypeAttribute("binary(2)")]
        [CustomConverterAttribute(typeof(BitCustomConverter))]
        public byte[] Binary { get; set; }

        [TempFieldTypeAttribute("bit")]
        public bool Bit { get; set; }

        [TempFieldTypeAttribute("date")]
        public DateTime Date { get; set; }

        [TempFieldTypeAttribute("datetime")]
        public DateTime Datetime { get; set; }

        [TempFieldTypeAttribute("datetime2")]
        [StringConverter("'{0:yyyy-MM-dd HH:mm:ss.fffffff}'")]
        public DateTime Datetime2 { get; set; }

        [TempFieldTypeAttribute("datetimeoffset")]
        [StringConverter("'{0:yyyy-MM-dd HH:mm:ss.fff}'")]
        public DateTimeOffset Datetimeoffset { get; set; }

        [TempFieldTypeAttribute("decimal(38, 0)")]
        public decimal Decimal { get; set; }
        
        [TempFieldTypeAttribute("varbinary(max)")]
        [CustomConverterAttribute(typeof(BitCustomConverter))]
        public byte[] Varbinary_Max { get; set; }

        [TempFieldTypeAttribute("float(53)")]
        [StringConverterAttribute("{0:r}")] //Because ToString() is rounding double.MaxValue (https://stackoverflow.com/questions/40562199/double-maxvalue-to-string-is-unconverted-back)
        public double Float { get; set; }

        [TempFieldTypeAttribute("image")]
        [CustomConverterAttribute(typeof(BitCustomConverter))]
        public byte[] Image { get; set; }

        [TempFieldTypeAttribute("int")]
        public Int32 Int { get; set; }

        [TempFieldTypeAttribute("nchar(15)")]
        public String Nchar { get; set; }

        [TempFieldTypeAttribute("ntext")]
        public String Ntext { get; set; }

        [TempFieldTypeAttribute("numeric(38, 0)")]
        public Decimal Numeric { get; set; }

        [TempFieldTypeAttribute("nvarchar(15)")]
        public String Nvarchar { get; set; }

        [TempFieldTypeAttribute("real")]
        [StringConverterAttribute("{0:r}")] //Because ToString() is rounding single.MaxValue (https://stackoverflow.com/questions/40562199/double-maxvalue-to-string-is-unconverted-back)
        public Single Real { get; set; }

        //[TempFieldTypeAttribute("rowversion")]
        //public byte[] Rowversion { get; set; } //Automatically generated value, can't be inserted, only read.

        [TempFieldTypeAttribute("smalldatetime")]
        [StringConverter("'{0:yyyy-MM-dd HH:mm:ss}'")]
        public DateTime Smalldatetime { get; set; }

        [TempFieldTypeAttribute("smallint")]
        public Int16 Smallint { get; set; }

        [TempFieldTypeAttribute("smallmoney")]
        public Decimal Smallmoney { get; set; }

        [TempFieldTypeAttribute("text")]
        public String Text { get; set; }

        [TempFieldTypeAttribute("Time")]
        [StringConverter("'{0:hh\\:mm\\:ss\\.fff}'")]
        public TimeSpan Time { get; set; }

        //[TempFieldTypeAttribute("timestamp")]
        //public byte[] Timestamp { get; set; } //Automatically generated value, can't be inserted, only read.

        [TempFieldTypeAttribute("tinyint")]
        public byte Tinyint { get; set; }

        [TempFieldTypeAttribute("uniqueidentifier")]
        [StringConverter("'{0}'")]
        public Guid Uniqueidentifier { get; set; }

        [TempFieldTypeAttribute("varbinary(4)")]
        [CustomConverterAttribute(typeof(BitCustomConverter))]
        public byte[] Varbinary { get; set; }

        [TempFieldTypeAttribute("varchar(50)")]
        public String Varchar_50 { get; set; }
    }

    [NotMapped]
    public class AllDataTypesDto : AllDataTypesTempTable
    {
    }
}
