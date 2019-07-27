/*
* Author: Allison
* File Name: ParticleEngine.cs
* Project Name: ForestTrail
* Creation Date: Dec. 14, 2016
* Modified Date: Dec. 31, 2017
* Description: Will emit particles
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ForestTrail
{
    public class ParticleEngine
    {
        //Will allow random numbers to be generate
        private Random rng = new Random();

        //Store the randomly generated number
        private double randomNum;

        //Store the location of the emitter
        public Vector2 emitterLoc;

        //Store the texture of the particle
        private Texture2D particleText;

        //Stores a list of the particles
        public List<Particle> particles = new List<Particle>();

        //Stores a list of the location of each individual particle (will be used to detect collison when used as an obstacle)
        public List<Vector2> particleLoc = new List<Vector2>();

        //Store whether the particle will be used as an obstacle
        public bool obstacle;

        //Store the variable that will keep count
        float count;

        //Pre: Texture, location, and use of the particle
        //Post: Update information about the particle
        //Desc: Retrieve data about the particle outside of this class
        public ParticleEngine(Texture2D textures, Vector2 location,bool isObstacle)
        {
            //Set the location of the particle emitter
            emitterLoc = location;

            //Update the texture that is used by the particle
            particleText = textures;

            //Set whether the particle will be used as an obstacle
            obstacle = isObstacle;
        }

        //Pre: None
        //Post: Particle info is updated and particles are generated
        //Desc: Uses subprograms and classes. Also updates list that contains the location of each particle
        public void Update()
        {
            //If the particle is not used as an obastacle
            //It is not used as an obstacle
            if (obstacle != true)
            {
                //Increase counter by a quarter
                count += 0.25f;
            }

            //If the particle is used as an obstacle
            //It is 
            if (obstacle == true)
            {
                //Generate a new particle
                particles.Add(GenParticles());
            }
            //It is not used as an obstacle
            //Note: Allows less particles to be generated for the bonus
            else
            {
                //If counter is an integer
                //It is an integer
                if (count % 1 == 0)
                {
                    //Generate a new particle
                    particles.Add(GenParticles());
                }
            }

            //From 0 to the number of particles
            for (int x = 0; x < particles.Count; x++)
            {
                //Update the information about the particle
                particles[x].UpdateParticles();

                //Update the list containing the position of each current particle
                particleLoc.Add(particles[x].position);

                //If the particle's time left alive has reached 0
                if (particles[x].TLA <= 0)
                {
                    //Remove the particle
                    particles.RemoveAt(x);

                    //Remove particle's information from the list that contains its location
                    particleLoc.RemoveAt(x);
                }
            }

        }

        //Pre: Spritebatch
        //Post: Particles will be drawn
        //Desc: Draw each particle that is alive
        public void Draw(SpriteBatch spriteBatch)
        {
            //From 0 to the amount of particles
            for (int a = 0; a < particles.Count; a++)
            {
                //Draw the particle
                particles[a].DrawParticles(spriteBatch);
            }
        }

        //Pre: None
        //Post: Generates new particles
        //Desc: Using retrieved data and varying information based on the use of the particle to generate a new particle
        private Particle GenParticles()
        {
            //Stores the velocity of the particle
            Vector2 velocity;

            //Stores the time left alive of the particle
            int TLA;

            //Stores how much the angle will change
            float angularVelocity;

            //Stores the colour of the particle
            Color colour;

            //If the particle is used as an obstacle
            //It is
            if (obstacle == true)
            {
                //Initialize velocity which is randomized
                velocity = new Vector2((float)RandomNumGenerator(1, 8), (float)RandomNumGenerator(1, 8));

                //Initialize angular velocity which is randomized
                angularVelocity = (float)RandomNumGenerator(-3, 3) / 3;

                //Set time left alive to 50
                TLA = 50;

                //Set the colour to white
                colour = Color.White;
            }
            //If the particle is not used as an obstacle
            else
            {
                //Initialize velocity which is randomized
                velocity = new Vector2((float)RandomNumGenerator(1,2)/8, (float)RandomNumGenerator(1, 3));

                //Initialize angular velocity which is randomized
                angularVelocity = (float)RandomNumGenerator(-3, 3) / 3;

                //Set time left alive to 30
                TLA = 30;

                //Set the colour to green
                colour = Color.Green;
            }

            //Set the size of the particle
            float size = 0.5f;

            //Return the particle that is generate using the data that is initialized
            return new Particle(particleText, emitterLoc, velocity, 0, angularVelocity, colour, TLA, size, obstacle);
        }

        //Pre: A min and max number
        //Post: Generates a number between the min and max number
        //Desc: Generates a new number
        private double RandomNumGenerator(int minValue, int maxValue)
        {
            //Generates a new number
            randomNum = rng.Next(minValue, maxValue);

            //Returns the newly generated number
            return randomNum;
        }

    }
}
