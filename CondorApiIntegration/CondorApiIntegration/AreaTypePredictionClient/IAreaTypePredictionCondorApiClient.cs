namespace Lutron.CondorApiIntegration.AreaTypePredictionClient
{
    public interface IAreaTypePredictionCondorApiClient
    {
        void PredictAreaType(string areaName, string token);
    }
}
