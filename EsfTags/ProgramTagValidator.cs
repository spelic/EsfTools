using System.Collections.Generic;
using EsfCore.Esf;

namespace EsfTags
{
    /// <summary>
    /// Validator for the PROGRAM tag, enforcing required attributes.
    /// </summary>
    public class ProgramTagValidator : ITagValidator
    {
        public string TagName => "PROGRAM";

        public List<string> Validate(IEsfTagModel model)
        {
            var errors = new List<string>();
            if (model is not ProgramTag tag)
            {
                errors.Add("Invalid tag type provided to ProgramTagValidator.");
                return errors;
            }

            if (string.IsNullOrWhiteSpace(tag.Name))
                errors.Add("'name' attribute is required.");
            if (string.IsNullOrWhiteSpace(tag.Type))
                errors.Add("'type' attribute is required.");

            return errors;
        }
    }
}
