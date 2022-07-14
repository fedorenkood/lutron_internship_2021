using System.Collections.Generic;

namespace XRayOperatorTool.Model
{
    class XRayProject
    {
        private static XRayProject instance = null;
        private bool canClose;
        public static XRayProject Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new XRayProject();
                }
                return instance;
            }
        }

        public List<AbstractPanel> Panels { get; set; }

        public int CurrentPanelIndex { get; set; }

        public AbstractPanel CurrentPanel { get; set; }

        public bool CanClose
        {
            get
            {
                if (!canClose)
                {
                    canClose = true;
                    foreach (var panel in Panels)
                    {
                        if (!panel.Status.Equals(PanelStatus.UploadedToDb))
                        {
                            canClose = false;
                            break;
                        }
                    }
                }
                return canClose;
            }
        }

        public XRayProject()
        {
            Panels = new List<AbstractPanel>();
        }

        public void AddPanelFolder(AbstractPanel panel)
        {
            panel.SelectPanelFolder();
            panel.ProcessPanelFolder();
            Panels.Add(panel);
            CurrentPanelIndex = Panels.Count - 1;
            CurrentPanel = panel;
        }
    }
}
