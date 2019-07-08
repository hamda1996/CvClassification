using System.Collections.Generic;

namespace ClassLibraryCV
{
    public class CvAnalysisResult
    {
        public CvAnalysisResult(List<CvQuality> listCvQuality, List<TechnoDuree> listTechnoDuree, string error)
        {
            ListCvQuality = listCvQuality;
            ListTechnoDuree = listTechnoDuree;
            Error = error;
        }

        public string Error { get; }
        public List<CvQuality> ListCvQuality { get; }
        public List<TechnoDuree> ListTechnoDuree { get; }
    }
}