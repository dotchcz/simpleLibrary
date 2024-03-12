# The purpose of this project
Lorem ipsum ...

# DB Migration

- in case there is no DB, you have to run the ```dotnet ef update database``` script to create a DB
- the project should contain a sample db

# Web API
- the Api contans the ```.http``` file to test some EPs
- Swagger is also working, including the ```Authorize``` functionality as almost all EPs require Authorization bearer token
## Authorization
- there is an EP providing a token to registered user, we calculate with 2 users: ```admin``` and ```reader```
- 