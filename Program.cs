using System;
using System.IO;
using System.Threading;

namespace ML_Recommendations_Trainer
{
    class Program
    {
        private static Timer TrainingTaskTimer = new Timer(TrainNeuralNetworkModel);
        public static void Main(string[] args)
        {
            var submissionDataFolder = Path.Combine(Directory.GetCurrentDirectory(), "Data");

            if (!Directory.Exists(submissionDataFolder))
            {
                Directory.CreateDirectory(submissionDataFolder);
            }

            var server = new SubmissionServer();
            server.StartServer();
            //Train the model every 12 hours
           
            TrainingTaskTimer.Change((int) TimeSpan.FromHours(12).TotalMilliseconds, (int) TimeSpan.FromHours(12).TotalMilliseconds);
            //var organizeData = new DataOrganization();
            //organizeData.Organize(submissionDataFolder);

            //var nn = new NeuralNetwork();
            //nn.ExecuteTraining();
            Console.ReadLine();
        }

        private static void TrainNeuralNetworkModel(object sender)
        {
            TrainingTaskTimer.Change(Timeout.Infinite, Timeout.Infinite);
            var nn = new NeuralNetwork();
            nn.ExecuteTraining();
            TrainingTaskTimer.Change((int) TimeSpan.FromHours(12).TotalMilliseconds, (int) TimeSpan.FromHours(12).TotalMilliseconds);
        }
    }
}
