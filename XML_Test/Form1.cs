using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        }


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


        }

        private void writeXML_click(object sender, System.EventArgs e)
        {

        }

        private void modifyXML_click(object sender, System.EventArgs e)
        {

        }

        private void readXML_click(object sender, System.EventArgs e)
        {

        }

        private void delXML_click(object sender, System.EventArgs e)
        {

        }

    }
}
