using System.Drawing;

namespace AntColony
{
    public class AntHill : Node
    {
        public AntHill(Point position) : base(position, false)
        {
        }

        protected override Brush getBrush()
        {
            return Utils.ANTHILL_BRUSH;
        }
    }
}