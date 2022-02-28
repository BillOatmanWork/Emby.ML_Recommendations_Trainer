using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.ML;
using Microsoft.ML.Trainers;

namespace ML_Recommendations_Trainer
{
    public class NeuralNetwork
    {
        public void ExecuteTraining()
        {
            
            MLContext mlContext = new MLContext();
            
            (IDataView trainingDataView, IDataView testDataView) = LoadData(mlContext);

            ITransformer model = BuildAndTrainModel(mlContext, trainingDataView);

            EvaluateModel(mlContext, testDataView, model);

            SaveModel(mlContext, trainingDataView.Schema, model);

        }

        private (IDataView training, IDataView test) LoadData(MLContext mlContext)
        {
            //var appDataFolder    = Environment.GetEnvironmentVariable(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "AppData" : "Home" );
            var dataFolder       = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            var trainingDataPath = Path.Combine(dataFolder, "recommendation-ratings-train-master.csv");
            var testDataPath     = Path.Combine(dataFolder, "recommendation-ratings-test.csv");
            
            IDataView trainingDataView = mlContext.Data.LoadFromTextFile<MovieRating>(trainingDataPath, hasHeader: true, separatorChar: ',');
            IDataView testDataView = mlContext.Data.LoadFromTextFile<MovieRating>(testDataPath, hasHeader: true, separatorChar: ',');

            return (trainingDataView, testDataView);
        }

        ITransformer BuildAndTrainModel(MLContext mlContext, IDataView trainingDataView)
        {
            IEstimator<ITransformer> estimator = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "userIdEncoded", inputColumnName: "userId")
                .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "tmdbIdEncoded", inputColumnName: "tmdbId"));

            var options = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = "userIdEncoded",
                MatrixRowIndexColumnName = "tmdbIdEncoded",
                LabelColumnName = "Label",
                NumberOfIterations = 20,
                ApproximationRank = 100,

            };

            var trainerEstimator = estimator.Append(mlContext.Recommendation().Trainers.MatrixFactorization(options));
            Console.WriteLine("Training the model");
            ITransformer model = trainerEstimator.Fit(trainingDataView);

            return model;
        }

        private void EvaluateModel(MLContext mlContext, IDataView testDataView, ITransformer model)
        {
            Console.WriteLine("Evaluating the model");
            var prediction = model.Transform(testDataView);
            var metrics = mlContext.Regression.Evaluate(prediction, labelColumnName: "Label", scoreColumnName: "Score");
            Console.WriteLine("Root Mean Squared Error : " + metrics.RootMeanSquaredError);
            Console.WriteLine("RSquared: " + metrics.RSquared);
        }

        private void SaveModel(MLContext mlContext, DataViewSchema trainingDataViewSchema, ITransformer model)
        {
            var modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "MovieRecommenderModel.zip");
            Console.WriteLine("Saving the model to a file");
            mlContext.Model.Save(model, trainingDataViewSchema, modelPath);
        }

    }
}
