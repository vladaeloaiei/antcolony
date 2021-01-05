using System.Drawing;

namespace AntColony
{
    public class AntHill : Node
    {
        public override void Draw(Graphics g)
        {
            g.FillRectangle(GetBrush(), Utils.WORLD_WIDTH / 2, Utils.WORLD_HEIGHT / 2, Utils.ANTHILL_WIDTH, Utils.ANTHILL_HEIGHT);
        }

        protected override Brush GetBrush()
        {
            return Utils.ANTHILL_BRUSH;
        }
    }
}