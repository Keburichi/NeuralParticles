using Microsoft.Xna.Framework;
using System;

namespace NeuralParticles.Entities
{
    public class ParticleBrain
    {
        int Step;
        Vector2[] Directions;
        Random rng = new Random();

        public ParticleBrain(int numberOfDirections)
        {
            Directions = new Vector2[numberOfDirections];
        }

        private void Randomize()
        {
            for (int i = 0; i < Directions.Length; i++)
            {
                var randomAngle = rng.Next(0, 360);
                Directions[i] = new Vector2(randomAngle);
            }
        }

        public ParticleBrain Clone()
        {
            ParticleBrain clone = new ParticleBrain(Directions.Length);

            for (int i = 0; i < Directions.Length; i++)
            {
                clone.Directions[i] = Directions[i];
            }

            return clone;
        }

        public void Mutate()
        {
            var mutationChance = 10;
            var rando = new Random();

            for (int i = 0; i < Directions.Length; i++)
            {
                if (rando.Next(0, 100) < mutationChance)
                {
                    var randomAngle = rng.Next(0, 360);
                    Directions[i] = new Vector2(randomAngle);
                }
            }
        }
    }
}
