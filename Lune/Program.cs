using System;

namespace Lune
{
    class Program
    {
        // Les phases de la lune
        const short PHASE_DERNIER_QUARTIER = 6;
        const short PHASE_GIBBEUSE_CROISSANTE = 3;
        const short PHASE_DERNIER_CROISSANT = 7;
        const short PHASE_PREMIER_CROISSANT = 1;
        const short PHASE_PREMIER_QUARTIER = 2;
        const short PHASE_GIBBEUSE_DECROISSANTE = 5;
        const short PHASE_NOUVELLE_LUNE = 0;
        const short PHASE_PLEINE_LUNE = 4;
 
        /// <summary>
        /// Calcule le numéro de jour julien d'une date. 
        /// Le jour julien d'une date est le nombre de jour qui la sépare de la date julienne (le 1e janvier -4713).
        /// Le calcul fonctionne pour toute date après le 23 novembre -4713.
        /// Le calcul provient de : https://en.wikipedia.org/wiki/Julian_day#Converting_Gregorian_calendar_date_to_Julian_Day_Number
        /// </summary>
        /// <param name="jour">Le jour d'une date</param>
        /// <param name="mois">Le mois d'une date</param>
        /// <param name="annee">L'année d'une date</param>
        /// <returns>Le jour julien pour une date</returns>
        static int JourJulien(int jour, int mois, int annee)
        {
            int jourJulien = -1;
            
            jourJulien=1461*(annee+4800+(      mois-14 )/12) /4 +367*(mois-2-12 *(   ( mois -    14)/12))/12-
                3*   (   (   annee   +4900+(mois-14)/12)/100)/4+jour-32075;

            return jourJulien;
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
            double longueur;
            int dateNouvelleLuneConnue = JourJulien(13, 1, 2021); 
            int joursDepuis = JourJulien(jour, mois, annee) - dateNouvelleLuneConnue;
            double nbNouvellesLunesDepuis = joursDepuis / 29.53;
            double age = nbNouvellesLunesDepuis % 1 * 29.53;
            if (age >= 0)
            {

            }
            else
            {
                age += 29.53;
            }
            return age;

            age = 0;
        }

        /// <summary>
        /// Détermine si une année est bissextile. 
        /// Ce calcul estime que l'année tropique dure 365.2425 jours
        /// </summary>
        /// <param name="annee">Une année</param>
        /// <returns>Vrai si l'année est bissextile, faux sinon</returns>
        static bool EstBissextile(int annee)
        {
            return (annee % 400 == 0 || annee % 4 == 0 && annee % 100 != 0);
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
        /// Dessine la lune en fonction de son âge, telle qu'observée dans l'hémisphère demandé.
        /// </summary>
        /// <param name="ageLune">L'âge de la lune en jours depuis la dernière nouvelle lune</param>
        /// <param name="hemisphereNord">Vrai si hémisphère nord, faux si hémishère sud.</param>
        static void DessinerLune(double ageLune, bool hemisphereNord)
        {
            const int TAILLE_DESSIN = 21;
            double luminosite = Luminosite(ageLune);
            bool droiteEclairee = EstCroissante(ageLune);
            if (!hemisphereNord)
                droiteEclairee = !droiteEclairee;

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
                        if (EstIlluminee(j - decalage, largeur, luminosite, droiteEclairee))
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
        /// <param name="droiteEclairee">Vrai si la section droite de la lune est éclairée, faux sinon</param>
        /// <returns>Vrai si le point est illuminé, faux sinon</returns>
        static bool EstIlluminee(double position, int largeur, double luminosite, bool droiteEclairee)
        {
            return droiteEclairee && (largeur - position) / largeur < luminosite ||
                !droiteEclairee && (position / largeur < luminosite);
        }


        /// <summary>
        /// Retourne le nombre de jours dans un mois donné. Si le mois n'existe pas, la fonction retourne 31.
        /// </summary>
        /// <param name="annee">L'année </param>
        /// <param name="mois">Le mois</param>
        /// <returns>Le nombre de jours que contient le mois demandé dans l'année demandée</returns>
        static int NbJoursDansMois(int annee, int mois)
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
            if (jour >= 1) 
                if (jour <= NbJoursDansMois(annee, mois))
                    if (mois >= 1)
                        if (mois <= 12)
                            return true;
            return false;
        }

        /// <summary>
        /// Détermine si la lune croit ou décroit selon son âge
        /// </summary>
        /// <param name="ageLune">L'âge de la lune en jours depuis la dernière nouvelle lune</param>
        /// <returns>Vrai si la lune croit, faux sinon</returns>
        static bool EstCroissante(double ageLune)
        {
            if (ageLune < 29.53 / 2)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Calcule la date du lendemain pour une date donnée
        /// </summary>
        /// <param name="jour">Le jour d'une date</param>
        /// <param name="mois">Le mois d'une date</param>
        /// <param name="annee">L'année d'une date</param>
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
        /// Détermine la proportion de la lune qui est éclairée pour un âge.
        /// La luminosité est un nombre à virgule situé entre 0 (complètement invisible) et 1 (complètement illuminée)
        /// </summary>
        /// <param name="ageLune">L'âge de la lune en jours depuis la dernière nouvelle lune</param>
        /// <returns>La luminosité de la lune pour un âge donné.</returns>
        static double Luminosite(double ageLune)
        {
            if (EstCroissante(ageLune))
                return ageLune / (29.53 / 2);
            else
                return (29.53 - ageLune) / (29.53 / 2);
        }

        /// <summary>
        /// Trouve la phase de la lune pour un âge donné. Les phases de la lune sont, en ordre :
        /// nouvelle lune, premier croissant,  premier quartier, lune gibbeuse croissante, 
        /// pleine lune, lune gibbeuse décroissante, dernier quartier et dernier croissant.
        /// </summary>
        /// <param name="ageLune">L'âge de la lune en jours depuis la dernière nouvelle lune</param>
        /// <returns>Un entier représentant la phase de la lune pour l'âge donné.</returns>
        static int Phase(double ageLune)
        {
            int phase = 0;
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
else phase = PHASE_PLEINE_LUNE; 
                            return phase;
        }


        /// <summary>
        /// Affiche l'état de la lune actuelle et pour toute date demandée par l'utilisateur
        /// Pour arrêter, l'utilisateur doit entrer Q. 
        /// </summary>
        static void Main(string[] args)
        {
            Console.WriteLine("Lune actuelle");

            int jour = DateTime.Now.Day;
            int mois = DateTime.Now.Month;
            int annee = DateTime.Now.Year;

            double ageLune;
            int phase;
            string descriptionPhase;
            double l;

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
            l = Luminosite(ageLune);
            Console.WriteLine($"En date du {jour}/{mois}/{annee}, à minuit heure locale.");
            Console.WriteLine($"La lune a {Math.Round(ageLune)} jour(s).");
            Console.WriteLine($"Elle est dans sa phase {descriptionPhase}. ({Math.Round(l * 100)}%)");
            DessinerLune(ageLune);
            Console.WriteLine();

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

                if (EstDateValide(jour, mois, annee) == true)
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
                    l = Luminosite(ageLune);
                    Console.WriteLine($"En date du {jour}/{mois}/{annee}, à minuit heure locale.");
                    Console.WriteLine($"La lune a {Math.Round(ageLune)} jour(s).");
                    Console.WriteLine($"Elle est dans sa phase {descriptionPhase}. ({Math.Round(l * 100)}%)");
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
