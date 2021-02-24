using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Linq;

namespace XML_Test
{
    class Xml_cls
    {
        public Xml_cls()
        {
            _IsExistXML();

        }

        public string _strXMLPath { get; set; }
        private bool _blexist = false;

        /// <summary>
        /// File 유무 여부 Check
        /// </summary>
        /// <returns></returns>
        public bool _IsExistXML()
        {
            if (File.Exists(_strXMLPath))
            {
                _blexist = true;
            }
            else
            {
                _blexist = false;
            }
            System.Diagnostics.Debug.Print("파일체크");
            return _blexist;
        }

        /// <summary>
        /// XML 파일 생성하기
        /// </summary>
        public void makeXMLFile()
        {
            // 파일이 없으면
            if (_blexist == false)
            {
                // xml writer UTF-8 형식으로 지정
                XmlTextWriter writer = new XmlTextWriter(_strXMLPath, System.Text.Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 2;
                writer.WriteStartElement("MemoLists");
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
            }
        }

        /// <summary>
        /// XML 작성하기 
        /// </summary>
        public void WriteXML()
        {
            var xDoc = XDocument.Load(_strXMLPath);

            string idValue = System.Guid.NewGuid().ToString();
            // ID Attribute의 값은 GUID 로 할 것.
            xDoc.Root.Add(
                new XElement("Memo",
                 new XElement("GUID", new XAttribute("value", idValue)),
                    new XElement("memo_title", "Untitle-01"),
                    new XElement("content", "아무내용")));
            xDoc.Save(_strXMLPath);
        }

        /// <summary>
        /// xml 수정하기
        /// </summary>
        public void XMLModify()
        {
            XDocument doc = XDocument.Load(_strXMLPath);

            var nodes = doc.Root.XPathSelectElements("//MemoLists//Memo").ToList();

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].Element("GUID").Attribute("value").Value.ToString() == "e8918ff3-9360-4e43-a5b9-d545831277ab")
                {
                    nodes[i].Element("content").Value = "new value";
                }
            }

            doc.Save(_strXMLPath);

        }

        public void XMLRead()
        {
            XmlTextReader reader = null;
            reader = new XmlTextReader(_strXMLPath);
            reader.WhitespaceHandling = WhitespaceHandling.None;

            try
            {
                while (reader.Read())
                {
                    if (reader.Name.ToString() == "GUID")
                    {
                        System.Diagnostics.Debug.Print(reader.GetAttribute("value").ToString());
                    }
                }
            }
            catch (System.IO.IOException e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
            finally
            {
                reader.Close();
            }


        }
        public void XMLGetIDList(System.Windows.Forms.ComboBox cb)
        {
            XmlTextReader reader = null;
            reader = new XmlTextReader(_strXMLPath);
            reader.WhitespaceHandling = WhitespaceHandling.None;
            cb.Items.Clear();

            try
            {
                while (reader.Read())
                {
                    if (reader.Name.ToString() == "GUID")
                    {
                        //System.Diagnostics.Debug.Print(reader.GetAttribute("value").ToString());
                        cb.Items.Add(reader.GetAttribute("value").ToString());
                        cb.SelectedIndex = 0;
                    }
                }
            }
            catch (System.IO.IOException e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
            finally
            {
                reader.Close();
            }

        }


    }

}
