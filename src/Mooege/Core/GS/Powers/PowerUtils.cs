using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mooege.Net.GS.Message.Fields;
using Mooege.Common.Helpers;
using Mooege.Core.GS.Common.Types.Math;

namespace Mooege.Core.GS.Powers
{
    static class PowerUtils
    {
        //Check if 2 position are within melee range
        public static bool isInMeleeRange(Vector3D agressorPos, Vector3D targetPos)
        {
            return distance(agressorPos, targetPos) > 12f;
        }

        //Roll dice !
        public static bool rollDice(int probability)
        {
            return RandomHelper.Next(0, 100) < probability;
        }

        //Distance between 2 vector
        public static float distance(Vector3D from, Vector3D to)
        {
            return (float)Math.Sqrt(Math.Pow(from.X - to.X, 2) + Math.Pow(from.Y - to.Y, 2) + Math.Pow(from.Z - to.Z, 2));
        }

        //Normal vector between 2 vector
        public static Vector3D getNormalVector(Vector3D from, Vector3D to)
        {
            float dist = distance(from, to);
            return new Vector3D((from.X - to.X) * (1f / dist), (from.Y - to.Y) * (1f / dist), (from.Z - to.Z) * (1f / dist));
        }

        //invert vector
        public static Vector3D invertVector(Vector3D v)
        {
            v.X *= -1;
            v.Y *= -1;
            v.Z *= -1;

            return v;
        }

        public static float getRadian(Vector3D from, Vector3D to)
        {
             return (float)Math.Atan2(from.Y - to.Y, from.X - to.X);
        }
           
    }
}
