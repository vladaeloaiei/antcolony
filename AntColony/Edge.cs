using System;
using System.Drawing;

namespace AntColony
{
    public class Edge : DrawableEntity
    {
        private const int MAX_WEIGHT = 20;
        public Node NodeA { get; set; }
        public Node NodeB { get; set; }
        private int _weight = 0;

        public Edge(Node a, Node b)
        {
            NodeA = a;
            NodeB = b;
        }

        public void IncreaseWeight()
        {
            _weight = MAX_WEIGHT < (_weight + 2) ? MAX_WEIGHT : (_weight + 2);
        }

        public void DecreaseWight()
        {
            if (_weight > 0)
            {
                --_weight;
            }
        }

        public void draw(Graphics g)
        {
            g.DrawLine(getPen(), NodeA.Position.X, NodeA.Position.Y, NodeB.Position.X, NodeB.Position.Y);
        }

        protected Pen getPen()
        {
            _weight++; // TODO: do not forget to remove this
            // Get a thicker pen for a bigger weight
            return new Pen(Utils.PATH_BRUSH[_weight / (MAX_WEIGHT / 3)], Utils.PATH_WIDTH[_weight / (MAX_WEIGHT / 3)]);
        }
    }
}