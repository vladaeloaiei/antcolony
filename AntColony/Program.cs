using ActressMas;

namespace AntColony
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            var env = new TurnBasedEnvironment(0, Utils.DELAY);
            var world = new World();
            var worldAgent = new WorldAgent(world, "world");

            env.Add(worldAgent);

            // for (var i = 1; i <= Utils.ANT_COUNT; i++)
            {
                var explorerAgent = new AntAgent(world, "ant" + 1);
                env.Add(explorerAgent);
            }

            env.Start();
        }
    }
}