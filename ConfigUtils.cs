using System.Xml;

namespace State_Downloader
{
    public static class ConfigUtils
    {
        public static string GetConfigValue(string tagName)
        {
            return GetNode(tagName).InnerText;
        }

        public static string GetConfigValue(string outerTag, string childTag)
        {
            return GetNode(outerTag).SelectSingleNode(childTag).InnerText;
        }

        public static bool GetConfigBool(string tagName)
        {
            return bool.Parse(GetNode(tagName).InnerText);
        }

        public static string[] GetConfigValues(string tagName)
        {
            var children = GetNode(tagName).ChildNodes;
            string[] retVal = new string[children.Count];

            for (int i = 0; i < children.Count; i++)
            {
                retVal[i] = children[i].InnerText;
            }

            return retVal;
        }

        /// <summary>
        /// Gets the text inside the child tags which are nested in the outer tag
        /// </summary>
        /// <param name="outerTag"></param>
        /// <param name="childTag"></param>
        /// <returns></returns>
        public static string[] GetConfigValues(string outerTag, string childTag)
        {
            var children = GetNode(outerTag).SelectNodes(childTag);
            string[] retVal = new string[children.Count];

            for (int i = 0; i < children.Count; i++)
            {
                retVal[i] = children[i].InnerText;
            }

            return retVal;
        }

        private static XmlNode GetNode(string tagName)
        {
            var xmlReader = new XmlDocument();
            xmlReader.Load("Config.xml");
            return xmlReader.SelectSingleNode("ConfigOptions").SelectSingleNode(tagName);
        }

        internal static int GetConfigInt(string tagName)
        {
            return int.Parse(GetNode(tagName).InnerText);
        }
    }
}