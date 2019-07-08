using Microsoft.ML.Data;

namespace ClassLibraryCV.Prediction
{
    public class TechnologyPrediction
    {
        public float[] Score { get; set; }

        [ColumnName("PredictedLabel")]
        public string Category { get; set; }
    }
}