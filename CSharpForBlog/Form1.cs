using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharpForBlog
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.KeyPreview = true;
          
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                    checkBox1.Left -= 1;
                    return true;
                case Keys.Right:
                    checkBox1.Left += 1;
                    return true;
                case Keys.Up:
                    checkBox1.Top -= 1;
                    return true;
                case Keys.Down:
                    checkBox1.Top += 1;
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

    }
}
