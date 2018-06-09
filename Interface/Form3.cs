using System;
using System.Windows.Forms;

namespace Interface
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            MessageBox.Show("Press Start to continue", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
    }
}
