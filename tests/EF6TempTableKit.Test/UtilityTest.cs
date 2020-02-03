using System.Linq;
using Xunit;
using EF6TempTableKit.Test.CodeFirst;
using EF6TempTableKit.Test.TempTables.Dependencies;
using EF6TempTableKit.Extensions;
using EF6TempTableKit.Test.TempTables;
using System.Collections.Generic;
using System.Text;
using EF6TempTableKit.Test.Extensions;

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

            var queryModel = new QueryModel(context);

            //Attach temp tables
            context.WithTempTableExpression<AdventureWorksCodeFirst>(queryModel.TempOfficeTypeQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(queryModel.TempAddressQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(queryModel.TempManufacturerQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(queryModel.TempPartTypeQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(queryModel.TempPartQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(queryModel.TempChairQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(queryModel.TempRoomQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(queryModel.TempPersonQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(queryModel.TempDepartmentQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(queryModel.TempOfficeQuery);

            return context;
        }

        [Fact]
        public void TempOnTempDependencies()
        {
            IDictionary<string, HashSet<string>> dependencies;
            using (var context = CreateTempQueries())
            {
                dependencies = context.TempTableContainer.TempOnTempDependencies;
            }

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
        }

        [Fact]
        public void TempOnTempDependenciesInQuery()
        {
            var finalQuery = new StringBuilder();
            using (var context = CreateTempQueries())
            {
                context.Database.Log = q => { finalQuery.AppendLine(q); };

                var tempOfficeTypeQuery = context.TempOfficeTypes.Take(1).FirstOrDefault();
                Assert.DoesNotContain(_tempTableAddress.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableManufacturer.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTablePart.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTablePartType.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableChair.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableRoom.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTablePerson.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableDepartment.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableOffice.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTableOfficeType.WrapWithSquareBrackets(), finalQuery.ToString());
                finalQuery.Clear();

                var tempAddress = context.TempAddresses.Take(1).FirstOrDefault();
                Assert.Contains(_tempTableAddress.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableManufacturer.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTablePart.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTablePartType.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableChair.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableRoom.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTablePerson.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableDepartment.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableOffice.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableOfficeType.WrapWithSquareBrackets(), finalQuery.ToString());
                finalQuery.Clear();

                var tempManufacturer = context.TempManufacturers.Take(1).FirstOrDefault();
                Assert.Contains(_tempTableAddress.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTableManufacturer.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTablePart.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTablePartType.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableChair.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableRoom.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTablePerson.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableDepartment.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableOffice.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableOfficeType.WrapWithSquareBrackets(), finalQuery.ToString());
                finalQuery.Clear();

                var tempPartType = context.TempPartTypes.Take(1).FirstOrDefault();
                Assert.DoesNotContain(_tempTableAddress.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableManufacturer.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTablePart.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTablePartType.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableChair.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableRoom.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTablePerson.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableDepartment.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableOffice.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableOfficeType.WrapWithSquareBrackets(), finalQuery.ToString());
                finalQuery.Clear();

                var tempPart = context.TempParts.Take(1).FirstOrDefault();
                Assert.Contains(_tempTableAddress.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTableManufacturer.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTablePart.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTablePartType.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableChair.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableRoom.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTablePerson.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableDepartment.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableOffice.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableOfficeType.WrapWithSquareBrackets(), finalQuery.ToString());
                finalQuery.Clear();

                var tempChair = context.TempChairs.Take(1).FirstOrDefault();
                Assert.Contains(_tempTableAddress.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTableManufacturer.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTablePart.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTablePartType.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTableChair.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableRoom.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTablePerson.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableDepartment.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableOffice.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableOfficeType.WrapWithSquareBrackets(), finalQuery.ToString());
                finalQuery.Clear();

                var tempRoom = context.TempRooms.Take(1).FirstOrDefault();
                Assert.Contains(_tempTableAddress.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTableManufacturer.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTablePart.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTablePartType.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTableChair.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTableRoom.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTablePerson.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableDepartment.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableOffice.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableOfficeType.WrapWithSquareBrackets(), finalQuery.ToString());
                finalQuery.Clear();

                var tempPerson = context.TempPersons.Take(1).FirstOrDefault();
                Assert.Contains(_tempTableAddress.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableManufacturer.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTablePart.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTablePartType.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableChair.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableRoom.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTablePerson.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableDepartment.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableOffice.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableOfficeType.WrapWithSquareBrackets(), finalQuery.ToString());
                finalQuery.Clear();

                var tempDepartment = context.TempDepartments.Take(1).FirstOrDefault();
                Assert.Contains(_tempTableAddress.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableManufacturer.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTablePart.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTablePartType.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableChair.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableRoom.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTablePerson.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTableDepartment.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableOffice.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.DoesNotContain(_tempTableOfficeType.WrapWithSquareBrackets(), finalQuery.ToString());
                finalQuery.Clear();

                var tempOffice = context.TempOffices.Take(1).FirstOrDefault();
                Assert.Contains(_tempTableAddress.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTableManufacturer.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTablePart.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTablePartType.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTableChair.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTableRoom.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTablePerson.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTableDepartment.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTableOffice.WrapWithSquareBrackets(), finalQuery.ToString());
                Assert.Contains(_tempTableOfficeType.WrapWithSquareBrackets(), finalQuery.ToString());
                finalQuery.Clear();
            }
        }

        [Fact]
        public void TempOnTempDependenciesInQueryOrderChanged()
        {
            var context = new AdventureWorksCodeFirst();
            var queryModel = new QueryModel(context);

            //Attach temp tables
            context.WithTempTableExpression<AdventureWorksCodeFirst>(queryModel.TempOfficeTypeQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(queryModel.TempAddressQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(queryModel.TempPersonQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(queryModel.TempDepartmentQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(queryModel.TempManufacturerQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(queryModel.TempPartTypeQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(queryModel.TempPartQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(queryModel.TempChairQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(queryModel.TempRoomQuery);
            context.WithTempTableExpression<AdventureWorksCodeFirst>(queryModel.TempOfficeQuery);

            var list = context.TempAddresses.Select(a => new
            {
                id1 = context.TempOffices.FirstOrDefault().Id,
                id2 = context.TempAddresses.FirstOrDefault().Id,
                id3 = context.TempManufacturers.FirstOrDefault().Id,
                id4 = context.TempPartTypes.FirstOrDefault().PartTypeId,
                id5 = context.TempChairs.FirstOrDefault().ChairId,
                id6 = context.TempRooms.FirstOrDefault().RoomId,
                id7 = context.TempPersons.FirstOrDefault().PersonId,
                id8 = context.TempDepartments.FirstOrDefault().DepartmentId,
                id9 = context.TempOffices.FirstOrDefault().Id,
            }).ToList();

            context.Dispose();

            Assert.NotEmpty(list);
        }
    }
}