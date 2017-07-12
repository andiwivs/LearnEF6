From NuGet Package Manager Console, with DataModel project selected:

> enable-migrations          # creates Migrations folder with basic configuration class

> add-migration Initial      # adds first migration script to Migrations folder, will be named "Initial"
                             # NOTE: add-migration only works when data model project is active startup project

> update-database -script    # generates a SQL script to create schema. Based on app.config settings, by default script generator assumes SQL server with local db

> update-database -verbose   # executes schema migrations directly based on config


Having later made changes to model classes:

> add-migration AddSomethingNew
> update-database -verbose