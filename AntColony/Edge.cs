using System.Drawing;

namespace AntColony
{
    public class Edge : IDrawableEntity
    {
        private const int MAX_WEIGHT = 20;
        public Node NodeA { get; set; }
        public Node NodeB { get; set; }

        public double Weight = 0;

        public Edge(Node a, Node b)
        {
            NodeA = a;
            NodeB = b;
        }

        public void IncreaseWeight()
        {
            Weight = MAX_WEIGHT < (Weight + 2) ? MAX_WEIGHT : (Weight + 2);
        }

        public void DecreaseWight()
        {
            if (Weight - Utils.EDGE_DECREASE > 0)
            {
                Weight -= Utils.EDGE_DECREASE;
            }
            else
            {
                Weight = 0;
            }
        }

        public void Draw(Graphics g)
        {
            g.DrawLine(GetPen(), NodeA.Position.X, NodeA.Position.Y, NodeB.Position.X, NodeB.Position.Y);
        }

        protected Pen GetPen()
        {
            if (Weight > 1)
                return new Pen(Utils.PATH_BRUSH[1], Utils.PATH_WIDTH[1]);
            if (Weight > 2)
                return new Pen(Utils.PATH_BRUSH[2], Utils.PATH_WIDTH[2]);
            else
                return new Pen(Utils.PATH_BRUSH[0], Utils.PATH_WIDTH[0]);
        }

        public new bool Equals(object edge)
        {
            var edgeObj = (Edge)edge;

            return (NodeA .Equals(edgeObj.NodeA) && NodeB.Equals(edgeObj.NodeB)) || (NodeA.Equals(edgeObj.NodeB) && NodeB.Equals(edgeObj.NodeA));
        }
    }
}