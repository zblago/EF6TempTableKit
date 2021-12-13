using EF6TempTableKit.Extensions;
using EF6TempTableKit.Test.CodeFirst;
using EF6TempTableKit.Test.TempTables;
using EF6TempTableKit.Test.TempTables.Dependencies;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace EF6TempTableKit.Test
{
    public class UnitTest
    {
        [Fact]
        public void GetAddress()
        {
            using (var context = new AdventureWorksCodeFirst())
            {
                var tempAddressQuery = context.Addresses.Select(a => new AddressTempTableDto { Id = a.AddressID, Name = a.AddressLine1 });

                var addressList = context
                        .WithTempTableExpression<AdventureWorksCodeFirst>(tempAddressQuery)
                        .TempAddresses.Join(context.Addresses,
                        (a) => a.Id,
                        (aa) => aa.AddressID,
                        (at, a) => new { Id = at.Id }).ToList();

                Assert.NotEmpty(addressList);
            }
        }

        /// <summary>
        /// Expected message: SqlException: 'aliastempAddressMultipleId' has fewer columns than were specified in the column list.
        /// </summary>
        [Fact]
        public void GetAddressMultipleId()
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
                            .TempAddressesMultipleId.Join(context.Addresses,
                            (a) => a.Id,
                            (aa) => aa.AddressID,
                            (at, a) => new { Id = at.Id }).ToList();
                }
            });
        }

        /// <summary>
        /// In order to reuse the same query DDL (previously created in some of the top queries) in a second query just join with 
        /// temp table. 
        /// Calling again WithTempTableExpression() not needed since DDL is already attached.
        /// </summary>
        [Fact]
        public void ReuseSameAlreadyAttachedQueryOnSubsequentCall()
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
                        .TempAddresses.Join(context.Addresses,
                        (a) => a.Id,
                        (aa) => aa.AddressID,
                        (at, a) => new { Id = at.Id }).ToList();
                Assert.NotEmpty(addressList);

                var shipToAddress = context
                        .TempAddresses.Join(context.SalesOrderHeaders, //WithTempTableExpression() not needed; temp table is already created.
                            (a) => a.Id,
                            (soh) => soh.ShipToAddressID,
                            (soh, a) => new { Id = soh.Id }).ToList();
                Assert.NotEmpty(shipToAddress);
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
                        .WithTempTableExpression<AdventureWorksCodeFirst>(productsCountCategoryQuery)
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
                        .Join(context.TempProductCategoryCounts,
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

        /// <summary>
        /// Demo with two temp tables
        /// </summary>
        [Fact]
        public void ProductListWithCategoryDetailsTwoTempTables()
        {
            using (var context = new AdventureWorksCodeFirst())
            {
                var categories = context.ProductCategories.Select(pc => new ProductCategoryTempTableDto
                {
                    Id = pc.ProductCategoryID,
                    CategoryName = pc.Name
                });

                var subCategories = context.ProductSubcategories.Select(psc => new ProductSubCategoryTempTableDto
                {
                    Id = psc.ProductSubcategoryID,
                    CategoryId = psc.ProductCategoryID,
                    CategoryName = psc.Name,
                });

                var productsQuery = context
                        .WithTempTableExpression<AdventureWorksCodeFirst>(categories)
                        .WithTempTableExpression<AdventureWorksCodeFirst>(subCategories)
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
                        .Join(context.TempProductCategories,
                            (wo) => wo.ProductSubcategoryId,
                            (ps) => ps.Id,
                            (wo, ps) => new
                            {
                                WorkOrderId = wo.WorkOrderId,
                                ScrappedQty = wo.ScrappedQty,
                                ProductId = wo.ProductId,
                                ProductSubcategoryId = wo.ProductSubcategoryId,
                                ProductNumber = wo.ProductNumber,
                                CategoryId = ps.Id
                            })
                        .Join(context.TempProductSubCategories,
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
                            });

                var productList = productsQuery.ToList();
                Assert.NotEmpty(productList);
            }
        }

        [Fact]
        public void ReinitializeTempTableContainer()
        {
            var wantedResult = new Dictionary<int, string>()
            {
                { 1, "1970 Napa Ct." },
                { 2, "9833 Mt. Dias Blv." },
                { 3, "7484 Roundtree Drive" },
                { 4, "9539 Glenside Dr" },
                { 5, "1226 Shoe St." },
                { 6, "1399 Firestone Drive" }
            };

            var result = new Dictionary<int, string>();

            using (var context = new AdventureWorksCodeFirst())
            {
                for (var i = 0; i < 6; i++)
                {
                    var tempAddressQuery = context.Addresses.Select(a => new AddressTempTableDto
                    {
                        Id = a.AddressID,
                        Name = a.AddressLine1
                    }).OrderBy(ta => ta.Id).Skip(i).Take(1);

                    context.ReinitializeTempTableContainer();

                    var address = context
                            .WithTempTableExpression<AdventureWorksCodeFirst>(tempAddressQuery)
                            .TempAddresses.Join(context.Addresses,
                            (a) => a.Id,
                            (aa) => aa.AddressID,
                            (at, a) => new { Id = at.Id, AddressLine1 = a.AddressLine1 }).Single();

                    result.Add(address.Id, address.AddressLine1);
                }
            }

            for (var i = 0; i < 6; i++)
            {
                Assert.Equal(wantedResult[i + 1], result[i + 1]);
            }
        }

        [Fact]
        public void WhereClauseWithMoreThan10Parameters_ConditionIsNeverMet_CompiledAndExecutedSuccesfully()
        {
            var p0 = "test";
            var p1 = "test";
            var p2 = "test";
            var p3 = "test";
            var p4 = "test";
            var p5 = "test";
            var p6 = "test";
            var p7 = "test";
            var p8 = "test";
            var p9 = "test";
            var p10 = "test";
            var falseParam = false;

            using (var context = new AdventureWorksCodeFirst())
            {

                var departmentQuery = context.Departments
                                            .Where(x =>
                                                    x.Name == p0 &&
                                                    x.Name == p1 ||
                                                    x.Name == p2 ||
                                                    x.Name == p3 &&
                                                    x.Name == p4 ||
                                                    x.Name == p5 &&
                                                    x.Name == p6 ||
                                                    x.Name == p7 ||
                                                    x.Name == p8 &&
                                                    x.Name == p9 ||
                                                    x.Name == p10 ||
                                                    true == falseParam
                                        ).Select(x => new DepartmentTempTableDto()
                                        {
                                            Name = x.Name
                                        });

                var temp = context.WithTempTableExpression<AdventureWorksCodeFirst>(departmentQuery).TempDepartments;
                var result = temp.ToList();
                Assert.True(!result.Any());
            }
        }
    }
}
