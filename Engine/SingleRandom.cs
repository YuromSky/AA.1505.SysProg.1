using System;

namespace Engine
{
    public class SingleRandom : Random
    {
        static SingleRandom instance;

        public static void Seed(int Seed)
        {
            instance = new SingleRandom(Seed);
        }

        public static SingleRandom Instance
        {
            get
            {
                if (instance == null)
                    instance = new SingleRandom();
                return instance;
            }
        }

        private SingleRandom()
        {
        }

        private SingleRandom(int Seed) : base(Seed)
        {
        }
    }
}
