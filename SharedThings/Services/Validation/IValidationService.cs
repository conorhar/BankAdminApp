namespace SharedThings.Services.Validation
{
    public interface IValidationService
    {
        bool Has2DecimalPlacesOrLess(decimal amount);
        bool BalanceIsInsufficient(decimal amount, int accountId, string type);
    }
}