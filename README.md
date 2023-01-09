# Technical assessment for fullstack developer

## The purpose of the assessment is to test your ability and resourcefulness to complete a task no documentation is provided:

### Backend
* to deliver a web API to be consumed by external systems
* design a SQL Server database
* integration with other external APIs.

### Frontend
* build a front end using HTML and CSS using responsive design
* use JavaScript efficiently
* integration with external APIs using JavaScript

## Please use the following tools as part of your solution:
* Database: SQL Server (any version or edition)
* API: MVC, WebApi
* Infrastructure: Dependency Injection
* ORM: NHibernate or Entity Framework
* Testing: xUnit or NUnit
* JavaScript, jQuery, Any UI Technology you want to use
* CSS(3)
* HTML (if required)

* ***1. The Backend***
  * Using a Four Square library, hard code into the script a location/s.
  * Using the Geo information received from foursquare api, collect photos from Flickr for this/these locations
  * Save the image URL and relevant information (Description etc) into a database.
  * Ensure that if the task is run more than once, no duplicates are added to the database.
  
* ***2. The Frontend***
  * Display the results from the database in a responsive manner (If using HTML output).
  * Search the database for images based on the fields you have chosen to save with the image URLs
  * Paginate the results limiting results to 10 per page.
  
* ***3. Bonus***
  * Instead of hard coding the locations in the script/task file, try using the database to store a list of locations.
  * To go even further, use a basic authentication:
    * Allow users to create accounts and login.
    * Allow users to edit a list of locations and then display and search only images from that users location list
  * Try using a javascript based search (if using HTML output)
  * Try using whatever template or custom javascript libraries you like to wow us all (if using HTML output)

## What should be part of your solution?
* The solution should be delivered as a collection (or single) page application with all the required CSS and JavaScript files.
* Any other documentation needed to execute your solution
* SQL Server scripts to create the database and other database objects required by the solution
* Any other documentation needed to execute your solution

## How will the technical assessment be evaluated?
* The code will evaluated for clarity, design and readability.
* If any tests are part of the submitted solution, the tests will be run the test the solution. The tests will be evaluated to verify the quality of the tests.
* The solution will be opened in the Chrome browser and the functionality described above will be tests. Both positive and negative tests will be done against the solution.
* Responsive layouts will be simulated using Chrome's developer tools.
* The database scripts provided will be run against a local SQL Server database (we'll update the connection string to point to the appropriate database)
* The two endpoints will be invoked, using Postman (https://www.getpostman.com/) to test the results returned. Both positive and negative tests will be executed

## How do I submit my solution?
Send a pull request from your repository.

## Questions or comments
Please email all to ootam@iplatform.com. They will be answered within 24hours.