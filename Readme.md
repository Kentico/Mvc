# Kentico Mvc-dev

### Instructions

We want to avoid keeping sensitive information in source control. Therefore, we removed `CMSHashStringSalt` and `CMSConnectionString` keys from web.config file. These settings are now loaded from separate files (`AppConfig.config` and `ConnectionStrings.config`), which are ignored during commit and won't get in source control. After the first download of the repository you need to create these files and set the correct values for `CMSConnectionString`, `CMSHashStringSalt` and `GoogleMapsApiKey` keys. We recommend using the prepared templates.


1. Copy `src\DancingGoat\ConnectionStrings.config.template` file as `ConnectionStrings.config`.
2. Copy `src\DancingGoat\AppSettings.config.template` file as `AppSettings.config`.
3. Copy the `CMSConnectionString` connection string from the Kentico `web.config` file to the `src\DancingGoat\ConnectionStrings.config` file.
4. Copy the `CMSHashStringSalt` app setting from the Kentico `web.config` file to the `src\DancingGoat\AppSettings.config` file.
5. [Get the google maps API key](https://code.google.com/apis/console/) and enter it into `GoogleMapsApiKey` section

Repeat the same for `internal/Sandbox` application. 

:warning: Do not rename or remove `*.template` files.
