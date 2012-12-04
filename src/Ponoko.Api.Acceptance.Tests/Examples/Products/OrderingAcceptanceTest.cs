using NUnit.Framework;
using Ponoko.Api.Core.Product;
using Ponoko.Api.Core.Shipping;

namespace Ponoko.Api.Acceptance.Tests.Examples.Products {
	public class OrderingAcceptanceTest : ProductAcceptanceTest {
		[SetUp]
		public void SetUp() {
			ExampleProduct = ExampleProduct ?? (ExampleProduct = NewProduct("A product to for making"));	
		}

		protected NameAndAddress ExampleShippingAddress {
			get { 
				return new NameAndAddress {
                  	FirstName		= "Jazz",
                  	LastName		= "Kang",
                  	LineOne			= "27 Dixon Street",
                  	LineTwo			= "Te Aro",
                  	City			= "Wellington",
                  	ZipOrPostalCode = "6021",
                  	State			= "NA",
                  	Country			= "NZ",
                  	Phone			= "Any telephone number"
				};
			}
		}

		protected Address ExampleAddress {
			get { 
				return new Address {
               		LineOne			= "27 Dixon Street",
               		LineTwo			= "Te Aro",
               		City			= "Wellington",
               		ZipOrPostalCode = "6021",
               		State			= "NA",
               		Country			= "New Zealand"
				};
			}
		}

		protected Address TenDowningStreet {
			get { 
				return new Address {
                   	LineOne			= "10 Downing Street",
                   	LineTwo			= "Westminster",
                   	City			= "London",
                   	ZipOrPostalCode = "SW1A 2AA",
                   	State			= "NA",
                   	Country			= "United Kingdom"
				};
			}
		}

		public ProductShippingInfo ExampleShippingInfo { 
			get {
				return new ProductShippingInfo {
					Key = ExampleProduct.Key,
				    Quantity = 1
				};
			}
		}

		protected Product ExampleProduct { get; set; }
	}
}