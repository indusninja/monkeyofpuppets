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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace NordicGameJam2010.Components
{
    public class Player : GameObject
    {
        Texture2D beatenTexture;

        bool hasKey = false;
        bool hasFinishedLevel = false;
        int maxDummyCount = 3;
        int dummiesLeft;
        Texture2D dummyTexture;
        Texture2D keyTexture;
        
        bool isSpacePressed = false;
        bool isAPressed = false;

        public bool HasKey
        {
            get
            {
                return hasKey;
            }
            set
            {
                hasKey = value;
            }
        }

        public bool HasFinishedLevel
        {
            get
            {
                return hasFinishedLevel;
            }
            set
            {
                hasFinishedLevel = value;
            }
        }

        public int DecoysLeft
        {
            get
            {
                return dummiesLeft;
            }
            set
            {
                dummiesLeft = value;
            }
        }

        public Player(Game game, TileLayer tileLayer, Vector2 position, string textureName,int decoyCount)
            : base(game, tileLayer, position, textureName)
        {
            dummyTexture = Game.Content.Load<Texture2D>("Textures/monkey_Dummy");
            beatenTexture = Game.Content.Load<Texture2D>("Textures/monkey_beaten");
            keyTexture = Game.Content.Load<Texture2D>("Textures/Key");
            UpdateOrder = 10;
            maxDummyCount = decoyCount;
            dummiesLeft = maxDummyCount;
            texture = Game.Content.Load<Texture2D>(textureName);
        }

        public override void StartCombat()
        {
            bDrawTexture = false;
            base.StartCombat();
            Audio.State = AudioState.Kill;
        }
        public override int TargetPriority()
        {
            return 1;
        }

        public override void Kill()
        {
            base.Kill();
            (Game as Game1).UpdateGameState(GameState.LevelLoose);
            texture = beatenTexture;
            UpdateOrder = 1000;
            bDrawTexture = true;
        }

        public override void Update(GameTime gameTime)
        {
            float fDeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);

            if (!IsAlive())
                return;

            // Get move dir
            Vector2 moveDir = Vector2.Zero;
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.D))
            {
                moveDir.X += 1;
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                moveDir.X -= 1;
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                moveDir.Y += 1;
            }
            if (keyboardState.IsKeyDown(Keys.W))
            {
                moveDir.Y -= 1;
            }
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                isSpacePressed = true;
            }
            else if (keyboardState.IsKeyUp(Keys.Space) && isSpacePressed && (dummiesLeft>=1))
            {
                isSpacePressed = false;
                Game.Components.Add(new Pickup(Game, tileLayer, this.position, "Textures\\monkey_Dummy"));
                dummiesLeft--;
            }
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                if (moveDir.Equals(Vector2.Zero))
                {
                    moveDir = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left;
                    moveDir.Y *= -1f;
                }

                if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A))
                    isAPressed = true;
                else if (GamePad.GetState(PlayerIndex.One).IsButtonUp(Buttons.A) && isAPressed && (dummiesLeft >= 1))
                {
                    isAPressed = false;
                    Game.Components.Add(new Pickup(Game, tileLayer, this.position, "Textures\\monkey_Dummy"));
                    dummiesLeft--;
                }
            }

            // Calculate new position
            float moveVel = Settings.PlayerVel;
            Vector2 newPos = position + fDeltaTime * moveDir * moveVel;

            // Calculate edge points for current and next pos
            float fCollisionSize = 0.3f;
            Vector2 collisionSize = new Vector2(fCollisionSize, fCollisionSize);
            Vector2 positionMin = position - collisionSize;
            Vector2 positionMax = position + collisionSize;
            Vector2 nextPosMin = newPos - collisionSize;
            Vector2 nextPosMax = newPos + collisionSize;
            
            // Wall collision
            if (moveDir.X > 0)
            {
                bool bUpper = IsMoveable(nextPosMax.X, positionMin.Y);
                bool bLower = IsMoveable(nextPosMax.X, positionMax.Y);
                if (!bUpper || !bLower)
                {
                    newPos.X = (float)Math.Ceiling(newPos.X) - collisionSize.X - 0.01f;

                    if (IsMoveable(position.X + fCollisionSize, positionMin.Y) && moveDir.Y == 0.0f)
                    {
                        if (bUpper)
                            newPos.Y -= moveVel * fDeltaTime;
                        if (bLower)
                            newPos.Y += moveVel * fDeltaTime;
                    }
                }
            }

            if (moveDir.X < 0)
            {
                bool bUpper = IsMoveable(nextPosMin.X, positionMin.Y);
                bool bLower = IsMoveable(nextPosMin.X, positionMax.Y);
                if (!bUpper || !bLower)
                {
                    newPos.X = (float)Math.Floor(newPos.X) + collisionSize.X + 0.01f;
                    if (IsMoveable(position.X - fCollisionSize, positionMin.Y) && moveDir.Y == 0.0f)
                    {
                        if (bUpper)
                            newPos.Y -= moveVel * fDeltaTime;
                        if (bLower)
                            newPos.Y += moveVel * fDeltaTime;
                    }
                }
            }


            if (moveDir.Y > 0)
            {
                bool bLeft = IsMoveable(positionMin.X, nextPosMax.Y);
                bool bRight = IsMoveable(positionMax.X, nextPosMax.Y);
                if (!bLeft || !bRight)
                {
                    newPos.Y = (float)Math.Ceiling(newPos.Y) - collisionSize.Y - 0.01f;
                    if (IsMoveable(position.X, positionMin.Y + fCollisionSize) && moveDir.X == 0.0f)
                    {
                        if (bLeft)
                            newPos.X -= moveVel * fDeltaTime;
                        if (bRight)
                            newPos.X += moveVel * fDeltaTime;
                    }
                }
            }

            if (moveDir.Y < 0)
            {
                bool bLeft = IsMoveable(positionMin.X, nextPosMin.Y);
                bool bRight = IsMoveable(positionMax.X, nextPosMin.Y);
                if (!bLeft || !bRight)
                {
                    newPos.Y = (float)Math.Floor(newPos.Y) + collisionSize.Y + 0.01f;
                    if (IsMoveable(position.X, positionMin.Y - fCollisionSize) && moveDir.X == 0.0f)
                    {
                        if (bLeft)
                            newPos.X -= moveVel * fDeltaTime;
                        if (bRight)
                            newPos.X += moveVel * fDeltaTime;
                    }
                }
            }

            // Set position
            position = newPos;
        }

        bool IsMoveable(float x, float y)
        {
            return TileLayer.GetCellMovability((int)Math.Floor(x), (int)Math.Floor(y));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            // Render HUD
            int startX = 1200;
            int startY = 10;
            int spacing = 80;
            float scaling = 1.0f;
            for (int i = 0; i < maxDummyCount; i++)
            {
                Color col = i < dummiesLeft ? new Color(new Vector4(1.0f, 1.0f, 1.0f, 1.0f)) : new Color(new Vector4(1.0f, 1.0f, 1.0f, 0.3f));
                Rectangle rect = new Rectangle(startX - i * spacing, startY,(int)(scaling * (float)dummyTexture.Width),(int)(scaling * (float)dummyTexture.Height));
                spriteBatch.Draw(dummyTexture, rect, col);
            }

            {
                Color col = hasKey ? new Color(new Vector4(1.0f, 1.0f, 1.0f, 1.0f)) : new Color(new Vector4(1.0f, 1.0f, 1.0f, 0.3f));
                Rectangle rect = new Rectangle(startX - maxDummyCount * spacing, 10, (int)(scaling * (float)keyTexture.Width), (int)(scaling * (float)keyTexture.Height));
                spriteBatch.Draw(keyTexture, rect, col);
            }

            base.Draw(spriteBatch);
        }
    }
}