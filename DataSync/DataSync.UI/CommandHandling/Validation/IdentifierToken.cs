using System;
using System.Text.RegularExpressions;

namespace DataSync.UI.CommandHandling.Validation
{
    public class IdentifierToken : IValidationToken
    {
        /// <summary>
        /// Gets or sets the validation error message.
        /// </summary>
        /// <value>
        /// The validation error message.
        /// </value>
        public string ValidationErrorMessage { get; private set; }

        /// <summary>
        /// Gets or sets the regex.
        /// </summary>
        /// <value>
        /// The regex.
        /// </value>
        public string Regex { get; set; }

        /// <summary>
        /// Validates the specified validation element.
        /// </summary>
        /// <param name="validationElement">The validation element.</param>
        /// <returns></returns>
        public bool Validate(string validationElement)
        {
            if (string.IsNullOrWhiteSpace(validationElement))
            {
                ValidationErrorMessage = "Element is empty!";
                return false; 
            }

            if (!new Regex(Regex).IsMatch(validationElement))
            {
                ValidationErrorMessage = String.Format("'{0}' doesnt match the given format!", validationElement);
                return false;
            }

            return true;
        }
    }
}
