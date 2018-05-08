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

namespace Interface
{
    public partial class Form2 : Form
    {
        int roundCount = 0;

        public static GameConfig game_config = new GameConfig();

        public Form2()
        {
            InitializeComponent();

            game_config.rounds = new List<RoundConfig>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RoundConfig round_config = new RoundConfig
            {
                width = Convert.ToInt32(textBox1.Text),
                height = Convert.ToInt32(textBox2.Text),
                steps = Convert.ToInt32(textBox11.Text),
                timeout = Convert.ToInt32(textBox6.Text),
                minRND = Convert.ToInt32(textBox22.Text),
                maxRND = Convert.ToInt32(textBox21.Text),
                max_energy = Convert.ToInt32(textBox10.Text),
                max_health = Convert.ToInt32(textBox9.Text),
                max_speed = Convert.ToInt32(textBox8.Text),
                max_radius = Convert.ToInt32(textBox7.Text),
                dHealth = Convert.ToInt32(textBox15.Text),
                dEv = Convert.ToInt32(textBox14.Text),
                dEs = Convert.ToInt32(textBox13.Text),
                dEd = Convert.ToInt32(textBox12.Text),
                dEa = Convert.ToInt32(textBox16.Text),
                dE = Convert.ToInt32(textBox17.Text),
                nEnergy = Convert.ToInt32(textBox20.Text),
                nHealth = Convert.ToInt32(textBox19.Text),
                K = Convert.ToInt32(textBox18.Text)
            };
            game_config.rounds.Insert(roundCount, round_config);

            button2.Enabled = true;
            roundCount = roundCount + 1;
            label2.Text = "Раунд " + (roundCount+1).ToString();

            if (roundCount > 3)
            {
                button1.Text = "Выход";
            }
            
            if (roundCount > 4)
            {
                // save отправить лист конфигов
                string configPath = "../../config.json";
                ConfigLoader.SaveConfig(configPath, game_config);
                Form form1 = new Form1();
                form1.Show();
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (roundCount <= 1)
            {
                button2.Enabled = false;
            }
            roundCount = roundCount - 1;
            label2.Text = "Раунд " + (roundCount + 1).ToString();
            button1.Text = "Далее";
        }

        private void Form2_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }
    }
}
