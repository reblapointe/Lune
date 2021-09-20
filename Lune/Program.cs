using System;

namespace Lune
{
    class Program
    {
        static int JourJulien(int jour, int mois, int annee)
        {
            // https://en.wikipedia.org/wiki/Julian_day#Converting_Gregorian_calendar_date_to_Julian_Day_Number

            return 1461 * (annee + 4800 + (mois - 14) / 12) / 4 + 
                367 * (mois - 2 - 12 * ((mois - 14 ) / 12)) / 12 - 
                3 * ((annee + 4900 + (mois - 14) / 12) / 100) / 4 + 
                jour - 32075;
        }

        static double AgeLune(int jour, int mois, int annee)
        {
            double DATE_JULIENNE_NOUVELLE_LUNE = JourJulien(6, 1, 2000); // Première nouvelle lune de l'an 2000
            const double PERIODE_LUNAIRE = 29.53;
            double jd = JourJulien(jour, mois, annee);
            double jourDepuisNouvelleLune = jd - DATE_JULIENNE_NOUVELLE_LUNE;
            double nbNouvellesLunes = jourDepuisNouvelleLune / PERIODE_LUNAIRE;
            double reste = nbNouvellesLunes % 1;
            return reste * PERIODE_LUNAIRE;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Lune actuelle : ");
            Console.WriteLine("Dernière pleine lune :");
            Console.WriteLine("Dernière pleine lune :");
            Console.WriteLine("Entrez une date");
            Console.WriteLine("À cette date, la lune est : ");

            Console.WriteLine(AgeLune(1, 3, 2017));
            Console.WriteLine(AgeLune(20, 9, 2021));
            Console.WriteLine(JourJulien(19, 9, 2021));
        }
           
    }
}
