using EF6TempTableKit.Test.CodeFirst;
using EF6TempTableKit.Test.TempTables;
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

                var tempAddress = context.Addresses.Select(a => new AddressDto { Id = a.AddressID, Name = a.AddressLine1 });

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

        [Fact]
        public void TestGetAddressMultipleId()
        {
            using (var context = new AdventureWorksCodeFirst())
            {
                //Database.SetInitializer<AdventureWorksCodeFirst>(null);
                var tempAddress = context.Addresses.Select(a => new AddressDto { Id = a.AddressID, Name = a.AddressLine1 });

                var addresses = context.Addresses.Join(tempAddress,
                    aL => aL.AddressID,
                    aR => aR.Id,
                    (aL, aR) => new
                    {
                        Id = aR.Id,
                        Id2 = aR.Id,
                        Name = aR.Name
                    }).ToList();

                Assert.True(false, "ID mapped twice.");
            }
        }

        int Add(int x, int y)
        {
            return x + y;
        }
    }
}
