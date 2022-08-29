using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace DTO
{
    [Serializable]
    public class Position
    {
        public Position()
        {
            this.X = 0;
            this.Y = 0;
        }

        public Position(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public override bool Equals(object other)
        {
            if ((other == null) 
                || !(other is Position))
            {
                return false;
            }

            return Equals((Position)other);
        }

        public bool Equals(Position other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ (this.Y.GetHashCode() << 2);
        }

        public int X { get; set; }
        public int Y { get; set; }
    }
}