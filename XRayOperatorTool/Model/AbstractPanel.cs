using com.ketra.ProductionCal.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using XRayOperatorTool.Converters;

namespace XRayOperatorTool.Model
{
    public abstract class AbstractPanel : INotifyPropertyChanged
    {
        private static readonly string ARCHIVE_DIRECTORY = "Archive";

        private int numRows = 0;
        private int numCols = 0;
        private PanelStatus status = PanelStatus.InReview;

        protected abstract FileProcessing FileProcessor { get; }
        protected virtual FileReader FileReader { get; set; }
        public Dictionary<PanelCoordinates, AbstractBoard> BoardsDict { get; set; }
        public virtual Guid SessionId { get; set; }
        public virtual string PanelId { get; set; }
        public abstract string PanelType { get; set; }
        public bool Archived { get; set; }
        public DateTime SessionRunTime { get; set; }
        public PanelCoordinates SelectedBoardCoordinates { get; set; }
        public AbstractBoard SelectedBoard { get; set; }
        public int NumRows
        {
            get
            {
                if (numRows == 0)
                {
                    CountRowsCols();
                }
                return numRows;
            }
        }
        public int NumCols
        {
            get
            {
                if (numCols == 0)
                {
                    CountRowsCols();
                }
                return numCols;
            }
        }

        public PanelStatus Status
        {
            get
            {
                if (status == PanelStatus.InReview || status == PanelStatus.UploadingToDbFailed)
                {
                    status = PanelStatus.Reviewed;
                    foreach (var entry in BoardsDict)
                    {
                        if (entry.Value.ApprovalStatus == BoardStatus.Check)
                        {
                            status = PanelStatus.InReview;
                            break;
                        }
                    }
                }
                return status;
            }
            set
            {
                status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        protected AbstractPanel()
        {
            this.Archived = false;
            this.BoardsDict = new Dictionary<PanelCoordinates, AbstractBoard>();
            this.FileReader = new FileReader();
            this.SessionId = Guid.NewGuid();
        }

        private void CountRowsCols()
        {
            foreach (var key in BoardsDict.Keys)
            {
                if (key.X > numCols)
                {
                    numCols = key.X;
                }
                if (key.Y > numRows)
                {
                    numRows = key.Y;
                }
            }
            numCols++;
            numRows++;
        }

        public void SetPanelFolder(string panelFolder)
        {
            FileReader.SelectedPath = panelFolder;
        }

        public void SelectPanelFolder()
        {
            FileReader.SelectFolder();
        }

        public void ProcessPanelFolder()
        {
            if (FileReader.SelectedPath == null)
            {
                return;
            }
            PanelId = FileProcessor.ProcessPanelFilename(Path.GetFileName(FileReader.SelectedPath));
            SessionRunTime = Directory.GetCreationTime(FileReader.SelectedPath);
            IEnumerable<string> csvFiles = FileReader.ReadFiles(FileReader.CSV_EXTENSION);
            foreach (string file in csvFiles)
            {
                var newBoard = InstantiateBoard(file);
                if (newBoard != null)
                {
                    BoardsDict.Add(newBoard.Coordinates, newBoard);
                }
            }
            IEnumerable<string> jpgFiles = FileReader.ReadFiles(FileReader.JPG_EXTENSION);
            foreach (string file in jpgFiles)
            {
                try
                {
                    var filename = Path.GetFileName(file);
                    var imgCoordinates = FileProcessor.ImgFilenameToCoordinates(filename);
                    if (BoardsDict.ContainsKey(imgCoordinates))
                    {
                        BoardsDict[imgCoordinates].ImagePath = file;
                    }
                } catch(Exception)
                {
                    // Do nothing
                }
            }
        }

        protected abstract AbstractBoard InstantiateBoard(string file);

        public virtual List<XRaySessionData> GetSessionData()
        {
            var sessionData = new List<XRaySessionData>();
            foreach (var entry in BoardsDict)
            {
                var board = entry.Value;
                foreach (var csvRow in board.CsvData)
                {

                    sessionData.Add(new XRaySessionData()
                    {
                        ID = Guid.NewGuid(),
                        Session_ID = SessionId,
                        Board_Position = board.PositionIdentifier,
                        Board_Result = EnumHelper.GetEnumDescription(board.ApprovalStatus),
                        Pad = FileProcessor.GetPadName(csvRow["ID"], board.Coordinates),
                        Pad_Result = csvRow["Result"],
                        Void_Area = double.Parse(csvRow["Overall Area Void (%)"])
                    });
                }

            }
            return sessionData;
        }

        public virtual XRaySession GetSessionInfo()
        {
            var currentPath = FileReader.SelectedPath;
            var currentDirectory = Path.GetFileName(currentPath);
            var parentPath = Directory.GetParent(currentPath).FullName;
            var directoryPathArchive = Path.Combine(parentPath, ARCHIVE_DIRECTORY, currentDirectory);
            return new XRaySession()
            {
                ID = SessionId,
                Panel_ID = PanelId,
                Panel_Type = PanelType,
                Time = SessionRunTime,
                Directory_Path = directoryPathArchive
            };
        }

        public virtual List<XRayPanelLookup> GetBoardPanelLookup()
        {
            var sessionData = new List<XRayPanelLookup>();
            foreach (var entry in BoardsDict)
            {
                var board = entry.Value;
                sessionData.Add(new XRayPanelLookup()
                {
                    Board_ID = board.Id,
                    Panel_ID = PanelId
                });

            }
            return sessionData;
        }

        public virtual void LogDataToDb()
        {
            Status = PanelStatus.UploadingToDb;
            Task.Run(() =>
            {
                try
                {
                    using (var db = ProductionCalDbContext.CreateProductionCalDbContext())
                    {
                        db.XRaySession.Add(GetSessionInfo());

                        var sessionData = GetSessionData();
                        foreach (var sessionDataInstance in sessionData)
                        {
                            db.XRaySessionData.Add(sessionDataInstance);
                        }
                        db.SaveChanges();
                    }

                    Status = PanelStatus.UploadedToDb;
                } catch (Exception)
                {
                    Status = PanelStatus.UploadingToDbFailed;
                }
            });
        }


        public void MoveFiles()
        {
            var oldPath = FileReader.SelectedPath;
            FileReader.CopyDirectory(ARCHIVE_DIRECTORY);
            // Change Image Path
            foreach (var entry in BoardsDict)
            {
                var imgName = Path.GetFileName(entry.Value.ImagePath);
                entry.Value.ImagePath = Path.Combine(FileReader.SelectedPath, imgName);
            }
            
            FileReader.DeleteDirectory(oldPath);
            Archived = true;
        }


        #region INotifyPropertyChanged Members

        /// <summary>
        /// Property changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// On Property Change
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
