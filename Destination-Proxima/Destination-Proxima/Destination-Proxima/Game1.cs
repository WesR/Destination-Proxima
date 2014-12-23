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

//Copyright Wes Ring 
namespace Destination_Proxima
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        enum GameState
        {
            MainMenu,
            LevelDisplay,
            PauseGame,
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
        KeyboardState oldState;

        //Varibles
        int currentLevel = 1;
        int playerSpeed = 2;
        int player1DriftRate = 10;
        int player1DriftCurrentX;
        int player1DriftCurrentY;
        int maxPlayerSpeed = 5;
        int maxPlayerShotSpeed = 20;
        int PlayerShotSpeedCurrent;

        //Enemy
        List<Vector2> enemyPositions;
        List<Rectangle> enemyRects;

        //Missiles
        List<Vector2> misslePositions;
        List<Rectangle> missleRects;

        //Fonts
        SpriteFont mainGameFont;

        //Textures
        Texture2D Player1Texture;
        Texture2D player1TextureBR;
        Texture2D player1TextureBL;
        Texture2D player1Shot;
        Texture2D enemy1Texture;
        Texture2D StarTexture;
        Texture2D startSplashScreen;

        //Positons
        int player1TravelV, player1TravelH;
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

            //Missile Spots
            misslePositions = new List<Vector2>();
            missleRects = new List<Rectangle>();
            //Enemy Spots
            enemyPositions = new List<Vector2>();
            enemyRects = new List<Rectangle>();

            mainGameFont = Content.Load<SpriteFont>("StartButton");
            player1TextureBR = Content.Load<Texture2D>("player1BR");
            player1TextureBL = Content.Load<Texture2D>("player1BL");
            Player1Texture = Content.Load<Texture2D>("player1");
            player1Shot = Content.Load<Texture2D>("player1Shot");
            enemy1Texture = Content.Load<Texture2D>("enemy1");
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
                gameState = GameState.PauseGame;

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

                case GameState.PauseGame:
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                        gameState = GameState.Play;
                    break;

                case GameState.Play:
                     currentState = Keyboard.GetState();
                     playerTilt = PlayerTilt.None; //Rests tilt
                    //X axis movment
                     if (currentState.IsKeyDown(Keys.A)) { player1TravelH = player1TravelH  - playerSpeed; playerTilt = PlayerTilt.Left; }
                     if (currentState.IsKeyDown(Keys.D)) { player1TravelH = player1TravelH + playerSpeed; playerTilt = PlayerTilt.Right; }
                    //Wrap around X
                     if (player1Pos.X < -10) { player1Pos.X = 890; }
                     else if (player1Pos.X > 890) { player1Pos.X = -10; }
                    //Drift
                     if (player1DriftCurrentX >= player1DriftRate) 
                     {
                         if (currentState.IsKeyUp(Keys.A) && player1TravelH < 0) { player1TravelH = player1TravelH + 1; }
                         if (currentState.IsKeyUp(Keys.D)  && player1TravelH > 0) { player1TravelH = player1TravelH - 1; }
                         player1DriftCurrentX = 0; //Reseting counter
                     }
                     else
                     {
                         player1DriftCurrentX++; //Adding to counter
                     }
                    //Y axis movment
                     if (currentState.IsKeyDown(Keys.W)) { player1TravelV = player1TravelV - playerSpeed; }
                     if (currentState.IsKeyDown(Keys.S)) { player1TravelV = player1TravelV + playerSpeed; }
                    //Wrap around Y
                    //if (player1Pos.Y < -40) { player1Pos.Y = 590; }
                    // else if (player1Pos.Y > 600) { player1Pos.Y = -10; }
                    //Limiting Y movement
                    if (player1Pos.Y < 5) { player1Pos.Y = 5; }
                     else if (player1Pos.Y > 540) { player1Pos.Y = 540; }
                     if (player1DriftCurrentY >= player1DriftRate) //Drift
                     {
                         if (currentState.IsKeyUp(Keys.W)  && player1TravelV < 0) { player1TravelV = player1TravelV + 1; }
                         if (currentState.IsKeyUp(Keys.S)  && player1TravelV > 0) { player1TravelV = player1TravelV - 1; }
                         player1DriftCurrentY = 0; //Reseting counter
                     }
                     else
                     {
                         player1DriftCurrentY++; //Adding to counter
                     }
                    //Shooting
                     if (PlayerShotSpeedCurrent >= maxPlayerShotSpeed || oldState.IsKeyUp(Keys.Space))
                     {
                         if (currentState.IsKeyDown(Keys.Space))
                         {
                             misslePositions.Add(new Vector2(player1Pos.X + (Player1Texture.Width / 3), player1Pos.Y));
                             misslePositions.Add(new Vector2(player1Pos.X, player1Pos.Y));
                             missleRects.Add(new Rectangle((int)(player1Pos.X + (Player1Texture.Width / 3)), (int)player1Pos.Y, player1Shot.Width, player1Shot.Height));
                             missleRects.Add(new Rectangle((int)(player1Pos.X), (int)player1Pos.Y, player1Shot.Width, player1Shot.Height));
                         }
                         PlayerShotSpeedCurrent = 0;
                     }
                     else
                     {
                         PlayerShotSpeedCurrent++;
                     }
                    //Setting the speed
                    player1Pos.X = player1Pos.X + player1TravelH; //Updateing speed
                    if (player1TravelH > maxPlayerSpeed){ player1TravelH = maxPlayerSpeed;} //Speed cap
                    if (player1TravelH < -maxPlayerSpeed){ player1TravelH = -maxPlayerSpeed;}//Speed cap
                    
                    player1Pos.Y = player1Pos.Y + player1TravelV; //Updating speed
                    if (player1TravelV > maxPlayerSpeed){ player1TravelV = maxPlayerSpeed;}//Speed cap
                    if (player1TravelV < -maxPlayerSpeed){ player1TravelV = -maxPlayerSpeed;}//Speed cap

                    //Move missiles
                     for (int i = 0; i < misslePositions.Count(); i++)
                     {
                         misslePositions[i] -= new Vector2(0, 8);
                         missleRects[i] = new Rectangle((int)misslePositions[i].X, (int)misslePositions[i].Y, player1Shot.Width, player1Shot.Height);

                         if (misslePositions[i].Y < 1)
                         {
                             misslePositions.RemoveAt(i);
                             missleRects.RemoveAt(i);
                             i--;
                         }
                    }
                    break;
                case GameState.End:
                    this.Exit();
                    break;
            }
            oldState = currentState;
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
                case GameState.PauseGame:
                    GamePaused();
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

        private void GamePaused()
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
            }
            //Player1 shots
            for (int i = 0; i < misslePositions.Count(); i++)
                spriteBatch.Draw(player1Shot, misslePositions[i], Color.White);
        
        //Menu bars and title
            spriteBatch.DrawString(mainGameFont, "Game Paused", new Vector2((graphics.PreferredBackBufferWidth / 3), 12), Color.White);
            spriteBatch.DrawString(mainGameFont, "Quit", new Vector2((graphics.PreferredBackBufferWidth / 5), (graphics.PreferredBackBufferHeight / 3)), Color.White);
            spriteBatch.DrawString(mainGameFont, "Resume", new Vector2((graphics.PreferredBackBufferWidth / 5 * 3), (graphics.PreferredBackBufferHeight / 3)), Color.White);
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
            }
            //Player1 shots
            for (int i = 0; i < misslePositions.Count(); i++)
                spriteBatch.Draw(player1Shot, misslePositions[i], Color.White);
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
            spriteBatch.DrawString(mainGameFont, "Start Game", new Vector2(startButtonPos.X, startButtonPos.Y), Color.White);
        }
    }
}