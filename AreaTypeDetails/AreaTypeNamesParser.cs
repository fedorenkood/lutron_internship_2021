using Lutron.Gulliver.Infrastructure.LoggingFramework;
using System;
using System.Collections.Generic;

namespace Lutron.Gulliver.DomainObjects.AreaTypeNamespace
{
    /// <summary>
    /// Parses AreaTypeNames
    /// </summary>
    public static class AreaTypeNamesParser
    {
        /// <summary>
        /// Parses returned string AreaTypes to the AreaTypeNames enum
        /// </summary>
        /// <param name="areaTypePredictedStringArray"></param>
        /// <returns></returns>
        public static List<AreaTypeNames> ParseAreaTypeNames(List<string> areaTypePredictedStringArray)
        {
            var AreaTypePredictedNamesList = new List<AreaTypeNames>();
            foreach (string areaTypePredictedString in areaTypePredictedStringArray)
            {
                var formattedString = areaTypePredictedString.Trim();
                formattedString = formattedString.Replace("/", "_");
                formattedString = formattedString.Replace(" ", "");
                try
                {
                    AreaTypePredictedNamesList.Add((AreaTypeNames)Enum.Parse(typeof(AreaTypeNames), formattedString));
                }
                catch (Exception ex)
                {
                    Log.WriteExceptionEntry(ex, $"Error parsing {areaTypePredictedString} to AreaTypeNames enum");
                    // In case the string from the server is not matched, it is replaced with others
                    AreaTypePredictedNamesList.Add(AreaTypeNames.Others);
                }
            }
            return AreaTypePredictedNamesList;
        }
    }
}
