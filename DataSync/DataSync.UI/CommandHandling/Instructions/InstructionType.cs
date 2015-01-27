// -----------------------------------------------------------------------
// <copyright file="InstructionType.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - InstructionType.cs</summary>
// -----------------------------------------------------------------------
// ReSharper disable InconsistentNaming
namespace DataSync.UI.CommandHandling.Instructions
{
    /// <summary>
    /// The instruction type enumeration.
    /// </summary>
    public enum InstructionType
    {
        /// <summary>
        /// The add pair.
        /// </summary>
        ADDPAIR, 

        /// <summary>
        /// The delete pair.
        /// </summary>
        DELETEPAIR, 

        /// <summary>
        /// The clear pairs.
        /// </summary>
        CLEARPAIRS, 

        /// <summary>
        /// The exit command.
        /// </summary>
        EXIT, 

        /// <summary>
        /// The set command.
        /// </summary>
        SET, 

        /// <summary>
        /// The log to command.
        /// </summary>
        LOGTO, 

        /// <summary>
        /// The switch command.
        /// </summary>
        SWITCH, 

        /// <summary>
        /// The list pairs command.
        /// </summary>
        LISTPAIRS, 

        /// <summary>
        /// The list pairs command.
        /// </summary>
        LISTSETTINGS, 

        /// <summary>
        /// The show pair detail command.
        /// </summary>
        SHOWPAIRDETAIL, 

        /// <summary>
        /// The help command.
        /// </summary>
        HELP, 
    }
}