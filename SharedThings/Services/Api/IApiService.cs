namespace SharedThings.Services.Api
{
    public interface IApiService
    {
        string GenerateJSONWebToken(string id);
    }
}