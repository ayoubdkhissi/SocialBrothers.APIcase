# SocialBrothers.APIcase
Ayoub DKHISSI - .NET/C# API Case, an assignment to show back-end skills for traineeship at Social Brothers.

## overview
This project is a Web REST API built with (C#/.NET) used to manage Addrresses, It gives the possibility to Get, Add, Update, Delete an address, search, filter and sort addresses, and also calculating the distance (in Km) between two previously added addresses by using another Geolocation API.

## How to start and test the project.
1. Clone the repository.
2. Open the solution with Visual Studio.
3. Make sure that the project **SocialBrothers.APIcase.Presentation** is the startup project.
4. Run the Project.

Swagger UI will be opened showing a list of all the end points and **Full Documentation** of the API, and you can test each end-point using Swagger UI (or you can use Postman).
![image](https://user-images.githubusercontent.com/73041562/208065882-2456e880-fa32-40cb-9b8b-b399ead37532.png)

* PS: SQLite is used as DBMS, the database.db file is in the API presentation projet, this database file is generated (and seeded with some buffer data) automatically after you run the project.


## Architecture
I have used a light-weigth version of clean architecture with only 3 layers Domain(core), Infrastructure, and Presentation.
* Domain: contains entities defining the shchema, Interfaces defining services(Repository Service Interface), and also other common classes used by all layers
* Infrastructure: contains the Db context and the implementation of the repository pattern
* Presentation: contains the WEB API (Controllers, Profiles, Validators, DTOs)


## Part 1: API General CRUD

* All controller actions are asynchrounous, because they use data access (through the injected repository). data access methods should ll be asynchrounous because we always need to free and offload work from the main thread.
* I have created a DTO of the Address entity to be used in the post and update method, and for mapping the DTO to an entity I have Used **AutoMapper**
* **Validation**: For validating requests (DTOs) adn entities, I have used **Fluent Validation**, I could have used attributes/annotations, but they break the rule of separation of concerns, because they introduce validation logic in entities, and also fluent validation gives you more options and flexibility.
*  Every action is well commented and documented. I tried to make the code as clean as possible, and I followed all naming conventions of C#/.NET.

## Part 2: Filters
* For filtering, searching, pagination and sorting, I was going to go with OData package, because it is the best and most safe solution out there, but after alot of thinking, I decided to implement the feature from scratch, 'cause why not?
The challenge was to make the search and the sort dynamic, without the use of if statements, switches and larg LINQ queries, so even if we add extra properties in the Address entity, the feature will still work. First I decided to build expression trees dynamically and then pass them to LINQ functions (where and orderBy) but it tured out it is very complex and might generate errors.
Then, I decided to use reflections to inspect the Address object and then build an SQL query using a string builder (using FromSqlRaw method provided by EFCore), it is always a bad idea to use RAW SQL, but it is simpler than building Expression Trees dynamically. The code for this is well commented. Please read the code in the Repository to know more about the approach I took. 

PS: For a production senario I wouls definetly go with OData.

## Part 3: Distances
* For calculating distances, I have used an external geolocation API (https://distancematrix.ai/). it is the most simple API I could find, It takes two addresses and then returns the distance between the two addresses as well as additional information. I could have used an API that retuns coordinates of the two addresses then do some calculations, but wyh would I intrduce additional calculations in my server and waste resources? while I could delegate the task to another API.
* The URL and The API key to access this external API are defined as constants in the project.  
* To test this method I needed to get real addresses. I got them from here : https://gist.github.com/m4ll0k/11f40f41fac6277dd5a7c57367094873.

## Used Design Patterns 
* Repository pattern for data access
* Dependency Injection: all used services in the cotroller are injected (mapper, validator, repository, logger, httpClient, DbContext...)
* Solid Principles are well respected
* I could have used a Method Factory pattern to create Address objects, but that would only add more complexity, so I decided to Keep It Simple Stupid (KISS). 

## Unit Testing
* For unit testing I have used **XUnit** as a testing tool, **FakeItEasy** for mocking dependencies and method calls, and **Fluent assertions** to make assertions more readable. 
* Full test of the Contorller: I have introduced multiple unit tests for each controller action and verified the return result for different cases
* There was really not much testing to do except for the controller (there is no much Business Logic)
* I didn't introduced tests for the repository service, because it is straigth forward EF Core functionalities, it is pointless to test them.
* I wanted to test the Distances action but I couldn't, because it is a little bit more complex. 

## Notes
* Logging: I have used **SeriLog**, and configured the logger to log to Console and also a file. in a real project, I would also use **Seq**, for troublshoting and visualisation of log data. The minimum Log level is set to Information because we still in developmenet, we can set the minimum level to Warning in production.
* Authentification: I could have added some sort of authentification using JWT. but this was not requested.
* The Parts I am proud of are: the respect of clean architecture, design patterns, clean code and documentation, and The Sorting, Searching and Filtering method.

