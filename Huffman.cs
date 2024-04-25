using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSI
{
    public class Huffman
    {
        public Noeud root { get; set; }
        public Dictionary<Pixel, int> dic_Frequence { get; set; }

        public Dictionary<Pixel, string> dic_Codes { get; set; }

        public Huffman(Dictionary<Pixel, int> f)
        {
            root = null;
            dic_Frequence = f;
        }

        public Huffman(MyImage entree)
        {
            Console.WriteLine("Etape 1");
            dic_Frequence = ObtenirFrequence(entree);
            Console.WriteLine("Etape 2");
            root = Arbre(dic_Frequence);
            Console.WriteLine("Etape 3");
            dic_Codes = new Dictionary<Pixel, string>();
            Genere_Code(root, "");
            Console.WriteLine("Etape 4");
        }

        private Dictionary<Pixel, int> ObtenirFrequence(MyImage entree)
        {
            Dictionary<Pixel, int> Dico = new Dictionary<Pixel, int>();
            for (int i = 0; i < entree.Hauteur; i++)
            {
                for (int j = 0; j < entree.Largeur; j++)
                {
                    if (Dico.ContainsKey(entree.Image[i, j]))
                    {
                        Dico[entree.Image[i, j]]++;
                    }
                    else
                    {
                        Dico.Add(entree.Image[i, j], 1);
                    }
                }
            }
            return Dico;
        }



        private Noeud Arbre(Dictionary<Pixel, int> f)
        {
            var Liste_Noeud = new List<Noeud>();

            //Ajouter chaque Pixel à la liste
            foreach (var X in f)
            {
                Liste_Noeud.Add(new Noeud(X.Key, X.Value));
            }

            //Tant qu'il reste plus d'un noeud dans la liste, fusionner les deux plus petit noeuds.
            while (Liste_Noeud.Count > 1)
            {
                //Fonction de tri de système trouvée sur internet par ordre croissant
                Liste_Noeud.OrderBy(x => x.frequence).ToList();
                //Prendre les deux noeuds avec les fréquences les plus basses
                var gauche = Liste_Noeud[0];
                var droite = Liste_Noeud[1];
                //Créer un nouveau noeud qui fusionne les deux noeuds, Il semblerait que la valeur initialisée ne change rien. On la laisse par défaut sur null donc.
                Noeud nouveveau = new Noeud(null, gauche.frequence + droite.frequence, gauche, droite);

                //Supprimer les anciens noeuds et ajouter le nouveau
                Liste_Noeud.Remove(gauche);
                Liste_Noeud.Remove(droite);
                Liste_Noeud.Add(nouveveau);

            }

            //Il ne reste plus qu'un seul noeud, c'est celui avec la plus haute fréquence
            return Liste_Noeud[0];
        }


        private void Genere_Code(Noeud n, string code)
        {

            if (n == null)
            {
                return;
            }

            if (n.left == null && n.right == null)
            {
                dic_Codes.Add(n.pix, code);
                return;
            }

            Genere_Code(n.left, code + "0");
            Genere_Code(n.right, code + "1");
        }

        public string EncodeImage(MyImage entree)
        {
            string str = "";
            for (int i = 0; i < entree.Hauteur; i++)
            {
                for (int j = 0; j < entree.Largeur; j++)
                {
                    Pixel p = entree.Image[i, j];
                    str += dic_Codes[p];
                }
            }
            return str;
        }
    }


}
