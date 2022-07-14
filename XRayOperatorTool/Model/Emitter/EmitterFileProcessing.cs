using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XRayOperatorTool.Model.Emitter
{
    public class EmitterFileProcessing : FileProcessing
    {
        protected static readonly Dictionary<PanelCoordinates, string> coordinatesIdentifierMapping = new Dictionary<PanelCoordinates, string>()
        {
            {new PanelCoordinates(0, 2), "A" },
            {new PanelCoordinates(0, 1), "B" },
            {new PanelCoordinates(0, 0), "C" },
            {new PanelCoordinates(1, 2), "D" },
            {new PanelCoordinates(1, 1), "E" },
            {new PanelCoordinates(1, 0), "F" },
            {new PanelCoordinates(2, 2), "G" },
            {new PanelCoordinates(2, 1), "H" },
            {new PanelCoordinates(2, 0), "J" },
            {new PanelCoordinates(3, 2), "K" },
            {new PanelCoordinates(3, 1), "L" },
            {new PanelCoordinates(3, 0), "M" },
            {new PanelCoordinates(4, 2), "N" },
            {new PanelCoordinates(4, 1), "P" },
            {new PanelCoordinates(4, 0), "Q" },
            {new PanelCoordinates(5, 2), "R" },
            {new PanelCoordinates(5, 1), "S" },
            {new PanelCoordinates(5, 0), "T" }
        };

        private static readonly Dictionary<string, string> padNameStep2 = new Dictionary<string, string>
        {
            {"1", "ground_top_l" },
            {"2", "vibrancy_1" },
            {"3", "vibrancy_2" },
            {"4", "vibrancy_3" },
            {"5", "vibrancy_4" },
            {"6", "ground_top_r" },
            {"7", "red_detector_anode"},
            {"8", "red_detector_cathode"},
            {"9", "green_detector_anode"},
            {"10","ground_center"},
            {"11","white_anode"},
            {"12","white_cathode"},
            {"13", "green_detector_cathode"},
            {"14","blue_anode"},
            {"15","blue_cathode"},
            {"16","ground_bottom_l"},
            {"17","green_anode"},
            {"18","green_cathode"},
            {"19","red_anode"},
            {"20","red_cathode"},
            {"21","ground_bottom_r"}
        };

        private static readonly Dictionary<string, string> padNameStep4 = new Dictionary<string, string>
        {
            {"1", "ground_bottom_r" },
            {"2", "red_cathode" },
            {"3", "red_anode" },
            {"4", "green_cathode" },
            {"5", "green_anode" },
            {"6", "ground_bottom_l" },
            {"7", "blue_cathode"},
            {"8", "blue_anode"},
            {"9", "white_cathode"},
            {"10","white_anode"},
            {"11", "ground_center" },
            {"12","green_detector_anode"},
            {"13","green_detector_cathode"},
            {"14","red_detector_cathode"},
            {"15","red_detector_anode"},
            {"16","ground_top_r"},
            {"17","vibrancy_4"},
            {"18","vibrancy_3"},
            {"19","vibrancy_2"},
            {"20","vibrancy_1"},
            {"21","ground_top_l"}
        };


        public EmitterFileProcessing()
        {
        }

        public override PanelCoordinates FilenameToCoordinates(string filename)
        {
            try
            {
                var groups = Regex.Matches(filename, "[A-Z][0-9]+").Cast<Match>().Select(m => m.Value).ToList();
                var positionDict = new Dictionary<string, int>();
                foreach (string g in groups)
                {
                    MatchCollection matchList = Regex.Matches(g, "[A-Z]|[0-9]+");
                    var identifierTuple = matchList.Cast<Match>().Select(match => match.Value).ToList();
                    positionDict.Add(identifierTuple[0], Int32.Parse(identifierTuple[1]));
                }
                return PositionDictToCoordinates(positionDict);
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Failed to parse coordinates");
            }
        }

        public override PanelCoordinates ImgFilenameToCoordinates(string filename)
        {
            try
            {
                MatchCollection matchList = Regex.Matches(filename, "[0-9]+");
                var identifierList = matchList.Cast<Match>().Select(match => match.Value).ToList();
                var dictKeysList = new List<string>() { "B", "P", "S" };
                var positionDict = new Dictionary<string, int>();
                for (int i = 0; i < 3; i++)
                {
                    positionDict.Add(dictKeysList[i], Int32.Parse(identifierList[i]));
                }
                return PositionDictToCoordinates(positionDict);
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Failed to parse coordinates");
            }
        }

        public override PanelCoordinates PositionDictToCoordinates(Dictionary<string, int> positionDict)
        {
            var coordinates = new PanelCoordinates();
            var board = "B";  // Board: 3 patterns vertically
            var pattern = "P"; // Pattern: 2 emitters horizontally
            var step = "S";  // Step: Individual emitter (2 or 4)
            coordinates.X = (positionDict[board] - 1) * 2 + (int)((float)positionDict[step] / 2 - 1);
            coordinates.Y = 3 - positionDict[pattern];
            return coordinates;
        }

        public override string GetPositionIdentifier(PanelCoordinates coordinates)
        {
            if (coordinatesIdentifierMapping.ContainsKey(coordinates))
            {
                return coordinatesIdentifierMapping[coordinates];
            }
            return "Not identified";
        }

        public override string ProcessPanelFilename(string filename)
        {
            var matchList = Regex.Split(filename, "-D3 Production Boards-");
            return matchList[0];
        }

        public override string GetPadName(string number, PanelCoordinates coordinates)
        {
            Dictionary<string, string> useMapping;
            if (coordinates.X % 2 == 0)
            {
                useMapping = padNameStep2;
            } 
            else
            {
                useMapping = padNameStep4;
            }
            if (useMapping.ContainsKey(number))
            {
                return useMapping[number];
            }
            return number;
        }
    }
}
