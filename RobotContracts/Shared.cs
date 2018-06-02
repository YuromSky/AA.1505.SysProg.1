using System;
using System.Collections.Generic;

namespace RobotContracts
{
    public class RobotState: IComparable<RobotState>
    {
        public int id;
        public int X, Y;
        public int energy, attack, defence, speed;
        public bool isAlive;
        public string name;
        public int kill;
        public int colour;
        public int []kill_id;

        public RobotState ShallowCopy()
        {
            return (RobotState)this.MemberwiseClone();
        }

        public int CompareTo(RobotState comparePart)
        {
            // A null value means that this object is greater.
            if (comparePart == null)
            {
                return 1;
            }
            else
            {
                return energy.CompareTo(comparePart.energy);
            }
        }
    }

    public class RoundConfig
    {
        public RoundConfig ShallowCopy()
        {
            return (RoundConfig)this.MemberwiseClone();
        }

        public int width;
        public int height;
        public int steps;
        public int timeout;

        public float minRND;
        public float maxRND;

        public int max_energy;
        public int max_health;
        public int max_speed;
        public int max_radius;

        public int dHealth;
        public int dEv;
        public int dEs;
        public int dEd;
        public int dEa;
        public int dE;

        public int nEnergy;
        public int nHealth;
        public int K;
    }

    public class GameConfig
    {
        public int seed;
        public IList<RoundConfig> rounds;
    }

    public class GameState
    {
        public GameState ShallowCopy()
        {
            return (GameState)this.MemberwiseClone();
        }

        public GameState DeepCopy()
        {
            GameState other = (GameState)this.MemberwiseClone();
            other.robots = new List<RobotState>();
            other.points = new List<Point>();
            foreach (RobotState rs in this.robots)
            {
                other.robots.Add(rs.ShallowCopy());
            }
            foreach (Point ps in this.points)
            {
                other.points.Add(ps.ShallowCopy());
            }

            return other;
        }

        public IList<RobotState> robots = new List<RobotState>();
        public IList<Point> points = new List<Point>();
    }

    public class RobotAction
    {
        public int dX, dY;
        public int targetId;
        public int dA, dD, dV;
    }

    public enum PointType
    {
        Energy,
        Health
    }

    public class Point
    {
        public Point ShallowCopy()
        {
            return (Point)this.MemberwiseClone();
        }
        public int X, Y;
        public PointType type;
    }
}
