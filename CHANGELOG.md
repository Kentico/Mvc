## Kentico.Activities

### 2.0.0 (2017-12-11)
#### Release notes
* This package version supports integration with Kentico 11.

### 1.0.1.1 (2017-02-08)
#### Fixed
* Added missing package dependencies.

### 1.0.1 (2017-02-01)
#### Fixed
* Updated Kentico package description for NuGet.org.

### 1.0.0 (2016-12-02)
#### Release notes
* New `Kentico.Activities` integration package that provides API for logging of Kentico activities. See [Logging activities on MVC sites](https://docs.kentico.com/x/foDFAw).
* This package version supports integration with Kentico 10.


## Kentico.Activities.Web.Mvc

### 2.0.0 (2017-12-11)
#### Release notes
* This package version supports integration with Kentico 11.

### 1.0.1.1 (2017-02-08)
#### Fixed
* Added missing package dependencies.

### 1.0.1 (2017-02-01)
#### Fixed
* Updated Kentico package description for NuGet.org.

### 1.0.0 (2016-12-02)
#### Release notes
* New `Kentico.Activities.Web.Mvc` integration package that provides API for logging of the external search and page related activities. See [Enabling tracking of activities on MVC sites](https://docs.kentico.com/x/qYLeAw).
* This package version supports integration with Kentico 10.


## Kentico.CampaignLogging.Web.Mvc

### 2.0.0 (2017-12-11)
#### Release notes
* This package version supports integration with Kentico 11.

### 1.0.1.1 (2017-02-08)
#### Fixed
* Added missing package dependencies.

### 1.0.1 (2017-02-01)
#### Fixed
* Updated Kentico package description for NuGet.org.

### 1.0.0 (2016-12-02)
#### Release notes
* New `Kentico.CampaignLogging.Web.Mvc` integration package that allows tracking of campaign conversions for pages hosted on external MVC sites. See [Tracking campaigns on MVC sites](https://docs.kentico.com/x/TqDlAw).
* This package version supports integration with Kentico 10.


## Kentico.ContactManagement

### 2.0.0 (2017-12-11)
#### Release notes
* This package version supports integration with Kentico 11.

### 1.0.1.1 (2017-02-08)
#### Fixed
* Added missing package dependencies.

### 1.0.1 (2017-02-01)
#### Fixed
* Updated Kentico package description for NuGet.org.

### 1.0.0 (2016-12-02)
#### Release notes
* New `Kentico.ContactManagement` integration package that provides an API for tracking contacts on MVC websites. See [Tracking contacts on MVC sites](https://docs.kentico.com/x/ygG9Aw).
* Improved the API for creating content personalization conditions within MVC code. See [Content personalization on MVC sites](https://docs.kentico.com/x/86HlAw).
* This package version supports integration with Kentico 10.


## Kentico.Content.Web.Mvc

### 2.0.0 (2017-12-11)
#### Release notes
* This package version supports integration with Kentico 11.

### 1.0.1.1 (2017-02-08)
#### Fixed
* Added missing package dependencies.

### 1.0.1 (2017-02-01)
#### Fixed
* Updated Kentico package description for NuGet.org.
* Fix client caching for static content when Preview module is enabled.
[#20](https://github.com/Kentico/Mvc/pull/20)

### 1.0.0 (2016-12-02)
#### Release notes
* New `Kentico.Content.Web.Mvc` integration package that provides API for working with attachments, media library files, and preview mode in MVC applications. See [Retrieving content on MVC sites](https://docs.kentico.com/x/1xyzAw).
* This package version supports integration with Kentico 10.


## Kentico.Core

### 2.0.0 (2017-12-11)
#### Release notes
* This package version supports integration with Kentico 11.

### 1.0.1.1 (2017-02-08)
#### Fixed
* Added missing package dependencies.

### 1.0.1 (2017-02-01)
#### Fixed
* Updated Kentico package description for NuGet.org.

### 1.0.0 (2016-12-02)
#### Release notes
* New `Kentico.Core` integration package that provides code used by other Kentico integration packages to help identify services and repositories suitable for management by a dependency injection container. This package is primarily intended for MVC projects using the Kentico integration packages.
* This package version supports integration with Kentico 10.


## Kentico.Ecommerce

### 2.0.0 (2017-12-11)
#### Release notes
* This package version supports integration with Kentico 11.
#### Breaking changes
* Property `Kentico.Ecommerce.ShoppingCartValidator.UserFromDifferentSite` has been removed.
* Properties and methods of `Kentico.Ecommerce.ShoppingCart` has been changed:
	* Property `System.Bool Kentico.Ecommerce.ShoppingCart.HasUsableCoupon` has been removed. Use `Kentico.Ecommerce.ShoppingCart.AppliedCouponCodes.Any()` instead.
	* Added property `System.Collections.Generic.IEnumerable<System.String> Kentico.Ecommerce.ShoppingCart.AppliedCouponCodes`.
	* Added method `System.Bool Kentico.Ecommerce.ShoppingCart.AddCouponCode(System.String)`.
	* Added method `System.Void Kentico.Ecommerce.ShoppingCart.RemoveCouponCode(System.String)`.
	* Method `System.Void Kentico.Ecommerce.ShoppingCart.ApplyCouponCode(System.String)` has been removed. Use `System.Bool Kentico.Ecommerce.ShoppingCart.AddCouponCode(System.String)`.
	* Second parameter from method `System.Void Kentico.Ecommerce.Order.SetPaymentResult(CMS.Ecommerce.PaymentResultInfo, System.Bool)` has been removed. Use property `System.Bool CMS.Ecommerce.PaymentResultInfo.PaymentIsFailed` instead.

### 1.0.1.1 (2017-02-08)
#### Fixed
* Added missing package dependencies.

### 1.0.1 (2017-02-01)
#### Fixed
* Updated Kentico package description for NuGet.org.

### 1.0.0 (2016-12-02)
#### Release notes
* New `Kentico.Ecommerce` integration package that provides API for developing and running on-line stores. See [Developing on-line stores in MVC](https://docs.kentico.com/x/-RyzAw).
* This package version supports integration with Kentico 10.


## Kentico.Glimpse

### 3.0.0 (2017-12-11)
#### Release notes
* This package version supports integration with Kentico 11.

### 2.0.0 (2016-12-02)
#### Release notes
* This package version supports integration with Kentico 10.

### 1.0.0 (2015-11-25)
#### Release notes
* New `Kentico.Glimpse` package that provides integration with [Glimpse](http://getglimpse.com/). Glimpse is a diagnostics platform that provides debug information from Kentico. See [Debugging SQL queries with Glimpse on MVC sites](https://docs.kentico.com/x/jYDFAw).
* This package version supports integration with Kentico 9.


## Kentico.MediaLibrary

### 2.0.0 (2017-12-11)
#### Release notes
* This package version supports integration with Kentico 11.

### 1.0.1.1 (2017-02-08)
#### Fixed
* Added missing package dependencies.

### 1.0.1 (2017-02-01)
#### Fixed
* Updated Kentico package description for NuGet.org.

### 1.0.0 (2016-12-02)
#### Release notes
* New `Kentico.MediaLibrary` integration package that provides API for working with media library files. See [Working with media libraries on MVC sites](https://docs.kentico.com/x/EYD5Aw).
* This package version supports integration with Kentico 10.


## Kentico.Membership

### 2.0.0 (2017-12-11)
#### Release notes
* This package version supports integration with Kentico 11.

### 1.0.1 (2017-11-27)
#### Fixed, Security
* `UserManager` no longer successfully verifies passwords for external and domain users.

### 1.0.0 (2016-12-02)
#### Release notes
* New `Kentico.Membership` integration package that provides an API for working with Kentico users and roles in MVC applications. See [Working with users on MVC sites](https://docs.kentico.com/x/Ph6zAw).
* This package version supports integration with Kentico 10.


## Kentico.Newsletters

### 3.0.0 (2017-12-11)
#### Release notes
* This package version supports integration with Kentico 11.

### 2.0.1.1 (2017-02-08)
#### Fixed
* Added missing package dependencies.

### 2.0.1 (2017-02-01)
#### Fixed
* Updated Kentico package description for NuGet.org.

### 2.0.0 (2016-12-02)
#### Release notes
* This package version supports integration with Kentico 10.
#### Breaking changes
* Methods of the newsletter subscription service (`NewsletterSubscriptionService`) were changed:
	* Removed the `Subscribe` methods accepting an email or `SubscriberInfo` parameters. Use `Subscribe(ContactInfo contact, NewsletterInfo newsletter, NewsletterSubscriptionSettings subscriptionSettings)` instead.
	* The return value of the `Subscribe` method now indicates whether a new subscription was created, or had already existed.
	* The `ConfirmSubscription` method was added to ease the use of the double opt-in feature.
	* The return type of the `Unsubscribe` and `UnsubscribeFromAll` methods is void.

### 1.0.0 (2015-11-25)
#### Release notes
* New `Kentico.Newsletter` intergration package that provides API for managing newsletter subscriptions and unsubscriptions for all types of email feeds in web applications that access Kentico externally. See [Email marketing on MVC sites](https://docs.kentico.com/x/i6HlAw).
* This package version supports integration with Kentico 9.

    
## Kentico.Newsletters.Web.Mvc
### 2.0.0 (2017-12-11)
#### Release notes
* This package version supports integration with Kentico 11.

### 1.0.1.1 (2017-02-08)
#### Fixed
* Added missing package dependencies.

### 1.0.1 (2017-02-01)
#### Fixed
* Updated Kentico package description for NuGet.org.

### 1.0.0 (2016-12-02)
#### Release notes
* New `Kentico.Newsletters.Web.Mvc` integration package that provides tracking of marketing emails on MVC sites. See [Tracking marketing emails on MVC sites](https://docs.kentico.com/x/P4PeAw).
* This package version supports integration with Kentico 10.


## Kentico.Search
### 3.0.0 (2017-12-11)
* This package version supports integration with Kentico 11.

### 2.0.1.1 (2017-02-08)
#### Fixed
* Added missing package dependencies.

### 2.0.1 (2017-02-01)
#### Fixed
* Updated Kentico package description for NuGet.org.

### 2.0.0 (2016-12-02)
#### Release notes
* This package version supports integration with Kentico 10.
#### Breaking changes
* The search result item class (`SearchResultItem`) is generic (with a constraint to the `BaseInfo` class). General fields of full-text search results are encapsulated in the `GeneralFields` property.
* The search service (`SearchService`) accepts search parameters and returns search results in the following format:
`SearchResult Search(SearchOptions options)`
	* Note: The `SearchOptions.PageNumber` property accepts one-based page numbers (i.e., numbers starting from 1 instead of 0).

### 1.0.1 (2016-02-23)
#### Fixed
* Fix searching in non-page search indexes. Search service no longer causes an exception for non-page search indexes.
[#11](https://github.com/Kentico/Mvc/pull/11)

### 1.0.0 (2015-11-25)
#### Release notes
* New `Kentico.Search` intergration package that provides API for working with the Kentico smart search in web applications that access Kentico externally. See [Providing smart search on MVC sites](https://docs.kentico.com/x/3xyzAw).
* This package version supports integration with Kentico 9.


## Kentico.Web.Mvc
### 3.0.0 (2017-12-11)
* This package version supports integration with Kentico 11.

### 2.0.0 (2016-12-02)
#### Release notes
* This package version supports integration with Kentico 10.
#### Breaking changes
* The `Kentico.Web.Mvc` integration package's logic for generating preview URLs, working with attachments, and managing media library files was transferred to a new `Kentico.Content.Web.Mvc` integration package. To continue using this functionality, you have to install the `Kentico.Content.Web.Mvc` integration package into your MVC solution.
* Attachment URL - To retrieve an attachment URL, install the `Kentico.Content.Web.Mvc` NuGet package and use the `GetPath()` method of the `Attachment` object instead of the `UrlHelper.Kentico().Attachment(attachment)` method.

### 1.0.1 (2015-12-14)
#### Fixed
* Fix an exception when invoking an asynchronous action if using global handling of the "Not found" status.
[#1](https://github.com/Kentico/Mvc/pull/1)    

### 1.0.0 (2015-11-25)
#### Release notes
* New `Kentico.Web.Mvc` package that provides integration with Kentico for ASP.NET MVC 5 applications. See [Developing sites using ASP.NET MVC](https://docs.kentico.com/x/zByzAw).
* This package version supports integration with Kentico 9.
