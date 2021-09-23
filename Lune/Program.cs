using System;
using System.Threading;

namespace Lune
{
    class Program
    {
        // Réparer l'indentation ctrl e -d
        // Renommer une variable
        // mettre en majuscule/minuscule

        // Commenter/décommenter

        // Changer plusieurs lignes
        // Déplacer une ligne
        // CTRL-MAJ-V

        // Suivre l'exécution (step over et step into) SEMAINE 7
        // Entourer de intellisense


        const double PERIODE_LUNAIRE = 29.53;

        // Les phases de la lune
        const int PHASE_NOUVELLE_LUNE = 0;
        const int PHASE_PREMIER_CROISSANT = 1;
        const int PHASE_PREMIER_QUARTIER = 2;
        const int PHASE_GIBBEUSE_CROISSANTE = 3;
        const int PHASE_PLEINE_LUNE = 4;
        const int PHASE_GIBBEUSE_DECROISSANTE = 5;
        const int PHASE_DERNIER_QUARTIER = 6;
        const int PHASE_DERNIER_CROISSANT = 7;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jour"></param>
        /// <param name="mois"></param>
        /// <param name="annee"></param>
        /// <returns></returns>
        static int JourJulien(int jour, int mois, int annee)
        {
            // https://en.wikipedia.org/wiki/Julian_day#Converting_Gregorian_calendar_date_to_Julian_Day_Number

            return 1461 * (annee + 4800 + (mois - 14) / 12) / 4 +
                367 * (mois - 2 - 12 * ((mois - 14) / 12)) / 12 -
                3 * ((annee + 4900 + (mois - 14) / 12) / 100) / 4 +
                jour - 32075;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jour"></param>
        /// <param name="mois"></param>
        /// <param name="annee"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="annee"></param>
        /// <returns></returns>
        static bool EstBissextile(int annee)
        {
            return annee % 400 == 0 || annee % 4 == 0 && annee % 100 != 0;
        }

        /// <summary>
        /// Hémisphère nord
        /// </summary>
        /// <param name="ageLune"></param>
        static void DessinerLune(double ageLune)
        {
            DessinerLune(ageLune, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ageLune"></param>
        /// <param name="hemisphereNord"></param>
        static void DessinerLune(double ageLune, bool hemisphereNord)
        {
            const int TAILLE_DESSIN = 20;
            double luminosite = Luminosite(ageLune);
            bool croissant = EstCroissante(ageLune);
            if (!hemisphereNord)
                croissant = !croissant;

            Console.WriteLine();
            Console.Write("(");
            for (int i = 0; i < TAILLE_DESSIN; i++)
            {
                if (croissant && (TAILLE_DESSIN - i) / (double)TAILLE_DESSIN < luminosite)
                {
                    Console.Write("█");
                }
                else if (!croissant && (i / (double)TAILLE_DESSIN < luminosite))
                {
                    Console.Write("█");
                }
                else
                {
                    Console.Write(" ");
                }
            }
            Console.Write(")");
            Console.WriteLine();
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jour"></param>
        /// <param name="mois"></param>
        /// <param name="annee"></param>
        /// <returns></returns>
        static bool EstDateValide(int jour, int mois, int annee)
        {
            return jour >= 1 && jour <= NbJoursDansMois(mois, annee) && mois >= 1 && mois <= 12;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ageLune"></param>
        /// <returns></returns>
        static bool EstCroissante(double ageLune)
        {
            return ageLune < PERIODE_LUNAIRE / 2;
        }

        static void Demain(ref int jour, ref int mois, ref int annee)
        {
            if (EstDateValide (jour + 1, mois, annee))
            {
                jour++;
                return;
            }
            else if (EstDateValide(1, mois + 1, annee)) 
            {
                jour = 1;
                mois++;
                return;
            }
            else
            {
                jour = 1;
                mois = 1;
                annee++;
            }
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ageLune"></param>
        /// <returns></returns>
        static double Luminosite(double ageLune)
        {
            if (EstCroissante(ageLune))
                return ageLune / (PERIODE_LUNAIRE / 2);
            else
                return (PERIODE_LUNAIRE - ageLune) / (PERIODE_LUNAIRE / 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ageLune"></param>
        /// <returns></returns>
        static int Phase(double ageLune)
        {
            int phase;
            double luminosite = Luminosite(ageLune);

            if (luminosite < 0.04)
                phase = PHASE_NOUVELLE_LUNE;
            else if (luminosite < 0.35)
            {
                if (EstCroissante(ageLune))
                    phase = PHASE_PREMIER_CROISSANT;
                else
                    phase = PHASE_DERNIER_CROISSANT;
            }
            else if (luminosite < 0.66)
            {
                if (EstCroissante(ageLune))
                    phase = PHASE_PREMIER_QUARTIER;
                else
                    phase = PHASE_DERNIER_QUARTIER;
            }
            else if (luminosite < 0.96)
            {
                if (EstCroissante(ageLune))
                    phase = PHASE_GIBBEUSE_CROISSANTE;
                else
                    phase = PHASE_GIBBEUSE_DECROISSANTE;
            }
            else
                phase = PHASE_PLEINE_LUNE;
            return phase;
        }

        
        static void Main(string[] args)
        {
            Console.WriteLine("Lune actuelle");

            int jour = DateTime.Now.Day;
            int mois = DateTime.Now.Month;
            int annee = DateTime.Now.Year;

            double ageLune;
            int phase;
            string descriptionPhase;
            double luminosite;

            ageLune = AgeLune(jour, mois, annee);
            phase = Phase(ageLune);
            switch(phase)
            {
                case PHASE_NOUVELLE_LUNE: descriptionPhase = "Nouvelle lune"; break;
                case PHASE_PREMIER_CROISSANT: descriptionPhase = "Premier croissant"; break;
                case PHASE_PREMIER_QUARTIER: descriptionPhase = "Premier quartier"; break;
                case PHASE_GIBBEUSE_CROISSANTE: descriptionPhase = "Gibbeuse croissante"; break;
                case PHASE_PLEINE_LUNE: descriptionPhase = "Pleine lune"; break;
                case PHASE_GIBBEUSE_DECROISSANTE: descriptionPhase = "Gibbeuse décroissante"; break;
                case PHASE_DERNIER_QUARTIER: descriptionPhase = "Dernier quartier"; break;
                case PHASE_DERNIER_CROISSANT: descriptionPhase = "Dernier croissant"; break;
                default: descriptionPhase = "Pas une phase"; break;
            }
            luminosite = Luminosite(ageLune);
            Console.Write($"En date du {jour}/{mois}/{annee}, ");
            Console.Write($"la lune a {Math.Round(ageLune)} jour(s). ");
            Console.Write($"Elle est dans sa phase {descriptionPhase}. ({Math.Round(luminosite * 100)}%)");
            DessinerLune(ageLune);
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
                    switch (phase)
                    {
                        case PHASE_NOUVELLE_LUNE: descriptionPhase = "Nouvelle lune"; break;
                        case PHASE_PREMIER_CROISSANT: descriptionPhase = "Premier croissant"; break;
                        case PHASE_PREMIER_QUARTIER: descriptionPhase = "Premier quartier"; break;
                        case PHASE_GIBBEUSE_CROISSANTE: descriptionPhase = "Gibbeuse croissante"; break;
                        case PHASE_PLEINE_LUNE: descriptionPhase = "Pleine lune"; break;
                        case PHASE_GIBBEUSE_DECROISSANTE: descriptionPhase = "Gibbeuse décroissante"; break;
                        case PHASE_DERNIER_QUARTIER: descriptionPhase = "Dernier quartier"; break;
                        case PHASE_DERNIER_CROISSANT: descriptionPhase = "Dernier croissant"; break;
                        default: descriptionPhase = "Pas une phase"; break;
                    }
                    luminosite = Luminosite(ageLune);
                    Console.Write($"En date du {jour}/{mois}/{annee}, ");
                    Console.Write($"la lune a {Math.Round(ageLune)} jour(s). ");
                    Console.Write($"Elle est dans sa phase {descriptionPhase}. ({Math.Round(luminosite * 100)}%)");
                    DessinerLune(ageLune);
                    Console.WriteLine();
                }
                else
                    Console.WriteLine("Ceci n'est pas une date");
            } while (valide); // Sortir quand l'usager entre une lettre

            Console.WriteLine("Bye!");
        }
    }
}
