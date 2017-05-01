using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NordicGameJam2010
{
    /// <summary>
    /// Custom struct type, representing a rectangular shape
    /// </summary>
    public struct TextureType
    {
        /// <summary>
        /// Backing Store for Width
        /// </summary>
        private string texName;

        /// <summary>
        /// Width of rectangle
        /// </summary>
        public string TextureName
        {
            get
            {
                return texName;
            }
            set
            {
                texName = value;
            }
        }

        /// <summary>
        /// True if the texture is passable, False if Non-Passable
        /// </summary>
        private bool ispassable;

        /// <summary>
        /// Height of rectangle
        /// </summary>
        public bool IsPassable
        {
            get
            {
                return ispassable;
            }
            set
            {
                ispassable = value;
            }
        }

        public TextureType(string name, bool passability)
        {
            texName = name;
            ispassable = passability;
        }
    }
}
