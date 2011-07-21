using System;

namespace Ponoko.Api.Sugar {
	public static class un {
		public static void less(Boolean condition, Action thenWhat) {
			un.less(() => condition, thenWhat);
		}

		public static void less(Func<bool> condition, Action thenWhat) {
			if (condition() == false) {
				thenWhat();
			}
		}
	}
}