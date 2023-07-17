using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using System;
using System.Diagnostics.Metrics;
using WiitaMod.Prim;

namespace WiitaMod.Projectiles.Magic
{
    class CoralStaffPrimTrail : PrimTrail
    {
        public CoralStaffPrimTrail(Projectile projectile)
        {
            Entity = projectile;
            EntityType = projectile.type;
            DrawType = PrimTrailManager.DrawProjectile;
        }

        public override void SetDefaults()
        {
            AlphaValue = 0.9f;
            Cap = 80;
            Width = 10;
            Pixellated = true;
        }
        public override void PrimStructure(SpriteBatch spriteBatch)
        {
            /*if (_noOfPoints <= 1) return; //for easier, but less customizable, drawing
            float colorSin = (float)Math.Sin(_counter / 3f);
            Color c1 = Color.Lerp(Color.White, Color.Cyan, colorSin);
            float widthVar = (float)Math.Sqrt(_points.Count) * _width;
            DrawBasicTrail(c1, widthVar);*/

            if (PointCount <= 6) return;
            float widthVar;
            //float colorSin = (float)Math.Sin(Counter / 3f);
            for (int i = 0; i < Points.Count; i++)
            {
                if (i == 0)
                {
                    widthVar = (float)Math.Sqrt(Points.Count) * Width;
                    Color c1 = Color.LightGreen;
                    Vector2 normalAhead = CurveNormal(Points, i + 1);
                    Vector2 secondUp = Points[i + 1] - normalAhead * widthVar;
                    //Vector2 secondDown = Points[i + 1] + normalAhead * widthVar;
                    AddVertex(Points[i], c1 * AlphaValue, new Vector2(0, 0.5f));
                    AddVertex(secondUp, c1 * AlphaValue, new Vector2((float)(i + 1) / (float)Cap, 0));
                    AddVertex(secondUp, c1 * AlphaValue, new Vector2((float)(i + 1) / (float)Cap, 1));
                }
                else
                {
                    if (i != Points.Count - 1)
                    {
                        widthVar = (float)Math.Sqrt(i) * Width;
                        //Color base1 = new Color(7, 86, 122);
                        //Color base2 = new Color(255, 244, 173);
                        Color c = Color.LightGreen;
                        Color CBT = Color.LightGreen;
                        Vector2 normal = CurveNormal(Points, i);
                        Vector2 normalAhead = CurveNormal(Points, i + 1);
                        float j = (Cap + ((float)(Math.Sin(Counter / 10f)) * 1) - i * 0.1f) / Cap;
                        widthVar *= j;
                        Vector2 firstUp = Points[i] - normal * widthVar;
                        Vector2 firstDown = Points[i] + normal * widthVar;
                        Vector2 secondUp = Points[i + 1] - normalAhead * widthVar;
                        Vector2 secondDown = Points[i + 1] + normalAhead * widthVar;

                        AddVertex(firstDown, c * AlphaValue, new Vector2((i / Cap), 1));
                        AddVertex(firstUp, c * AlphaValue, new Vector2((i / Cap), 0));
                        AddVertex(secondDown, CBT * AlphaValue, new Vector2((i + 1) / Cap, 1));

                        AddVertex(secondUp, CBT * AlphaValue, new Vector2((i + 1) / Cap, 0));
                        AddVertex(secondDown, CBT * AlphaValue, new Vector2((i + 1) / Cap, 1));
                        AddVertex(firstUp, c * AlphaValue, new Vector2((i / Cap), 0));
                    }
                    else
                    {

                    }
                }
            }
        }
        public override void SetShaders()
        {
            Effect effect = WiitaMod.PrimitiveTextureMap;
            effect.Parameters["uTexture"].SetValue(ModContent.Request<Texture2D>("WiitaMod/Textures/GlowTrail", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);
            effect.Parameters["additive"].SetValue(true);
            effect.Parameters["intensify"].SetValue(true);
            PrepareShader(effect, "MainPS", Counter);
        }
        public override void OnUpdate()
        {
            if (Entity is not Projectile)
                return;

            Counter++;
            PointCount = Points.Count * 6;
            if (Cap < PointCount / 6)
            {
                Points.RemoveAt(0);
            }
            if ((!Entity.active && Entity != null) || Destroyed)
            {
                OnDestroy();
            }
            else
            {
                Points.Add(Entity.Center);
            }
        }
        public override void OnDestroy()
        {
            Destroyed = true;
            Width *= 0.93f;
            if (Width < 0.05f)
            {
                Dispose();
            }
        }
    }
}