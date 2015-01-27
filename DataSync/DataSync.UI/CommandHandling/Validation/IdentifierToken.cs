// -----------------------------------------------------------------------
// <copyright file="IdentifierToken.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - IdentifierToken.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.UI.CommandHandling.Validation
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The identifier token.
    /// </summary>
    public class IdentifierToken : IValidationToken
    {
        /// <summary>
        /// Gets or sets the regex.
        /// </summary>
        /// <value>
        /// The regex.
        /// </value>
        public string Regex { get; set; }

        /// <summary>
        /// Gets the validation error message.
        /// </summary>
        /// <value>
        /// The validation error message.
        /// </value>
        public string ValidationErrorMessage { get; private set; }

        /// <summary>
        /// Validates the specified validation element.
        /// </summary>
        /// <param name="validationElement">
        /// The validation element.
        /// </param>
        /// <returns>
        /// The validation status.
        /// </returns>
        public bool Validate(string validationElement)
        {
            if (string.IsNullOrWhiteSpace(validationElement))
            {
                this.ValidationErrorMessage = "Element is empty!";
                return false;
            }

            if (!new Regex(this.Regex).IsMatch(validationElement))
            {
                this.ValidationErrorMessage = string.Format("'{0}' doesnt match the given format!", validationElement);
                return false;
            }

            return true;
        }
    }
}