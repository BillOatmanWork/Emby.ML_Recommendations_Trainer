using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ML_Recommendations_Trainer
{
    public class DataOrganization
    {
        public void Organize(string dataPath)
        {
            var master = new FileInfo(Path.Combine(dataPath, "recommendation-ratings-train-master.csv"));
            var submittedTrainingData = Directory.GetFiles(dataPath).Where(f => f.EndsWith("-train.csv"));

            var masterMovieRatingList = DeserializeCsvData(master.FullName, "userId,tmdbId,rating");
            Console.WriteLine(submittedTrainingData.Count() + " files will be processed.");
            foreach (var file in submittedTrainingData)
            {
                
                var data = DeserializeCsvData(file);
                foreach (var item in data)
                {
                    if (!masterMovieRatingList.Exists(i => i.userId == item.userId && i.tmdbId == item.tmdbId))
                    {
                        masterMovieRatingList.Add(item);
                    }
                }
            }

            SerializeCsvData(masterMovieRatingList, master.FullName, "userId,tmdbId,rating");
        }

        private List<MovieRating> DeserializeCsvData(string path, string header = "") 
        {
            var t = new List<MovieRating>();
            using (var sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (line == header || line.Contains(",,")) continue; //strip the header, and ignore lines with missing data
                    if (line.Contains("--bound")) continue;
                    if (line == "Content-Disposition: form-data; name=bilddatei; filename=upload.csv; filename*=utf-8''upload.csv") continue;
                    var lineValues = line.Split(",");
                    t.Add(new MovieRating()
                    {
                        userId = float.Parse(lineValues[0], CultureInfo.InvariantCulture.NumberFormat),
                        tmdbId = float.Parse(lineValues[1], CultureInfo.InvariantCulture.NumberFormat),
                        Label  = float.Parse(lineValues[2], CultureInfo.InvariantCulture.NumberFormat),
                    });
                    
                }
            }

            return t;
        }

        private void SerializeCsvData(List<MovieRating> data, string path, string header = null) 
        {
            using (var sw = new StreamWriter(path))
            {
                if (!string.IsNullOrEmpty(header))
                {
                    sw.WriteLine(header);
                }
                foreach (var item in data)
                {
                    sw.WriteLine($"{item.userId},{item.tmdbId},{item.Label}");
                }
            }
        }
    }

    
}
