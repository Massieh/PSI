﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Media;
using System.ComponentModel;
using System.Diagnostics;


namespace PSI
{
    public class Pixel
    {
        private byte red;
        private byte green;
        private byte blue;
        public Pixel(byte red, byte green, byte blue)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }
        public byte Red
        { 
            get { return red; } 
            set { red = value; }
        }
        public byte Green
        { 
            get { return green; }
            set { green = value; }
        }
        public byte Blue
        { 
            get { return blue; }
            set { blue = value; }
        }
    }
}
