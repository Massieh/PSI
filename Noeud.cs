using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSI
{
    public class Noeud
    {
        public Pixel pix { get; set; }
        public int frequence { get; set; }
        public Noeud left { get; set; }
        public Noeud right { get; set; }

        public Noeud(Pixel Pix, int Frequence, Noeud Left = null, Noeud Right = null)
        {
            pix = Pix;
            frequence = Frequence;
            left = Left;
            right = Right;
        }

        public static Noeud Add(Noeud racine, Pixel pixel, int frequence)
        {
            if (racine == null)
            {
                return new Noeud(pixel, frequence);
            }
            else if (frequence <= racine.frequence)
            {
                Noeud nouveau = new Noeud(pixel, frequence);
                nouveau.left = racine;
                return nouveau;
            }
            else
            {
                racine.right = Add(racine.right, pixel, frequence);
                return racine;
            }
        }

    }
}
