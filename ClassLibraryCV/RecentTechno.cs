using System;

namespace ClassLibraryCV
{
    internal class RecentTechno
    {
        public float CalculRecentTechno(DateTime EndDate, string techno)
        {
            var calculLatestDate = CalculDiffBetweenTwoDates(DateTime.Now, EndDate);
            var poidsTechnoEvol = EvolutionTechno(techno);
            var poidsTechnoRecent = 0;

            if (calculLatestDate < 6)
            {
                poidsTechnoRecent = 10;
            }
            else if (calculLatestDate >= 6 && calculLatestDate < 12)
            {
                poidsTechnoRecent = 9;
            }
            else if (calculLatestDate >= 12 && calculLatestDate < 24)
            {
                poidsTechnoRecent = 7;
            }
            else if (calculLatestDate >= 24 && calculLatestDate < 48)
            {
                poidsTechnoRecent = 4;
            }
            else
            {
                poidsTechnoRecent = 2;
            }
            return poidsTechnoRecent * poidsTechnoEvol;
        }

        private int CalculDiffBetweenTwoDates(DateTime date1, DateTime date2)
        {
            int number = 0;
            number = (date1.Year - date2.Year) * 12 + date1.Month - date2.Month + 1 + (date1.Day >= date2.Day ? 0 : -1);

            return number;
        }

        private float EvolutionTechno(string techno)
        {
            var poidsTechnoEvol = 1.0f;
            string[] technoEvol = new[] { "C#", "Java", "VBA" };
            foreach (var tech in technoEvol)
            {
                if (techno == tech)
                {
                    poidsTechnoEvol = 0.5f;
                }
            }

            return poidsTechnoEvol;
        }
    }
}