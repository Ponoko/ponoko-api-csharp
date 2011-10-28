using System;

namespace Ponoko.Api.Rest.Sugar {
	public static class un {
		public static void less(Boolean condition, Action thenDoThis) {
			un.less(() => condition, thenDoThis);
		}

		public static void less(Func<bool> condition, Action thenDoThis) {
			if (condition() == false) {
				thenDoThis();
			}
		}
	}
}