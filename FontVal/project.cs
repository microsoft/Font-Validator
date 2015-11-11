using System;
using System.Xml;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;

using OTFontFile;

namespace FontVal
{
    /// <summary>
    /// Summary description for project.
    /// </summary>
    public class project
    {
        public project()
        {
            Clear();
        }

        public void Clear()
        {
            m_hashTestsToPerform = new Hashtable();
            string [] sTableTypes = TableManager.GetKnownOTTableTypes();
            for (int i=0; i<sTableTypes.Length; i++)
            {
                m_hashTestsToPerform.Add(sTableTypes[i], true);
            }

            m_sFilesToTest = new ArrayList();

            try
            {
                m_sComputerName = SystemInformation.ComputerName;
            }
            catch
            {
                m_sComputerName = "unavailable";
            }

            m_sFilename = null;
        }

        public void LoadFromXmlFile(string sFile)
        {
            Clear();

            XmlTextReader xr = new XmlTextReader(sFile);
            while (xr.Read())
            {
                if (xr.NodeType == XmlNodeType.Element)
                {
                    if (xr.Name == "FontValidatorProject")
                    {
                        m_sComputerName = xr.GetAttribute("MachineName");
                    }
                    else if (xr.Name == "FontFile")
                    {
                        string sPath = xr.GetAttribute("Path");
                        m_sFilesToTest.Add(sPath);
                    }
                    else if (xr.Name == "Test")
                    {
                        string sTableName = xr.GetAttribute("Name");
                        string sValue = xr.GetAttribute("Value");
                        bool bTest = true;
                        if (sValue.ToLower().CompareTo("true") != 0)
                        {
                            bTest = false;
                        }
                        m_hashTestsToPerform[sTableName] = bTest;
                    }
                }
            }
            xr.Close();

            m_sFilename = sFile;
        }

        public void SaveToXmlFile()
        {
            XmlTextWriter xw = new XmlTextWriter(m_sFilename, System.Text.Encoding.UTF8);

            // begin font validator project
            xw.WriteStartElement("FontValidatorProject");
            xw.WriteAttributeString("MachineName", m_sComputerName);

            // font file list
            xw.WriteStartElement("FontFileList");
            for (int i=0; i<m_sFilesToTest.Count; i++)
            {
                xw.WriteStartElement("FontFile");
                xw.WriteAttributeString("Path", (string)m_sFilesToTest[i]);
                xw.WriteEndElement();
            }
            xw.WriteEndElement();

            // test list
            xw.WriteStartElement("TestList");
            IDictionaryEnumerator en = m_hashTestsToPerform.GetEnumerator();
            while (en.MoveNext()) 
            {
                xw.WriteStartElement("Test");
                xw.WriteAttributeString("Name", (string)en.Key);
                xw.WriteAttributeString("Value", en.Value.ToString());
                xw.WriteEndElement();
            }
            xw.WriteEndElement();

            // end font validator project
            xw.WriteEndElement();

            xw.Close();
        }

        public string [] GetFilesToTest()
        {
            string [] sFiles = new string[m_sFilesToTest.Count];
            for (int i=0; i<m_sFilesToTest.Count; i++)
            {
                sFiles[i] = (string)m_sFilesToTest[i];
            }
            return sFiles;
        }

        public void SetFilesToTest(string [] sFilesToTest)
        {
            m_sFilesToTest.Clear();
            if (sFilesToTest != null)
            {
                for (int i=0; i<sFilesToTest.Length; i++)
                {
                    m_sFilesToTest.Add(sFilesToTest[i]);
                }
            }
        }

        public Hashtable GetTests()
        {
            return m_hashTestsToPerform;
        }

        public void SetTableTest(string sTable, bool bTest)
        {
            m_hashTestsToPerform[sTable] = bTest;
        }

        public bool GetTableTest(string sTable)
        {
            return (bool)m_hashTestsToPerform[sTable];
        }

        public void SetComputerName(string sName)
        {
            m_sComputerName = sName;
        }

        public string GetComputerName()
        {
            return m_sComputerName;
        }

        public void SetFilename(string sFile)
        {
            m_sFilename = sFile;
        }

        public string GetFilename()
        {
            return m_sFilename;
        }

        ArrayList m_sFilesToTest;
        Hashtable m_hashTestsToPerform;
        string m_sComputerName;
        string m_sFilename;
    }
}
