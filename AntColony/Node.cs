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

        public void IncreaseFoodQuantity()
        {
            if(Position.X == Utils.WORLD_WIDTH / 2 && Position.Y == Utils.WORLD_HEIGHT / 2)
            {
                return;
            }

            if (!HasFood)
            {
                HasFood = true;
            }

            ++FoodQuantity;
        }

        public virtual void Draw(Graphics g)
        {
            float width, height;
            width = HasFood ? Utils.NODE_WIDTH + FoodQuantity * 3 : Utils.NODE_WIDTH;
            height = HasFood ? Utils.NODE_HEIGHT + FoodQuantity * 3 : Utils.NODE_HEIGHT;

            g.FillEllipse(GetBrush(), Position.X - Utils.NODE_WIDTH / 2, Position.Y - Utils.NODE_HEIGHT / 2,
                width, height);
        }

        protected virtual Brush GetBrush()
        {
            return HasFood ? Utils.FOOD_NODE_BRUSH : Utils.EMPTY_NODE_BRUSH;
        }

        public new bool Equals(object other)
        {
            var node = (Node)other;
            return (Position.X == node.Position.X) && (Position.Y == node.Position.Y);
        }

        public new string ToString()
        {
            return $"{this.Position.X} {this.Position.Y}";
        }
    }
}