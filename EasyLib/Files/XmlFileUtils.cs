﻿using System.Text;
using System.Xml;
using EasyLib.Json;

namespace EasyLib.Files;

public static class XmlFileUtils
{
    /// <summary>
    /// Add the log to the xml file, log must be type of LogElement
    /// </summary>
    /// <param name="xmlFilePath"></param>
    /// <param name="log"></param>
    public static void AddXmlLog(string xmlFilePath, LogElement log)
    {
        if (!File.Exists(xmlFilePath) || new FileInfo(xmlFilePath).Length <= 3)
        {
            var xmlSettings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t",
                NewLineChars = "\n",
                NewLineOnAttributes = true
            };
            var xmlWriter = XmlWriter.Create(xmlFilePath, xmlSettings);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Logs");
            xmlWriter.WriteStartElement("Log");
            xmlWriter.WriteAttributeString("JobName", log.JobName);
            xmlWriter.WriteAttributeString("TransferTime", log.TransferTime.ToString());
            xmlWriter.WriteAttributeString("SourcePath", log.SourcePath);
            xmlWriter.WriteAttributeString("DestinationPath", log.DestinationPath);
            xmlWriter.WriteAttributeString("FileSize", log.FileSize.ToString());
            xmlWriter.WriteAttributeString("CryptoTime", log.CryptoTime.ToString());
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }
        else
        {
            using var fs = new FileStream(xmlFilePath, FileMode.Open, FileAccess.ReadWrite);
            fs.Seek(-1, SeekOrigin.End);

            // Open JSON list
            var foundReturnChar = false;
            while (fs.Position > 0 && !foundReturnChar)
            {
                var c = (char)fs.ReadByte();

                switch (c)
                {
                    case '\n':
                        foundReturnChar = true;
                        break;
                    default:
                        fs.Seek(-2, SeekOrigin.Current);
                        continue;
                }
            }

            // Add comma and new line
            fs.WriteByte((byte)'\n');

            // Serialize and write object
            var logLine = "\t<Log \n\t\t JobName=\"" + log.JobName + "\" \n\t\t TransferTime=\"" + log.TransferTime +
                          "\" \n\t\t SourcePath=\"" + log.SourcePath + "\" \n\t\t DestinationPath=\"" +
                          log.DestinationPath + "\" \n\t\t FileSize=\"" + log.FileSize + "\" \n\t\t CryptoTime=\"" +
                          log.CryptoTime + "\" /> \n </Logs>";
            var bytes = Encoding.UTF8.GetBytes(logLine);
            fs.Write(bytes);

            fs.Close();
        }
    }
}
