using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace NeuralParticles.Entities
{
    public class Particle
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

        private Vector2 Size = new Vector2(10, 10);
        private Vector2 Velocity;
        private Rectangle Bounds;
        private Color Color = Color.Black;

        private Texture2D Texture;

        private ParticleBrain Brain;

        private bool Alive = true;
        bool ReachedGoal = false;
        bool Best = false;

        public Particle(Vector2 position, int numberOfMoves)
        {
            this.Position = position;
            Brain = new ParticleBrain(numberOfMoves);
        }

        public Rectangle GetBounds()
        {
            return Bounds;
        }

        public void Update()
        {
            Position = new Vector2(_position.X + new Random().Next(-10, 10), _position.Y + new Random().Next(-10, 10));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Texture == null)
            {
                Texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                Texture.SetData(new Color[] { Color.Black });
            }

            spriteBatch.Draw(Texture, Bounds, Color);
        }

        public delegate void ParticleMoved();
        public event ParticleMoved ParticleMovedEvent;
    }
}
