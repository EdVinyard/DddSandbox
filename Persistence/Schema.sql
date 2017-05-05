if object_id('dbo.Waypoint', 'U') is not null begin
  drop table dbo.Waypoint
  end

if object_id('dbo.Route', 'U') is not null begin
  drop table dbo.Route
  end

create table Route (
	RouteId int identity primary key,
	Label nvarchar(max) not null,
	)

create table Waypoint (
	WaypointId int identity primary key,
	RouteId int foreign key references Route (RouteId),
	[Address] nvarchar(max),
	GeoCoordinate geography not null,
	Early datetimeoffset not null,
	Late datetimeoffset not null,
	)
