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

        private Vector2 Acceleration;
        private Vector2 Velocity;
        private Rectangle Bounds;

        public double Fitness;

        private Color Color = Color.Black;
        private Color ColorDeath = Color.Red;
        private Color ColorReached = Color.LightGoldenrodYellow;

        private Texture2D Texture;

        private ParticleBrain Brain;

        public bool Alive
        {
            get { return _alive; }
            set
            {
                _alive = value;
                UpdateTexture();
            }
        }
        private bool _alive = true;

        public bool ReachedGoal = false;
        public bool Best = false;

        public Particle()
        {

        }

        public Particle(Vector2 position, int numberOfMoves)
        {
            this.Position = position;
            Brain = new ParticleBrain(numberOfMoves);
        }

        public Rectangle GetBounds()
        {
            return Bounds;
        }

        private Color GetColor()
        {
            return Best ? ColorReached : Alive ? Color : ColorDeath;
        }

        private void UpdateTexture()
        {
            if (Texture != null)
                Texture.SetData(new Color[] { GetColor() });
        }

        public void CalculateFitness(Vector2 goalPosition)
        {
            // Wenn das Ziel erreicht wurde, dann gebe einen Fitnessscore anhand der verwendeten schritte
            if (ReachedGoal)
                Fitness = 1.0 / 16.0 + 10000.0 / (float)(Brain.Step * Brain.Step);
            else
            {
                // Wenn das Ziel nicht erreicht wurde, dann anhand von Distanz errechnen
                var distanceToGoal = Vector2.Distance(goalPosition, Position);
                Fitness = 1.0 / (distanceToGoal * distanceToGoal);
            }

            // Wenn vorzeitig in eine Wand geraten, dann keine Fitness, da schlecht?
            if (!Alive && Brain.Step < Brain.Directions.Length)
                Fitness = 0;
        }

        public Particle Clone()
        {
            var clone = new Particle();
            clone.Brain = Brain.Clone();

            return clone;
        }

        public void Update()
        {
            // Wenn noch bewegungen übrig, dann als beschleunigung setzen
            if (Brain.Directions.Length > Brain.Step)
            {
                Acceleration = Brain.Directions[Brain.Step];
                Brain.Step++;
            }
            else
            {
                Alive = false;
            }

            Velocity.X += Acceleration.X;
            Velocity.Y += Acceleration.Y;

            // Prüfen ob zu schnell bewegen
            if (Velocity.X > 5)
                Velocity.X = 5;

            if (Velocity.Y > 5)
                Velocity.Y = 5;

            Position = Vector2.Add(Position, Acceleration);            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Texture == null)
            {
                Texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                Texture.SetData(new Color[] { GetColor() });
            }

            // Textur updaten
            UpdateTexture();

            // Prüfen in welcher Farbe gemalt werden soll
            spriteBatch.Draw(Texture, Bounds, GetColor());
        }

        public delegate void ParticleMoved();
        public event ParticleMoved ParticleMovedEvent;
    }
}
