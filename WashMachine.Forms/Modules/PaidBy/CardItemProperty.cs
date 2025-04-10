﻿using System;
using System.Drawing;
using System.IO;

namespace WashMachine.Forms.Modules.PaidBy
{
    public class CardItemProperty
    {
        public string CoverImageBase64 { get; set; }
        public string Title { get; set; }
        public string BackgroundColor { get; set; }
        public string TitleColor { get; set; }
        public Image GetCoverImage()
        {
            byte[] bytes = Convert.FromBase64String(CoverImageBase64);
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                return Image.FromStream(ms);
            }
        }
    }
}
