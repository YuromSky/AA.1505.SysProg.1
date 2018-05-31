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
using System.Threading;
using System.IO;

//using Engine;

namespace Interface
{

    //public List<RobotState> listRobots = new List<RobotState>(); 

    public partial class Form1 : Form
    {
        int dN = 0;
        public static List<ProgressBar> pbList;
        List<Label> lbList;
        public static int roundId = 0;

        static string configPath = "../../config.json";
        static GameConfig config = ConfigLoader.LoadConfig(configPath);
        Game game = new Game(config);
        static string directoryPath = "../../Robots";
        List<IRobot> robots = new List<IRobot>();
        IList<IRobot> robots_base = RobotLoader.LoadRobots(directoryPath);

        EventWaitHandle evStop = new EventWaitHandle(false, EventResetMode.AutoReset, "EventStart2");
        EventWaitHandle evConfirm = new EventWaitHandle(false, EventResetMode.AutoReset, "EventStart2");
        public Form1()
        {
            InitializeComponent();
        }

        public void createField()
        {

            evConfirm.Set();
        }

        public void gameStats()
        {
            string path = @"round" + roundId.ToString() + ".txt";
            using (StreamWriter fs = File.CreateText(path))
            {
                game.future_robots.Sort();
                Form form3 = new Form3();
                form3.Text = "Раунд: " + (roundId + 1).ToString();
                form3.Show();
                int mesto = 1;
                int countMesto = 0;
                for (int j = game.future_robots.Count - 1; j >= 0; j--)
                {
                    countMesto++;
                    Form3.dataGridView3.Rows.Add();
                    int i = 0;
                    if (j == game.future_robots.Count - 1)
                    {
                        Form3.dataGridView3.Rows[game.future_robots.Count - 1 - j].Cells[i++].Value = mesto;
                    }
                    else
                    {
                        if (game.future_robots[j].energy != game.future_robots[j + 1].energy)
                        {
                            mesto = countMesto;
                        }
                        Form3.dataGridView3.Rows[game.future_robots.Count - 1 - j].Cells[i++].Value = mesto;
                    }
                    Form3.dataGridView3.Rows[game.future_robots.Count - 1 - j].Cells[i++].Value = game.future_robots[j].id;
                    Form3.dataGridView3.Rows[game.future_robots.Count - 1 - j].Cells[i++].Value = game.future_robots[j].name;
                    Form3.dataGridView3.Rows[game.future_robots.Count - 1 - j].Cells[i++].Value = game.future_robots[j].defence + game.future_robots[j].attack + game.future_robots[j].speed;
                    Form3.dataGridView3.Rows[game.future_robots.Count - 1 - j].Cells[i++].Value = game.future_robots[j].energy;
                    Form3.dataGridView3.Rows[game.future_robots.Count - 1 - j].Cells[i++].Value = game.future_robots[j].isAlive;

                    if (mesto == 1)
                    {
                        Form3.dataGridView3.Rows[game.future_robots.Count - 1 - j].Cells[i++].Value = 5;
                        fs.WriteLine(mesto.ToString() + "   " + game.future_robots[j].name + " " + "5");
                    }
                    if (mesto > 1 && mesto < 4)
                    {
                        Form3.dataGridView3.Rows[game.future_robots.Count - 1 - j].Cells[i++].Value = 4;
                        fs.WriteLine(mesto.ToString() + "   " + game.future_robots[j].name + " " + "4");
                    }

                    if (mesto > 3 && mesto < 11)
                    {
                        Form3.dataGridView3.Rows[game.future_robots.Count - 1 - j].Cells[i++].Value = 3;
                        fs.WriteLine(mesto.ToString() + "   " + game.future_robots[j].name + " " + "3");
                    }

                    if (mesto > 10 && mesto < 21)
                    {
                        Form3.dataGridView3.Rows[game.future_robots.Count - 1 - j].Cells[i++].Value = 2;
                        fs.WriteLine(mesto.ToString() + "   " + game.future_robots[j].name + " " + "2");
                    }

                    if (mesto > 20 && mesto < 31)
                    {
                        Form3.dataGridView3.Rows[game.future_robots.Count - 1 - j].Cells[i++].Value = 1;
                        fs.WriteLine(mesto.ToString() + "   " + game.future_robots[j].name + " " + "1");
                    }



                }
            }

           

            
        }

        public void newGame(Object obj)
        {
            //createField
            Param param = new Param();
            param = (Param)obj;
            game.Loop(param.g, param.rId, param.d);
            evStop.Set();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form form2 = new Form2();
            form2.Show();
            this.Hide();
        }

        public void button2_Click(object sender, EventArgs e)
        {
            //Proverka.flag = true;
            //while(true)
            //{
            //    if(Proverka.flag)
            //    {
            //        Proverka.flag = false;
            //        start();
            //    }
            //}

            start();
            
            
        }

        public void start()
        {
            label2.Text = "Раунд: " + (roundId + 1).ToString();

            Draw.Start();//Draw.bmp, Draw.xList,Draw.yList);
            //функция старта, внутри нее отрисовка,и создание параметров поля,узнаем количество роботов

            if (config == null)
            {
                return;
            }



            if (robots != null)
            {

                dN = 0;
                pbList = new List<ProgressBar>();
                lbList = new List<Label>();
                robots_base = RobotLoader.LoadRobots(directoryPath);
                foreach (RobotState rs in game.future_robots)
                {
                    if (rs.isAlive == true)
                    {
                        robots_base.Add(robots[rs.id]);
                        dN++;
                    }
                }
                robots.Clear();
                robots.AddRange(robots_base);

                //createField
                //int n = /*game.future_robots.Count;*/robots.Count;
                ////listBox1.Height = 14 * n;
                ////Draw.param(100,100);
                //for (int i = 0; i < n; i++)
                //{
                //    // listBox1.Items.Add(/*game.future_robots[i].id*/robots[i]);
                //    Label lb = new Label();
                //    lb.Size = label1.Size;
                //    lb.Font = label1.Font;
                //    lb.Name = "lb" + i.ToString();
                //    lb.Location = new System.Drawing.Point(764, 68 + 18 * i);
                //    lb.Text = i.ToString()+". "+robots[i].Name;
                //    this.Controls.Add(lb);
                //    lbList.Add(lb);
                //}

                //    ProgressBar pb = new ProgressBar();
                //    pb.Name = "pbH" + i.ToString();
                //    pb.Location = new System.Drawing.Point(890, 68 + 18 * i);
                //    pb.Size = progressBar1.Size;

                //    this.Controls.Add(pb);
                //    pbList.Add(pb);
                //    pb = new ProgressBar();
                //    pb.Name = "pbR" + i.ToString();
                //    pb.Location = new System.Drawing.Point(980, 68 + 18 * i);//подобрать значения
                //    pb.Size = progressBar1.Size;
                //    this.Controls.Add(pb);
                //    pbList.Add(pb);
                //}

                Param p = new Param();
                p.rId = roundId;
                p.d = dN;
                p.g = robots;

                var th = new Thread(newGame);
                th.Start(p);
                evStop.WaitOne();
                gameStats();
                evStop.Reset();
                //game
                //game.Loop(robots, roundId, dN);

                //Thread.Sleep(5000);
                //clearField
                //listBox1.Items.Clear();
                //for(int i = 0; i<pbList.Count;i++)
                //{
                //    pbList[i].Dispose();
                //}
                //pbList.Clear();
                //statView
                //Thread.Sleep(3000);
                //game.future_robots.Sort();
                //Form form3 = new Form3();
                //form3.Show();
                //for (int j = 0; j < game.future_robots.Count; j++)
                //{
                //    Form3.dataGridView3.Rows.Add();
                //    Form3.dataGridView3.Rows[j].Cells[0].Value = game.future_robots[j].id;
                //    Form3.dataGridView3.Rows[j].Cells[1].Value = game.future_robots[j].name;
                //    Form3.dataGridView3.Rows[j].Cells[2].Value = game.future_robots[j].defence + game.future_robots[j].attack + game.future_robots[j].speed;
                //    Form3.dataGridView3.Rows[j].Cells[3].Value = game.future_robots[j].energy;
                //    Form3.dataGridView3.Rows[j].Cells[4].Value = game.future_robots[j].isAlive;
                //}

                roundId++;
                if (roundId > 4)
                {
                    Application.Exit();
                }
            }
        }
    }

    public class Param
    {
        public List<IRobot> g;
        public int rId;
        public int d;
    }

    //public static class GameStart
    //{
    //    static string configPath = "../../config.json";
    //    static GameConfig config = ConfigLoader.LoadConfig(configPath);
    //    static Game game = new Game(config);
    //    static string directoryPath = "../../Robots";
    //    static List<IRobot> robots = new List<IRobot>();
    //    static IList<IRobot> robots_base = RobotLoader.LoadRobots(directoryPath);
    //}
}
