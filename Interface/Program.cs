using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using RobotContracts;

namespace Interface
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    public class Draw
    {
        public static void Start()
        {

        }

        public static void drawing(IList<RobotState> robots, IList<RobotContracts.Point>points, int w, int h)
        {
            int width = 750 / w; //Form1.pictureBox1.Width / w; ;
            int height = 750 / h;//Form1.pictureBox1.Height / h;
            List<int> xList = new List<int>();
            List<int> yList = new List<int>();

            for (int i = 0; i < w; i++)
            {
                xList.Add(width * i);
            }

            for (int i = 0; i < h; i++)
            {
                yList.Add(height * i);
            }

            Bitmap bmp = new Bitmap(750, 750);//(Form1.pictureBox1.Width, Form1.pictureBox1.Height);
            Graphics graph = Graphics.FromImage(bmp);
            Pen pen = new Pen(Color.Blue);
            SolidBrush brushR1 = new SolidBrush(Color.Blue);
            SolidBrush brushR2 = new SolidBrush(Color.Purple);
            SolidBrush brushR3 = new SolidBrush(Color.Red);
            SolidBrush brushR4 = new SolidBrush(Color.GreenYellow);
            SolidBrush brushSelect = new SolidBrush(Color.Green);
            SolidBrush brushHealth = new SolidBrush(Color.Black);
            SolidBrush brushEnergy = new SolidBrush(Color.Green);
 

            Form1.pictureBox1.Image = bmp;

            graph.Clear(Color.White);

            for (int i = 0; i < robots.Count; i++)
            {
                graph.FillEllipse(brushR4, xList[robots[i].X], yList[robots[i].Y], width, height);

                //if(robots[i].color == 0)
                //{
                //    graph.FillEllipse(brushR1, xList[robots[i].X], yList[robots[i].Y], width, height);
                //}
                //else if (robots[i].color == 1)
                //{
                //    graph.FillEllipse(brushR2, xList[robots[i].X], yList[robots[i].Y], width, height);
                //}
                //else if (robots[i].color == 2)
                //{
                //    graph.FillEllipse(brushR3, xList[robots[i].X], yList[robots[i].Y], width, height);
                //}
                //else if (robots[i].color == 3)
                //{
                //    graph.FillEllipse(brushR4, xList[robots[i].X], yList[robots[i].Y], width, height);
                //}
            }

            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].type == PointType.Energy)
                {
                    graph.FillRectangle(brushEnergy, xList[points[i].X], yList[points[i].Y], width, height);
                }
                else
                {
                    graph.FillRectangle(brushHealth, xList[points[i].X], yList[points[i].Y], width, height);
                }               
            }
        }
        public static void param(int w, int h)
        {
            //xList.Clear();
            //yList.Clear();

            //width = Form1.pictureBox1.Width / w; ;
            //height = Form1.pictureBox1.Height / h;

            //for (int i = 0; i < w; i++)
            //{
            //    xList.Add(width * i);
            //}

            //for(int i = 0; i<h;i++)
            //{
            //    yList.Add(height * i);
            //}
        }
    }

    public class Results
    {
        List<ProgressBar> pbList;
        //public static void showResults(List<RobotStats> robots)
        //{
        //    for(int i = 0; i < robots.Count; i++)
        //    {
        //        Form3.dataGridView1[0, i] = robots[i].name;
        //        Form3.dataGridView1[0, i] = robots[i].energy;
        //        Form3.dataGridView1[0, i] = robots[i].kills;
        //        Form3.dataGridView1[0, i] = robots[i].score;
        //    }
        //}
    }

    public static class Proverka
    {
        public static bool flag = false;
    }
}
