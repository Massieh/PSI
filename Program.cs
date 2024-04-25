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


namespace PSI
{
    internal class Program
    {
        static void Ouvrir_Fichier(string pic_name)
        {
            Console.WriteLine("\nOuverture du fichier '" + pic_name + "'...\n");
            Process.Start(pic_name);
        }

        static void Main(string[] args)
        {
            bool fin = false;
            string nom_image = "";
            string rep;
            string modifs = "";
            MyImage image;
            Console.ForegroundColor = ConsoleColor.Cyan;
            do
            {
                Console.Clear();
                Console.WriteLine("***Menu principal***\n\n\nQue souhaitez-vous faire ? (un entier est attendu)\n\nEdition d'image :\n0) saisir une autre image\n1) Coco\n2) Lac\n3) Lena\n4) Test\n\nGeneration de figures :\n5) Fractales");
                rep = Console.ReadLine();
                Console.Clear();
                switch (Convert.ToInt32(rep))
                {
                    case 0:
                        Console.WriteLine("Donnez moi le nom de l'image que vous souhaitez modifier (elle doit être au format .bmp et dans le dossier debug");
                        nom_image = Console.ReadLine();
                        break;
                    case 1:
                        nom_image = "coco";
                        break;
                    case 2:
                        nom_image = "lac";
                        break;
                    case 3:
                        nom_image = "lena";
                        break;
                    case 4:
                        nom_image = "test";
                        break;
                    case 5:
                        Console.Clear();
                        Console.WriteLine("***Génération de fractales de Mandelbrot***\n\nSaisir le nombre d'itérations n");
                        rep = Console.ReadLine();
                        image = new MyImage("test.bmp");
                        image.Fractale_Mandel(Convert.ToInt32(rep));
                        image.From_Image_To_File("Fractale_Mandel_" + rep + "_itérations");
                        Ouvrir_Fichier("Fractale_Mandel_" + rep + "_itérations.bmp");
                        Console.WriteLine("Génération terminée ! Taper n'importe qu'elle touche pour revenir au menu principal");
                        Console.ReadKey();
                        break;
                    default:
                        nom_image = "coco";
                        Console.WriteLine("Entier saisi invalide : image 'coco' sélectionnée \n\nTaper n'importe quelle touche pour continuer");
                        Console.ReadLine();
                        break;
                }
                if(nom_image != "")
                {
                    image = new MyImage(nom_image + ".bmp");
                    while (!fin)
                    {
                        Console.Clear();
                        Console.WriteLine("Vous avez sélectionné l'image '" + nom_image + "'. Liste des modifications disponibles :" +
                            "\n\n1 - Nuance de gris\n2 - Quart d'image\n3 - Aggrendissement des dimensions\n4 - Rotation\n5 - Flou\n6 - Détection des bords\n7 - Renforcement des bords\n8 - Repoussage\n9 - Effet miroir\n\nModifications déjà effectuées : " + modifs +"\n\nQuelle modification souhaitez-vous appliquer à cette image ? (un entier est attendu)");
                        rep = Console.ReadLine();
                        switch (Convert.ToInt32(rep))
                        {
                            case 1:
                                image.ToGrey();
                                modifs += "_Gris";
                                Console.WriteLine("Nuance de gris appliquée à '" + nom_image + "' !");
                                break;
                            case 2:
                                image.Crop_Coin();
                                modifs += "_Cropped";
                                Console.WriteLine("Recadrement quart d'image appliqué à '" + nom_image + "' !");
                                break;
                            case 3:
                                Console.WriteLine("Saisir le coefficient d'agrandissement des dimensions de l'image");
                                rep = Console.ReadLine();
                                image.Aggrandissement(Convert.ToInt32(rep));
                                modifs += "_Agrandi" + rep;
                                Console.WriteLine("Agrandissement de facteur " + rep + " appliqué à '" + nom_image + "' !");
                                break;
                            case 4:
                                Console.WriteLine("Saisir le degré de rotation de l'image");
                                rep = Console.ReadLine();
                                image.Rotation(Convert.ToInt32(rep));
                                modifs += "_Rotation" + rep + "d";
                                Console.WriteLine("Rotation de " + rep +"° appliquée à '" + nom_image + "' !");
                                break;
                            case 5:
                                image.Flou();
                                modifs += "_Flou";
                                Console.WriteLine("Flou appliqué à '" + nom_image + "' !");
                                break;
                            case 6:
                                image.Detection();
                                modifs += "_Detect";
                                Console.WriteLine("Détection des bords appliquée à '" + nom_image + "' !");
                                break;
                            case 7:
                                image.Renforcement();
                                modifs += "_Renforce";
                                Console.WriteLine("Renforcement des bords appliqué à '" + nom_image + "' !");
                                break;
                            case 8:
                                image.Repoussage();
                                modifs += "_Repousse";
                                Console.WriteLine("Repoussage appliqué à '" + nom_image + "' !");
                                break;
                            case 9:
                                Console.WriteLine("Souhaitez vous un effet miroir par rapport l'axe horizontal ou vertical ? (H/V)");
                                rep = Console.ReadLine().ToUpper().Trim();
                                image.Miroir(Convert.ToChar(rep));
                                modifs += "_Miroir" + rep;
                                Console.WriteLine("Effet miroir appliqué à '" + nom_image + "' !");
                                break;
                            default:
                                Console.WriteLine("Entier saisi invalide : aucun filtre n'a été appliqué");
                                break;
                        }
                        Console.WriteLine("\nSouhaitez-vous (un entier est attendu) :\n1 - continuer à modifier cette image\n2 - l'enregistrer en format bmp et revenir au menu principal");
                        rep = Console.ReadLine();
                        if (rep == "2")
                        {
                            image.From_Image_To_File(nom_image + modifs);
                            Ouvrir_Fichier(nom_image + modifs + ".bmp");
                            modifs = "";
                            fin = true;
                            nom_image = "";
                        }
                    }
                    fin = false;
                }
                Console.Clear();
                Console.WriteLine("***Menu principal***\n\n\nSouhaitez-vous quitter le programme ? (Y/N)");
                rep = Console.ReadLine().ToUpper().Trim();
                if(rep == "Y")
                {
                    fin = true;
                }
            }
            while (!fin);
            Console.Clear();
            Console.WriteLine("Programme terminé");

            MyImage image1 = new MyImage("lac.bmp");
            MyImage image2 = new MyImage("coco.bmp");
            image1.Stegano(image2);
            image1.From_Image_To_File("2imEnUne.bmp");
            image1.Decode_Stegano();
            image1.From_Image_To_File("apparu.bmp");

            //image1.Fractale_Mandel(40);
            //Ouvrir_Fichier("Fractale_Mandel_40_itérations.bmp");
            Console.ReadKey();
        }
    }
}
