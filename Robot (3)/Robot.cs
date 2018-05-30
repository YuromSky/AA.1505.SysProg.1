using System;
using RobotContracts;

namespace Robot
{
    public class Robot : IRobot
    {
        public string Name
        {
            get
            {
                return "Baseline Robot";
            }
        }

        public RobotAction Tick(int robotId, RoundConfig config, GameState state)
        {
            RobotState self = state.robots[robotId];
            RobotAction action = new RobotAction();

            Random rng = new Random();
            action.targetId = -1;
            if (rng.Next(0, 2) > 0)
                action.dX = 1;

            if (rng.Next(0, 2) > 0)
                action.dY = 1;

            return action;
        }
    }
}
