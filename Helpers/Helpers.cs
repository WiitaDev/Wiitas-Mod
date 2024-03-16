using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;

namespace WiitaMod
{
    public static partial class Helper
    {
        internal static readonly List<Vector2> Directions = new List<Vector2>()
        {
            new Vector2(-1f, -1f),
            new Vector2(1f, -1f),
            new Vector2(-1f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0f, -1f),
            new Vector2(-1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 0f),
        };

        public static float PerlinNoise2D(float x, float y, int octaves, int seed)
        {
            float SmoothFunction(float n) => 3f * n * n - 2f * n * n * n;
            float NoiseGradient(int s, int noiseX, int noiseY, float xd, float yd)
            {
                int hash = s;
                hash ^= 1619 * noiseX;
                hash ^= 31337 * noiseY;

                hash = hash * hash * hash * 60493;
                hash = (hash >> 13) ^ hash;

                Vector2 g = Directions[hash & 7];

                return xd * g.X + yd * g.Y;
            }

            int frequency = (int)Math.Pow(2D, octaves);
            x *= frequency;
            y *= frequency;

            int flooredX = (int)x;
            int flooredY = (int)y;
            int ceilingX = flooredX + 1;
            int ceilingY = flooredY + 1;
            float interpolatedX = x - flooredX;
            float interpolatedY = y - flooredY;
            float interpolatedX2 = interpolatedX - 1;
            float interpolatedY2 = interpolatedY - 1;

            float fadeX = SmoothFunction(interpolatedX);
            float fadeY = SmoothFunction(interpolatedY);

            float smoothX = MathHelper.Lerp(NoiseGradient(seed, flooredX, flooredY, interpolatedX, interpolatedY), NoiseGradient(seed, ceilingX, flooredY, interpolatedX2, interpolatedY), fadeX);
            float smoothY = MathHelper.Lerp(NoiseGradient(seed, flooredX, ceilingY, interpolatedX, interpolatedY2), NoiseGradient(seed, ceilingX, ceilingY, interpolatedX2, interpolatedY2), fadeX);
            return MathHelper.Lerp(smoothX, smoothY, fadeY);
        }
    }
}