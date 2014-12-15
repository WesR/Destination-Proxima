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

        enum GameState
        {
            MainMenu,
            Play,
            End,
        }
        enum PlayerTilt
        {
            None,
            Left,
            Right,
        }
       
        //States
        PlayerTilt playerTilt;
        GameState gameState;
        KeyboardState currentState;

        //Varibles
        int playerSpeed = 4;

        //Missiles
        List<Vector2> misslePositions;
        List<Rectangle> missleRects;

        //Fonts
        SpriteFont startButtonFont;

        //Textures
        Texture2D Player1Texture;
        Texture2D player1TextureBR;
        Texture2D player1TextureBL;
        Texture2D player1Shot;
        Texture2D StarTexture;
        Texture2D startSplashScreen;

        //Positons
        Rectangle player1Pos = new Rectangle(445, 500, 39, 49);
        Rectangle startButtonPos = new Rectangle(340, 310, 270,60);



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
            IsMouseVisible = true;
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            misslePositions = new List<Vector2>();
            missleRects = new List<Rectangle>();

            startButtonFont = Content.Load<SpriteFont>("StartButton");
            player1TextureBR = Content.Load<Texture2D>("player1BR");
            player1TextureBL = Content.Load<Texture2D>("player1BL");
            Player1Texture = Content.Load<Texture2D>("player1");
            player1Shot = Content.Load<Texture2D>("player1Shot");
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

            switch (gameState)
            {
                case GameState.MainMenu:
                    Point mousePoint = new Point(Mouse.GetState().X, Mouse.GetState().Y);
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        if (startButtonPos.Contains(mousePoint))
                            gameState = GameState.Play;
                    }
                    break;
                case GameState.Play:
                     currentState = Keyboard.GetState();
                     playerTilt = PlayerTilt.None;
                     if (currentState.IsKeyDown(Keys.A) && player1Pos.X > 3) { player1Pos.X = player1Pos.X - playerSpeed; playerTilt = PlayerTilt.Left; }
                     if (currentState.IsKeyDown(Keys.D) && player1Pos.X < 860) { player1Pos.X = player1Pos.X + playerSpeed; playerTilt = PlayerTilt.Right; }
                     if (currentState.IsKeyDown(Keys.W) && player1Pos.Y > 0) { player1Pos.Y = player1Pos.Y - playerSpeed;}
                     if (currentState.IsKeyDown(Keys.S) && player1Pos.Y < 550) { player1Pos.Y = player1Pos.Y + playerSpeed;}
                     if (currentState.IsKeyDown(Keys.Space))
                     {
                         misslePositions.Add(new Vector2(player1Pos.X + (Player1Texture.Width / 2), player1Pos.Y));
                         missleRects.Add(new Rectangle((int)(player1Pos.X + (Player1Texture.Width / 2)), (int)player1Pos.Y, player1Shot.Width, player1Shot.Height));
                     }

                     for (int i = 0; i < misslePositions.Count(); i++)
                     {
                         misslePositions[i] -= new Vector2(0, 2);
                         missleRects[i] = new Rectangle((int)misslePositions[i].X, (int)misslePositions[i].Y, player1Shot.Width, player1Shot.Height);

                        // if (8==7)
                       //  {
                         //    misslePositions.RemoveAt(i);
                        //     missleRects.RemoveAt(i);
                      //       i--;
                     //    }
                     }
                    break;
                case GameState.End:
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();                    //spriteBatch.Draw(Player1Texture, player1Pos, Color.White);
            switch (gameState)
            {
                case GameState.MainMenu:
                    DrawContentStartscreen();
                    break;
                case GameState.Play:
                    DrawGamePlay();
                    break;
                case GameState.End:
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawGamePlay()
        {
            switch (playerTilt)
            {
                    //Player 1
                case PlayerTilt.Left:
                    spriteBatch.Draw(player1TextureBL, player1Pos, Color.White);
                    break;
                case PlayerTilt.Right:
                    spriteBatch.Draw(player1TextureBR, player1Pos, Color.White);
                    break;
                case PlayerTilt.None:
                    spriteBatch.Draw(Player1Texture, player1Pos, Color.White);
                    break;


                    //Player1 shots
                    for (int  i = 0; i < misslePositions.Count(); i++)
                        spriteBatch.Draw(player1Shot, misslePositions[i], Color.White);
            }
            
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
            spriteBatch.DrawString(startButtonFont, "Start Game", new Vector2(startButtonPos.X, startButtonPos.Y), Color.White);
        }
    }
}