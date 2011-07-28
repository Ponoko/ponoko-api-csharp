using System;
using System.Net;
using NUnit.Framework;
using Ponoko.Api.Core;
using Ponoko.Api.Json;
using Ponoko.Api.Rest;
using Ponoko.Api.Rest.Security.OAuth.Core;
using Ponoko.Api.Rest.Security.OAuth.Http;
using Ponoko.Api.Rest.Security.OAuth.Impl.OAuth.Net;

namespace Ponoko.Api.Acceptance.Tests.Examples {
	[TestFixture]
	public class AboutMaterials : AcceptanceTest {
		private String _firstNodeKey;

		[Test]
		public void first_you_need_key_for_a_making_node() {
			var uri = Map("/nodes");
			
			using (var response = Get(uri)) {
				var json = new Deserializer().Deserialize(Body(response));

				var theNodeKey = json["nodes"].First.Value<String>("key");
				
				Assert.IsNotEmpty(theNodeKey, "Expected at least one node key.");
			}
		}

		[Test]
		public void can_get_the_full_catalogue_of_materials() {
			var authorizationPolicy = new OAuthAuthorizationPolicy(
				new MadgexOAuthHeader(new SystemClock(), new SystemNonceFactory()),
				Settings.Credentials
			);
			
			var theInternet = new SystemInternet(authorizationPolicy);
			var catalogue = new MaterialsCatalogue(theInternet, Settings.BaseUrl);
			
			var all = catalogue.FindAll(FirstNodeKey);

			foreach (var material in all) {
				Console.WriteLine("{0}, {1}, {2}", material.Name, material.Key, material.Type);
			}

			Assert.Greater(all.Count, 0, "Expected at least some materials for the node key <{0}>.", FirstNodeKey);
		}

		private String FirstNodeKey {
			get { return _firstNodeKey ?? (_firstNodeKey = FindFirstNodeKey()); }
		}

		private string FindFirstNodeKey() {
			var uri = Map("/nodes");

			using (var response = Get(uri)) {
				var json = new Deserializer().Deserialize(Body(response));

				return json["nodes"].First.Value<String>("key");
			}
		}
	}
}
