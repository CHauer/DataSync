// -----------------------------------------------------------------------
// <copyright file="Instruction.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - Instruction.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.UI.CommandHandling.Instructions
{
    using System.Collections.Generic;

    /// <summary>
    /// The instruction class.
    /// </summary>
    public class Instruction
    {
        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public List<Parameter> Parameters { get; set; }

        /// <summary>
        /// Gets or sets the plain instruction.
        /// </summary>
        /// <value>
        /// The plain instruction.
        /// </value>
        public string PlainInstruction { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type value.
        /// </value>
        public InstructionType Type { get; set; }
    }
}