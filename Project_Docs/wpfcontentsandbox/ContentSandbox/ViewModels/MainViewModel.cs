using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ContentSandbox.Models;
using UIFramework.Core;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace ContentSandbox.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        #region Private Fields
        private Content activeContent;
        private Button selectedButton;
        #endregion

        #region Properties
        public Project Project { get; private set; }

        public AvailableLayers LayerList { get; set; }

        public Dictionary<Zone, ColorPoint> ZonePreviews { get; set; }

        public Dictionary<Zone, ColorPoint> KitchenZonePreviews {
            get
            {
                // Note: Assumes zone names are prefized with area name
                return
                    (from zp in ZonePreviews
                    where zp.Key.Name.Contains("Kitchen")
                    select zp).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        public Dictionary<Zone, ColorPoint> LivingZonePreviews
        {
            get
            {
                return
                    (from zp in ZonePreviews
                     where zp.Key.Name.Contains("Living")
                     select zp).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        public List<Button> ButtonList { get; set; }

        public Button SelectedButton
        {
            get { return this.selectedButton; }
            set
            {
                if (this.selectedButton != value)
                {
                    this.selectedButton = value;
                    this.NotifyPropertyChanged("SelectedButton");
                }
            }
        }

        //public Content 

        public ObservableCollection<Content> ContentList { get; set; }

        public ObservableCollection<Content> TrimmedContentList
        {
            get
            {
                var trimmedList = (from content in ContentList
                                   where content != null
                                   select content);
                return new ObservableCollection<Content>(trimmedList);
            }
        }
        public Content ActiveContent {
            get { return this.activeContent; }
            set
            {
               /* if (this.activeContent != value) // this prevents the update of active content when a new layer is added. Another way?
                {
                    this.activeContent = value;
                    this.NotifyPropertyChanged("ActiveContent");
                }*/
                this.activeContent = value;
                this.NotifyPropertyChanged("ActiveContent");
            }
        }
        #endregion

        #region Commands
        public ICommand AddContentCommand { get; private set; }
        public ICommand PreviewContentCommand { get; private set; }
        public ICommand Button1PressedCommand { get; private set; }
        public ICommand Button2PressedCommand { get; private set; }
        public ICommand Button3PressedCommand { get; private set; }
        public ICommand Button4PressedCommand { get; private set; }
        public ICommand ContentSelectedCommand { get; private set; }
        #endregion

        private ColorPoint defaultColorPoint()
        {
            // Necessary so each zone has a unique object, otherwise changing one changes
            // them all if it's the same object.
            return new ColorPoint() { Color = Brushes.Black };
        }
        public MainViewModel()
        {
            var ambientLayer = new Layer() { Id = 1, Name = "Ambient" };
            var taskLayer = new Layer() { Id = 2, Name = "Task" };
            var accentLayer = new Layer() { Id = 3, Name = "Accent" };
            
            this.LayerList = new AvailableLayers() { Layers = new ObservableCollection<Layer>() { taskLayer, accentLayer, ambientLayer } };

            #region Zone Tree Creation
            var zone1 = new Zone() { Id = 1, Name = "Kitchen Island", DefaultLayer = taskLayer };
            var zone2 = new Zone() { Id = 2, Name = "Kitchen Undercab", DefaultLayer = taskLayer };
            var zone3 = new Zone() { Id = 3, Name = "Kitchen Artwork", DefaultLayer = ambientLayer };
            var zone4 = new Zone() { Id = 4, Name = "Kitchen Downlights", DefaultLayer = accentLayer };

            var kitchenArea = new Area() { Name = "Kitchen", Zones = new List<Zone>() { zone1, zone2, zone3, zone4 } };

            var zone5 = new Zone() { Id = 5, Name = "Living Room Downlights", DefaultLayer = ambientLayer };
            var zone6 = new Zone() { Id = 6, Name = "Living Room Table Lamp", DefaultLayer = taskLayer };
            var zone7 = new Zone() { Id = 7, Name = "Living Room Artwork", DefaultLayer = accentLayer };
            var zone8 = new Zone() { Id = 8, Name = "Living Room Cove", DefaultLayer = ambientLayer };

            var livingArea = new Area() { Name = "Living Room", Zones = new List<Zone>() { zone5, zone6, zone7, zone8 } };

            this.Project = new Project() { Name = "HBD", Areas = new List<Area>() { kitchenArea, livingArea } };
            #endregion

            this.ZonePreviews = new Dictionary<Zone, ColorPoint>();
            this.ZonePreviews.Add(zone1, defaultColorPoint());
            this.ZonePreviews.Add(zone2, defaultColorPoint());
            this.ZonePreviews.Add(zone3, defaultColorPoint());
            this.ZonePreviews.Add(zone4, defaultColorPoint());
            this.ZonePreviews.Add(zone5, defaultColorPoint());
            this.ZonePreviews.Add(zone6, defaultColorPoint());
            this.ZonePreviews.Add(zone7, defaultColorPoint());
            this.ZonePreviews.Add(zone8, defaultColorPoint());

            #region Default Content Creation
            Content relax = new Content() { Id = 1, Name = "Relax" };
            Dictionary<Layer, ColorPoint> relaxSettings = new Dictionary<Layer, ColorPoint>();
            relaxSettings.Add(taskLayer, new ColorPoint() { Color = Brushes.Aqua });
            relaxSettings.Add(accentLayer, new ColorPoint() { Color = Brushes.CadetBlue });
            relaxSettings.Add(ambientLayer, new ColorPoint() { Color = Brushes.AliceBlue });
            relax.LayerSettings = relaxSettings;

            Content cook = new Content() { Id = 1, Name = "Cook" };
            Dictionary<Layer, ColorPoint> cookSettings = new Dictionary<Layer, ColorPoint>();
            cookSettings.Add(taskLayer, new ColorPoint() { Color = Brushes.Yellow });
            cookSettings.Add(accentLayer, new ColorPoint() { Color = Brushes.Orange });
            cookSettings.Add(ambientLayer, new ColorPoint() { Color = Brushes.LightYellow });
            cook.LayerSettings = cookSettings;

            Content off = new Content() { Id = 1, Name = "Off" };
            Dictionary<Layer, ColorPoint> offSettings = new Dictionary<Layer, ColorPoint>();
            offSettings.Add(taskLayer, new ColorPoint() { Color = Brushes.Black });
            offSettings.Add(accentLayer, new ColorPoint() { Color = Brushes.Black });
            offSettings.Add(ambientLayer, new ColorPoint() { Color = Brushes.Black });
            off.LayerSettings = offSettings;
            #endregion

            this.ActiveContent = relax;

            this.ContentList = new ObservableCollection<Content>();
            this.ContentList.Add(null); // Wait what? Why do we add null here?
            this.ContentList.Add(relax);
            this.ContentList.Add(cook);
            this.ContentList.Add(off);

            var button1 = new Button() { Name = "Button 1", Programming = new ObservableCollection<AreaProgramming>() };
            button1.Programming.Add(new AreaProgramming() { AffectedArea = kitchenArea, Content = cook });
            button1.Programming.Add(new AreaProgramming() { AffectedArea = livingArea, Content = null });
            var button2 = new Button() { Name = "Button 2", Programming = new ObservableCollection<AreaProgramming>() };
            button2.Programming.Add(new AreaProgramming() { AffectedArea = kitchenArea, Content = relax });
            button2.Programming.Add(new AreaProgramming() { AffectedArea = livingArea, Content = relax });
            var button3 = new Button() { Name = "Button 3", Programming = new ObservableCollection<AreaProgramming>() };
            button3.Programming.Add(new AreaProgramming() { AffectedArea = kitchenArea, Content = null });
            button3.Programming.Add(new AreaProgramming() { AffectedArea = livingArea, Content = null });
            var button4 = new Button() { Name = "Button 4", Programming = new ObservableCollection<AreaProgramming>() };
            button4.Programming.Add(new AreaProgramming() { AffectedArea = kitchenArea, Content = off });
            button4.Programming.Add(new AreaProgramming() { AffectedArea = livingArea, Content = off });
            this.SelectedButton = button1;
            this.ButtonList = new List<Button>() { button1, button2, button3, button4 };

            # region setup command handlers
            this.AddContentCommand = new DelegateCommand(onAddContentEvent); // TODO: finish
            this.PreviewContentCommand = new DelegateCommand(onPreviewContentEvent);
            this.Button1PressedCommand = new DelegateCommand(onButton1PressedEvent);
            this.Button2PressedCommand = new DelegateCommand(onButton2PressedEvent);
            this.Button3PressedCommand = new DelegateCommand(onButton3PressedEvent);
            this.Button4PressedCommand = new DelegateCommand(onButton4PressedEvent);
            #endregion
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region INotifyPropertyChanged method implementation

        /// <summary>
        /// On Property Change
        /// </summary>
        /// <param name="propertyName"></param>
        public virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Command Handlers
        private void onAddContentEvent(object o)
        {
            Layer customLayer = new Layer() { Id = 4, Name = "Custom" };
            this.LayerList.Layers.Add(customLayer);
            foreach (var contentType in this.ContentList)
            {
                if (contentType != null)
                {
                    contentType.LayerSettings.Add(customLayer, new ColorPoint() { Color = Brushes.Black });
                    if (contentType.Equals(this.ActiveContent))
                    {
                        this.ActiveContent = (contentType);
                    }
                }
                
            }
        }

        private void onPreviewContentEvent(object o)
        {
            foreach (var layer in this.ActiveContent.LayerSettings)
            {
                foreach (var zoneKey in this.ZonePreviews.Keys)
                {
                    if (zoneKey.DefaultLayer == layer.Key)
                    {
                        this.ZonePreviews[zoneKey].Color = layer.Value.Color;
                    }
                }
            }
        }

        private void onButton1PressedEvent(object o)
        {
            activateButton(0);
        }
        private void onButton2PressedEvent(object o)
        {
            activateButton(1);
        }
        private void onButton3PressedEvent(object o)
        {
            activateButton(2);
        }
        private void onButton4PressedEvent(object o)
        {
            activateButton(3);
        }
        #endregion

        private void activateButton(int buttonIndex)
        {
            Button b = this.ButtonList[buttonIndex];

            foreach (var areaSetting in b.Programming)
            {
                foreach (var zoneKey in this.ZonePreviews.Keys)
                {
                    if (zoneKey.Name.Contains(areaSetting.AffectedArea.Name))
                    {
                        try
                        {
                            // Pull zone preview color from corresponding area.layer progrmming on button
                            this.ZonePreviews[zoneKey].Color = areaSetting.Content.LayerSettings[zoneKey.DefaultLayer].Color;
                        }
                        catch (NullReferenceException) { }
                        catch (KeyNotFoundException) { }
                        
                    }
                }
            }
        }
    }
}
