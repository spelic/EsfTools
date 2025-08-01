namespace EsfParser.Esf
{
    /// <summary>
    /// Interface for validating parsed ESF tag models.
    /// </summary>
    public interface ITagValidator
    {
        /// <summary>
        /// The tag name this validator supports.
        /// </summary>
        string TagName { get; }

        /// <summary>
        /// Validates the provided tag model.
        /// </summary>
        /// <param name="model">The tag model instance.</param>
        /// <returns>A list of validation error messages. Empty if valid.</returns>
        List<string> Validate(IEsfTagModel model);
    }
}
