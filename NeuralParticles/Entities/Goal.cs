using Microsoft.Xna.Framework;

namespace NeuralParticles.Entities
{
    public class Goal
    {
        private Vector2 Position;
        private Rectangle Size;
        private Rectangle Bounds;

        private Color Color = Color.LawnGreen;

        public Goal(int positionX, int positionY, Rectangle size)
        {
            this.Position.X = positionX;
            this.Position.Y = positionY;
            this.Size = size;
        }

        public Goal(Vector2 position, Rectangle size)
        {
            this.Position = position;
            this.Size = size;
        }

        public bool IsColliding(Particle particle)
        {
            // Prüfen ob particle trifft.
            if (this.Bounds.Intersects(particle.GetBounds()))
                return true;
            else return false;
        }
    }
}
