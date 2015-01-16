// -----------------------------------------------------------------------
// <copyright file="InstructionExecuteAttribute.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - InstructionExecuteAttribute.cs</summary>
// -----------------------------------------------------------------------

using System;
using DataSync.UI.CommandHandling.Instructions;

namespace DataSync.UI.CommandHandling
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class InstructionExecuteAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstructionExecuteAttribute" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public InstructionExecuteAttribute(InstructionType type)
        {
            this.Type = type;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public InstructionType Type { get; private set; }
    }
}