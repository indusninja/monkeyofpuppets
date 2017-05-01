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
    public class Pickup : GameObject
    {
        public PickupType type;
        Texture2D deadDummyTexture;
       
        public Pickup(Game game, TileLayer tileLayer, Vector2 position, string textureName)
            : base(game, tileLayer, position, textureName)
        {
            UpdateOrder = -100;
            texture = Game.Content.Load<Texture2D>(textureName);
            deadDummyTexture = Game.Content.Load<Texture2D>("Textures/monkey_Dummy_broken");
            if (textureName.Contains("Key"))
                type = PickupType.Key;
            else if (textureName.Contains("Door"))
                type = PickupType.Door;
            else if (textureName.Contains("Dummy"))
                type = PickupType.Dummy;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void StartCombat()
        {
            bDrawTexture = false;
            base.StartCombat();
        }

        public override void Kill()
        {
            if (type == PickupType.Dummy)
            {
                texture = deadDummyTexture;
                // Disable dead dummy drawing
                //bDrawTexture = true;
                TileLayer.Player.DecoysLeft += 1;
            }
            base.Kill();
        }
        public override int TargetPriority()
        {
            if(type == PickupType.Dummy)
                return 1;
            return -1;
        }

        public override void Update(GameTime gameTime)
        {
            //if (this.position.Equals(TileLayer.Player.position))
            if (((this.position.X > TileLayer.Player.position.X - 0.2f) && (this.position.X < TileLayer.Player.position.X + 0.2f)) &&
                ((this.position.Y > TileLayer.Player.position.Y - 0.2f) && (this.position.Y < TileLayer.Player.position.Y + 0.2f)))
            {
                if (type == PickupType.Key)
                {
                    TileLayer.Player.HasKey = true;
                    Game.Components.Remove((IGameComponent)this);
                }
                if ((type == PickupType.Door) && (TileLayer.Player.HasKey))
                {
                    TileLayer.Player.HasFinishedLevel = true;
                    (Game as Game1).UpdateGameState(GameState.LevelWin);
                    Game.Components.Remove((IGameComponent)this);
                }
            }

            base.Update(gameTime);
        }
    }
}