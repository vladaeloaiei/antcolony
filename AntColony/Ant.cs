using System.Drawing;

namespace AntColony
{
    public class Ant : IDrawableEntity
    {
        public Node CurrentNode { get; set; }
        public AntState State { get; set; }

        public Ant(Node currentNode)
        {
            CurrentNode = currentNode;
            State = AntState.Free;
        }

        public enum AntState
        {
            Free,
            Carrying
        };

        public void Draw(Graphics g)
        {
            int pozX, pozY;
            pozX = (State == AntState.Carrying) ? CurrentNode.Position.X - Utils.ANT_WIDTH : CurrentNode.Position.X;
            pozY = (State == AntState.Carrying) ? CurrentNode.Position.Y - Utils.ANT_HEIGHT : CurrentNode.Position.Y;

            g.FillEllipse(GetBrush(), pozX, pozY, Utils.ANT_WIDTH, Utils.ANT_HEIGHT);
        }


        private Brush GetBrush()
        {
            return (State == AntState.Carrying) ? Utils.FOOD_ANT_BRUSH : Utils.EMPTY_ANT_BRUSH;
        }
    }
}
