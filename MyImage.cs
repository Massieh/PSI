using System;
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
using Microsoft.Win32;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace PSI
{
    public class MyImage
    {
        private string nom;
        private string pic_type;//BM or else
        private int taille;//taille du fichier en octets
        private int taille_offset;//taille de l'entête en octets
        private int largeur;//largeur image
        private int hauteur;//hauteur image
        private int nb_bits_couleur;
        private Pixel[,] image;

        public MyImage(string Nom, string Pic_type, int Taille, int Taille_offset, int Largeur, int Hauteur, int Nb_bits, Pixel[,] Image)//à partir de chaque élément
        {
            nom = Nom;
            pic_type = Pic_type;
            taille = Taille;
            taille_offset = Taille_offset;
            largeur = Largeur;
            hauteur = Hauteur;
            nb_bits_couleur = Nb_bits;
            image = Image;
        }
        public MyImage(string file_name)//à partir d'un fichier
        {
            Init_From_Image_File(file_name);
        }
        public void Init_From_Image_File(string myfile)
        {
            int k = 0;
            nom = "";
            do//remplissagee du nom
            {
                nom = nom + myfile[k];
                k++;
            }
            while (myfile[k] != '.');

            byte[] image_file = File.ReadAllBytes(myfile); //stockage du fichier de l'image myfile dans l'attribut image_file
            pic_type = "";
            if (image_file[0] == 66 && image_file[1] == 77)
            {
                pic_type = "bmp";  //Vérifier si 66 77 est une signature ou se traduit en BMP
            }
            //remplissage des attributs contenus dans le header
            taille = Convertir_Endian_To_Int(image_file, 2, 4);
            taille_offset = Convertir_Endian_To_Int(image_file, 14, 4);
            largeur = Convertir_Endian_To_Int(image_file, 18, 4);
            hauteur = Convertir_Endian_To_Int(image_file, 22, 4);
            nb_bits_couleur = Convertir_Endian_To_Int(image_file, 28, 2); //Demander à Aline si c'est bien ça

            //remplissage de la matrice de pixels "image"
            image = new Pixel[hauteur, largeur];
            k = 54;
            int j = 0;
            for (int i = 0; i < hauteur; i++)
            {
                while (j < largeur && k + 2 < image_file.Length)
                {
                    image[i, j] = new Pixel(image_file[k], image_file[k + 1], image_file[k + 2]);
                    k += 3;
                    j++;
                }
                j = 0;
            }
        }

        //propriétés de MyImage
        public string Nom
        {
            get { return nom; }
        }
        public string Pic_type
        {
            get { return pic_type; }
        }
        public int Taille
        {
            get { return taille; }
        }
        public int Taille_offset
        {
            get { return taille_offset; }
        }
        public int Largeur
        {
            get { return largeur; }
        }
        public int Hauteur
        {
            get { return hauteur; }
        }
        public int Nb_bits_couleurs
        {
            get { return nb_bits_couleur; }
        }
        public Pixel[,] Image
        {
            get { return image; }
        }
        public string Data_to_String()
        {
            string rep = "Nom de l'image : " + nom + "\nFormat : " + pic_type + "\nTaille du fichier (en octets) : " + taille + "\nTaille de l'offset (en octets) : " + taille_offset + "\nLargeur de l'image : " + largeur + "\nHauteur de l'image : " + hauteur + "\nNombre de bits par couleur : " + nb_bits_couleur;
            return rep;
        }

        public int verif_dim(int dimension)
        {
            int valid_dim = dimension;
            if (valid_dim % 4 != 0)//voir si on peut pas mettre une fonction de sécurité globale qui permet d'empêcher la présence de dimensions invalides
            {
                valid_dim += 4 - valid_dim % 4;
            }
            return valid_dim;
        }

    /*public void Init_MyImage()//initialise tous les attributs à partir de l'image en entrée
    {

    }*/
    public void From_Image_To_File(string file_name)//) prend une instance de MyImage et la transforme en fichier binaire respectant la structure du fichier.bmp
    {
        byte[] image_file = new byte[taille];
        image_file[0] = 66;
        image_file[1] = 77;
        image_file[10] = 54;
        image_file[26] = 1;
        byte[] X = Convertir_Int_To_Endian(taille);
        Inserer_Endian(image_file, X, 2, 4);
        X = Convertir_Int_To_Endian(taille_offset);
        Inserer_Endian(image_file, X, 14, 4);
        X = Convertir_Int_To_Endian(largeur);
        Inserer_Endian(image_file, X, 18, 4);
        X = Convertir_Int_To_Endian(hauteur);
        Inserer_Endian(image_file, X, 22, 4);
        X = Convertir_Int_To_Endian(nb_bits_couleur);
        Inserer_Endian(image_file, X, 28, 4);
        //Vérifier les bytes d'index 42 à 49 ?
        int k = 54;
        for (int i = 0; i < hauteur; i++)
        {
            for (int j = 0; j < largeur; j++)
            {
                image_file[k] = image[i, j].Red;
                image_file[k + 1] = image[i, j].Green;
                image_file[k + 2] = image[i, j].Blue;
                k = k + 3;
            }
        }
        File.WriteAllBytes(file_name + ".bmp", image_file);
    }
    public int Convertir_Endian_To_Int(byte[] tab, int IndexDépart, int Size)
        {
            int Somme = 0;
            for (int i = 0; i < Size; i++)
            {
                Somme = Somme + (tab[IndexDépart + i] * (int)(Math.Pow(256, i)));
            }
            return Somme;
        }

        /// <summary>
        /// permet de blabla
        /// </summary>
        /// <param name="val">etnier à convertir en endian</param>
        /// <returns>renvoie endian sous forme de tableau de byte</returns>
        static byte[] Convertir_Int_To_Endian(int val) //Je fais pas de vérif de sécurité car je pars du principe que la fonction est toujours utilisée correctement. Si t'en as besoin je te fais ça
        {
            //Si le nombre rentré en entrée est négatif tous les byte = 0:
            //Si le nombre rentré est >= à 2^32 il sera négatif car int
            byte[] X = new byte[4];
            int temporaire;
            X[0] = (byte)(val % 256);
            for (int i = 1; i < 4; i++)
            {
                temporaire = val - (val % (int)Math.Pow(256, i));
                temporaire = temporaire % (int)Math.Pow(256, i + 1);
                temporaire = temporaire / (int)Math.Pow(256, i);   //Décomposé en trois étapes pour que tu puisses relire facilement
                X[i] = (byte)temporaire;
            }
            return X;
        }

        /// <summary>
        /// Uwu le poulet
        /// </summary>
        public void ToGrey()
        {
            Pixel[,] grise = new Pixel[image.GetLength(0), image.GetLength(1)];
            byte gris;
            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    gris = Convert.ToByte((image[i, j].Red + image[i, j].Green + image[i, j].Blue) / 3);
                    grise[i, j] = new Pixel(gris, gris, gris);
                }
            }
            image = grise;
            //From_Image_To_File(nom+ "_ToGrey");
        }
        public void Inserer_Endian(byte[] Fichier, byte[] tab, int Index, int Duree)
        {
            for (int i = 0; i < Duree; i++)
            {
                Fichier[Index + i] = tab[i];
        }
    }
    public void Crop_Coin()
    {
        hauteur = hauteur / 2;
        largeur = largeur / 2;
        taille = 54 + 3 * (hauteur * largeur);
        Pixel[,] image_temp = new Pixel[hauteur, largeur];
        int j = 0;
        for (int i = 0; i < hauteur; i++)//remplissage de la matrice de pixels
        {
            while (j < largeur)
            {
                image_temp[i, j] = new Pixel(image[image.GetLength(0) - hauteur + i, j].Red, image[image.GetLength(0) - hauteur + i, j].Green, image[image.GetLength(0) - hauteur + i, j].Blue);
                j++;
            }
            j = 0;
        }
        image = image_temp;
        //From_Image_To_File(nom + "_Cropped");
    }
        public void Miroir(char axe)
        {
            Pixel[,] mat_miroir = image;
            int k;
            if(axe == 'H')
            {
                k = hauteur / 2 - 1;
                for(int i = hauteur / 2; i < hauteur; i++)
                {
                    for(int j = 0; j < largeur; j++)
                    {
                        mat_miroir[i, j] = image[k, j];
                    }
                    k--;
                }
            }
            else
            {
                k = largeur / 2 - 1;
                for(int i = 0; i < hauteur; i++)
                {
                    for(int j = largeur / 2; j < largeur; j++)
                    {
                        mat_miroir[i, j] = image[i, k];
                        k--;
                    }
                    k = largeur / 2 - 1;
                }
            }
            image = mat_miroir;
        }

        public void Aggrandissement(int Zoom)
        {
            hauteur = hauteur * Zoom;
            largeur = largeur * Zoom;
            taille = 54 + 3 * hauteur * largeur;
            Pixel[,] image_temp = new Pixel[hauteur, largeur];


            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    image_temp[i * Zoom, j * Zoom] = new Pixel(image[i, j].Red, image[i, j].Green, image[i, j].Blue);
                }
            }
            int compteur;
            for (int i = 0; i < hauteur; i = i + Zoom)
            {
                compteur = 0;
                for (int j = 0; j < largeur - Zoom + 1; j++)
                {
                    if (image_temp[i, j] == null)
                    {
                        compteur++;

                        image_temp[i, j] = new Pixel(0, 0, 0);

                        image_temp[i, j].Red = (byte)(image_temp[i, j - compteur].Red * (float)(Zoom - compteur) / Zoom);
                        image_temp[i, j].Red += (byte)(image_temp[i, j - compteur + Zoom].Red * (float)compteur / Zoom);

                        image_temp[i, j].Green = (byte)(image_temp[i, j - compteur].Green * (float)(Zoom - compteur) / Zoom);
                        image_temp[i, j].Green += (byte)(image_temp[i, j - compteur + Zoom].Green * (float)compteur / Zoom);

                        image_temp[i, j].Blue = (byte)(image_temp[i, j - compteur].Blue * (float)(Zoom - compteur) / Zoom);
                        image_temp[i, j].Blue += (byte)(image_temp[i, j - compteur + Zoom].Blue * (float)compteur / Zoom);
                    }
                    else
                    {
                        compteur = 0;
                    }
                }
            }
            compteur = 1;
            for (int i = 1; i < hauteur - Zoom + 1; i++)
            {
                for (int j = 0; j < largeur - Zoom + 1; j++)
                {
                    if (i % Zoom != 0)
                    {
                        image_temp[i, j] = new Pixel(0, 0, 0);

                        image_temp[i, j].Red = (byte)(image_temp[i - compteur, j].Red * (float)(Zoom - compteur) / Zoom);
                        image_temp[i, j].Red += (byte)(image_temp[i - compteur + Zoom, j].Red * (float)compteur / Zoom);

                        image_temp[i, j].Green = (byte)(image_temp[i - compteur, j].Green * (float)(Zoom - compteur) / Zoom);
                        image_temp[i, j].Green += (byte)(image_temp[i - compteur + Zoom, j].Green * (float)compteur / Zoom);

                        image_temp[i, j].Blue = (byte)(image_temp[i - compteur, j].Blue * (float)(Zoom - compteur) / Zoom);
                        image_temp[i, j].Blue += (byte)(image_temp[i - compteur + Zoom, j].Blue * (float)compteur / Zoom);

                    }
                    else
                    {
                        compteur = 0; j = largeur;
                    }
                }
                compteur++;
            }
            image = image_temp;
            hauteur = hauteur - Zoom - (4 - Zoom % 4);
            largeur = largeur - Zoom - (4 - Zoom % 4);
            image_temp = new Pixel[hauteur, largeur];
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {

                    image_temp[i, j] = image[i, j];

                }
            }
            image = image_temp;
        }
            public void Rotation(int degre)
        {
            //Coordonnées centre de la matrice d'origine
            int xc1 = (hauteur - 1) / 2;
            int yc1 = (largeur - 1) / 2;
            if (90 <= degre && degre < 180)
            {
                yc1 = (largeur) / 2;
            }
            else
            {
                if (180 <= degre)
                {
                    xc1 = (hauteur) / 2;
                    yc1 = (largeur) / 2;
                }
            }
            //Coordonnées dans matrice de départ déduites de celles de la matrice d'arrivée
            int x;
            int y;
            //conversion en radians et stockage du cos et du sin
            double cos = Math.Cos(degre * Math.PI / 180);
            double sin = Math.Sin(degre * Math.PI / 180);
            //création de la matrice de fond
            int hauteur_rotate = (int)(Math.Abs(hauteur * cos) + Math.Abs(largeur * sin));//voir si changer position abs change algo
            hauteur_rotate = verif_dim(hauteur_rotate);
            int largeur_rotate = (int)(Math.Abs(largeur * cos) + Math.Abs(hauteur * sin));
            largeur_rotate = verif_dim(largeur_rotate);
            //Coordonnées centre de la matrice d'arrivée
            int xc2 = (hauteur_rotate - 1) / 2;
            int yc2 = (largeur_rotate - 1) / 2;
            Pixel[,] rotate = new Pixel[hauteur_rotate, largeur_rotate];
            for (int i = 0; i < hauteur_rotate; i++)
            {
                for (int j = 0; j < largeur_rotate; j++)
                {
                    x = (int)Math.Round(cos * (i - xc2) + sin * (j - yc2) + xc1);
                    y = (int)Math.Round(cos * (j - yc2) - sin * (i - xc2) + yc1);
                    if (0 <= x && x < hauteur && 0 <= y && y < largeur)//on vérifie si ce pixel de la matrice d'arrivée correspond à un pixel de la matrice de départ
                    {
                        rotate[i, j] = new Pixel(image[x, y].Red, image[x, y].Green, image[x, y].Blue);
                    }
                    else//ce pixel est en dehors des dimensions de la matrice de départ on met donc un pixel de remplissage "gris perle"
                    {
                        rotate[i, j] = new Pixel(206, 206, 206);
                    }
                }
            }
            //changement des attributs avec ceux de la nouvelle image pivotée
            hauteur = hauteur_rotate;
            largeur = largeur_rotate;
            image = rotate;
            taille = 54 + 3 * hauteur_rotate * largeur_rotate;
            //From_Image_To_File(nom + "_Rotation_" + degre + "_degres");

        }

        public void Detection()
        {
            int[,] matrice = { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };
            Convolution(matrice, "_Detection");
        }
        public void Renforcement()
        {
            int[,] matrice = { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };
            //int[,] matrice = { { 0, 0, 0 },{ -1, 1, 0 },{ 0, 0, 0 } };
            Convolution(matrice, "_Renforcé");
        }
        public void Flou()
        {
            int[,] matrice = { { 1, 2, 1 }, { 2, 4, 2 }, { 1, 2, 1 } };
            Convolution(matrice, "_Flou");
        }
        public void Repoussage()
        {
            //int[,] matrice = { { 0, -1, -2 }, { -1,1, 1 }, { 2, 1, 0 } };
            //int[,] matrice = { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };
            int[,] matrice = { { -1, -1, -1 }, { -1, 9, -1 }, { -1, -1, -1 } };
            Convolution(matrice, "_Repoussé");
        }

        public void Convolution(int[,] matrice, string operation)
        {
            Pixel[,] transfo = new Pixel[hauteur + 2, largeur + 2];
            for (int i = 0; i < hauteur + 2; i++)
            {
                for (int j = 0; j < largeur + 2; j++)
                {
                    if (i > 0 && i < hauteur + 1 && j > 0 && j < largeur + 1)
                    {
                        transfo[i, j] = new Pixel(image[i - 1, j - 1].Red, image[i - 1, j - 1].Green, image[i - 1, j - 1].Blue);
                    }
                    else
                    {
                        transfo[i, j] = new Pixel(0, 0, 0);
                    }
                }
            }
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    //Calcul du nouveau pixel rouge
                    int temp = transfo[i, j].Red * matrice[0, 2] + transfo[i, j + 1].Red * matrice[0, 1] + transfo[i, j + 2].Red * matrice[0, 0];
                    temp += transfo[i + 1, j].Red * matrice[1, 2] + transfo[i + 1, j + 1].Red * matrice[1, 1] + transfo[i + 1, j + 2].Red * matrice[1, 0];
                    temp += transfo[i + 2, j].Red * matrice[2, 2] + transfo[i + 2, j + 1].Red * matrice[2, 1] + transfo[i + 2, j + 2].Red * matrice[2, 0];
                    if (operation == "_Flou")
                    {
                        temp = temp / 16;
                    }
                    //Si la valeur dépasse la plage autorisée on la tronque 
                    if (temp > 255)
                    {
                        image[i, j].Red = 255;
                    }
                    else if (temp < 0)
                    {
                        image[i, j].Red = 0;
                    }
                    else
                    {
                        image[i, j].Red = (byte)temp;
                    }
                    //Calcul du nouveau pixel bleu 
                    temp = transfo[i, j].Blue * matrice[0, 2] + transfo[i, j + 1].Blue * matrice[0, 1] + transfo[i, j + 2].Blue * matrice[0, 0];
                    temp += transfo[i + 1, j].Blue * matrice[1, 2] + transfo[i + 1, j + 1].Blue * matrice[1, 1] + transfo[i + 1, j + 2].Blue * matrice[1, 0];
                    temp += transfo[i + 2, j].Blue * matrice[2, 2] + transfo[i + 2, j + 1].Blue * matrice[2, 1] + transfo[i + 2, j + 2].Blue * matrice[2, 0];
                    if (operation == "_Flou")
                    {
                        temp = temp / 16;
                    }
                    //Si la valeur dépasse la plage autorisée on la tronque 
                    if (temp >= 255)
                    {
                        image[i, j].Blue = 255;
                    }
                    else if (temp < 0)
                    {
                        image[i, j].Blue = 0;
                    }
                    else
                    {
                        image[i, j].Blue = (byte)temp;
                    }
                    //Calcul du nouveau pixel vert
                    temp = transfo[i, j].Green * matrice[0, 2] + transfo[i, j + 1].Green * matrice[0, 1] + transfo[i, j + 2].Green * matrice[0, 0];
                    temp += transfo[i + 1, j].Green * matrice[1, 2] + transfo[i + 1, j + 1].Green * matrice[1, 1] + transfo[i + 1, j + 2].Green * matrice[1, 0];
                    temp += transfo[i + 2, j].Green * matrice[2, 2] + transfo[i + 2, j + 1].Green * matrice[2, 1] + transfo[i + 2, j + 2].Green * matrice[2, 0];
                    if (operation == "_Flou")
                    {
                        temp = temp / 16;
                    }
                    //Si la valeur dépasse la plage autorisée on la tronque 
                    if (temp > 255)
                    {
                        image[i, j].Green = 255;
                    }
                    else if (temp < 0)
                    {
                        image[i, j].Green = 0;
                    }
                    else
                    {
                        image[i, j].Green = (byte)temp;
                    }
                }
            }
            //From_Image_To_File(nom + operation);
        }

        private int Appartient_A_Fractale(Complexe c, int n)
        {
            int nb_iterations = 0;
            Complexe z = new Complexe(0, 0);
            while (nb_iterations < n && z.Module() <= 2)
            {
                z = z.Produit(z);
                z = z.Addition(c);      // zn+1 = zn^2 + c : https://en.wikipedia.org/wiki/Mandelbrot_set
                nb_iterations++;
            }
            return nb_iterations;
        }

        public void Fractale_Mandel(int n)
        {
            int hauteur_f = 2000;
            int largeur_f = 2000;
            byte k = 0;
            Complexe coord_complexes = new Complexe (0,0);
            Pixel[,] mat_mandel = new Pixel[largeur_f, hauteur_f];
            for(int i = 0; i < hauteur_f; i++)
            {
                for(int j = 0; j < largeur_f; j++)
                {
                    //passage coordonnées complexes
                    coord_complexes.Re = (j - (largeur_f / 2.0)) / (largeur_f / 3.0);
                    coord_complexes.Im = (i - (hauteur_f / 2.0)) / (hauteur_f / 3.0);
                    if(Appartient_A_Fractale(coord_complexes, n) == n)
                    {
                        mat_mandel[i, j] = new Pixel(0, 0, 0);
                    }
                    else
                    {
                        if(k < 255)
                        {
                            k = (byte)(90 + 120 * Appartient_A_Fractale(coord_complexes, n)/n);
                        }
                        mat_mandel[i, j] = new Pixel((byte)(255 - k/2), k, (byte)(255 + k / 2));
                    }
                }
            }
            hauteur = hauteur_f;
            largeur = largeur_f;
            image = mat_mandel;
            taille = 54 + 3 * hauteur * largeur;
            //From_Image_To_File("Fractale");

        }
        public void Stegano(MyImage a)
        {
            Pixel[,] mat_stegano;
            Pixel[,] mat_cachee = null;
            if (a.Largeur <= largeur && a.hauteur <= hauteur)
            {
                mat_stegano = image;
                mat_cachee = a.Image;
            }
            else
            {
                if (a.Largeur >= largeur && a.hauteur >= hauteur)
                {
                    mat_stegano = a.Image;
                    mat_cachee = image;
                }
                else
                {
                    mat_stegano = null;
                    Console.WriteLine("L'image " + a.Nom + " ne tient pas dans les dimensions de l'image " + nom);
                }
            }
            if (mat_stegano != null)
            {
                for (int i = 0; i < mat_cachee.GetLength(0); i++)
                {
                    for (int j = 0; j < mat_cachee.GetLength(1); j++)
                    {
                        //
                        mat_stegano[i, j].Red = (byte)((mat_stegano[i, j].Red & 0b11111100) | ((mat_cachee[i, j].Red & 0b11000000) >> 6));
                        mat_stegano[i, j].Green = (byte)((mat_stegano[i, j].Green & 0b11111100) | ((mat_cachee[i, j].Green & 0b11000000) >> 6));
                        mat_stegano[i, j].Blue = (byte)((mat_stegano[i, j].Blue & 0b11111100) | ((mat_cachee[i, j].Blue & 0b11000000) >> 6));
                    }
                }
                hauteur = mat_stegano.GetLength(0);
                largeur = mat_stegano.GetLength(1);
                image = mat_stegano;
                taille = 54 + 3 * hauteur * largeur;
            }
        }
        public void Decode_Stegano()
        {
            Console.WriteLine("Décodage de l'image");
            for(int i = 0; i < hauteur; i++)
            {
                for(int j = 0; j < largeur; j++)
                {
                    //on remplace les 4 most significant bits de chaque couleur par les 4 least significant et on remplace les 4 least significant par 0
                    image[i, j].Red = (byte)((image[i, j].Red & 0b00000011) << 6);
                    image[i, j].Green = (byte)((image[i, j].Green & 0b00000011) << 6);
                    image[i, j].Blue = (byte)((image[i, j].Blue & 0b00000011) << 6);
                }
            }
        }

    }
}
