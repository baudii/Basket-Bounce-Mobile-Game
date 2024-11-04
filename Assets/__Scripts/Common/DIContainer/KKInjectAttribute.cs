using System;

namespace KK.Common
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method)]
	public class KKInjectAttribute : Attribute
	{
		public string Tag;
		public KKInjectAttribute(string tag = null)
		{
			Tag = tag;
		}
	}
}
