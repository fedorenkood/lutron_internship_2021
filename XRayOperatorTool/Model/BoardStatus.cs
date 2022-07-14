using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace XRayOperatorTool.Model
{
    public enum BoardStatus
    {
        [Description("OK")]
        Ok,
        [Description("CHECK")]
        Check,
        [Description("FAIL")]
        Fail,
        [Description("MANUAL_PASS")]
        ManualPass,
        [Description("MANUAL_FAIL")]
        ManualFail
    }

    public static class StatusEnumToColor
    {
        private static Dictionary<BoardStatus, SolidColorBrush> statusColor = new Dictionary<BoardStatus, SolidColorBrush> 
        {
            {BoardStatus.Ok, Brushes.Green },
            {BoardStatus.Check, Brushes.Yellow },
            {BoardStatus.Fail, Brushes.Red },
            {BoardStatus.ManualPass, Brushes.Green },
            {BoardStatus.ManualFail, Brushes.Red },
        };
        public static SolidColorBrush GetColor(BoardStatus status)
        {
            if (statusColor.ContainsKey(status))
            {
                return statusColor[status];
            }
            return Brushes.Red;
        }

        public static BoardStatus GetStatus(SolidColorBrush color)
        {
            var colorStatus = new Dictionary<SolidColorBrush, BoardStatus>();
            foreach (var entry in statusColor)
            {
                colorStatus.Add(entry.Value, entry.Key);
            }
            if (colorStatus.ContainsKey(color))
            {
                return colorStatus[color];
            }
            return BoardStatus.Check;
        }
    }


    public static class EnumExtensions
    {
        public static string GetEnumDescriptionValue<T>(this T @enum) where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new InvalidOperationException();

            return typeof(T).GetField(@enum.ToString()).GetCustomAttribute<DescriptionAttribute>(false).Description;
        }
    }

}
