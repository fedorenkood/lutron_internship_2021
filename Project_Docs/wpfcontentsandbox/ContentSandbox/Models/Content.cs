using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;

namespace ContentSandbox.Models
{
    class Layer
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
    
    class AvailableLayers : INotifyPropertyChanged
    {
        public ObservableCollection<Layer> layers;
        public ObservableCollection<Layer> Layers {
            get { return this.layers; }
            set
            {
                if (this.layers != value)
                {
                    this.layers = value;
                    this.NotifyPropertyChanged("LayerSettings");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }

    class Content
    {
        public string Name { get; set; }
        public int Id { get; set; }
        // Maps layer to color setting
        public Dictionary<Layer, ColorPoint> LayerSettings {get; set;}

        // Maps Zone Id to Layer Id
        public Dictionary<Zone, Layer> Overrides { get; set; }
    }

    class ColorPoint : INotifyPropertyChanged
    {
        private SolidColorBrush color;
        public SolidColorBrush Color
        {
            get { return this.color; }
            set
            {
                if (this.color != value)
                {
                    this.color = value;
                    this.NotifyPropertyChanged("Color");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
