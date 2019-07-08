using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Newtonsoft.Json;

namespace ClassLibraryCV.Prediction
{
    public class TechnologyLabelService
    {
        private readonly MLContext _mlContext;
        private PredictionEngine<TechnologyData, TechnologyPrediction> _predEngine;

        public TechnologyLabelService()
        {
            _mlContext = new MLContext(seed: 0);
            LoadModel("Model.zip");
        }

        public void LoadModel(string modelPath)
        {
            List<TechnologyData> trainingData = GetTrainingData();
            var trainingService = new TechnologyTrainingService();
            trainingService.Train(trainingData, modelPath);

            ITransformer loadedModel;
            using (var stream = new FileStream(modelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                loadedModel = _mlContext.Model.Load(stream, out var modelInputSchema);
            _predEngine = _mlContext.Model.CreatePredictionEngine<TechnologyData, TechnologyPrediction>(loadedModel);
        }

        public string PredictCategory(TechnologyData technologie, string description)
        {
            var prediction = new TechnologyPrediction();
            _predEngine.Predict(technologie, ref prediction);
            var scorePrediction = prediction.Score.Max();    //dégré de prediction de chaque techno

            if (scorePrediction < 0.90)
            {
                prediction.Category = description;
            }
            return prediction?.Category;
        }

        private static List<TechnologyData> GetTrainingData()
        {
            return JsonConvert.DeserializeObject<List<TechnologyData>>(System.Text.Encoding.Default.GetString(Properties.Resources.training));
        }
    }
}