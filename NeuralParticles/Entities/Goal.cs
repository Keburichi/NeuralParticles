using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace NeuralParticles.Entities
{
    public class Goal
    {
        private Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                Bounds = new Rectangle(Convert.ToInt32(_position.X), Convert.ToInt32(_position.Y), Convert.ToInt32(Size.X), Convert.ToInt32(Size.Y));
            }
        }
        private Vector2 _position;

        private Vector2 Size;
        private Rectangle Bounds;

        private Texture2D Texture;

        private Color Color = Color.LawnGreen;

        public Goal(int positionX, int positionY, Vector2 size)
        {            
            this.Size = size;
            this.Position = new Vector2(positionX, positionY);
        }

        public Goal(Vector2 position, Vector2 size)
        {
            this.Size = size;
            this.Position = position;            
        }

        public Rectangle GetBounds()
        {
            return Bounds;
        }

        public Vector2 GetCenter()
        {
            return new Vector2(Bounds.X + Bounds.Width / 2, Bounds.Y + Bounds.Height / 2);
        }

        public bool IsColliding(Particle particle)
        {
            // Prüfen ob particle trifft.
            if (this.Bounds.Intersects(particle.GetBounds()))
                return true;
            else return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Texture == null)
            {
                Texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                Texture.SetData(new Color[] { Color });
            }

            spriteBatch.Draw(Texture, Bounds, Color);
        }
    }
}
