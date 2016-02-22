# Kentico MVC

[![Build status](https://ci.appveyor.com/api/projects/status/59lkg4bmkiwtoa6x/branch/master?svg=true)](https://ci.appveyor.com/project/kentico/mvc/branch/master)

[Kentico 9](https://docs.kentico.com/display/K9/) brings support for the development of [ASP.NET MVC 5 applications](https://docs.kentico.com/display/K9/Developing+sites+using+ASP.NET+MVC). This repository contains source code for Kentico integration packages and a sample web application _Dancing Goat_ that demonstrates all supported features related to the MVC development with Kentico. To find out more about the design of the sample web application, see the [How it works](https://github.com/Kentico/Mvc/wiki/how-it-works) page in the [Wiki](https://github.com/Kentico/Mvc/wiki). The page contains explanations of the decisions made when creating the application and can help you when building your own projects.

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
3. [Enable](https://docs.kentico.com/display/K9/Configuring+web+farm+servers#Configuringwebfarmservers-Configuringwebfarmsautomatically) web farm in the automatic mode.
4. Rename the `src\DancingGoat\ConnectionStrings.Template.config` file to `ConnectionStrings.config`.
5. Rename the `src\DancingGoat\AppSettings.Template.config` file to `AppSettings.config`.
6. Copy the `CMSConnectionString` connection string from the Kentico `web.config` file to the `src\DancingGoat\ConnectionStrings.config` file.
7. Copy the `CMSHashStringSalt` app setting from the Kentico `web.config` file to the `src\DancingGoat\AppSettings.config` file.
8. Open the `KenticoMvc` solution in Visual Studio and run the Dancing Goat web application.
9. Open the Smart search application and rebuild the `Dancing Goat MVC` search index. 

**Note:** The initial build can take a little longer as it needs to restore NuGet packages.
