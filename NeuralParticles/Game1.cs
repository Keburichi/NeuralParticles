using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NeuralParticles.Entities;
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

        // ------------------------------------------------------------

        int numberOfParticles = 100;
        int numberOfParticleMoves = 1000;
        List<Particle> Particles = new List<Particle>();

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

            // Ziel initialisieren
            goalPosition = new Vector2(graphics.PreferredBackBufferWidth / 2, 100);
            goal = new Goal(goalPosition, goalSize);

            ParticleStartingPosition = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight - 100);

            // create particles
            for (int i = 0; i < numberOfParticles; i++)
            {
                Particles.Add(new Particle(ParticleStartingPosition, numberOfParticleMoves));
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

            // TODO: Add your update logic here
            foreach (var particle in Particles.Where(x => x.Alive))
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
            foreach (var particle in Particles)
            {
                particle.CalculateFitness(goal.GetCenter());
            }

            var bestParticle = Particles.OrderByDescending(x => x.Fitness).First();
            bestParticle.Best = true;
            Console.WriteLine($"Best Particle had a fitness of: {bestParticle.Fitness}");
        }

        private bool CollidesWithWall(Rectangle bounds)
        {
            //if()
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
