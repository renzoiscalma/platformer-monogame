using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Monogame_Assignment2.Sprites
{
    /// <summary>
    /// class of the platforms for the game. This builds a 64x64 block. 
    /// </summary>
    internal class Platform : Sprite
    {
        // enums used for building platforms from a single spritesheet
        public enum BuildingBlock : byte
        {
            GRASS_LEFT,
            GRASS_CENTER,
            GRASS_RIGHT,
            GROUND_LEFT,
            GROUND_CENTER,
            GROUND_RIGHT,
        }
        public enum PlatformType : byte
        {
            GROUND,
            BLOCK,
            GROUND_GRASS_LEFT,
            GROUN_GRASS_RIGHT,
            GROUND_GRASS_CENTER,
            GROUND_LEFT,
            GROUND_CENTER,
            GROUND_RIGHT,
            GROUND_GRASS_PATCH_LEFT,
            GROUND_GRASS_PATCH_RIGHT,
        }
        /// <summary>
        /// building blocks for each surface of the platforms, each block is a 16x16 image that is a part of the image,
        /// since we're using a spritesheet we have to only get a part of the image that is relevant to the block
        /// </summary>
        private static readonly Rectangle[] buildingRectangles =
        {
            new Rectangle(BUILDING_BLOCK_SIZE * 6, 0, BUILDING_BLOCK_SIZE, BUILDING_BLOCK_SIZE),
            new Rectangle(BUILDING_BLOCK_SIZE * 7, 0, BUILDING_BLOCK_SIZE, BUILDING_BLOCK_SIZE),
            new Rectangle(BUILDING_BLOCK_SIZE * 8, 0, BUILDING_BLOCK_SIZE, BUILDING_BLOCK_SIZE),
            new Rectangle(BUILDING_BLOCK_SIZE * 6, 1 * BUILDING_BLOCK_SIZE, BUILDING_BLOCK_SIZE, BUILDING_BLOCK_SIZE),
            new Rectangle(BUILDING_BLOCK_SIZE * 7, 1 * BUILDING_BLOCK_SIZE, BUILDING_BLOCK_SIZE, BUILDING_BLOCK_SIZE),
            new Rectangle(BUILDING_BLOCK_SIZE * 8, 1 * BUILDING_BLOCK_SIZE, BUILDING_BLOCK_SIZE, BUILDING_BLOCK_SIZE),
        };
        /// <summary>
        /// Building block for each platform types
        /// each platform property will have 4 building blocks
        /// if we place the 4 building blocks as grid we come up with a 32x32 block, it is then scaled to become 64x64 
        /// </summary>
        private static readonly BuildingBlock[,] platformProperties = {
            { BuildingBlock.GRASS_LEFT, BuildingBlock.GRASS_RIGHT,              // building block for ground
                BuildingBlock.GROUND_LEFT, BuildingBlock.GROUND_RIGHT },        
            { BuildingBlock.GRASS_LEFT, BuildingBlock.GRASS_LEFT,               // building block for block
                BuildingBlock.GRASS_LEFT, BuildingBlock.GRASS_LEFT },
            { BuildingBlock.GRASS_LEFT, BuildingBlock.GRASS_CENTER,             // GRASS_LEFT
                BuildingBlock.GROUND_LEFT, BuildingBlock.GROUND_CENTER },
            { BuildingBlock.GRASS_CENTER, BuildingBlock.GRASS_RIGHT,            // GRASS_RIGHT
                BuildingBlock.GROUND_CENTER, BuildingBlock.GROUND_RIGHT },
            { BuildingBlock.GRASS_CENTER, BuildingBlock.GRASS_CENTER,           // GRASS_CENTER
                BuildingBlock.GROUND_CENTER, BuildingBlock.GROUND_CENTER},
            { BuildingBlock.GROUND_LEFT, BuildingBlock.GROUND_CENTER,           // GROUND_LEFT
                BuildingBlock.GROUND_LEFT, BuildingBlock.GROUND_CENTER },       
            { BuildingBlock.GROUND_CENTER, BuildingBlock.GROUND_CENTER,         // GROUND_CENTER
                BuildingBlock.GROUND_CENTER, BuildingBlock.GROUND_CENTER },     
            { BuildingBlock.GROUND_CENTER, BuildingBlock.GROUND_RIGHT,          // GROUND_RIGHT
                BuildingBlock.GROUND_CENTER, BuildingBlock.GROUND_RIGHT },      
            { BuildingBlock.GROUND_CENTER, BuildingBlock.GROUND_RIGHT,          // GROUND_GRASS_PATCH_LEFT
                BuildingBlock.GROUND_CENTER, BuildingBlock.GROUND_RIGHT },
            { BuildingBlock.GROUND_CENTER, BuildingBlock.GROUND_RIGHT,          // GROUND_GRASS_PATCH_RIGHT
                BuildingBlock.GROUND_CENTER, BuildingBlock.GROUND_RIGHT },
        };

        /// <summary>
        /// Type of this platform
        /// </summary>
        private PlatformType Type { get; set; }
        /// <summary>
        /// Size of a building block
        /// </summary>
        private const short BUILDING_BLOCK_SIZE = 16;
        private Point PLATFORM_SIZE = new(60, 60);
        public Platform(Texture2D texture, Vector2 position, PlatformType platformType) 
            : base(texture) // constructor for the parent class
        {
            Rectangle = new Rectangle(position.ToPoint(), PLATFORM_SIZE);
            Type = platformType;
        }
        // override SetAnimation of parent class to empty since blocks have no animations
        protected override void SetAnimations() { }
        /// <summary>
        /// Draws the platform into the screen, each platform has 4 building blocks that are of size 32x32 
        /// and when placed into a grid makes up a 64x64 block
        /// </summary>
        /// <param name="spriteBatch">The spritebatch of the game</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            // declare variables for the building blocks
            short i, j;
            BuildingBlock bb;
            Rectangle bRect;    // rectangle of the block
            Vector2 pos;    // position of a building block
            Vector2 scale = new(2, 2);  // 2x scale for the building block
            for (i = 0; i < 2; i++)
            {
                for (j = 0; j < 2; j++)
                {
                    // since we have a building block of 32x32 we need to offset each block by 32 pixels
                    pos = new(Rectangle.X + (j * 32), Rectangle.Y + (i * 32));
                    // the the property of the platform
                    bb = platformProperties[(int) Type, (i * 2 + j)];
                    // get hte building rectangle of the platform
                    bRect = buildingRectangles[(int) bb];
                    // draw the building block into the screen
                    spriteBatch.Draw(
                            _texture,
                            pos,
                            bRect,
                            Color.White,
                            0,
                            Vector2.Zero,
                            scale,
                            0, 0
                        );

                }
            }

        }
    }
}
