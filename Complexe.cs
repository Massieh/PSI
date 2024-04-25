using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSI
{
    internal class Complexe
    {
        private double re;
        private double im;

        public Complexe(double re, double im)
        {
            this.re = re;
            this.im = im;
        }
        public double Re
        {
            get { return re; }
            set { re = value; }
        }
        public double Im
        {
            get { return im; }
            set { im = value; }
        }
        public string toString()
        {
            string rep = re + " + i" + im;
            return rep;
        }
        public Complexe Addition(Complexe a)
        {
            return new Complexe(re + a.re, im + a.im);
        }

        public Complexe Produit(Complexe a)
        {
            return new Complexe (re * a.re - im * a.im, re * a.im + im * a.re);
        }
        public double Module()
        {
            return Math.Sqrt(re * re + im * im);
        }
    }
}
