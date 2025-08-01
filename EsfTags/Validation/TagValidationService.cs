using System.Collections.Generic;
using System.Linq;
using EsfCore.Esf;

namespace EsfCore.Validation
{
    /// <summary>
    /// Central service for validating all tag models in an EsfProgram.
    /// </summary>
    public class TagValidationService
    {
        private readonly List<ITagValidator> _validators;

        public TagValidationService(IEnumerable<ITagValidator> validators)
        {
            _validators = validators.ToList();
        }

        public List<string> ValidateAll(Tags.EsfProgram program)
        {
            var errors = new List<string>();

            foreach (var tag in program.Tags)
            {
                var validator = _validators.FirstOrDefault(v =>
                    string.Equals(v.TagName, tag.TagName, System.StringComparison.OrdinalIgnoreCase));

                if (validator != null)
                {
                    var tagErrors = validator.Validate(tag);
                    if (tagErrors.Any())
                    {
                        errors.Add($"Validation errors in {tag.TagName} tag:");
                        errors.AddRange(tagErrors.Select(e => "  - " + e));
                    }
                }
            }

            return errors;
        }
    }
}
