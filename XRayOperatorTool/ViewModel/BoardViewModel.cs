using System;
using System.Windows.Input;
using XRayOperatorTool.EventHandlers;
using XRayOperatorTool.Model;

namespace XRayOperatorTool.ViewModel
{
    public class BoardViewModel : ViewModelBase
    {
        public AbstractBoard CurrentBoard { get; set; }
        public bool StatusModifiable 
        { 
            get
            {
                return (CurrentBoard.ApprovalStatus != BoardStatus.Fail && CurrentBoard.ApprovalStatus != BoardStatus.Ok);
            }
        }

        private ICommand panelViewCommand;
        public ICommand PanelViewCommand
        {
            get
            {
                if (panelViewCommand == null)
                {
                    panelViewCommand = new DelegateCommand(this.BackToPanel);
                }
                return panelViewCommand;
            }
            private set
            {
                if (value != null)
                {
                    panelViewCommand = value;
                }
            }
        }

        public ICommand ManualPassCommand { get; set; }

        public ICommand ManualFailCommand { get; set; }
        public BoardViewModel()
        {
            CurrentBoard = XRayProject.Instance.CurrentPanel.SelectedBoard;
            // Instantiate commands
            ManualFailCommand = new DelegateCommand(this.ManualFailBoard);
            ManualPassCommand = new DelegateCommand(this.ManualPassBoard);
            PanelViewCommand = new DelegateCommand(this.BackToPanel);
            // Subscribe to Events
            EventSystem.Subscribe<BoardButtonClickedMessenger>(this.BoardButtonClicked);
        }

        private void ManualPassBoard(object obj)
        {
            if (StatusModifiable)
            {
                CurrentBoard.ApprovalStatus = BoardStatus.ManualPass;
            }
            this.BackToPanel(obj);
        }

        private void ManualFailBoard(object obj)
        {
            if (StatusModifiable)
            {
                CurrentBoard.ApprovalStatus = BoardStatus.ManualFail;
            }
            this.BackToPanel(obj);
        }

        public void BoardButtonClicked(object board)
        {
            if (board is BoardButtonClickedMessenger)
            {
                CurrentBoard = ((BoardButtonClickedMessenger)board).ClickedBoard;
            }
        }

        public void BackToPanel(object someObject)
        {
            // Broadcast Events
            EventSystem.Publish<PanelViewMessenger>(new PanelViewMessenger() { CurrentPanel = XRayProject.Instance.CurrentPanel });
        }
    }
}
