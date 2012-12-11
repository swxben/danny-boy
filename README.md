swxben.dataaccess
=================

Simple data access layer for SQL Server


## Installation

Install the engine via [NuGet](http://nuget.org/packages/swxben.dataaccess), either in Visual Studio (right-click project, Manage NuGet Packages, search for docxtemplateengine) or via the package manager console using `Install-Package swxben.dataaccess`.


## Usage

Create a new `DataAccess` instance and pass in the database connection string (do this with your favourite IoC container):

    IDataAccess dataAccess = new DataAccess(@"Server=.\sqlexpress; Database=swxben_dataaccess; User Id=sa; Password=test;");


### Insert

Insert using strongly typed DTOs. The table name is by convention the type name plus `s`.

    dataAccess.Insert(new MyThing { MyId = 13, MyName = "Ben" });

This roughly corresponds to:

    INSERT INTO MyThings(MyId, MyName) VALUES(13, 'Ben')

It actually uses named fields so SQL injection shouldn't be possible in normal use.


### Update

Update needs the field name for the identifier, rather than using a strict convention:

    dataAccess.Update(new MyThing { MyId = 13, MyName = "Ben"}, "MyId");

    UPDATE MyThings SET MyName = 'Ben' WHERE MyId = 13


### Select

The select method uses a strongly typed DTO:

	var myThings = dataAccess.Select<MyThing>(
		where: new { MyName = "Ben" },
		orderBy: "MyId"
	);

    SELECT * FROM MyThings WHERE MyName = 'Ben' ORDER BY MyId


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

### Other methods

- `DropTable(string tableName)` drops the specified table if it exists. This is useful for automated tests.
- `TestConnection()` returns any exception thrown when opening the connection. If there are no exceptions, null is returned. This is used to test the connection without bombing out the application.



## Contribute

If you want to contribute to this project, start by forking the repo: <https://github.com/swxben/swxben.dataaccess>. Create an issue if applicable, create a branch in your fork, and create a pull request when it's ready. Thanks!


## Building, running tests and the NuGet package

THe VS2010 solution is in the root folder. Unit tests (src\Swxben.DataAccess.Tests\bin\Debug\Swxben.DataAccess.Tests.dll) can be run in a console using `tests.bat`. The NuGet package can be built by running `build-nuget-package.cmd`. A database named `swxben_dataaccess` running on `.\sqlexpress` is needed.


## Licenses

All files [CC BY-SA 3.0](http://creativecommons.org/licenses/by-sa/3.0/) unless otherwise specified.

### Third party licenses

Third party libraries or resources have been included in this project under their respective licenses.


## Contributors


