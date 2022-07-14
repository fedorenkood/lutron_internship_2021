using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ContentSandbox.Models
{
    class Button : INotifyPropertyChanged
    {
        private string name;
        private ObservableCollection<AreaProgramming> programming;

        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.NotifyPropertyChanged("Name");
                }
            }
        }

        public ObservableCollection<AreaProgramming> Programming
        {
            get { return this.programming; }
            set
            {
                if (this.programming != value)
                {
                    this.programming = value;
                    this.NotifyPropertyChanged("Programming");
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

    class AreaProgramming : INotifyPropertyChanged
    {
        private Content content;

        public Area AffectedArea { get; set; }

        public Content Content
        {
            get { return this.content; }
            set
            {
                if (this.content != value)
                {
                    this.content = value;
                    this.NotifyPropertyChanged("Content");
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
