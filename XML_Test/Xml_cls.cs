using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Linq;
using System;
using System.Windows.Forms;

namespace XML_Test
{
    class Xml_cls
    {
        public Xml_cls()
        {
            _IsExistXML();
            _blexist = false;
        }

        //public string _strXMLPath { get; set; }
        public string _strXMLPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "XMLFile1.xml");

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
            System.Diagnostics.Debug.Print("현재 경로" + _strXMLPath);
            return _blexist;
        }

        /// <summary>
        /// Class 내에서 로그를 찍기 위함.
        /// </summary>
        /// <param name="msg"> 로그를 찍을 문자열 </param>
        private void print(string msg)
        {
            string s = string.Empty;
            s = @"XML Class = " + msg;
            System.Diagnostics.Debug.Print(s);
        }

        /// <summary>
        /// XML 파일 생성하기
        /// </summary>
        public void makeXMLFile()
        {
            // 파일이 없으면
            if (_blexist == false)
            {
                try
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
                catch (Exception e)
                {
                    print(e.Message);
                }

            }
        }

        /// <summary>
        /// XML 작성하기 
        /// </summary>
        public bool WriteXML()
        {
            bool err_chk = false;
            try
            {
                var xDoc = XDocument.Load(_strXMLPath);

                string idValue = System.Guid.NewGuid().ToString();
                // ID Attribute의 값은 GUID 로 할 것.
                xDoc.Root.Add(
                    new XElement("Memo", new XAttribute("GUID", idValue),
                        new XElement("memo_title", "Untitle-01"),
                        new XElement("content", "아무내용")));
                xDoc.Save(_strXMLPath);
                err_chk = false;
            }
            catch (Exception e)
            {
                print(e.Message);
                err_chk = true; 
            }

            return err_chk;
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
                if (nodes[i].Attribute("GUID").ToString() == "e8918ff3-9360-4e43-a5b9-d545831277ab")
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
            reader.ReadToDescendant("Memo");
            do
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.HasAttributes == true)
                            print(reader.Name + " : " + reader.GetAttribute("GUID").ToString());
                        break;
                    case XmlNodeType.Text:
                        print(reader.NodeType + " : " + reader.Name + " : " + reader.Value);
                        break;
                }
            } while (reader.Read());

            reader.Close();

            /*                    if (reader.Name.ToString() == "GUID")
                                {
                                    string _id = reader["value"].ToString() ;
                                    string _title = reader["memo_title"].ToString();
                                    string _content = reader["content"].ToString();

                                    string temp = string.Empty;
                                    temp = string.Format("" +
                                        "Memo id : {_id}" +
                                        "  - Memo Title : {_title}" +
                                        "  - Contents : " +
                                        "    {_content} " +
                                        "----------------------");
                                    System.Diagnostics.Debug.Print(temp);
                                }*/
        }

        /// <summary>
        /// XML 내용 읽기
        /// </summary>
        /// <returns></returns>
        public string XMLread()
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(_strXMLPath);

            XmlNode root = xml.SelectNodes("MemoLists")[0];

            string Return_msg = string.Empty;
            string msg = string.Empty;
            foreach (XmlNode n in root.SelectNodes("Memo"))
            {
                string guid = n.Attributes["GUID"].Value;
                string title = n.SelectNodes("memo_title")[0].InnerText;
                string content = n.SelectNodes("content")[0].InnerText;

                msg = $"\n-------------------------------------------- \n< GUID : {guid} >\n < Title : {title} >\n <Content : {content} >\n --------------------------------------------";
                //print(@msg);
                Return_msg += msg;

            }
            print(Return_msg);
            return Return_msg;

        }
    



        

        /// <summary> 
        /// XML의 ID값 가져오는 부분 
        /// </summary> 
        /// <param name="cb"></param>
        public void XMLGetIDList(System.Windows.Forms.ComboBox cb)
        {
            XmlTextReader reader = null;
            reader = new XmlTextReader(_strXMLPath);
            reader.WhitespaceHandling = WhitespaceHandling.None;
            reader.ReadToDescendant("Memo");
            cb.Items.Clear();

            try
            {
                do
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.HasAttributes == true)
                            {
                                print(reader.Name + " : " + reader.GetAttribute("GUID").ToString());
                                cb.Items.Add(reader.GetAttribute("GUID").ToString());
                                //cb.SelectedIndex = 0;
                                cb.Text = "여기를 눌러 GUID 를 선택하세요.";
                            }
                            break;
                    }
                } while (reader.Read());
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

        public void XMLDelete()
        {
            try
            {
                string strResult = del(@_strXMLPath);
                System.Windows.Forms.MessageBox.Show(@strResult);
                _blexist = false;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
            

        }
        private string del(string filepath)
        {
            if (File.Exists(@filepath))
            {
                try
                {
                    File.Delete(@filepath);
                }
                catch (Exception e)
                {
                    return "File couldn't be deleted because : " + e.GetType().Name;
                }
            }
            else
            {
                return "File doesn't exsist";
            }

            return "File Successfully deleted";
        }


    }

}
