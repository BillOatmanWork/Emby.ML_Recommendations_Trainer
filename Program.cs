using System;
using System.IO;

namespace ML_Recommendations_Trainer
{
    class Program
    {
        static void Main(string[] args)
        {
            var submissionDataFolder = Path.Combine(Directory.GetCurrentDirectory(), "Data");

            if (!Directory.Exists(submissionDataFolder))
            {
                Directory.CreateDirectory(submissionDataFolder);
            }

            var server = new SubmissionServer();
            server.StartServer();
            Console.ReadLine();
            //var organizeData = new DataOrganization();
            //organizeData.Organize(submissionDataFolder);

            //var nn = new NeuralNetwork();
            //nn.ExecuteTraining();
        }
    }
}
