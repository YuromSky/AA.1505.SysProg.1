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
				return "Gasanov Robot";
			}
        }
        public int Colour
        {
            get
            {
                return 2;
            }
        }
		public int Sign(int i)
		{
			if (i > 0)
				return 1;
			if (i < 0)
				return -1;
			else
				return 0;
		}
		public class coords
		{
			public int x;
			public int y;
		}

		public int TakeDistance(int x1, int y1, int x2, int y2)
		{
			return (int)Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
		}

		public int Pifagor(int x, int y)
		{
			return (int)Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
		}

		public coords MoveTo(RobotState self, RoundConfig config, coords coords)
		{
			int maxdistance = 10 * config.max_speed * self.speed / config.max_health * self.energy / config.max_energy;

			coords Move = new coords();
			coords finalPosition = new coords();
			coords tryPosition = new coords();
			int DX = coords.x - self.X;
			int DY = coords.y - self.Y;
			bool sw = false;
			coords sign_cd = new coords();
			sign_cd.x = Sign(DX);
			sign_cd.y = Sign(DY);

			finalPosition.x = 0;
			finalPosition.y = 0;
			if (TakeDistance(self.X, self.Y, coords.x, coords.y) < maxdistance)
			{
				finalPosition.x = DX;
				finalPosition.y = DY;
			}
			else
			{
				tryPosition.x = 0;
				tryPosition.y = 0;

				while (Pifagor(tryPosition.x, tryPosition.y) <= maxdistance)
				{
					if (sw)
					{
						tryPosition.x += sign_cd.x;
						if (Pifagor(tryPosition.x, tryPosition.y) > maxdistance)
							break;
						else
							finalPosition.x += sign_cd.x;
					}
					else
					{
						tryPosition.y += sign_cd.y;
						if (Pifagor(tryPosition.x, tryPosition.y) > maxdistance)
							break;
						else
							finalPosition.y += sign_cd.y;
					}
					sw = !sw;
				}

			}
			return finalPosition;
		}



		private Point GetNearestStation(int Id, RoundConfig config, GameState state, PointType type)
		{
			RobotState self = state.robots[Id];

			int pointDistance = config.width * config.height;
			int pointId = 0;
			for (int id = 0; id < state.points.Count; id++)
			{
				Point pt = state.points[id];
				if (pt.type == type)
				{
					int ptDistance = TakeDistance(self.X, self.Y, pt.X, pt.Y);
					if (ptDistance < pointDistance)
					{
						pointDistance = ptDistance;
						pointId = id;
					}
				}
			}

			return state.points[pointId];
		}

		private int GetNearestRobot(int Id, RoundConfig config, GameState state, bool enemyOnly, bool aliveOnly)
		{
			RobotState self = state.robots[Id];

			int targetDistance = config.width * config.height;
			int targetId = -1;
			for (int id = 0; id < state.robots.Count; id++)
			{
				RobotState rs = state.robots[id];

				if (!enemyOnly || rs.name != self.name)
				{
					if (!aliveOnly || rs.isAlive)
					{
						int rsDistance = TakeDistance(self.X, self.Y, rs.X, rs.Y);
						if (rsDistance < targetDistance)
						{
							targetDistance = rsDistance;
							targetId = id;
						}
					}
				}
			}

			return targetId;
		}

		private void HealthRedestribution(RobotState self, RoundConfig config, RobotAction action, float attack, float defence, float speed)
		{
			int health = self.attack + self.defence + self.speed;
			action.dA = (int)((attack - self.attack / (float)health) * (config.dHealth - 3));
			action.dD = (int)((defence - self.defence / (float)health) * (config.dHealth - 3));
			action.dV = (int)((speed - self.speed / (float)health) * (config.dHealth - 3));

			if (self.speed + action.dV > config.max_speed)
			{
				int delta = action.dV - (config.max_speed - self.speed);
				action.dA += delta / 2;
				action.dD += delta / 2;
				action.dV -= delta;
			}

			int sum = action.dA + action.dD + action.dV;
			if (sum != 0)
			{
				action.dV -= sum;
			}
		}

			public RobotAction Tick(int robotId, RoundConfig config, GameState state)
            {
			int MinDistance = 999999;
			coords NextCoords = new coords();

            RobotState self = state.robots[robotId];
            RobotAction action = new RobotAction();


			action.targetId = -1;



			foreach (Point P in state.points)
			{
				int a = TakeDistance(self.X, self.Y, P.X, P.Y);
				if(P.type==PointType.Energy && (a < MinDistance))
				{
					MinDistance = a;
					NextCoords.x = P.X;
					NextCoords.y = P.Y;
				}
			}

			int enemy_id = -1;
			coords destination = new coords();
			destination = MoveTo(self, config, NextCoords);

			coords destination2 = new coords();
			destination2 = MoveTo(self, config, NextCoords);

			action.dX = destination.x;
			action.dY = destination.y;

			int maxdefdistance = 10 * config.max_radius * self.speed / config.max_health * self.energy / config.max_energy;
			bool attacked = false;
			int enemy_id1 = -1;
			for (int id = 0; id < state.robots.Count; id++)
			{
				RobotState rstates = state.robots[id];
				if (rstates.name != self.name)
				{
					int enemy_distance_attack = 10 * config.max_radius * rstates.speed / config.max_health * rstates.energy / config.max_energy;
					int distance = TakeDistance(self.X, self.Y, rstates.X, rstates.Y);
					if (distance <= enemy_distance_attack && distance <= maxdefdistance)
					{
						attacked = true;
						enemy_id = id;
						action.targetId = enemy_id;
					}
				}
			}

			if ((self.attack + self.defence + self.speed) < 0.4 * config.max_health)
			{
				action.dX = destination2.x;
				action.dY = destination2.y;
			}
			else if (self.energy < 0.4 * config.max_energy)
			{
				action.dX = destination.x;
				action.dY = destination.y;
			}
			return action;
		}
    }
}
