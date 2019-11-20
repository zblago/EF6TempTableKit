using EF6TempTableKit.Extensions;
using EF6TempTableKit.Test.CodeFirst;
using EF6TempTableKit.Test.TempTables;
using System.Collections.Generic;
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
                var tempAddressQuery = context.Addresses.Select(a => new AddressTempTableDto { Id = a.AddressID, Name = a.AddressLine1 });

                var addressList = context
                        .WithTempTableExpression<AdventureWorksCodeFirst>(tempAddressQuery)
                        .AddressesTempTable.Join(context.Addresses,
                        (a) => a.Id,
                        (aa) => aa.AddressID,
                        (at, a) => new { Id = at.Id }).ToList();

                Assert.NotEmpty(addressList);
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
                        //AddressID is mapped twice; EF throws exception
                        Id = a.AddressID,
                        Id2 = a.AddressID,
                        Name = a.AddressLine1
                    });

                    var addressList = context
                            .WithTempTableExpression<AdventureWorksCodeFirst>(tempAddressQuery)
                            .AddressesTempTableMultipleId.Join(context.Addresses,
                            (a) => a.Id,
                            (aa) => aa.AddressID,
                            (at, a) => new { Id = at.Id }).ToList();
                }
            });
        }

        [Fact(DisplayName = "Reuse temp table")]
        //In order to reuse the same table table (previously created in some of the top queries) in a second query just join with temp table. 
        //Calling again WithTempTableExpression() not needed as temp table already exists.
        public void ReuseSameTempTable()
        {
            using (var context = new AdventureWorksCodeFirst())
            {
                var tempAddressQuery = context.Addresses.Select(a => new AddressTempTableDto
                {
                    Id = a.AddressID,
                    Name = a.AddressLine1
                });

                var addressList = context
                        .WithTempTableExpression<AdventureWorksCodeFirst>(tempAddressQuery)
                        .AddressesTempTable.Join(context.Addresses,
                        (a) => a.Id,
                        (aa) => aa.AddressID,
                        (at, a) => new { Id = at.Id }).ToList();
                Assert.NotEmpty(addressList);

                var shipToAddress = context
                        .AddressesTempTable.Join(context.SalesOrderHeaders, //WithTempTableExpression() not needed; temp table is already created.
                            (a) => a.Id,
                            (soh) => soh.ShipToAddressID,
                            (soh, a) => new { Id = soh.Id }).ToList();
                Assert.NotEmpty(shipToAddress);
            }
        }

        [Fact(DisplayName = "Don't reuse temp table (reuseExisting = false")]
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

        [Fact(DisplayName = "Reuse temp table using flag (reuseExisting = true")]
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
                Assert.NotEmpty(addressList);

                var addressCount = joinAddressQuery.Count();
                Assert.True(addressCount > 0);
            }
        }

        [Fact]
        public void ProductListWithCategoryDetails()
        {
            using (var context = new AdventureWorksCodeFirst())
            {
                var productsCountCategoryQuery = context.Products.Join(context.ProductSubcategories,
                        (p) => p.ProductSubcategoryID,
                        (pcs) => pcs.ProductSubcategoryID,
                        (p, pcs) => new
                        {
                            CategoryId = pcs.ProductCategoryID,
                            ProductId = p.ProductID
                        })
                        .GroupBy((cp) => cp.CategoryId, (x) => new
                        {
                            x.CategoryId,
                            x.ProductId
                        })
                        .Select(x => new 
                        {
                            CategoryId = x.Key,
                            ProductCount = x.Count()
                        })
                        .Join(context.ProductCategories,
                            (tr) => tr.CategoryId,
                            (p) => p.ProductCategoryID,
                            (tr, p) => new ProductCategoryCountTempTableDto
                            {
                                CategoryId = tr.CategoryId,
                                CategoryName = p.Name,
                                ProductCount = tr.ProductCount
                            }
                        );

                var productsQuery = context
                        .WithTempTableExpression<AdventureWorksCodeFirst>(productsCountCategoryQuery, true)
                        .WorkOrders
                        .Join(context.Products,
                            (wo) => wo.ProductID,
                            (p) => p.ProductID,
                            (wo, p) => new
                            {
                                WorkOrderId = wo.WorkOrderID,
                                ScrappedQty = wo.ScrappedQty,
                                ProductId = p.ProductID,
                                ProductSubcategoryId = p.ProductSubcategoryID,
                                ProductNumber = p.ProductNumber,
                            })
                        .Join(context.ProductSubcategories,
                            (wo) => wo.ProductSubcategoryId,
                            (ps) => ps.ProductSubcategoryID,
                            (wo, ps) => new
                            {
                                WorkOrderId = wo.WorkOrderId,
                                ScrappedQty = wo.ScrappedQty,
                                ProductId = wo.ProductId,
                                ProductSubcategoryId = wo.ProductSubcategoryId,
                                ProductNumber = wo.ProductNumber,
                                CategoryId = ps.ProductCategoryID
                            })
                        .Join(context.ProductCategoryCountTempTables,
                            (wo) => wo.CategoryId,
                            (temp) => temp.CategoryId,
                            (wo, temp) => new
                            {
                                WorkOrderId = wo.WorkOrderId,
                                ScrappedQty = wo.ScrappedQty,
                                ProductId = wo.ProductId,
                                ProductSubcategoryId = wo.ProductSubcategoryId,
                                ProductName = wo.ProductNumber,
                                CategoryName = temp.CategoryName,
                                ProductCountPerCategory = temp.ProductCount
                            });

                var productList = productsQuery.ToList();
                Assert.NotEmpty(productList);

                var productCount = productsQuery.Count();
                Assert.True(productCount > 0);
            }
        }
    }
}
