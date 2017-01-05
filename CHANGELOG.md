## Kentico.Activities

### 1.0.1 (2016-12-21)

#### Fixed

* Updated Kentico package description for NuGet.org.

### 1.0.0 (2016-12-02)

#### Release notes

* New `Kentico.Activities` integration package that provides API for logging of Kentico activities. See [Logging activities on MVC sites](https://docs.kentico.com/x/foDFAw).

## Kentico.Activities.Web.Mvc

### 1.0.1 (2016-12-21)

#### Fixed

* Updated Kentico package description for NuGet.org.

### 1.0.0 (2016-12-02)

#### Release notes

* New `Kentico.Activities.Web.Mvc` integration package that provides API for logging of the external search and page related activities. See [Enabling tracking of activities on MVC sites](https://docs.kentico.com/x/qYLeAw).

## Kentico.CampaignLogging.Web.Mvc

### 1.0.1 (2016-12-21)

#### Fixed

* Updated Kentico package description for NuGet.org.

### 1.0.0 (2016-12-02)

#### Release notes

* New `Kentico.CampaignLogging.Web.Mvc` integration package that allows tracking of campaign conversions for pages hosted on external MVC sites. See [Tracking campaigns on MVC sites](https://docs.kentico.com/x/TqDlAw).

## Kentico.ContactManagement

### 1.0.1 (2016-12-21)

#### Fixed

* Updated Kentico package description for NuGet.org.

### 1.0.0 (2016-12-02)

#### Release notes

* New `Kentico.ContactManagement` integration package that provides an API for tracking contacts on MVC websites. See [Tracking contacts on MVC sites](https://docs.kentico.com/x/ygG9Aw).
* Improved the API for creating content personalization conditions within MVC code. See [Content personalization on MVC sites](https://docs.kentico.com/x/86HlAw).

## Kentico.Content.Web.Mvc

### 1.0.1 (2016-12-21)

#### Fixed

* Updated Kentico package description for NuGet.org.

## Kentico.Core

### 1.0.1 (2016-12-21)

#### Fixed

* Updated Kentico package description for NuGet.org.

## Kentico.Ecommerce

### 1.0.1 (2016-12-21)

#### Fixed

* Updated Kentico package description for NuGet.org.

### 1.0.0 (2016-12-02)

#### Release notes

* New `Kentico.Ecommerce` integration package that provides API for developing and running on-line stores. See [Developing on-line stores in MVC](https://docs.kentico.com/x/-RyzAw).

## Kentico.MediaLibrary

### 1.0.1 (2016-12-21)

#### Fixed

* Updated Kentico package description for NuGet.org.

### 1.0.0 (2016-12-02)

#### Release notes

* New `Kentico.MediaLibrary` integration package that provides API for working with media library files. See [Working with media libraries on MVC sites](https://docs.kentico.com/x/EYD5Aw).

## Kentico.Membership

### 1.0.0 (2016-12-02)

#### Release notes

* New `Kentico.Membership` integration package that provides an API for working with Kentico users and roles in MVC applications. See [Working with users on MVC sites](https://docs.kentico.com/x/Ph6zAw).

## Kentico.Newsletter

### 2.0.1 (2016-12-21)

#### Fixed

* Updated Kentico package description for NuGet.org.

### 2.0.0 (2016-12-02)

#### Breaking changes

* Methods of the newsletter subscription service (`NewsletterSubscriptionService`) were changed:
	* Removed the `Subscribe` methods accepting an email or `SubscriberInfo` parameters. Use `Subscribe(ContactInfo contact, NewsletterInfo newsletter, NewsletterSubscriptionSettings subscriptionSettings)` instead.
	* The return value of the `Subscribe` method now indicates whether a new subscription was created, or had already existed.
	* The `ConfirmSubscription` method was added to ease the use of the double opt-in feature.
	* The return type of the `Unsubscribe` and `UnsubscribeFromAll` methods is void.

## Kentico.Newsletters.Web.Mvc

### 1.0.1 (2016-12-21)

#### Fixed

* Updated Kentico package description for NuGet.org.

### 1.0.0 (2016-12-02)

#### Release notes

* New `Kentico.Newsletters.Web.Mvc` integration package that provides tracking of marketing emails on MVC sites. See [Tracking marketing emails on MVC sites](https://docs.kentico.com/x/P4PeAw).

## Kentico.Search

### 2.0.1 (2016-12-21)

#### Fixed

* Updated Kentico package description for NuGet.org.

### 2.0.0 (2016-12-02)

#### Breaking changes

* The search result item class (`SearchResultItem`) is generic (with a constraint to the `BaseInfo` class). General fields of full-text search results are encapsulated in the `GeneralFields` property.

* The search service (`SearchService`) accepts search parameters and returns search results in the following format:
`SearchResult Search(SearchOptions options)`
	* Note: The `SearchOptions.PageNumber` property accepts one-based page numbers (i.e., numbers starting from 1 instead of 0).

### 1.0.1 (2016-02-23)

#### Fixed

- Fix searching in non-page search indexes. Search service no longer causes an exception for non-page search indexes.
[#11](https://github.com/Kentico/Mvc/pull/11)

## Kentico.Web.Mvc

### 2.0.0 (2016-12-02)

#### Breaking changes

* The `Kentico.Web.Mvc` integration package's logic for generating preview URLs, working with attachments, and managing media library files was transferred to a new `Kentico.Content.Web.Mvc` integration package. To continue using this functionality, you have to install the `Kentico.Content.Web.Mvc` integration package into your MVC solution.

### 1.0.1 (2015-12-14)

#### Fixed

- Fix an exception when invoking an asynchronous action if using global handling of the "Not found" status.
[#1](https://github.com/Kentico/Mvc/pull/1)    
