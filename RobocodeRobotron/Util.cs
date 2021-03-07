﻿using System;

using Robocode.Util;

namespace RC
{
    public static class Util
    {
        private static readonly Random RandomGen = new Random();

        public static (double, double) CalculateXYPos(double startX, double startY, double bearingRadians, double distance)
        {
            double enemyX = startX + distance * Math.Cos(bearingRadians);
            double enemyY = startY + distance * Math.Sin(bearingRadians);

            return (enemyX, enemyY);
        }

        public static double CalculateBearingRadians(double startX, double startY, double endX, double endY)
        {
            return Math.Atan2(endY - startY, endX - startX);
        }

        public static bool GetRandomBool()
        {
            return Convert.ToBoolean(RandomGen.Next() % 2);
        }

        public static double GetRandom()
        {
            return RandomGen.NextDouble();
        }

        public static double CalculateDistance(double startX, double startY, double endX, double endY)
        {
            double deltaX = endX - startX;
            double deltaY = endY - startY;

            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }
    }
}