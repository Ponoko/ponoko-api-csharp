using System;
using OAuth.Net.Common;
using OAuth.Net.Components;
using Ponoko.Api.Rest.Security.OAuth.Core;
using Ponoko.Api.Rest.Sugar;

namespace Ponoko.Api.Rest.Security.OAuth.Impl.OAuth.Net {
    public class MadgexSignature {
        public String Sign(
            Request request,
            String consumerSecret,
            String tokenSecret,
            OAuthParameters parameters
        ) {
        	var baseString = ToBaseString(request, parameters);

        	return new HmacSha1SigningProvider().ComputeSignature(baseString, consumerSecret, tokenSecret);
        }

    	private string ToBaseString(Request request, OAuthParameters parameters) {
    		if (String.IsNullOrEmpty(request.ContentType))
    			throw new ArgumentException(
    				"It should really specify the content type otherwise we can't " + 
					"decide whether or not to include parameters in signature.", "request"
				);
			
			// TODO: Perhaps this class should be collecting the parameters from the URI
			//
			// http://oauth.net/core/1.0/#anchor14 9.1.1
			// The request parameters are collected, sorted and concatenated into a normalized string:
			//
			// Parameters in the OAuth HTTP Authorization header excluding the realm parameter.
			// Parameters in the HTTP POST request body (with a content-type of application/x-www-form-urlencoded).
			// HTTP GET parameters added to the URLs in the query part (as defined by [RFC3986] section 3).

			// [2011-07-08, BJB]: This implies we ought to be collecting them all. 
			// The request contains all of this stuff - it's just that we're treating URI and
			// parameters separately further up.

			// You can tell if there is a body or not by checking the existence of Content-length -- but our
			// requests are not fully-formed enough -- we don't have concept of body.
    		un.less(() => ParameterInclusionPolicy.IncludeParameters(request), () =>  
				parameters.AdditionalParameters.Clear()
    		);

    		return SignatureBase.Create(request.RequestLine.Verb, request.RequestLine.Uri, parameters);
    	}
    }
}