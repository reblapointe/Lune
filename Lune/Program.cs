using System;

namespace Lune
{


    // Nouvelle lune 2% --> 0% --> 2%
    // Premier croissant 3% --> 34%
    // Premier quartier 35% --> 65%
    // Lune gibbeuse croissante --> 66% à 96%
    // Pleine lune 97% --> 100% --> 97%
    // Lune gibbeuse décroissante --> 96% à 66%
    // Dernier quartier 65% --> 35%
    // Dernier croissant 34% --> 3%


    class Program
    {
        // REB : Rajouter de la duplication
        // Réparer l'indentation ctrl e -d
        // Renommer une variable
        // mettre en majuscule/minuscule

        // Augmenter/diminuer le retrait NON
        // Commenter/décommenter

        // Changer plusieurs lignes
        // Déplacer une ligne
        // CTRL-MAJ-V

        // Entourer de intellisense


        const double PERIODE_LUNAIRE = 29.53;


        // Suivre l'exécution (step over et step into) SEMAINE 7
        static int JourJulien(int jour, int mois, int annee)
        {
            // https://en.wikipedia.org/wiki/Julian_day#Converting_Gregorian_calendar_date_to_Julian_Day_Number

            return 1461 * (annee + 4800 + (mois - 14) / 12) / 4 +
                367 * (mois - 2 - 12 * ((mois - 14) / 12)) / 12 -
                3 * ((annee + 4900 + (mois - 14) / 12) / 100) / 4 +
                jour - 32075;
        }

        static double AgeLune(int jour, int mois, int annee)
        {
            int dateNouvelleLuneConnue = JourJulien(13, 1, 2021);
            int joursDepuis = JourJulien(jour, mois, annee) - dateNouvelleLuneConnue;
            double nbNouvellesLunesDepuis = joursDepuis / PERIODE_LUNAIRE;
            double age = nbNouvellesLunesDepuis % 1 * PERIODE_LUNAIRE;
            if (age < 0)
                age += PERIODE_LUNAIRE;
            return age;
        }

        static bool EstBissextile(int annee)
        {
            return annee % 400 == 0 || annee % 4 == 0 && annee % 100 != 0;
        }

        /// <summary>
        /// 
        /// Si le mois n'existe pas, la fonction retourne 31.
        /// </summary>
        /// <param name="mois"></param>
        /// <param name="annee"></param>
        /// <returns></returns>
        static int NbJoursDansMois(int mois, int annee)
        {
            switch (mois)
            {
                case 4:
                case 6:
                case 9:
                case 11:
                    return 30;
                case 2:
                    if (EstBissextile(annee))
                        return 29;
                    else
                        return 28;
                default:
                    return 31;
            }
        }

        static bool EstDateValide(int jour, int mois, int annee)
        {
            return jour >= 1 && jour <= NbJoursDansMois(mois, annee) && mois >= 1 && mois <= 12;
        }

        static bool EstCroissante(double ageLune)
        {
            return ageLune < PERIODE_LUNAIRE / 2;
        }

        static double Luminosite(double ageLune)
        {
            if (EstCroissante(ageLune))
                return ageLune / (PERIODE_LUNAIRE / 2);
            else
                return (PERIODE_LUNAIRE - ageLune) / (PERIODE_LUNAIRE / 2);
        }

        static string Phase(double ageLune)
        {
            string phase;
            double luminosite = Luminosite(ageLune);

            if (luminosite < 0.04)
                phase = "Nouvelle lune";
            else if (luminosite < 0.35)
            {
                if (EstCroissante(ageLune))
                    phase = "Premier croissant";
                else
                    phase = "Dernier croissant";
            }
            else if (luminosite < 0.66)
            {
                if (EstCroissante(ageLune))
                    phase = "Quartier croissant";
                else
                    phase = "Quartier décroissant";
            }
            else if (luminosite < 0.96)
            {
                if (EstCroissante(ageLune))
                    phase = "Lune gibbeuse croissant";
                else
                    phase = "Lune gibbeuse décroissant";
            }
            else
                phase = "Pleine lune";
            return phase;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Lune actuelle");

            int jour = DateTime.Now.Day;
            int mois = DateTime.Now.Month;
            int annee = DateTime.Now.Year;

            double ageLune = AgeLune(jour, mois, annee);
            string phase = Phase(ageLune);
            double luminosite = Luminosite(ageLune);
            Console.Write($"En date du {jour}/{mois}/{annee}, ");
            Console.Write($"la lune a {Math.Round(ageLune)} jour(s). ");
            Console.Write($"Elle est dans sa phase {phase}. ({Math.Round(luminosite * 100)}%)");
            Console.WriteLine();

            bool valide;
            Console.WriteLine("-------------------------------------------------------------------");
            Console.WriteLine("Entrez des dates pour voir l'âge de la lune. Pour quitter, entrez Q");
            do
            {
                // Lire les trois nombres
                Console.WriteLine("Entrez un jour");
                valide = int.TryParse(Console.ReadLine(), out jour);
                Console.WriteLine("Entrez un mois");
                valide = valide && int.TryParse(Console.ReadLine(), out mois);
                Console.WriteLine("Entrez une annee");
                valide = valide && int.TryParse(Console.ReadLine(), out annee);

                if (EstDateValide(jour, mois, annee))
                {
                    ageLune = AgeLune(jour, mois, annee);
                    phase = Phase(ageLune);
                    luminosite = Luminosite(ageLune);
                    Console.Write($"En date du {jour}/{mois}/{annee}, ");
                    Console.Write($"la lune a {Math.Round(ageLune)} jour(s). ");
                    Console.Write($"Elle est dans sa phase {phase}. ({Math.Round(luminosite * 100)}%)");
                    Console.WriteLine();
                }
                else
                    Console.WriteLine("Ceci n'est pas une date");
            } while (valide); // Sortir quand l'usager entre une lettre

            Console.WriteLine("Bye!");
        }
    }
}
