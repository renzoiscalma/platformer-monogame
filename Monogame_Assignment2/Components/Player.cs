using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monogame_Assignment2.Models;
using Monogame_Assignment2.Sprites;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monogame_Assignment2.Components
{
    internal class Player : Sprite
    {
       /// <summary>
       /// possible state for the player
       /// </summary>
        public enum PlayerState : byte
        {
            IDLE = 0,
            FALL = 1,
            JUMP = 2,
            RUN = 3,
        }
        /// <summary>
        /// Current velocity of the player
        /// </summary>
        public Vector2 Velocity;
        /// <summary>
        /// Size of the player, did not make it 32x32 as it's too huge for collision
        /// </summary>
        private Point PLAYER_SIZE = new(29, 30);
        /// <summary>
        /// speed when the player jumps
        /// </summary>
        private readonly float JUMP_VELOCITY = 430f;
        /// <summary>
        /// speed when the player is running
        /// </summary>
        private readonly float RUN_VELOCITY = 200f;
        /// <summary>
        /// speed of fall of the player
        /// </summary>
        private readonly float FALL_VELOCITY = 10f;
        /// <summary>
        /// boolean to tell if the player jumped
        /// </summary>
        private bool jumped = false;
        /// <summary>
        /// boolean to tell if the player is at the floor
        /// </summary>
        private bool onFloor = true;
        /// <summary>
        /// current player state
        /// </summary>
        private PlayerState state = 0;

        /// <summary>
        /// Possible input keys that the player reacts with
        /// Space for jump, A and D for running left and right
        /// </summary>
        private static readonly Input Input = new()
        {
            Jump = Keys.Space,
            Left = Keys.A,
            Right = Keys.D,
        };
        /// <summary>
        /// Constructor that initializes the animation and the position of the player instance
        /// </summary>
        /// <param name="animation">The possible animations for the player</param>
        /// <param name="position">The initial position of the player</param>
        public Player(Dictionary<string, Animation> animation, Vector2 position)
            : base(animation) 
        {
            // set the rectangle of the player
            Rectangle = new(position.ToPoint(), PLAYER_SIZE);
        }

        /// <summary>
        /// Handles the input events when a certain key is pressed
        /// </summary>
        protected virtual void Handle_Input_Events()
        {
            /// <summary>
            /// Possible input keys that the player reacts with
            /// Space for jump, A and D for running left and right
            /// </summary>
            if (Keyboard.GetState().IsKeyDown(Input.Right))
            {
                Velocity.X = RUN_VELOCITY;
            }
            if (Keyboard.GetState().IsKeyDown(Input.Left))
            {
                Velocity.X = -RUN_VELOCITY;
            }
            if (!Keyboard.GetState().IsKeyDown(Input.Left) 
                && !Keyboard.GetState().IsKeyDown(Input.Right))
            {
                Velocity.X = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Input.Jump))
            {
                Jump();
            } else
            {
                jumped = false;
            }
        }

        /// <summary>
        /// Set the current animation for the player
        /// </summary>
        protected override void SetAnimations()
        {
            _animationManager.Play(_animations[state.ToString()]);
        }
        /// <summary>
        /// Updates the state of the player object when claled
        /// </summary>
        /// <param name="gameTime">Elapsed time of the game</param>
        /// <param name="worldState">State of the game</param>
        public void Update(GameTime gameTime, World worldState)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // check if player is below the bottom of the screen
            // game over!
            if (Rectangle.Top > 768)
            {
                worldState.TriggerGameOver();
            }
            // move to next position when player reaches position 1280
            // (max screen width of the game, in x axis)
            if (Velocity.X > 0 && Rectangle.Right > 1280)
            {

                if (worldState.NextView())
                {
                    MoveTo(-15, Rectangle.Y);
                }
            }
            // move to previous view when player reaches position 0 (in x axis)
            if (Velocity.X < 0 && Rectangle.Left < 0)
            {
                if (worldState.PrevView())
                {
                    MoveTo(1270, Rectangle.Y);
                }
            }
            // call handle input events
            Handle_Input_Events();
            // get the displacement of the player based on the elapsed time
            float dx = Velocity.X * dt;
            Velocity.Y += FALL_VELOCITY;
            float dy = (Velocity.Y) * dt;
            // handle x-axis displacements
            OffsetX(dx);
            HandleHorizontalCollisions(worldState);
            // handle y-axis displacements
            OffsetY(dy);
            HandleVerticalCollisions(worldState);
            // update the state of the player
            UpdateState();
            // set animation of the player
            SetAnimations();
            // update the animation of the player
            _animationManager.Update(gameTime);
        }
        /// <summary>
        /// Draws the player into the screen
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            // if going right, render noramlly
            if (Velocity.X >= 0)
            {
                _animationManager.Draw(spriteBatch, Rectangle.Location.ToVector2());
            }
            else // else, if the player is going left, flip the rendered image
            {
                _animationManager.DrawReverse(spriteBatch, Rectangle.Location.ToVector2());
            }
        }
        /// <summary>
        /// Handles the horizontal collision of the player
        /// </summary>
        /// <param name="worldState"></param>
        private void HandleHorizontalCollisions(World worldState)
        {   
            List<Platform> currPlatforms = worldState.GetCurrentPlatforms();
            List<Goal> currGoals = worldState.GetCurrentGoals();
            foreach(Platform platform in currPlatforms)
            {
                // if colliding with a platform
                if (Rectangle.Intersects(platform.Rectangle))
                {
                    float displacementX;
                    if (Velocity.X < 0) // if going left
                    {
                        // set the player beside the platform. do not let it go through ti
                        displacementX = platform.Rectangle.Right - Rectangle.Left;
                        OffsetX(displacementX);
                    } else if (Velocity.X > 0) // if going right
                    {   
                        // set the player beside the platform. do not let it go through a platform
                        displacementX = platform.Rectangle.Left - Rectangle.Right; // get the difference of the collision box
                        OffsetX(displacementX); // offset the player by the collision 
                    }
                }
            }
            // handle collisions with goals
            foreach(Goal goal in currGoals)
            {
                if (Rectangle.Intersects(goal.Rectangle))
                {
                    // increment collectedGoal counter
                    if (goal.Collect())
                    {
                        worldState.AddCollected();
                    }
                }
            }
        }
        /// <summary>
        /// Handles the vertical collisions of the player
        /// </summary>
        /// <param name="worldState">The current world state of the player</param>
        private void HandleVerticalCollisions(World worldState)
        {
            // handle collisions with platforms
            List<Platform> currPlatforms = worldState.GetCurrentPlatforms();
            List<Goal> currGoals = worldState.GetCurrentGoals();
            foreach (Platform platform in currPlatforms)
            {
                if (Rectangle.Intersects(platform.Rectangle))
                {
                    float displacementY;
                    if (Velocity.Y < 0) // if the player is going up
                    {
                        // place the player at below the platform
                        displacementY = platform.Rectangle.Bottom - Rectangle.Top;
                        OffsetY(displacementY);
                        Velocity.Y = 0;
                    }
                    else if (Velocity.Y > 0) // if going down
                    {
                        // place the player on top of the platform
                        displacementY = platform.Rectangle.Top - Rectangle.Bottom;
                        OffsetY(displacementY);
                        Velocity.Y = 0;
                        onFloor = true; // toggle onFloor since it's just standing
                    }
                }
            }
            // handle collisions with goals
            foreach (Goal goal in currGoals)
            {
                if (Rectangle.Intersects(goal.Rectangle))
                {
                    // increment collectedGoal counter
                    if (goal.Collect())
                    {
                        worldState.AddCollected();
                    }
                }
            }
        }

        /// <summary>
        /// Updates the state of the player
        /// </summary>
        private void UpdateState()
        {
            // if the player is on floor
            if (onFloor)
            {
                // and is just standing, state is idle
                if (Velocity.X == 0)
                {
                    state = PlayerState.IDLE;
                }
                else
                {
                    // else state is running
                    state = PlayerState.RUN;
                }
            } else
            {
                // if falling (speed is > 0) 
                if (Velocity.Y > 0)
                {
                    state = PlayerState.FALL;
                }
                else 
                {
                    state = PlayerState.JUMP;
                }
            }
        }

        /// <summary>
        /// Adjusts the Y velocity of the player to make it jump!
        /// </summary>
        private void Jump()
        {
            if (onFloor && !jumped)
            {
                Velocity.Y = -JUMP_VELOCITY;
                onFloor = false;
            }
        }

        // restarts the position of the player
        public void Restart()
        {
            Rectangle = new(new Point(64 * 3, 64 * 10), PLAYER_SIZE);
        }
    }


}
