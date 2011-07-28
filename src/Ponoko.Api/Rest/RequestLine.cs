using System;
using System.Collections.Generic;

namespace Ponoko.Api.Rest {
	public class RequestLine {
		public String Verb		{ get; private set; }
		public Uri Uri			{ get; private set; }
		public Decimal Version	{ get; private set; }

		public static RequestLine Head(Uri uri) { return new RequestLine("HEAD", uri); }
		public static RequestLine Get(Uri uri)	{ return new RequestLine("GET", uri); }
		public static RequestLine Post(Uri uri) { return new RequestLine("POST", uri); }

		public RequestLine(String method, Uri uri) : this(method, uri, Convert.ToDecimal(1.0d)) {}
		public RequestLine(String method, Uri uri, Decimal version) {
			Verb	= method;
			Uri		= uri;
			Version = version;
		}

		public List<Parameter> Parameters {
			get {
				var temp = System.Web.HttpUtility.ParseQueryString(Uri.Query);
				var result = new List<Parameter>(temp.Count);
				
				foreach (var key in temp.AllKeys) {
					foreach (var value in temp[key].Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries)) {
						result.Add(new Parameter {Name = key, Value = value});	
					}
				}

				return result;
			}
		}
	}
}