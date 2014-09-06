﻿using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DND.Gui.Zen
{
    public static class ZenParams
    {
        public static readonly float HeaderHeight = 40;
        public static readonly float InnerPadding = 8;
        public static readonly Color BorderColor = Color.FromArgb(128, 128, 128);
        public static readonly Color PaddingBackColor = Color.Honeydew;
        public static readonly Color HeaderBackColor = Color.YellowGreen;
        public static readonly string HeaderTabFontFamily = "Segoe UI";
        public static readonly float HeaderTabFontSize = 12.0F;
        public static readonly float HeaderTabPadding = 12.0F;
        public static readonly string HeaderFontFamily = "Segoe UI";
        public static readonly float HeaderFontSize = 13.0F;
        public static readonly Color CloseColorBase = Color.FromArgb(199, 80, 80);
        public static readonly Color CloseColorHover = Color.FromArgb(224, 67, 67);
        public static readonly string GenericFontFamily = "Segoe UI";
        public static readonly float StandardFontSize = 12.0F;
        public static readonly Color StandardTextColor = Color.Black;
        public static readonly Color DisabledTextColor = Color.FromArgb(128, 128, 128);

        public static readonly Color BtnGradLightColor = Color.White;
        public static readonly Color BtnGradDarkColorBase = Color.FromArgb(175, 238, 238);
        public static readonly Color BtnGradDarkColorHover = Color.YellowGreen;
        public static readonly Color BtnGradDarkColorDisabled = Color.FromArgb(240, 240, 240);
        public static readonly Color BtnPressColor = Color.FromArgb(240, 128, 128);

        //public static readonly string ZhoFontFamily = "DFKai-SB";
        //public static readonly string ZhoFontFamily = "䡡湄楮札䍓ⵆ潮瑳";
        public static readonly string ZhoFontFamily = "Noto Sans S Chinese Regular";
        public static readonly float ZhoFontSize = 22.0F;
        public static readonly string PinyinFontFamily = "Tahoma";
        public static readonly float PinyinFontSize = 11.0F;
        public static readonly string LemmaFontFamily = "Segoe UI";
        public static readonly float LemmaFontSize = 10.0F;
        public static readonly float ResultsCountFontSize = 9.0F;
        public static readonly Color PinyinHiliteColor = Color.FromArgb(255, 232, 189);
        public static readonly Color HanziHiliteColor = Color.FromArgb(255, 232, 189);
        public static readonly float ZhoButtonFontSize = 18.0F;
    }
}
