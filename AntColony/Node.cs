using System;
using System.Drawing;

namespace AntColony
{
    public class Node : DrawableEntity
    {
        public Point Position { get; set; }
        public bool HasFood { get; set; }
        public int FoodQuantity { get; set; }

        public Node(Point position, bool hasFood)
        {
            Position = position;
            HasFood = hasFood;
        }

        public void draw(Graphics g)
        {
            g.FillEllipse(getBrush(), Position.X - Utils.NODE_WIDTH / 2, Position.Y - Utils.NODE_HEIGHT / 2,
                Utils.NODE_WIDTH, Utils.NODE_HEIGHT);
        }

        protected virtual Brush getBrush()
        {
            return HasFood ? Utils.FOOD_NODE_BRUSH : Utils.EMPTY_NODE_BRUSH;
        }

        public bool Equals(Node other)
        {
            return (Position.X == other.Position.X) && (Position.Y == other.Position.Y);
        }
    }
}