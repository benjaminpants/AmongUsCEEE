using Rewired;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using UnityEngine;

namespace AmongUsCEEE
{
    [MoonSharpUserData]
    public struct ColorData //container for player cosmetics
    {
        [MoonSharpHidden]
        public Color32 Color = new Color32(255,255,255,255);

        [MoonSharpVisible(true)]
        public byte R { 
            get 
            {
                return Color.r;
            } 
            set 
            {
                Color.r = value;
            }
        }

        [MoonSharpVisible(true)]
        public byte G
        {
            get
            {
                return Color.g;
            }
            set
            {
                Color.g = value;
            }
        }

        [MoonSharpVisible(true)]
        public byte B
        {
            get
            {
                return Color.b;
            }
            set
            {
                Color.b = value;
            }
        }

        [MoonSharpVisible(true)]
        public byte A
        {
            get
            {
                return Color.a;
            }
            set
            {
                Color.a = value;
            }
        }

        public ColorData(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g; 
            B = b; 
            A = a;
        }

        public static explicit operator Color32(ColorData cd) => cd.Color;

    }

}
