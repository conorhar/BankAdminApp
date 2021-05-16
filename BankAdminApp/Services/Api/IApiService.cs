namespace BankAdminApp.Services.API
{
    public interface IApiService
    {
        string GenerateJSONWebToken(string id);
    }
}