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
        public static void Init()
        {

        }

        public static void Update(IList<RobotState> robots, IList<RobotContracts.Point> points, int w, int h)
        {
            int width = 750 / w; //Form1.pictureBox1.Width / w;
            int height = 750 / h; //Form1.pictureBox1.Height / h;
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

            Bitmap bmp = new Bitmap(750, 750); // (Form1.pictureBox1.Width, Form1.pictureBox1.Height);
            Graphics graph = Graphics.FromImage(bmp);

            Pen pen = new Pen(Color.Blue);
            SolidBrush brushSelect = new SolidBrush(Color.Green);

            graph.Clear(Color.White);

            for (int i = 0; i < points.Count; i++)
            {
                SolidBrush brushPoint;
                if (points[i].type == PointType.Energy)
                {
                    brushPoint = new SolidBrush(Color.Green);
                }
                else
                {
                    brushPoint = new SolidBrush(Color.Black);
                }
                graph.FillRectangle(brushPoint, xList[points[i].X], yList[points[i].Y], width, height);
            }

            for (int i = 0; i < robots.Count; i++)
            {
                SolidBrush brushRobot;
                switch (robots[i].colour)
                {
                    case 0:
                        brushRobot = new SolidBrush(Color.Blue);
                        break;
                    case 1:
                        brushRobot = new SolidBrush(Color.Purple);
                        break;
                    case 2:
                        brushRobot = new SolidBrush(Color.Red);
                        break;
                    case 3:
                        brushRobot = new SolidBrush(Color.GreenYellow);
                        break;
                    default:
                        brushRobot = new SolidBrush(Color.Blue);
                        break;
                }

                graph.FillEllipse(brushRobot, xList[robots[i].X], yList[robots[i].Y], width, height);
            }

            Form1.pictureBox1.Image = bmp;
        }
    }
}
