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
using System.Diagnostics;


namespace NordicGameJam2010.Components
{
    public class Enemy : GameObject
    {
        List<Vector2> path;
        int nextPathPoint;
        GameObject target;
        float countDown;
        Vector2 startPos;
        Vector2 targetPos;
        Texture2D beatingTexture;
        Texture2D puzzledTexture;

        enum State
        {
            Idle,
            Killing,
            Hunting,
            Returning,
        }
        State state;

        public Enemy(Game game, TileLayer tileLayer, Vector2 position, string textureName)
            : base(game, tileLayer, position, textureName)
        {
            UpdateOrder = 100;
            texture = Game.Content.Load<Texture2D>(textureName);
            beatingTexture = Game.Content.Load<Texture2D>("Textures/beating");
            puzzledTexture = Game.Content.Load<Texture2D>("Textures/questionmark");
            startPos = position;
            SetState(State.Idle);
        }

        public override void Initialize()
        {
            base.Initialize();

        }

        void SetState(State state)
        {
            Debug.WriteLine("state:" + state.ToString());
            this.state = state;

            switch (state)
            {
	            case State.Idle:
                    target = null;
                    if (Vector2.Distance(position, startPos) > 0.5f)
                        countDown = Settings.ReturnDelay;
                    else
                        countDown = float.MaxValue;
                    break;
                case State.Killing:
                    //Audio.State = AudioState.Kill;
                    countDown = Settings.KillDuration;
                    break;
                case State.Hunting:
                    Audio.State = AudioState.Angry;
                    path = null;
                    targetPos = target.position;
                    break;
                case State.Returning:
                    path = null;
                    targetPos = startPos;
                    break;
                default:
                    break;
            }
            
        }

        public override void Update(GameTime gameTime)
        {
            switch (state)
            {
	            case State.Idle:
                    IdleUpdate(gameTime);
                    if (TestKill())
                    {
                        SetState(State.Killing);
                    }
                    break;
                case State.Killing:
                    if (TestKill())
                    {
                        SetState(State.Killing);
                    }
                    KillUpdate(gameTime);
                    break;
                case State.Hunting:
                    target = UpdateTarget();
                    if (target != null)
                        targetPos = target.position;
                    if (MoveUpdate(gameTime, Settings.EnemyHuntVel))
                    {
                        if (TestKill())
                            SetState(State.Killing);
                    }
                    else
                        SetState(State.Idle);
                    break;
                case State.Returning:
                    target = UpdateTarget();
                    if (target != null)
                        SetState(State.Hunting);
                    else
                    {
                        MoveUpdate(gameTime, Settings.EnemyRoamVel);
                        if (TestKill())
                            SetState(State.Killing);
                    }
                    break;
                default:
                    break;
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch batch)
        {
            switch (state)
            {
                case State.Idle:
                    base.Draw(batch);

                     
                    // Show puzzled icon (when counting down for return)
                    if (countDown < Settings.ReturnDelay - 0.3)
                    {
                        Color col = new Color(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
                        Rectangle rect = new Rectangle((int)(position.X * TileLayer.TileWidth) - 0 + tileLayer.OffsetX, (int)(position.Y * TileLayer.TileHeight) + tileLayer.OffsetY - 50, puzzledTexture.Width, puzzledTexture.Height);
                        batch.Draw(puzzledTexture, rect, new Color(new Vector4(1.0f, 1.0f, 1.0f, 1.0f)));
                    }
                    break;
                case State.Killing:
                    {
                        Color col = new Color(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
                        Rectangle rect = new Rectangle((int)(position.X * TileLayer.TileWidth - TileLayer.TileWidth * 0.5f + tileLayer.OffsetX), (int)(position.Y * TileLayer.TileHeight - TileLayer.TileHeight * 0.5f + tileLayer.OffsetY), TileLayer.TileWidth, TileLayer.TileHeight);
                        batch.Draw(beatingTexture, rect, new Color(new Vector4(1.0f, 1.0f, 1.0f, 1.0f)));
                    }
                    break;
                default:
                    base.Draw(batch);
                    break;
            }

        }

        void IdleUpdate(GameTime gameTime)
        {
            // Search for targets
            target = UpdateTarget();

            if(target != null)
            {
                SetState(State.Hunting);
            }

            if (countDown != float.MaxValue)
            {
                countDown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (countDown <= 0.0f)
                {
                    SetState(State.Returning);
                }
            }
        }

        bool MoveUpdate(GameTime gameTime, float moveVel)
        {

            if (path == null)
            {
                Vector2 targetVec = targetPos - position;

                if (targetVec.Length() > 1.0f)
                {
                    path = TileLayer.MapPath(position, targetPos);
                    nextPathPoint = 0;
                    if (path.Count == 0)
                        path = null;
                }
                
                if(path == null)
                {
                    float moveDist = moveVel * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (targetVec.Length() > moveDist)
                    {
                        position += Vector2.Normalize(targetVec) * moveDist;
                    }
                    else
                    {
                        position = targetPos;
                        return false;
                    }
                }
            }

            if (path != null)
            {
                // Test that path is good enough
                if (Vector2.Distance(path[path.Count - 1], targetPos) > 1.0f)
                {
                    path = TileLayer.MapPath(position, targetPos);
                    nextPathPoint = 0;
                    Debug.Assert(path.Count > 0);
                }

                Vector2 currentPos = position;
                Vector2 pathPos = path[nextPathPoint];
                float moveDist = moveVel * (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Move along path
                Vector2 targetVec = pathPos - currentPos;
                while (targetVec.Length() < moveDist)
                {
                    moveDist -= targetVec.Length();
                    currentPos = path[nextPathPoint];
                    nextPathPoint += 1;
                    if (nextPathPoint == path.Count)
                        break;

                    pathPos = path[nextPathPoint];
                    targetVec = pathPos - position;
                }

                position = currentPos + Vector2.Normalize(targetVec) * moveDist;

                // If heading for last pos we delete path and move straight towards target.position
                if (nextPathPoint >= path.Count - 1)
                {
                    path = null;
                }
                else
                    position = currentPos + Vector2.Normalize(targetVec) * moveDist;
            }

            return true;
        }


        bool TestKill()
        {
            // Kill all enemy targets close
            foreach (GameObject gameObject in Game.Components)
            {
                if ((gameObject.TargetPriority() > 0) && (gameObject.IsAlive()))
                {
                    if (Vector2.Distance(gameObject.Position, position) < 0.5f)
                    {
                        gameObject.StartCombat();

                        // If new target kill current
                        if ((target != gameObject) && (target != null))
                            target.Kill();

                        target = gameObject;
                        return true;
                    }
                }
            }

            return false;
        }

        GameObject UpdateTarget()
        {
            float minTargetDist = float.MaxValue;
            int maxTargetPriority = 0;
            GameObject newTarget = null;

            // Kill all enemy targets close
            foreach (GameObject gameObject in Game.Components)
            {
                int targetPriority = gameObject.TargetPriority();
                if((targetPriority > 0) && gameObject.IsAlive())
                {
                    float targetDist = Vector2.Distance(gameObject.Position, position);
                    if (targetDist < minTargetDist)
                    {
                        if (!TileLayer.RayTest(position, gameObject.Position))
                        {
                            maxTargetPriority = targetPriority;
                            minTargetDist = targetDist;
                            newTarget = gameObject;
                        }
                    }
                }
            }
            return newTarget;
        }

        void KillUpdate(GameTime gameTime)
        {
            countDown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (countDown <= 0.0f)
            {
                if (target != null)
                {
                    target.Kill();
                }
         
                SetState(State.Idle);
            }
        }
    }
}