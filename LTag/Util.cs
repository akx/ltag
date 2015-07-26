using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTag
{
	static class Util
	{
		public static void Dispose<T>(ref T disposable) where T: IDisposable
		{
			if (disposable != null)
			{
				disposable.Dispose();
				disposable = default(T);
			}
		}
	}
}
