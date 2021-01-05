using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace AntColony
{
    public static class Utils
    {
        public static readonly Random RandNoGen = new Random();

        public const int VERSION = 1;

        /// SIZES
        public const int MIN_DISTANCE_BETWEEN_NODES = 20;

        public const int WORLD_HEIGHT = 800;
        public const int WORLD_WIDTH = 800;

        public const int NODE_WIDTH = 12;
        public const int NODE_HEIGHT = 12;

        public const int ANT_WIDTH = 8;
        public const int ANT_HEIGHT = 8;

        public const int ANTHILL_WIDTH = 20;
        public const int ANTHILL_HEIGHT = 20;

        /// COLORS
        public static Color BACKGROUND_COLOR = Color.White;

        public static Brush EMPTY_ANT_BRUSH = Brushes.Blue;
        public static Brush FOOD_ANT_BRUSH = Brushes.DarkBlue;
        public static Brush EMPTY_NODE_BRUSH = Brushes.Black;
        public static Brush FOOD_NODE_BRUSH = Brushes.YellowGreen;
        public static Brush ANTHILL_BRUSH = Brushes.Red;
        public static Brush[] PATH_BRUSH = {Brushes.LightSkyBlue, Brushes.CornflowerBlue, Brushes.RoyalBlue};
        public static float[] PATH_WIDTH = {1F, 2F, 3F};

        /// OTHER
        public const int NODE_COUNT = 10;

        public const int EDGE_PER_NODE_COUNT = 3;
        public const int ANT_COUNT = 2;
        public const int FOOD_RATIO = 50;
        public const int FOOD_QUANTITY = 3;
        public const int DELAY = 1000;

        public static void ParseMessage(string content, out string action, out List<string> parameters)
        {
            var t = content.Split();

            action = t[0];

            parameters = new List<string>();
            for (var i = 1; i < t.Length; i++)
                parameters.Add(t[i]);
        }

        public static void ParseMessage(string content, out string action, out string parameters)
        {
            var t = content.Split(new[] { ' ' }, 2);

            action = t[0];
            parameters = t.Length == 2 ? t[1] : string.Empty;
        }

        public static string Serialize(string action, object obj)
        {
            return $"{action} {JsonConvert.SerializeObject(obj)}";
        }

        /// <summary>
        /// Generates a random true/false based on the input ratio.
        /// For 50 ratio, the chances are 50/50 true/false
        /// For 20 ratio, the chances are 20/80 true/false
        /// </summary>
        /// <param name="ratio">Value between 0 - 100</param>
        /// <returns>true/false</returns>
        public static bool RandomBool(int ratio)
        {
            return RandNoGen.Next(100) <= ratio;
        }
    }
}