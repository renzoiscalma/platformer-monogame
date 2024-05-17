using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monogame_Assignment2.Models
{
    internal class Animation
    {   
        /// <summary>
        /// Current frame of the animation
        /// </summary>
        public int CurrentFrame { get; set; }
        /// <summary>
        /// Number of frames for this animation
        /// </summary>
        public int FrameCount { get; set; }
        /// <summary>
        /// Height of the animation
        /// </summary>
        public int FrameHeight { get { return Texture.Height; } }
        /// <summary>
        /// Speed of the animation
        /// </summary>
        public float FrameSpeed { get; set; }
        /// <summary>
        /// width of the animation
        /// </summary>
        public int FrameWidth { get { return Texture.Width / FrameCount; } }
        /// <summary>
        /// is the animation looping
        /// </summary>
        public bool IsLooping { get; set; }
        /// <summary>
        /// Image texture of the animation
        /// </summary>
        public Texture2D Texture { get; private set; }
        /// <summary>
        /// Constructor for the animation
        /// </summary>
        /// <param name="texture">The spritesheet texture of the animation</param>
        /// <param name="frameCount">The number of frames of the spritesheet</param>
        public Animation(Texture2D texture, int frameCount)
        {
            // assign texture
            Texture = texture;
            // assign frameCount
            FrameCount = frameCount;
            // make the animation loop
            IsLooping = true;
            // speed of the animation
            FrameSpeed = 0.1f;
        }
    }
}
