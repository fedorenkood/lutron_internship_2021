using Lutron.CondorApiIntegration;
using Lutron.CondorApiIntegration.AreaTypePredictionClient;
using Lutron.Gulliver.DomainObjects.AreaTypeNamespace;
using Lutron.Gulliver.DomainObjects.Preferences;
using Lutron.Gulliver.FeatureFlagServiceProvider;
using Lutron.Gulliver.InfoObjects.ReferenceInfo;
using Lutron.Gulliver.Infrastructure.CachingFramework;
using Lutron.Gulliver.Infrastructure.Configuration;
using Lutron.Gulliver.Infrastructure.myLutronService;
using Lutron.Gulliver.Infrastructure.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockHttpServer;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;


namespace Lutron.Gulliver.DomainObjects.Test
{
    [TestClass]
    public class AreaTypeDetailsTest : UnitTestBase
    {
        #region Instance Variables

        private Project testProject;
        private Area rootArea1;
        private Area rootArea2;
        #endregion

        #region Mock Server Initiation
        private static int TestPort = 9900;
        private Mock<IAreaTypePredictionCondorApiClient> AreaTypePredictor;
        private AreaTypePredictionSource areaTypePredictionSourceTest;

        #endregion

        #region Initialize

        [Owner("ofedorenko")]
        protected override void OnTestInitialize()
        {
            SetProductType(ProductType.QuantumResi);
            testProject = Project.Create("Test Project");

            testProject.AddNewDomain();

            if (GulliverConfiguration.Instance.ProductType.IsProcessorSystemBasedHierarchy())
            {
                rootArea1 = testProject.GetDomains()[0].RootArea.ObservableProcessorSystems[0].RootArea;
                rootArea2 = testProject.GetDomains()[1].RootArea.ObservableProcessorSystems[0].RootArea;
            }
            else
            {
                rootArea1 = testProject.GetDomains()[0].RootArea;
                rootArea2 = testProject.GetDomains()[1].RootArea;
            }

            testProject.InitializeAreaGroups();

            // Initialize AreaTypePredictor
            AreaTypePredictor = new Mock<IAreaTypePredictionCondorApiClient>();
            AreaTypePredictor.Setup(m => m.PredictAreaType(It.IsAny<string>(), It.IsAny<string>()))
                .Callback((string areaName, string token) => {
                    // This feature currently can only work for the English language
                    if (GulliverConfiguration.Instance.ApplicationCulture != CultureConstants.CultureNameUS
                        || GulliverConfiguration.Instance.ProductType != ProductType.QuantumResi)
                    {
                        return;
                    }
                    bool? anonymousDataEnabled = (bool?)PreferenceManager.Instance.GetPreferenceValue(PreferenceInfoType.IsCloudSyncEnabled, PreferenceTypes.Project);
                    bool areaTypePredictionEnabled = FeatureFlagServiceFactory.GetService().IsFeatureFlagEnabled(FeatureFlagType.UseAreaTypePrediction);
                    if (anonymousDataEnabled != null && anonymousDataEnabled == true && areaTypePredictionEnabled)
                    {
                        var datumArray = new List<AreaTypePredictionData> {
                            new AreaTypePredictionData(areaName)
                        };
                        var areaPredCondorApi = CondorApiClientProvider.GetAreaTypePredictionClient(token, $"http://localhost:{TestPort}/");
                        var response = areaPredCondorApi.ExecuteRequest(datumArray);
                        response.Wait();
                        var result = response.Result;
                        if (result.Item1 == ResponseResult.Success)
                        {
                            areaTypePredictionSourceTest = AreaTypePredictionSource.MlAlgorithm;
                            return;
                        }
                    }
                    areaTypePredictionSourceTest = AreaTypePredictionSource.FuzzyMatchAlgorithm;
                }).Verifiable();
        }

        private MockServer GetAreaTypePredictionMockServer(int responseStatus, string responseString)
        {
            var requestHandlers = new List<MockHttpHandler>()
            {
                new MockHttpHandler("/v1/login", "POST", (req, rsp, prm) => {
                    rsp.StatusCode = (int)HttpStatusCode.OK;
                    return "{\"aws_api_key\": \"somevalue\", \"aws_access_key_id\": \"somevalue\",\"aws_secret_access_key\": \"somevalue\"}";
                }),
                new MockHttpHandler("v1/run/roomtype-prediction-residential/1.0.1", "POST", (req, rsp, prm) => {
                    rsp.StatusCode = responseStatus;
                    return responseString;
                })
            };
            return new MockServer(TestPort, requestHandlers);
        }

        #endregion

        #region Base Class Overrides

        /// <summary>
        /// Cleanup the Project DB
        /// </summary>
        protected override void CleanupProject()
        {
            //Cleanup any previous project and clear cache
            DomainObjectTestHelper.CleanupProject();
        }

        #endregion

        #region Save to DB Tests

        /// <summary>
        /// Verifies that AreaTypeDetails can be set and fetched
        /// </summary>
        [TestMethod]
        [Owner("ofedorenko")]
        public void AreaTypeDetails_Functional_SaveAndFetch()
        {
            Area area = rootArea1.AddNewArea();

            UInt32 associatedAreaId = area.AreaTypeDetails.AssociatedAreaId;
            var areaTypePredicted = AreaTypeNames.Atrium;
            var areaTypeSelected = AreaTypeNames.Atrium;
            var areaTypePredictionSource = AreaTypePredictionSource.FuzzyMatchAlgorithm;

            area.AreaTypeDetails.AreaTypePredicted = areaTypePredicted;
            area.AreaTypeDetails.AreaTypeSelected = areaTypeSelected;
            area.AreaTypeDetails.AreaTypePredictionSource = areaTypePredictionSource;


            //Area should be marked dirty
            Assert.IsTrue(area.IsDirty, "Area didn't mark dirty when AreaTypeDetails got changed.");

            Project.SaveProject();

            Cache.Clear();

            area = Area.Fetch(area.Id);
            Assert.AreEqual(associatedAreaId, area.AreaTypeDetails.AssociatedAreaId, "AssociatedAreaId didn't save to DB while setting it via AreaTypeDetails");
            Assert.AreEqual(areaTypePredicted, area.AreaTypeDetails.AreaTypePredicted, "AreaTypePredicted didn't save to DB while setting it via AreaTypeDetails");
            Assert.AreEqual(areaTypeSelected, area.AreaTypeDetails.AreaTypeSelected, "AreaTypeSelected didn't save to DB while setting it via AreaTypeDetails");
            Assert.AreEqual(areaTypePredictionSource, area.AreaTypeDetails.AreaTypePredictionSource, "AreaTypePredictionSource didn't save to DB while setting it via AreaTypeDetails");
        }

        /// <summary>
        /// Verifies that AreaTypeDetails can be saved and deleted
        /// </summary>
        [TestMethod]
        [Owner("ofedorenko")]
        public void AreaTypeDetails_Functional_SaveAndDelete()
        {
            Area area = rootArea1.AddNewArea();

            UInt32 associatedAreaId = area.Id;
            area.AreaTypeDetails.AreaTypePredicted = AreaTypeNames.Atrium;
            area.AreaTypeDetails.AreaTypeSelected = AreaTypeNames.Atrium;
            area.AreaTypeDetails.AreaTypePredictionSource = AreaTypePredictionSource.FuzzyMatchAlgorithm;


            //Area should be marked dirty
            Assert.IsTrue(area.IsDirty, "Area didn't mark dirty when AreaTypeDetails got changed.");

            Project.SaveProject();

            Cache.Clear();

            area = Area.Fetch(associatedAreaId);
            area.Delete();

            var areaTypeDetails = AreaTypeDetails.Fetch(associatedAreaId);

            Assert.IsNull(areaTypeDetails, "AreaTypeDetails was not deleted on Area delete.");
        }
        #endregion

        #region Prediction Tests
        /// <summary>
        /// Verifies that MlAlgorithm when everything is ok
        /// </summary>
        [TestMethod]
        [Owner("ofedorenko")]
        public void PredictAreaType_DataSharingEnabled()
        {
            SetProductType(ProductType.QuantumResi);
            GulliverConfiguration.Instance.ApplicationCulture = CultureConstants.CultureNameUS;
            areaTypePredictionSourceTest = AreaTypePredictionSource.Unset;
            var responseStatus = (int)HttpStatusCode.OK;
            var responseString = "{\"data\": [[\"somevalue\"], [\"somevalue\"]], \"result\": [[\"value_one\",\"value_two\"], [\"value_one\",\"value_two\"]]}";
            using (GetAreaTypePredictionMockServer(responseStatus, responseString))
            {
                PreferenceManager.Instance.SetPreferenceValue(PreferenceInfoType.IsCloudSyncEnabled, PreferenceTypes.Project, true);
                SetUpFlagTypeService(FeatureFlagType.UseAreaTypePrediction, true);
                AreaTypePredictor.Object.PredictAreaType("", "");

                Assert.AreEqual(areaTypePredictionSourceTest, AreaTypePredictionSource.MlAlgorithm);
            }
        }

        /// <summary>
        /// Verifies that FuzzyMatchAlgorithm is used when Data sharing is disabled
        /// </summary>
        [TestMethod]
        [Owner("ofedorenko")]
        public void PredictAreaType_DataSharingDisabled()
        {
            SetProductType(ProductType.QuantumResi);
            GulliverConfiguration.Instance.ApplicationCulture = CultureConstants.CultureNameUS;
            areaTypePredictionSourceTest = AreaTypePredictionSource.Unset;

            var responseStatus = (int)HttpStatusCode.OK;
            var responseString = "{\"data\": [[\"somevalue\"], [\"somevalue\"]], \"result\": [[\"value_one\",\"value_two\"], [\"value_one\",\"value_two\"]]}";
            using (GetAreaTypePredictionMockServer(responseStatus, responseString))
            {
                PreferenceManager.Instance.SetPreferenceValue(PreferenceInfoType.IsCloudSyncEnabled, PreferenceTypes.Project, false);
                SetUpFlagTypeService(FeatureFlagType.UseAreaTypePrediction, true);
                AreaTypePredictor.Object.PredictAreaType("", "");

                Assert.AreEqual(areaTypePredictionSourceTest, AreaTypePredictionSource.FuzzyMatchAlgorithm);
            }
        }

        /// <summary>
        /// Verifies that FuzzyMatchAlgorithm is used when Feature is disabled
        /// </summary>
        [TestMethod]
        [Owner("ofedorenko")]
        public void PredictAreaType_FeatureDisabled()
        {
            SetProductType(ProductType.QuantumResi);
            GulliverConfiguration.Instance.ApplicationCulture = CultureConstants.CultureNameUS;
            areaTypePredictionSourceTest = AreaTypePredictionSource.Unset;

            var responseStatus = (int)HttpStatusCode.OK;
            var responseString = "{\"data\": [[\"somevalue\"], [\"somevalue\"]], \"result\": [[\"value_one\",\"value_two\"], [\"value_one\",\"value_two\"]]}";
            using (GetAreaTypePredictionMockServer(responseStatus, responseString))
            {
                PreferenceManager.Instance.SetPreferenceValue(PreferenceInfoType.IsCloudSyncEnabled, PreferenceTypes.Project, true);
                SetUpFlagTypeService(FeatureFlagType.UseAreaTypePrediction, false);
                AreaTypePredictor.Object.PredictAreaType("", "");

                Assert.AreEqual(areaTypePredictionSourceTest, AreaTypePredictionSource.FuzzyMatchAlgorithm);
            }
        }

        /// <summary>
        /// Verifies that FuzzyMatchAlgorithm is used when request fails
        /// </summary>
        [TestMethod]
        [Owner("ofedorenko")]
        public void PredictAreaType_MalformatedResponse()
        {
            SetProductType(ProductType.QuantumResi);
            GulliverConfiguration.Instance.ApplicationCulture = CultureConstants.CultureNameUS;
            areaTypePredictionSourceTest = AreaTypePredictionSource.Unset;

            var responseStatus = (int)HttpStatusCode.OK;
            var responseString = "{}";
            using (GetAreaTypePredictionMockServer(responseStatus, responseString))
            {
                PreferenceManager.Instance.SetPreferenceValue(PreferenceInfoType.IsCloudSyncEnabled, PreferenceTypes.Project, true);
                SetUpFlagTypeService(FeatureFlagType.UseAreaTypePrediction, true);
                AreaTypePredictor.Object.PredictAreaType("", "");

                Assert.AreEqual(areaTypePredictionSourceTest, AreaTypePredictionSource.FuzzyMatchAlgorithm);
            }
        }

        /// <summary>
        /// Verifies that FuzzyMatchAlgorithm there is a server error
        /// </summary>
        [TestMethod]
        [Owner("ofedorenko")]
        public void PredictAreaType_ServerError()
        {
            SetProductType(ProductType.QuantumResi);
            GulliverConfiguration.Instance.ApplicationCulture = CultureConstants.CultureNameUS;
            areaTypePredictionSourceTest = AreaTypePredictionSource.Unset;

            var responseStatus = (int)HttpStatusCode.InternalServerError;
            var responseString = "{}";
            using (GetAreaTypePredictionMockServer(responseStatus, responseString))
            {
                PreferenceManager.Instance.SetPreferenceValue(PreferenceInfoType.IsCloudSyncEnabled, PreferenceTypes.Project, true);
                SetUpFlagTypeService(FeatureFlagType.UseAreaTypePrediction, true);
                AreaTypePredictor.Object.PredictAreaType("", "");

                Assert.AreEqual(areaTypePredictionSourceTest, AreaTypePredictionSource.FuzzyMatchAlgorithm);
            }
        }


        /// <summary>
        /// Verifies that nothing happens when the project type is wrong
        /// </summary>
        [TestMethod]
        [Owner("ofedorenko")]
        public void PredictAreaType_WrongProjectType()
        {
            SetProductType(ProductType.Quantum);
            GulliverConfiguration.Instance.ApplicationCulture = CultureConstants.CultureNameUS;
            areaTypePredictionSourceTest = AreaTypePredictionSource.Unset;

            var responseStatus = (int)HttpStatusCode.OK;
            var responseString = "{\"data\": [[\"somevalue\"], [\"somevalue\"]], \"result\": [[\"value_one\",\"value_two\"], [\"value_one\",\"value_two\"]]}";
            using (GetAreaTypePredictionMockServer(responseStatus, responseString))
            {
                PreferenceManager.Instance.SetPreferenceValue(PreferenceInfoType.IsCloudSyncEnabled, PreferenceTypes.Project, true);
                SetUpFlagTypeService(FeatureFlagType.UseAreaTypePrediction, true);
                AreaTypePredictor.Object.PredictAreaType("", "");

                Assert.AreEqual(areaTypePredictionSourceTest, AreaTypePredictionSource.Unset);
            }
        }

        /// <summary>
        /// Verifies that nothing happens when language is not English
        /// </summary>
        [TestMethod]
        [Owner("ofedorenko")]
        public void PredictAreaType_NonEnglish()
        {
            SetProductType(ProductType.QuantumResi);
            GulliverConfiguration.Instance.ApplicationCulture = "de-DE";
            areaTypePredictionSourceTest = AreaTypePredictionSource.Unset;

            var responseStatus = (int)HttpStatusCode.OK;
            var responseString = "{\"data\": [[\"somevalue\"], [\"somevalue\"]], \"result\": [[\"value_one\",\"value_two\"], [\"value_one\",\"value_two\"]]}";
            using (GetAreaTypePredictionMockServer(responseStatus, responseString))
            {
                PreferenceManager.Instance.SetPreferenceValue(PreferenceInfoType.IsCloudSyncEnabled, PreferenceTypes.Project, true);
                SetUpFlagTypeService(FeatureFlagType.UseAreaTypePrediction, true);
                AreaTypePredictor.Object.PredictAreaType("", "");

                Assert.AreEqual(areaTypePredictionSourceTest, AreaTypePredictionSource.Unset);
            }
        }
        #endregion

        #region Test AreaTypeParser

        /// <summary>
        /// Verifies that FuzzyMatchAlgorithm is used when Feature is disabled
        /// </summary>
        [TestMethod]
        [Owner("ofedorenko")]
        public void AreaTypeParserTest()
        {
            var bedroom = AreaTypeNamesParser.ParseAreaTypeNames(new List<string>()
            {
                "Bedroom"
            });
            var kitchen = AreaTypeNamesParser.ParseAreaTypeNames(new List<string>()
            {
                "Kitchen"
            });
            var hallway_stairs = AreaTypeNamesParser.ParseAreaTypeNames(new List<string>()
            {
                "Hallway/Stairs"
            });
            var entertaining_room = AreaTypeNamesParser.ParseAreaTypeNames(new List<string>()
            {
                "Entertaining Room"
            });
            var others = AreaTypeNamesParser.ParseAreaTypeNames(new List<string>()
            {
                "Others"
            });
            var unknown = AreaTypeNamesParser.ParseAreaTypeNames(new List<string>()
            {
                "asldfj"
            });
            Assert.AreEqual(bedroom[0], AreaTypeNames.Bedroom);
            Assert.AreEqual(kitchen[0], AreaTypeNames.Kitchen);
            Assert.AreEqual(hallway_stairs[0], AreaTypeNames.Hallway_Stairs);
            Assert.AreEqual(entertaining_room[0], AreaTypeNames.EntertainingRoom);
            Assert.AreEqual(others[0], AreaTypeNames.Others);
            Assert.AreEqual(unknown[0], AreaTypeNames.Others);
        }

        #endregion

    }
}
