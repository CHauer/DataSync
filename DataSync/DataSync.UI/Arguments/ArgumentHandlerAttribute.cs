// -----------------------------------------------------------------------
// <copyright file="ArgumentHandlerAttribute.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - ArgumentHandlerAttribute.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.UI.Arguments
{
    using System;

    /// <summary>
    /// The argument handler attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ArgumentHandlerAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentHandlerAttribute"/> class.
        /// </summary>
        /// <param name="argumentToHandle">
        /// The argument to handle.
        /// </param>
        public ArgumentHandlerAttribute(string argumentToHandle)
        {
            this.Argument = argumentToHandle;
        }

        /// <summary>
        /// Gets the argument.
        /// </summary>
        /// <value>
        /// The argument.
        /// </value>
        public string Argument { get; private set; }
    }
}