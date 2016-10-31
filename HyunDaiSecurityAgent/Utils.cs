﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Xml;


namespace HyunDaiSecurityAgent
{
    class Utils
    {
        private const int RetryCountMax = 1000;
        private const int SleepDuration = 1000; //1 second
        private static EventLog _localLog = LogManager.getLocalLog();

        // 다른 프로세스에서 사용하고 있는 지 체크 (이벤트 핸들러 trigger가 너무 빨라서 변경 완료 하기 전보다 먼저 실행 되어 다른 프로세스가 파일을 사용중이라는 오류 발생)
        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }

        public static String getUUID() {
            // config 파일에 uuid 항목이 없을 경우 새로 생성해서 파일에 쓰기
            Assembly assembly = Assembly.GetExecutingAssembly();
            XmlDocument doc = new XmlDocument();
            doc.Load(ConfigManager.getConfigFilePath());

            XmlNode xmlNodeUuid = doc.SelectSingleNode("/config/uuid[1]");

            if (xmlNodeUuid == null) {
                XmlElement el = (XmlElement)doc.SelectSingleNode("/config");
                XmlElement guidXmlNode = doc.CreateElement("uuid");
                guidXmlNode.InnerText = Guid.NewGuid().ToString();
                el.AppendChild(guidXmlNode);
                doc.Save(ConfigManager.getConfigFilePath());

                xmlNodeUuid = doc.SelectSingleNode("/config/uuid[1]");
            }

            return xmlNodeUuid.InnerText.Replace("\r\n", "").Trim();
        }
    }
}
