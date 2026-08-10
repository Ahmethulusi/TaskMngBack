namespace TaskMngBack.Exceptions
{
    // İsim kasıtlı olarak "UnauthorizedAccessException" değil — .NET'in kendi
    // tipiyle karışmaması için.
    public class ForbiddenAccessException : Exception
    {
        public ForbiddenAccessException(string message) : base(message)
        {
        }
    }
}
