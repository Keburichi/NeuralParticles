using Microsoft.Xna.Framework;
using NeuralParticles.Helper;
using System;

namespace NeuralParticles.Entities
{
    public class ParticleBrain
    {
        public int Step;
        public Vector2[] Directions;
        //Random rng = new Random();

        public ParticleBrain(int numberOfDirections)
        {
            Directions = new Vector2[numberOfDirections];
            Randomize();
        }

        private void Randomize()
        {
            for (int i = 0; i < Directions.Length; i++)
            {
                Directions[i] = new Vector2
                {
                    X = (float)Math.Cos(RNG.rng.Next(360)),
                    Y = (float)Math.Sin(RNG.rng.Next(360))
                };
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
                    Directions[i] = new Vector2
                    {
                        X = (float)Math.Cos(RNG.rng.Next(360)),
                        Y = (float)Math.Sin(RNG.rng.Next(360))
                    };
                }
            }
        }
    }
}
