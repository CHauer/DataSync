// -----------------------------------------------------------------------
// <copyright file="Parameter.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - Parameter.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.UI.CommandHandling.Instructions
{
    /// <summary>
    /// The Instruction Parameter class.
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public object Content { get; set; }

        /// <summary>
        /// Gets or sets the type of the parameter.
        /// </summary>
        /// <value>
        /// The ParameterType enumeration type value.
        /// </value>
        public ParameterType Type { get; set; }
    }
}