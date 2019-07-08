using System;
using System.IO;
using ClassLibraryCV;

namespace CvClassificationConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ClassLibraryCV.CvService cvService = new ClassLibraryCV.CvService();
            string file = @"C:\Users\khdiallo.stage\Downloads\CROUZET PASCAL - CV.docx";
            byte[] byteArray = File.ReadAllBytes(file);
            MemoryStream stream = new MemoryStream(byteArray);

            CvAnalysisResult technoDureeQuality;
            technoDureeQuality = cvService.TextFromWord(file, stream);

            foreach (var tech in technoDureeQuality.ListTechnoDuree)
            {
                // foreach (var date in technoDureeQuality.listCvQuality)
                {
                    //  Console.WriteLine("{0},{1},{2},{3}", date.nomfichier,date.type, date.erreur, date.details);
                    Console.WriteLine("{0},{1},{2}, {3}, {4}", tech.NomFichier, tech.NomCollaborateur, tech.Technologie, tech.Duree, tech.Score);
                }
            }

            // Some manually chosen transactions with some modifications.
            //    Console.WriteLine("Loading training data...");
            //    List<TechnologyData> trainingData = GetTrainingData();

            //    Console.WriteLine("Training the model...");
            //    var trainingService = new TechnologyTrainingService();
            //    trainingService.Train(trainingData, "Model.zip");

            //    Console.WriteLine("Prepare transaction labeler...");
            //    var labelService = new TechnologyLabelService();
            //    labelService.LoadModel("Model.zip");

            //    Console.WriteLine("Predict some transactions based on their description and type...");
            //    Console.WriteLine();

            //    MakePrediction(labelService, "VS 2018");

            //    MakePrediction(labelService, ".NET");

            //    MakePrediction(labelService, "SQL-SERVER 2007");
            //    MakePrediction(labelService, "git");

            //}

            //public static void MakePrediction(TechnologyLabelService labelService, string description)
            //{
            //    string prediction = labelService.PredictCategory(new TechnologyData
            //    {
            //        Description = description

            //    }, description);

            //    Console.WriteLine($"{description}  => {prediction}");
            //}

            //private static List<TechnologyData> GetTrainingData()
            //{
            //    return JsonConvert.DeserializeObject<List<TechnologyData>>(File.ReadAllText("training.json"));
            //}
        }
    }
}