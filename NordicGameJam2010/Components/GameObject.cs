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
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public abstract class GameObject : Microsoft.Xna.Framework.GameComponent
    {
        public Vector2 position;
        public TileLayer tileLayer;
        public Texture2D texture;
        public string textureName;
        bool isAlive = true;
        protected bool bDrawTexture = true;

        public Vector2 Position
        {
            get { return position; }
        }

        public GameObject(Game game, TileLayer tileLayer, Vector2 position, string textureName)
            : base(game)
        {
            UpdateOrder = 10;
            this.tileLayer = tileLayer;
            this.position = position;
            this.textureName = textureName;
        }

        public virtual void StartCombat()
        {
            isAlive = false;
        }

        public virtual void Kill()
        {
        }

        public virtual bool IsAlive()
        {
            return isAlive;
        }

        public virtual int TargetPriority()
        {
            return -1;
        }

        public virtual void Draw(SpriteBatch batch)
        {
            if (bDrawTexture)
            {
                Color col = new Color(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
                Rectangle rect = new Rectangle((int)(position.X * TileLayer.TileWidth - TileLayer.TileWidth * 0.5f + tileLayer.OffsetX), (int)(position.Y * TileLayer.TileHeight - TileLayer.TileHeight * 0.5f + tileLayer.OffsetY), TileLayer.TileWidth, TileLayer.TileHeight);
                batch.Draw(texture, rect, new Color(new Vector4(1.0f, 1.0f, 1.0f, 1.0f)));
            }
        }
    }
}