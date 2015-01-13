// -----------------------------------------------------------------------
// <copyright file="ValueToken.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - ValueToken.cs</summary>
// -----------------------------------------------------------------------

using System;

namespace DataSync.UI.CommandHandling.Validation
{
    /// <summary>
    /// 
    /// </summary>
    public class ValueToken : IValidationToken
    {
        /// <summary>
        /// Gets or sets the validation error message.
        /// </summary>
        /// <value>
        /// The validation error message.
        /// </value>
        public string ValidationErrorMessage { get; private set; }

        /// <summary>
        /// Gets or sets the type of the target.
        /// </summary>
        /// <value>
        /// The type of the target.
        /// </value>
        public Type TargetType { get; set; }

        /// <summary>
        /// Validates the specified validation element.
        /// </summary>
        /// <param name="validationElement">The validation element.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">TargetType is null!</exception>
        public bool Validate(string validationElement)
        {
            if (TargetType == null)
            {
                throw new ArgumentException("TargetType is null!");
            }

            if (TargetType == typeof (Int32))
            {
                try
                {
                    // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                    Convert.ToInt32(validationElement);
                }
                catch (Exception ex)
                {
                    ValidationErrorMessage = ex.Message;
                    return false;
                }
            }

            return true;
        }
    }
}