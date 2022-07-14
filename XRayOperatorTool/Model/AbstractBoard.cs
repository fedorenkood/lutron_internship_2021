using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace XRayOperatorTool.Model
{
    public abstract class AbstractBoard : INotifyPropertyChanged
    {
        private BoardStatus approvalStatus;
        private string id = null;
        private string imagePath = null;
        private BitmapFrame imageInstance = null;


        public string Id
        {
            get
            {
                if (id == null)
                {
                    id = "Undefined";
                }
                return id;
            }
            set
            {
                if (value != null)
                {
                    id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }
        public string ImagePath
        {
            get
            {
                return imagePath;
            }
            set
            {
                if (value != null)
                {
                    imagePath = value;
                    ImageInstance = GetBitmapFrame((Bitmap)Image.FromFile(ImagePath));
                    OnPropertyChanged(nameof(ImagePath));
                }
            }
        }

        public BitmapFrame ImageInstance
        {
            get
            {
                return imageInstance;
            }
            set
            {
                if (value != null)
                {
                    imageInstance = value;
                    OnPropertyChanged(nameof(ImageInstance));
                }
            }
        }
        public PanelCoordinates Coordinates { get; set; }
        public Tuple<GridLength, GridLength> ButtonDimensions { get; set; }
        public string PositionIdentifier { get; set; }
        public List<Dictionary<string, string>> CsvData { get; set; }
        public BoardStatus ApprovalStatus { 
            get
            {
                return approvalStatus;
            }
            set
            {
                approvalStatus = value;
                OnPropertyChanged(nameof(ApprovalStatus));
            }
        }

        public SolidColorBrush GetColor()
        {
            return StatusEnumToColor.GetColor(ApprovalStatus);
        }

        protected AbstractBoard(PanelCoordinates coordinates, string positionIdentifier, BoardStatus status)
        {
            this.Coordinates = coordinates;
            this.PositionIdentifier = positionIdentifier;
            this.ApprovalStatus = status;
            this.ButtonDimensions = new Tuple<GridLength, GridLength>(new GridLength(200), new GridLength(150));
        }

        public static BitmapFrame GetBitmapFrame(System.Drawing.Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Bgr24, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);

            return BitmapFrame.Create(bitmapSource);
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
