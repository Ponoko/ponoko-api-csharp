using System;
using System.Collections.Specialized;
using Ponoko.Net.Rest;

namespace Ponoko.Net.Security.OAuth.Core {
    public class Request {
    	public static Request Get(Uri uri) { return Get(uri, Empty); }
    	public static Request Get(Uri uri, NameValueCollection parameters) { return Get(uri, Empty, parameters); }
        public static Request Get(Uri uri, NameValueCollection headers, NameValueCollection parameters) {
            return new Request(RequestLine.Get(uri), headers, new Payload(parameters));
        }

    	public RequestLine RequestLine { get; private set; }
    	public NameValueCollection Headers { get; private set; }
    	public Boolean HasAnyParameters { get { return Payload.Parameters.Count > 0; } }
        public Payload Payload { get; private set; }
    	public String ContentType { get; set; }

        public Request(RequestLine requestLine) : this(requestLine, Empty, Rest.Payload.Empty) {}
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

	// TODO: Introduce RequestBody which represents the body of a request.
	// There is also the concept of encoding which may be multipart-form or atom or xml perhaps.
}