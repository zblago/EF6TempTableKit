using EF6TempTableKit.Exceptions;
using EF6TempTableKit.Extensions;
using EF6TempTableKit.Test.CodeFirst;
using EF6TempTableKit.Test.TempTables;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace EF6TempTableKit.Test
{
    public class UnitTestMemoryData
    {
        private const decimal SMALL_MONEY_MAX = 214748.3647M;

        private IList<string> _addressList = new List<string>{ "1970 Napa Ct.", "9833 Mt. Dias Blv.", "7484 Roundtree Drive" };

        [Fact]
        public void ThrowExceptionIfMoreThanOneFormmaterIsSpecified()
        {
            Assert.Throws<EF6TempTableKitGenericException>(() =>
            {
                using (var context = new AdventureWorksCodeFirst())
                {
                    var addressList = _addressList.Select(x => new AddressTempTableWrongAttributeDto
                    {
                        Name = x
                    });

                    var data = context
                            .WithTempTableExpression<AdventureWorksCodeFirst>(addressList)
                            .TempAddresses.Join(context.Addresses,
                            (a) => a.Id,
                            (aa) => aa.AddressID,
                            (at, a) => new { Id = at.Id }).ToList();
                }
            });
        }

        [Fact]
        public void ThrowExceptionWrongFormmaterSignatureIsSpecified()
        {
            Assert.Throws<EF6TempTableKitGenericException>(() =>
            {
                using (var context = new AdventureWorksCodeFirst())
                {
                    var addressList = _addressList.Select(x => new AddressTempTableWrongAttribute1Dto
                    {
                        Name = x
                    });

                    var data = context
                            .WithTempTableExpression<AdventureWorksCodeFirst>(addressList)
                            .TempAddresses.Join(context.Addresses,
                            (a) => a.Id,
                            (aa) => aa.AddressID,
                            (at, a) => new { Id = at.Id }).ToList();
                }
            });
        }

        [Fact]
        public void LoadFromMemoryAndDatabase()
        {
            using (var context = new AdventureWorksCodeFirst())
            {
                var maxId = context.Addresses.Max(x => x.AddressID);
                var totalAddressesInDb = context.Addresses.Count();
                var addressesInMemory = _addressList.Select((x, i) => new AddressTempTableTwoDataSourcesDto
                {
                    Id = maxId + i + 1,
                    Name = x
                });

                var tempAddressQuery = context.Addresses.Select(a => new AddressTempTableTwoDataSourcesDto {
                    Id = a.AddressID, 
                    Name = a.AddressLine1 
                });

                var totalCount = context
                        .WithTempTableExpression<AdventureWorksCodeFirst>(addressesInMemory)
                        .WithTempTableExpression<AdventureWorksCodeFirst>(tempAddressQuery)
                        .TempAddressesTwoDataSources.Count();

                Assert.Equal(addressesInMemory.Count() + totalAddressesInDb, totalCount);
            }
        }

        [Fact]
        public void LoadFromMemoryAndDatabaseAppendData()
        {
            using (var context = new AdventureWorksCodeFirst())
            {
                var maxId = context.Addresses.Max(x => x.AddressID);
                var totalAddressesInDb = context.Addresses.Count();
                var addressesInMemory = _addressList.Select((x, i) => new AddressTempTableTwoDataSourcesDto
                {
                    Id = maxId + i + 1,
                    Name = x
                });
                var tempAddressQuery = context.Addresses.Select(a => new AddressTempTableTwoDataSourcesDto
                {
                    Id = a.AddressID,
                    Name = a.AddressLine1
                });

                var totalCount = context
                        .WithTempTableExpression<AdventureWorksCodeFirst>(addressesInMemory)
                        .WithTempTableExpression<AdventureWorksCodeFirst>(tempAddressQuery)
                        .TempAddressesTwoDataSources.Count();
                Assert.Equal(addressesInMemory.Count() + totalAddressesInDb, totalCount);

                var updatedAddressList = new List<string>
                {
                    "8157 W. Book",
                    "6696 Anchor Drive",
                    "6872 Thornwood Dr.",
                    "636 Vine Hill Way",
                    "7484 Roundtree Drive"
                };
                
                var totalCountQuery = context.WithTempTableExpression<AdventureWorksCodeFirst>(updatedAddressList.Select((x, i) => new AddressTempTableTwoDataSourcesDto
                {
                    Id = maxId + i + 1 + addressesInMemory.Count(),
                    Name = x
                }));

                totalCount = totalCountQuery.TempAddressesTwoDataSources.Count();
                Assert.Equal(addressesInMemory.Count() + totalAddressesInDb + updatedAddressList.Count(), totalCount);
            }
        }

        /// <summary>
        /// EF6TempTableKit ensures that each data type is properly converted to the corresponding SQL Server data type (date to date, int to int, ....).
        /// https://docs.microsoft.com/en-us/sql/connect/ado-net/sql-server-data-type-mappings?view=sql-server-ver15.
        /// But there are some exceptions when you convert certain types of data yourself.
        /// Most of the needed converters are here in the UnitTest project.
        /// Take this as an example and change as per your needs.
        /// </summary>
        [Fact]
        public void MapNetToSqlDataTypes()
        {
            using (var context = new AdventureWorksCodeFirst())
            {
                var allDataTypesList = new List<AllDataTypesDto> 
                { 
                    new AllDataTypesDto
                    { 
                        Bigint = Int64.MaxValue,
                        Binary = new byte[] { 0x45, 0x46 },
                        Bit = true,
                        Date = DateTime.MaxValue.Date,
                        Datetime = DateTime.MaxValue.AddMilliseconds(-2), //Time range:	00:00:00 through 23:59:59.997 https://docs.microsoft.com/en-us/sql/t-sql/data-types/datetime-transact-sql?view=sql-server-ver15
                        Datetime2 = DateTime.MaxValue,
                        Datetimeoffset = DateTimeOffset.UtcNow.Date,
                        Decimal = Decimal.MaxValue,
                        Varbinary_Max = new byte[] { 0x4B, 0x49, 0x54, 0x41 },
                        Float = double.MaxValue,
                        Image = new byte[] { 0x45, 0x46 },
                        Int = int.MaxValue,
                        Nchar = "Ef6TempTableKit",
                        Ntext = "Ef6TempTableKit",
                        Numeric = Decimal.MaxValue,
                        Nvarchar = "Ef6TempTableKit",
                        Real = Single.MaxValue,
                        Smalldatetime = new DateTime(2079, 6, 5, 23, 59, 0),
                        Smallint = Int16.MaxValue,
                        Smallmoney = SMALL_MONEY_MAX,
                        Text = "Ef6TempTableKit",
                        Time = new TimeSpan(0, 4, 54, 56, 234),
                        Tinyint = byte.MaxValue,
                        Uniqueidentifier = Guid.NewGuid(),
                        Varbinary = new byte[] { 0x4B, 0x49, 0x54, 0x41 },
                        Varchar_50 = "wqS5LQa67cxMReRRFHC5CKptEnCVqieB04mOXbBl5ahk0M3S8j"
                    }
                };

                var allDataTypeItemFromDb = context
                        .WithTempTableExpression<AdventureWorksCodeFirst>(allDataTypesList)
                        .AllDataTypesTempTable.First();

                var allDataTypeItemFromMemory = allDataTypesList.First();

                Assert.Equal(allDataTypeItemFromDb.Bigint, allDataTypeItemFromMemory.Bigint);
                Assert.Equal(allDataTypeItemFromDb.Binary, allDataTypeItemFromMemory.Binary);
                Assert.Equal(allDataTypeItemFromDb.Bit, allDataTypeItemFromMemory.Bit);
                Assert.Equal(allDataTypeItemFromDb.Date, allDataTypeItemFromMemory.Date);
                Assert.Equal(allDataTypeItemFromDb.Datetime.ToString("yyyy-MM-dd HH:mm:ss.fff"), allDataTypeItemFromMemory.Datetime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                Assert.Equal(allDataTypeItemFromDb.Datetime2.ToString("yyyy-MM-dd HH:mm:ss.fffffff"), allDataTypeItemFromMemory.Datetime2.ToString("yyyy-MM-dd HH:mm:ss.fffffff"));
                Assert.Equal(allDataTypeItemFromDb.Datetimeoffset.Date, allDataTypeItemFromMemory.Datetimeoffset.Date);
                Assert.Equal(allDataTypeItemFromDb.Decimal, allDataTypeItemFromMemory.Decimal);
                Assert.Equal(allDataTypeItemFromDb.Varbinary_Max, allDataTypeItemFromMemory.Varbinary_Max);
                Assert.Equal(allDataTypeItemFromDb.Float, allDataTypeItemFromMemory.Float);
                Assert.Equal(allDataTypeItemFromDb.Image, allDataTypeItemFromMemory.Image);
                Assert.Equal(allDataTypeItemFromDb.Int, allDataTypeItemFromMemory.Int);
                Assert.Equal(allDataTypeItemFromDb.Nchar, allDataTypeItemFromMemory.Nchar);
                Assert.Equal(allDataTypeItemFromDb.Ntext, allDataTypeItemFromMemory.Ntext);
                Assert.Equal(allDataTypeItemFromDb.Numeric, allDataTypeItemFromMemory.Numeric);
                Assert.Equal(allDataTypeItemFromDb.Nvarchar, allDataTypeItemFromMemory.Nvarchar);
                Assert.Equal(allDataTypeItemFromDb.Real, allDataTypeItemFromMemory.Real);
                Assert.Equal(allDataTypeItemFromDb.Smalldatetime.ToString("yyyy-MM-dd HH:mm:ss"), allDataTypeItemFromMemory.Smalldatetime.ToString("yyyy-MM-dd HH:mm:ss"));
                Assert.Equal(allDataTypeItemFromDb.Smallint, allDataTypeItemFromMemory.Smallint);
                Assert.Equal(allDataTypeItemFromDb.Smallmoney, allDataTypeItemFromMemory.Smallmoney);
                Assert.Equal(allDataTypeItemFromDb.Text, allDataTypeItemFromMemory.Text);
                Assert.Equal(allDataTypeItemFromDb.Time, allDataTypeItemFromMemory.Time);
                Assert.Equal(allDataTypeItemFromDb.Tinyint, allDataTypeItemFromMemory.Tinyint);
                Assert.Equal(allDataTypeItemFromDb.Uniqueidentifier, allDataTypeItemFromMemory.Uniqueidentifier);
                Assert.Equal(allDataTypeItemFromDb.Varbinary, allDataTypeItemFromMemory.Varbinary);
                Assert.Equal(allDataTypeItemFromDb.Varchar_50, allDataTypeItemFromMemory.Varchar_50);
            }
        }

        [Fact]
        public void Load1000RecordsFromMemory()
        {
            var sampleList = new List<AllDataTypesDto>();
            for (var i = 0; i < 1000; i++) 
            {
                sampleList.Add(new AllDataTypesDto
                {
                    Bigint = Int64.MaxValue,
                    Binary = new byte[] { 0x45, 0x46 },
                    Bit = true,
                    Date = DateTime.MaxValue.Date,
                    Datetime = DateTime.MaxValue.AddMilliseconds(-2), //Time range:	00:00:00 through 23:59:59.997 https://docs.microsoft.com/en-us/sql/t-sql/data-types/datetime-transact-sql?view=sql-server-ver15
                    Datetime2 = DateTime.MaxValue,
                    Datetimeoffset = DateTimeOffset.UtcNow.Date,
                    Decimal = Decimal.MaxValue,
                    Varbinary_Max = new byte[] { 0x4B, 0x49, 0x54, 0x41 },
                    Float = double.MaxValue,
                    Image = new byte[] { 0x45, 0x46 },
                    Int = int.MaxValue,
                    Nchar = "Ef6TempTableKit",
                    Ntext = "Ef6TempTableKit",
                    Numeric = Decimal.MaxValue,
                    Nvarchar = "Ef6TempTableKit",
                    Real = Single.MaxValue,
                    Smalldatetime = new DateTime(2079, 6, 5, 23, 59, 0),
                    Smallint = Int16.MaxValue,
                    Smallmoney = SMALL_MONEY_MAX,
                    Text = "Ef6TempTableKit",
                    Time = new TimeSpan(0, 4, 54, 56, 234),
                    Tinyint = byte.MaxValue,
                    Uniqueidentifier = Guid.NewGuid(),
                    Varbinary = new byte[] { 0x4B, 0x49, 0x54, 0x41 },
                    Varchar_50 = "wqS5LQa67cxMReRRFHC5CKptEnCVqieB04mOXbBl5ahk0M3S8j"
                });
            }

            using (var context = new AdventureWorksCodeFirst())
            {
                var allDataFromDb = context.WithTempTableExpression<AdventureWorksCodeFirst>(sampleList).AllDataTypesTempTable.ToList();
                Assert.Equal(allDataFromDb.Count, sampleList.Count);
            };
        }

        [Fact]
        public void Load1000RecordsFromMemoryAndEntireAddressTableFromDb()
        {
            var sampleList = new List<AllDataTypesDto>();
            for (var i = 0; i < 1000; i++)
            {
                sampleList.Add(new AllDataTypesDto
                {
                    Bigint = Int64.MaxValue,
                    Binary = new byte[] { 0x45, 0x46 },
                    Bit = true,
                    Date = DateTime.MaxValue.Date,
                    Datetime = DateTime.MaxValue.AddMilliseconds(-2), //Time range:	00:00:00 through 23:59:59.997 https://docs.microsoft.com/en-us/sql/t-sql/data-types/datetime-transact-sql?view=sql-server-ver15
                    Datetime2 = DateTime.MaxValue,
                    Datetimeoffset = DateTimeOffset.UtcNow.Date,
                    Decimal = Decimal.MaxValue,
                    Varbinary_Max = new byte[] { 0x4B, 0x49, 0x54, 0x41 },
                    Float = double.MaxValue,
                    Image = new byte[] { 0x45, 0x46 },
                    Int = int.MaxValue,
                    Nchar = "Ef6TempTableKit",
                    Ntext = "Ef6TempTableKit",
                    Numeric = Decimal.MaxValue,
                    Nvarchar = "Ef6TempTableKit",
                    Real = Single.MaxValue,
                    Smalldatetime = new DateTime(2079, 6, 5, 23, 59, 0),
                    Smallint = Int16.MaxValue,
                    Smallmoney = SMALL_MONEY_MAX,
                    Text = "Ef6TempTableKit",
                    Time = new TimeSpan(0, 4, 54, 56, 234),
                    Tinyint = byte.MaxValue,
                    Uniqueidentifier = Guid.NewGuid(),
                    Varbinary = new byte[] { 0x4B, 0x49, 0x54, 0x41 },
                    Varchar_50 = "wqS5LQa67cxMReRRFHC5CKptEnCVqieB04mOXbBl5ahk0M3S8j"
                });
            }

            using (var context = new AdventureWorksCodeFirst())
            {
                var addressCount = context.Addresses.Count();

                var addressQuery = context.Addresses.Select(x => new AllDataTypesDto
                {
                    Int = x.AddressID,
                    Varchar_50 = x.AddressLine1
                });

                var allDataFromDb = context
                    .WithTempTableExpression<AdventureWorksCodeFirst>(sampleList)
                    .WithTempTableExpression<AdventureWorksCodeFirst>(addressQuery)
                    .AllDataTypesTempTable.ToList();

                Assert.Equal(allDataFromDb.Count, sampleList.Count + addressCount);
            };
        }
    }
}
