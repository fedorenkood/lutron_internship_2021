using XRayOperatorTool.EventHandlers;
using XRayOperatorTool.Model;
using XRayOperatorTool.Model.Emitter;

namespace XRayOperatorTool.ViewModel
{
    public class MainViewModel : ViewModelBase
    {

        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged("CurrentView");
            }
        }

        public MainViewModel()
        {
            XRayProject.Instance.AddPanelFolder(new EmitterPanel());
            // Subscribe to Events
            EventSystem.Subscribe<BoardButtonClickedMessenger>(this.ParseMessage);
            EventSystem.Subscribe<PanelViewMessenger>(this.ParseMessage);

            _currentView = new PanelView();
        }


        public void ParseMessage(object message)
        {
            if (message is BoardButtonClickedMessenger)
            {
                CurrentView = new BoardView();
            }
            if (message is PanelViewMessenger)
            {
                CurrentView = new PanelView();
            }
        }
    }
}
