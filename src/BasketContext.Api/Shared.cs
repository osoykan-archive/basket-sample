using System.Text;

namespace BasketContext.Api
{
	public static class Shared
	{
		public static byte[] Key() => Encoding.UTF8.GetBytes("This is my secret key!");
	}
}