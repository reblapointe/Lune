using System;

namespace Lune
{
    class Program
    {
        const double PERIODE_LUNAIRE = 29.53;

        const int TAILLE_DESSIN = 21;

        // Les phases de la lune
        enum Phase
        {
            nouvelleLune, premierCroissant, premierQuartier, gibbeuseCroissante,
            pleineLune, gibbeuseDecroissante, dernierQuartier, dernierCroissant
        }

        const double LUMINOSITE_NOUVELLE = 0.04;
        const double LUMINOSITE_CROISSANT = 0.35;
        const double LUMINOSITE_QUARTIER = 0.66;
        const double LUMINOSITE_GIBBEUSE = 0.96;

        /// <summary>
        /// Calcule le numéro de jour julien d'une date. 
        /// Le jour julien d'une date est le nombre de jour qui la sépare de la date julienne (le 1e janvier -4713).
        /// Le calcul fonctionne pour toute date après le 23 novembre -4713.
        /// Pour une date invalide, la fonction retourne -1
        /// Le calcul provient de : https://en.wikipedia.org/wiki/Julian_day#Converting_Gregorian_calendar_date_to_Julian_Day_Number
        /// </summary>
        /// <param name="jour">Le jour d'une date</param>
        /// <param name="mois">Le mois d'une date</param>
        /// <param name="annee">L'année d'une date</param>
        /// <returns>Le jour julien pour une date</returns>
        static int JourJulien(int jour, int mois, int annee)
        {
            int jourJulien = -1;
            if (EstDateValide(jour, mois, annee))
                jourJulien = 1461 * (annee + 4800 + (mois - 14) / 12) / 4 + 367 * (mois - 2 - 12 * ((mois - 14) / 12)) / 12 -
                    3 * ((annee + 4900 + (mois - 14) / 12) / 100) / 4 + jour - 32075;
            return jourJulien;
        }

        ///// <summary>
        ///// Calcule la date du lendemain pour une date donnée
        ///// </summary>
        ///// <param name="jour">Le jour d'une date</param>
        ///// <param name="mois">Le mois d'une date</param>
        ///// <param name="annee">L'année d'une date</param>
        //static void Demain(ref int jour, ref int mois, ref int annee)
        //{
        //    if (EstDateValide(jour + 1, mois, annee))
        //    {
        //        jour++;
        //        return;
        //    }
        //    else if (EstDateValide(1, mois + 1, annee))
        //    {
        //        jour = 1;
        //        mois++;
        //        return;
        //    }
        //    else
        //    {
        //        jour = 1;
        //        mois = 1;
        //        annee++;
        //    }
        //    return;
        //}

        /// <summary>
        /// Détermine si une année est bissextile. 
        /// Ce calcul estime que l'année tropique dure 365.2425 jours
        /// </summary>
        /// <param name="annee">Une année</param>
        /// <returns>Vrai si l'année est bissextile, faux sinon</returns>
        static bool EstBissextile(int annee)
        {
            return annee % 400 == 0 || annee % 4 == 0 && annee % 100 != 0;
        }

        /// <summary>
                 /// Retourne le nombre de jours dans un mois donné. Si le mois n'existe pas, la fonction retourne 31.
                 /// </summary>
                 /// <param name="mois">Le mois</param>
                 /// <param name="annee">L'année </param>
                 /// <returns>Le nombre de jours que contient le mois demandé dans l'année demandée</returns>
        static int NbJoursDansMois(int mois, int annee)
        {
            switch (mois)
            {
                case 4: case 6: case 9: case 11:
                    return 30;
                case 2:
                    return EstBissextile(annee) ? 29 : 28;
                default:
                    return 31;
            }
        }

        /// <summary>
        /// Détermine si le jour, le mois et l'année donnée forment une date valide.
        /// Par exemple 42/-3/3000 est une date invalide, mais 29/2/2000 en est une.
        /// </summary>
        /// <param name="jour">Jour d'une date</param>
        /// <param name="mois">Mois d'une date</param>
        /// <param name="annee">Année d'une date</param>
        /// <returns></returns>
        static bool EstDateValide(int jour, int mois, int annee)
        {
            return jour >= 1 && jour <= NbJoursDansMois(mois, annee) && mois >= 1 && mois <= 12;
        }

        /// <summary>
        /// Calcule l'âge en jours de la lune pour une date. 
        /// Le cycle de la lune commence (jour 0) dans sa phase nouvelle lune.
        /// Le cycle de la lune a une longueur de 29.53 jours.
        /// </summary>
        /// <param name="jour">Le jour d'une date</param>
        /// <param name="mois">Le mois d'une date</param>
        /// <param name="annee">L'année d'une date</param>
        /// <returns>L'âge de la lune pour une date</returns>
        static double AgeLune(int jour, int mois, int annee)
        {
            int dateNouvelleLuneConnue = 2_459_228; // 13 janvier 2021;
            int joursDepuis = JourJulien(jour, mois, annee) - dateNouvelleLuneConnue;
            double nbNouvellesLunesDepuis = joursDepuis / PERIODE_LUNAIRE;
            double age = nbNouvellesLunesDepuis % 1 * PERIODE_LUNAIRE;
            if (age < 0)
                age += PERIODE_LUNAIRE;
            return age;
        }

        /// <summary>
        /// Détermine si la lune croit ou décroit selon son âge
        /// </summary>
        /// <param name="ageLune">L'âge de la lune en jours depuis la dernière nouvelle lune</param>
        /// <returns>Vrai si la lune croit, faux sinon</returns>
        static bool EstCroissante(double ageLune)
        {
            return ageLune < PERIODE_LUNAIRE / 2;
        }

        /// <summary>
        /// Détermine la proportion de la lune qui est éclairée pour un âge.
        /// La luminosité est un nombre à virgule situé entre 0 (complètement invisible) et 1 (complètement illuminée)
        /// </summary>
        /// <param name="ageLune">L'âge de la lune en jours depuis la dernière nouvelle lune</param>
        /// <returns>La luminosité de la lune pour un âge donné.</returns>
        static double Luminosite(double ageLune)
        {
            if (EstCroissante(ageLune))
                return ageLune / (PERIODE_LUNAIRE / 2);
            else
                return (PERIODE_LUNAIRE - ageLune) / (PERIODE_LUNAIRE / 2);
        }

        /// <summary>
        /// Trouve la phase de la lune pour un âge donné. Les phases de la lune sont, en ordre :
        /// nouvelle lune, premier croissant,  premier quartier, lune gibbeuse croissante, 
        /// pleine lune, lune gibbeuse décroissante, dernier quartier et dernier croissant.
        /// </summary>
        /// <param name="ageLune">L'âge de la lune en jours depuis la dernière nouvelle lune</param>
        /// <returns>Un entier représentant la phase de la lune pour l'âge donné.</returns>
        static Phase CalculerPhase(double ageLune)
        {
            Phase phase;
            double luminosite = Luminosite(ageLune);

            if (luminosite < LUMINOSITE_NOUVELLE)
                phase = Phase.nouvelleLune;
            else if (luminosite < LUMINOSITE_CROISSANT)
            {
                if (EstCroissante(ageLune))
                    phase = Phase.premierCroissant;
                else
                    phase = Phase.dernierCroissant;
            }
            else if (luminosite < LUMINOSITE_QUARTIER)
            {
                if (EstCroissante(ageLune))
                    phase = Phase.premierQuartier;
                else
                    phase = Phase.dernierQuartier;
            }
            else if (luminosite < LUMINOSITE_GIBBEUSE)
            {
                if (EstCroissante(ageLune))
                    phase = Phase.gibbeuseCroissante;
                else
                    phase = Phase.gibbeuseDecroissante;
            }
            else phase = Phase.pleineLune;
            return phase;
        }

        /// <summary>
        /// Détermine si un point est dans un cercle compris dans le premier cadran du plan
        /// et tangeant aux deux axes.
        /// </summary>
        /// <param name="rangee">Coordonnée en abscisse du point</param>
        /// <param name="colonne">Coordonnée en ordonnée du point</param>
        /// <param name="diametre">Diamètre du cercle.</param>
        /// <returns>Vrai si le point se situe dans le cercle, faux sinon.</returns>
        static bool EstDansCercle(int rangee, int colonne, int diametre)
        {
            double rayon = diametre / 2.0;
            double x = rangee - rayon;
            double y = colonne - rayon;
            return x * x + y * y < rayon * rayon;
        }

        /// <summary>
        /// Dessine la lune en fonction de son âge, telle qu'observée dans l'hémisphère nord.
        /// </summary>
        /// <param name="ageLune">L'âge de la lune en jours depuis la dernière nouvelle lune</param>
        static void DessinerLune(double ageLune)
        {
            DessinerLune(ageLune, true);
        }

        /// <summary>
        /// Dessine la lune en fonction de son âge, telle qu'observée dans l'hémisphère demandé.
        /// </summary>
        /// <param name="ageLune">L'âge de la lune en jours depuis la dernière nouvelle lune</param>
        /// <param name="hemisphereNord">Vrai si hémisphère nord, faux si hémishère sud.</param>
        static void DessinerLune(double ageLune, bool hemisphereNord)
        {
            double luminosite = Luminosite(ageLune);
            bool droiteIlluminee = EstCroissante(ageLune);
            if (!hemisphereNord)
                droiteIlluminee = !droiteIlluminee;

            for (int i = 0; i <= TAILLE_DESSIN; i++)
            {
                int largeur = 0;
                for (int j = 0; j < TAILLE_DESSIN; j++)
                    if (EstDansCercle(i, j, TAILLE_DESSIN))
                        largeur++;

                for (int j = 0; j <= TAILLE_DESSIN; j++)
                {
                    if (EstDansCercle(i, j, TAILLE_DESSIN))
                    {
                        double decalage = (TAILLE_DESSIN - largeur) / 2.0;
                        if (EstIlluminee(j - decalage, largeur, luminosite, droiteIlluminee))
                            Console.Write("██");
                        else
                            Console.Write("  ");
                    }
                    else
                        Console.Write("··");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Détermine si un point sur un dessin de la lune est illuminé.
        /// </summary>
        /// <param name="position">Position du point sur le dessin</param>
        /// <param name="largeur">Largeur du dessin</param>
        /// <param name="luminosite">Proportion de la lune qui est illuminée</param>
        /// <param name="droiteIlluminee">Vrai si la section droite de la lune est illuminé, 
        /// faux si la section gauche est éclairée.</param>
        /// <returns>Vrai si le point est illuminé, faux sinon</returns>
        static bool EstIlluminee(double position, int largeur, double luminosite, bool droiteIlluminee)
        {
            return droiteIlluminee && (largeur - position) / largeur < luminosite ||
                !droiteIlluminee && (position / largeur < luminosite);
        }

        /// <summary>
        /// Affiche l'était de la lune pour une date donnée
        /// </summary>
        /// <param name="jour">Jour d'une date</param>
        /// <param name="mois">Mois d'une date</param>
        /// <param name="annee">Année d'une date</param>
        private static void AfficherLune(int jour, int mois, int annee)
        {
            double ageLune = AgeLune(jour, mois, annee);
            string descriptionPhase = CalculerPhase(ageLune) switch
            {
                Phase.nouvelleLune => "Nouvelle lune",
                Phase.premierCroissant => "Premier croissant",
                Phase.premierQuartier => "Premier quartier",
                Phase.gibbeuseCroissante => "Gibbeuse croissante",
                Phase.pleineLune => "Pleine lune",
                Phase.gibbeuseDecroissante => "Gibbeuse décroissante",
                Phase.dernierQuartier => "Dernier quartier",
                Phase.dernierCroissant => "Dernier croissant",
                _ => "Pas une phase",
            };
            double luminosité = Luminosite(ageLune) * 100;
            Console.WriteLine($"En date du {jour}/{mois}/{annee}, à minuit heure locale.");
            Console.WriteLine($"La lune a {Math.Round(ageLune)} jour(s).");
            Console.WriteLine($"Elle est dans sa phase {descriptionPhase}. ({Math.Round(luminosité)}%)");
            DessinerLune(ageLune);
            Console.WriteLine();
        }

        /// <summary>
        /// Affiche l'état de la lune actuelle et pour toute date demandée par l'utilisateur
        /// Pour arrêter, l'utilisateur doit entrer Q. 
        /// </summary>
        static void Main()
        {
            int jour = DateTime.Now.Day;
            int mois = DateTime.Now.Month;
            int annee = DateTime.Now.Year;
            Console.WriteLine("Lune actuelle");
            AfficherLune(jour, mois, annee);

            bool valide;
            do
            {
                Console.WriteLine("-------------------------------------------------------------------");
                Console.WriteLine("Entrez une date pour voir l'état de la lune. Pour quitter, entrez Q");

                // Lire les trois nombres
                Console.WriteLine("Entrez un jour");
                valide = int.TryParse(Console.ReadLine(), out jour);
                Console.WriteLine("Entrez un mois");
                valide = valide && int.TryParse(Console.ReadLine(), out mois);
                Console.WriteLine("Entrez une annee");
                valide = valide && int.TryParse(Console.ReadLine(), out annee);

                if (EstDateValide(jour, mois, annee))
                    AfficherLune(jour, mois, annee);
                else
                    Console.WriteLine("Ceci n'est pas une date");
            } while (valide); // Sortir quand l'usager entre une lettre
            Console.WriteLine("Bye!");
        }
    }
}
