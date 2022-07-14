using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FuzzyMatchTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\Users\ofedorenko\OneDrive - Lutron Electronics Co., Inc\Documents\Project_Docs\FuzzyMatchTesting\keyword_dict_tf_30p.json";
            // Open the file to read from.
            string keywordFile = File.ReadAllText(path);

            var keywordDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(keywordFile);
            var keyList = keywordDict.Keys.ToList();

            #region FuzzySharp
            /*var topFuzzySharp = Process.ExtractTop("my bedroom", keyList, limit: 1).ToList()[0];
            var areaTypeFuzzySharp = keywordDict[topFuzzySharp.Value];
            Console.WriteLine("FuzzySharp: " + areaTypeFuzzySharp);*/
            #endregion

            #region FuzzyMatch
            /*FuzzyMatcher instance = new FuzzyMatcher(keyList);
            var topFuzzyMatch = instance.FuzzyMatch("my bedroom")[0];
            var areaTypeFuzzyMatch = topFuzzyMatch.FormattedString;
            Console.WriteLine("FuzzyMatch: " + areaTypeFuzzyMatch);*/
            #endregion

            run_cmd("get", "hall");
        }

        private static void run_cmd(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"C:\Users\ofedorenko\OneDrive - Lutron Electronics Co., Inc\Documents\Project_Docs\FuzzyMatchTesting\FuzzySetForCSharp\FuzzySetForCSharp.exe";
            start.Arguments = string.Format("{0} {1}", cmd, args);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }
            }
        }
    }
}
