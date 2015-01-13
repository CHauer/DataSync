// -----------------------------------------------------------------------
// <copyright file="IValidationToken.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - IValidationToken.cs</summary>
// -----------------------------------------------------------------------

namespace DataSync.UI.CommandHandling.Validation
{
    /// <summary>
    /// 
    /// </summary>
    public interface IValidationToken
    {
        /// <summary>
        /// Gets or sets the validation error message.
        /// </summary>
        /// <value>
        /// The validation error message.
        /// </value>
        string ValidationErrorMessage { get; }

        /// <summary>
        /// Validates the specified validation element.
        /// </summary>
        /// <param name="validationElement">The validation element.</param>
        /// <returns></returns>
        bool Validate(string validationElement);
    }
}