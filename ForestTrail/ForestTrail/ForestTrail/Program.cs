using System;

namespace ForestTrail
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ForestTrail game = new ForestTrail())
            {
                game.Run();
            }
        }
    }
#endif
}

