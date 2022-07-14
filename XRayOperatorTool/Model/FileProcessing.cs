using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace XRayOperatorTool.Model
{
    public abstract class FileProcessing
    {
        public abstract PanelCoordinates FilenameToCoordinates(string filename);

        public abstract PanelCoordinates ImgFilenameToCoordinates(string filename);

        public abstract PanelCoordinates PositionDictToCoordinates(Dictionary<string, int> positionDict);

        public abstract string GetPositionIdentifier(PanelCoordinates coordinates);

        public abstract string ProcessPanelFilename(string filename);

        public abstract string GetPadName(string number, PanelCoordinates coordinates);

        public static List<Dictionary<string, string>> CsvToListOfDicts(string file)
        {
            var listOfLines = File.ReadLines(file).Select(row => row.Split(',').ToList()).ToList();
            var headerList = listOfLines[0];
            // Removing non-ascii characters
            for (int i = 0; i < headerList.Count; i++)
            {
                headerList[i] = Regex.Replace(headerList[i], @"[^\u0000-\u007F]+", string.Empty).Trim();
            }
            var rows = new List<Dictionary<string, string>>();
            for (int r = 1; r < listOfLines.Count; r++)
            {
                var rowDict = new Dictionary<string, string>();
                for (int i = 0; i < headerList.Count; i++)
                {
                    rowDict.Add(headerList[i], listOfLines[r][i].Trim());
                }
                rows.Add(rowDict);
            }
            // This can only be theoretically used if headers are unique
            // var dict = listOfLines.Select(row => row.ToDictionary<string, string>(cell => headerList[row.IndexOf(cell)], cell => cell));
            return rows;
        }
    }
}
