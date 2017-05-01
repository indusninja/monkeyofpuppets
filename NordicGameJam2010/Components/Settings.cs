using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace NordicGameJam2010.Components
{
    class Settings
    {
        static float playerVel = 5.0f;
        public static float PlayerVel
        {
            get { return playerVel; }
        }

        static float enemyHuntVel = 10.0f;
        public static float EnemyHuntVel
        {
            get { return enemyHuntVel; }
        }

        static float enemyRoamVel = 10.0f;
        public static float EnemyRoamVel
        {
            get { return enemyRoamVel; }
        }

        static float killDuration = 10.0f;
        public static float KillDuration
        {
            get { return killDuration; }
        }

        static float returnDelay = 10.0f;
        public static float ReturnDelay
        {
            get { return returnDelay; }
        }

        public static void ReadSettings(String filename)
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();

                    if (string.IsNullOrEmpty(line))
                        continue;

                    string[] split = line.Split(' ');

                    if (split[0].Equals("playerVel"))
                        playerVel = float.Parse(split[1]);
                    else if (split[0].Equals("enemyHuntVel"))
                        enemyHuntVel = float.Parse(split[1]);
                    else if (split[0].Equals("enemyRoamVel"))
                        enemyRoamVel = float.Parse(split[1]);
                    else if (split[0].Equals("killDuration"))
                        killDuration = float.Parse(split[1]);
                    else if (split[0].Equals("returnDelay"))
                        returnDelay = float.Parse(split[1]);
                    else
                        Debug.WriteLine("Error: dont know property:" + split[0]);
                }
            }
        }
    }
}
