Danny Boy
=========

Simple data access layer for SQL Server.


## Versioning

This project uses [SemVer](http://semver.org) for versioning guidelines. The public interface is what is presented in the `IDataAccess` interface and the attributes/decorators documented below. Relying on the public methods in the `DataAccess` class or other public helper classes may break backwards compatibility even without a new major revision.


## Installation

Install the engine via [NuGet](http://nuget.org/packages/dannyboy), either in Visual Studio (right-click project, Manage NuGet Packages, search for docxtemplateengine) or via the package manager console using `Install-Package dannyboy`.


## Usage

Create a new `dannyboy.DataAccess` instance and pass in the database connection string (do this with your favourite IoC container):

    IDataAccess dataAccess = new DataAccess(@"Server=.\sqlexpress; Database=swxben_dataaccess; User Id=sa; Password=test;");


### Select

The select method uses a strongly typed DTO:

    var myThings = dataAccess.Select<MyThing>(
        where: new { MyName = "Ben" },
        orderBy: "MyId"
    );

    SELECT * FROM MyThings WHERE MyName = 'Ben' ORDER BY MyId

The table name can be passed in explicitly without relying on the type name:

    var myThings = dataAccess.Select("MyThings");   // returns IEnumerable<dynamic>
    var myThings = dataAccess.Select<Thing>("MyThings");    // returns IEnumerable<Thing>

#### Properties

Public auto-implemented properties with a public or protected setter are automatically set, as are non-auto-implemented properties with a public, protected or private setter. Protected and private properties, as well as public auto-implemented properties with a private setter, are not automatically set.

    // can be set:
    public string Foo { get; set }
    public string Foo { get; protected set; }
    public string Foo { get { .. } set { .. } }
    public string Foo { get { .. } protected set { .. } }
    public string Foo { get { .. } private set { .. } }

    // cannot be set:
    public string Foo { get; private set; }
    protected string Foo { get; set; }
    private string Foo { get; set; }

#### Default constructors

Prior to version 2.6.0, only classes with public default constructors could be automatially instantiated. Beginning with version 2.6.0, classes with public, protected or private default constructors can be automatically instantiated. Non-default constructors are ignored, unless explicitly called using the `factory` parameter. A class without a default constructor cannot be automatically instantiated.

    // Cannot be automatically instantiated using dataAccess.ExecuteQuery<Foo>(".."):
    public class Foo
    {
        public Foo(int bar) { }

        public int Bar { get; set; }
    }

### Insert

Insert using strongly typed DTOs. The table name is by convention the type name plus `s`.

    dataAccess.Insert(new MyThing { MyId = 13, MyName = "Ben" });

This roughly corresponds to:

    INSERT INTO MyThings(MyId, MyName) VALUES(13, 'Ben')

It actually uses named fields so SQL injection shouldn't be possible in normal use.

The table name can be passed in expicitly without relying on the type name:

    dataAccess.Insert(new { MyId = 13, MyName = "Ben"}, "MyThings");

    INSERT INTO MyThings(MyId, MyName) VALUES(13, 'Ben')

The value of `@@IDENTITY` is returned as a dynamic object by the `Insert` method. Columns marked as identifier fields (`DataAccess.Identifier`) that are `int` types are not included in the `INSERT` statement.

    note.NoteId = (int)dataAccess.Insert(note);


### Update

Update needs the field name for the identifier, rather than using a strict convention. An array of field names is passed in allowing for compound keys. Note that the field names changed from `params string[]` to `string[]` in version 2.0.0 to allow for explicitly specified table names.

    dataAccess.Update(new MyThing { MyId = 13, MyName = "Ben"}, new[] { "MyId" });

    UPDATE MyThings SET MyName = 'Ben' WHERE MyId = 13

Compound keys can be specified:

    dataAccess.Update(
        new MyThing { ForeignKey1 = 5, ForeignKey2 = 8, AtDate = DateTime.Now }, 
        new [] { "ForeignKey1", "ForeignKey2" });

        UPDATE MyThings SET AtDate = '...' WHERE ForeignKey1 = 5 AND ForeignKey2 = 8

Alternatively, identifier fields can be decorated:

    class MyThing {
        [DataAccess.Identifier]
        public int MyId;
        public string MyName;
    }

    dataAccess.Update(new MyThing { MyId = 4, MyName = "Ben" });

    UPDATE MyThings SET MyName = 'Ben' WHERE MyId = 4

Multiple fields can be decorated with the `DataAccess.Identifier` attribute for compound keys. Specifying the identifier columns using the first overload above will override the decorated fields.

The table name can be passed in explicitly without relying on the type name:

    dataAccess.Update(new[] { MyId = 4, MyName = "Ben"}, new[] { "MyId"}, "MyThings");

    UPDATE MyThings SET MyName = 'Ben' WHERE MyId = 4


### Execute arbitrary queries

There are two versions of `ExecuteQuery()`, one returns strongly-typed DTOs, the other returns dynamics:

	IEnumerable<MyThing> myThings = dataAccess.ExecuteQuery<MyThing>(
		"SELECT * FROM MyThings WHERE MyName = @MyName", 
		new { MyName = "Ben" });

	IEnumerable<dynamic> myThings = dataAccess.ExecuteQuery<MyThing>(
		"SELECT * FROM MyThings WHERE MyName = @MyName", 
		new { MyName = "Ben" });


### Execute an arbitrary command

	dataAccess.ExecuteCommand(
		"DELETE FROM MyThings WHERE MyId = @MyId",
		new { MyId = 17 });


### Ignore fields and properties

Decorate fields and properties with the `[DataAccess.Ignore]` attribute to indicate that they should be ignored by the `Insert` and `Update` commands.

    class MyThing {
    	public string Name{ get; set; }
    	[DataAccess.Ignore]
    	public int WidgetCount { get; set; }
    }

    dataAccess.Insert(new MyThing { Name = "Ben" });

    INSERT INTO MyThings(Name) VALUES('Ben')

Ignored fields and properties are still assigned when a value comes in from a query:

    dataAccess.ExecuteQuery<MyThing>(
    	"SELECT t.*, (SELECT COUNT(*) FROM Widgets w WHERE w.Owner = t.Name) AS WidgetCount FROM MyThings t");

    /* MyThing.Name == "Ben", MyThing.WidgetCount == 12 */


### DTOs without a parameterless constructor

Most simple DTOs will just have a default constructor, however there are some cases where a DTO 
will require a more complex constructor. `ExecuteQuery<T>()` and `Select<T>()` have overloads
that accept a factory method - an object of type `Func<T>`:

    var service = Get.Some<Service>();

    var things = dataAccess.Select<MyThing>(() => new MyThing(service));
    var someThings = dataAccess.ExecuteQuery<MyThing>(
        () => new MyThing(service),
        "SELECT TOP 5 * FROM MyThings"
    );


### DTOs with a complex graph

This could also be used in place of the factory method above for DTOs with a non-parameterless constructor. `ExecuteQuery<T>()` and `Select<T>()` have overloads that accept a tranform method, a `Func<dynamic, T>` that converts the dynamic row into a T:

    class Address { 
        public string Line1; 
        public string Line2; 
    }
    class Customer { 
        public string Name; 
        public Address BillingAddress; 
        public Address ShippingAddress; 
    }

    var customers = dataAccess.Select<Customer>(transform: r => new Customer { 
            Name = (string)r.Name,
            BillingAddress = new Address { Line1 = (string)r.BillingAddressLine1, Line2 = (string)r.BillingAddressLine2 },
            ShippingAddress = new Address { Line1 = (string)r.ShippingAddressLine1, Line2 = (string)r.ShippingAddressLine2 }
        });
        
    // or:
    Customer GetCustomer(dynamic row) { return ... }
    var customers = dataAccess.Select(GetCustomer);


### ExecuteScalar

Returns the first column of the first result of the specified SQL. Eg:

    var barsCount = dataAccess.ExecuteScalar<int>("SELECT COUNT(*) FROM THINGS WHERE Foo = @Foo", new { Foo = "bar" });
    var averageScore = ((double)dataAccess.ExecuteScalar("SELECT AVG(Age) FROM Things"));


### Any, Exists

Returns a boolean indicating if there are any rows in the specified table that satisfy the specified criteria.

    Any<T>(object where = null)
    Any(Type t, object where = null)
    Any(string tableName, object where = null)

`Exists` methods alias the corresponding `Any` methods:

    Exists<T>(object where = null)
    Exists(Type t, object where = null)
    Exists(string tableName, object where = null)



### Other methods

- `DropTable(string tableName)` drops the specified table if it exists. This is useful for automated tests.
- `TestConnection()` returns any exception thrown when opening the connection. If there are no exceptions, null is returned. This is used to test the connection without bombing out the application.
- `GetDatabaseName()` returns the name of the database, extracted from the connection string.


## Versions

### 3.0.0

- Switch to .NET 4.5.1 (breaking change)
- Async versions of all methods on IDataAccess


## Contribute

If you want to contribute to this project, start by forking the repo: <https://github.com/swxben/danny-boy>. Create an issue if applicable, create a branch in your fork, and create a pull request when it's ready. Thanks!

### Contributors

## Building, running tests and the NuGet package

THe VS2010 solution is in the root folder. Unit tests (src\swxben.dannyboy.Tests\bin\Debug\swxben.dannyboy.Tests.dll) can be run in a console using `tests.bat`. The NuGet package can be built by running `build-nuget-package.cmd`. A database named `swxben_dataaccess` running on `.\sqlexpress` with integrated access is needed.


## Version history

### 2.6.0

- Remove entity type `new()` generic constraint on query methods
- Search for a default public, protected or private constructor when attempting to instantiate a new entity type


## Licenses

All files [CC BY-SA 3.0](http://creativecommons.org/licenses/by-sa/3.0/) unless otherwise specified.

### Third party licenses

Third party libraries or resources have been included in this project under their respective licenses.




