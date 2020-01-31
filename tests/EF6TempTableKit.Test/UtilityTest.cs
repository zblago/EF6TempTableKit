using System.Linq;
using Xunit;
using EF6TempTableKit.Test.CodeFirst;
using EF6TempTableKit.Test.TempTables.Dependencies;
using EF6TempTableKit.Extensions;
using EF6TempTableKit.Test.TempTables;

namespace EF6TempTableKit.Test
{
    public class UtilityTest
    {
        public readonly string _tempTableAddress = "#tempAddress";
        public readonly string _tempTableManufacturer = "#tempManufacturer";
        public readonly string _tempTablePart = "#tempPart";
        public readonly string _tempTablePartType = "#tempPartType";
        public readonly string _tempTableChair = "#tempChair";
        public readonly string _tempTableRoom = "#tempRoom";
        public readonly string _tempTablePerson = "#tempPerson";
        public readonly string _tempTableDepartment = "#tempDepartment";
        public readonly string _tempTableOffice = "#tempOffice";
        public readonly string _tempTableOfficeType = "#tempOfficeType";

        //                               O f f i c e
        //                             /    \       \
        //                            /      \       \
        //                           /        \       \
        //                       Room     Department  OfficeType
        //                      /            |
        //                  Chair         People
        //                  /                |
        //              Part              Address
        //              /    \
        //      Manufacturer PartType
        //           /
        //      Address
        ///
        /// Queries and how are they joined doesn't matter!
        /// This is only for internal test purpose. Tables such as Address, PartType and OfficeType don't have any dependencies whereas Office depends on all nodes below it (subtree).
        private AdventureWorksCodeFirst CreateTempQueries()
        {
            var context = new AdventureWorksCodeFirst();
            
            #region Create temp tables

            var tempAddressQuery = context.Addresses.Select(a => new AddressTempTableDto { Id = a.AddressID, Name = a.AddressLine1 });

            var tempPartTypeQuery = context.Addresses.Select(a => new PartTypeTempTableDto { PartTypeId = a.AddressID, Name = a.AddressLine1 });

            var tempManufacturerQuery = context.Addresses
                .Join(context.TempAddresses,
                    (a) => a.AddressID,
                    (ta) => ta.Id,
                    (a, ta) => new ManufacturerTempTableDto { Id = a.AddressID, AddressId = ta.Id, Name = a.AddressLine1 });

            var tempPartQuery = context.Addresses
                .Join(context.TempManufacturers,
                    (p) => p.AddressID,
                    (tm) => tm.AddressId,
                    (p, tm) => new
                    {
                        AddressId = p.AddressID,
                        Id = tm.Id,
                        Name = tm.Name,
                    })
                .Join(context.TempPartTypes,
                    (a) => a.Id,
                    (tmp) => tmp.PartTypeId,
                    (a, tmp) => new PartTempTableDto
                    {
                        PartId = a.AddressId,
                        PartTypeId = tmp.PartTypeId,
                        Name = tmp.Name,
                        ManufacturerId = a.Id
                    });

            var tempChairQuery = context.Addresses
                    .Join(context.TempParts,
                        (a) => a.AddressID,
                        (tp) => tp.PartId,
                        (a, tp) => new ChairTempTableDto
                        {
                            ChairId = a.AddressID,
                            PartId = tp.PartId,
                            Name = a.AddressLine1
                        });

            var tempRoomQuery = context.Addresses
                .Join(context.TempChairs,
                    (a) => a.AddressID,
                    (tc) => tc.ChairId,
                    (a, tc) => new RoomTempTableDto
                    {
                        RoomId = a.AddressID,
                        Name = a.AddressLine1,
                        ChairId = tc.ChairId
                    });

            var tempOfficeTypeQuery = context.Addresses.Select(tot => new OfficeTypeTempTableDto { Id = tot.AddressID, Name = tot.AddressLine1 });

            var tempOfficeQuery = context.Addresses
                .Join(context.TempRooms,
                    (a) => a.AddressID,
                    (to) => to.RoomId,
                    (a, to) => new OfficeTempTableDto
                    {
                        Id = a.AddressID,
                        RoomId = to.RoomId,
                        Name = a.AddressLine1,
                        OfficeTypeId = context.TempOfficeTypes.FirstOrDefault().Id,
                        DepartmentId = context.TempDepartments.FirstOrDefault().DepartmentId
                    });

            var tempPersonQuery = context.Addresses
                .Join(context.TempAddresses,
                    (a) => a.AddressID,
                    (ta) => ta.Id,
                    (a, ta) => new PersonTempTableDto
                    {
                        PersonId = a.AddressID,
                        Name = a.AddressLine2,
                        AddressId = ta.Id
                    });

            var tempDepartmentQuery = context.Addresses
                .Join(context.TempPersons,
                    (a) => a.AddressID,
                    (tp) => tp.AddressId,
                    (a, tp) => new DepartmentTempTableDto
                    {
                        DepartmentId = a.AddressID,
                        Name = a.AddressLine1,
                        LeadId = tp.PersonId
                    });

            #endregion

            //Attach temp tables
            context.WithTempTableExpression<AdventureWorksCodeFirst>(tempOfficeTypeQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(tempAddressQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(tempManufacturerQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(tempPartTypeQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(tempPartQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(tempChairQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(tempRoomQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(tempPersonQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(tempDepartmentQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(tempOfficeQuery);

            return context;
        }

        [Fact]
        public void TempOnDependenciese()
        {
            var context = CreateTempQueries();
            var dependencies = context.TempTableContainer.TempOnTempDependencies;

            var firstShouldBeManufacturer = dependencies.Skip(0).First();
            Assert.Equal(_tempTableManufacturer, firstShouldBeManufacturer.Key);
            Assert.Equal(string.Join(",", new string[] { _tempTableAddress }), string.Join(",", firstShouldBeManufacturer.Value));

            var secondShouldBePart = dependencies.Skip(1).First();
            Assert.Equal(_tempTablePart, secondShouldBePart.Key);
            Assert.Equal(string.Join(",", new string[] { _tempTableAddress, _tempTableManufacturer, _tempTablePartType }), string.Join(",", secondShouldBePart.Value));

            var thirdShouldBeChair = dependencies.Skip(2).First();
            Assert.Equal(_tempTableChair, thirdShouldBeChair.Key);
            Assert.Equal(string.Join(",", new string[] { _tempTableAddress, _tempTableManufacturer, _tempTablePartType, _tempTablePart }), string.Join(",", thirdShouldBeChair.Value));

            var fourthShouldBeRoom = dependencies.Skip(3).First();
            Assert.Equal(_tempTableRoom, fourthShouldBeRoom.Key);
            Assert.Equal(string.Join(",", new string[] { _tempTableAddress, _tempTableManufacturer, _tempTablePartType, _tempTablePart, _tempTableChair }), string.Join(",", fourthShouldBeRoom.Value));

            var fifthShouldBePerson = dependencies.Skip(4).First();
            Assert.Equal(_tempTablePerson, fifthShouldBePerson.Key);
            Assert.Equal(string.Join(",", new string[] { _tempTableAddress, }), string.Join(",", fifthShouldBePerson.Value));

            var sixthShouldBeDepartment = dependencies.Skip(5).First();
            Assert.Equal(_tempTableDepartment, sixthShouldBeDepartment.Key);
            Assert.Equal(string.Join(",", new string[] { _tempTableAddress, _tempTablePerson }), string.Join(",", sixthShouldBeDepartment.Value));

            var seventhShouldBeOffice = dependencies.Skip(6).First();
            Assert.Equal(_tempTableOffice, seventhShouldBeOffice.Key);
            Assert.Equal(string.Join(",", new string[] { _tempTableOfficeType, _tempTableAddress, _tempTableManufacturer, _tempTablePartType,
                _tempTablePart, _tempTableChair, _tempTableRoom, _tempTablePerson, _tempTableDepartment }),
                string.Join(",", seventhShouldBeOffice.Value));

            var office = context.TempOffices.ToList();

            //Test dependencies among temp tables
            //var queryAfterInterceptor = new StringBuilder();
            //context.Database.Log = q => { queryAfterInterceptor.AppendLine(q); };
            //var chairs = context.TempParts.ToList();

            //Test final DB query for only needed dependencies
        }
    }
}
