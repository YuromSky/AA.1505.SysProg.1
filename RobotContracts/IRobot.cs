using System;
using System.Collections.Generic;

namespace RobotContracts
{
    public interface IRobot
    {
        string Name
        {
            get;
        }
        int Colour
        {
            get;
        }

        RobotAction Tick(int robotId, RoundConfig config, GameState state);
    }
}
