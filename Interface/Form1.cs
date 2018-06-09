using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using RobotContracts;
using Newtonsoft.Json;
using System.Text;

namespace Interface
{
    public partial class Form1 : Form
    {
        int roundId = 0;

        string configPath;

        GameConfig config;
        Game game;

        List<Tuple<string, IRobot>> robots;

        EventWaitHandle evStop;

        public Form1()
        {
            InitializeComponent();

            configPath = "../../config.json";
            config = ConfigLoader.LoadConfig(configPath);
            game = new Game(config);

            robots = new List<Tuple<string, IRobot>>();
            game.future_robots = Load_save_robots();
            evStop = new EventWaitHandle(false, EventResetMode.AutoReset, "EventStop");
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

                int place = 1;
                int countPlace = 0;
                for (int j = game.future_robots.Count - 1; j >= 0; j--)
                {
                    countPlace++;

                    Form3.dataGridView3.Rows.Add();
                    int i = 0;
                    if (j == game.future_robots.Count - 1)
                    {
                        Form3.dataGridView3.Rows[game.future_robots.Count - 1 - j].Cells[i++].Value = place;
                    }
                    else
                    {
                        if (game.future_robots[j].energy != game.future_robots[j + 1].energy)
                        {
                            place = countPlace;
                        }
                        Form3.dataGridView3.Rows[game.future_robots.Count - 1 - j].Cells[i++].Value = place;
                    }
                    Form3.dataGridView3.Rows[game.future_robots.Count - 1 - j].Cells[i++].Value = game.future_robots[j].id;
                    Form3.dataGridView3.Rows[game.future_robots.Count - 1 - j].Cells[i++].Value = game.future_robots[j].name;
                    Form3.dataGridView3.Rows[game.future_robots.Count - 1 - j].Cells[i++].Value = game.future_robots[j].defence + game.future_robots[j].attack + game.future_robots[j].speed;
                    Form3.dataGridView3.Rows[game.future_robots.Count - 1 - j].Cells[i++].Value = game.future_robots[j].energy;
                    Form3.dataGridView3.Rows[game.future_robots.Count - 1 - j].Cells[i++].Value = game.future_robots[j].isAlive;

                    if (place == 1)
                    {
                        Form3.dataGridView3.Rows[game.future_robots.Count - 1 - j].Cells[i++].Value = 5;
                        fs.WriteLine(place.ToString() + "   " + game.future_robots[j].name + " " + "5");
                    }
                    if (place > 1 && place < 4)
                    {
                        Form3.dataGridView3.Rows[game.future_robots.Count - 1 - j].Cells[i++].Value = 4;
                        fs.WriteLine(place.ToString() + "   " + game.future_robots[j].name + " " + "4");
                    }

                    if (place > 3 && place < 11)
                    {
                        Form3.dataGridView3.Rows[game.future_robots.Count - 1 - j].Cells[i++].Value = 3;
                        fs.WriteLine(place.ToString() + "   " + game.future_robots[j].name + " " + "3");
                    }

                    if (place > 10 && place < 21)
                    {
                        Form3.dataGridView3.Rows[game.future_robots.Count - 1 - j].Cells[i++].Value = 2;
                        fs.WriteLine(place.ToString() + "   " + game.future_robots[j].name + " " + "2");
                    }

                    if (place > 20 && place < 31)
                    {
                        Form3.dataGridView3.Rows[game.future_robots.Count - 1 - j].Cells[i++].Value = 1;
                        fs.WriteLine(place.ToString() + "   " + game.future_robots[j].name + " " + "1");
                    }
                }
            }
        }
        public static List<RobotState> Load_save_robots()
        {
            string logPath = "../../log_interface.txt";
            try
            {
                using (StreamReader sr = new StreamReader("../../round6_data.json"))
                {
                    string json = sr.ReadToEnd();
                    return JsonConvert.DeserializeObject<List<RobotState>>(json);
                }
            }
            catch (Exception e)
            {
                File.AppendAllText(logPath, "Round 6 data load failed: " + e.ToString() + Environment.NewLine, Encoding.UTF8);
            }

            return null;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Form form2 = new Form2();
            form2.Show();
            this.Hide();
        }

        public void button2_Click(object sender, EventArgs e)
        {
            Start();
        }

        public void GameThread(Object obj)
        {
            ThreadParam param = new ThreadParam();
            param = (ThreadParam)obj;
            game.Loop(param.robots, param.round, param.dN);

            evStop.Set();
        }

        private void Start()
        {
            label2.Text = "Раунд: " + (roundId + 1).ToString();

            Draw.Init();

            if (config == null)
            {
                return;
            }

            if (roundId == 0)
            {
                if (game.future_robots !=null)
                {
                    robots = RobotLoader.LoadRobots(config.robots);
                    int n = config.rounds.Count;
                    for (int i = 1; i < n; i++)
                    {
                        robots.AddRange(RobotLoader.LoadRobots(config.robots));
                    }
                    
                }
                else
                {
                    game.future_robots = new List<RobotState>();
                }
            }
                if (robots != null)
            {
                List<Tuple<string, IRobot>> robots_base = RobotLoader.LoadRobots(config.robots);
                int[] unique_id = new int[game.future_robots.Count+1];
                if (game.future_robots.Count > 0)
                {
                    for (int unic = 0; unic < game.future_robots.Count + 1; unic++)
                    {
                        unique_id[unic] = -1;
                    }
                }
                int dN = 0;
                bool isUnic;
                foreach (RobotState rs in game.future_robots)
                {
                    for (int robot_id = 0; robot_id < robots.Count; robot_id++)
                    {
                        isUnic = true;
                        for (int unic = 0; unic <= dN; unic++)
                        {
                            if (robot_id == unique_id[unic])
                            {
                                isUnic = false;
                            }
                        }

                        if ((rs.isAlive == true) && (robots[robot_id].Item2.Name == rs.name)&&(isUnic))
                        {
                            robots_base.Add(robots[robot_id]);
                            unique_id[dN] = robot_id;
                            dN++;
                            break;
                        }
                        
                    }
                        
                }

                robots.Clear();
                robots.AddRange(robots_base);

                ThreadParam param = new ThreadParam();
                param.robots = robots;
                param.round = roundId;
                param.dN = dN;

                Thread game_thread = new Thread(GameThread);
                game_thread.Start(param);

                evStop.WaitOne();
                evStop.Reset();

                gameStats();

                roundId++;

                label2.Text = "Раунд: " + (roundId + 1).ToString();

                if (roundId >= config.rounds.Count)
                {
                    string logPath = "../../log_interface.txt";

                    try
                    {
                        string json = JsonConvert.SerializeObject(game.future_robots);
                        File.WriteAllText("../../round6_data.json", json);
                    }
                    catch (Exception e)
                    {
                        File.AppendAllText(logPath, "Round 6 data dump failed: " + e.ToString() + Environment.NewLine, Encoding.UTF8);
                    }

                    Application.Exit();
                }
            }
        }
    }

    public class ThreadParam
    {
        public List<Tuple<string, IRobot>> robots;
        public int round;
        public int dN;
    }
}
