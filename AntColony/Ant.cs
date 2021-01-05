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
            g.FillEllipse(GetBrush(), CurrentNode.Position.X, CurrentNode.Position.Y, Utils.ANT_WIDTH, Utils.ANT_HEIGHT);
        }


        private Brush GetBrush()
        {
            return (State == AntState.Carrying) ? Utils.FOOD_ANT_BRUSH : Utils.EMPTY_ANT_BRUSH;
        }
    }
}
