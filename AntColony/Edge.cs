using System.Drawing;

namespace AntColony
{
    public class Edge : IDrawableEntity
    {
        private const int MAX_WEIGHT = 20;
        public Node NodeA { get; set; }
        public Node NodeB { get; set; }

        public int Weight = 1;

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
            if (Weight > 0)
            {
                --Weight;
            }
        }

        public void Draw(Graphics g)
        {
            g.DrawLine(GetPen(), NodeA.Position.X, NodeA.Position.Y, NodeB.Position.X, NodeB.Position.Y);
        }

        protected Pen GetPen()
        {
            if (Weight > 2)
                return new Pen(Utils.PATH_BRUSH[2], Utils.PATH_WIDTH[2]);
            else
                return new Pen(Utils.PATH_BRUSH[0], Utils.PATH_WIDTH[0]);
        }
    }
}