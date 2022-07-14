using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XRayOperatorTool.Model;

namespace XRayOperatorTool.CustomXamlProperties
{
    public static class BoardButtonProperties
    {

        // ButtonWidthProperty
        public static GridLength GetButtonWidth(DependencyObject obj)
        {
            return (GridLength)obj.GetValue(ButtonWidthProperty);
        }

        public static void SetButtonWidth(DependencyObject obj, GridLength value)
        {
            obj.SetValue(ButtonWidthProperty, value);
        }

        public static readonly DependencyProperty ButtonWidthProperty =
            DependencyProperty.RegisterAttached(
                "ButtonWidth",
                typeof(GridLength),
                typeof(BoardButtonProperties),
                new UIPropertyMetadata(new GridLength(200)));


        // ButtonHeightProperty
        public static GridLength GetButtonHeight(DependencyObject obj)
        {
            return (GridLength)obj.GetValue(ButtonHeightProperty);
        }

        public static void SetButtonHeight(DependencyObject obj, GridLength value)
        {
            obj.SetValue(ButtonHeightProperty, value);
        }

        public static readonly DependencyProperty ButtonHeightProperty =
            DependencyProperty.RegisterAttached(
                "ButtonHeight",
                typeof(GridLength),
                typeof(BoardButtonProperties),
                new UIPropertyMetadata(new GridLength(150)));


        // TickBrushProperty
        public static SolidColorBrush GetTickBrush(DependencyObject obj)
        {
            return (SolidColorBrush) obj.GetValue(TickBrushProperty);
        }

        public static void SetTickBrush(DependencyObject obj, SolidColorBrush value)
        {
            obj.SetValue(TickBrushProperty, value);
        }

        public static readonly DependencyProperty TickBrushProperty =
            DependencyProperty.RegisterAttached(
                "TickBrush",
                typeof(SolidColorBrush),
                typeof(BoardButtonProperties),
                new FrameworkPropertyMetadata(System.Windows.Media.Brushes.Black));

        // ImageBitmapProperty
        public static BitmapFrame GetImageBitmap(DependencyObject obj)
        {
            return (BitmapFrame)obj.GetValue(ImageBitmapProperty);
        }

        public static void SetImageBitmap(DependencyObject obj, BitmapFrame value)
        {
            obj.SetValue(ImageBitmapProperty, value);
        }

        public static readonly DependencyProperty ImageBitmapProperty =
            DependencyProperty.RegisterAttached(
                "ImageBitmap",
                typeof(BitmapFrame),
                typeof(BoardButtonProperties),
                new FrameworkPropertyMetadata());

        // ImageInstanceProperty
        public static BitmapImage GetImageInstance(DependencyObject obj)
        {
            return (BitmapImage)obj.GetValue(ImageInstanceProperty);
        }

        public static void SetImageInstance(DependencyObject obj, BitmapImage value)
        {
            obj.SetValue(ImageInstanceProperty, value);
        }

        public static readonly DependencyProperty ImageInstanceProperty =
            DependencyProperty.RegisterAttached(
                "ImageInstance",
                typeof(BitmapImage),
                typeof(BoardButtonProperties),
                new FrameworkPropertyMetadata());


        // ImagePathProperty
        public static string GetImagePath(DependencyObject obj)
        {
            return (string)obj.GetValue(ImagePathProperty);
        }

        public static void SetImagePath(DependencyObject obj, string value)
        {
            obj.SetValue(ImagePathProperty, value);
        }

        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.RegisterAttached(
                "ImagePath",
                typeof(string),
                typeof(BoardButtonProperties),
                new FrameworkPropertyMetadata(""));

        // ButtonTextProperty
        public static string GetButtonText(DependencyObject obj)
        {
            return (string)obj.GetValue(ButtonTextProperty);
        }

        public static void SetButtonText(DependencyObject obj, string value)
        {
            obj.SetValue(ButtonTextProperty, value);
        }

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.RegisterAttached(
                "ButtonText",
                typeof(string),
                typeof(BoardButtonProperties),
                new FrameworkPropertyMetadata(""));

        
    }
}
