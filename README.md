# EF6TempTableKit 

EF6TempTableKit is a library that helps you utilize temporary tables in your Entity Framework 6 context mapped to Microsoft SQL Server database.

## Overview

Sometimes, when you write LINQ-to-Entities queries, you would like to have a benefit of using temp tables (e.g. create and insert records in temporary tables and later on reusing it as much as you want in a query). By default, EF doesn't support temporary tables and there is a reason why it is like that. To overcome this weakness, using EF6TempTableKit, we can add a "temporary" entity as we are used to do it with "permanent" entities. In generated T-SQL query "temporary" entity will be mapped to a temporary table which resides in `tempDb` database and used normally like other tables.
Don't forget: You are still writing LINQ-to-Entities to insert records in a "temporary" entity.

## Getting Started

Follow these steps:
1. Install Nuget package (will be right command placed here)
2. Implement `IDbContextWithTempTable` under your context. What does that mean? Add a public property and initialize it via constructor or auto-property initializer
```csharp
  public TempTableContainer TempTableContainer { get; set; } = new TempTableContainer();
```
3. Add a "temporary" entity and a DTO entity which inherits the previouse one. You need a both to make it work.
Ensure unique temporary table name that starts with # and has an marker interface `ITempTable`. Also, add  a sufix `TempTable` to make it unique and easy to distinguish later in a code.
```csharp
  [Table("#tempAddress")]
  public class AddressTempTable : ITempTable
  {
      public int Id { get; set; }

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
If you don't have already any configuration, use `EF6TempTableKitDbConfiguration`. Otherwise, apply your custom configuration.
```csharp
    [DbConfigurationType(typeof(CustomDbContextConfiguration))]
    public partial class AdventureWorksCodeFirst : DbContext, IDbContextWithTempTable
````
 But be sure that you have injected `EF6TempTableKitQueryInterceptor` interceptor.
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
      //Be sure that result is mapped into Dto table
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

### Features

What things you need to install the software and how to install them

```
Give examples
```

### Installing

A step by step series of examples that tell you how to get a development env running

Say what the step will be

```
Give the example
```

And repeat

```
until finished
```

End with an example of getting some data out of the system or using it for a little demo

## Running the tests

Explain how to run the automated tests for this system

### Break down into end to end tests

Explain what these tests test and why

```
Give an example
```

### And coding style tests

Explain what these tests test and why

```
Give an example
```

## Deployment

Add additional notes about how to deploy this on a live system

## Built With

* [Dropwizard](http://www.dropwizard.io/1.0.2/docs/) - The web framework used
* [Maven](https://maven.apache.org/) - Dependency Management
* [ROME](https://rometools.github.io/rome/) - Used to generate RSS Feeds

## Contributing

Please read [CONTRIBUTING.md](https://gist.github.com/PurpleBooth/b24679402957c63ec426) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/your/project/tags). 

## Authors

* **Billie Thompson** - *Initial work* - [PurpleBooth](https://github.com/PurpleBooth)

See also the list of [contributors](https://github.com/your/project/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* Hat tip to anyone whose code was used
* Inspiration
* etc
