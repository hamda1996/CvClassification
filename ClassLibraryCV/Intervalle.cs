using System;

namespace ClassLibraryCV
{
    public class Intervalle
    {
        public Intervalle(DateTime dateStart, DateTime dateEnd)
        {
            DateStart = dateStart;
            DateEnd = dateEnd;
        }

        public DateTime DateEnd { get; set; }
        public DateTime DateStart { get; }
    }
}