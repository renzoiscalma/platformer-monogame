using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monogame_Assignment2.Managers;
using Monogame_Assignment2.Models;
using Monogame_Assignment2.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Monogame_Assignment2.Components
{
    /// <summary>
    /// Class Goal is the items to collect per "view", inherits sprite to have its helper functions
    /// </summary>
    internal class Goal : Sprite
    {
        /// <summary>
        /// Goals of the view, view 0 will have kiwi, 
        /// view 1 will have a melon, and so on and so forth
        /// </summary>
        public enum GoalType : byte
        {
            KIWI = 0,
            MELON = 1,
            CHERRIES = 2,
        }

        /// <summary>
        /// THe type of this goal
        /// </summary>
        private GoalType Type { get; set; }
        /// <summary>
        /// Size of the goal
        /// </summary>
        private Vector2 GOAL_SIZE = new(32, 32);
        /// <summary>
        /// Is the goal collected
        /// </summary>
        private bool _collected = false;
        /// <summary>
        /// Is the goal done with collected animation
        /// </summary>
        public bool End { get; private set; } = false;
        /// <summary>
        /// Constructor for the gaoal
        /// </summary>
        /// <param name="animation">Possible animation for the gola</param>
        /// <param name="position">Position of the goal</param>
        /// <param name="goalType">Type of the goal</param>
        public Goal(Dictionary<string, Animation> animation, Vector2 position, GoalType goalType)
            : base(animation)
        {
            // assign rectangle based on position and size
            Rectangle = new(position.ToPoint(), GOAL_SIZE.ToPoint());
            // assign the type of goal
            Type = goalType;
        }
        /// <summary>
        /// Sets the animation of the goal
        /// </summary>
        protected override void SetAnimations()
        {
            // use collected animation of the goal when collected
            if (_collected)
                _animationManager.Play(_animations["COLLECTED"]);
            else
                // use the play the normal animation
                _animationManager.Play(_animations[Type.ToString()]);
        }
        /// <summary>
        /// Updates the goal
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            // if collected animation has ended
            if (_collected && _animationManager.GetCurrentFrame() >= _animationManager.GetMaxFrame() - 1)
            {
                End = true;
            }
            // update the animation manager
            _animationManager.Update(gameTime);
            // set the animations
            SetAnimations();
        }
        /// <summary>
        /// Draws the goal into the screen
        /// </summary>
        /// <param name="spriteBatch">The spritebatch of the world</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            // draws the goal based on the position and scales it by 2
            _animationManager.Draw(spriteBatch, Rectangle.Location.ToVector2(), new(2, 2));
        }

        /// <summary>
        /// Collects the goal if the state of the goal is not collectible
        /// </summary>
        /// <returns>
        /// Returns if the goal has been successfuly collected. 
        /// e.g. Returns true if _collected has just been switched to true,
        /// else it will be returned false.
        /// </returns>
        public bool Collect()
        {
            if (!_collected)
            {
                _collected = true;
                _animationManager.ResetFrame();
                return true;
            }
            return false;
        }

        // resets the goal
        public void Reset()
        {
            _collected = false;
            End = false;
        }

    }
}
