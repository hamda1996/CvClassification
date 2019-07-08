using System;
using System.Collections.Generic;

namespace ClassLibraryCV
{
    public class TechnoDuree
    {
        public TechnoDuree(string nomFichier, string nomCollaborateur, string technologie, Intervalle intervalle)
        {
            NomFichier = nomFichier;
            NomCollaborateur = nomCollaborateur;
            Technologie = technologie;
            ListIntervalle = new List<Intervalle> { intervalle };
            Score = 0;
        }

        public int Duree { get; set; }
        public List<Intervalle> ListIntervalle { get; }
        public string NomCollaborateur { get; }
        public string NomFichier { get; }
        public double Score { get; set; }
        public string Technologie { get; }

        public void UpdateDuree()
        {
            Duree = 0;
            foreach (var date in ListIntervalle)
            {
                Duree += (date.DateEnd.Year - date.DateStart.Year) * 12 + date.DateEnd.Month - date.DateStart.Month + 1 + (date.DateEnd.Day >= date.DateStart.Day ? 0 : -1);
            }
        }

        public void UpdateListIntervalle()
        {
            ListIntervalle.Sort((d1, d2) => DateTime.Compare(d1.DateStart, d2.DateStart)); //tri croissant des dates de debut de la liste

            for (int i = 0; i < ListIntervalle.Count - 1; i++)
            {
                if (ListIntervalle[i].DateEnd >= ListIntervalle[i + 1].DateStart)
                {
                    if (ListIntervalle[i].DateEnd > ListIntervalle[i + 1].DateEnd)
                    {
                        ListIntervalle.RemoveAt(i + 1);
                        i--;
                    }
                    else
                    {
                        ListIntervalle[i].DateEnd = ListIntervalle[i + 1].DateEnd;
                        ListIntervalle.RemoveAt(i + 1);
                        i--;
                    }
                }
            }
        }
    }
}