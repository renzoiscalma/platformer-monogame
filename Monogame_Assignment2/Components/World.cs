using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monogame_Assignment2.Models;
using Monogame_Assignment2.Sprites;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Monogame_Assignment2.Components
{
    internal class World
    {
        /// <summary>
        /// List of platforms per screen view
        /// </summary>
        public List<List<Platform>> PlatformsPerView { get; private set; }
        /// <summary>
        /// list of goals per screen view
        /// </summary>
        public List<List<Goal>> GoalsPerView { get; private set; }
        /// <summary>
        ///  textures of the platform
        /// </summary>
        private readonly Texture2D _platformTextures;
        /// <summary>
        /// The animations of the goal
        /// </summary>
        private readonly Dictionary<string, Animation> _goalAnimations;
        /// <summary>
        /// The current screen view
        /// </summary>
        private int _view;
        /// <summary>
        /// The maximum amount of views in this world
        /// </summary>
        private int _maxView;
        /// <summary>
        /// The number of the goals collected in this world
        /// </summary>
        private int _collectedCount = 0;
        /// <summary>
        /// Is the game completed?
        /// </summary>
        private bool _completed = false;
        /// <summary>
        /// Is the game over?
        /// </summary>
        private bool _gameOver = false;
        /// <summary>
        /// The font used for the game
        /// </summary>
        private SpriteFont _font;
        // strings to show in the screen
        private const string GAME_FINISHED = "Game Finished!";
        private Vector2 _gameFinishedTextSize; 
        private const string GAME_OVER = "Game Over!";
        private Vector2 _gameOverTextSize;
        private const string RESTART = "Press R to Restart";
        private Vector2 _restartTextSize;
        private const string INSTRUCTIONS = "Press a or d to move or space to jump.";

        public World(Texture2D platformTextures, Dictionary<string, Animation> goalAnimations, SpriteFont font)
        {
            _platformTextures = platformTextures;
            _goalAnimations = goalAnimations;
            populateWorld();
            _view = 0;
            _font = font;

            // measure the string
            _gameFinishedTextSize = _font.MeasureString(GAME_FINISHED);
            _gameOverTextSize = _font.MeasureString(GAME_OVER);
            _restartTextSize = _font.MeasureString(RESTART);
        }

        /// <summary>
        /// populates the platformsPerView object. 
        /// NOTE, since the screen is 1280 x 768, and each platforms are 64 pixels,
        /// at most there will be 20x12 platforms. which can be expressed in a grid.
        /// </summary>
        private void populateWorld()
        {
            // define necessary variables for loading the world
            PlatformsPerView = new List<List<Platform>>();
            GoalsPerView = new List<List<Goal>>();
            char[] line;
            byte row, col, platformType;
            short views, vcounter;
            List<Platform> platformsInView;
            List<Goal> goalsInView;
            try
            {
                // read the map file
                StreamReader sr = new("Content/Maps/World1.dat");
                // get the amount of views in the map, at the first line of the file
                views = short.Parse(sr.ReadLine());
                _maxView = views - 1;
                Vector2 platPos;
                // for each view
                for (vcounter = 0; vcounter < views; vcounter++)
                {
                    // create a list of platform and list of goals for this view
                    platformsInView = new List<Platform>();
                    goalsInView = new List<Goal>();
                    // for each row of the view
                    // NOTE since the blocks are 64 pixels, and the height of the screen is 768, there will be 
                    // 12 blocks per row of the screen. On the other  hand since the screen's width is 1280, 
                    // there can be 20 blocks at most per row of the screen (if we divide each row and col by 64)
                    for (row = 0; row < 12; row++)
                    {
                        // split the line into a character array and iterate through it
                        line = sr.ReadLine().ToCharArray();
                        for (col = 0; col < 20; col++)
                        {
                            if (line[col] == 'G')   // if a character is G
                            {
                                // add a goal into the list
                                goalsInView.Add(new Goal(_goalAnimations,
                                    new(col * 64, row * 64), (Goal.GoalType)vcounter));
                            }
                            // if a character is X
                            else if (line[col] != 'X' && line[col] != '\n')
                            {   
                                platformType = (byte)(line[col] - '0'); // subtracting character '0' from char will result in its numerical character value 
                                // get the position of the platform
                                platPos = new(col * 64, row * 64);
                                // create a new platform 
                                platformsInView.Add(new Platform(
                                        _platformTextures,
                                        platPos,
                                        (Platform.PlatformType)platformType
                                    ));
                            }
                        }
                    }
                    // read empty line
                    sr.ReadLine();
                    // add to goals per view
                    GoalsPerView.Add(goalsInView);
                    // add to platforms per view
                    PlatformsPerView.Add(platformsInView);
                }
            } catch (Exception)
            {
                throw new FileNotFoundException("File not found! Error will occur!");
            }
        }
        /// <summary>
        /// Updates all the updatable sprites in view
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            // update remaining existing goals in current view
            foreach(Goal goalInView in GoalsPerView[_view])
            {
                goalInView.Update(gameTime);
            }
        }
        /// <summary>
        /// Draws all the sprites in the view
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // draw each platform
            foreach (Platform platformInView in PlatformsPerView[_view])
            {
                platformInView.Draw(spriteBatch);
            }
            // draw each goals
            foreach (Goal goalInView in GoalsPerView[_view])
            {
                // only drow goals that are not finished animating collected animation or
                // draw goals that are not yet collected
                if (!goalInView.End)
                    goalInView.Draw(spriteBatch);
            }
            // draw the texts
            DrawTexts(spriteBatch);
        }

        public void DrawTexts(SpriteBatch spriteBatch)
        {
            Vector2 restartCenter;
            if (_gameOver)
            {
                // draw the game over text at the center of the screen
                Vector2 gameOverCenter = new(1280 / 2 - _gameOverTextSize.X / 2, 768 / 2 - _gameOverTextSize.Y / 2);
                spriteBatch.DrawString(_font, GAME_OVER, gameOverCenter, Color.White);
                // draw RESTART text below the gameOver text
                restartCenter = new(1280 / 2 - _restartTextSize.X / 2, 768 / 2 + _gameOverTextSize.Y);
                spriteBatch.DrawString(_font, RESTART, restartCenter, Color.White);
            }
            if (_completed)
            {
                // draw game finished text at the center of the screen
                Vector2 gameFinishedCenter = new(1280 / 2 - _gameFinishedTextSize.X / 2, 768 / 2 - _gameFinishedTextSize.Y / 2);
                spriteBatch.DrawString(_font, GAME_FINISHED, gameFinishedCenter, Color.White);
                // draw RESTARt text below the gameFinished text
                restartCenter = new(1280 / 2 - _restartTextSize.X / 2, 768 / 2 + _gameFinishedTextSize.Y);
                spriteBatch.DrawString(_font, RESTART, restartCenter, Color.White);
            }
            // draw the instruction text at the top right of the screen
            spriteBatch.DrawString(_font, INSTRUCTIONS, new Vector2(20, 20), Color.White);
        }

        /// <summary>
        /// Returns the current visible platforms on the screen
        /// </summary>
        /// <returns></returns>
        public List<Platform> GetCurrentPlatforms ()
        {
            return PlatformsPerView[_view];
        }

        /// <summary>
        /// Returns the current visible goals on the screen
        /// </summary>
        /// <returns></returns>
        public List<Goal> GetCurrentGoals()
        {
            return GoalsPerView[_view];
        }

        /// <summary>
        /// Returns true if next view is successful, else false
        /// </summary>
        /// <returns></returns>
        public bool NextView()
        {
            if (_view <= _maxView)
            {
                _view++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Return false if prev view is successful, else false
        /// </summary>
        /// <returns></returns>
        public bool PrevView()
        {
            if (_view > 0)
            {
                _view--;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Increments collected count
        /// </summary>
        public void AddCollected()
        {
            _collectedCount++;
            if (_collectedCount > _maxView)
            {
                _completed = true;
            }
        }
        /// <summary>
        /// Sets the game state to game over
        /// </summary>
        public void TriggerGameOver()
        {
            _gameOver = true;
        }
        /// <summary>
        /// Restarts the world
        /// </summary>
        public void Restart()
        {
            // reset each goal
            foreach (List<Goal> listGoal in GoalsPerView)
            {
                foreach (Goal goal in listGoal)
                {
                    goal.Reset();
                }
            }
            // go to first screen
            _view = 0;
            // amount of collected goals is 0
            _collectedCount = 0;
            // not game over
            _gameOver = false;
            // not game complete
            _completed = false;
        }
    }
}
