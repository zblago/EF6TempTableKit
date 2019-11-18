using EF6TempTableKit.Extensions;
using EF6TempTableKit.Test.CodeFirst;
using EF6TempTableKit.Test.TempTables;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Xunit;

namespace EF6TempTableKit.Test
{
    public class UnitTest
    {
        [Fact]
        public void TestGetAddress()
        {
            using (var context = new AdventureWorksCodeFirst())
            {
                //Database.SetInitializer<AdventureWorksCodeFirst>(null);

                var tempAddress = context.Addresses.Select(a => new AddressTempTableDto { Id = a.AddressID, Name = a.AddressLine1 });

                var addresses = context.Addresses.Join(tempAddress,
                    aL => aL.AddressID,
                    aR => aR.Id,
                    (aL, aR) => new
                    {
                        Id = aL.AddressID,
                        Name = aR.Name
                    }).ToList();

                Assert.NotEmpty(addresses);
            }
        }

        //Expected message: SqlException: 'aliastempAddressMultipleId' has fewer columns than were specified in the column list.
        [Fact]
        public void TestGetAddressMultipleId()
        {
            Assert.Throws<System.Data.Entity.Core.EntityCommandExecutionException>(() =>
            {
                using (var context = new AdventureWorksCodeFirst())
                {
                    var tempAddressQuery = context.Addresses.Select(a => new AddressTempTableMultipleIdDto
                    {
                        //AddressID is mapped twice; exception is throwing up
                        Id = a.AddressID,
                        Id2 = a.AddressID,
                        Name = a.AddressLine1
                    });

                    var joinedAddress = context
                            .WithTempTableExpression<AdventureWorksCodeFirst>(tempAddressQuery)
                            .AddressesTempTableMultipleId.Join(context.Addresses,
                            (a) => a.Id,
                            (aa) => aa.AddressID,
                            (at, a) => new { Id = at.Id }).ToList();
                }
            });
        }

        [Fact(DisplayName = "Reuse temp table")]
        //In order to reuse the same table table (created in the some of the top queries) in a second query just join with temp table. Running code behind WithTempTableExpression() not needed as it is already exist.
        public void ReuseSameTempTable()
        {
            using (var context = new AdventureWorksCodeFirst())
            {
                var tempAddressQuery = context.Addresses.Select(a => new AddressTempTableDto
                {
                    //AddressID is mapped twice; exception is throwing up
                    Id = a.AddressID,
                    Name = a.AddressLine1
                });

                var joinedAddress = context
                        .WithTempTableExpression<AdventureWorksCodeFirst>(tempAddressQuery)
                        .AddressesTempTable.Join(context.Addresses,
                        (a) => a.Id,
                        (aa) => aa.AddressID,
                        (at, a) => new { Id = at.Id }).ToList();

                var shipToAddress = context
                        .AddressesTempTable.Join(context.SalesOrderHeaders,
                            (a) => a.Id,
                            (soh) => soh.ShipToAddressID,
                            (soh, a) => new { Id = soh.Id }).ToList();

                Assert.NotEmpty(shipToAddress);
            }
        }

        [Fact(DisplayName = "Don't reuse temp table (reuse existing set to false")]
        public void DonReuseSameTempTable()
        {
            using (var context = new AdventureWorksCodeFirst())
            {
                var tempAddressQuery = context.Addresses.Select(a => new AddressTempTableDto
                {
                    Id = a.AddressID,
                    Name = a.AddressLine1
                });

                IQueryable<int> joinAddressQuery = context
                        .WithTempTableExpression<AdventureWorksCodeFirst>(tempAddressQuery)
                        .AddressesTempTable.Join(context.Addresses,
                        (a) => a.Id,
                        (aa) => aa.AddressID,
                        (at, a) => at.Id);

                IList<int> addressList = joinAddressQuery.ToList();
                var addressCount = joinAddressQuery.Count();

                Assert.True(addressCount > 0);
            }
        }

        [Fact(DisplayName = "Reuse temp table using flag (reuse existing set to true")]
        public void ReuseSameTempTableWithUsingFlag()
        {
            using (var context = new AdventureWorksCodeFirst())
            {
                var tempAddressQuery = context.Addresses.Select(a => new AddressTempTableDto
                {
                    Id = a.AddressID,
                    Name = a.AddressLine1
                });

                IQueryable<int> joinAddressQuery = context
                        .WithTempTableExpression<AdventureWorksCodeFirst>(tempAddressQuery, true)
                        .AddressesTempTable.Join(context.Addresses,
                        (a) => a.Id,
                        (aa) => aa.AddressID,
                        (at, a) => at.Id);

                IList<int> addressList = joinAddressQuery.ToList();
                var addressCount = joinAddressQuery.Count();

                Assert.True(addressCount > 0);
            }
        }
    }
}
