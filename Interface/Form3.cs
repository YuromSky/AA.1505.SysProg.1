using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RobotContracts;
//using Engine;

namespace Interface
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            //Form3.Text = "Раунд " + Form1.roundId.ToString();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Form1.bu
            MessageBox.Show("Press Start to continue", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            //Proverka.flag = true;
        }
    }
}
