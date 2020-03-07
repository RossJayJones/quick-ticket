namespace QuickTicket.Core.Tests.Fixtures
{
    public static class CommonValidationMessages
    {
        public static string ArgumentNull(string parameterName)
        {
            return $"Value cannot be null. (Parameter '{parameterName}')";
        }
    }
}