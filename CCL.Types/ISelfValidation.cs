namespace CCL.Types
{
    public enum SelfValidationResult
    {
        Pass,
        Warning,
        Fail,
        Critical
    }

    public interface ISelfValidation
    {
        public SelfValidationResult Validate(out string message);
    }
}
