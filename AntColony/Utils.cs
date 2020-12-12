using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;

namespace AntColony
{
    public static class Utils
    {
        public static readonly Random RandNoGen = new Random();

        /// SIZES
        public const int MIN_DISTANCE_BETWEEN_NODES = 20;

        public const int WORLD_HEIGHT = 800;
        public const int WORLD_WIDTH = 800;

        public const int NODE_WIDTH = 10;
        public const int NODE_HEIGHT = 10;

        public const int ANT_WIDTH = 8;
        public const int ANT_HEIGHT = 8;

        /// COLORS
        public static Color BACKGROUND_COLOR = Color.White; //Color.FromArgb(64, 64, 64);

        public static Brush EMPTY_ANT_BRUSH = Brushes.Black;
        public static Brush FOOD_ANT_BRUSH = Brushes.DarkSeaGreen;
        public static Brush EMPTY_NODE_BRUSH = Brushes.Black;
        public static Brush FOOD_NODE_BRUSH = Brushes.YellowGreen;
        public static Brush ANTHILL_BRUSH = Brushes.Red;
        public static Brush[] PATH_BRUSH = {Brushes.LightSkyBlue, Brushes.CornflowerBlue, Brushes.RoyalBlue};
        public static float[] PATH_WIDTH = {1F, 2F, 3F};

        /// OTHER
        public const int NODE_COUNT = 20;

        public const int EDGE_PER_NODE_COUNT = 4;
        public const int ANT_COUNT = 5;
        public const int FOOD_RATIO = 50;
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
            var t = content.Split();

            action = t[0];
            parameters = "";

            if (t.Length > 1)
            {
                for (var i = 1; i < t.Length - 1; i++)
                {
                    parameters += t[i] + " ";
                }

                parameters += t[t.Length - 1];
            }
        }

        public static string Str(object p1, object p2)
        {
            return $"{p1} {p2}";
        }

        public static string Str(object p1, object p2, object p3)
        {
            return $"{p1} {p2} {p3}";
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