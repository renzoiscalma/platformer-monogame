using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monogame_Assignment2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monogame_Assignment2.Managers
{
    /*
     * Keeps track of what animation to play
     */
    internal class AnimationManager
    {
        /// <summary>
        /// The animation to manage
        /// </summary>
        private Animation _animation;
        /// <summary>
        /// Timer for the animations
        /// </summary>
        private float _timer;
        /// <summary>
        /// Creates and object Animation Manager to manage the animations
        /// </summary>
        /// <param name="animation">The animation to manage</param>
        public AnimationManager(Animation animation)
        {
            _animation = animation;
        }
        /// <summary>
        /// Plays an animation
        /// </summary>
        /// <param name="animation">Animation the play</param>
        public void Play(Animation animation)
        {
            // if same animation, no need to to play it 
            if (_animation == animation)
                return;
            // if different animation set it as the animation to manage
            _animation = animation;
            // reset frames
            _animation.CurrentFrame = 0;
            // reset the timer
            _timer = 0;
        }
        /// <summary>
        /// stop the animation
        /// </summary>
        public void Stop()
        {
            _timer = 0f;
            _animation.CurrentFrame = 0;
        }
        /// <summary>
        /// Updates the animation
        /// </summary>
        /// <param name="gameTime">The elapsed time of the game</param>
        public void Update(GameTime gameTime)
        {
            // increase timer by the elapsed time
            _timer += (float) gameTime.ElapsedGameTime.TotalSeconds;
            // if animation timer reaches the threshold of the frame
            if (_timer > _animation.FrameSpeed)
            {
                // set the timer back to 0
                _timer = 0f;
                // set the animation to next frame
                _animation.CurrentFrame++;
                // if at the end of animation, go back to the first frame
                if (_animation.CurrentFrame >= _animation.FrameCount)
                    _animation.CurrentFrame = 0;
            }
        }
        /// <summary>
        /// Draws the animation
        /// </summary>
        /// <param name="spriteBatch">The spritebatch of the game</param>
        /// <param name="position">The position of the animation in the screen</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            // scale the drawing based on the rectangle
            spriteBatch.Draw(_animation.Texture,
                position,
                // gets the frame of the spritesheet based on its width and height
                new Rectangle(_animation.CurrentFrame * _animation.FrameWidth,
                              0,
                              _animation.FrameWidth,
                              _animation.FrameHeight),
                Color.White);
        }

        /// <summary>
        /// Draws the animation, and scales it
        /// </summary>
        /// <param name="spriteBatch">The spritebatch of the game</param>
        /// <param name="position">The position of the animation in the screen</param>
        /// <param name="scale">The amount of scale to be done to the animation</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Vector2 scale)
        {
            spriteBatch.Draw(_animation.Texture,
                position,
                // gets the frame of the spritesheet based on its width and height
                new Rectangle(_animation.CurrentFrame * _animation.FrameWidth,
                              0,
                              _animation.FrameWidth,
                              _animation.FrameHeight),
                Color.White,
                0,
                Vector2.Zero,
                scale,
                0,
                0);
        }

        /// <summary>
        /// Draws the animation, but the texture is flipped horizontally
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="position"></param>
        public void DrawReverse(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(_animation.Texture,
                position,
                // gets the frame of the spritesheet based on its width and height
                new Rectangle(_animation.CurrentFrame * _animation.FrameWidth,
                              0,
                              _animation.FrameWidth,
                              _animation.FrameHeight),
                Color.White,
                0,
                Vector2.Zero,
                1f,
                SpriteEffects.FlipHorizontally,
                0);
        }
        /// <summary>
        /// Gets the current frame of the animation
        /// </summary>
        /// <returns></returns>
        public int GetCurrentFrame()
        {
            return _animation.CurrentFrame;
        }
        /// <summary>
        /// Returns the maximum frame of the animation
        /// </summary>
        /// <returns></returns>
        public int GetMaxFrame()
        {
            return _animation.FrameCount;
        }
        /// <summary>
        /// Resets the frame of the animation
        /// </summary>
        public void ResetFrame()
        {
            _animation.CurrentFrame = 0;
        }
    }


}
