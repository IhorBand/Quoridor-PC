using System;
using System.Collections;
using System.Collections.Generic;
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

        public int X { get; set; }
        public int Y { get; set; }
    }
}