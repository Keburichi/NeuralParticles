using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NeuralParticles.Entities;
using NeuralParticles.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuralParticles
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class NeuralParticles : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // ------------------------------------------------------------

        int generation = 0;
        int FitnessSum = 0;

        // ------------------------------------------------------------

        int numberOfParticles = 100;
        int numberOfParticleMoves = 1000;
        Particle[] Particles;

        Vector2 ParticleStartingPosition;

        // ------------------------------------------------------------

        Goal goal;
        Vector2 goalPosition;
        Vector2 goalSize = new Vector2(10, 10);

        // ------------------------------------------------------------

        public NeuralParticles()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferHeight = 900;
            graphics.PreferredBackBufferWidth = 900;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Particles = new Particle[numberOfParticles];

            // Ziel initialisieren
            goalPosition = new Vector2(graphics.PreferredBackBufferWidth / 2, 100);
            goal = new Goal(goalPosition, goalSize);

            ParticleStartingPosition = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight - 100);

            // create particles
            for (int i = 0; i < numberOfParticles; i++)
            {
                Particles[i] = new Particle(ParticleStartingPosition, numberOfParticleMoves);
            }

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var highestFitness = 0;
            var bestParticleIndex = 0;

            //for (int i = 0; i < Particles.Where(x => x.Alive).Count(); i++)
            //{
            //    Particles[i].Update();

            //    // Prüfen ob Wand berührt wird
            //    if (CollidesWithWall(Particles[i].GetBounds()))
            //        Particles[i].Alive = false;

            //    // Prüfen ob particle am ziel ist
            //    if (goal.IsColliding(Particles[i]))
            //        Particles[i].ReachedGoal = true;

            //    Particles[i].CalculateFitness(goal.GetCenter());

            //    if (Particles[i].Fitness > highestFitness)
            //    {
            //        Console.WriteLine($"New best particle with fitness: {Particles[i].Fitness} and index: {i}");
            //        bestParticleIndex = i;
            //    }
            //}

            // TODO: Add your update logic here
            foreach (var particle in Particles.Where(x => x.Alive && !x.ReachedGoal))
            {
                particle.Update();

                // Prüfen ob Wand berührt wird
                if (CollidesWithWall(particle.GetBounds()))
                    particle.Alive = false;

                // Prüfen ob particle am ziel ist
                if (goal.IsColliding(particle))
                    particle.ReachedGoal = true;
            }

            // Prüfen ob alle Particle tot sind oder Ziel erreicht haben
            if (Particles.All(x => !x.Alive || x.ReachedGoal))
                NextGen();

            base.Update(gameTime);
        }

        private void NextGen()
        {
            FitnessSum = 0;

            foreach (var particle in Particles)
            {
                particle.Best = false;
                particle.CalculateFitness(goal.GetCenter());
                FitnessSum += particle.Fitness;
            }

            var bestParticle = Particles.OrderByDescending(x => x.Fitness).First();
            bestParticle.Best = true;
            Console.WriteLine($"Best Particle had a fitness of: {bestParticle.Fitness}");

            var soManyBestParticles = Particles.Where(x => x.Best).Count();
            Console.WriteLine($"So many best Particles exist: {soManyBestParticles}");

            // Prüfen ob Ziel erreicht wurde, wenn ja, dann maximale anzahl an schritten verringern, damit nächsten besser sein müssen
            // wenn ziel nicht erreicht, dann mehr schritte geben
            if (bestParticle.ReachedGoal)
                numberOfParticleMoves = bestParticle.GetUsedSteps();
            else
                numberOfParticleMoves += (int)(numberOfParticleMoves * 0.1);

            NaturalSelection();
            MutateNewParticles();

            generation++;
            Console.WriteLine($"Starting new run with generation: {generation} and {numberOfParticleMoves} number of steps.");
        }

        private void MutateNewParticles()
        {
            for (int i = 1; i < numberOfParticles; i++)
            {
                Particles[i].MutateBrain();
            }
        }

        private void NaturalSelection()
        {
            Particle[] newParticles = new Particle[numberOfParticles]; // Next gen!!!!!
            newParticles[0] = Particles.FirstOrDefault(x => x.Best).Clone();
            newParticles[0].Best = true;

            for (int i = 1; i < newParticles.Length; i++)
            {
                // Parent anhand fitness finden
                var parent = SelectParent();

                newParticles[i] = parent.Clone();
            }

            newParticles.CopyTo(Particles, 0);
        }

        private Particle SelectParent()
        {
            var rand = RNG.rng.Next(FitnessSum);

            long runningSum = 0;

            for (int i = 0; i < Particles.Length; i++)
            {
                runningSum += Particles[i].Fitness;
                if(runningSum > rand)
                {
                    return Particles[i];
                }
            }

            // Sollte niemals erreicht werden
            return null;
        }

        private bool CollidesWithWall(Rectangle bounds)
        {
            if (bounds.Left < 0 || bounds.Right > graphics.PreferredBackBufferWidth || bounds.Top < 0 || bounds.Bottom > graphics.PreferredBackBufferHeight)
                return true;
            else
                return false;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            // Ziel zeichnen
            goal.Draw(spriteBatch);

            //spriteBatch.DrawString(SpriteFont.Glyph(), $"Generation: {generation}", new Vector2(20, 20), Color.Black);

            // TODO: Add your drawing code here
            foreach (var particle in Particles)
            {
                particle.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
