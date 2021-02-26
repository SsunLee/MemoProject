using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
            init_Control();
            
            System.Diagnostics.Debug.Print("xml adress check");
          }
        private void init_Control()
        {
            this.textBox1.ScrollBars = ScrollBars.Vertical;
            this.textBox1.ReadOnly = true;

            this.txtTitle.Enabled = ctr_disable;
            this.txtContent.Enabled = ctr_disable;
        }
        private bool ctr_disable = false;

        private Xml_cls c = new Xml_cls();

        private void init_event()
        {
            this.btnMake.Click += new System.EventHandler(this.makeXML_click);
            this.btnWrite.Click += new System.EventHandler(this.writeXML_click);
            this.btnModify.Click += new System.EventHandler(this.modifyXML_click);
            this.btnRead.Click += new System.EventHandler(this.readXML_click);
            this.btnDel.Click += new System.EventHandler(this.delXML_click);
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.cbBox_SelectedChange);
        }

        private void makeXML_click(object sender, System.EventArgs e)
        {

            if (c._IsExistXML() == true)
            {
                string msg = "xml 파일이 이미 존재 합니다.";
                System.Windows.Forms.MessageBox.Show(@msg);

            }
            else
            {
                c.makeXMLFile();
                // 파일이 만들어졌으면 Directory Open
                string strOpenPath = string.Empty;
                strOpenPath = Path.GetDirectoryName(c._strXMLPath);
                System.Diagnostics.Debug.Print(@strOpenPath);
                System.Diagnostics.Process.Start(@"C:\Windows\explorer.exe ", String.Format("/n, /e, {0}", c._strXMLPath));
            }

    

        }

        private void writeXML_click(object sender, System.EventArgs e)
        {
            if (c._IsExistXML() == true)
            {
                if (c.WriteXML() == false)
                {
                    MessageBox.Show("성공적으로 Xml이 작성 되었습니다.");
                    readXML_click(sender, e);
                 }
            }
            else 
            {
                MessageBox.Show("경로에 XML 파일이 없습니다.");
            }
        }

        private TextBox TxtBoxIsEnabled(TextBox tx, bool flag)
        {
            tx.Enabled = flag;
            return tx;
        }

        private void modifyXML_click(object sender, System.EventArgs e)
        {
            if(c._IsExistXML() == true)
            {
                //c.XMLModify();

                if (ctr_disable == true)
                {
                    ctr_disable = false;
                    TxtBoxIsEnabled(this.txtTitle, ctr_disable);
                    TxtBoxIsEnabled(this.txtContent, ctr_disable);

                }
                else
                {
                    ctr_disable = true;
                    TxtBoxIsEnabled(this.txtTitle, ctr_disable);
                    TxtBoxIsEnabled(this.txtContent, ctr_disable);
 
                }


            }
            else
            {
                MessageBox.Show("경로에 XML 파일이 없습니다.");
            }
        }


        private void readXML_click(object sender, System.EventArgs e)
        {
            //c.XMLRead();
     
            if (c._IsExistXML() == true)
            {
                //c.XMLRead();
                string msg = c.XMLread();
                msg = msg.Replace("\n", "\r\n");
                textBox1.Clear();
                textBox1.AppendText(msg);
            }
            else
            {
                MessageBox.Show("경로에 XML 파일이 없습니다.");
            }
        }

        private void delXML_click(object sender, System.EventArgs e)
        {
            if (c._IsExistXML() == true)
            {
                c.XMLDelete();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (c._IsExistXML() == true)
            {
                c.XMLGetIDList(this.comboBox1);
            }
            else
            {
                this.comboBox1.Items.Clear();
                this.comboBox1.Text = "xml 파일이 없습니다.";
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (c._IsExistXML() == true)
            {
                c.XMLGetIDList(this.comboBox1);
            }
            else
            {
                this.comboBox1.Items.Clear();
                this.comboBox1.Text = "xml 파일이 없습니다.";
            }
        }

        private void cbBox_SelectedChange(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            Debug.Print(cb.Items[cb.SelectedIndex].ToString());
            string key = string.Empty;
            string msg = string.Empty;
            key = cb.Items[cb.SelectedIndex].ToString();
            msg = c.XMLread(key);
            Debug.Print(msg);
            msg = msg.Replace("\n", "\r\n");
            textBox1.Clear();
            textBox1.AppendText(msg);

        }

    }
}
