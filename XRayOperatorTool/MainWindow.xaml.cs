using System.ComponentModel;
using System.Windows;
using XRayOperatorTool.Model;
using XRayOperatorTool.ViewModel;

namespace XRayOperatorTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!XRayProject.Instance.CanClose)
            {
                string msg = "Project is not uploaded to the database. Close without uploading?";
                MessageBoxResult result =
                  MessageBox.Show(
                    msg,
                    "XRayOperatorTool",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    // If user doesn't want to close, cancel closure
                    e.Cancel = true;
                }
            }
        }
    }
}
