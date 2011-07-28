using System;

namespace Ponoko.Api.Rest.Mime {
	public interface HttpContentType {
		Body Format(Payload payload);
		String ContentType { get; }
	}
}