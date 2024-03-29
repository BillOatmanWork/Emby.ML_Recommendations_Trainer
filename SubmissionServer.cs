﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ML_Recommendations_Trainer
{
    public class SubmissionServer
    {

        public void StartServer()
        {
            //This will most like be behind a reverse proxy
            var httpListener = new HttpListener();
            var submissionServer = new SubmissionService(httpListener, "http://192.168.1.104:1234/csv/", ProcessYourResponse);
            submissionServer.Start();
        }

        private byte[] ProcessYourResponse(string data)
        {
            //TODO: We need some security here. Some kind of data check, and sender ip check so we don't get Ddos'd.
            var submissionDataFolder = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            var files = Directory.GetFiles(submissionDataFolder).Where(f => f.EndsWith(".csv"));
            var trainingFileName = $"recommendation-ratings-{files.Count() + 1}-train.csv";
            Console.WriteLine($"New CSV - {DateTime.Now}");
            
            using (var sw = new StreamWriter(Path.Combine(submissionDataFolder, trainingFileName)))
            {
                sw.Write(data);
            }

            var organizeData = new DataOrganization();
            organizeData.Organize(submissionDataFolder);


           
            return new byte[0]; // TODO when you want return some response
        }


    }
}
