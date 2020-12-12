using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;


namespace AntColony
{
    public class World
    {
        public Dictionary<Node, List<Edge>> Graph { get; set; }
        public AntHill AntHill { get; set; }

        public World()
        {
            //Place the anthill in the center 
            AntHill = new AntHill(new Point(Utils.WORLD_WIDTH / 2, Utils.WORLD_HEIGHT / 2));
            Graph = new Dictionary<Node, List<Edge>>();
            Graph.Add(AntHill, new List<Edge>());
            generateNodes();
            generateEdges();
        }

        private void generateNodes()
        {
            for (var i = 0; i < Utils.NODE_COUNT; ++i)
            {
                Graph.Add(generateNewNode(), new List<Edge>());
            }
        }

        private Node generateNewNode()
        {
            Node node;

            do
            {
                var x = Utils.RandNoGen.Next(Utils.WORLD_WIDTH);
                var y = Utils.RandNoGen.Next(Utils.WORLD_HEIGHT);
                node = new Node(new Point(x, y), false);
            } while (!isNodeOk(node));

            //Set food
            node.HasFood = Utils.RandomBool(Utils.FOOD_RATIO);
            return node;
        }

        private bool isNodeOk(Node node)
        {
            foreach (var existingNode in Graph.Keys)
            {
                if (areClose(node.Position, existingNode.Position))
                {
                    return false;
                }
            }

            return true;
        }

        private bool areClose(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) < Utils.MIN_DISTANCE_BETWEEN_NODES &&
                   Math.Abs(a.Y - b.Y) < Utils.MIN_DISTANCE_BETWEEN_NODES;
        }


        private void generateEdges()
        {
            foreach (var node in Graph.Keys)
            {
                for (var i = 0; i < Utils.EDGE_PER_NODE_COUNT; ++i)
                {
                    Graph[node].Add(generateNewEdge(node));
                }
            }
        }

        private Edge generateNewEdge(Node a)
        {
            Edge edge;

            do
            {
                var b = Graph.Keys.ElementAt(Utils.RandNoGen.Next(Utils.NODE_COUNT));

                edge = new Edge(a, b);
            } while (!isEdgeOk(edge));

            return edge;
        }

        private bool isEdgeOk(Edge edge)
        {
            foreach (var existingEdge in Graph[edge.NodeA])
            {
                if (edge.NodeB.Equals(existingEdge.NodeB))
                {
                    return false;
                }
            }

            return true;
        }
    }
}