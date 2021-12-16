# EF6TempTableKit - version 3.0.1
EF6TempTableKit is a library that enriches Entity Framework 6 by introducing new type of entities which are not natively supported - temporary entities.

[![Build status](https://ci.appveyor.com/api/projects/status/tsdv1s7v897f2mwv?svg=true)](https://ci.appveyor.com/project/zblago/ef6temptablekit)
[![AppVeyor tests](https://img.shields.io/appveyor/tests/zblago/EF6TempTableKit)](https://ci.appveyor.com/project/zblago/ef6temptablekit/build/tests)
[![Nuget](https://img.shields.io/nuget/v/EF6temptablekit)](https://www.nuget.org/packages/EF6TempTableKit/)

## Versions
1.0.0 - check the details [here](https://github.com/zblago/EF6TempTableKit/tree/EF6TempTableKit_version_1_0_0)  
2.0.0 - check the details [here](https://github.com/zblago/EF6TempTableKit/tree/EF6TempTableKit_version_2_0_0)

## Overview

We all know how to write LINQ-to-Entities(L2E) queries to fetch data from the database. No T-SQL, only C#.<br/>But, in some cases, writing and optimizing LINQ-to-Entities(L2E) queries may be easier and pleasnt if we can use MS SQL Server temporary tables.<br/><br/>What does that mean?<br/>Imagine yourself declaring and loading data into temp tables just as you are used to do with regular EF context entities. By default, EF doesn't support temporary tables and there is a reason why is like that. To overcome this "weakness", by plugging EF6TempTableKit into your project, you can introduce a "temporary" entity as we are used to do it with "permanent" entity. In generated T-SQL query, "temporary" entity will be mapped to the temporary table which resides in `tempDb` database and then used normally like any other table.<br/>
Keep in mind: You are still writing LINQ-to-Entities to insert records into a "temporary" entity.

## What is changed in version 3.0.1

Version 3.0.1 has some bug fixes

| Bug description | Resolution |
| --------------- |------------|
| LINQ query empowered by LinqKit and EF6TempTableKit throws exception|LinqKit driven LINQ query has stored raw sql at some deeper level. Inspecting where is needed property now we have corresponding object being used later to get raw sql |
|LINQ query that has more than 10 parameters in where clause behaves unexpectedly because they are iterated in ascending order|By changing sort order, same parameter will not be replace twice or more |

... changes

| Change | Description |
| --------------- |------------|
| `reuseExisting` is becoming obsolete| Internally each executed query is followed by `exec sp_reset_connection` making all the queries scoped within the same [SPID](https://docs.microsoft.com/en-us/sql/t-sql/functions/spid-transact-sql?view=sql-server-ver15) separated by individual batch which doesn't give you any chance to reuse existing temp table that you already have created within the same database connection. So, this parameter literally does nothing. |

.... and improvements

| Improvement | Description |
| --------------- |------------|
| Load the data from memory | Now we know how to prepare our context and LINQ to make our plugin work, but you've noticed that query can be optimized by sending data from the memory since they are already cached but needed for the query. Writing query to get same data again makes your final query complex and potentionally increases total execution time. This improvement helps you generate T-SQL along with the data from the memory so your query can be simplified by keeping focus on the main part. This improvement is covered with the unit tests with almost all possible .NET data types. |

## Getting Started

Follow these steps:
1. Install Nuget package (`Install-Package EF6TempTableKit -Version 3.0.1`)
2. Implement `IDbContextWithTempTable` within your context. What does that mean? Add a public property and initialize it via constructor or auto-property initializer
```csharp
  public TempTableContainer TempTableContainer { get; set; } = new TempTableContainer();
```
3. Add a "temporary" entity and a DTO entity which inherits the previouse one. You need a both of them to make it work.
Ensure unique temporary table name that starts with # and has implemented marker interface `ITempTable`. Also, add  a sufix `TempTable` to make it unique and easy to distinguish it later in a code. For each field, set appropiate SQL Server data type that will be used throughout table creation. 
```csharp
  [Table("#tempAddress")]
  public class AddressTempTable : ITempTable
  {
      [TempFieldTypeAttribute("int")]
      public int Id { get; set; }
      
      [TempFieldTypeAttribute("varchar(200)")]
      public string AddressLine { get; set; }
  }

  [NotMapped]
  public class AddressTempTableDto : AddressTempTable
  {
  }
```
4. Add a "temporary" entity on a context 

```csharp
  public virtual DbSet<AddressTempTable> AddressesTempTable { get; set; }
```
5. Apply a configuration on a context.
```csharp
    [DbConfigurationType(typeof(EF6TempTableKitDbConfiguration))]
    public partial class AdventureWorksCodeFirst : DbContext, IDbContextWithTempTable
    ...
    ...    
```
If you don't have already any configuration, use `EF6TempTableKitDbConfiguration`. Otherwise, apply your own custom configuration ...
```csharp
    [DbConfigurationType(typeof(CustomDbContextConfiguration))]
    public partial class AdventureWorksCodeFirst : DbContext, IDbContextWithTempTable
````
 ... but be sure that you have injected `EF6TempTableKitQueryInterceptor` interceptor.
````csharp
    public class CustomDbContextConfiguration : DbConfiguration
    {
        public CustomDbContextConfiguration()
        {
            AddInterceptor(new AdventureWorkQueryInterceptor());
            AddInterceptor(new EF6TempTableKitQueryInterceptor());
        }
    }
````
6. Write a query
```csharp
  using (var context = new AdventureWorksCodeFirst())
  {
      //Be sure that result is mapped into DTO object
      var tempAddressQuery = context.Addresses.Select(a => new AddressTempTableDto { Id = a.AddressID, Name = a.AddressLine1 });

      var addressList = context
              .WithTempTableExpression<AdventureWorksCodeFirst>(tempAddressQuery)
              .AddressesTempTable.Join(context.Addresses,
              (a) => a.Id,
              (aa) => aa.AddressID,
              (at, a) => new { Id = at.Id }).ToList();                
  }
```
7. Run a code.

## Documentation

EF6TempTableKit supports some features like reusing existing table within the same connection([SPID](https://docs.microsoft.com/en-us/sql/t-sql/functions/spid-transact-sql?view=sql-server-ver15)), clustered index and non-clustered indexes. Here is a short documentation:

| Extension       | Description |
| --------------- |-------------|
| `WithTempTableExpression` | Extension that accepts an expression being translated into T-SQL query that has a logic for inserting records into a temp table. `WithTempTableExpression<T>(this System.Data.Entity.DbContext dbContexWithTempTable, IQueryable<ITempTable> expression)`<br/>**Important - You can attach an expression that requires for its creation some other expression(s). In that case, you have to take care of an order in which expressions are being attached in a way that first are coming expressions that have a little or no dependencies to those that have dependencies on previously attached expressions. Regularly, in T-SQL you will do the same. At the top of T-SQL query, we are declaring and inserting data into tables that are used later in a code for inserting data into some other temporary tables. The same logic is applied here, but it is on a developer to keep an eye on expression order.**|
| `ReinitializeTempTableContainer` | Clears out the attached expressions. |

| Attribute       | Description |
| --------------- |-------------|
| `ClusteredIndex` | Associate this attribute with a field(s) you want in clustered index. |
| `NonClusteredIndex("indexName, [orderNo = 0]")` | Associate this attribute with a field(s) you want in non-clustered index. Number of non-clustered index is limited by SQL Server. If you want more columns within the same non-clustered index, just add a same name. You can set order of the columns by using `orderNo` parameter. |
| `TempFieldTypeAttribute` | Use this attribute to define field data type in a SQL Server manner. E.g. `([TempFieldTypeAttribute("varchar(200)")])`. |
|`CustomConverterAttribute`| Use this attribute to define custom `ICustomConverter` converter for in-memory data. You are defining own converter if EF6TempTableKit is not smart enough to convert the data for you. When do I need this? E.g. for some reason _date_ is not properly _translated_ from memory to T-SQL query. This way you are free to define own converter based on your local settings and convert such a data as per your needs. Note: take a reference to the UnitTest project, there is covered almost every scenario you need |
|`StringConverterAttribute`| A lots of data types can be send as a string and properly stored into any data type (date, int...), hence this one is coming along with extension. Same as `CustomConverterAttribute` but dedicated to the string data type.|

## How it works

Before brief explanation of how EF6TempTableKit does his work keep in mind that **EF6TempTableKit doesn't affect EF6 default behaviour at all**. So, how it works? It uses EF6 ability to intercept a generated query before it hits a DB. But, before that, it does some digging through the internal/hidden EF6 properties and fields to get needed metadata (e.g. column order) and raw query. Using those informations it builds DML and DDL queries. When code execution goes through the attached `EF6TempTableKitQueryInterceptor` interceptor, previously attached queries are being attached at the begining of the intercepted query.

![Final T-SQL](EF6TempTableKit-T-SQL.png)

The code below represents importance of an order in which expressions are attached. Expressions with no or little dependencies are coming first, those with dependencies are coming afterwards.

```csharp
    ....
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
    
    var tempAddress = context.TempAddresses.Take(1).FirstOrDefault();
```
Final T-SQL query will be formed of only query against TempAddress temp table regardless of how many expressions are in `TempTableContainer`. No killing performances by creating and loading data into not used tables.

## Known issues

EF6TempTableKit has been implemented on enterprise project which has VS solution with more than 15 projects, where base context is inherited on multiple levels, where DBContext has DbConfiguration in a different project, etc... <br/>In such a bit complicated scenario, ony one exception occured that was not explicity related to E6TempTableKit. It was about how to apply custom configuration on your `DbContext`.<br/>So, If you get exception like this:<br/>

*The default DbConfiguration instance was used by the Entity Framework before the 'DbConfig' type was discovered. An instance of 'DbConfig' must be set at application start before using any Entity Framework features or must be registered in the application's config file. See http://go.microsoft.com/fwlink/?LinkId=260883*

just follow solution from [here](https://docs.microsoft.com/hr-hr/ef/ef6/fundamentals/configuring/code-based?redirectedfrom=MSDN)
or [here](https://stackoverflow.com/questions/19929282/ef6-modelconfiguration-set-but-not-discovered)

In my case, the following code was enough:<br/>
`<entityFramework codeConfigurationType="MyNamespace.MyDbConfiguration, MyAssembly">
    ...Your EF config...
</entityFramework>`

In addition to that, if you are getting the following exception:

*One or more validation errors were detected during model generation:
EF6TempTableKit.Test.CodeFirst.ProductCategoryCountTempTable: : EntityType 'ProductCategoryCountTempTable' has no key defined. Define the key for this EntityType.
ProductCategoryCountTempTables: EntityType: EntitySet 'ProductCategoryCountTempTables' is based on type 'ProductCategoryCountTempTable' that has no keys defined.
Also, that's the case when we don't have Id field (CategoryId - throws exception, if we rename it to Id- it works)*

Ensure that your "temporary" entity has ID field (`public int ID {get; set;}`) or `[Key]` attribute associated with a column that represents ID.

If you are mapping the same field twice or more into DTO entity, like here (a.AddressID) <br/>
```csharp
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
```
you may get an exception like *SqlException: 'tempTableName' has fewer columns than were specified in the column list*. In that case ensure that is mapped only once. Later on, if you need to map it again, do that on materialized data (in memory). This case is also covered in test project.

## (S)o(l)utio(n) file & how to run it

Solution has a source code and tests that covers all features from Documentation section.

After downloading source code, you can run and debug provided tests. Also, here is a simple Web application. It has almost nothing. The idea is to show how to write and run integration test.<br/>
Before you run test project, be sure that you have executed DB script from database folder:
1. Navigate to `...\EF6TempTableKit\database\oltp-install-script`
2. Open `instawdb.sql` in SQL Server Management Studio
3. Enable [SQLCMD Mode](https://www.sqlshack.com/use-sqlcmd-commands-ssms-query-editor/)
4. This variable should point on directory where you downloaded this project
`:setvar SqlSamplesSourceDataPath "C:\Projects\EF6TempTableKit\DBScript\oltp-install-script\"`<br/>
5. Run a script

### Running tests

Test project is based on [xunit](https://xunit.net/) testing framework. In order to run test project, just follow these [steps](https://xunit.net/docs/getting-started/netfx/visual-studio). Before you run test project, make sure that you have changed connection strings.

## Installation Prerequisites
- EF6TempTableKit is built under [.NET Framework 4.5](https://www.microsoft.com/en-us/download/details.aspx?id=30653)

Projects listed below are built under [.NET Framework 4.6](https://www.microsoft.com/en-us/download/details.aspx?id=48130) <br/>
- EF6TempTableKit.Test
- EF6TempTableKit.Test.Web

## Final word
I was very motivated to finish up this project. I have proven to myself that I can do this after regular work hours, playing with the kids, etc... At the end, I spent almost 3 months coding this extension, including documentation, tests, articles.

You might find this library useful for you. However, this library is not tested for insert, delete and update scenarios. It's on you to test whether it works or not.

<a href="https://www.buymeacoffee.com/ZnPcX6D" target="_blank"><img src="https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png" alt="Buy Me A Coffee" style="height: 41px !important;width: 174px !important;box-shadow: 0px 3px 2px 0px rgba(190, 190, 190, 0.5) !important;-webkit-box-shadow: 0px 3px 2px 0px rgba(190, 190, 190, 0.5) !important;"></a>

## Authors

[**Zoran Blagojevic**](https://www.linkedin.com/in/zoran-blagojevic/)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE) file for details

## Acknowledgments

Hat tip to guys who made some useful things used here:
* https://www.stevefenton.co.uk/2015/07/getting-the-sql-query-from-an-entity-framework-iqueryable/
* http://www.davidberube.me/getting-the-sql-generated-by-entity-framework-queryable/
* https://www.tpeczek.com/2016/10/entity-framework-6-dynamically-creating_13.html
