﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ctrl_Space
{
    class SpeedBonus : GameObject
    {
        public SpeedBonus(Vector2 position)
        {
            Speed = Vector2.Zero;
            Size = 15;
        }

        public override Microsoft.Xna.Framework.Graphics.Texture2D GetTexture(TextureManager textureManager)
        {
            return textureManager.SpeedBonusTexture;
        }
    }
}
