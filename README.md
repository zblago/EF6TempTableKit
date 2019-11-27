# EF6TempTableKit 

EF6TempTableKit is a library that helps you utilize temporary tables in your Entity Framework 6 context mapped to Microsoft SQL Server database.

## Overview

Sometimes, when you write LINQ-to-Entities queries, you would like to have a benefit of using temp tables (e.g. create and insert records in temporary tables and later on reusing it as much as you want in a query). By default, EF doesn't support temporary tables and there is a reason why is like that. To overcome this weakness, using EF6TempTableKit, we can add a "temporary" entity as we are used to do it with "permanent" entities. In generated T-SQL query "temporary" entity will be mapped to a temporary table which resides in `tempDb` database and used normally like other tables.<br/>
Keep in mind: You are still writing LINQ-to-Entities to insert records in a "temporary" entity.

## Getting Started

Follow these steps:
1. Install Nuget package (will be right command placed here)
2. Implement `IDbContextWithTempTable` under your context. What does that mean? Add a public property and initialize it via constructor or auto-property initializer
```csharp
  public TempTableContainer TempTableContainer { get; set; } = new TempTableContainer();
```
3. Add a "temporary" entity and a DTO entity which inherits the previouse one. You need a both to make it work.
Ensure unique temporary table name that starts with # and has a marker interface `ITempTable`. Also, add  a sufix `TempTable` to make it unique and easy to distinguish later in a code. For each field set appropiate SQL Server data type that will be used on table creation. 
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
4. Add a "temporary" entity into your context 

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
If you don't have already any configuration, use `EF6TempTableKitDbConfiguration`. Otherwise, apply your custom configuration...
```csharp
    [DbConfigurationType(typeof(CustomDbContextConfiguration))]
    public partial class AdventureWorksCodeFirst : DbContext, IDbContextWithTempTable
````
 ...but be sure that you have injected `EF6TempTableKitQueryInterceptor` interceptor.
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

## Extension & Attributes

EF6TempTableKit supports some features like reusing existing table under the same connection, clustered index and non-clustered indexes.

| Extension       | Description |
| --------------- |-------------|
| `WithTempTableExpression`| Extension method `WithTempTableExpression<T>(this System.Data.Entity.DbContext dbContexWithTempTable, IQueryable<ITempTable> expression, bool reuseExisting = false)` supports reusing existing temp table under the same [SPID](https://docs.microsoft.com/en-us/sql/t-sql/functions/spid-transact-sql?view=sql-server-ver15). If you set this flag on `true` generated T-SQL will check whether temp table already exists or not. That means if you run mutliple queries under the same connection you can reuse created temp table as temp table is scoped in SPID in which is created http://www.sqlservertutorial.net/sql-server-basics/sql-server-temporary-tables/|

| Attribute       | Description |
| --------------- |-------------|
|`ClusteredIndex`| Add this attribute on all fields you want in clustered index. Name in T-SQL code will be generated automatically|
|`NonClusteredIndex("nameOfIndex")`| Add this attribute under the fields you want in non-clustered index. Number of non-clustered index is limited by SQL Server. If you want more columns under the same non-clustered index, just add a same name. Currently, order of columns in index is not supported|
| `TempFieldTypeAttribute` |Use this attribute to define field data type |

## How it works

Before brief explanation of how EF6TempTableKit does his work keep in mind that **EF6TempTableKit doesn't interfer EF6 default behaviour at all**. So, how it works? It uses EF6 ability to intercept a generated query before it hits a DB. But, before that, it does some digging through the internal/hidden EF6 properties and fields to get needed metadata (e.g. column order) and raw query. Using those informations it builds DML and DDL queries. When code execution goes through the attached `EF6TempTableKitQueryInterceptor` interceptor, previously generated query is being attached at the begining of the existing query.

## Problems

## So(l)utio(n) file & how to run it

Describe here solution file. Connect test with description.

### Running the tests

Explain how to run the automated tests for this system

### Installation Prerequisites
- EF6TempTableKit is built on [.NET Framework 4.5](https://www.microsoft.com/en-us/download/details.aspx?id=30653)

Projects liste below are built on [.NET Framework 4.6](https://www.microsoft.com/en-us/download/details.aspx?id=48130) <br/>
- EF6TempTableKit.Test
- EF6TempTableKit.Test.Web
- EF6TempTableKit.Test.Web.Temp

### Built With

* [Dropwizard](http://www.dropwizard.io/1.0.2/docs/) - The web framework used
* [Maven](https://maven.apache.org/) - Dependency Management
* [ROME](https://rometools.github.io/rome/) - Used to generate RSS Feeds

### Contributing

Please read [CONTRIBUTING.md](https://gist.github.com/PurpleBooth/b24679402957c63ec426) for details on our code of conduct, and the process for submitting pull requests to us.

### Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/your/project/tags). 

### Authors

* **Zoran Blagojevic** - https://www.linkedin.com/in/zoran-blagojevi%C4%87-b2540a6/

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE) file for details

## Acknowledgments

* Hat tip to anyone whose code was used
* Inspiration
* etc
