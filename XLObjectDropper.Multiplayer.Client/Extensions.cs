using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace XLObjectDropper.Multiplayer.Client
{
	public static class Extensions
	{
		// Serialize to bytes (BinaryFormatter)
		public static byte[] SerializeToBytes<T>(this T source)
		{
			using (var stream = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(stream, source);
				return stream.ToArray();
			}
		}

		// Deerialize from bytes (BinaryFormatter)
		public static T DeserializeFromBytes<T>(this byte[] source)
		{
			using (var stream = new MemoryStream(source))
			{
				var formatter = new BinaryFormatter();
				stream.Seek(0, SeekOrigin.Begin);
				return (T)formatter.Deserialize(stream);
			}
		}
	}
}
