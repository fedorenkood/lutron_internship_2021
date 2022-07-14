using System.Collections.Generic;

namespace Lutron.CondorApiIntegration.AreaTypePredictionClient
{

    /// <summary>
    /// Maps the string array of AreaTypes to the Integer array based on db
    /// </summary>
    public class AreaTypePredictionResult
    {
        /// <summary>
        /// 
        /// </summary>
        public string AreaName { get; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> AreaTypePredictedStringArray { get; }
        /// <summary>
        /// 
        /// </summary>
        public AreaTypePredictionResult(string name, List<string> areaTypePredictedStringArray)
        {
            AreaName = name;
            AreaTypePredictedStringArray = areaTypePredictedStringArray;
        }
    }

    /// <summary>
    /// Provides data about the area to the request
    /// Now contains only name, but it can be expanded in the future
    /// </summary>
    public class AreaTypePredictionData
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public AreaTypePredictionData(string name)
        {
            Name = name;
        }
    }
}
