using EF6TempTableKit.Exceptions;
using EF6TempTableKit.Extensions;
using EF6TempTableKit.Test.CodeFirst;
using EF6TempTableKit.Test.TempTables;
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
    }
}
