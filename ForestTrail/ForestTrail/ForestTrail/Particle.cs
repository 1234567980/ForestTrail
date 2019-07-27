/*
* Author: Allison
* File Name: Particles.cs
* Project Name: ForestTrail
* Creation Date: Dec. 14, 2016
* Modified Date: Dec. 31, 2017
* Description: Creates particles
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ForestTrail
{
    public class Particle
    {
        //Store the image of the particle
        public Texture2D textureImg;

        //Store the location of theparticle
        public Vector2 position;

        //Store how much the location of the particle will change
        public Vector2 velocity;

        //Store the angle of the particle
        public float angle;

        //Store how much the angle of the particle will change
        public float angleChangeRate;

        //Store the colour of the particle
        public Color particleColour;

        //Store the time left alive
        public int TLA;

        //Store the size of the particle
        public float size;

        //Store the location where the particles are emitted from
        public Vector2 emitLoc;

        //Store whether the particle will be used as an obstacle
        public bool obstacle;

        //Store the depth of the layer of the particle
        private float layerDepth = 0f;

        //Store the sprite effects that will be applied on the particle
        private SpriteEffects spriteEff = SpriteEffects.None;

        //Pre: The image, location, velocity, angle, angle change rate, colour, time left alive, size, and use of the particle
        //Post: Sets respective values
        //Description: Setting the values in the class to their respective values that are retrieved
        public Particle (Texture2D partImg, Vector2 partPosition, Vector2 partVelocity, float partAngle, float partACR, Color partColour, int partTLA, float partSize, bool isObstacle)
        {
            //Sets respective information about particles
            textureImg = partImg;
            position = partPosition;
            velocity = partVelocity;
            angle = partAngle;
            angleChangeRate = partACR;
            particleColour = partColour;
            TLA = partTLA;
            size = partSize;
            obstacle = isObstacle;
        }

        //Pre: None
        //Post: Information about the particles will be update
        //Description: Increase counter, decrease the particle's time left alive, and change the angle's position based on the use of the particle
        public void UpdateParticles()
        {

            //Decrease time left alive
            TLA--;

            //Increase the angle by the rate of which it changes
            angle += angleChangeRate;

            //If the particle is used as an obstacle
            //It is used as an obstacle
            if (obstacle == true)
            {
                //Increase the y value of the velocity by how much the angle changes
                velocity.Y = angleChangeRate;
            }
            //It is not used as an obstacle
            else
            {
                //Increase the x value of the velocity by how much the angle of the particle changes
                velocity.X = angleChangeRate;
            }

            //Decrease the particle's position by velocity's respective X and Y values
            //Note: Works to emit particles upwards for bonus indication, and emit particles towards the left when used as an obstacle
            position -= velocity;
        }

        //Pre: A spritebatch
        //Post: Particle will be drawn
        //Description: Set the source box and origin of the particle, then draw the particle
        public void DrawParticles (SpriteBatch spriteBatch)
        {
            //Initialize source box
            Rectangle textureSrcBox = new Rectangle(0, 0, textureImg.Width, textureImg.Height);

            //Initialize the origin of the particle
            Vector2 origin = new Vector2(textureImg.Width / 2, textureImg.Height / 2);

            //Draw particle
            spriteBatch.Draw (textureImg, position, textureSrcBox, particleColour, angle, origin, size, spriteEff, layerDepth); 

        }

    }
}
