using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monogame_Assignment2.Managers;
using Monogame_Assignment2.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monogame_Assignment2.Sprites
{
    internal class Sprite
    {
        /// <summary>
        /// The animation manager for the sprite
        /// </summary>
        protected AnimationManager _animationManager;
        /// <summary>
        /// The animations for the sprite
        /// </summary>
        protected Dictionary<string, Animation> _animations;
        /// <summary>
        /// The image of the sprite
        /// </summary>
        protected Texture2D _texture;

        public Rectangle Rectangle { get; set; }

        /// <summary>
        /// Constructor for sprites with animation, e.g. Goals and the Player
        /// </summary>
        /// <param name="animations"></param>
        public Sprite(Dictionary<string, Animation> animations)
        {
            _animations = animations;
            // pass in the first frame as the first animation
            _animationManager = new AnimationManager(_animations.First().Value);
        }
        /// <summary>
        /// For sprites with no animations, e.g. blocks
        /// </summary>
        /// <param name="texture"></param>
        public Sprite(Texture2D texture)
        {
            _texture = texture;
        }
        /// <summary>
        /// Set Animation of the block
        /// </summary>
        protected virtual void SetAnimations()
        {

        }
        /// <summary>
        /// Moves the sprite along the X-axis
        /// </summary>
        /// <param name="dx">The amount of X-axis displacement of the spirite</param>
        public void OffsetX(float dx)
        {
            // define a new rectangle
            Rectangle newRect = Rectangle;
            // move the  new rectangle
            newRect.Offset(dx, 0);
            // assign the new rectangle to this sprite
            this.Rectangle = newRect;
        }
        /// <summary>
        /// Moves the sprite along the Y-axis
        /// </summary>
        /// <param name="dy"></param>
        public void OffsetY(float dy)
        {
            // define a new rectangle
            Rectangle newRect = Rectangle;
            // move the new rectangle
            newRect.Offset(0, dy);
            // assign the new rectangle 
            Rectangle = newRect;
        }
        /// <summary>
        /// Movest the sprite to a specific x and y coordinate
        /// </summary>
        /// <param name="x">x coordinate target</param>
        /// <param name="y">y coordinate target</param>
        public void MoveTo(float x, float y)
        {
            // define new rectangle
            Rectangle newRect = Rectangle;
            // modify lcoation of the rectangle
            newRect.Location = new Vector2(x, y).ToPoint();
            // assigne the new recatngle to this sprite
            Rectangle = newRect;
        }
        /// <summary>
        /// Updates the sprite
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            // update the animation
            _animationManager.Update(gameTime);
            // set the animation
            SetAnimations();
        }
        /// <summary>
        /// Draws the sprite into the screen
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (_texture != null)
            {
                spriteBatch.Draw(_texture, Rectangle.Location.ToVector2(), Color.White);
            }
            else if (_animationManager != null)
                _animationManager.Draw(spriteBatch, Rectangle.Location.ToVector2());
            else
                throw new Exception("Should not happen");
            }
        }
}
