using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml;

namespace XmlLoadAndGenerationFlatFile
{
    public class Xml
    {
        /// <summary>
        /// Save data xml
        /// </summary>
        /// <param name="IClass"></param>
        /// <param name="filename"></param>
        public static void SaveData(object IClass, string filename)
        {
            StreamWriter writer = null;
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer((IClass.GetType()));
                writer = new StreamWriter(filename);
                xmlSerializer.Serialize(writer, IClass);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
                writer = null;
            }
        }

        /// <summary>
        /// Read and load xml version 1
        /// </summary>
        /// <param name="nodeList"></param>
        /// <returns>scheme</returns>
        public static string GetChild(XmlNodeList nodeList)
        {
            StringBuilder str = new StringBuilder();
            foreach (XmlNode node in nodeList)
            {
                switch (node.NodeType)
                {
                    case XmlNodeType.XmlDeclaration:
                        str.Append("XML Declaration:");
                        str.AppendLine(node.Name + " " + node.InnerText);
                        break;
                    case XmlNodeType.Element:
                        str.Append("Element:");
                        str.AppendLine(node.InnerText);
                        break;
                    case XmlNodeType.Text:
                        str.AppendLine(node.Value);
                        break;
                    case XmlNodeType.Comment:
                        str.Append("Comment: ");
                        str.AppendLine(node.Value);
                        break;
                }
                if (node.Attributes != null)
                {
                    foreach (XmlAttribute atr in node.Attributes)
                    {
                        str.AppendLine(atr.Name + " " + atr.Value);
                    }
                }
                if (node.HasChildNodes)
                {
                    str.AppendLine(GetChild(node.ChildNodes));
                }
            }
            return str.ToString();
        }

        /// <summary>
        /// Read and load xml version 2
        /// </summary>
        /// <param name="navigator"></param>
        /// <returns>scheme</returns>
        public static string GetXmlRead(XPathNavigator navigator)
        {

            StringBuilder str = new StringBuilder();
            
            switch (navigator.NodeType)
            {
                case XPathNodeType.Element:
                    str.AppendLine(navigator.Name);
                    break;
                case XPathNodeType.Text:
                    str.AppendLine(navigator.Value);
                    break;
                case XPathNodeType.Comment:
                    str.AppendLine(navigator.Value);
                    break;
            }
            if (navigator.HasAttributes)
            {
                while (navigator.MoveToFirstAttribute())
                {
                    str.AppendLine(navigator.Name + " " + navigator.Value);
                }
                navigator.MoveToParent();
            }
            if (navigator.HasChildren)
            {
                navigator.MoveToFirstChild();
                do
                {
                    str.AppendLine(GetXmlRead(navigator));
                } while (navigator.MoveToNext());
                navigator.MoveToParent();
            }
            return str.ToString();
        }

        /// <summary>
        /// Load xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class XmlLoad<T>
        {
            public static Type type;

            public XmlLoad()
            {
                type = typeof(T);
            }

            public T LoadData(string filename)
            {
                T result;
                XmlSerializer xmlserializer = new XmlSerializer(type);
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                result = (T)xmlserializer.Deserialize(fs);
                fs.Close();
                return result;
            }
        }
    }

}