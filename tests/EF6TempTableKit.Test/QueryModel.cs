using EF6TempTableKit.Test.CodeFirst;
using EF6TempTableKit.Test.TempTables;
using EF6TempTableKit.Test.TempTables.Dependencies;
using System.Linq;

namespace EF6TempTableKit.Test
{
    public class QueryModel
    {
        public IQueryable<AddressTempTableDto> TempAddressQuery;
        public IQueryable<PartTypeTempTableDto> TempPartTypeQuery;
        public IQueryable<ManufacturerTempTableDto> TempManufacturerQuery;
        public IQueryable<PartTempTableDto> TempPartQuery;
        public IQueryable<ChairTempTableDto> TempChairQuery;
        public IQueryable<RoomTempTableDto> TempRoomQuery;
        public IQueryable<OfficeTypeTempTableDto> TempOfficeTypeQuery;
        public IQueryable<OfficeTempTableDto> TempOfficeQuery;
        public IQueryable<PersonTempTableDto> TempPersonQuery;
        public IQueryable<DepartmentTempTableDto> TempDepartmentQuery;

        public QueryModel(AdventureWorksCodeFirst context)
        {

            TempAddressQuery = context.Addresses.Select(a => new AddressTempTableDto { Id = a.AddressID, Name = a.AddressLine1 });

            TempPartTypeQuery = context.Addresses.Select(a => new PartTypeTempTableDto { PartTypeId = a.AddressID, Name = a.AddressLine1 });

            TempManufacturerQuery = context.Addresses
                .Join(context.TempAddresses,
                    (a) => a.AddressID,
                    (ta) => ta.Id,
                    (a, ta) => new ManufacturerTempTableDto { Id = a.AddressID, AddressId = ta.Id, Name = a.AddressLine1 });

            TempPartQuery = context.Addresses
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

            TempChairQuery = context.Addresses
                    .Join(context.TempParts,
                        (a) => a.AddressID,
                        (tp) => tp.PartId,
                        (a, tp) => new ChairTempTableDto
                        {
                            ChairId = a.AddressID,
                            PartId = tp.PartId,
                            Name = a.AddressLine1
                        });

            TempRoomQuery = context.Addresses
                .Join(context.TempChairs,
                    (a) => a.AddressID,
                    (tc) => tc.ChairId,
                    (a, tc) => new RoomTempTableDto
                    {
                        RoomId = a.AddressID,
                        Name = a.AddressLine1,
                        ChairId = tc.ChairId
                    });

            TempOfficeTypeQuery = context.Addresses.Select(tot => new OfficeTypeTempTableDto { Id = tot.AddressID, Name = tot.AddressLine1 });

            TempOfficeQuery = context.Addresses
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

            TempPersonQuery = context.Addresses
                .Join(context.TempAddresses,
                    (a) => a.AddressID,
                    (ta) => ta.Id,
                    (a, ta) => new PersonTempTableDto
                    {
                        PersonId = a.AddressID,
                        Name = a.AddressLine2,
                        AddressId = ta.Id
                    });

            TempDepartmentQuery = context.Addresses
                .Join(context.TempPersons,
                    (a) => a.AddressID,
                    (tp) => tp.AddressId,
                    (a, tp) => new DepartmentTempTableDto
                    {
                        DepartmentId = a.AddressID,
                        Name = a.AddressLine1,
                        LeadId = tp.PersonId
                    });
        }
    }
}
