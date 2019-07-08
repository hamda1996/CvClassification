using System.Runtime.Serialization;

namespace ClassLibraryCV.Prediction
{
    [DataContract]
    public class TechnologyData
    {
        [DataMember(Name = "desc")]
        public string Description { get; set; }

        [DataMember(Name = "category")]
        public string Category { get; set; }
    }
}