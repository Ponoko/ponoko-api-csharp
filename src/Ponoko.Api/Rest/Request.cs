using System;
using System.Collections.Specialized;

namespace Ponoko.Api.Rest {
    public class Request {
    	public static Request Get(Uri uri) { return Get(uri, Empty); }
        public static Request Get(Uri uri, NameValueCollection headers) {
            return new Request(RequestLine.Get(uri), headers, Payload.Empty);
        }

    	public RequestLine RequestLine { get; private set; }
    	public NameValueCollection Headers { get; private set; }
        public Payload Payload { get; private set; }
    	public String ContentType { get; set; }

        public Request(RequestLine requestLine) : this(requestLine, Empty, Payload.Empty) {}
        public Request(RequestLine requestLine, Payload payload) : this(requestLine, Empty, payload) {}
        public Request(RequestLine requestLine, NameValueCollection headers, Payload payload) {
        	RequestLine = requestLine;
        	Headers		= headers;
        	Payload		= payload;
        	ContentType = String.Empty; // TODO: perhaps this belongs with payload
        }

		private static NameValueCollection Empty {
            get { return new NameValueCollection(0); }
        }
    }
}