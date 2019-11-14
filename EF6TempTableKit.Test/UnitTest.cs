using EF6TempTableKit.Extensions;
using EF6TempTableKit.Test.CodeFirst;
using EF6TempTableKit.Test.TempTables;
using System;
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

        //Excepted message: SqlException: 'aliastempAddressMultipleId' has fewer columns than were specified in the column list.
        [Fact]
        public void TestGetAddressMultipleId()
        {
            Assert.Throws<System.Data.Entity.Core.EntityCommandExecutionException> (() => {
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

        int Add(int x, int y)
        {
            return x + y;
        }
    }
}
