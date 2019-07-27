/*
* Author: Allison
* File Name: ForestTrain.cs
* Project Name: ForestTrail
* Creation Date: Dec. 14, 2016
* Modified Date: Jan. 19, 2017
* Description: This is an endless runner game where the user has to reach the door, 
 * however they also need to select the correct direction to continue to the next level.
 * There are 2 obstacles: a blob and a gargoyle. The blob can be killed to get seconds removed, but the gargoyle cannot.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace ForestTrail
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ForestTrail : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Store the dimensions of the screen
        int screenWidth = 1000;
        int screenHeight = 600;

        MouseState prevMouse;
        MouseState mouse;

        //Store the state of the game
        int gameState = 0;

        //The possible game states
        const int MENU = 0;
        const int LEADERBOARD = 1;
        const int PLAY = 2;
        const int GAMEOVER = 3;
        const int GAMEEND = 4;
        const int PAUSED = 5;
        const int RIDDLE = 6;

        //Allows randomly generated numbers
        Random rng = new Random();

        //Stores the randomly generated number for levels
        int randomNum;

        KeyboardState prevKb;
        KeyboardState kb;

        //Stores the user's actions
        int charActions;

        //Store the character's possible actions
        const int STAND = 0;
        const int RUN = 1;
        const int JUMP = 2;
        const int ATTACK = 3;

        //Store whether the player was previously facing right
        bool charRight = true;

        //Will store the keys that are responsible for moving the character
        Keys[] userKbInput = new Keys[4] { Keys.Left, Keys.Right, Keys.Up, Keys.Space };

        //Store whether the character is near outside the forest
        bool outerForest = true;

        //Store the image of the light forest background
        Texture2D lightImg;

        //Store the second image of the light forest background
        Texture2D lightImg2;

        //Store the rectangle that will store the first image of the light background
        Rectangle lightBox1;

        //Store the rectangle that will come after the first rectangle
        Rectangle lightBox2;

        //Store the rectangle that will come after the second rectangle
        Rectangle lightBox3;

        //Store the image of the dark forest
        Texture2D darkImg;

        //Store the rectangle tha will hold the first image of the dark forest
        Rectangle darkBox1;

        //Store the rectangle that will come after the first rectangle
        Rectangle darkBox2;

        //Will store the character
        Character charAction;

        //Store all the images that will be used to animate the character
        Texture2D[] charRightTextures = new Texture2D[4];
        Texture2D[] charLeftTextures = new Texture2D[4];

        //Store the max frame numbers and number of frames in each row for each respective image
        int[] charTextMaxFrames = new int[4];
        int[] charTextNumFrames = new int[4];

        //Store the character's location, will be used initially to determine the character's initial location, but will 
        //be update as the character moves so that new animations can occur without having the character's location restarting
        Vector2 characterLocation;

        //Store whether the character can stand
        bool canStand = false;

        //The angle and speedof the character's jump
        float angle = 73;
        float speed = 7;

        //The trajectory's of the character's jump
        Vector2 traj;
        Vector2 scaledTraj;

        //Store the default trajectory that will be used to reset the character's trajectory after each jump
        Vector2 startingTraj;

        //Gravity
        Vector2 gravity;
        
        //To scale a meter for a pixel
        float scale = 1.0f;

        //Store the original location of the character
        Vector2 origLocation;

        //Store the rate which the screen will be animate by
        int screenAnimRate;

        //Store the images of the platforms
        Texture2D platformFull;
        Texture2D platformHalf;

        //Level Template Locations
        //Store the locations of the platforms
        //The location of platforms for varying levels
        Vector2[][] lvType = new Vector2[][]
            {
               new Vector2[7]  { new Vector2(0, 564), new Vector2(464, 564), new Vector2(540, 380), new Vector2(720, 170), new Vector2(1320, 170), new Vector2(2007, 564), new Vector2(1900, 170)},
               new Vector2[9] { new Vector2 (0, 149), new Vector2 (0, 564), new Vector2 (465, 564), new Vector2 (933,534), new Vector2 (1403, 564), new Vector2 (1173, 337), new Vector2 (1743, 149), new Vector2 (329, 337), new Vector2 (827, 144)},
               new Vector2[8] { new Vector2(0, 187), new Vector2(0, 564), new Vector2(445, 385), new Vector2(815, 187), new Vector2(965, 517), new Vector2(1463, 323), new Vector2(1617, 513), new Vector2(1741, 150)},
               new Vector2 [10] { new Vector2(0, 137), new Vector2(0, 564), new Vector2(325, 345), new Vector2(741, 137), new Vector2(687, 564), new Vector2(1059, 564), new Vector2(1734, 557), new Vector2(1893, 353), new Vector2(1739, 160), new Vector2(1497, 115)},
               new Vector2[7] { new Vector2(0, 169), new Vector2(467, 115), new Vector2(335, 400), new Vector2(857, 564), new Vector2(1383, 405), new Vector2(1781, 217), new Vector2(1739, 564)}
            };
        //Store the locations of the obstacles
        Vector2[][] lvTypeObst = new Vector2[][]
            {
                new Vector2[5] {new Vector2(877, 503), new Vector2(621, 318), new Vector2(1315, 110), new Vector2(724, 129), new Vector2(1981, 515)},
                new Vector2[6] {new Vector2(323, 499), new Vector2(950, 470), new Vector2(1497, 277), new Vector2(827, 94), new Vector2(329, 287), new Vector2(1743, 99)},
                new Vector2[8] {new Vector2(815, 137), new Vector2(1463, 1866), new Vector2(1741, 100), new Vector2(0, 514), new Vector2(417, 501), new Vector2(1227, 455), new Vector2 (859, 325), new Vector2(1849, 447)},
                new Vector2[8] {new Vector2(357, 499), new Vector2(709, 280), new Vector2(1153, 499), new Vector2(2023, 499), new Vector2(1837, 100), new Vector2(1893, 305), new Vector2(687, 514), new Vector2(741, 87) },
                new Vector2[8] {new Vector2(657, 340), new Vector2(1193, 500), new Vector2(2159, 149), new Vector2(1781, 167), new Vector2(1383, 355), new Vector2(1739, 514), new Vector2(857, 514), new Vector2(467, 70)}
            };

        //Will store the end of where the obstacle is supposed to move
        int[][] motionEnd = new int[][]
            {
                new int[5]{ 0, 0, 0, 1327, 2300},
                new int[6]{ 0, 0, 0,1230, 732, 2146},
                new int[8]{ 1218, 1866, 2144, 367, 0, 0, 0, 0},
                new int[8]{ 0, 0, 0, 0, 0, 2061, 1087, 1144},
                new int[8]{ 0, 0, 0, 2093, 1786, 2142, 1127, 870}
            };

        //Store whether the obstacle will emmit fire
        bool[][] isFireEmmit = new bool[][]
            {
                new bool[5]{ true, true, true, false, false},
                new bool[6]{ true, true, true, false, false, false},
                new bool[8] { false, false, false, false, true, true, true, true},
                new bool[8] {true, true, true, true, true, false, false, false},
                new bool[8] {true, true, true, false, false, false, false, false} 
            };

        //Store the location of the bonuses in each level
        Vector2[][] bonusLv = new Vector2[][]
            {
                new Vector2[2] { new Vector2(700, 564), new Vector2(925, 380) },
                new Vector2[3] { new Vector2(250, 564), new Vector2(350, 149), new Vector2(1200, 337) },
                new Vector2[3] { new Vector2(150, 564), new Vector2(2017, 513), new Vector2(1000, 187) },
                new Vector2[3] { new Vector2(175, 564), new Vector2(2000, 353), new Vector2(1900, 557) },
                new Vector2[2] { new Vector2(450, 400), new Vector2(1900, 217)}
            };

        //Store the bonuses in the level
        ParticleEngine[] bonus;

        //Store how many bonuses have been collected
        int bonusObt;

        //Store the characters that will be the obstacles
        Character[] obstacles;

        //Will store the rectangles that will store the images of the obstacles
        Rectangle[] obstacleBox;

        //Will store whether the obstacles will emit fire
        bool[] fireEmit;

        //Will store where the obstacle will move
        int[] motionObst;

        //Store the orig location of the obstacles
        Vector2[] origObstLoc;

        //Store the particle engine, which will be the fire
        ParticleEngine[] particleEngineObst;

        //Will store the images of the obstacles
        Texture2D blobImg;
        Texture2D bodyImg;
        Texture2D headImg;

        //Stores the max number of frames, which is also the number of frames in each row since there is only one row
        int blobMaxFrame = 5;
        int headMaxFrame = 6;

        //Will store the rectangles that will store the images of the platforms
        Rectangle[] platformBox;

        //Will store whether the platform is moving down or up
        bool pfHalfDown = true;

        //Store which platform the character is on
        int curPlatform = 0;

        //Store whether the character is falling
        bool charFall = false;

        //Store how much time has passed in the game
        int counter;

        //Store the textures that will be used for the particles depending on what the particle is used for
        Texture2D starImg;
        Texture2D fireImg;

        //Store whether the moving obstacle is moving right
        bool obstRight = true;

        //Store the images of the game title and company logo
        Texture2D gameTitleImg;
        Texture2D companyImg;

        //Store the boxes that will store the game title and comapny logo
        Rectangle gameTitleBox;
        Rectangle companyBox;

        //Store the menu background img
        Texture2D gameTitleScreenImg;

        //Store the rectangle that will store the menu backgorund img
        Rectangle gameTitleScreenBox;

        //Store the buttons that will be visible
        Texture2D playBttnImg;
        Texture2D leaderboardBttnImg;
        Texture2D quitBttnImg;
        Texture2D loadBttnImg;
        Texture2D playAgainBttnImg;
        Texture2D returnBttnImg;

        //Store the rectangles that will store the button imgs
        Rectangle playBttnBox;
        Rectangle leaderboardBttnBox;
        Rectangle quitBttnBox;
        Rectangle loadBttnBox;
        Rectangle playAgainBttnBox;
        Rectangle returnBttnBox;

        //Store which game template is the game currently using
        int gameTemplate = 4;

        //Stores whether the user has landed onto a firebreathing obstacle
        bool firstLanded = false;

        //Stores which fire breathing obstacle the character has collided with 
        int collidedObst = -1;

        //Audio
        //Songs
        Song menuBgMusic;
        Song lightBgMusic;
        Song darkBgMusic;

        //Sound Effect
        SoundEffect bonusObtSe;
        SoundEffect burnedSe;
        SoundEffect buttonClkSe;
        SoundEffect fireSe;
        SoundEffect gameOverSe;
        SoundEffect attackSe;
        SoundEffect gameSuccessSe;
        SoundEffect landingSe;
        SoundEffect bounceSe;

        //Sound Effect Instances
        SoundEffectInstance bonusObtSeInstance;
        SoundEffectInstance burnedSeInstance;
        SoundEffectInstance buttonClkSeInstance;
        SoundEffectInstance fireSeInstance;
        SoundEffectInstance gameOverSeInstance;
        SoundEffectInstance attackSeInstance;
        SoundEffectInstance gameSuccessSeInstance;
        SoundEffectInstance landingSeInstance;
        SoundEffectInstance bounceSeInstance;

        //Store whether the user wants music
        bool music = false;

        //Store whether the user wants sound effects
        bool soundEffects = false;

        //Store the order of the templates used
        string tempLvOrder;

        //Order of levels in an array
        int[] lvlOrder = new int[5];

        //Whether the user is on an animated platform
        bool movingObst;

        //Loction of the door in the game
        Vector2[] doorLoc = new Vector2[5] { new Vector2(2201, 414), new Vector2(1930, -1), new Vector2(1955, 5), new Vector2(2040, 10), new Vector2(2028, 414) };

        //Store the image of a door
        Texture2D doorImg;

        //Store the rectangle box
        Rectangle doorBox;

        //Location of the save sign in the game
        Vector2[] saveSignLoc = new Vector2[5] { new Vector2(200, 505), new Vector2(200, 90), new Vector2(200, 128), new Vector2(200, 78), new Vector2(200, 110) };

        //Stores the save sign image
        Texture2D saveSignImg;

        //Stores the save sign rectangle
        Rectangle saveSignBox;

        //First level
        int level = 1;

        //Score/timer (the lower the time it takes to complete the game the better)
        int second;

        //Stores the font used to display stuff
        SpriteFont font;

        //Store the starting scores
        int[] scoreRecord = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        //Used to move scores around
        int tempScore;

        //Store the devices used to read and write into files outside of the game
        StreamWriter outFile;
        StreamReader inFile;

        //Store whether ther is a save file
        bool savedFile;

        //Store the location of the first score
        Vector2 scoreLocInitial;

        //Store the location of the title
        Vector2 leaderBoardLoc;

        //Stores the location of the lines of scores on the scoreboard
        Vector2[] scoreRecordLoc = new Vector2[10];

        //Store if the user has saved
        bool saved;

        //Store how many seconds have passed since the save
        int saveCount;

        //Stores whether the score has been updated
        bool scoreUpdated;

        //Store the sound buttons
        Texture2D soundBttn;
        Texture2D muteBttn;

        //Store the rectangles that will store the sound bttn imgs
        Rectangle soundBttnBox;
        Rectangle muteBttnBox;

        //Store the riddle that will be presented at the beginning
        String riddle = "What is at the end of the rainbow?\nA pot of gold (L)     The letter W(R)\nWhat is harder to catch the faster you run?\nYour breath(L)     Apples(R)\nWhat flies without wings?\nTime(R)     Flying squirrel(L)\nWhat vehicle is spelt the same backwards?\nRacecar(R)     Car(L)";

        //Store the location of the riddle
        Vector2 riddleLoc = new Vector2(50, 100);

        //Stores the correct answer
        bool[] correctAnsRight = new bool[4] { true, false, true, true};

        //Store the buttton images
        Texture2D rightBttn;
        Texture2D leftBttn;

        //Store the boudnig rectangles
        Rectangle rightBttnBox;
        Rectangle leftBttnBox;

        public ForestTrail()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>Boc
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //Specifies the screen size that will change depending on where the player is
            this.graphics.PreferredBackBufferWidth = screenWidth;
            this.graphics.PreferredBackBufferHeight = screenHeight;

            //Apply such changes to screen size
            this.graphics.ApplyChanges();

            //The mouse is visible
            IsMouseVisible = true;

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

            //Generate levels
            LvGen();

            //Set the game templat
            gameTemplate = lvlOrder[0];

            //Initialize the stuff needed for the game
            InitPreGame();

            //Character is standing
            charActions = STAND;

            //Initialize the character images
            CharTextInit();

            //Initialize the background images
            BackgroundInit();

            //Initializes the platforms
            InitPlatforms(gameTemplate);

            //Initializes the obstacles
            InitObstacles(gameTemplate);

            //Initializes the bonuses
            InitBonus(gameTemplate);

            //Initialize the character's location
            characterLocation = new Vector2(0, platformBox[curPlatform].Y - 100);

            //Update the origiinal location
            origLocation = characterLocation;

            //Set the character's animation
            charAction = new Character(charRightTextures[0], charTextNumFrames[0], charTextMaxFrames[0], characterLocation, false);

            //Jumping
            //Set the value of the original trajectory
            traj = new Vector2(speed * (float)Math.Cos(MathHelper.ToRadians(angle)), speed * (float)Math.Sin(MathHelper.ToRadians(angle)) * -1);

            //Stores the original trajectory
            startingTraj = traj;

            //Set gravity
            gravity.Y = 9.8f / 60;

            //Load the sounds
            LoadSounds();

            //Check if there are previously saved files
            CheckFiles();

            //Read and load saved scores
            ReadFiles(true, @"/Score.txt");

            //Initializes the leaderboard title
            leaderBoardLoc = new Vector2(350, 50);

            //Initializes the location of the first score
            scoreLocInitial = new Vector2(25, 100);

            //Initializes the location of the scoreboard scores
            for (int i = 0; i < scoreRecordLoc.Length; i = i + 1)
            {
                //For each index initilize its x and y location
                scoreRecordLoc[i] = new Vector2(scoreLocInitial.X, scoreLocInitial.Y);

                //Allows each index's x value to be 25 more than the previous
                scoreLocInitial.Y = scoreLocInitial.Y + 30;
            }

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            //Store previous mouse state and get new mouse state
            prevMouse = mouse;
            mouse = Mouse.GetState();
            
            //Store previous keyoard state and get new keyboard state
            prevKb = kb;
            kb = Keyboard.GetState();

            //Which game state
            switch (gameState)
            {
                //Menu
                case MENU:
                    //Alter the size of the screen
                    ScreenSize(gameTitleScreenImg.Width, gameTitleScreenImg.Height);

                    //Play background music if there is sound
                    PlayMusic(menuBgMusic);

                    //Increase counter
                    counter++;

                    //New mouse click
                    //There was
                    if (NewMouseClick() == true)
                    {
                        //Which button was clicked
                        //Play
                        if (MouseClicked(playBttnBox) == true)
                        {
                            //Play the game
                            gameState = PLAY;

                            //Reset the counter
                            counter = 0;

                            //Resets music
                            MusicReset(menuBgMusic);
                        }
                        //Load
                        else if (MouseClicked(loadBttnBox) == true)
                        {
                            //If there is a saved file
                            //There is
                            if (savedFile == true)
                            {
                                //Read the file
                                ReadFiles(false, @"/Levels.txt");

                                //For ever character in the string
                                for (int i = 0; i < tempLvOrder.Length; i++)
                                {
                                    //Convert to an integer and store in the array
                                    lvlOrder[i] = Convert.ToInt32(Convert.ToString(tempLvOrder[i]));
                                }

                                //Set the game template
                                gameTemplate = lvlOrder[level - 1];

                                //Update the platforms, bonuses, and obstacles
                                ContNextLvl(gameTemplate);

                                //Play the game
                                gameState = PLAY;
                            }
                        }
                        //Leaderboard
                        else if (MouseClicked(leaderboardBttnBox) == true)
                        {
                            //Go to the leaderboard
                            gameState = LEADERBOARD;
                        }
                        //Quit
                        else if (MouseClicked(quitBttnBox) == true)
                        {
                            //Exit the game
                            this.Exit();
                        }
                        //Sound
                        else if (MouseClicked(soundBttnBox) == true)
                        {
                            //There will be sound and music
                            music = true;
                            soundEffects = true;
                        }
                        //Mute
                        else if (MouseClicked(muteBttnBox) == true)
                        {
                            //There will be no sound or music
                            music = false;
                            soundEffects = false;
                        }

                        //Play the sound of a button clicking
                        PlaySoundEffects(buttonClkSeInstance);
                    }

                    //Exit switch statement
                    break;

                case LEADERBOARD:
                    //Play the background music
                    PlayMusic(menuBgMusic);

                    //If new mouse click
                    //New mouse click
                    if (NewMouseClick() == true)
                    {
                        //What did the mouse click
                        //The return button
                        if (MouseClicked(returnBttnBox) == true)
                        {
                            //Return to menu
                            gameState = MENU;
                        }
                    }

                    //Exit switch statement
                    break;

                case PLAY:

                    //Increase the counter
                    counter++;

                    //If the counter is divisible by 60
                    //It is
                    if (counter % 60 == 0)
                    {
                        //Increase by one seconf
                        second++;
                    }

                    //Adjust the size of the screen
                    ScreenSize(1000, 600);

                    //If the character is on the outside of the forest
                    //It is
                    if (outerForest == true)
                    {
                        //Play the proper music
                        PlayMusic(lightBgMusic);
                    }
                    //It is not
                    else
                    {
                        //Play the music for within the forest
                        PlayMusic(darkBgMusic);
                    }

                    //Update the game
                    GameUpdate();

                    //Check to see if the user is entering the door
                    DoorCollision();

                    //Check to see if the character has fallen off screen
                    if (charAction.charBox.Y >= screenHeight + 100)
                    {
                        //Gameover
                        gameState = GAMEOVER;
                    }

                    //If the user has saved
                    //It has
                    if (saved == true)
                    {
                        //Increase
                        saveCount++;

                        //If saved count is larger than 120
                        if (saveCount > 120)
                        {
                            //Reset saved count
                            saveCount = 0;

                            //Set saved to false
                            saved = false;
                        }
                    }

                    //Check to see if the user is saving the file
                    SaveCollision();

                    //Exit switch statement
                    break;

                case GAMEOVER:

                    //Play the sound effect of game over
                    PlaySoundEffects(gameOverSeInstance);

                    //Play menu music
                    PlayMusic(menuBgMusic);

                    ButtonClick();

                    //Exit switch statement
                    break;

                case GAMEEND:

                    //Adjust the size of the screen
                    ScreenSize(gameTitleScreenImg.Width, gameTitleScreenImg.Height);

                    //Play menu music
                    PlayMusic(menuBgMusic);

                    //If the score has not been updated
                    if (scoreUpdated == false)
                    {
                        //Update score
                        UpdateScore();
                    }

                    //Check for button clicks
                    ButtonClick();

                    //Exit switch statement
                    break;

                case RIDDLE:

                    //Which level
                    //Level 2
                    if (level == 2)
                    {
                        //If the user has pressed shift
                        //They have
                        if (prevKb.IsKeyUp(Keys.RightShift) == true && kb.IsKeyDown(Keys.RightShift) == true)
                        {
                            //Continue ot next level
                            gameState = PLAY;
                        }
                    }
                    //Not level 2
                    else
                    {
                        //If there is a new mouse click
                        if (NewMouseClick() == true)
                        {
                            //For every stage
                            for (int i = 0; i < correctAnsRight.Length; i++)
                            {
                                //If it is the proper answer for the level
                                //It is
                                if (i == level - 3)
                                {
                                    //Which box was clicked and was right clicked correctly
                                    //Clicked correctly
                                    if (MouseClicked(rightBttnBox) == correctAnsRight[i])
                                    {
                                        //Which level
                                        //Last stage
                                        if (level == 6)
                                        {
                                            //End game
                                            gameState = GAMEEND;
                                        }
                                        //Not the last stage
                                        else
                                        {
                                            //Cont to next level
                                            gameState = PLAY;
                                        }
                                    }
                                    //Not clicked correctly
                                    else if (MouseClicked(leftBttnBox) == true)
                                    {
                                        //Reset the level
                                        level = 1;

                                        //Update the game template
                                        gameTemplate = lvlOrder[level - 1];

                                        //Load the proper things to continue to next level
                                        ContNextLvl(lvlOrder[level - 1]);

                                        //Play from the begginign
                                        gameState = PLAY;
                                    }
                                }
                            }
                        }
                    }

                    break;
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Blue background
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            //Which game state
            switch (gameState)
            {
               //Menu
                case MENU:

                    //If the counter is over 60
                    //It is
                    if (counter > 60)
                    {
                        //Draw the game screen and title
                        spriteBatch.Draw(gameTitleScreenImg, gameTitleScreenBox, Color.White);
                        spriteBatch.Draw(gameTitleImg, gameTitleBox, Color.White);

                        //Draw the buttons
                        ButtonDraw();
                    }
                    //It isnt
                    else
                    {
                        //Draw the company name on a light gray background
                        GraphicsDevice.Clear(Color.LightGray);
                        spriteBatch.Draw(companyImg, companyBox, Color.White);
                    }

                    //Exit switch statement
                    break;

                //Leaderboard
                case LEADERBOARD:            

                    //Displays the scoreboard
                    spriteBatch.DrawString(font, "LeaderBoard", leaderBoardLoc, Color.White);

                    //Displays each score in the index in the order of highest to lowest
                    for (int i = 0; i < scoreRecord.Length; i = i + 1)
                    {
                        //Displays each score and is numbered
                        spriteBatch.DrawString(font, (i + 1) + ".................................. " + scoreRecord[i] + "seconds" , scoreRecordLoc[i], Color.White);
                    }

                    //Exit switch statement
                    break;

                //Play
                case PLAY:
                    DrawGame();

                    //Display the score
                    spriteBatch.DrawString(font, "Timer: " + second + " sec", new Vector2(0, 0), Color.White);

                    //Display the level
                    spriteBatch.DrawString(font, "Level   " + level, new Vector2(screenWidth - 150, 0), Color.White);

                    //If it has saved
                    if (saveCount <= 120 && saved == true)
                    {
                        //Write saved
                        spriteBatch.DrawString(font, "Saved", new Vector2 (500, 300), Color.LawnGreen);
                    }

                    //Exit switch statement
                    break;

                //Gameover
                case GAMEOVER:

                    //Draw the obstacles, character, bonuses, and platforms
                    DrawGame();

                    //Display the game over message and score
                    spriteBatch.DrawString(font, "Game Over", new Vector2(300, 200), Color.Red);
                    spriteBatch.DrawString(font, "Score: " + second + " seconds", new Vector2(300, 250), Color.Red);

                    //Draw the buttons
                    ButtonDraw();

                    //Exit switch statement
                    break;

                //End of game
                case GAMEEND:

                    //Draw the game tile screen and title
                    spriteBatch.Draw(gameTitleScreenImg, gameTitleScreenBox, Color.White);
                    spriteBatch.Draw(gameTitleImg, gameTitleBox, Color.White);

                    //Display congraulations message and score
                    spriteBatch.DrawString(font, "Congrats", new Vector2(300, 200), Color.Black);
                    spriteBatch.DrawString(font, "Score: " + second + " seconds", new Vector2(300, 250), Color.Black);

                    //Draw the buttons
                    ButtonDraw();

                    //Exit switch statement
                    break;

                //Riddle
                case RIDDLE:

                    //If it is level 2
                    //It is
                    if (level == 2)
                    {
                        //Display the riddles
                        spriteBatch.DrawString(font, riddle, riddleLoc, Color.White);

                        //Display instructions
                        spriteBatch.DrawString(font, "Press SHIFT to continue", new Vector2(300, 500), Color.White);
                    }
                    //It isnt
                    else
                    {
                        //Display the buttons
                        spriteBatch.Draw(leftBttn, leftBttnBox, Color.White);
                        spriteBatch.Draw(rightBttn, rightBttnBox, Color.White);
                    }

                    //Exit switch statement
                    break;

            }
            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        //Pre: None
        //Post: Return whether there was a new mouse click
        //Desc: Detect whether there was a was a mouse click and whether there was a mouse click before
        private bool NewMouseClick()
        {
            //Stores if there is a new click
            Boolean clicked = false;

            //If there is a new click
            //There is
            if (mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton != ButtonState.Pressed)
            {
                //Update clicked
                clicked = true;
            }

            //Return clicked
            return clicked;
        }

        //Pre: A rectangle
        //Post: Whether the user clicked on the rectangle
        //Desc: Detect the user's mouse is within the x and y coordinates of the rectangle
        private bool MouseClicked(Rectangle box)
        {
            //Store whether the box has been clicked
            Boolean result = false;

            //IF the box has been clicked
            //It has
            if (mouse.X >= box.X && mouse.X <= (box.X + box.Width) && mouse.Y >= box.Y && mouse.Y <= (box.Y + box.Height))
            {
                //Update results
                result = true;
            }

            //Return results
            return result;
        }

        //Pre: minValue is equal to 0, maxValue is a positive number larger than 0
        //Post: Returns a random number between the minValue and the maxValue
        //Description: Generates a random number that will determine which type of level is displayed
        private int RandomNumGenerator(int minValue, int maxValue)
        {
            //Generates a random number that will be assigned to the variable randomNum
            randomNum = rng.Next(minValue, maxValue);

            //Return the random number
            return randomNum;
        }

        //Pre: None
        //Post: The character's action will change
        //Desc: Detecting input from the keyboard and checking if the user can make that action
        private void CharActionDetect()
        { 
            //Which action is the character currently commintin
            //Running
            if (charActions == RUN)
            {
                //For each of the desire keyboard input
                for (int i = 0; i < userKbInput.Length; i++)
                {
                    //If an arrow is pressef
                    //It is
                    if (i == 0 || i == 1)
                    {
                        //What is pressed next
                        //STill an arrow
                        if (prevKb.IsKeyDown(userKbInput[i]) == true && kb.IsKeyDown(userKbInput[i]) == true)
                        {
                            //Exit for loop
                            break;
                        }
                        //A new action
                        else if (prevKb.IsKeyDown(userKbInput[i]) == true && kb.IsKeyDown(userKbInput[i]) != true)
                        {
                            //Which key was pressed?
                            //The up button
                            if (prevKb.IsKeyDown(userKbInput[2]) != true && kb.IsKeyDown(userKbInput[2]) == true)
                            {
                                //Update character actions
                                charActions = JUMP;
                                canStand = false;

                                //Play the sound of a bounce
                                PlaySoundEffects(bounceSeInstance);

                            }
                            //The space button
                            else if (prevKb.IsKeyDown(userKbInput[3]) != true && kb.IsKeyDown(userKbInput[3]) == true)
                            {
                                //Update character actions
                                charActions = ATTACK;
                                canStand = false;

                                //Play the sound of an attack
                                PlaySoundEffects(attackSeInstance);

                            }
                            //None
                            else
                            {
                                //Update character actions
                                canStand = false;
                                charActions = STAND;
                            }
                            CharActionsLoadText();

                            //Exit for loop
                            break;
                        }
                    }
                    //Not a left or right arrow
                    else
                    {
                        //If it is a new key
                        //It is
                        if (prevKb.IsKeyDown(userKbInput[i]) != true && kb.IsKeyDown(userKbInput[i]) == true)
                        {
                            //Which key
                            switch (i)
                            {
                                //Up
                                case 2:

                                    //Update the character's action
                                    charActions = JUMP;
                                    canStand = false;

                                    //Play the sound of a bounce
                                    PlaySoundEffects(bounceSeInstance);

                                    //Exit switch statement
                                    break;
                                //Space
                                case 3:
                                    //Update the character's action
                                    charActions = ATTACK;
                                    canStand = false;

                                    //Play the sound of a bounce
                                    PlaySoundEffects(attackSeInstance);

                                    //Exit the switch statement
                                    break;
                            }

                            //Load the proper images
                            CharActionsLoadText();

                            //Exit for loop
                            break;
                        }
                        //No key is being pressed and character can stand
                        else if (prevKb.IsKeyDown(userKbInput[i]) != true && kb.IsKeyDown(userKbInput[i]) != true && canStand == true)
                        {
                            //Character is standing
                            charActions = STAND;
                            CharActionsLoadText();
                        }
                    }
                }
            }
            //Standing
            else
            {
                //For every desired keyboard input
                for (int i = 0; i < userKbInput.Length; i++)
                {
                    //If new key
                    //New key was entered
                    if (prevKb.IsKeyDown(userKbInput[i]) != true && kb.IsKeyDown(userKbInput[i]) == true)
                    {
                        //Which key
                        switch (i)
                        {
                            //Left
                            case 0:
                                //If the character is not jumping or running
                                //It isnt
                                if (charActions != JUMP && charActions != ATTACK)
                                {
                                    //Character is running towards the left
                                    charActions = RUN;
                                    charRight = false;
                                    canStand = true;
                                }
                                break;
                            //Right
                            case 1:
                                //If the character is not jumping or running
                                //It isnt
                                if (charActions != JUMP && charActions != ATTACK)
                                {
                                    //Character is running towards the right
                                    charActions = RUN;
                                    charRight = true;
                                    canStand = true;
                                }
                                break;
                            //Up
                            case 2:
                                //Characyer is jumpu
                                charActions = JUMP;
                                canStand = false;

                                //Play the sound of a bounce
                                PlaySoundEffects(bounceSeInstance);


                                //Exit switch statement
                                break;
                            //Space
                            case 3:
                                //Character is attacking
                                charActions = ATTACK;
                                canStand = false;

                                //Play the sound of a bounce
                                PlaySoundEffects(attackSeInstance);

                                //Exit switch statement
                                break;
                        }

                        //Load preper imafes
                        CharActionsLoadText();

                        //Exit forloop
                        break;
                    }
                    //No new key
                    else if (prevKb == kb && canStand == true)
                    {
                        //Character is standing
                        charActions = STAND;
                        CharActionsLoadText();
                    }
                }
            }
        }

        //Pre: None
        //Post: The character will have the proper image for their actions
        //Desc: Load the proper images depending on the actions of the character
        private void CharActionsLoadText()
        {
            //If the character is facing the right
            //It is
            if (charRight == true)
            {
                //Fore every action
                for (int i = 0; i <= 3; i++)
                {
                    //If the character action is equivalent to the one reached
                    //It is
                    if (charActions == i)
                    {
                        //Update the character
                        charAction = new Character(charRightTextures[i], charTextNumFrames[i], charTextMaxFrames[i], characterLocation, false);
                    }
                }

                //If character's frame num has reached the max and the character is attacking
                //It is
                if (charAction.frameNum >= charAction.maxFrame && charActions == ATTACK)
                {
                    //Reset character to standin
                    charAction = new Character(charRightTextures[0], charTextNumFrames[0], charTextMaxFrames[0], characterLocation, false);
                }
            }
            //It is not
            else
            {
                //For every action
                for (int i = 0; i <= 3; i++)
                {
                    //If the character action is equivalent to the one reached
                    //It is
                    if (charActions == i)
                    {
                        //Update the character
                        charAction = new Character(charLeftTextures[i], charTextNumFrames[i], charTextMaxFrames[i], characterLocation, false);
                    }
                }

                //If character's frame num has reached the max and the character is attacking
                //It is
                if (charAction.frameNum >= charAction.maxFrame && charActions == ATTACK)
                {
                    //Reset character to standin
                    charAction = new Character(charLeftTextures[0], charTextNumFrames[0], charTextMaxFrames[0], characterLocation, false);
                }
            }
        }

        //Pre: None
        //Post: The images and rectangles will be loaded 
        //Desc: Load the images and rectangles that will be used to display the character
        private void CharTextInit()
        {
            //For each possible action
            for (int i = 0; i < charRightTextures.Length; i++)
            {
                //Which actions
                switch (i)
                {
                    //Character is standing
                    case 0:
                        //Load the images for both directions
                        charRightTextures[i] = Content.Load<Texture2D>("Images/CharActions/Right/RightStanding");
                        charLeftTextures[i] = Content.Load<Texture2D>("Images/CharActions/Left/LeftStanding");

                        //Specify the amount of frames
                        charTextMaxFrames[i] = 1;
                        charTextNumFrames[i] = 1;

                        //Exit switch statement
                        break;
                    //Run
                    case 1:
                        //Load the images for both directions
                        charRightTextures[i] = Content.Load<Texture2D>("Images/CharActions/Right/RightRun");
                        charLeftTextures[i] = Content.Load<Texture2D>("Images/CharActions/Left/LeftRun");

                        //Specify the amount of frames
                        charTextMaxFrames[i] = 6;
                        charTextNumFrames[i] = 6;

                        //Exit switch statement
                        break;
                    //Jump
                    case 2:
                        //Load the images for both directions
                        charRightTextures[i] = Content.Load<Texture2D>("Images/CharActions/Right/RightJump");
                        charLeftTextures[i] = Content.Load<Texture2D>("Images/CharActions/Left/LeftJump");

                        //Specify the amount of frames
                        charTextMaxFrames[i] = 5;
                        charTextNumFrames[i] = 5;

                        //Exit switch statement
                        break;
                    //Attack
                    case 3:
                        //Load the images for both directions
                        charRightTextures[i] = Content.Load<Texture2D>("Images/CharActions/Right/RightAttack");
                        charLeftTextures[i] = Content.Load<Texture2D>("Images/CharActions/Left/LeftAttack");

                        //Specify the amount of frames
                        charTextMaxFrames[i] = 13;
                        charTextNumFrames[i] = 13;

                        //Exit switch statement
                        break;
                }
            }
        }

        //Pre: None
        //Post: The images and rectangles needed for the menu and obstacles will be loaded
        //Desc: Load the images and rectangles of the menu, buttons, and obstacles
        private void InitPreGame()
        {
            //Load the images that will be used as the game title, company logo and menu background
            gameTitleImg = Content.Load<Texture2D>("Images/Titles/GameTitle");
            companyImg = Content.Load<Texture2D>("Images/Titles/CompanyName");
            gameTitleScreenImg = Content.Load<Texture2D>("Images/Background/GameTitleScreen");

            //Initialize the rectangles that will hold the images above
            gameTitleBox = new Rectangle((gameTitleScreenImg.Width / 2) - (gameTitleImg.Width / 2), 50, gameTitleImg.Width, gameTitleImg.Height);
            companyBox = new Rectangle((gameTitleScreenImg.Width / 2) - (gameTitleImg.Width / 2), 100, gameTitleImg.Width, gameTitleImg.Height);
            gameTitleScreenBox = new Rectangle(0, 0, gameTitleScreenImg.Width, gameTitleScreenImg.Height);

            //Load the images of buttons
            playBttnImg = Content.Load<Texture2D>("Images/Buttons/PlayBttn");
            leaderboardBttnImg = Content.Load<Texture2D>("Images/Buttons/LeaderboardBttn");
            quitBttnImg = Content.Load<Texture2D>("Images/Buttons/QuitBttn");
            loadBttnImg = Content.Load<Texture2D>("Images/Buttons/LoadBttn");
            playAgainBttnImg = Content.Load<Texture2D>("Images/Buttons/PlayAgainBttn");
            returnBttnImg = Content.Load<Texture2D>("Images/Buttons/ReturnBttn");
            soundBttn = Content.Load<Texture2D>("Images/Buttons/SoundBttn");
            muteBttn = Content.Load<Texture2D>("Images/Buttons/MuteBttn");
            rightBttn = Content.Load<Texture2D>("Images/Buttons/RightBttn");
            leftBttn = Content.Load<Texture2D>("Images/Buttons/LeftBttn");

            //Initialize the rectangles that will store the buttons
            playBttnBox = new Rectangle((gameTitleScreenImg.Width / 2) - (playBttnImg.Width / 2), gameTitleBox.Y + gameTitleBox.Height + 50, playBttnImg.Width, playBttnImg.Height);
            loadBttnBox = new Rectangle((gameTitleScreenImg.Width / 2) - (loadBttnImg.Width / 2), playBttnBox.Y + playBttnBox.Height + 10, loadBttnImg.Width, loadBttnImg.Height);
            leaderboardBttnBox = new Rectangle((gameTitleScreenImg.Width / 2) - (leaderboardBttnImg.Width / 2), loadBttnBox.Y + playBttnBox.Height + 10, leaderboardBttnImg.Width, leaderboardBttnImg.Height);
            quitBttnBox = new Rectangle((gameTitleScreenImg.Width / 2) - (quitBttnImg.Width / 2), leaderboardBttnBox.Y + leaderboardBttnBox.Height + 10, quitBttnImg.Width, quitBttnImg.Height);
            playAgainBttnBox = new Rectangle((gameTitleScreenImg.Width / 2) - (playAgainBttnImg.Width), 350, playAgainBttnImg.Width, playAgainBttnImg.Height);
            returnBttnBox = new Rectangle(playAgainBttnBox.Right + 50, playAgainBttnBox.Y, returnBttnImg.Width, returnBttnImg.Height);
            muteBttnBox = new Rectangle(screenWidth - 100 - muteBttn.Width, 10, muteBttn.Width, muteBttn.Height);
            soundBttnBox = new Rectangle(muteBttnBox.X - 5 - soundBttn.Width, muteBttnBox.Y, soundBttn.Width, soundBttn.Height);
            leftBttnBox = new Rectangle((screenWidth / 2) - 25 - leftBttn.Width, (screenHeight / 2) - (leftBttn.Height / 2), leftBttn.Width, leftBttn.Height);
            rightBttnBox = new Rectangle(leftBttnBox.X + leftBttnBox.Width + 50, leftBttnBox.Y, rightBttn.Width, rightBttn.Height);

            //Load the images of the obstacles
            blobImg = Content.Load<Texture2D>("Images/Obstacles/Blob");
            headImg = Content.Load<Texture2D>("Images/Obstacles/FireHead");
            bodyImg = Content.Load<Texture2D>("Images/Obstacles/Fire");

            //Initialize the images of the particles
            starImg = Content.Load<Texture2D>("Images/Particles/Star");
            fireImg = Content.Load<Texture2D>("Images/Particles/Fire");
        }

        //Pre: None
        //Post: The images for the background and platforms will be loaded
        //Desc: Load the images and rectangles that will be used for the backgrounds, platforms, signs, and font
        private void BackgroundInit()
        {
            //Load background images
            lightImg = Content.Load<Texture2D>("Images/Background/LightBackgrnd");
            lightImg2 = Content.Load<Texture2D>("Images/Background/LightBackgrnd2");
            darkImg = Content.Load<Texture2D>("Images/Background/DarkBackgrnd");

            //Initialize the rectangles that will hold the images for the outer  part of the forest
            lightBox1 = new Rectangle(0, 0, lightImg.Width, lightImg.Height);
            lightBox2 = new Rectangle(lightBox1.X + lightImg.Width, 0, lightImg.Width, lightImg.Height);
            lightBox3 = new Rectangle(lightBox2.X + lightImg.Width, 0, lightImg2.Width, lightImg2.Height);

            //Initialize the rectangles that will hold the images for the inner part of the forest
            darkBox1 = new Rectangle(0, 0, darkImg.Width, darkImg.Height);
            darkBox2 = new Rectangle(darkBox1.X + darkImg.Width, 0, darkImg.Width, darkImg.Height);

            //Load platform imgs
            platformFull = Content.Load<Texture2D>("Images/Platforms/PfFull");
            platformHalf = Content.Load<Texture2D>("Images/Platforms/PfHalf");

            //Load the door img
            doorImg = Content.Load<Texture2D>("Images/Misc/DoorImg");

            //Initialize the rectangle that will hold the door
            doorBox = new Rectangle((int)doorLoc[gameTemplate - 1].X, (int)doorLoc[gameTemplate - 1].Y, doorImg.Width, doorImg.Height);

            //Load the save sign image
            saveSignImg = Content.Load<Texture2D>("Images/Misc/SaveSign");

            //Initialize the save sign
            saveSignBox = new Rectangle((int)saveSignLoc[gameTemplate - 1].X, (int)saveSignLoc[gameTemplate - 1].Y, saveSignImg.Width, saveSignImg.Height);

            //Load font
            font = Content.Load<SpriteFont>("Font/Font");
        }

        //Pre: None
        //Post: Sounds will be available
        //Desc: Load the songs and sound effects
        private void LoadSounds()
        {
            //Load the background music for different stages of the game
            menuBgMusic = Content.Load<Song>("Audio/Music/MenuMusic");
            lightBgMusic = Content.Load<Song>("Audio/Music/LightGameMusic");
            darkBgMusic = Content.Load<Song>("Audio/Music/DarkGameMusic");

            //Load the different sound effects
            bonusObtSe = Content.Load<SoundEffect>(@"Audio/SoundEffects/BonusObtained");
            bounceSe = Content.Load<SoundEffect>(@"Audio/SoundEffects/BounceSE");
            burnedSe = Content.Load<SoundEffect>(@"Audio/SoundEffects/BurnedSE");
            buttonClkSe = Content.Load<SoundEffect>(@"Audio/SoundEffects/ButtonClickedSE");
            fireSe = Content.Load<SoundEffect>(@"Audio/SoundEffects/FireBreathSE");
            gameOverSe = Content.Load<SoundEffect>(@"Audio/SoundEffects/GameOverSE");
            attackSe = Content.Load<SoundEffect>(@"Audio/SoundEffects/JabSE");
            landingSe = Content.Load<SoundEffect>(@"Audio/SoundEffects/LandingSE");
            gameSuccessSe = Content.Load<SoundEffect>(@"Audio/SoundEffects/SuccessSE");

            //Initialize the sound effect instances
            bonusObtSeInstance = bonusObtSe.CreateInstance();
            bounceSeInstance = bounceSe.CreateInstance();
            burnedSeInstance = burnedSe.CreateInstance();
            buttonClkSeInstance = buttonClkSe.CreateInstance();
            fireSeInstance = fireSe.CreateInstance();
            gameOverSeInstance = gameOverSe.CreateInstance();
            attackSeInstance = attackSe.CreateInstance();
            landingSeInstance = landingSe.CreateInstance();
            gameSuccessSeInstance = gameSuccessSe.CreateInstance();

            //Adjust audio settings
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 1f;
        }

        //Pre: The template number
        //Post: There will be bonuses
        //Desc: Create a new array of bonuses based on the template number
        private void InitBonus(int bonusNum)
        {
            //Initialize the array
            bonus = new ParticleEngine[bonusLv[bonusNum - 1].Length];

            //From 0 to every bonus
            for (int i = 0; i < bonus.Length; i++)
            {
                //Create new particle engine
                bonus[i] = new ParticleEngine(starImg, bonusLv[bonusNum - 1][i], false);
            }
        }

        //Pre: The template number
        //Post: There will be obstacless
        //Desc: Create a new array of obstacles based on the template number
        private void InitObstacles(int obstacleNum)
        {
            //Create a new array for all the factors needed for an obstacle
            obstacleBox = new Rectangle[lvTypeObst[obstacleNum - 1].Length];
            obstacles = new Character[lvTypeObst[obstacleNum - 1].Length];
            fireEmit = new bool[lvTypeObst[obstacleNum - 1].Length];
            motionObst = new int[lvTypeObst[obstacleNum - 1].Length];

            //Create a new array for the particle engines
            particleEngineObst = new ParticleEngine[lvTypeObst[obstacleNum - 1].Length];

            //Create a new array of the original obstacle locations for the moving osbtacles
            origObstLoc = new Vector2[lvTypeObst[obstacleNum - 1].Length];

            //For each obstacle
            for (int a = 0; a < lvTypeObst[obstacleNum - 1].Length; a++)
            {
                //Set whether the obstacle will emit fire to a general array
                fireEmit[a] = isFireEmmit[obstacleNum - 1][a];

                //Set where the obstacle will move
                motionObst[a] = motionEnd[obstacleNum - 1][a];

                //Set the origninal obstacle
                origObstLoc[a] = lvTypeObst[obstacleNum - 1][a];

                //If it is a fire emmiting obstacle
                //It is
                if (isFireEmmit[obstacleNum - 1][a] == true)
                {
                    //Load images
                    obstacles[a] = new Character(headImg, headMaxFrame, headMaxFrame, new Vector2(lvTypeObst[obstacleNum - 1][a].X - 15, lvTypeObst[obstacleNum - 1][a].Y), true);
                    obstacleBox[a] = new Rectangle((int)lvTypeObst[obstacleNum - 1][a].X, (int)lvTypeObst[obstacleNum - 1][a].Y, bodyImg.Width, bodyImg.Height);

                    //Initialize the particle engine
                    particleEngineObst[a] = new ParticleEngine(fireImg, lvTypeObst[obstacleNum - 1][a], true);
                }
                //It is not
                else
                {
                    //Load Images
                    obstacles[a] = new Character(blobImg, blobMaxFrame, blobMaxFrame, lvTypeObst[obstacleNum - 1][a], true);
                }
            }
        }

        //Pre: The template number
        //Post: There will be platforms
        //Desc: Create a new array of platforms based on the template number
        private void InitPlatforms(int platformNum)
        {
            //Create a new array of rectangles for the platforms
            platformBox = new Rectangle[lvType[platformNum - 1].Length];

            //If it is the template that has a moving platform
            //It is
            if (platformNum == 1 || platformNum == 4)
            {
                //For every platform
                for (int i = 0; i < lvType[platformNum - 1].Length; i++)
                {
                    //If it is the last platform
                    //It is
                    if (i == lvType[platformNum - 1].Length - 1)
                    {
                        //Initialize the rectangle box
                        platformBox[i] = new Rectangle((int)lvType[platformNum - 1][i].X, (int)lvType[platformNum - 1][i].Y, platformHalf.Width, platformHalf.Height);
                    }
                    //It is not
                    else
                    {
                        //Initialize the rectangle box
                        platformBox[i] = new Rectangle((int)lvType[platformNum - 1][i].X, (int)lvType[platformNum - 1][i].Y, platformFull.Width, platformFull.Height);
                    }
                }
            }
            //It is not
            else
            {
                //For every platform
                for (int i = 0; i < lvType[platformNum - 1].Length; i++)
                {
                    //Initialize the rectangle
                    platformBox[i] = new Rectangle((int)lvType[platformNum - 1][i].X, (int)lvType[platformNum - 1][i].Y, platformFull.Width, platformFull.Height);
                }
            }
        }

        //Pre: None
        //Post: Character will be animated
        //Desc: Change the location of the character based on their actions and location
        private void CharAnimate()
        {
            //Which action
            //Running
            if (charActions == 1)
            {
                //Update screen animate rate
                screenAnimRate = 5;

                //Animate the character
                CharActionsAnim();
            }
            //Jumping
            else if (charActions == 2)
            {
                //Set up trajectory
                traj = traj + gravity;
                scaledTraj = traj * scale;

                //Update the rate at which the screen is going to be animated by
                screenAnimRate = (int)scaledTraj.X;

                //If the character is on the outside forest
                //It is
                if (outerForest == true)
                {
                    //Which box
                    //Box 1
                    if (lightBox1.X <= 100 && lightBox3.X >= lightBox1.Width * 2)
                    {
                        //Location of the character
                        //Beyond 0
                        if (charAction.charBox.X > 0)
                        {
                            //Location of character
                            //Near the end of the screen
                            if (charAction.charBox.X + charAction.charBox.Width >= screenWidth - 250)
                            {
                                //If the character is facing the right
                                //It is not
                                if (charRight != true)
                                {
                                    //The character is jumping to the left without screen movement
                                    CharMovement(charRight, true, false, false);
                                }
                                //It is
                                else
                                {
                                    //The character is jumpin to the right with screen movement
                                    CharMovement(charRight, true, true, false);
                                }
                            }
                            //Near the beginning of the screen
                            else if (charAction.charBox.X + charAction.charBox.Width <= screenWidth - 250 && charAction.charBox.X > 0)
                            {
                                //The character is jumping without screen movement
                                CharMovement(charRight, true, false, false);
                            }
                            //Near the middle of the screen
                            else
                            {
                                //If the character is facing the right
                                //It is
                                if (charRight == true)
                                {
                                    //The character is jumping to the right without screen movement
                                    CharMovement(charRight, true, false, false);
                                }
                                //It is not
                                else
                                {
                                    //The character is only jumping up and down 
                                    CharMovement(charRight, true, false, true);
                                }
                            }
                        }
                        //At 0
                        else if (charAction.charBox.X <= 0)
                        {
                            //If the character is facing the right
                            //It is
                            if (charRight == true)
                            {
                                //The character is jumping to the right without screen movement
                                CharMovement(charRight, true, false, false);
                            }
                            //It is not
                            else
                            {
                                //The character is only jumping up and down 
                                CharMovement(charRight, true, false, true);
                            }

                        }
                    }
                    //Box 3
                    else if (lightBox3.X + lightBox3.Width >= screenWidth && lightBox2.X <= 0 && lightBox3.X + lightBox3.Width <= screenWidth + 10)
                    {
                        //Loaction of the character
                        //Beginnig of screen
                        if (charAction.charBox.X <= 250)
                        {
                            //If the character is facing the right
                            //It is
                            if (charRight == true)
                            {
                                //The character is jumping to the right without screen movement
                                CharMovement(charRight, true, false, false);
                            }
                            //It is not
                            else
                            {
                                //The character is jumpin to the right with screen movement
                                CharMovement(charRight, true, true, false);
                            }
                        }
                        //Middle of screen
                        else if (charAction.charBox.X + charAction.charBox.Width < screenWidth && charAction.charBox.X > 250)
                        {
                            //The character is jumping without screen movement
                            CharMovement(charRight, true, false, false);
                        }
                        //End of screen
                        else
                        {
                            //The character is jumping to the left without screen movement
                            CharMovement(false, true, false, false);
                        }
                    }
                    //Box 2
                    else
                    {
                        //Location of char
                        //Beginning
                        if (charAction.charBox.X <= 250)
                        {
                            //If the character is facing the right
                            //It is
                            if (charRight == true)
                            {
                                //The character is jumping to the right without screen movement
                                CharMovement(charRight, true, false, false);
                            }
                            //It is not
                            else
                            {
                                //If the character is at 0
                                //It is not
                                if (charAction.charBox.X != 0)
                                {
                                    //The character is jumpin to the right with screen movement
                                    CharMovement(true, true, true, false);
                                }
                                //It is
                                else if (charAction.charBox.X <= 0)
                                {
                                    //The character is jumpin to the left with screen movement
                                    CharMovement(charRight, true, true, false);
                                }
                            }
                        }
                        //Middle
                        else if (charAction.charBox.X + charAction.charBox.Width >= screenWidth - 250)
                        {
                            //If the character is facing the right
                            //It is not
                            if (charRight != true)
                            {
                                //The character is jumping to the right without screen movement
                                CharMovement(charRight, true, false, false);
                            }
                            //It is
                            else
                            {
                                //The character is jumpin to the right with screen movement
                                CharMovement(charRight, true, true, false);
                            }
                        }
                        //End
                        else
                        {
                            //The character is jumping without screen movement
                            CharMovement(charRight, true, false, false);
                        }
                    }
                }
                //The character is within the forest
                else
                {
                    //Which box
                    //Box 1
                    if (darkBox1.X <= 0 && darkBox1.X >= -10)
                    {
                        //Location of char
                        //After 0
                        if (charAction.charBox.X > 0)
                        {
                            //Where on the screen
                            //End of screen
                            if (charAction.charBox.X + charAction.charBox.Width >= screenWidth - 250)
                            {
                                //If the character is facing the right
                                //It is not
                                if (charRight != true)
                                {
                                    //The character is jumping to the left without screen movement
                                    CharMovement(charRight, true, false, false);
                                }
                                //It is
                                else
                                {
                                    //The character is jumpin to the right with screen movement
                                    CharMovement(charRight, true, true, false);
                                }
                            }
                            //Middle
                            else if (charAction.charBox.X + charAction.charBox.Width <= screenWidth - 250 && charAction.charBox.X > 0)
                            {
                                //The character is jumping to the right without screen movement
                                CharMovement(charRight, true, false, false);
                            }
                        }
                        //It 0
                        else if (charAction.charBox.X <= 0)
                        {
                            //If the character is facing the right
                            //It is 
                            if (charRight == true)
                            {
                                //The character is jumping to the right without screen movement
                                CharMovement(charRight, true, false, false);
                            }
                            //It is not
                            else
                            {
                                //The character is only jumping up and down 
                                CharMovement(charRight, true, false, true);
                            }

                        }
                    }
                    //Box 2
                    else if (darkBox2.X + darkBox2.Width >= screenWidth && darkBox2.X < -179)
                    {
                        //Char location
                        //Beginnning
                        if (charAction.charBox.X <= 250)
                        {
                            //If the character is facing the right
                            //It is 
                            if (charRight == true)
                            {
                                //The character is jumping to the right without screen movement
                                CharMovement(charRight, true, false, false);
                            }
                            //It is not
                            else
                            {
                                //The character is jumpin to the right with screen movement
                                CharMovement(charRight, true, true, false);
                            }
                        }
                        //Middle
                        else if (charAction.charBox.X + charAction.charBox.Width < screenWidth && charAction.charBox.X > 250)
                        {
                            //The character is jumping without screen movement
                            CharMovement(charRight, true, false, false);
                        }
                        //End
                        else
                        {
                            //The character is only jumping up and down 
                            CharMovement(charRight, true, false, true);
                        }
                    }
                    //Box 3
                    else
                    {
                        //Location of character
                        //Beggining
                        if (charAction.charBox.X <= 250)
                        {
                            //If the character is facing the right
                            //It is 
                            if (charRight == true)
                            {
                                //The character is jumping to the right without screen movement
                                CharMovement(charRight, true, false, false);
                            }
                            //It is not
                            else
                            {
                                if (charAction.charBox.X != 0)
                                {
                                    //The character is jumping to the right with screen movement
                                    CharMovement(true, true, true, false);
                                }

                                else if (charAction.charBox.X <= 0)
                                {

                                    //The character is jumpin to the left with screen movement
                                    CharMovement(charRight, true, true, false);
                                }
                            }
                        }
                        //End
                        else if (charAction.charBox.X + charAction.charBox.Width > screenWidth - 250)
                        {
                            //If the character is facing the right
                            //It is not
                            if (charRight != true)
                            {
                                //The character is jumping to the left without screen movement
                                CharMovement(charRight, true, false, false);
                            }
                            //It is
                            else
                            {
                                //The character is jumpin to the right with screen movement
                                CharMovement(charRight, true, true, false);
                            }
                        }
                        //Middle
                        else
                        {
                            //The character is jumping without screen movement
                            CharMovement(charRight, true, false, false);
                        }
                    }
                }

                //If the character's y value exceeds the original
                //It does
                if (charAction.charBox.Y >= origLocation.Y)
                {
                    //Reset the trajectory
                    ResetTraj();

                    //Play the sound of a bounce
                    PlaySoundEffects(landingSeInstance);

                }
            }
            //Attacking
            else if (charActions == 3)
            {
                //Update the rate at which the screen is animated
                screenAnimRate = 5;

                //Animate the character
                CharActionsAnim();

                //If it reaches the last frame
                //It does
                if (Math.Round(charAction.frameNum) >= charAction.maxFrame)
                {
                    //Character has finished attacking and will now stand
                    charActions = STAND;
                    CharActionsLoadText();
                }
            }
        }

        //Pre: whether the character will go towards the right, is jumping, will the sceen move, and is the character at the end of the screen
        //Post: The character will know how to move
        //Desc: Based on the pre variables, the character will move a certain way
        private void CharMovement(bool right, bool jump, bool screenMotion, bool end)
        {
            //Character's actions
            //Jumping
            if (jump == true)
            {
                //If the screen is moving
                //It is
                if (screenMotion == true)
                {
                    //If the character is facing the right
                    //It is
                    if (right == true)
                    {
                        //Update the character's location
                        characterLocation.Y += (int)scaledTraj.Y * 2;
                        charAction.charBox.Y += (int)scaledTraj.Y * 2;
                    }
                    //It isn't
                    else
                    {
                        //Update the character's location
                        characterLocation.Y -= (int)scaledTraj.Y * 2;
                        charAction.charBox.Y -= (int)scaledTraj.Y * 2;
                    }

                    //Update the lcoation of objects to match the character's pace
                    BackgroundAnim(screenAnimRate);
                    PlatformsAnim(screenAnimRate);
                    AnimateObstacles(screenAnimRate);
                    AnimateBonus(screenAnimRate);
                }
                //It is not
                else
                {
                    //Update the y location of the character
                    characterLocation.Y += (int)scaledTraj.Y * 2;
                    charAction.charBox.Y += (int)scaledTraj.Y * 2;

                    //If the character is at the edge of the screen
                    //It isnt
                    if (end != true)
                    {
                        //If the character is facing the right
                        //It is
                        if (right == true)
                        {
                            //Update the x location of the character
                            characterLocation.X += (int)scaledTraj.X;
                            charAction.charBox.X += (int)scaledTraj.X;
                        }
                        //It is not
                        else
                        {
                            //Update the x location of the character
                            characterLocation.X -= (int)scaledTraj.X;
                            charAction.charBox.X -= (int)scaledTraj.X;
                        }
                    }
                }
            }
            //Running
            else
            {
                //IF the screen is moving
                //It is
                if (screenMotion == true)
                {
                    BackgroundAnim(screenAnimRate);
                    PlatformsAnim(screenAnimRate);
                    AnimateObstacles(screenAnimRate);
                    AnimateBonus(screenAnimRate);
                }
                //It is not
                else
                {
                    //If the character is facing the right
                    //It is
                    if (right == true)
                    {
                        characterLocation.X += 5;
                        charAction.charBox.X += 5;
                    }
                    //It isnt
                    else
                    {
                        characterLocation.X -= 5;
                        charAction.charBox.X -= 5;
                    }
                }
            }
        }

        //Pre: A rectangle
        //Post: Whether it is true or not that the character has collided with the box
        //Desc: Detect whether it is impossible for a collision to ocur. If not, then a collision has occured
        private bool CollisionDetection(Rectangle objectBox)
        {
            //Store whether they have collied
            bool collision = false;

            //If the character has collided with the box
            //The character has collided with teh box
            if (!(charAction.charBox.Right - 10 < objectBox.X || charAction.charBox.X + 10 > objectBox.Right || charAction.charBox.Bottom < objectBox.Y || charAction.charBox.Y + 10 > objectBox.Bottom))
            {
                //Update collision
                collision = true;
            }

            //Return collsion
            return collision;
        }

        //Pre: Which platform has it collided with
        //Post: The character's location will change
        //Desc: Detect how has the character collided with the platform. From the top, bottom or sides, and then act accordingly
        private void PlatformCollision(int collidedPf)
        {
            //Where has the character collided with the platform
            //The top of the platform
            if (charAction.charBox.Y + charAction.charBox.Height < platformBox[collidedPf].Y + 10)
            {
                //If the character is jumping
                if (charActions == JUMP)
                {
                    //Reset the trajectory and the character is now standing
                    ResetTraj();
                }

                //Update the y value of the character so that it is properly standing on the platform
                charAction.charBox.Y = platformBox[collidedPf].Y - charAction.charBox.Height;
                characterLocation.Y = platformBox[collidedPf].Y - charAction.charBox.Height;

                //Update the current platform
                curPlatform = collidedPf;

                //Update the original location
                origLocation.Y = charAction.charBox.Y;

                //Character is not falling
                charFall = false;

                //Play the sound of a landing
                PlaySoundEffects(landingSeInstance);

                //Which templat
                //1 or 4
                if (gameTemplate == 1 || gameTemplate == 4)
                {
                    //If the character has collided with a moving platform
                    //It has
                    if (collidedPf == platformBox.Length - 1)
                    {
                        //It is moving with the platform
                        movingObst = true;
                    }
                    //It has not
                    else
                    {
                        //It is not moving with the platform
                        movingObst = false;
                    }
                }
            }

            //The character is colliding with the side of the box
            else if (!(charAction.charBox.Y + charAction.charBox.Height <= platformBox[collidedPf].Y + 10) && ((charAction.charBox.X + charAction.charBox.Width <= platformBox[collidedPf].X + 5 || charAction.charBox.X >= (platformBox[collidedPf].X + platformBox[collidedPf].Width - 5))))
            {
                //If the character ahs run into the platform
                //IT has
                if (charActions == RUN)
                {
                    //If the character has collided with the left side of the platform
                    //IT has
                    if (charAction.charBox.X + charAction.charBox.Width <= platformBox[collidedPf].X + 10)
                    {
                        //Decrease the x value of the character's location so that it is not moving
                        characterLocation.X -= 5;
                        charAction.charBox.X -= 5;
                    }
                    //It has not, instead it has collided with the right side
                    else if (charAction.charBox.X >= (platformBox[collidedPf].X + platformBox[collidedPf].Width - 10))
                    {
                        //Increase the x value of the character's location so that it is not moving
                        characterLocation.X += 5;
                        charAction.charBox.X += 5;
                    }
                }
                //It has not
                else
                {
                    //The character will fall down
                    charFall = true;
                }
            }
            //The character is colliding with underneath the box
            else if (charAction.charBox.Y >= platformBox[collidedPf].Y + platformBox[collidedPf].Height - 20)
            {
                //If it is one of the game templates that has a half platform
                //It is
                if (gameTemplate == 1 || gameTemplate == 4)
                {
                    //If the current platform a moving platform
                    //It is
                    if (curPlatform == platformBox.Length - 1)
                    {
                        //If the character is also coliding with the moving platform
                        //It is
                        if (CollisionDetection(platformBox[curPlatform]) == true)
                        {
                            //The user has died
                            gameState = GAMEOVER;
                        }
                    }
                }

                //The character is falling
                charFall = true;

                //The character is standing ad update the values
                ResetTraj();
            }
        }

        //Pre: None
        //Post: Whether the character will fall
        //Desc: Detect whether the character has fallen of a platform or whether they have hit the bottom of a platform
        private void PlatformFall()
        {
            //Location of the character
            //The character is not jumpping and is not on the platform
            if (CollisionDetection(platformBox[curPlatform]) == false && charActions != JUMP && firstLanded != true)
            {
                //Character is falling
                charFall = true;
            }
            //If the character has jumped and hit the bottom of a platform
            else if (CollisionDetection(platformBox[curPlatform]) == false && charAction.charBox.Y > platformBox[curPlatform].Y && firstLanded != true)
            {
                //Character is falling
                charFall = true;
            }
        }

        //Pre: None
        //Post: Character will be animated
        //Desc: Change the location of the character based on their location
        private void CharActionsAnim()
        {
            //If the character is on the outside forest
            //It is
            if (outerForest == true)
            {
                //Which box
                //Box 1
                if (lightBox1.X >= -20 && lightBox1.X <= 0)
                {
                    //Location of the box
                    //The box is not at the beginning
                    if (charAction.charBox.X > 0)
                    {
                        //Location of the character
                        //The character is at the end
                        if (charAction.charBox.X + charAction.charBox.Width >= screenWidth - 250)
                        {
                            //If the character is going right
                            //It is not
                            if (charRight != true)
                            {
                                //The character is moving to the left
                                CharMovement(charRight, false, false, false);
                            }
                            //It is
                            else
                            {
                                //The screen will move
                                CharMovement(charRight, false, true, false);
                            }
                        }
                        //The character not at the end
                        else
                        {
                            //The character is moving
                            CharMovement(charRight, false, false, false);
                        }
                    }
                    //The box is at the beginning
                    else if (charAction.charBox.X <= 0)
                    {
                        if (charRight == true)
                        {
                            //The character is moving to the right
                            CharMovement(charRight, false, false, false);
                        }
                    }
                }
                //Box 3
                else if (lightBox3.X + lightBox3.Width >= screenWidth && lightBox2.X <= 0 && lightBox3.X + lightBox3.Width <= screenWidth + 10)
                {
                    //Location of the character
                    //The character is at te beginning
                    if (charAction.charBox.X <= 250)
                    {
                        //Where is the character facing
                        //The character is facing right
                        if (charRight == true)
                        {
                            //The character is moving to the right
                            CharMovement(charRight, false, false, false);
                        }
                        //The character is facing left
                        else
                        {
                            //The screen will move
                            CharMovement(charRight, false, true, false);
                        }
                    }
                    //The character is in the middle
                    else if (charAction.charBox.X + charAction.charBox.Width < screenWidth && charAction.charBox.X > 250)
                    {
                        //The character is moving
                        CharMovement(charRight, false, false, false);
                    }
                    //The character is at the end
                    else
                    {
                        //If the character is facing left
                        //It is facing left
                        if (charRight != true)
                        {
                            //The character is moving to the left
                            CharMovement(charRight, false, false, false);
                        }
                    }
                }
                //Box 2
                else
                {
                    //the character is located near the beginning
                    if (charAction.charBox.X <= 250)
                    {
                        //Which direction is the character facing
                        //Right
                        if (charRight == true)
                        {
                            //The character is moving to the right
                            CharMovement(charRight, false, false, false);
                        }
                        //Left
                        else
                        {
                            //The screen will move
                            CharMovement(charRight, false, true, false);
                        }
                    }
                    //the character is near the end of the screen
                    else if (charAction.charBox.X + charAction.charBox.Width >= screenWidth - 250)
                    {
                        //Which direction is the character facing
                        //Left
                        if (charRight != true)
                        {
                            //The character is moving to the left
                            CharMovement(charRight, false, false, false);
                        }
                        //Right
                        else
                        {
                            //The screen will move
                            CharMovement(charRight, false, true, false);
                        }
                    }
                    //The character is at the middle of the screen
                    else
                    {
                        //The character is moving
                        CharMovement(charRight, false, false, false);
                    }
                }
            }
            //The character is within the forest
            else
            {
                //Location of the rectangle
                //The rectangle is located at the beginning
                if (darkBox1.X <= 0 && darkBox1.X >= -10)
                {
                    //Location of the character
                    //the character is not at the beginning
                    if (charAction.charBox.X > 0)
                    {
                        //If the character has reach the max
                        //It has
                        if (charAction.charBox.X + charAction.charBox.Width >= screenWidth - 250)
                        {
                            //Which direction is the character facign
                            //The character is facing left
                            if (charRight != true)
                            {
                                //The character is moving to the left
                                CharMovement(charRight, false, false, false);
                            }
                            //The character is facing right
                            else
                            {
                                //The screen will move
                                CharMovement(charRight, false, true, false);
                            }
                        }
                        //It has not
                        else
                        {
                            //The character is moving
                            CharMovement(charRight, false, false, false);
                        }
                    }
                    //the character is at the beginning
                    else if (charAction.charBox.X <= 0)
                    {
                        //If the character is facign right
                        //IT is
                        if (charRight == true)
                        {
                            //The character is moving to the right
                            CharMovement(charRight, false, false, false);
                        }
                    }
                }
                //The rectangle is at the middle
                else if (darkBox2.X + darkBox2.Width >= screenWidth && darkBox2.X < -179)
                {
                    //Location fo the character
                    //the character is before 250
                    if (charAction.charBox.X <= 250)
                    {
                        //Which direction is the character moving
                        //Right
                        if (charRight == true)
                        {
                            //The character is moving to the right
                            CharMovement(charRight, false, false, false);
                        }
                        //Left
                        else
                        {
                            //The screen will move
                            CharMovement(charRight, false, true, false);
                        }
                    }
                    //the character is beyond 250 but has not reached the screen width
                    else if (charAction.charBox.X + charAction.charBox.Width < screenWidth && charAction.charBox.X > 250)
                    {
                        //The character is moving
                        CharMovement(charRight, false, false, false);
                    }
                    //The character has reached the screen width
                    else
                    {
                        //If the character is facing left
                        //It is
                        if (charRight != true)
                        {
                            //The character is moving to the left
                            CharMovement(charRight, false, false, false);
                        }
                    }
                }
                //The rectangle has reached the end
                else
                {
                    //Location of the character
                    //The character is near the beginning
                    if (charAction.charBox.X <= 250)
                    {
                        //Which direction is the character moving
                        //Right
                        if (charRight == true)
                        {
                            //The character is moving to the right
                            CharMovement(charRight, false, false, false);
                        }
                        //Left
                        else
                        {
                            //The screen will move
                            CharMovement(charRight, false, true, false);
                        }
                    }
                    //The character is near the end of the screen
                    else if (charAction.charBox.X + charAction.charBox.Width >= screenWidth - 250)
                    {
                        //Which direction is the character moving
                        //Left
                        if (charRight != true)
                        {
                            //The character is moving to the left
                            CharMovement(charRight, false, false, false);
                        }
                        //Right
                        else
                        {
                            //The screen will move
                            CharMovement(charRight, false, true, false);
                        }
                    }
                    //The character is at the middle
                    else
                    {
                        //The character is moving
                        CharMovement(charRight, false, false, false);
                    }
                }
            }
        }

        //Pre: The template number
        //Post: Platforms will be drawn
        //Desc: Detect whether there is a half platform and draw accordingly
        private void DrawPlatforms(int platformNum)
        {
            //Which template of platforms
            //It is the first or the fourth
            if (platformNum == 0 || platformNum == 3)
            {
                //From one to the number of platforms
                for (int i = 0; i < lvType[platformNum].Length; i++)
                {
                    //If it is the last platform
                    //It is
                    if (i == lvType[platformNum].Length - 1)
                    {
                        //Draw the half platform 
                        spriteBatch.Draw(platformHalf, platformBox[i], Color.White);
                    }
                    //It is not
                    else
                    {
                        //Draw the platform
                        spriteBatch.Draw(platformFull, platformBox[i], Color.White);
                    }
                }
            }
            //It is the others
            else
            {
                //From one to the number of platforms
                for (int i = 0; i < lvType[platformNum].Length; i++)
                {
                    //Draw the platform
                    spriteBatch.Draw(platformFull, platformBox[i], Color.White);
                }
            }

            //Draw the doors
            spriteBatch.Draw(doorImg, doorBox, Color.White);

            //Draw the save sign
            spriteBatch.Draw(saveSignImg, saveSignBox, Color.White);
        }

        //Pre: The template number
        //Post: The half platform will move
        //Desc: Detect whether there is a moving platform, and animate it
        private void HalfPlatformAnim(int platformNum)
        {
            //Is it one of the type of platform templates that has a mobile platform
            //It is
            if (platformNum == 1 || platformNum == 4)
            {
                //If the platform is moving down
                //It is moving down
                if (pfHalfDown == true)
                {
                    //Increase the platform's y value
                    platformBox[platformBox.Length - 1].Y += 3;
                }
                //It is moving up
                else if (pfHalfDown != true)
                {
                    //Decrease the platform's y value
                    platformBox[platformBox.Length - 1].Y -= 3;
                }

                //If it is the first template
                //It is 
                if (platformNum == 1)
                {
                    //If the platform's Y value is larger or equal to the 500 y value
                    //It is
                    if (platformBox[platformBox.Length - 1].Y >= 500)
                    {
                        //Platform now needs to move up
                        pfHalfDown = false;
                    }
                    //It is not
                    else if (platformBox[platformBox.Length - 1].Y <= lvType[platformNum][platformBox.Length - 1].Y)
                    {
                        //Platform now needs to move down
                        pfHalfDown = true;
                    }
                }
                //It is not
                else
                {
                    //If the platform's y valyue has reached 400
                    //It has
                    if (platformBox[platformBox.Length - 1].Y >= 400)
                    {
                        //Platform needs to move up
                        pfHalfDown = false;
                    }
                    //It has not
                    else if (platformBox[platformBox.Length - 1].Y <= lvType[platformNum - 1][platformBox.Length - 1].Y)
                    {
                        //Platform needs to move up
                        pfHalfDown = true;
                    }
                }
            }
        }

        //Pre: None
        //Post: The character will fall
        //Desc: Increase its y valye
        private void CharacterFall()
        {
            //If the character is jumping
            //It is
            if (charActions == JUMP)
            {
                //Reset the trajectory and character is tanding
                ResetTraj();
            }

            //For every platform
            for (int i = 0; i < platformBox.Length; i++)
            {
                //If there is no collision with the platform
                //There is no collision
                if (CollisionDetection(platformBox[i]) != true)
                {
                    //Increase the character's Y value by 1
                    charAction.charBox.Y += 1;
                    characterLocation.Y += 1;
                }
                //There is
                else if (charAction.charBox.Y + charAction.charBox.Height < platformBox[i].Y + 10)
                {
                    //Set the character's Y value so that the character's y value is equivalent to the platform's y value decrease by tha character's height
                    charAction.charBox.Y = platformBox[i].Y - charAction.charBox.Height;
                    characterLocation.Y = platformBox[i].Y - charAction.charBox.Height;

                    //Charcter is no longer falling
                    charFall = false;

                    //Exit for loop
                    break;
                }

            }
        }

        //Pre: None
        //Post: Obstacles will be drawn
        //Desc: Detect which obstacle it is and draw all the obstacles in the array accordingly
        private void DrawObstacles()
        {
            //For each obstacle
            for (int i = 0; i < fireEmit.Length; i++)
            {
                //If there is an obstacle
                //There is
                if (obstacles[i] != null)
                {
                    //Draw the obstacle
                    obstacles[i].Draw(spriteBatch);

                    //If it is a fire emmiting obstacle
                    //It is
                    if (fireEmit[i] == true)
                    {
                        //Draw the body and fire
                        spriteBatch.Draw(bodyImg, obstacleBox[i], Color.White);
                        particleEngineObst[i].Draw(spriteBatch);
                    }
                }
            }
        }

        //Pre: None
        //Post: Obstacles will be animated
        //Desc:  Update obstacles and detect which obstacle it is, and update its location or fireemmiting process accordingly
        private void ObstaclesMotion()
        {
            //For every obstacle
            for (int i = 0; i < obstacles.Length; i++)
            {
                //If there is an obstacle
                //There is
                if (obstacles[i] != null)
                {
                    //Update teh obstacle
                    obstacles[i].Update();

                    //If it is a fire emmiting obstacle
                    //It is
                    if (fireEmit[i] == true)
                    {
                        //If the character has its mouth open
                        //It does
                        if (obstacles[i].frameNum > 2 && obstacles[i].frameNum < 4)
                        {
                            //Update the particle engine
                            particleEngineObst[i].Update();

                            //If it is between the frames 2 and 3
                            //It is
                            if (obstacles[i].frameNum >= 2 || obstacles[i].frameNum <= 3)
                            {
                                //Play the sound of fire
                                PlaySoundEffects(fireSeInstance);
                            }
                        }
                        //It does not
                        else
                        {
                            //New particle engine
                            particleEngineObst[i] = new ParticleEngine(fireImg, new Vector2(obstacles[i].charBox.X, obstacles[i].charBox.Y), true);
                        }
                    }
                    //It is a blob
                    else
                    {
                        //If the obstacle has reached it's goal
                        //It has 
                        if (obstacles[i].charBox.X >= motionObst[i])
                        {
                            //Return by going right
                            obstRight = false;
                        }
                        //It has not
                        else if (obstacles[i].charBox.X <= origObstLoc[i].X)
                        {
                            //Go towards right
                            obstRight = true;
                        }

                        //Which direction is the obstacle facing
                        //Right
                        if (obstRight == true)
                        {
                            //Update the obstacle location and effects
                            obstacles[i].spriteEffect = SpriteEffects.FlipHorizontally;
                            obstacles[i].charBox.X += 1;
                        }
                        //Left
                        else
                        {
                            //Update the obstacle location and effects
                            obstacles[i].spriteEffect = SpriteEffects.None;
                            obstacles[i].charBox.X -= 1;
                        }
                    }

                }
            }
        }

        //Pre: None
        //Post: The character will die, gain points, or stand on the statue
        //Desc: Detect what the character has collided with and how
        private void ObstacleCollision()
        {
            //For each obstacle
            for (int i = 0; i < obstacles.Length; i++)
            {
                //If it is a fire emmitting obstacle
                //It is
                if (fireEmit[i] == true)
                {
                    //If the character has collided with the fire emmiting obstacle
                    //It has
                    if (CollisionDetection(obstacles[i].charBox) == true || CollisionDetection(obstacleBox[i]) == true)
                    {
                        //If the character has landed above the obstacle
                        //It has
                        if (charAction.charBox.Y + charAction.charBox.Height < obstacles[i].charBox.Y + 10)
                        {
                            //Set that the character is not falling
                            charFall = false;

                            //If it is the first time the obstacle has collided with the obstacle
                            //It is
                            if (firstLanded == false)
                            {

                                //Set the character's Y location so that it seems to be standing on th eobstacle
                                characterLocation.Y = obstacles[i].charBox.Y - charAction.charBox.Height;
                                charAction.charBox.Y = obstacles[i].charBox.Y - charAction.charBox.Height;

                                //Update the y value of the original value
                                origLocation.Y = charAction.charBox.Y;

                                //Character is standing
                                charActions = STAND;

                                //Reset jump
                                ResetTraj();

                                //Load proper char images
                                CharActionsLoadText();

                                //Set that the character has already landed on the obstacle
                                firstLanded = true;

                                //Set which obstacle the user has collided with
                                collidedObst = i;

                                //For every platform
                                for (int x = 0; x < platformBox.Length; x++)
                                {
                                    //If the gargoyle's body is colliding with the platform
                                    //It is
                                    if (!(obstacleBox[i].Right - 10 < platformBox[x].X || obstacleBox[i].X + 10 > platformBox[x].Right || obstacleBox[i].Bottom < platformBox[x].Y || obstacleBox[i].Y + 10 > platformBox[x].Bottom))
                                    {
                                        //Set the current platform
                                        curPlatform = x;

                                        //Update location
                                        origLocation.Y = platformBox[x].Y - charAction.charBox.Height;

                                        //Exit for loop
                                        break;
                                    }
                                }
                            }
                        }
                        //It has not
                        else
                        {
                            //Which side has the character collided with 
                            //Left
                            if (charAction.charBox.X < obstacles[i].charBox.X)
                            {
                                //Prevent the character from moving forwards
                                CharObstCol(true, i);
                            }
                            //Right
                            else if (charAction.charBox.X > obstacles[i].charBox.X)
                            {
                                //Prevent the character from moving backwards
                                CharObstCol(false, i);
                            }
                        }
                    }
                    //For every particle
                    for (int x = 0; x < particleEngineObst[i].particleLoc.Count; x++)
                    {
                        //If there is collision with a particle
                        //There is
                        if (CollisionDetection(new Rectangle((int)particleEngineObst[i].particleLoc[x].X, (int)particleEngineObst[i].particleLoc[x].Y, fireImg.Width, fireImg.Height)) == true)
                        {
                            //Play the sound of the character being burned
                            PlaySoundEffects(burnedSeInstance);

                            //Player has died
                            gameState = GAMEOVER;
                        }
                    }
                }
                //It is not a fire emitting obstacle
                else
                {
                    //If there is an obstacle
                    //There is
                    if (obstacles[i] != null)
                    {
                        //If the character has collided with an obstacle
                        //It has
                        if (CollisionDetection(obstacles[i].charBox) == true)
                        {
                            //If the character is attacking
                            //It is
                            if (charActions == ATTACK)
                            {
                                //Remove the obstacle
                                obstacles[i] = null;

                                //Add score bonus
                                second -= 5;
                            }
                            //If it is not
                            else
                            {
                                //Player has died
                                gameState = GAMEOVER;
                            }
                        }
                    }
                }
            }

            //If the character has landed on the gargoyle
            //It has
            if (firstLanded == true && collidedObst != -1)
            {
                //Whether the character is away from the gargoyle
                //It is
                if (CollisionDetection(new Rectangle(obstacles[collidedObst].charBox.X - 20, obstacles[collidedObst].charBox.Y - 20, obstacles[collidedObst].charBox.Width + 40, obstacles[collidedObst].charBox.Height + 20)) == false && CollisionDetection(new Rectangle(obstacleBox[collidedObst].X - 20, obstacleBox[collidedObst].Y - 20, obstacleBox[collidedObst].Width + 40, obstacleBox[collidedObst].Height + 20)) == false)
                {
                    //Update variables
                    firstLanded = false;
                    collidedObst = -1;
                }
            }
        }

        //Pre: None
        //Post: Bonus will be animate
        //Desc: Update the particle engine class
        private void UpdateBonus()
        {
            //From 0 to every bonus
            for (int i = 0; i < bonus.Length; i++)
            {
                //If there is a bonus
                //There is
                if (bonus[i] != null)
                {
                    //Update the bonus
                    bonus[i].Update();
                }
            }
        }

        //Pre: None
        //Post: Player will get seconds off
        //Desc: Detect for collision with the bonus by attacking
        private void BonusCollision()
        {
            //From 0 to every bonus
            for (int i = 0; i < bonus.Length; i++)
            {
                //If there is a bonus
                //There is
                if (bonus[i] != null)
                {
                    //For every particle
                    for (int x = 0; x < bonus[i].particles.Count; x++)
                    {
                        //If there is a collision
                        //There is
                        if (CollisionDetection(new Rectangle((int)bonus[i].particles[x].position.X, (int)bonus[i].particles[x].position.Y, starImg.Width, starImg.Height)) == true)
                        {
                            //If the character is attacking
                            //IT i
                            if (charActions == ATTACK)
                            {
                                //Remove the bonus
                                bonus[i] = null;

                                //Increase the number of bonuses obtained
                                bonusObt++;

                                //Decrease score by 60
                                second -= 10;

                                //Play the sound of a bounce
                                PlaySoundEffects(bonusObtSeInstance);


                                //Exit for loop
                                break;
                            }
                        }
                    }
                }
            }
        }

        //Pre: None
        //Post: Displays bonus 
        //Desc: Draw each particle
        private void BonusDraw()
        {
            //From 0 to every bonus
            for (int i = 0; i < bonus.Length; i++)
            {
                //If there is a bonus
                //There is
                if (bonus[i] != null)
                {
                    //Draw the bonus
                    bonus[i].Draw(spriteBatch);
                }
            }
        }

        //Pre: How much should it move
        //Post: Background will be animated
        //Desc: Move it accordingly by the moverate and direction
        private void BackgroundAnim(int moveRate)
        {
            //If the character is on the outer forest
            //It is
            if (outerForest == true)
            {
                //If the character is moving right
                //It is
                if (charRight == true)
                {
                    //Decrease the x values of the backgrnd rectangles
                    lightBox1.X -= moveRate;
                    lightBox2.X -= moveRate;
                    lightBox3.X -= moveRate;

                    //Decrease the x values of the door
                    doorBox.X -= moveRate;

                    //Decrease the x value of the save sign
                    saveSignBox.X -= moveRate;
                }
                //It is not
                else
                {
                    //Increase the x value of the backgrnd rectangles
                    lightBox1.X += moveRate;
                    lightBox2.X += moveRate;
                    lightBox3.X += moveRate;

                    //Increase the x values of the door
                    doorBox.X += moveRate;

                    //Increase the x value of the save sign
                    saveSignBox.X += moveRate;
                }
            }
            //It is not
            else
            {
                //If the character is moving right
                //It is
                if (charRight == true)
                {
                    //Decrease the x values of the backgrnd rectangles
                    darkBox1.X -= moveRate;
                    darkBox2.X -= moveRate;

                    //Decrease the x values of the door
                    doorBox.X -= moveRate;

                    //Decrease the x value of the save sign
                    saveSignBox.X -= moveRate;
                }
                //It is not
                else
                {
                    //Increase the x value of the backgrnd rectangles
                    darkBox1.X += moveRate;
                    darkBox2.X += moveRate;

                    //Increase the x values of the door
                    doorBox.X += moveRate;

                    //Increase the x value of the save sign
                    saveSignBox.X += moveRate;
                }
            }
        }

        //Pre: How much should it move
        //Post: Platforms will be animated
        //Desc: Move it accordingly by the moverate and direction
        private void PlatformsAnim(int moveRate)
        {
            //IF the character is facing right
            //It is facign right
            if (charRight == true)
            {
                //For every platform
                for (int i = 0; i < platformBox.Length; i++)
                {
                    //Decrease the x value of the platform 
                    platformBox[i].X -= moveRate;
                }
            }
            //It is not
            else
            {
                //For every platform
                for (int i = 0; i < platformBox.Length; i++)
                {
                    //Increase the x value of the platform
                    platformBox[i].X += moveRate;
                }
            }

        }

        //Pre: How much should it move
        //Post: Bonuses will be animated
        //Desc: Move it accordingly by the moverate and direction
        private void AnimateBonus(int moveRate)
        {
            //From 0 to every bonus
            for (int i = 0; i < bonus.Length; i++)
            {
                //If there is a bonus
                //There is
                if (bonus[i] != null)
                {
                    //If the character is facing right
                    //It is
                    if (charRight == true)
                    {
                        //Increase the emmiter's x value
                        bonus[i].emitterLoc.X -= moveRate;

                        //For every particle
                        for (int x = 0; x < bonus[i].particles.Count; x++)
                        {
                            //Increase the particle's x value to move it right
                            bonus[i].particles[x].position.X -= moveRate;
                        }
                    }
                    //It is not
                    else
                    {
                        //Decrease the emmiter's x value
                        bonus[i].emitterLoc.X += moveRate;

                        //For every particle
                        for (int x = 0; x < bonus[i].particles.Count; x++)
                        {
                            //Decrease the particle's x value to move it right
                            bonus[i].particles[x].position.X += moveRate;
                        }
                    }

                }
            }
        }

        //Pre: How much should it move
        //Post: Obstacles will be animated
        //Desc: Move it accordingly by the moverate and direction 
        private void AnimateObstacles(int moveRate)
        {
            //From 0 to the amount of obstacles
            for (int a = 0; a < obstacles.Length; a++)
            {
                //If there is an obstacle
                //There is
                if (obstacles[a] != null)
                {
                    //If it is the obstacle that emmits fire
                    //It is
                    if (fireEmit[a] == true)
                    {
                        //If the character is going right
                        //It is 
                        if (charRight == true)
                        {
                            //Decrease the X value of both the head and body
                            obstacles[a].charBox.X -= moveRate;
                            obstacleBox[a].X -= moveRate;

                            //Decrease the x value of the particles and particle emitter
                            particleEngineObst[a].emitterLoc.X -= moveRate;

                            //From 0 to the amount of particles
                            for (int i = 0; i < particleEngineObst[a].particles.Count; i++)
                            {
                                //Draw the particle
                                particleEngineObst[a].particles[i].position.X -= moveRate;
                            }
                        }
                        //It is not
                        else
                        {
                            //Increase the X value of both the head and body
                            obstacles[a].charBox.X += moveRate;
                            obstacleBox[a].X += moveRate;

                            //Decrease the x value of the particles and particle emmiter
                            particleEngineObst[a].emitterLoc.X += moveRate;

                            //From 0 to the amount of particles
                            for (int i = 0; i < particleEngineObst[a].particles.Count; i++)
                            {
                                //Draw the particle
                                particleEngineObst[a].particles[i].position.X += moveRate;
                            }
                        }
                    }
                    //It is not
                    else
                    {
                        //If the character is facing right
                        //It is
                        if (charRight == true)
                        {
                            //Decrease the obstacle's x value
                            obstacles[a].charBox.X -= moveRate;

                            //Decrease the final destination's x value
                            motionObst[a] -= moveRate;

                            //Decrease the original x value
                            origObstLoc[a].X -= moveRate;
                        }
                        //It is
                        else
                        {
                            //Increase the obstacle's x value
                            obstacles[a].charBox.X += moveRate;

                            //Increase the final destination's x value
                            motionObst[a] += moveRate;

                            //Increase the original x value
                            origObstLoc[a].X += moveRate;
                        }
                    }
                }
            }
        }

        //Pre: A song
        //Post: Song will be played if there is music
        //Desc: Detect if there is music and play song if there ismusic
        private void PlayMusic(Song song)
        {
            //If the music can play
            //It can
            if (music == true)
            {
                //IF music is not already playing
                //It is not
                if (MediaPlayer.State != MediaState.Playing)
                {
                    //Play music
                    MediaPlayer.Play(song);
                }
            }
            //It cannot
            else
            {
                //IF music is playing
                //It is
                if (MediaPlayer.State == MediaState.Playing)
                {
                    //Stop Music
                    MediaPlayer.Stop();
                }
            }
        }

        //Pre: A song
        //Post: The song will change
        //Desc: Stop the current song then start the new sond
        private void MusicReset(Song song)
        {
            //If there is music
            if (music == true)
            {
                //Set music to false so it stops playing
                music = false;
                PlayMusic(song);

                //Set music to true to resume playing
                music = true;
            }
        }

        //Pre: A sound effect
        //Post: Sound effect will be played if there are sound effects
        //Desc: Detect whether there are sound effects and if so, play sound effect
        private void PlaySoundEffects(SoundEffectInstance soundEffInst)
        {
            //If there are sound effects
            //There are
            if (soundEffects == true)
            {
                //Play the sound effect
                soundEffInst.Play();
            }
        }

        //Pre: None
        //Post: New order of game templates
        //Desc: Generate a new number and detect whether it has been there in the first place, if not, add it to the order. The repeat until full
        private void LvGen()
        {
            //For each level
            for (int x = 0; x < lvlOrder.Length; x++)
            {
                //Generate a number from 1 to 5
                int i = RandomNumGenerator(1, 5);

                //If there is no levels or the number generated is a new number
                //There is
                if (tempLvOrder == null || tempLvOrder.Contains(Convert.ToString(i)) != true)
                {
                    //Convert the number to a string and add it to both the array and string
                    tempLvOrder += Convert.ToString(i);
                    lvlOrder[x] = i;
                }
                //There are no
                else
                {
                    //If there are currently 4 levels
                    //There are
                    if (tempLvOrder.Length == 4)
                    {
                        //Fore each possible level
                        for (int a = 1; a <= 5; a++)
                        {
                            //If it does not already have that level
                            //It does not
                            if (tempLvOrder.Contains(Convert.ToString(a)) == false)
                            {
                                //Add it to the string and array
                                tempLvOrder += Convert.ToString(a);
                                lvlOrder[x] = a;

                                //Break the for loop
                                break;
                            }
                        }
                    }
                    //There is less than 4 levels
                    else
                    {
                        //Decrease the value of x
                        x--;
                    }
                }
            }
        }

        //Pre: None
        //Post: Character will be animated
        //Desc: Detect whether the character is on a moving platform and if so, change the character's y location acordingly
        private void PlatformCharAnim()
        {
            //If the platform is moving down
            //IT is 
            if (pfHalfDown == true)
            {
                //Increase the y location of the character
                charAction.charBox.Y += 3;
                characterLocation.Y += 3;

                //Update the original location 
                origLocation.Y = charAction.charBox.Y;
            }
            //It is not
            else
            {
                //Decrease the y location of the character
                charAction.charBox.Y -= 3;
                characterLocation.Y -= 3;

                //Update the original location 
                origLocation.Y = charAction.charBox.Y;
            }
        }

        //Pre: None
        //Post: New level is generated and riddles are displayed
        //Desc: Detect the level and update gamestate and game if it is not at the end. However regardless of level, they will be taken to the riddle gamestate
        private void DoorCollision()
        {
            //If the user has collided with the door and has pressed shift
            //The user has collided with the door and has pressed shift
            if (CollisionDetection(doorBox) == true && (prevKb.IsKeyDown(Keys.RightShift) != true && kb.IsKeyDown(Keys.RightShift) == true))
            {
                //For every level
                for (int i = 1; i <= lvlOrder.Length; i++)
                {
                    //IF it the current level
                    //It is the current level
                    if (lvlOrder[i - 1] == gameTemplate)
                    {
                        //Increase the level
                        level++;

                        //If the game has reached the end
                        //It has
                        if (i == lvlOrder.Length)
                        {
                            //The game has ended successfully
                            gameState = GAMEEND;

                            //Resets music
                            MusicReset(menuBgMusic);
                        }
                        //It has not
                        else
                        {
                            //Update the game template
                            gameTemplate = lvlOrder[i];

                            //Load the proper things to continue to next level
                            ContNextLvl(lvlOrder[i]);
                        }

                        //Enter riddle stage
                        gameState = RIDDLE;

                        //Exit for loop
                        break;
                    }
                }
            }
        }

        //Pre:The level
        //Post: Next level will be loaded
        //Desc: Music is set accordingly, variables are updated and reset
        private void ContNextLvl(int levelNum)
        {
            //If it is on the first or last level
            //First or last level
            if (level == 1 || level == 5)
            {
                //Outside forest is true
                outerForest = true;

                //Resets music
                MusicReset(lightBgMusic);
            }
            //Not on the first or last level
            else
            {
                //Not on the outside of the forest
                outerForest = false;

                //If it was not already in the dark forest
                //It was not
                if (outerForest == false)
                {
                    //Resets music
                    MusicReset(darkBgMusic);
                }
            }

            //Reset game to fit new level
            InitBonus(levelNum);
            InitObstacles(levelNum);
            InitPlatforms(levelNum);

            //Reset
            Reset();
        }

        //Pre: None
        //Post: A new level can begin
        //Desc: Background, signs, character location, and current platfrom is reset
        private void Reset()
        {
            //Reset the rectangles that will hold the images for the outer  part of the forest
            lightBox1.X = 0;
            lightBox2.X = lightBox1.X + lightImg.Width;
            lightBox3.X = lightBox2.X + lightImg.Width;

            //Reset the rectangles that will hold the images for the inner part of the forest
            darkBox1.X = 0;
            darkBox2.X = darkBox1.X + darkImg.Width;

            //Reset the rectangle that will hold the door
            doorBox = new Rectangle((int)doorLoc[gameTemplate - 1].X, (int)doorLoc[gameTemplate - 1].Y, doorImg.Width, doorImg.Height);

            //Reset the rectangle that will hold the save sign
            saveSignBox = new Rectangle((int)saveSignLoc[gameTemplate - 1].X, (int)saveSignLoc[gameTemplate - 1].Y, saveSignImg.Width, saveSignImg.Height);

            //Reset the character's current platform
            curPlatform = 0;

            //Reset the character's location
            characterLocation = new Vector2(0, platformBox[curPlatform].Y - 100);

            //Reset the original location
            origLocation = characterLocation;

            //Set the character to standing
            charAction = new Character(charRightTextures[0], charTextNumFrames[0], charTextMaxFrames[0], characterLocation, false);

            //Character is not on a moving obstacle
            movingObst = false;
        }

        //Pre: None
        //Post: An entirely new game can begin
        //Desc: Generate new levels and reset all variables
        private void GameReset()
        {
            //Reset order of level
            tempLvOrder = "";

            //Generate levels
            LvGen();

            //Set the game templat
            gameTemplate = lvlOrder[0];

            //Reset game
            InitBonus(gameTemplate);
            InitObstacles(gameTemplate);
            InitPlatforms(gameTemplate);

            //Reset score
            second = 0;
            
            //Reset level
            level = 1;

            //Reset
            Reset();
        }

        //Pre: None
        //Post: Sort scores for leaderboard
        //Description: Check to see which scores are smaller, and move them accordingly
        private void ScoreSort()
        {
            //For each possible slot
            for (int a = scoreRecord.Length - 1; a > 0; a = a - 1)
            {
                //For each possible slot
                for (int i = scoreRecord.Length - 1; i > 0; i = i - 1)
                {
                    //If i is not a 0
                    //It is not
                    if (scoreRecord[i] != 0)
                    {
                        //if the score is smaller than the next score
                        //It is
                        if (scoreRecord[i] < scoreRecord[i - 1] || scoreRecord[i - 1] == 0)
                        {
                            //Rotate the scores
                            tempScore = scoreRecord[i];
                            scoreRecord[i] = scoreRecord[i - 1];
                            scoreRecord[i - 1] = tempScore;

                        }
                    }
                }
            }
        }

        //Pre: Whether it should be looking for the score, and the name of the file
        //Post: Files will be saved
        //Desc: Detect what is saved and write the proper data in the respective files accordingly
        private void SaveFiles(bool score, string txtFile)
        {
            try
            {
                //Get file path
                string filePath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

                //Get file path to directory
                filePath = Path.GetDirectoryName(filePath);

                //Remove the first 6 characters
                filePath = filePath.Substring(6);

                //Specify the location
                filePath += txtFile;

                //What is it saving
                //Levels
                if (score == false)
                {
                    //Write in the text no matter what is in it
                    outFile = File.CreateText(filePath);

                    //Write the order of the levels
                    outFile.WriteLine(tempLvOrder);

                    //Write the level that the player is currently on
                    outFile.WriteLine(level);

                    //Write the score the user has
                    outFile.WriteLine(second);
                }
                //Score
                else
                {
                    //Write in the text no matter what is in it
                    outFile = File.CreateText(filePath);

                    //For every possible score slot
                    for (int i = 0; i < scoreRecord.Length; i++)
                    {
                        //Write the levels
                        outFile.WriteLine(Convert.ToString(scoreRecord[i]));
                    }
                }

                //Closes file
                outFile.Close();
            }

            catch (FileNotFoundException fnf)
            {
                Console.WriteLine(fnf.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        //Pre: Whether it should be looking for the score, and the name of the file
        //Post: Previous info is loaded
        //Desc: Detect what it is looking for, and look into the proper file accordingly for the lines
        private void ReadFiles(bool score, string txtFile)
        {
            try
            {
                //Stores one line of text
                string line;

                string filePath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                filePath = Path.GetDirectoryName(filePath);

                //Remove the first 6 characters
                filePath = filePath.Substring(6);

                //Specify the location of the text
                filePath = filePath + txtFile;

                //Open file
                inFile = File.OpenText(filePath);

                //Store the row of the lines
                int row = 0;

                //While it has not reached the end
                while (!inFile.EndOfStream)
                {
                    //Read line
                    line = inFile.ReadLine();

                    //If it is looking for the score
                    //IT is not
                    if (score == false)
                    {
                        //Which row
                        //First 
                        if (row == 0)
                        {
                            //Update order of templats
                            tempLvOrder = line;
                        }
                        //Second
                        else if (row == 1)
                        {
                            //Update the levels
                            level = Convert.ToInt32(line);
                        }
                        //Third
                        else if (row == 2)
                        {
                            //Update the scores
                            second = Convert.ToInt32(line);
                        }

                        //Increase the row
                        row++;
                    }
                    //It is
                    else
                    {
                        //Save each line into an index
                        scoreRecord[row] = Convert.ToInt32(line);

                        //Increase the row number
                        row++;
                    }
                }

                //Closes file
                inFile.Close();
            }

            catch (FileNotFoundException fnf)
            {
                Console.WriteLine(fnf.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //Pre: None
        //Post: The user will be able to save their process
        //Desc: Save accordingly using the save files subprogram
        private void SaveCollision()
        {
            //If the user has collided with the save sign and pressed shift
            //It has
            if (CollisionDetection(saveSignBox) == true && (prevKb.IsKeyDown(Keys.RightShift) != true && kb.IsKeyDown(Keys.RightShift) == true))
            {
                //Save files
                SaveFiles(false, @"\Levels.txt");

                //Saved
                saved = true;
            }
        }

        //Pre: None
        //Post: The user will know whether there was a previously save file
        //Desc: Detect whether there are any lines of code in file
        private void CheckFiles()
        {
            try
            {
                //Get file path
                string filePath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

                //Get file path to directory
                filePath = Path.GetDirectoryName(filePath);

                //Remove the first 6 characters
                filePath = filePath.Substring(6);

                //Specify the location
                filePath += @"\Levels.txt";

                //Open the file
                inFile = File.OpenText(filePath);

                //Stores the row number
                int row = 0;
                
                //While it has not reached the end of the file
                while (!inFile.EndOfStream)
                {
                    //Read the line
                    inFile.ReadLine();

                    //Increase the row
                    row++;
                }

                //If the number of rows is more than 3
                //It is
                if (row >= 3)
                {
                    //There is a saved file
                    savedFile = true;
                }
                //It is not
                else
                {
                    //There is not a saved file
                    savedFile = false;
                }

                //Closes file
                inFile.Close();

            }

            catch (FileNotFoundException fnf)
            {
                Console.WriteLine(fnf.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //Pre: None
        //Post: Display buttons
        //Desc: Draw buttons accordingly for the respective game states
        private void ButtonDraw()
        {
            //Which gamestate
            switch (gameState)
            {
                //Menu
                case MENU:

                    //Draw Buttons
                    spriteBatch.Draw(playBttnImg, playBttnBox, Color.White);
                    spriteBatch.Draw(leaderboardBttnImg, leaderboardBttnBox, Color.White);
                    spriteBatch.Draw(quitBttnImg, quitBttnBox, Color.White);
                    spriteBatch.Draw(soundBttn, soundBttnBox, Color.White);
                    spriteBatch.Draw(muteBttn, muteBttnBox, Color.White);

                    //If there is a saved file
                    //There is 
                    if (savedFile == true)
                    {
                        //Clearly draw the load button
                        spriteBatch.Draw(loadBttnImg, loadBttnBox, Color.White);
                    }
                    //There is not saved file
                    else
                    {
                        //Draw the load button faded
                        spriteBatch.Draw(loadBttnImg, loadBttnBox, Color.Gray);
                    }

                    //Exit switch loop
                    break;

                //Leaderboard
                case LEADERBOARD:
                    //Reset the location of the return button
                    returnBttnBox.X = screenWidth - returnBttnImg.Width - 5;
                    returnBttnBox.Y = screenHeight - returnBttnImg.Height - 5;

                    //Draw the return button
                    spriteBatch.Draw(returnBttnImg, returnBttnBox, Color.White);

                    break;

                //Gameover screen
                case GAMEOVER:
                    spriteBatch.Draw(returnBttnImg, returnBttnBox, Color.White);
                    spriteBatch.Draw(playAgainBttnImg, playAgainBttnBox, Color.White);

                    break;

                //Gameend screen
                case GAMEEND:

                    spriteBatch.Draw(returnBttnImg, returnBttnBox, Color.White);
                    spriteBatch.Draw(playAgainBttnImg, playAgainBttnBox, Color.White);
                    break;

            }
        }

        //Pre: None
        //Post: Score will be saved and updated
        //Desc: Add new score into the leaderboard if applicable, and save using the savefile subprogram
        private void UpdateScore()
        {
            //For each possible score slot
            for (int i = scoreRecord.Length - 1; i >= 0; i--)
            {
                //If the slot does not have a score or the score is less than the value in the slot
                //It is
                if (scoreRecord[i] == 0 || scoreRecord[i] > second)
                {
                    //Upgrade the score slot
                    scoreRecord[i] = second;

                    //Sort the score
                    ScoreSort();

                    //Save the score
                    SaveFiles(true, @"Score.txt");

                    //Score has been updated
                    scoreUpdated = true;

                    //Exit for loop
                    break;
                }
            }
        }

        //Pre: None
        //Post:The character will stand after a jump 
        //Desc: Reset trajectory and make it so that the character is standing, then load proper images
        private void ResetTraj()
        {
            //Reset tajectory
            traj = startingTraj;

            //Character is now standing and update the images
            charActions = STAND;
            CharActionsLoadText();
        }

        //Pre: None
        //Post: Game will be displayed
        //Desc: Draw the character, obstacles, platforms, bonuses, signs, and bakcgrounds
        private void DrawGame()
        {
            //Where is the character
            //On the outer forest
            if (outerForest == true)
            {
                //Draw the backgrounds
                spriteBatch.Draw(lightImg, lightBox1, Color.White);
                spriteBatch.Draw(lightImg, lightBox2, Color.White);
                spriteBatch.Draw(lightImg, lightBox3, Color.White);
            }
            //On the outer forest
            else
            {
                //Draw the backgrounds
                spriteBatch.Draw(darkImg, darkBox1, Color.White);
                spriteBatch.Draw(darkImg, darkBox2, Color.White);
            }

            //Draw the platforms, obstacles, characters and bonuses
            DrawPlatforms(gameTemplate - 1);
            DrawObstacles();
            charAction.Draw(spriteBatch);
            BonusDraw();
        }

        //Pre: Width and height of the screen
        //Post: The screen size will be the desired size
        //Desc: Check whether the screen size is already the desired size. If not, change it
        private void ScreenSize(int width, int height)
        {
            //If the screen size is not the preferred size
            //It is not
            if (this.graphics.PreferredBackBufferWidth != width || this.graphics.PreferredBackBufferHeight != height)
            {
                //Update the screen width and height
                screenWidth = width;
                screenHeight = height;

                //Specifies the screen size that will change depending on where the player is
                this.graphics.PreferredBackBufferWidth = screenWidth;
                this.graphics.PreferredBackBufferHeight = screenHeight;

                //Apply such changes to screen size
                this.graphics.ApplyChanges();
            }
        }

        //Pre: None
        //Post: Play again will create a new game and return will go to the menu and reset for a possible new game
        //Desc: Detect which button was clicked and reset variables
        private void ButtonClick()
        {
            //New mouse click
            //There was
            if (NewMouseClick() == true)
            {
                //Which button was clicked
                //Play again
                if (MouseClicked(playAgainBttnBox) == true)
                {
                    //Player will play again
                    gameState = PLAY;

                    //Reset counter
                    counter = 0;

                    //Reset the game
                    GameReset();

                    //Resets music
                    MusicReset(menuBgMusic);
                }
                //Return
                else if (MouseClicked(returnBttnBox) == true)
                {
                    //Return to menu
                    gameState = MENU;

                    //Reset counter
                    counter = 0;

                    //Reset the game
                    GameReset();

                    //Resets music
                    MusicReset(menuBgMusic);
                }

                //Play the sound of a button clicking
                PlaySoundEffects(buttonClkSeInstance);
            }
        }

        //Pre: None
        //Post: Game can be played and animated
        //Desc: Update the animations for the character, obstacles, and bonuses. Then detect the character's actions and change its location and the locations of the things around it accordingly
        private void GameUpdate()
        {
            //Detect the character's actions and update the sprite accordingly
            CharActionDetect();
            CharAnimate();
            charAction.Update();

            //Animate the half platforms
            HalfPlatformAnim(gameTemplate);

            //Update the bonus particles and check for collision
            UpdateBonus();
            BonusCollision();

            //For each platform
            for (int x = 0; x < platformBox.Length; x++)
            {
                //If there is a platform collision with a new platform
                //There is
                if (CollisionDetection(platformBox[x]) == true && x != curPlatform)
                {
                    //Check where the character has collided with the platform
                    PlatformCollision(x);
                }
            }

            //If the character has fallen from the platform
            PlatformFall();

            //If the character has fallen from the platform
            //It has
            if (charFall == true)
            {
                //Character is falling
                CharacterFall();
            }

            //If the character is on a moving platform
            //It is
            if (movingObst == true)
            {
                //Update the character's location accordingly
                PlatformCharAnim();
            }

            //Update the obstacles motion and check for collision
            ObstaclesMotion();
            ObstacleCollision();
        }

        //Pre: Whether the character has collided with the left side of the obstacle and which obstacle
        //Post: The character will not move
        //Desc: Detect its action, which side has it collided with and what has it collided with then change it's x location and y location accordingly
        private void CharObstCol(bool left, int collidedObst)
        {
            //Store the integer that will determine whether the values are negative or positive
            int i = 1;

            //If the collision is on the left
            if (left == true)
            {
                //Values will be nagative
                i = -1;
            }
            //If the character is running or attacking
            if (charActions == RUN || charActions == ATTACK)
            {
                //Update the character's location accordingly
                characterLocation.X += 5 * i;
                charAction.charBox.X += 5 * i;
            }
            //If the character is jumping
            else if (charActions == JUMP)
            {
                //Update the coordinates
                characterLocation.X += (int)scaledTraj.X * i;
                charAction.charBox.X += (int)scaledTraj.X * i;

                //Reset trajectory and make the character stand
                ResetTraj();

            }
            
        }
    }
}
