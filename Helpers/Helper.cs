using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using ReLogic.Graphics;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Utilities;

namespace WiitaMod
{
    public static partial class Helper
    {
        public static Vector3 Vec3(this Vector2 vector)
        {
            return new Vector3(vector.X, vector.Y, 0);
        }
    }
}