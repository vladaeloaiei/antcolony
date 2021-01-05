using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AntColony
{
    public class World
    {
        public Dictionary<Node, IList<Edge>> Graph { get; set; }
        public AntHill AntHill { get; set; }
        public IDictionary<string, Ant> Ants { get; set; }

        public World()
        {
            Ants = new Dictionary<string, Ant>();

            AntHill = new AntHill
            {
                Position = new Point(Utils.WORLD_WIDTH / 2, Utils.WORLD_HEIGHT / 2)
            };

            Graph = new Dictionary<Node, IList<Edge>>
            {
                {AntHill, new List<Edge>()}
            };

            GenerateNodes();
            GenerateEdges();
        }

        public void AddOrUpdateAnt(string antName, Ant ant)
        {
            if (!Ants.ContainsKey(antName))
            {
                Ants.Add(antName, ant);
            }
            else
            {
                Ants[antName] = ant;
            }
        }

        private void GenerateNodes()
        {
            for (var i = 0; i < Utils.NODE_COUNT; ++i)
            {
                Graph.Add(GenerateNewNode(), new List<Edge>());
            }
        }

        private Node GenerateNewNode()
        {
            Node node;

            do
            {
                var x = Utils.RandNoGen.Next(Utils.WORLD_WIDTH);
                var y = Utils.RandNoGen.Next(Utils.WORLD_HEIGHT);
                node = new Node { Position = new Point(x, y) };
            } while (!IsNodeOk(node));

            node.HasFood = Utils.RandomBool(Utils.FOOD_RATIO);
            node.FoodQuantity = node.HasFood ? Utils.FOOD_QUANTITY : 0;

            return node;
        }

        private bool IsNodeOk(Node node)
        {
            foreach (var existingNode in Graph.Keys)
            {
                if (AreClose(node.Position, existingNode.Position))
                {
                    return false;
                }
            }

            return true;
        }

        private bool AreClose(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) < Utils.MIN_DISTANCE_BETWEEN_NODES &&
                   Math.Abs(a.Y - b.Y) < Utils.MIN_DISTANCE_BETWEEN_NODES;
        }


        private void GenerateEdges()
        {
            //First, connect all nodes with a complete "circle"
            //Each node will have exactly 2 edges (one with the previous node and one with the next node)
            for (var i = 0; i < Graph.Keys.Count; ++i)
            {
                var nodeA = Graph.Keys.ElementAt(i);
                var nodeB = ((i + 1) < Graph.Keys.Count) ? Graph.Keys.ElementAt(i + 1) : Graph.Keys.ElementAt(0);

                Graph[nodeA].Add(new Edge(nodeA, nodeB));
                Graph[nodeB].Add(new Edge(nodeB, nodeA));
            }

            //Now, generate new edges for each node with a random one
            foreach (var node in Graph.Keys)
            {
                for (var i = 0; i < (Utils.EDGE_PER_NODE_COUNT - 2); ++i)
                {
                    Graph[node].Add(new Edge(node, Graph.Keys.ElementAt(Utils.RandNoGen.Next(Utils.NODE_COUNT))));
                }
            }
        }
    }
}