using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monogame_Assignment2.Sprites;
using Monogame_Assignment2.Models;
using System.Collections.Generic;
using System;
using Monogame_Assignment2.Components;

namespace Monogame_Assignment2
{
    public class Game1 : Game
    {
        /// <summary>
        /// Monogame graphics device manager
        /// </summary>
        private GraphicsDeviceManager _graphics;
        /// <summary>
        /// MOnogame spritebatch 
        /// </summary>
        private SpriteBatch _spriteBatch;
        /// <summary>
        /// define player variable
        /// </summary>
        private Player _player;
        /// <summary>
        /// Define world variable
        /// </summary>
        private World _world;
        /// <summary>
        /// Define font to render
        /// </summary>
        private SpriteFont _font;
        // constructor of the game, autofilled by monogame template
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// autofilled by monogame template
        /// </summary>
        protected override void Initialize()
        {
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 768;
            _graphics.ApplyChanges();
            base.Initialize();
        }

        /// <summary>
        /// loads the assets, and aniamtions for the game
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // load the animation for the player
            var animations = new Dictionary<string, Animation>()
            {
                { Player.PlayerState.IDLE.ToString(), new Animation(Content.Load<Texture2D>("Player/Idle (32x32)"), 11) },
                { Player.PlayerState.RUN.ToString(), new Animation(Content.Load<Texture2D>("Player/Run (32x32)"), 12) },
                { Player.PlayerState.FALL.ToString(), new Animation(Content.Load<Texture2D>("Player/Fall (32x32)"), 1) },
                { Player.PlayerState.JUMP.ToString(), new Animation(Content.Load<Texture2D>("Player/Jump (32x32)"), 1) },

            };
            // load the platform spritesheet
            var platformTextures = Content.Load<Texture2D>("Platform/Terrain (16x16)");
            // load the animations for the goals
            var goalAnimations = new Dictionary<string, Animation>()
            {
                { Goal.GoalType.CHERRIES.ToString(), new Animation(Content.Load<Texture2D>("Goals/Cherries"), 17) },
                { Goal.GoalType.KIWI.ToString(), new Animation(Content.Load<Texture2D>("Goals/Kiwi"), 17) },
                { Goal.GoalType.MELON.ToString(), new Animation(Content.Load<Texture2D>("Goals/Melon"), 17) },
                { "COLLECTED", new Animation(Content.Load<Texture2D>("Goals/Collected"), 6) },

            };
            // load the font for the game
            _font = Content.Load<SpriteFont>("game");
            // instantiate world
            _world = new World(platformTextures, goalAnimations, _font);
            // instantiate player
            _player = new Player(animations, new Vector2(64 * 3, 64 * 10));
        }
        /// <summary>
        /// Updates the game based on the time passed
        /// </summary>
        /// <param name="gameTime">The elapsed tiem of the game</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.R)) {
                // restart world and player
                _world.Restart();
                _player.Restart();
            }
            // update the wolrd
            _world.Update(gameTime);
            // update the layer
            _player.Update(gameTime, _world);
            base.Update(gameTime);
        }

        /// <summary>
        /// Calls the draw function for each sprites
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // use point clamp for the spritebatch to have a pixelated effect for the scaled images
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _world.Draw(_spriteBatch);
            _player.Draw(_spriteBatch);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}