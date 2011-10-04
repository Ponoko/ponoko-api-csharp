using System;
using NUnit.Framework;
using Ponoko.Api.Core;
using Ponoko.Api.Core.Repositories;

namespace Ponoko.Api.Acceptance.Tests.Examples {
	[TestFixture]
	public class AboutMaterials : AcceptanceTest {
		private String _firstNodeKey;

		[Test]
		public void first_you_need_key_for_a_making_node() {
			var nodes = new Nodes(Internet, Settings.BaseUrl);

    		var all = nodes.FindAll();

			Assert.That(all.Count, Is.GreaterThan(0), "Expected at least one node");
		}

		[Test]
		public void can_get_the_full_catalogue_of_materials() {
			var catalogue = new MaterialsCatalogue(Internet, Settings.BaseUrl);
			var allMaterials = catalogue.FindAll(FirstNodeKey);
			Assert.That(allMaterials.Count, Is.GreaterThan(0), 
				"Expected at least some materials for the node key <{0}>.", FirstNodeKey
			);
		}

		private String FirstNodeKey {
			get { return _firstNodeKey ?? (_firstNodeKey = FindFirstNodeKey()); }
		}

		private string FindFirstNodeKey() {
			var nodes = new Nodes(Internet, Settings.BaseUrl).FindAll();
			return nodes[0].Key;
		}
	}
}
