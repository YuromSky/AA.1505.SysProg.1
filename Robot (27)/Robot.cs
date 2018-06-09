using System;
using RobotContracts;
using System.Collections.Generic;

namespace Robot
{
    public class Robot : IRobot
    {
        public string Name
        {
            get
            {
                return "Ryzhov";
            }
        }
        public int Colour
        {
            get
            {
                return 2;
            }
        }


        public bool move = true;
        public int step = 0;
        public int round = 0;
        public bool done = false;
        coords target_cd_extra = new coords();
        bool ooouch = false;
        protected int getAliveRobots(List<string> FR, GameState gc)
        {
            int count = 0;
            foreach (RobotState r in gc.robots)
            {
                if ((r.isAlive == true) && (!FR.Contains(r.name)))
                    count++;
            }
            return count;
        }

        public RobotAction Tick(int robotId, RoundConfig config, GameState state)
        {
            coords cd = new coords();
            coords target_cd = new coords();
            
            RobotState self = state.robots[robotId];
            RobotAction action = new RobotAction();

            List<string> friendRobots = new List<string>() { "Ryzhov", "Haritonov", "Nikandrov", "Dmitrakov", "Sinyavsky", "Frolov", "Orlov", "Kamshilov" };

            int hp = self.attack + self.defence + self.speed;
            bool check = true;
            List<int> NT = new List<int>();
            List<int> RT = new List<int>();
            if (round == 4)
            {
                PumpDefence(ref action, self);
                target_cd = getNearestEnergy(friendRobots, state, self);
                cd = MotionTo(config, self, target_cd);
                action.targetId = -1;
                action.dX = cd.X;
                action.dY = cd.Y;
                return action;
            }


            if (step >= config.steps - 20)
            {
                target_cd = getNearestEnergy(friendRobots, state, self);
                cd = MotionTo(config, self, target_cd);
                action.targetId = -1;
                PumpDefence(ref action, self);
                if (step == config.steps - 1)
                {
                    step = 0;
                    move = true;
                    done = false;
                    ooouch = false;
                    round++;
                    return action;
                }

                }
            else
            {
                if (getAliveRobots(friendRobots, state) == 0)
                {
                    target_cd = getNearestEnergy(friendRobots, state, self);

                    cd = MotionTo(config, self, target_cd);
                    action.targetId = -1;
                }
                else
                {
                    if ((self.energy >= 0.85*config.max_energy) && (hp >= config.max_health) && (move == true))
                    {
                        //PumpAttack(ref action, self);
                        if (check)
                        {
                            PumpAttack(ref action, self);
                            if ((self.attack - self.defence >= 20) && (self.attack - self.defence <= 30))
                                check = false;
                        }

                        NT = getNearestTargets(friendRobots, config, state, self);
                        if (NT.Count == 0)
                        {
                            RT = getRadiusTargets(friendRobots, config, state, self);
                            target_cd = getNearestRobot(RT, config, state, self);

                            cd = MotionTo(config, self, target_cd);
                            action.targetId = -1;

                        }
                        else
                        {
                            action.targetId = getWeakestTarget(NT, config, state, self);

                        }

                    }
                    else
                    {


                        if ((hp < 100) && (ooouch==false))
                        {
                            ooouch = true;
                            target_cd_extra = getFurtherEnergy(friendRobots, state, self);
                        }
                        
                            

                        move = false;
                        if ((self.energy <= 999) && (done == false))
                        {
                            if(ooouch)
                            {
                                cd = MotionTo(config, self, target_cd_extra);
                                action.dX = cd.X;
                                action.dY = cd.Y;
                            }
                            else
                            {
                                target_cd = getNearestEnergy(friendRobots, state, self);
                                cd = MotionTo(config, self, target_cd);

                                action.targetId = getWeakestTarget(getNearestTargets(friendRobots, config, state, self), config, state, self);
                            }
    
                            if (self.energy >= 999)
                            {
                                done = true;
                            }

                        }
                        else
                        {
                            done = true;
                        }


                        if ((hp < 100) && (done == true))
                        {
                            PumpDefence(ref action, self);
                            target_cd = getNearestHealth(friendRobots, state, self);
                            cd = MotionTo(config, self, target_cd);
                            action.targetId = -1;
                            if (hp >= 100)
                            {
                                done = false;
                            }

                        }
                        else
                        {
                            done = false;
                        }

                        if ((self.energy >= 999) && (hp >= 100))
                        {
                            move = true;
                            check = true;
                            ooouch = false;
                        }


                    }


                }
            }

            action.dX = cd.X;
            action.dY = cd.Y;
            step++;
            return action;
        }

        public struct coords
        {
            public int X;
            public int Y;
        }




        protected void PumpAttack(ref RobotAction action, RobotState myself)
        {

            if (myself.defence > 10)
            {
                action.dD = -10;
                action.dA = 10;
            }

        }

        protected void PumpDefence(ref RobotAction action, RobotState myself)
        {

            if (myself.attack > 10)
            {
                action.dA = -10; ;
                action.dD = 10;
            }

        }
        protected int Sign(int i)
        {
            if (i > 0)
                return 1;
            if (i < 0)
                return -1;
            else
                return 0;
        }
        protected coords MotionTo(RoundConfig config, RobotState myself, coords cd)
        {
            coords res_cd = new coords();
            coords try_cd = new coords();

            int max_distance = 10 * config.max_speed * myself.speed / config.max_health * myself.energy / config.max_energy;



            int dx = cd.X - myself.X;

            int dy = cd.Y - myself.Y;

            coords sign_cd = new coords();
            sign_cd.X = Sign(dx);
            sign_cd.Y = Sign(dy);

            res_cd.X = 0;
            res_cd.Y = 0;
            if (getDistance(myself.X, myself.Y, cd.X, cd.Y) < max_distance)
            {
                res_cd.X = dx;
                res_cd.Y = dy;
            }
            else
            {

                try_cd.X = 0;
                try_cd.Y = 0;

                if (dx == 0)
                {
                    res_cd.X = 0;
                    res_cd.Y = sign_cd.Y * max_distance;
                    return res_cd;
                }

                if (dy == 0)
                {
                    res_cd.X = sign_cd.X * max_distance;
                    res_cd.Y = 0;
                    return res_cd;
                }


                bool sw = false;
                while (Pifagor(try_cd.X, try_cd.Y) <= max_distance)
                {
                    if (sw)
                    {
                        try_cd.X += sign_cd.X;
                        if (Pifagor(try_cd.X, try_cd.Y) > max_distance)
                            break;
                        else
                            res_cd.X += sign_cd.X;

                    }
                    else
                    {
                        try_cd.Y += sign_cd.Y;
                        if (Pifagor(try_cd.X, try_cd.Y) > max_distance)
                            break;
                        else
                            res_cd.Y += sign_cd.Y;

                    }
                    sw = !sw;
                }

            }
            return res_cd;
        }

        protected int getDistance(int X1, int Y1, int X2, int Y2)
        {
            int dx = Math.Abs(X2 - X1);
            int dy = Math.Abs(Y2 - Y1);
            return (int)Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
        }

        protected coords getNearestEnergy(List<string> friends, GameState gs, RobotState myself)
        {
            int dist = 999999999;
            coords cd = new coords();
            foreach (Point p in gs.points)
            {
                foreach (RobotState rs in gs.robots)
                {
                    if((p.X == rs.Y) && (p.Y == rs.X) && (friends.Contains(rs.name)))
                    {
                        cd.X = p.X;
                        cd.Y = p.Y;
                        dist = getDistance(myself.X, myself.Y, p.X, p.Y);
                    }
                    else
                    {
                        if ((p.type == PointType.Energy) && (getDistance(myself.X, myself.Y, p.X, p.Y) < dist) && (p.X != rs.Y) && (p.Y != rs.X) && (rs.isAlive == true) && (rs.id != myself.id))
                        {
                            cd.X = p.X;
                            cd.Y = p.Y;
                            dist = getDistance(myself.X, myself.Y, p.X, p.Y);
                        }
                    }
                    
                }

            }
            //if (dist == 999999999)
            //    cd = NearestEnergy(gs, myself);

            return cd;
        }

        //protected coords NearestEnergy(GameState gs, RobotState myself)
        //{
        //    int dist = 999999999;
        //    coords cd = new coords();
        //    foreach (Point p in gs.points)
        //    {
        //        if ((p.type == PointType.Energy) && (getDistance(myself.X, myself.Y, p.X, p.Y) < dist) && (p.X != rs.Y) && (p.Y != rs.X) && (rs.isAlive == true) && (rs.id != myself.id) && (friends.Contains(rs.name)))
        //        {
        //            cd.X = p.X;
        //            cd.Y = p.Y;
        //            dist = getDistance(myself.X, myself.Y, p.X, p.Y);
        //        }

        //    }
        //    return cd;
        //}

        protected coords getFurtherEnergy(List<string> friends, GameState gs, RobotState myself)
        {
            int dist = 0;
            coords cd = new coords();




            foreach (Point p in gs.points)
            {
                foreach (RobotState rs in gs.robots)
                {
                    if ((p.X == rs.Y) && (p.Y == rs.X) && (friends.Contains(rs.name)))
                    {
                        cd.X = p.X;
                        cd.Y = p.Y;
                        dist = getDistance(myself.X, myself.Y, p.X, p.Y);
                    }
                    else
                    {
                        if ((p.type == PointType.Energy) && (getDistance(myself.X, myself.Y, p.X, p.Y) > dist) && (p.X != rs.Y) && (p.Y != rs.X) && (rs.isAlive == true) && (rs.id != myself.id))
                        {
                            cd.X = p.X;
                            cd.Y = p.Y;
                            dist = getDistance(myself.X, myself.Y, p.X, p.Y);
                        }
                    }

                }

            }



            return cd;
        }

        protected List<RobotState> getRobotsOnPoints(List<string> friends, GameState gc, RobotState self)
        {
            List<RobotState> RoP = new List<RobotState>();
            foreach (RobotState r in gc.robots)
            {
                foreach (Point p in gc.points)
                {
                    if ((p.type == PointType.Energy) && (p.X == r.X) && (p.Y == r.Y) && (r.isAlive==true) && (p.X != r.Y) && (p.Y != r.X) && (r.isAlive == true) && (r.id != self.id) && (!friends.Contains(r.name)))
                    {
                        RoP.Add(r);
                    }
                }
            }
            return RoP;
        }

        protected coords getFurtherHealth(List<string> friends, GameState gs, RobotState myself)
        {
            int dist = 0;
            coords cd = new coords();
            
            foreach (Point p in gs.points)
            {
                foreach(RobotState r in gs.robots)
                {
                    if ((p.type == PointType.Health) && (getDistance(myself.X, myself.Y, p.X, p.Y) > dist) && (p.X != r.Y) && (p.Y != r.X) && (r.isAlive == true) && (friends.Contains(r.name)))
                    {
                        cd.X = p.X;
                        cd.Y = p.Y;
                        dist = getDistance(myself.X, myself.Y, p.X, p.Y);
                    }
                }
                
            }
            return cd;
        }

        protected coords getNearestHealth(List<string> friends, GameState gs, RobotState myself)
        {
            int dist = 999999999;
            coords cd = new coords();
            foreach (Point p in gs.points)
            {
                foreach (RobotState rs in gs.robots)
                {
                    if ((p.X == rs.Y) && (p.Y == rs.X) && (friends.Contains(rs.name)))
                    {
                        cd.X = p.X;
                        cd.Y = p.Y;
                        dist = getDistance(myself.X, myself.Y, p.X, p.Y);
                    }
                    else
                    {
                        if ((p.type == PointType.Health) && (getDistance(myself.X, myself.Y, p.X, p.Y) < dist) && (p.X != rs.Y) && (p.Y != rs.X) && (rs.isAlive == true) && (rs.id != myself.id))
                        {
                            cd.X = p.X;
                            cd.Y = p.Y;
                            dist = getDistance(myself.X, myself.Y, p.X, p.Y);
                        }
                    }

                }

            }
            //if (dist == 999999999)
            //    cd = NearestEnergy(gs, myself);

            return cd;
        }



        public List<int> getNearestTargets(List<string> friends, RoundConfig config, GameState gs, RobotState myself)
        {
            int max_distance_attack = 10 * config.max_radius * myself.speed / config.max_health * myself.energy / config.max_energy;
            List<int> NT = new List<int>();

            foreach (RobotState rs in gs.robots)
            {
                if ((rs.isAlive == true) && (rs.id != myself.id) && (!friends.Contains(rs.name)) && (getDistance(myself.X, myself.Y, rs.X, rs.Y) < max_distance_attack))
                {
                    NT.Add(rs.id);
                }
            }
            return NT;
        }

        protected List<int> getRadiusTargets(List<string> friends, RoundConfig config, GameState gs, RobotState myself)
        {
            int max_distance_attack = 10 * config.max_radius * myself.speed / config.max_health * myself.energy / config.max_energy;
            List<int> RT = new List<int>();
            foreach (RobotState r in gs.robots)
            {
                if ((getDistance(myself.X, myself.Y, r.X, r.Y) < max_distance_attack) && (!friends.Contains(r.name)) && (r.id != myself.id))
                    RT.Add(r.id);

            }
            return RT;
        }
        protected coords getNearestRobot(List<int> RT, RoundConfig config, GameState gs, RobotState myself)
        {
            coords cd = new coords();
            int dist = 9999999;
            int id = -1;
            int min_def = 999999999;
            foreach (RobotState r in gs.robots)
            {
                if (!RT.Contains(r.id))
                {
                    if ((getDistance(myself.X, myself.Y, r.X, r.Y) < dist) && (r.isAlive == true) && (r.id != myself.id) && (myself.attack * myself.energy > r.defence * r.energy) && (r.defence * r.energy < min_def))
                    {

                        cd.X = r.X;
                        cd.Y = r.Y;
                        dist = getDistance(myself.X, myself.Y, r.X, r.Y);
                        min_def = r.defence * r.energy;
                    }
                }



            }

            return cd;
        }


        protected int getWeakestTarget(List<int> NT, RoundConfig config, GameState gs, RobotState myself)
        {
            int t = -1;
            coords cd = new coords();



            int min_def = 999999999;
            foreach (int r in NT)
            {

                if ((myself.attack * myself.energy > gs.robots[r].defence * gs.robots[r].energy) && (gs.robots[r].defence * gs.robots[r].energy < min_def))
                {
                    t = r;
                    min_def = gs.robots[r].defence * gs.robots[r].energy;
                }

            }

            return t;
        }

        protected int getStrongestTarget(List<int> NT, RoundConfig config, GameState gs, RobotState myself)
        {
            int t = -1;
            int max_def = 0;

            foreach (int r in NT)
            {

                if ((myself.attack * myself.energy < gs.robots[r].defence * gs.robots[r].energy) && (gs.robots[r].defence * gs.robots[r].energy > max_def))
                {
                    t = r;
                    max_def = gs.robots[r].defence * gs.robots[r].energy;
                }

            }


            return t;
        }
        protected int Pifagor(int X, int Y)
        {
            return (int)Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
        }
    }
}

