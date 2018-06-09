using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using RobotContracts;

namespace Interface
{
    public class Game
    {
        string logpath;
        public GameConfig config;
        public GameState state;
        public List<RobotState> future_robots = new List<RobotState>();
        delegate int del();

        public Game(GameConfig game_config)
        {
            config = game_config;
            state = new GameState();

            SingleRandom.Seed(config.seed);
        }

        private void UpdateState(RoundConfig round_config, int id, RobotAction action)
        {
            RobotState robot = state.robots[id].ShallowCopy();
            future_robots.Add(robot);
            if (!robot.isAlive)
            {
                return;
            }

            if (action == null)
            {
                File.AppendAllText(logpath, "Timeout. id: " + robot.id + Environment.NewLine, Encoding.UTF8);
                robot.energy -= round_config.dEs;
                return;
            }

            del GetHealth = () => { return robot.attack + robot.defence + robot.speed; };

            // charging stations
            foreach (Point point in state.points)
            {
                if ((robot.X == point.X) && (robot.Y == point.Y))
                {
                    switch (point.type)
                    {
                        case PointType.Energy:
                            robot.energy = (robot.energy + round_config.dE <= round_config.max_energy) ? (robot.energy + round_config.dE) : round_config.max_energy;
                            File.AppendAllText(logpath, "Added energy on point. id: " + robot.id + "; pos: (" + point.X + ", " + point.Y + "); energy: " + robot.energy + Environment.NewLine, Encoding.UTF8);
                            break;
                        case PointType.Health:
                            int health = GetHealth();
                            if (health < round_config.max_health)
                            {
                                int dAt = (robot.attack == 0)? 1 : (int)(((float)robot.attack / (float)health) * round_config.dHealth);
                                int dDf = (robot.defence == 0) ? 1: (int)(((float)robot.defence / (float)health) * round_config.dHealth);
                                int dSp = (robot.speed == 0) ? 1 : (int)(((float)robot.speed / (float)health) * round_config.dHealth);
                                robot.attack += (dAt + GetHealth() < round_config.max_health) ? dAt: round_config.max_health - GetHealth();
                                robot.defence += (dDf + GetHealth() < round_config.max_health) ? dDf : round_config.max_health - GetHealth(); 
                                if (robot.speed < round_config.max_speed)
                                    robot.speed += (dSp + GetHealth() < round_config.max_health) ? dSp : round_config.max_health - GetHealth();

                                File.AppendAllText(logpath, "Added health on point. id: " + robot.id + "; pos: (" + point.X + ", " + point.Y + "); health: " + GetHealth() + Environment.NewLine, Encoding.UTF8);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            // drain energy from dead robots
            foreach (RobotState donor in state.robots)
            {
                if ((robot.id != donor.id) && (robot.X == donor.X) && (robot.Y == donor.Y) && !donor.isAlive)
                {
                    int health = GetHealth();
                    if (health < round_config.max_health)
                    {
                        int donor_health = donor.attack + donor.defence + donor.speed;
                        if (donor_health > 0)
                        {
                            if (donor.attack > 0)
                            {
                                robot.attack += robot.attack / donor_health * round_config.dHealth;
                                donor.attack -= donor.attack / donor_health * round_config.dHealth;
                            }

                            if (donor.defence > 0)
                            {
                                robot.defence += robot.defence / donor_health * round_config.dHealth;
                                donor.defence -= donor.defence / donor_health * round_config.dHealth;
                            }

                            if (donor.speed > 0 && robot.speed <= round_config.max_speed)
                            {
                                robot.speed += robot.speed / donor_health * round_config.dHealth;
                                donor.speed -= donor.speed / donor_health * round_config.dHealth;
                            }

                            if (health != GetHealth())
                            {
                                File.AppendAllText(logpath, "Health drain. id: " + robot.id + "; pos: (" + robot.X + ", " + robot.Y + "); target: " + donor.id + "; health: " + GetHealth() + Environment.NewLine, Encoding.UTF8);
                            }
                        }
                    }
                }
            }

            // movement
            int distance = (int)Math.Sqrt(Math.Pow(action.dX, 2) + Math.Pow(action.dY, 2));
            int max_distance = 10 * round_config.max_speed * robot.speed / round_config.max_health * robot.energy / round_config.max_energy;

            if ((distance > 0) && (distance <= max_distance))
            {
                robot.X = Math.Max(Math.Min(robot.X + action.dX, round_config.width - 1), 0);
                robot.Y = Math.Max(Math.Min(robot.Y + action.dY, round_config.height - 1), 0);
                robot.energy -= round_config.dEv;

                File.AppendAllText(logpath, "Movement. id: " + robot.id + "; pos: (" + robot.X + ", " + robot.Y + "); energy: " + robot.energy + Environment.NewLine, Encoding.UTF8);
            }
            else
            {
                robot.energy -= round_config.dEs;

                File.AppendAllText(logpath, "No movement. id: " + robot.id + "; pos: (" + robot.X + ", " + robot.Y + "); energy: " + robot.energy + Environment.NewLine, Encoding.UTF8);
            }

            // attack and defence
            if (action.targetId != -1)
            {
                RobotState target_robot = null;
                if (action.targetId < robot.id)
                {
                    target_robot = future_robots[action.targetId];
                }
                else
                {
                    target_robot = state.robots[action.targetId];
                }

                int distance_attack = (int)Math.Sqrt(Math.Pow((target_robot.X - robot.X), 2) + Math.Pow((target_robot.Y - robot.Y), 2));
                int max_distance_attack = (int)Math.Round(10 * (float)round_config.max_radius * (float)robot.speed / (float)round_config.max_health * (float)robot.energy / (float)round_config.max_energy);

                int real_power = SingleRandom.Instance.Next((int)(round_config.minRND), (int)(round_config.maxRND)) * robot.attack / 10;
                int max_power = real_power * robot.energy / round_config.max_energy;

                int real_target_defence = ((10 - SingleRandom.Instance.Next((int)(round_config.minRND), (int)(round_config.maxRND))) * target_robot.defence) / 10;
                int max_target_defence = (int)Math.Round((float)real_target_defence * (float)robot.energy / (float)round_config.max_energy);

                if ((distance_attack <= max_distance_attack) && (target_robot.energy > 0))
                {
                    if (max_power >= max_target_defence)
                    {
                        int attack = max_power - max_target_defence;
                        if (target_robot.defence <= 0)
                        {
                            target_robot.defence = 0;
                            target_robot.energy -= attack * round_config.max_energy / round_config.max_health;
                        }
                        else
                        {
                            target_robot.defence -= attack;
                        }

                        File.AppendAllText(logpath, "Attack. id: " + robot.id + "; pos: (" + robot.X + ", " + robot.Y + "); target: " + target_robot.id + "; attack: " + attack + Environment.NewLine, Encoding.UTF8);
                    }
                    else
                    {
                        int defence = max_target_defence - max_power;
                        if (robot.attack <= 0)
                        {
                            robot.attack = 0;
                            robot.energy -= defence * round_config.max_energy / round_config.max_health;
                        }
                        else
                        {
                            robot.attack -= defence;
                        }

                        File.AppendAllText(logpath, "Defence. id: " + robot.id + "; pos: (" + robot.X + ", " + robot.Y + "); target: " + target_robot.id + "; defence: " + defence + Environment.NewLine, Encoding.UTF8);
                    }

                    robot.energy -= round_config.dEa;
                    target_robot.energy -= round_config.dEd;
                }

                if ((target_robot.energy <= 0) && (target_robot.defence == 0))
                {
                    bool flag = true;
                    foreach (RobotState rb in state.robots)
                    {
                        for (int i = 0; i < rb.kill; i++)
                        {
                            if (target_robot.id == rb.kill_id[i])
                            {
                                flag = false;
                            }
                        }
                    }

                    if (flag)
                    {
                        target_robot.isAlive = false;
                        robot.kill++;
                        robot.kill_id[robot.kill - 1] = target_robot.id;
                        File.AppendAllText(logpath, "Kill. id: " + robot.id + "; pos: (" + robot.X + ", " + robot.Y + "); target: " + target_robot.id + Environment.NewLine, Encoding.UTF8);
                    }
                }

                target_robot.defence = (target_robot.defence < 0) ? 0 : target_robot.defence;
                target_robot.energy = (target_robot.energy < 0) ? 0 : target_robot.energy;
            }

            robot.defence = (robot.defence < 0) ? 0 : robot.defence;
            robot.attack = (robot.attack < 0) ? 0 : robot.attack;
            robot.speed = (robot.speed < 0) ? 0 : robot.speed;


            if (robot.energy <= 0)
            {
                robot.isAlive = false;

                File.AppendAllText(logpath, "Death. id: " + robot.id + "; pos: (" + robot.X + ", " + robot.Y + ")" + Environment.NewLine, Encoding.UTF8);
            }

            // health redistribution
            if(GetHealth() > 0)
            {
                if ((Math.Abs(action.dA) + Math.Abs(action.dD) + Math.Abs(action.dV) != 0) && (Math.Abs(action.dA) + Math.Abs(action.dD) + Math.Abs(action.dV) <= 2 * round_config.dHealth) && (robot.speed + action.dV <= round_config.max_speed) && (action.dA + action.dD + action.dV == 0))
                {
                    File.AppendAllText(logpath, "Health redistribution. id: " + robot.id + "; pos: (" + robot.X + "," + robot.Y + "); attack: " + robot.attack + ", " + action.dA + "; defence: " + robot.defence + ", " + action.dD + "; speed: " + robot.speed + ", " + action.dV + Environment.NewLine, Encoding.UTF8);

                    robot.attack += action.dA;
                    robot.defence += action.dD;
                    robot.speed += action.dV;
                }
            }
           

            //robot.energy = (robot.energy > round_config.max_energy) ? round_config.max_energy : robot.energy;
            robot.energy = Math.Min(robot.energy, round_config.max_energy);
        }

        public void Loop(IList<Tuple<string, IRobot>> robots, int round, int dN)
        {
            logpath = "../../log_" + round + ".txt";
            File.WriteAllText(logpath, "round: " + round + Environment.NewLine, Encoding.UTF8);

            RoundConfig round_config = config.rounds[round];
            state.robots.Clear();


            for (int robot_id = 0; robot_id < robots.Count - dN; robot_id++)
            {
                RobotState robot = new RobotState
                {
                    id = robot_id,
                    X = SingleRandom.Instance.Next(0, round_config.width),
                    Y = SingleRandom.Instance.Next(0, round_config.height),
                    energy = round_config.max_energy,
                    attack = 45,
                    speed = 10,
                    defence = 45,
                    isAlive = true,
                    kill = 0,
                    kill_id = new int[robots.Count],
                    name = robots[robot_id].Item2.Name,
                    colour = robots[robot_id].Item2.Colour,
                    dll_path = robots[robot_id].Item1
                };
                state.robots.Add(robot);
            }
            int robot_id_no_iter = robots.Count - dN;
            int[] unique_id= new int[dN+1];
            if (dN > 0)
            {
                for (int unic = 0; unic < dN + 1; unic++)
                {
                    unique_id[unic] = -1;
                }
            }
            bool isUnic;
            foreach (RobotState rs in future_robots)
            {
                for (int robot_id = robots.Count - dN; robot_id < robots.Count; robot_id++)
                {
                    isUnic = true;
                    for (int unic = 0; unic <= robot_id_no_iter - (robots.Count - dN); unic++)
                    {
                        if (robot_id == unique_id[unic])
                        {
                            isUnic = false;
                        }
                    }

                    if ((rs.isAlive == true) && (robots[robot_id].Item2.Name == rs.name) && (isUnic))
                    {
                        RobotState rs_copy = rs.ShallowCopy();
                        rs_copy.id = robot_id_no_iter++;

                        for (int kc = 0; kc < rs_copy.kill; kc++)
                        {
                            rs_copy.kill_id[kc] = 0;
                        }
                        rs_copy.kill = 0;
                        unique_id[robot_id_no_iter - (robots.Count - dN)] = robot_id;
                        state.robots.Add(rs_copy);
                        break;
                    }
                }
            }

            state.points.Clear();
            for (int i = 0; i < round_config.nEnergy; i++)
            {
                Point point = new Point
                {
                    X = SingleRandom.Instance.Next(0, round_config.width),
                    Y = SingleRandom.Instance.Next(0, round_config.height),
                    type = PointType.Energy
                };
                state.points.Add(point);
            }

            for (int i = 0; i < round_config.nHealth; i++)
            {
                Point point = new Point
                {
                    X = SingleRandom.Instance.Next(0, round_config.width),
                    Y = SingleRandom.Instance.Next(0, round_config.height),
                    type = PointType.Health
                };
                state.points.Add(point);
            }

            for (int n = 0; n < round_config.steps; n++)
            {
                future_robots.Clear();
                File.AppendAllText(logpath, "step: " + n + Environment.NewLine, Encoding.UTF8);

                for (int i = 0; i < robots.Count; i++)
                {
                    IRobot robot = robots[i].Item2;
                    Task<RobotAction> task = Task.Run(() => robot.Tick(i, round_config, state.DeepCopy()));

                    try
                    {
                        UpdateState(round_config, i, task.Wait(round_config.timeout) ? task.Result : null);
                        File.AppendAllText(logpath, state.robots[i].id +": " + state.robots[i].X + "x" + state.robots[i].Y + "; health: " + (state.robots[i].attack + state.robots[i].defence + state.robots[i].speed) + "; energy: " + state.robots[i].energy + Environment.NewLine, Encoding.UTF8);
                    }
                    catch (Exception e)
                    {
                        if (task.IsFaulted)
                        {
                            File.AppendAllText(logpath, "robot \"" + robot.Name + "\" failed" + Environment.NewLine, Encoding.UTF8);
                        }
                        else
                        {
                            File.AppendAllText(logpath, "oh no" + Environment.NewLine, Encoding.UTF8);
                        }
                        File.AppendAllText(logpath, e.Message + Environment.NewLine, Encoding.UTF8);

                        robots.RemoveAt(i);
                    }
                }

                if (n < round_config.steps - 1)
                {
                    state.robots.Clear();
                    foreach (RobotState rs in future_robots)
                    {
                        state.robots.Add(rs);
                    }
                }
                else if (n == round_config.steps - 1)
                {
                    for (int k = 0; k < future_robots.Count; k++)
                    {
                        if (future_robots[k].isAlive)
                        {
                            future_robots[k].energy += round_config.K * future_robots[k].kill;
                        }
                        else
                        {
                            future_robots[k].energy += round_config.K * future_robots[k].kill / 2;
                        }
                    }
                }

                Draw.Update(state.robots, state.points, round_config.width, round_config.height);

                for (int i = 0; i < future_robots.Count; i++)
                {
                    if (future_robots[i].energy < 0)
                    {
                        future_robots[i].energy = 0;
                    }

                    if (future_robots[i].defence + future_robots[i].attack + future_robots[i].speed < 0)
                    {
                        future_robots[i].defence = 0;
                        future_robots[i].attack = 0;
                        future_robots[i].speed = 0;
                    }
                    /*
                    if (i % 2 == 0)
                    {
                        int energyValue = future_robots[i].energy / 10;
                        Form1.pbList[i].Value = energyValue;
                    }
                    else
                    {
                        Form1.pbList[i].Value = future_robots[i].defence + future_robots[i].attack + future_robots[i].speed;
                    }
                    */
                }
            }
        }
    }
}
