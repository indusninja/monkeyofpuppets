using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NordicGameJam2010.Components;

namespace NordicGameJam2010
{
    public class TileLayer
    {
        static int tileWidth = 64;
        static int tileHeight = 64;

        static NavMap navMap = new NavMap();

        public static int TileWidth
        {
            get { return tileWidth; }
            set
            {
                tileWidth = (int)MathHelper.Clamp(value, 20.0f, 100.0f);
            }
        }

        public static int TileHeight
        {
            get { return tileHeight; }
            set
            {
                tileHeight = (int)MathHelper.Clamp(value, 20.0f, 100.0f);
            }
        }

        static List<Texture2D> tileTextures = new List<Texture2D>();
        static List<int> passableTiles = new List<int>();
        static List<int> nonpassableTiles = new List<int>();

        static int[,] map;

        float alpha = 1.0f;

        static Player player;
        public static Player Player
        {
            get { return player; }
            set { player = value; }
        }

        public float Alpha
        {
            get { return alpha; }
            set { alpha = MathHelper.Clamp(value, 0.0f, 1.0f); }
        }

        public int WidthInPixels
        {
            get
            {
                return Width * tileWidth;
            }
        }

        public int HeightInPixels
        {
            get
            {
                return Height * tileWidth;
            }
        }

        public static int Width
        {
            get { return map.GetLength(1); }
        }

        public static int Height
        {
            get { return map.GetLength(0); }
        }

        public int OffsetX
        {
            get
            {
                return (Game1.ScreenWidth - this.WidthInPixels) / 2;
            }
        }

        public int OffsetY
        {
            get
            {
                return (Game1.ScreenHeight - this.HeightInPixels) / 2 + 38;  // move down a bit to give space for decoy HUD
            }
        }

        public TileLayer(int width, int height)
        {
            map = new int[height, width];

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    map[y, x] = -1;
        }

        public TileLayer(int[,] existingMap)
        {
            map = (int[,])existingMap.Clone();
        }

        public static TileLayer FromFile(ContentManager content, string fileName)
        {
            TileLayer tileLayer;
            bool readingPassable = false;
            bool readingNonPassable = false;
            bool readingLevelMap = false;
            List<TextureType> textureNames = new List<TextureType>();
            List<List<int>> tempLayout = new List<List<int>>();
            List<int> passTiles = new List<int>();
            List<int> nonpassTiles = new List<int>();

            using (StreamReader reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();

                    if (string.IsNullOrEmpty(line))
                        continue;

                    if (line.Contains("[Passable]"))
                    {
                        readingPassable = true;
                        readingNonPassable = false;
                        readingLevelMap = false;
                    }
                    else if (line.Contains("[NonPassable]"))
                    {
                        readingPassable = false;
                        readingNonPassable = true;
                        readingLevelMap = false;
                    }
                    else if (line.Contains("[LevelMap]"))
                    {
                        readingPassable = false;
                        readingNonPassable = false;
                        readingLevelMap = true;
                    }
                    else if (line.Contains("[Character]") ||
                        line.Contains("[Enemy]") ||
                        line.Contains("[Pickups]"))
                    {
                        readingPassable = false;
                        readingNonPassable = false;
                        readingLevelMap = false;
                    }
                    else if (readingPassable || readingNonPassable)
                    {
                        textureNames.Add(new TextureType(line, readingPassable));
                    }
                    else if (readingLevelMap)
                    {
                        List<int> row = new List<int>();

                        string[] cells = line.Split(' ');

                        foreach (string c in cells)
                        {
                            if (!string.IsNullOrEmpty(c))
                                row.Add(int.Parse(c));
                        }

                        tempLayout.Add(row);
                    }
                }
            }

            int width = tempLayout[0].Count;
            int height = tempLayout.Count;

            tileLayer = new TileLayer(width, height);

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    tileLayer.SetCellIndex(x, y, tempLayout[y][x]);

            tileLayer.LoadTileTextures(content, textureNames);


            navMap.Allocate();
            navMap.CalculateWeightMap(0, 0);

            return tileLayer;
        }

        public void LoadTileTextures(ContentManager content, List<TextureType> textureNames)
        {
            Texture2D texture;

            for (int i = 0; i < textureNames.Count;i++ )
            {
                texture = content.Load<Texture2D>(textureNames[i].TextureName);
                tileTextures.Add(texture);
                if (textureNames[i].IsPassable)
                    passableTiles.Add(i);
                else
                    nonpassableTiles.Add(i);
            }
        }

        public void AddTexture(Texture2D texture)
        {
            tileTextures.Add(texture);
        }

        public void SetCellIndex(int x, int y, int cellIndex)
        {
            map[y, x] = cellIndex;
        }

        public static int GetCellIndex(int x, int y)
        {
            return map[y, x];
        }

        public static bool GetCellMovability(int x, int y)
        {
            if((x < 0) || (x >= Width) || (y < 0) || (y >= Height))
                return false;

            foreach (int index in passableTiles)
                if (index == GetCellIndex(x, y))
                    return true;
            return false;
        }

        public void Draw(SpriteBatch batch)
        {
            //batch.Begin(SpriteBlendMode.AlphaBlend);

            int tileMapWidth = map.GetLength(1);
            int tileMapHeight = map.GetLength(0);

            for (int x = 0; x < tileMapWidth; x++)
            {
                for (int y = 0; y < tileMapHeight; y++)
                {
                    int textureIndex = map[y, x];

                    if (textureIndex == -1)
                        continue;

                    Texture2D texture = tileTextures[textureIndex];

                    batch.Draw(
                        texture,
                        new Rectangle(
                            x * tileWidth + OffsetX,
                            y * tileHeight + OffsetY,
                            tileWidth,
                            tileHeight),
                            new Color(new Vector4(1.0f, 1.0f, 1.0f, Alpha)));
                }
            }

            //batch.End();
        }

        public static List<Vector2> MapPath(Vector2 start, Vector2 end)
        {
            return navMap.MapPath(start, end);
        }

        // Return true on collision
        public static bool RayTest(Vector2 startPos, Vector2 endPos)
        {
            for (int x = 0; x < TileLayer.Width; x++)
            {
                for (int y = 0; y < TileLayer.Height; y++)
                {
                    if (!TileLayer.GetCellMovability(x, y))
                    {
                        if (SegmentIntersectRectangle(new Vector2(x, y), new Vector2(x + 1, y + 1), startPos, endPos))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        static bool SegmentIntersectRectangle(Vector2 rectMin, Vector2 rectMax, Vector2 p1, Vector2 p2)
        {
            // Find min and max X for the segment

            double minX = p1.X;
            double maxX = p2.X;

            if (p1.X > p2.X)
            {
                minX = p2.X;
                maxX = p1.X;
            }

            // Find the intersection of the segment's and rectangle's x-projections
            if (maxX > rectMax.X)
            {
                maxX = rectMax.X;
            }

            if (minX < rectMin.X)
            {
                minX = rectMin.X;
            }

            if (minX > maxX) // If their projections do not intersect return false
            {
                return false;
            }

            // Find corresponding min and max Y for min and max X we found before
            double minY = p1.Y;
            double maxY = p2.Y;

            double dx = p2.X - p1.X;

            if (Math.Abs(dx) > 0.0000001)
            {
                double a = (p2.Y - p1.Y) / dx;
                double b = p1.Y - a * p1.X;
                minY = a * minX + b;
                maxY = a * maxX + b;
            }

            if (minY > maxY)
            {
                double tmp = maxY;
                maxY = minY;
                minY = tmp;
            }

            // Find the intersection of the segment's and rectangle's y-projections
            if (maxY > rectMax.Y)
            {
                maxY = rectMax.Y;
            }

            if (minY < rectMin.Y)
            {
                minY = rectMin.Y;
            }

            if (minY > maxY) // If Y-projections do not intersect return false
            {
                return false;
            }

            return true;
        }
    }
}
