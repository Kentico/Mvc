# Kentico MVC

[Kentico 10](https://docs.kentico.com/display/K10/) brings support for the development of [ASP.NET MVC 5 applications](https://docs.kentico.com/display/K10/Developing+sites+using+ASP.NET+MVC). This repository contains source code for Kentico integration packages and a sample web application _Dancing Goat_ that demonstrates all supported features related to the MVC development with Kentico. To find out more about the design of the sample web application, see the [How it works](https://github.com/Kentico/Mvc/wiki/how-it-works) page in the [Wiki](https://github.com/Kentico/Mvc/wiki). The page contains explanations of the decisions made when creating the application and can help you when building your own projects.

## Running Dancing Goat

### Requirements

- IIS 7.5+
- Microsoft .NET Framework 4.5+
- Visual Studio 2015 Update 1+

### Instructions

1. [Install](https://docs.kentico.com/display/K10/Installation) Kentico.
2. [Import](https://docs.kentico.com/display/K10/Importing+a+site+or+objects) the Dancing Goat site from the `webtemplates/DancingGoatMvc.zip` import package.
3. [Enable](https://docs.kentico.com/display/K10/Configuring+web+farm+servers#Configuringwebfarmservers-Configuringwebfarmsautomatically) web farm in the automatic mode.
4. Rename the `src\DancingGoat\ConnectionStrings.Template.config` file to `ConnectionStrings.config`.
5. Rename the `src\DancingGoat\AppSettings.Template.config` file to `AppSettings.config`.
6. Copy the `CMSConnectionString` connection string from the Kentico `web.config` file to the `src\DancingGoat\ConnectionStrings.config` file.
7. Copy the `CMSHashStringSalt` app setting from the Kentico `web.config` file to the `src\DancingGoat\AppSettings.config` file.
8. Open the `KenticoMvc` solution in Visual Studio and run the Dancing Goat web application.
9. (Optional) Open the Smart search application and rebuild the `Dancing Goat MVC` search index. 

**Note:** The initial build can take a little longer as it needs to restore NuGet packages.
