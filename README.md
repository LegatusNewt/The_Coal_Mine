# The_Coal_Mine

## Core Technologies Used

- Server: .NET Core 8 w/ Entity Framework
- DB: Postgresql w/ Npgsql & Postgis
- Client: React w/ Mapbox-gl
- Docker
- Helpful GIS Packages: NetTopologySuite, Turf.js

I haven't used C# in over 5 years so I'm certainly open to any feedback you could provide. It was a bit of a hurdle coming from Flask w/ SQLAlchemy to .NET w/ Entity Framework. I'm also sure there are other C# web service frameworks that may have been more apt for a small project like this.

## Running the Code

### Server
In order to run the server all you should have to do is run docker-compose up app, this will also run the docker-compose up postgis command as a pre-requisite.

The database migrations and seeding of data happens in automaitcally in code when in Development mode set in the appsettings.json.

There are two controllers which handle the REST endpoints

EmissionsController
- GET emissions/data -> Returns all the data from the emissions table with WKT geometry
- GET emissions/layer -> Returns all the data from emissions table as a Geojson Feature Collection

CoverageController
- GET coverages/data -> Same as above but for Coverages table
- GET coverages/layer -> Same as above but for Coverages table
- POST coveratges/data -> Takes in a CoveragePostBody ( Name, Description, Buffersize, Feature (as a geojson string)) and stores the coverage in the database Coverages table

### Client
The React web app is run by running npm install in the client directory and then npm start.


## Mapping coding exercise

The goal of this exercise is to plot the locations of each record in data.csv file on a map and save a coverage polygon.
Each point should have some visual indicator of the value. For example higher concentrations should be red and lower ones should be green.

There should be a dropdown to choose between Ethane (C2H6) and Methane (CH4).

React map gl should be used for the front end mapping visualization

Saving the coverage
	There should be an input field to set a buffer (in meters) around each point.
	Upon clicking "Save Coverage" the geometry will be saved to a database table with a timestamp, buffer size, and geom column.

Your backend should have 2 API calls
	get the data that is stored in the database (The csv file is your seed data).
	Save the coverage geom


The backend can be written in any language (C# preferred) 
Its strongly encouraged to use an ORM to generate your database table and to read the data in your API call.


Please email back a zip file with all the code and instructions to run the app locally.