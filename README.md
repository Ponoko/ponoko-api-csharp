## Getting Started

To get the acceptance tests to pass, you will need your Ponoko API consumer credential and a valid access token. 
Place them in the spaces provided in Ponoko.Api.Acceptance.Tests/App.example.config, and rename it to App.config.

## Examples/How-to

There is a fully-executable specification in *Ponoko.Api.Acceptance.Tests.Examples*.

##Known issues

###Four failing tests

These are mainly related to deletes not returning the correct result:

1. AboutCreatingProducts you_must_supply_a_material_with_the_design
1. AboutDeletingProducts can_delete_a_product
1. AboutUpdatingProducts you_can_delete_a_design
1. AboutUpdatingProducts you_cannot_delete_the_last_design  
