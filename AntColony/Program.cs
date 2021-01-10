using ActressMas;
using System;
using System.Diagnostics;

namespace AntColony
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var env = new TurnBasedEnvironment(0, Utils.DELAY);
            var world = new World();
            var worldAgent = new WorldAgent(world, "worldAgent", stopwatch);

            env.Add(worldAgent);

            for (var i = 1; i <= Utils.ANT_COUNT; i++)
            {
                var explorerAgent = new AntAgent("antAgent" + i);
                env.Add(explorerAgent);
            }

            env.Start();
        }
    }
}