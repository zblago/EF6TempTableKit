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
                        Binary = new byte[] { 0x45, 0x46},
                        Bit = true,
                        Date = DateTime.Today,
                        Datetime = DateTime.Today,
                        Datetime2 = DateTime.Today,
                        Datetimeoffset = DateTimeOffset.UtcNow.Date,
                        Decimal = Decimal.MaxValue,
                        Varbinary_Max = new byte[] { 0x4B, 0x49, 0x54, 0x41 },
                        Float = double.MaxValue
                    }
                };

                //var totalCount = context
                //        .WithTempTableExpression<AdventureWorksCodeFirst>(allDataTypesList)
                //        .AllDataTypesTempTable.Count();

                var allDataTypeItemFromDb = context
                        .WithTempTableExpression<AdventureWorksCodeFirst>(allDataTypesList)
                        .AllDataTypesTempTable.First();

                var allDataTypeItemFromMemory = allDataTypesList.First();

                //Assert.True(totalCount > 0);
                Assert.Equal(allDataTypeItemFromDb.Bigint, allDataTypeItemFromMemory.Bigint);
                Assert.Equal(allDataTypeItemFromDb.Binary, allDataTypeItemFromMemory.Binary);
                Assert.Equal(allDataTypeItemFromDb.Bit, allDataTypeItemFromMemory.Bit);
                Assert.Equal(allDataTypeItemFromDb.Date, allDataTypeItemFromMemory.Date);
                Assert.Equal(allDataTypeItemFromDb.Datetime, allDataTypeItemFromMemory.Datetime);
                Assert.Equal(allDataTypeItemFromDb.Datetime2, allDataTypeItemFromMemory.Datetime2);
                Assert.Equal(allDataTypeItemFromDb.Datetimeoffset.Date, allDataTypeItemFromMemory.Datetimeoffset.Date);
                Assert.Equal(allDataTypeItemFromDb.Decimal, allDataTypeItemFromMemory.Decimal);
                Assert.Equal(allDataTypeItemFromDb.Varbinary_Max, allDataTypeItemFromMemory.Varbinary_Max);
                Assert.Equal(allDataTypeItemFromDb.Float, allDataTypeItemFromMemory.Float);
            }
        }
    }
}
