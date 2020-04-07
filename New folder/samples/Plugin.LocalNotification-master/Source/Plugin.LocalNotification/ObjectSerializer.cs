﻿using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Plugin.LocalNotification
{
    /// <summary>
    ///
    /// </summary>
    public static class ObjectSerializer
    {
        /// <summary>
        /// Deserialize Returning Data.
        /// </summary>
        /// <param name="returningData"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string returningData)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            using var stringReader = new StringReader(returningData);
            using var xmlReader = XmlReader.Create(stringReader,
                new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit
                });
            var notification = (T)xmlSerializer.Deserialize(xmlReader);
            return notification;
        }

        /// <summary>
        /// Serialize Returning Data
        /// </summary>
        /// <param name="returningData"></param>
        /// <returns></returns>
        public static string SerializeObject<T>(T returningData)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            using var stringWriter = new StringWriter();
            xmlSerializer.Serialize(stringWriter, returningData);
            return stringWriter.ToString();
        }
    }
}