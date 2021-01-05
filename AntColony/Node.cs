using System.Drawing;

namespace AntColony
{
    public class Node : IDrawableEntity
    {
        public Point Position { get; set; }
        public bool HasFood { get; set; }
        public int FoodQuantity { get; set; }

        public void DecreaseFoodQuantity()
        {
            if (!HasFood || FoodQuantity <= 0)
            {
                return;
            }

            --FoodQuantity;

            if (FoodQuantity == 0)
            {
                HasFood = false;
            }
        }

        public virtual void Draw(Graphics g)
        {
            g.FillEllipse(GetBrush(), Position.X - Utils.NODE_WIDTH / 2, Position.Y - Utils.NODE_HEIGHT / 2,
                Utils.NODE_WIDTH, Utils.NODE_HEIGHT);
        }

        protected virtual Brush GetBrush()
        {
            return HasFood ? Utils.FOOD_NODE_BRUSH : Utils.EMPTY_NODE_BRUSH;
        }

        public bool Equals(Node other)
        {
            return (Position.X == other.Position.X) && (Position.Y == other.Position.Y);
        }
    }
}