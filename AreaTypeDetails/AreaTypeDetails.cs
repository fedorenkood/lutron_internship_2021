using Lutron.CondorApiIntegration;
using Lutron.CondorApiIntegration.AreaTypePredictionClient;
using Lutron.Gulliver.DomainObjects.Preferences;
using Lutron.Gulliver.FeatureFlagServiceProvider;
using Lutron.Gulliver.InfoObjects.ReferenceInfo;
using Lutron.Gulliver.Infrastructure.Configuration;
using Lutron.Gulliver.Infrastructure.DatabaseFramework;
using Lutron.Gulliver.Infrastructure.LoggingFramework;
using Lutron.Gulliver.Infrastructure.myLutronService;
using Lutron.Gulliver.Infrastructure.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lutron.Gulliver.DomainObjects.AreaTypeNamespace
{
    /// <summary>
    /// 
    /// </summary>
    public class AreaTypeDetails : INotifyPropertyChanged, IAreaTypePredictionCondorApiClient
    {
        #region Fields

        /// <summary>
        /// Associates AreaTypeDetails with an Area DomainObject
        /// </summary>
        private UInt32 associatedAreaId;

        /// <summary>
        /// AreaTypePredicted with default value. 
        /// AreaTypePredicted is predicted using ML form the Area Name
        /// </summary> 
        private AreaTypeNames areaTypePredicted = AreaTypeNames.Unset;

        /// <summary>
        /// AreaTypeSelected with default value
        /// AreaTypeSelected is set either the same as AreaTypePredicted or overridden by user
        /// </summary>
        private AreaTypeNames areaTypeSelected = AreaTypeNames.Unset;

        /// <summary>
        /// 
        /// </summary>
        private AreaTypePredictionSource areaTypePredictionSource = AreaTypePredictionSource.Unset;

        /// <summary>
        /// 
        /// </summary>
        private static Dictionary<string, string> fuzzyMatchDict = null;

        private const string FUZZY_DICT_FILE_NAME = "fuzzy-area-type-dict.json";
        private const string FUZZY_MANIFEST_RESOURCE_NAME = "Lutron.Gulliver.DomainObjects.AreaTypeDetails." + FUZZY_DICT_FILE_NAME;
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public UInt32 AssociatedAreaId
        {
            get
            {
                return this.associatedAreaId;
            }
            private set
            {
                if (associatedAreaId != value)
                {
                    associatedAreaId = value;
                    OnPropertyChanged(nameof(AssociatedAreaId));
                }
            }
        }

        /// <summary>
        /// Gets or sets the Area Type Predicted for this Area
        /// </summary>
        public AreaTypeNames AreaTypePredicted
        {
            get
            {
                return this.areaTypePredicted;
            }
            set
            {
                if (areaTypePredicted != value)
                {
                    areaTypePredicted = value;
                    OnPropertyChanged(nameof(AreaTypePredicted));
                }
            }
        }

        /// <summary>
        /// Gets or sets the Area Type Selected for this Area
        /// </summary>
        public AreaTypeNames AreaTypeSelected
        {
            get
            {
                return this.areaTypeSelected;
            }
            set
            {
                if (areaTypeSelected != value)
                {
                    areaTypeSelected = value;
                    OnPropertyChanged(nameof(AreaTypeSelected));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public AreaTypePredictionSource AreaTypePredictionSource
        {
            get
            {
                return this.areaTypePredictionSource;
            }
            set
            {
                if (areaTypePredictionSource != value)
                {
                    areaTypePredictionSource = value;
                    OnPropertyChanged(nameof(AreaTypePredictionSource));
                }
            }
        }

        private static Dictionary<string, string> FuzzyMatchDict
        {
            get
            {
                if (fuzzyMatchDict == null)
                {
                    try
                    {
                        using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(FUZZY_MANIFEST_RESOURCE_NAME))
                        using (StreamReader streamReader = new StreamReader(stream))
                        {

                            fuzzyMatchDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(streamReader.ReadToEnd());

                        }

                    }
                    catch (Exception ex)
                    {
                        Log.WriteExceptionEntry(ex, $"Error parsing {FUZZY_MANIFEST_RESOURCE_NAME} resource");
                        fuzzyMatchDict = new Dictionary<string, string>();
                    }
                }
                return fuzzyMatchDict;
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="areaId"></param>
        public AreaTypeDetails(UInt32 areaId)
        {
            AssociatedAreaId = areaId;
        }

        #region DB interaction
        internal void WriteProperties()
        {
            WritePropertiesToProjectDB();
        }

        /// <summary>
        /// To write AreaTypeDetails to database
        /// </summary>
        private void WritePropertiesToProjectDB()
        {
            using (IDBProvider dbProvider = DataAccessManager.NewProjectProvider("ins_AreaTypeDetails"))
            {
                dbProvider.Add("@" + nameof(AssociatedAreaId), AssociatedAreaId);
                dbProvider.Add("@" + nameof(AreaTypePredicted), (int)AreaTypePredicted);
                dbProvider.Add("@" + nameof(AreaTypeSelected), (int)AreaTypeSelected);
                dbProvider.Add("@" + nameof(AreaTypePredictionSource), (byte)AreaTypePredictionSource);

                dbProvider.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Used to fetch AreaTypeDetails from database
        /// </summary>
        /// <returns></returns>
        internal static AreaTypeDetails Fetch(UInt32 areaId)
        {
            Area area = Area.Fetch(areaId);
            if (area == null)
            {
                return null;
            }
            AreaTypeDetails areaTypeDetails = new AreaTypeDetails(areaId);
            if (!area.IsNew)
            {
                using (IDBProvider provider = DataAccessManager.NewProjectProvider("sel_AreaTypeDetails"))
                {
                    provider.Add("@" + nameof(AssociatedAreaId), areaId);
                    ILutronDataReader reader = provider.ExecuteReader();
                    if (reader.Read())
                    {
                        areaTypeDetails.AssociatedAreaId = reader.GetNotNullUInt32(nameof(AssociatedAreaId));
                        areaTypeDetails.AreaTypePredicted = (AreaTypeNames)reader.GetInt32(nameof(AreaTypePredicted));
                        areaTypeDetails.AreaTypeSelected = (AreaTypeNames)reader.GetInt32(nameof(AreaTypeSelected));
                        areaTypeDetails.AreaTypePredictionSource = (AreaTypePredictionSource)reader.GetByte(nameof(AreaTypePredictionSource));
                    }
                }
            }
            return areaTypeDetails;
        }

        /// <summary>
        /// Clears the opportunity, place number and quotation number.
        /// </summary>
        internal void ClearData()
        {
            AreaTypePredicted = AreaTypeNames.Unset;
            AreaTypeSelected = AreaTypeNames.Unset;
            AreaTypePredictionSource = AreaTypePredictionSource.Unset;
        }

        /// <summary>
        /// Deletes the record from the database
        /// </summary>
        /// <param name="areaId"></param>
        internal static void Delete(UInt32 areaId)
        {
            using (IDBProvider provider = DataAccessManager.NewProjectProvider("del_AreaTypeDetails"))
            {
                provider.Add("@" + nameof(AssociatedAreaId), areaId);
                provider.ExecuteReader();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Overload of PredictAreaType without token parameter
        /// </summary>
        /// <param name="areaName"></param>
        internal void PredictAreaType(string areaName)
        {
            string token = UserManager.Instance.SecurityToken;
            PredictAreaType(areaName, token);
        }

        /// <summary>
        /// Predicts an AreaType using the remote ML algorithm if online, anonymousDataEnabled, and feature enabled
        /// In any other situation or error uses fuzzy match algorithm
        /// </summary>
        /// <param name="areaName"></param>
        /// <param name="token">Must be a UserManager.Instance.SecurityToken</param>
        public void PredictAreaType(string areaName, string token)
        {
            // This feature currently can only work for the English language
            if (GulliverConfiguration.Instance.ApplicationCulture != CultureConstants.CultureNameUS
                || GulliverConfiguration.Instance.ProductType != ProductType.QuantumResi)
            {
                return;
            }
            bool? anonymousDataEnabled = (bool?)PreferenceManager.Instance.GetPreferenceValue(PreferenceInfoType.IsCloudSyncEnabled, PreferenceTypes.Project);
            bool areaTypePredictionEnabled = FeatureFlagServiceFactory.GetService().IsFeatureFlagEnabled(FeatureFlagType.UseAreaTypePrediction);
            Task.Run(() =>
            {
                if (anonymousDataEnabled != null && anonymousDataEnabled == true && areaTypePredictionEnabled)
                {
                    var datumArray = new List<AreaTypePredictionData> {
                        new AreaTypePredictionData(areaName)
                    };
                    var areaPredCondorApi = CondorApiClientProvider.GetAreaTypePredictionClient(token);
                    var response = areaPredCondorApi.ExecuteRequest(datumArray);
                    response.Wait();
                    var result = response.Result;
                    if (result.Item1 == ResponseResult.Success)
                    {
                        SetPrediction(result.Item2[0].AreaTypePredictedStringArray, AreaTypePredictionSource.MlAlgorithm);
                        return;
                    }
                }
                UseFuzzyMatch(areaName);
            });
        }

        /// <summary>
        /// Predicts AreaType Using fuzzy match algorithm
        /// </summary>
        /// <param name="areaName"></param>
#pragma warning disable S1172 // Unused method parameters should be removed
        private void UseFuzzyMatch(string areaName)
#pragma warning restore S1172 // Unused method parameters should be removed
        {
            SetPrediction(AreaTypeNames.Unset, AreaTypePredictionSource.FuzzyMatchAlgorithm);
#pragma warning disable S1135 // Track uses of "TODO" tags
            // TODO: SYSENG-33634
#pragma warning restore S1135 // Track uses of "TODO" tags

#pragma warning disable S125 // Sections of code should not be commented out
            /*// Preprocess name
            Regex rgx = new Regex("[^a-zA-Z]");
            areaName = rgx.Replace(areaName, "");

            // Read dictionary
            var keyList = FuzzyMatchDict.Keys.ToList();

            // Do prediction
            var topFuzzySharp = FuzzySharp.Process.ExtractTop(areaName, keyList, limit: 1).ToList();
            if (topFuzzySharp.Any() && FuzzyMatchDict.ContainsKey(topFuzzySharp[0].Value))
            {
                var areaTypeFuzzySharp = FuzzyMatchDict[topFuzzySharp[0].Value];
                
                SetPrediction(new List<string> { areaTypeFuzzySharp }, AreaTypePredictionSource.FuzzyMatchAlgorithm);
            }
            else 
            {
                SetPrediction(AreaTypeNames.Other, AreaTypePredictionSource.FuzzyMatchAlgorithm);
            }
             */

#pragma warning restore S125 // Sections of code should not be commented out
        }


        /// <summary>
        /// Updates predicted AreaType and source
        /// </summary>
        /// <param name="predictedAreaTypeList"></param>
        /// <param name="source"></param>
        private void SetPrediction(List<string> predictedAreaTypeList, AreaTypePredictionSource source)
        {
            var areaTypeList = AreaTypeNamesParser.ParseAreaTypeNames(predictedAreaTypeList);
            if (areaTypeList != null && areaTypeList.Any())
            {
                SetPrediction(areaTypeList[0], source);
            }
        }

        /// <summary>
        /// Updates predicted AreaType and source
        /// </summary>
        /// <param name="predictedAreaType"></param>
        /// <param name="source"></param>
        private void SetPrediction(AreaTypeNames predictedAreaType, AreaTypePredictionSource source)
        {
            // logic to update the area type details
            AreaTypePredictionSource = source;
            AreaTypePredicted = predictedAreaType;
        }

        

        #endregion


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