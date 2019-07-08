namespace ClassLibraryCV
{
    public class CvQuality
    {
        public CvQuality(string nomFichier, string type, string erreur, string details)
        {
            NomFichier = nomFichier;
            Type = type;
            Erreur = erreur;
            Details = details;
        }

        public string Details { get; }
        public string Erreur { get; }
        public string NomFichier { get; }
        public string Type { get; }
    }
}