/*
* Author: Allison
* File Name: Character.cs
* Project Name: ForestTrail
* Creation Date: Dec. 14, 2016
* Modified Date: Dec. 31, 2017
* Description: Allows characters to be aninmated
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ForestTrail
{
    public class Character
    {
        //Store the location of the character
        public Vector2 charLoc;

        //Stores the image that will be used
        public Texture2D charText;

        //Stores the amount of frames in each row
        public int frameRowNum;

        //Stores the max amount of frames
        public int maxFrame;

        //Stores the box that will store the image
        public Rectangle charBox;

        //Stores the source box
        private Rectangle charSrcBox;

        //Current frame 
        public float frameNum;

        //Stores the height and width of each frame
        float frameHeight;
        float frameWidth;

        //Allows the animation to be slower
        float smooth = 0.25f;

        //Store whether it is an obstacle
        public bool isObstacle;

        //Will store how much time has passed
        int counter;

        //Store whether the image will have an effect
        public SpriteEffects spriteEffect = SpriteEffects.None;

        //Pre: The image that will be used to diplay the character. Also information about the image
        //Post: Sets respective values
        //Description: Setting the values in the class to their respective values that are retrieved. Also initializing the rectangles, and calculating the height and width of the frames
        public Character(Texture2D textureUsed, int numFrameRow, int maxFrames, Vector2 charLocation, bool obstacle)
        {
            //Set the respective values
            charText = textureUsed;
            frameRowNum = numFrameRow;
            maxFrame = maxFrames;
            charLoc = charLocation;
            frameNum = 0;
            isObstacle = obstacle;

            //Calculate the height and width of each frame
            frameHeight = (float)(charText.Height);
            frameWidth = (float)(charText.Width / frameRowNum);

            //Initialize the rectangles that will contain images of the character
            charBox = new Rectangle((int)charLoc.X, (int)charLoc.Y, (int)frameWidth, (int)frameHeight);

            //Initialize the source boxes that will allow animation
            charSrcBox = new Rectangle(0, 0, (int)frameWidth, (int)frameHeight);
        }

        //Pre: None
        //Post: Displayed image is animated
        //Description: Updates the location of the source box and framenumber
        public void Update()
        {
            //Set the collumn and row where the frame is located
            int col = (int)frameNum % frameRowNum;

            //Increase the counter
            counter++;

            //If it is an obstacle and it's max amount of frames is 6
            //It is
            if (isObstacle == true && maxFrame == 6)
            {
                //Which frames
                //If its frame nume is less than or equal to 2
                if (frameNum <= 2 || frameNum >= 5)
                {
                    //Updates the location of the source box
                    charSrcBox.X = col * (int)frameWidth;
                    charSrcBox.Y = 0;

                    //Increase the frame number
                    frameNum += smooth;
                }
                //It is the frames in between
                else 
                {
                    //Updates the location of the source box
                    charSrcBox.X = col * (int)frameWidth;
                    charSrcBox.Y = 0;

                    //Increase the frame number
                    frameNum += smooth / 50f;
                }
            }
            //It is not
            else
            {
                //Updates the location of the source box
                charSrcBox.X = col * (int)frameWidth;
                charSrcBox.Y = 0;

                //Increase the frame number
                frameNum += smooth;
            }

            //If the frame number has reached the max
            //It has
            if (frameNum >= maxFrame)
            {
                //Set the frame number to 0
                frameNum = 0;
            }
        }

        //Pre: A spritebatch
        //Post: Character is displayed
        //Description: Draws the character
        public void Draw(SpriteBatch spriteBatch)
        {
            //Draws character
            spriteBatch.Draw(charText, charBox, charSrcBox,  Color.White, 0f, Vector2.Zero, spriteEffect, 0);
        }
    }
}
