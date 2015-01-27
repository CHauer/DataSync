// -----------------------------------------------------------------------
// <copyright file="PathToken.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - PathToken.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.UI.CommandHandling.Validation
{
    using System;
    using System.IO;

    /// <summary>
    /// The path token class.
    /// </summary>
    public class PathToken : IValidationToken
    {
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

            validationElement = validationElement.Replace("\"", string.Empty);

            try
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                Path.GetFullPath(validationElement);
            }
            catch (Exception ex)
            {
                this.ValidationErrorMessage = string.Format(
                    "'{0}' is not a valid path or file name!" + " Details:{1}", 
                    validationElement, 
                    ex.Message);
                return false;
            }

            return true;
        }
    }
}