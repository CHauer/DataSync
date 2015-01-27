// -----------------------------------------------------------------------
// <copyright file="ValueToken.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - ValueToken.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.UI.CommandHandling.Validation
{
    using System;

    /// <summary>
    /// The value token class.
    /// </summary>
    public class ValueToken : IValidationToken
    {
        /// <summary>
        /// Gets or sets the type of the target.
        /// </summary>
        /// <value>
        /// The type of the target.
        /// </value>
        public Type TargetType { get; set; }

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
        /// <exception cref="System.ArgumentException">
        /// TargetType is null!.
        /// </exception>
        public bool Validate(string validationElement)
        {
            if (this.TargetType == null)
            {
                throw new ArgumentException("TargetType is null!");
            }

            if (this.TargetType == typeof(int))
            {
                try
                {
                    int value = Convert.ToInt32(validationElement);

                    if (value <= 0)
                    {
                        this.ValidationErrorMessage = "Value has to be greater than 0.";
                    }
                }
                catch (Exception ex)
                {
                    this.ValidationErrorMessage = ex.Message;
                    return false;
                }
            }

            return true;
        }
    }
}