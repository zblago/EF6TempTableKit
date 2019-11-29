# EF6TempTableKit 

EF6TempTableKit is a library that helps you utilize temp tables in your Entity Framework 6 context mapped to Microsoft SQL Server database.

## Overview

In some cases, when you write LINQ-to-Entities(L2E) queries, you would like to have a benefit like temp tables (e.g. create and insert records in temporary tables and later on reusing them as much as you want in a query). By default, EF doesn't support temporary tables and there is a reason why is like that. To overcome this "weakness", introducing EF6TempTableKit in your project, you can add a "temporary" entity as we are used to do it with "permanent" entitiy. In generated T-SQL query "temporary" entity will be mapped to a temporary table which resides in `tempDb` database and used normally like any other table.<br/>
Keep in mind: You are still writing LINQ-to-Entities to insert records in a "temporary" entity.

## Getting Started

Follow these steps:
1. Install Nuget package (will be right command placed here)
2. Implement `IDbContextWithTempTable` under your context. What does that mean? Add a public property and initialize it via constructor or auto-property initializer
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

EF6TempTableKit supports some features like reusing existing table under the same connection([SPID](https://docs.microsoft.com/en-us/sql/t-sql/functions/spid-transact-sql?view=sql-server-ver15)), clustered index and non-clustered indexes. Here is a short documentation:

| Extension       | Description |
| --------------- |-------------|
| `WithTempTableExpression` | Extension that accepts an expression being translated into T-SQL query that has a logic for inserting records in temp table. `WithTempTableExpression<T>(this System.Data.Entity.DbContext dbContexWithTempTable, IQueryable<ITempTable> expression, bool reuseExisting = false)` supports reusing existing temp table under the same [SPID](https://docs.microsoft.com/en-us/sql/t-sql/functions/spid-transact-sql?view=sql-server-ver15). If you set `reuseExisting` flag on `true`, generated T-SQL will check whether temp table already exists or not. That means, if you run mutliple queries under the same connection, you can reuse created temp table as temp table is scoped in SPID in which is created |

| Attribute       | Description |
| --------------- |-------------|
| `ClusteredIndex` | Associate this attribute with a field(s) you want in clustered index. |
| `NonClusteredIndex("nameOfIndex")` | Associate this attribute with a field(s) you want in non-clustered index. Number of non-clustered index is limited by SQL Server. If you want more columns under the same non-clustered index, just add a same name. Currently, order of columns in index is not supported |
| `TempFieldTypeAttribute` | Use this attribute to define field data type in a SQL Server manner. E.g. `([TempFieldTypeAttribute("varchar(200)")])`. |

## How it works

Before brief explanation of how EF6TempTableKit does his work keep in mind that **EF6TempTableKit doesn't affect EF6 default behaviour at all**. So, how it works? It uses EF6 ability to intercept a generated query before it hits a DB. But, before that, it does some digging through the internal/hidden EF6 properties and fields to get needed metadata (e.g. column order) and raw query. Using those informations it builds DML and DDL queries. When code execution goes through the attached `EF6TempTableKitQueryInterceptor` interceptor, previously generated query is being attached at the begining of the existing query.

![Final T-SQL](EF6TempTableKit-T-SQL.png)

## Problems

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

## So(l)utio(n) file & how to run it

Solution has a source code and tests that covers all features from Documentation secion.

After downloading source code, you can run and debug provided tests. Also, here is a simple Web application. It has almost nothing. The idea is to show how to write and run integration test.<br/>
Before you run test project, be sure that you have executed DB script from database folder:
1. Navigate to `...\EFIntercept\database\oltp-install-script`
2. Open `instawdb.sql` in SQL Server Management Studio
3. Enable [SQLCMD Mode](https://www.sqlshack.com/use-sqlcmd-commands-ssms-query-editor/)
4. This variable should point on directory where you downloaded this project
`:setvar SqlSamplesSourceDataPath "C:\Projects\EFIntercept\DBScript\oltp-install-script\"`<br/>
5. Run a script

## Running the tests

Test project is based on [xunit](https://xunit.net/) testing framework. In order to run test project, just follow these [steps](https://xunit.net/docs/getting-started/netfx/visual-studio).

### Installation Prerequisites
- EF6TempTableKit is built on [.NET Framework 4.5](https://www.microsoft.com/en-us/download/details.aspx?id=30653)

Projects liste below are built on [.NET Framework 4.6](https://www.microsoft.com/en-us/download/details.aspx?id=48130) <br/>
- EF6TempTableKit.Test
- EF6TempTableKit.Test.Web
- EF6TempTableKit.Test.Web.Temp

### Final word
Say that this is a hacky way to introduce some new functionallity into DBContext. It's not tested with delete, update, insert statemetns.

### Authors

[**Zoran Blagojevic**](https://www.linkedin.com/in/zoran-blagojevic/)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE) file for details

## Acknowledgments

* Hat tip to anyone whose code was used
* Hat tip to guys who has found how to get metadata from IQueryable
* Inspiration
* etc
