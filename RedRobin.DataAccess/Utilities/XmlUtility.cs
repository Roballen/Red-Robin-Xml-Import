using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace RedRobin.DataAccess.Utilities
{

    public class XmlUtility
    {
        public static string BuildXmlText(string xmlRootName, List<string> values)
        {
            StringBuilder result = new StringBuilder();
            result.AppendFormat("<{0}>", xmlRootName);
            foreach (string value in values)
            {
                result.AppendFormat("<value>{0}</value>", value);
            }
            result.AppendFormat("</{0}>", xmlRootName);
            return result.ToString();
        }

        public static string ToString(XmlDocument xmlDocument)
        {
            string result = "";
            if (xmlDocument != null)
            {
                StringWriter writer = new StringWriter();
                xmlDocument.Save(writer);
                result = writer.ToString();
            }
            return result;
        }
    }
}
