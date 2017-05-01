using System;
using System.IO;
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

namespace NordicGameJam2010
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class NavMap
    {
        float[,] distanceMap;
        int playerTilePositionX = -1;
        int playerTilePositionY = -1;


        public void Allocate()
        {
            distanceMap = new float[TileLayer.Width, TileLayer.Height];
        }

        public void ClearMap()
        {
            for (int x = 0; x < TileLayer.Width; x++)
                for (int y = 0; y < TileLayer.Height; y++)
                    if (TileLayer.GetCellMovability(x, y))
                        distanceMap[x, y] = float.MaxValue;
                    else
                        distanceMap[x, y] = -1.0f;
        }

        public List<Vector2> MapPath(Vector2 start, Vector2 end)
        {
            int startX = (int)Math.Floor(start.X);
            int startY = (int)Math.Floor(start.Y);
            int endX = (int)Math.Floor(end.X);
            int endY = (int)Math.Floor(end.Y);
            CalculateWeightMap(endX, endY);

            List<Vector2> path = new List<Vector2>();

            RecursiveMapPath(path, startX, startY);

            return path;
        }


        void RecursiveMapPath(List<Vector2> path,int x,int y)
        {
            if(GetDistance(x, y) == 0.0f)
                return;

            int deltaX = 0;
            int deltaY = 0;
            float minDist = float.MaxValue;

            float dist;
            //dist = GetDistance(x-1, y-1);
            //if(dist < minDist)
            //{
            //    minDist = dist;
            //    deltaX = -1;
            //    deltaY = -1;
            //}

            dist = GetDistance(x, y - 1);
            if (dist < minDist)
            {
                minDist = dist;
                deltaX = 0;
                deltaY = -1;
            }

            //dist = GetDistance(x + 1, y - 1);
            //if (dist < minDist)
            //{
            //    minDist = dist;
            //    deltaX = 1;
            //    deltaY = -1;
            //}

            dist = GetDistance(x - 1, y );
            if (dist < minDist)
            {
                minDist = dist;
                deltaX = -1;
                deltaY = 0;
            }

            dist = GetDistance(x + 1, y);
            if (dist < minDist)
            {
                minDist = dist;
                deltaX = 1;
                deltaY = 0;
            }

            //dist = GetDistance(x - 1, y + 1);
            //if (dist < minDist)
            //{
            //    minDist = dist;
            //    deltaX = -1;
            //    deltaY = 1;
            //}

            dist = GetDistance(x, y + 1);
            if (dist < minDist)
            {
                minDist = dist;
                deltaX = 0;
                deltaY = 1;
            }

            //dist = GetDistance(x + 1, y + 1);
            //if (dist < minDist)
            //{
            //    minDist = dist;
            //    deltaX = 1;
            //    deltaY = 1;
            //}

            if ((deltaX != 0) || (deltaY != 0))
            {
                path.Add(new Vector2((float)x + deltaX + 0.5f, (float)y + deltaY + 0.5f));
                RecursiveMapPath(path, x + deltaX, y + deltaY);
            }
        }

        
        float GetDistance(int x, int y)
        {
            if ((x < 0) || (x >= TileLayer.Width) || (y < 0) || (y >= TileLayer.Height))
                return float.MaxValue;

            if (distanceMap[x, y] == -1)
                return float.MaxValue;
    
            return distanceMap[x, y];
        }

        public Vector2 CalculateWeightMap(int x,int y)
        {
            bool refresh = false;
            if (playerTilePositionX != x)
            {
                playerTilePositionX = x;
                refresh = true;
            }
            if (playerTilePositionY != y)
            {
                playerTilePositionY = y;
                refresh = true;
            }

            if (refresh)
            {
                ClearMap();

                RecursiveGenerate(x, y, 0);
            }

            //Save("NavData.txt");

            return Vector2.Zero;
        }

        void RecursiveGenerate(int x, int y,float dist)
        {
            if ((x < 0) || (x >= TileLayer.Width) || (y < 0) || (y >= TileLayer.Height))
                return;

            // Is this better than current position
            if (distanceMap[x, y] > dist)
            {
                distanceMap[x, y] = dist;

                float delta = 1.0f;
                RecursiveGenerate(x - 1, y, dist + delta);
                RecursiveGenerate(x + 1, y, dist + delta);
                RecursiveGenerate(x, y + 1, dist + delta);
                RecursiveGenerate(x, y - 1, dist + delta);

                delta = 1.3f;
                RecursiveGenerate(x - 1, y - 1, dist + delta);
                RecursiveGenerate(x + 1, y - 1, dist + delta);
                RecursiveGenerate(x - 1, y + 1, dist + delta);
                RecursiveGenerate(x + 1, y + 1, dist + delta);
            }
            
            return;
        }

        public void Save(string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.WriteLine("[Navigation Map]");

                writer.WriteLine("");

                for (int y = 0; y < distanceMap.GetLength(1); y++)
                {
                    string line = string.Empty;

                    for (int x = 0; x < distanceMap.GetLength(0); x++)
                    {
                        line += distanceMap[x, y].ToString() + "\t";
                    }

                    writer.WriteLine(line);
                }
            }
        }
    }
}