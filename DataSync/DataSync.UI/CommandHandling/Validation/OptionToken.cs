// -----------------------------------------------------------------------
// <copyright file="OptionToken.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - OptionToken.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.UI.CommandHandling.Validation
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The option token.
    /// </summary>
    public class OptionToken : IValidationToken
    {
        /// <summary>
        /// Gets or sets the option list.
        /// </summary>
        /// <value>
        /// The option list.
        /// </value>
        public List<string> OptionList { get; set; }

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
        public virtual bool Validate(string validationElement)
        {
            string capsElement = validationElement.ToUpper();

            if (this.OptionList == null)
            {
                throw new ArgumentException("OptionList is null!");
            }

            if (this.OptionList.Count == 0)
            {
                throw new ArgumentException("OptionList does'nt contain options!");
            }

            if (string.IsNullOrWhiteSpace(validationElement))
            {
                this.ValidationErrorMessage = "Element is empty!";
                return false;
            }

            if (!this.OptionList.Contains(capsElement))
            {
                this.ValidationErrorMessage = string.Format(
                    "{0} should be one of the options {1}", 
                    capsElement, 
                    string.Join(",", this.OptionList));
                return false;
            }

            return true;
        }
    }
}