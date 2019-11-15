using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace State_Downloader
{
    public static class Extensions
    {
        public static int GetDaysInMonth(this Calendar c, DateTime dt)
        {
            return c.GetDaysInMonth(dt.Year, dt.Month);
        }

        public static T Clone<T>(this T source) where T : class
        {
            //if (!typeof(T).IsSerializable)
            //{
            //    throw new ArgumentException("The type must be serializable.", "source");
            //}

            // Don't serialize a null object, simply return the default for that object
            //if (source is null)
            //{
            //    return default;
            //}

            IFormatter formatter = new BinaryFormatter();

            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
