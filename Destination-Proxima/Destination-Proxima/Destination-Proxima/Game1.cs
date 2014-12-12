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

namespace Destination_Proxima
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Textures
        Texture2D Player1Texture;
        Texture2D StarTexture;
        Texture2D startSplashScreen;

        //Positons
        Rectangle player1Pos = new Rectangle(445, 500, 39, 49);



        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 900;
            graphics.PreferredBackBufferHeight = 600;
            Window.Title = "Destination Proxima";
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }


        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Player1Texture = Content.Load<Texture2D>("player1");
            startSplashScreen = Content.Load<Texture2D>("Destination_Proxima");
            StarTexture = Content.Load<Texture2D>("Star");
            // TODO: use this.Content to load your game content here
        }


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            //spriteBatch.Draw(Player1Texture, player1Pos, Color.White);
            DrawContentStartscreen();
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawContentStartscreen()
        {
            Random r = new Random();
            int i = 0; 
            while (i < 20)
            {
                i++;
                spriteBatch.Draw(StarTexture, new Vector2(r.Next(0, graphics.PreferredBackBufferWidth), r.Next(0, graphics.PreferredBackBufferHeight)), Color.White);
            }
            spriteBatch.Draw(startSplashScreen, new Rectangle(graphics.PreferredBackBufferWidth / 9, graphics.PreferredBackBufferHeight / 6,700,200), Color.White);
        }
    }
}