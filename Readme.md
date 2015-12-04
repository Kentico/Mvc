# Kentico MVC

Starting from [Kentico 9](https://docs.kentico.com/display/K9/) development of ASP.NET MVC 5 applications is [supported](https://docs.kentico.com/display/K9/Developing+sites+using+ASP.NET+MVC). This repository contains a sample web application `Dancing Goat` that demonstrates all features related to MVC development support. We hope it helps you when building your projects.

## Repository structure

The repository consists of the following projects:

- **DancingGoat** - sample ASP.NET MVC 5 application built with Kentico.
- **Kentico.Web.Mvc** - provides features for development of ASP.NET MVC 5 applications with Kentico:
  - [Localization of validation rules](https://docs.kentico.com/display/K9/Localizing+data+on+MVC+sites). 
  - [Support of Kentico preview mode](https://docs.kentico.com/display/K9/Adding+preview+mode+support+for+MVC+sites). 
  - [Global handling of 404 (Not Found) HTTP status](https://docs.kentico.com/display/K9/Handling+404+Not+Found+globally+in+MVC+applications).
- **Kentico.Search** - provides an improved API for Kentico smart search and [working with search results](https://docs.kentico.com/display/K9/Providing+smart+search+on+MVC+sites). 
- **Kentico.Newsletters** - provides an improved API for management of [Kentico newsletter](https://docs.kentico.com/display/K9/Email+marketing) subscriptions.  
- **Kentico.Glimpse** - provides Kentico [SQL debug information](https://docs.kentico.com/display/K9/Debugging#Debugging-SQLqueries) via the [Glimpse](http://getglimpse.com/) web diagnostic platform.

## Running Dancing Goat

### Requirements

- IIS 7.5+
- Microsoft .NET Framework 4.5+
- Visual Studio 2013+

### Instructions

1. [Install](https://docs.kentico.com/display/K9/Installation) Kentico.
2. [Import](https://docs.kentico.com/display/K9/Importing+a+site+or+objects) the Dancing Goat site from the `webtemplates/DancingGoatMvc.zip` import package.
3. Rename the `src\DancingGoat\ConnectionStrings.Template.config` file to `ConnectionStrings.config`.
4. Rename the `src\DancingGoat\AppSettings.Template.config` file to `AppSettings.config`.
5. Copy the `CMSConnectionString` connection string from the Kentico `web.config` file to the `src\DancingGoat\ConnectionStrings.config` file.
6. Copy the `CMSHashStringSalt` app setting from the Kentico `web.config` file to the `src\DancingGoat\AppSettings.config` file.
7. Open the `KenticoMvc` solution in Visual Studio and run the Dancing Goat web application.
8. Open the Smart search application and rebuild the `Dancing Goat MVC` search index. 

**Note:** The initial build can take a little longer as it needs to restore NuGet packages.