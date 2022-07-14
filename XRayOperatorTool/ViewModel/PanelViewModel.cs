using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using XRayOperatorTool.EventHandlers;
using XRayOperatorTool.Model;
using XRayOperatorTool.Model.Emitter;

namespace XRayOperatorTool.ViewModel
{
    public class PanelViewModel : ViewModelBase
    {
        public Int32 RowCount
        {
            get;
            set;
        }
        public Int32 ColumnCount
        {
            get;
            set;
        }

        private bool popupOpen = false;
        private bool canUploadToDb = true;
        public bool PopupOpen
        {
            get
            {
                return popupOpen;
            }
            set
            {
                popupOpen = value;
                OnPropertyChanged(nameof(PopupOpen));
            }
        }

        public bool CanUploadToDb
        {
            get
            {

                return canUploadToDb;
            }
            set
            {
                canUploadToDb = value;
                OnPropertyChanged(nameof(CanUploadToDb));
            }
        }

        public AbstractPanel CurrentPanel { get; set; }
        public List<AbstractBoard> BoardList
        {
            get
            {
                return CurrentPanel.BoardsDict.Select(d => d.Value).ToList();
            }
        }

        private ICommand boardButtonCommand;
        public ICommand BoardButtonCommand
        {
            get
            {
                if (boardButtonCommand == null)
                {
                    boardButtonCommand = new DelegateCommand(this.OpenBoard);
                }
                return boardButtonCommand;
            }
            private set
            {
                if (value != null)
                {
                    boardButtonCommand = value;
                }
            }
        }

        public ICommand SendToServerCommand { get; set; }

        public PanelViewModel()
        {
            CurrentPanel = XRayProject.Instance.CurrentPanel;
            // Add event
            CurrentPanel.PropertyChanged += CurrentPanel_PropertyChanged;
            // Subscribe event
            EventSystem.Subscribe<PanelStatusMessenger>(this.PanelStatusChange);

            // Instantiate Properties
            RowCount = CurrentPanel.NumRows;
            ColumnCount = CurrentPanel.NumCols;
            BoardButtonCommand = new DelegateCommand(this.OpenBoard);
            SendToServerCommand = new DelegateCommand(this.SendToServer);
        }

        private void OpenBoard(object board)
        {
            if (board is AbstractBoard)
            {
                var clickedBoard = (AbstractBoard)board;
                XRayProject.Instance.CurrentPanel.SelectedBoardCoordinates = clickedBoard.Coordinates;
                XRayProject.Instance.CurrentPanel.SelectedBoard = clickedBoard;
                Console.WriteLine(((AbstractBoard)board).Coordinates.ToString());
                // Broadcast Events
                EventSystem.Publish<BoardButtonClickedMessenger>(new BoardButtonClickedMessenger() { ClickedBoard = clickedBoard });
            }
        }

        private void SendToServer(object commanderObject)
        {
            if (commanderObject is AbstractPanel)
            {
                var panel = (AbstractPanel)commanderObject;
                if (panel.Status.Equals(PanelStatus.Reviewed))
                {
                    panel.LogDataToDb();
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Reveiw all boards before saving to the database.",
                                          "Confirmation",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);
                }
            }
        }

        private void PanelStatusChange(object obj)
        {
            if (CurrentPanel.Status.Equals(PanelStatus.UploadedToDb) && !CurrentPanel.Archived)
            {
                CurrentPanel.MoveFiles();
            }
        }

        private void CurrentPanel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChange(e.PropertyName);
        }

        private void RaisePropertyChange(string propertyName)
        {
            if (propertyName == "Status")
            {
                PopupOpen = CurrentPanel.Status.Equals(PanelStatus.UploadingToDb);
                CanUploadToDb = !CurrentPanel.Status.Equals(PanelStatus.UploadedToDb);
                if (CurrentPanel.Status.Equals(PanelStatus.UploadedToDb) && !CurrentPanel.Archived)
                {
                    // Move Files
                    System.Windows.Application.Current.Dispatcher.Invoke(
                        (Action)(() => { CurrentPanel.MoveFiles(); })
                    );
                }
            }
        }
    }
}
