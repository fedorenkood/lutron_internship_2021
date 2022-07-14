using System;
using System.IO;

namespace XRayOperatorTool.Model.Emitter
{
    public class EmitterPanel : AbstractPanel
    {
        private string panelType;
        protected FileProcessing fileProcessor = new EmitterFileProcessing();
        protected override FileProcessing FileProcessor => fileProcessor;


        public override string PanelType 
        { 
            get => panelType; 
            set {
                if (value != null)
                {
                    panelType = value;
                }
            } 
        }

        public EmitterPanel() : base()
        {
            panelType = "D3CookieEmitter";
        }

        protected override AbstractBoard InstantiateBoard(string file)
        {
            try
            {
                var filename = Path.GetFileName(file);
                var boardCoordinates = FileProcessor.FilenameToCoordinates(filename);
                var positionIdentifier = FileProcessor.GetPositionIdentifier(boardCoordinates);
                var status = BoardStatus.Ok;
                var overspecVoidCount = 0;
                var rowDict = FileProcessing.CsvToListOfDicts(file);
                foreach (var row in rowDict)
                {
                    if (row["Result"] != "OK")
                    {
                        status = BoardStatus.Fail;
                        break;
                    }
                    if (float.Parse(row["Overall Area Void (%)"]) >= 28.0)
                    {
                        overspecVoidCount++;
                    }
                    if (overspecVoidCount >= 3)
                    {
                        status = BoardStatus.Check;
                    }
                }
                var newBoard = new EmitterBoard(boardCoordinates, positionIdentifier, status);
                newBoard.CsvData = rowDict;
                return newBoard;
            } catch (Exception)
            {
                return null;
            }
        }

    }
}
