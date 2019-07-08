using System;

namespace ClassLibraryCV
{
    internal class Scoring
    {
        public double ScoringCalculate(int duree, string techno, DateTime EndDate)
        {
            RecentTechno recentTechno = new RecentTechno();
            var poids = recentTechno.CalculRecentTechno(EndDate, techno);
            var poidsDuree = 0;

            if (duree > 0 && duree <= 12)
            {
                poidsDuree = 2;
            }
            else if (duree > 12 && duree < 24)
            {
                poidsDuree = 3;
            }
            else if (duree >= 24 && duree < 48)
            {
                poidsDuree = 5;
            }
            else if (duree >= 48 && duree < 72)
            {
                poidsDuree = 7;
            }
            else if (duree >= 72 && duree < 96)
            {
                poidsDuree = 8;
            }
            else if (duree >= 96 && duree < 120)
            {
                poidsDuree = 9;
            }
            else if (duree >= 120)
            {
                poidsDuree = 10;
            }

            return Math.Round(poids * poidsDuree, 2);
        }
    }
}