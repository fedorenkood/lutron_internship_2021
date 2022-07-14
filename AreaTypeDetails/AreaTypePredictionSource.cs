using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lutron.Gulliver.DomainObjects.AreaTypeNamespace
{
    /// <summary>
    /// AreaTypePredictionSource is used for logging purposes 
    /// to indicate which algorithm set the AreaTypePredicted
    /// </summary>
    public enum AreaTypePredictionSource : byte
    {
        /// <summary>
        /// Used in case the AreaType was not predicted for any reason
        /// </summary>
        Unset = 0,

        /// <summary>
        /// Used to indicated that user was online
        /// and remote ML algorithm was used to predict the AreaType
        /// </summary>
        MlAlgorithm = 1,

        /// <summary>
        /// Used to indicated that user was offline
        /// and local FuzzyMatch algorithm was used to predict the AreaType
        /// </summary>
        FuzzyMatchAlgorithm = 2
    }
}