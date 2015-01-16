using System;
using System.Collections.Generic;

namespace DataSync.UI.CommandHandling.Validation
{
    public class OptionToken : IValidationToken
    {
        /// <summary>
        /// Gets or sets the validation error message.
        /// </summary>
        /// <value>
        /// The validation error message.
        /// </value>
        public string ValidationErrorMessage { get; private set; }

        /// <summary>
        /// Gets or sets the option list.
        /// </summary>
        /// <value>
        /// The option list.
        /// </value>
        public List<string> OptionList { get; set; }

        /// <summary>
        /// Validates the specified validation element.
        /// </summary>
        /// <param name="validationElement">The validation element.</param>
        /// <returns></returns>
        public virtual bool Validate(string validationElement)
        {
            string capsElement = validationElement.ToUpper();

            if (OptionList == null)
            {
                throw new ArgumentException("OptionList is null!");
            }

            if (OptionList.Count == 0)
            {
                throw new ArgumentException("OptionList does'nt contain options!");
            }

            if (string.IsNullOrWhiteSpace(validationElement))
            {
                ValidationErrorMessage = "Element is empty!";
                return false;
            }

            if (!OptionList.Contains(capsElement))
            {
                ValidationErrorMessage = string.Format("{0} should be one of the options {1}",
                    capsElement, string.Join(",", OptionList));
                return false;
            }

            return true;
        }

    }
}
