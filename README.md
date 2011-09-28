## Getting Started

To get the acceptance tests to pass, you will need: 

* your consumer credential and 
* a valid access token
* and a base url for the version of the API you're connecting to

Place them in the spaces provided in __Ponoko.Api.Acceptance.Tests/App.example.config__, and rename it to __App.config__.

## Examples/How-to

There is a fully-executable specification in __Ponoko.Api.Acceptance.Tests.Examples__.

### Key Abstractions

#### Internet
The domain repositories require access to the internet. 
There is a default implementation (SystemInternet) which uses System.Net classes.

#### AuthorizationPolicy
SystemInternet requires an AuthorizationPolicy, and there is a default one of those too (OAuthAuthorizationPolicy), 
see below for how to create one.

For example, here is how to get the materials catalogue:

	// You need some credentials 
	var consumer = new Credential("your_consumer_key", "your_consumer_secret");
	var token = new Credential("your_token_key", "your_token_secret");
	var credentials = new CredentialSet(consumer, token);
	
	// and an authorization policy
	var authPolicy = new OAuthAuthorizationPolicy(
		new MadgexOAuthHeader(new SystemClock(), new SystemNonceFactory()),
		credentials
	);

	// which is used by the Internet
	var theInternet = new SystemInternet(authPolicy);
	
	var baseUrl = "https://sandbox.ponoko.com/services/api/v2";
	
	// To get a catalogue, first you need a Node			
	var nodes = new Nodes(theInternet, baseUrl);
	var all = nodes.FindAll();
	var firstNode = all[0];
	
	// then you can get the catalogue
	var catalogue = new MaterialsCatalogue(theInternet, baseUrl);
	var allMaterials = catalogue.FindAll(firstNode);			      

## Known issues

### One failing test

-1. AboutDesignImages you_get_an_error_if_you_supply_incorrect_content_type

## Previewing readme

    $ rake -s preview_github_readme[README.md] > readme.html