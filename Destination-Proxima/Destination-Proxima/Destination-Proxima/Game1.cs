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
        int player1Health = 99;
        float healthBarRotation = 0.0f;
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

        //Start Screen Stars
        List<Vector2> starPositions;
        List<Rectangle> starRects;

        //Fonts
        SpriteFont mainGameFont;

        //Button Hovering
        Boolean startGameHover = false;

        //Strings
        string[] characterMap = {"a","A","b","B","c","C","d","D","e","E","f","F","g","G","h","H","i","I","j","J","k","K",
                                       "l","L","m","M","n","N","o","O","p","P","q","Q","r","R","s","S","t","T","u","U","v",
                                       "V","w","W","x","X","y","Y","z","Z",";",":","'","[","]","{","}",",","/",".","+","=",
                                       "!","@","#","$","%","^","&","*","(",")","~","-","_","`"};
        string startString = "Start Game";
        string pausedString = "Game Paused";
        string resumeString = "Resume";
        string gameOverString = "Game Over";
        string quitString = "Quit";
        string levelString = "Level:";

        //Textures
        Texture2D Player1Texture;
        Texture2D player1TextureBR;
        Texture2D player1TextureBL;
        Texture2D player1Shot;
        Texture2D player1HealthMeter;
        Texture2D enemy1Texture;
        Texture2D starTexture;
        Texture2D startSplashScreen;


        //Positons
        int player1TravelV, player1TravelH;
        Rectangle player1Pos;
        Rectangle startButtonPos;



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
            //Positions
            player1Pos = new Rectangle(graphics.PreferredBackBufferWidth / 2, 500, 39, 49);
            startButtonPos = new Rectangle(graphics.PreferredBackBufferWidth / 3, 310, 270, 60);

            IsMouseVisible = true;
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Start Star Spots
            starPositions = new List<Vector2>();
            starRects = new List<Rectangle>();
            //Missile Spots
            misslePositions = new List<Vector2>();
            missleRects = new List<Rectangle>();
            //Enemy Spots
            enemyPositions = new List<Vector2>();
            enemyRects = new List<Rectangle>();
            //Fonts
            mainGameFont = Content.Load<SpriteFont>("StartButton");
            //Player 1
            player1TextureBR = Content.Load<Texture2D>("player1BR");
            player1TextureBL = Content.Load<Texture2D>("player1BL");
            Player1Texture = Content.Load<Texture2D>("player1");
            player1Shot = Content.Load<Texture2D>("player1Shot");
            player1HealthMeter = Content.Load<Texture2D>("healthBar");
            //Enemy
            enemy1Texture = Content.Load<Texture2D>("enemy1");
            //Menues
            startSplashScreen = Content.Load<Texture2D>("Destination_Proxima");
            starTexture = Content.Load<Texture2D>("Star");
        }


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        protected override void Update(GameTime gameTime)
        {
            //Exit Game
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && gameState == GameState.MainMenu)
                this.Exit();
            // Pause game
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && gameState == GameState.Play)
                gameState = GameState.PauseGame;

            if (Keyboard.GetState().IsKeyDown(Keys.P) && gameState == GameState.Play)
                player1Health--;

            switch (gameState)
            {
                case GameState.MainMenu:
                    Point mousePoint = new Point(Mouse.GetState().X, Mouse.GetState().Y);
                    if (startButtonPos.Contains(mousePoint))
                    {
                        startGameHover = true;
                    }                    
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
                     if (player1Pos.X < -10) { player1Pos.X = (graphics.PreferredBackBufferWidth - 10); }
                     else if (player1Pos.X > (graphics.PreferredBackBufferWidth - 10)) { player1Pos.X = -10; }
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
                     else if (player1Pos.Y > 540) { player1Pos.Y = (graphics.PreferredBackBufferHeight - 60); }
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
            healthCheck();
            oldState = currentState;
            base.Update(gameTime);
        }

        private void healthCheck()
        {
            if (player1Health > 90) { healthBarRotation = 0.0f; }
            else if (player1Health < 89 && player1Health > 80) { healthBarRotation = -0.1f; }
            else if (player1Health < 79 && player1Health > 75) { healthBarRotation = -0.2f; }
            else if (player1Health < 74 && player1Health > 70) { healthBarRotation = -0.3f; }
            else if (player1Health < 69 && player1Health > 65) { healthBarRotation = -0.4f; }
            else if (player1Health < 64 && player1Health > 60) { healthBarRotation = -0.5f; }
            else if (player1Health < 59 && player1Health > 55) { healthBarRotation = -0.6f; }
            else if (player1Health < 54 && player1Health > 50) { healthBarRotation = -0.7f; }
            else if (player1Health < 49 && player1Health > 45) { healthBarRotation = -0.8f; }
            else if (player1Health < 44 && player1Health > 40) { healthBarRotation = -0.9f; }
            else if (player1Health < 39 && player1Health > 35) { healthBarRotation = -1.0f; }
            else if (player1Health < 34 && player1Health > 30) { healthBarRotation = -1.1f; }
            else if (player1Health < 29 && player1Health > 25) { healthBarRotation = -1.2f; }
            else if (player1Health < 24 && player1Health > 20) { healthBarRotation = -1.3f; }
            else if (player1Health < 19 && player1Health > 10) { healthBarRotation = -1.4f; }
            else if (player1Health < 9 && player1Health > 0) { healthBarRotation = -1.5f; }
            else if (player1Health < 0) { healthBarRotation = -1.6f; }
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

        private void playerHealth()
        {

            spriteBatch.Draw(player1HealthMeter, new Rectangle(0,graphics.PreferredBackBufferHeight,130,130), null, Color.White, healthBarRotation,new Vector2(0,player1HealthMeter.Height),SpriteEffects.None, 0f);
            spriteBatch.DrawString(mainGameFont, player1Health.ToString(), new Vector2(9,graphics.PreferredBackBufferHeight - 60), Color.White);
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
            spriteBatch.DrawString(mainGameFont, pausedString, new Vector2((graphics.PreferredBackBufferWidth / 3), 12), Color.White);
            spriteBatch.DrawString(mainGameFont, quitString, new Vector2((graphics.PreferredBackBufferWidth / 5), (graphics.PreferredBackBufferHeight / 3)), Color.White);
            spriteBatch.DrawString(mainGameFont, resumeString, new Vector2((graphics.PreferredBackBufferWidth / 5 * 3), (graphics.PreferredBackBufferHeight / 3)), Color.White);
            playerHealth();
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
            playerHealth();
        }
        private void DrawContentStartscreen()
        {
            generateStartStars();
            moveStartStars();
            for (int i = 0; i < starPositions.Count(); i++)
                spriteBatch.Draw(starTexture, starPositions[i], Color.White);
            spriteBatch.Draw(startSplashScreen, new Rectangle(graphics.PreferredBackBufferWidth / 9, graphics.PreferredBackBufferHeight / 6,700,200), Color.White);
            spriteBatch.DrawString(mainGameFont, startString, new Vector2(startButtonPos.X, startButtonPos.Y), Color.White);
        }
        public void generateStartStars()
        {
            Random r = new Random();
            for (int i = 0; i < 1; i++)
            {
                int randomHight = r.Next(1, graphics.PreferredBackBufferHeight - starTexture.Height);
                starPositions.Add(new Vector2(0, randomHight));
                starRects.Add(new Rectangle((int)(0), (int)randomHight, starTexture.Width, starTexture.Height));
            }
        }
        public void moveStartStars()
        {
            for (int i = 0; i < starPositions.Count(); i++)
            {
                starPositions[i] += new Vector2(4, 0);
                starRects[i] = new Rectangle((int)starPositions[i].X, (int)starPositions[i].Y, starTexture.Width, starTexture.Height);

                if (starPositions[i].Y < 1)
                {
                    starPositions.RemoveAt(i);
                    starRects.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}