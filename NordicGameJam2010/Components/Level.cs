using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;

namespace NordicGameJam2010
{
    public enum PickupType
    {
        Key,
        Door,
        Dummy
    }

    public class Level
    {
        public List<TileLayer> Layers = new List<TileLayer>();

        public string playerTexture;
        public Vector2 playerStartPosition;
        public List<string> enemyTextures = new List<string>();
        public List<Vector2> enemyPositions = new List<Vector2>();
        public List<string> pickupTextures = new List<string>();
        public List<Vector2> pickupPositions = new List<Vector2>();
        int decoyCount = 3;

        public int DecoyCount
        {
            get { return decoyCount; }
        }

        public int GetWidthInPixels()
        {
            return GetWidth() * TileLayer.TileWidth;
        }

        public int GetHeightInPixels()
        {
            return GetHeight() * TileLayer.TileHeight;
        }

        public int GetWidth()
        {
            int width = 0;

            foreach (TileLayer layer in Layers)
                width = (int)Math.Max(width, TileLayer.Width);

            return width;
        }

        public int GetHeight()
        {
            int height = 0;

            foreach (TileLayer layer in Layers)
                height = (int)Math.Max(height, TileLayer.Height);

            return height;
        }

        public void ResetLevel(ContentManager content, string filename)
        {
            bool readingPlayer = false;
            bool readingEnemies = false;
            bool readingPickups = false;

            playerTexture = string.Empty;
            playerStartPosition = Vector2.Zero;
            enemyTextures = new List<string>();
            enemyPositions = new List<Vector2>();
            pickupTextures = new List<string>();
            pickupPositions = new List<Vector2>();

            using (StreamReader reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();

                    if (string.IsNullOrEmpty(line))
                        continue;

                    if (line.Contains("[Character]"))
                    {
                        readingPlayer = true;
                        readingEnemies = false;
                        readingPickups = false;
                    }
                    else if (line.Contains("[Enemy]"))
                    {
                        readingPlayer = false;
                        readingEnemies = true;
                        readingPickups = false;
                    }
                    else if (line.Contains("[Pickups]"))
                    {
                        readingPlayer = false;
                        readingEnemies = false;
                        readingPickups = true;
                    }
                    else if (line.Contains("[Passable]") ||
                        line.Contains("[NonPassable]") ||
                        line.Contains("[LevelMap]"))
                    {
                        readingPlayer = false;
                        readingEnemies = false;
                        readingPickups = false;
                    }
                    else if(line.Contains("decoyCount"))
                    {
                        string[] split = line.Split(' ');
                        decoyCount = int.Parse(split[1]);
                    }
                    else if (readingPlayer)
                    {
                        string[] cells = line.Split(' ');

                        if ((cells.Length == 3)&&
                            (!string.IsNullOrEmpty(cells[0]))&&
                            (!string.IsNullOrEmpty(cells[1]))&&
                            (!string.IsNullOrEmpty(cells[2])))
                        {
                            playerTexture = cells[0];

                            playerStartPosition = new Vector2(float.Parse(cells[1]), float.Parse(cells[2]));
                        }
                    }
                    else if (readingEnemies)
                    {
                        string[] cells = line.Split(' ');

                        if ((cells.Length == 3) &&
                            (!string.IsNullOrEmpty(cells[0])) &&
                            (!string.IsNullOrEmpty(cells[1])) &&
                            (!string.IsNullOrEmpty(cells[2])))
                        {
                            enemyTextures.Add(cells[0]);

                            enemyPositions.Add(new Vector2(float.Parse(cells[1]), float.Parse(cells[2])));
                        }
                    }
                    else if (readingPickups)
                    {
                        string[] cells = line.Split(' ');

                        if ((cells.Length == 3) &&
                            (!string.IsNullOrEmpty(cells[0])) &&
                            (!string.IsNullOrEmpty(cells[1])) &&
                            (!string.IsNullOrEmpty(cells[2])))
                        {
                            pickupTextures.Add(cells[0]);

                            pickupPositions.Add(new Vector2(float.Parse(cells[1]), float.Parse(cells[2])));
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (TileLayer layer in Layers)
                layer.Draw(spriteBatch);
        }
    }
}
