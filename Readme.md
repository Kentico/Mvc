# Kentico MVC

[![Build status](https://ci.appveyor.com/api/projects/status/59lkg4bmkiwtoa6x/branch/master?svg=true)](https://ci.appveyor.com/project/kentico/mvc/branch/master) [![Q&As](https://img.shields.io/badge/chat-on%20DevNet-orange.svg)](https://devnet.kentico.com/questions-answers)

[Kentico 11](https://docs.kentico.com/k11/) brings support for the development of [ASP.NET MVC 5 applications](https://docs.kentico.com/x/PQgbB). This repository contains source code for Kentico integration packages and a sample _Dancing Goat_ web application  that demonstrates all supported features related to the MVC development with Kentico. To find out more about the design of the sample web application, see the [How the Dancing Goat web application works](https://github.com/Kentico/Mvc/wiki/How-Dancing-Goat-web-application-works) page in the [Wiki](https://github.com/Kentico/Mvc/wiki). The page contains explanations of the decisions made when creating the application and can help you when building your own projects.

## Repository structure

The repository consists of projects representing integration packages specified in the [Kentico documentation](https://docs.kentico.com/x/tAgbB).

There are also the **DancingGoat** and **LearningKit** projects. Both are sample ASP.NET MVC 5 applications built using Kentico. The Dancing Goat site represents a real-life website suitable for exploring the full MVC experience.

The Learning Kit is a functional website for learning purposes. It demonstrates how to implement various Kentico features on MVC websites in the form of code snippets, which you can run if you connect the website to a Kentico database. Also, the project source code is used in documentation examples.

## Accessing older versions of the repository

You can find the MVC repository for older versions of Kentico under the [Releases](https://github.com/Kentico/Mvc/releases) section (open the corresponding commit for the required version and click **Browse files**).

Quick links:

- [Kentico 10 (latest hotfix)](https://github.com/Kentico/Mvc/tree/2a6e48b1e4a3e27da562becccec4d9a584ef5a0c)
- [Kentico 9 (latest hotfix)](https://github.com/Kentico/Mvc/tree/29c7a7ca9c99e80f4a34c267a912ff348002a775)

## Running sample sites

### Requirements

- IIS 7.5+
- Microsoft .NET Framework 4.6/4.7
- Visual Studio 2015 Update 1+

### Instructions &ndash; Dancing Goat

1. [Install](https://docs.kentico.com/x/SREbB) Kentico.
2. [Import](https://docs.kentico.com/x/bg0bB) the Dancing Goat site from the `webtemplates/DancingGoatMvc.zip` import package.
3. [Enable web farms](https://docs.kentico.com/k11/configuring-kentico/optimizing-website-performance/setting-up-web-farms/configuring-web-farm-servers#Configuringwebfarmservers-Configuringwebfarmsautomatically) in automatic mode.
4. Rename the `src\DancingGoat\ConnectionStrings.config.template` file to `ConnectionStrings.config`.
5. Rename the `src\DancingGoat\AppSettings.config.template` file to `AppSettings.config`.
6. Copy the `CMSConnectionString` connection string from the Kentico `web.config` file to the `src\DancingGoat\ConnectionStrings.config` file.
7. Copy the `CMSHashStringSalt` app setting from the Kentico `web.config` file to the `src\DancingGoat\AppSettings.config` file.
8. Open the `KenticoMvc` solution in Visual Studio and run the src\Dancing Goat web application.
9. (Optional) Open the **Smart search** application and rebuild the `Dancing Goat MVC` search index.

**Note:** The initial build can take a little longer as it needs to restore NuGet packages.

### Instructions &ndash; Learning Kit

1. [Install](https://docs.kentico.com/x/SREbB) Kentico.
2. [Create a new site](https://docs.kentico.com/x/ow0bB) in the **Sites** application based on the **MVC Blank Site** web template.
3. [Enable web farms](https://docs.kentico.com/k11/configuring-kentico/optimizing-website-performance/setting-up-web-farms/configuring-web-farm-servers#Configuringwebfarmservers-Configuringwebfarmsautomatically) in automatic mode.
4. Rename the `samples\LearningKit\ConnectionStrings.config.template` file to `ConnectionStrings.config`.
5. Rename the `samples\LearningKit\AppSettings.config.template` file to `AppSettings.config`.
6. Copy the `CMSConnectionString` connection string from the Kentico `web.config` file to the `samples\LearningKit\ConnectionStrings.config` file.
7. Copy the `CMSHashStringSalt` app setting from the Kentico `web.config` file to the `samples\LearningKit\AppSettings.config` file.
8. Open the `KenticoMvc` solution in Visual Studio and run the samples\LearningKit web application.

## Feedback & Contributing
Check out the [contributing](https://github.com/Kentico/Mvc/blob/master/CONTRIBUTING.md) page to see the best places to file issues, start discussions and begin contributing.
