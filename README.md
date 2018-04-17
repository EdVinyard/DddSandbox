Purpose
--------

<img src="ProjectDependencies.png" 
    style="float:right;margin-left:1em" 
    align="right"
    alt="simplified project dependencies"
    />
<!-- 
diagram source: https://drive.google.com/file/d/1YGiO0Z_0-VR1vuAwUtS0di-N9PVg1sZA/view?usp=sharing
-->

This is a "scale model" project to explore what [Domain Driven Design](https://en.wikipedia.org/wiki/Domain-driven_design) can look like in the presence of some pre-existing technology decisions:

- [StructureMap](http://structuremap.github.io/) for dependency inversion
- [Fluent](http://www.fluentnhibernate.org/) [NHibernate](http://nhibernate.info/) as an ORM
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/) as the principle data store
- [ASP.NET WebAPI](https://www.asp.net/web-api) for web services

Project directories (either do or will) contain further detail.

Installation
-------------

### SQL Server

The PersistenceTest, ApplicationTest, UserInterface, and UserInterfaceTest projects assume that 

1. SQL Server is installed and running locally,
1. a database exists with the name `DddExample`, and
1. the current user can connect using Windows Authentication

This is the connection string used by those projects `app.config` and `web.config` files:

    Server=.;Database=DddExample;Trusted_Connection=True;

### Test Runner

I've had mixed luck with the NUnit adapter for Visual Studio (specifically with test discovery) so the [RunAllTests.ps1](RunAllTests.ps1) script in the root directory relies instead on the [NUnit 3 Console runner](https://github.com/nunit/nunit-console), which I installed using Chocolatey:

    choco install nunit-console-runner
