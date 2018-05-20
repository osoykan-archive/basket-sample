using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

using Newtonsoft.Json;

namespace BasketContext.Api.IntegrationTests
{
	public static class ObjectExtensions
	{
		public static ByteArrayContent ToByteArrayContent(this object @object) => new ByteArrayContent(@object.ToByteArray());

		public static ByteArrayContent ToJsonStringContent(this object @object) => new StringContent(JsonConvert.SerializeObject(@object), Encoding.UTF8, "application/json");

		public static byte[] ToByteArray(this object @object)
		{
			var bf = new BinaryFormatter();
			using (var ms = new MemoryStream())
			{
				bf.Serialize(ms, @object);
				return ms.ToArray();
			}
		}
	}
}
