using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XML_Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            init_event();
            // git 반영 test
            c._strXMLPath =Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "XMLFile1.xml");
            System.Diagnostics.Debug.Print("xml adress check");
    }
        private Xml_cls c = new Xml_cls();

        private void init_event()
        {
            this.btnMake.Click += new System.EventHandler(this.makeXML_click);
            this.btnWrite.Click += new System.EventHandler(this.writeXML_click);
            this.btnModify.Click += new System.EventHandler(this.modifyXML_click);
            this.btnRead.Click += new System.EventHandler(this.readXML_click);
            this.btnDel.Click += new System.EventHandler(this.delXML_click);
        }

        private void makeXML_click(object sender, System.EventArgs e)
        {
            c.makeXMLFile();

        }

        private void writeXML_click(object sender, System.EventArgs e)
        {
            c.WriteXML();
        }

        private void modifyXML_click(object sender, System.EventArgs e)
        {
            c.XMLModify();
        }

        private void readXML_click(object sender, System.EventArgs e)
        {
            c.XMLRead();
        }

        private void delXML_click(object sender, System.EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (c._IsExistXML() == true)
            {
                c.XMLGetIDList(this.comboBox1);
            }
        }
    }
}
