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
    /// 
    /// </summary>
    public enum InstructionType
    {
        /// <summary>
        /// The addpair
        /// </summary>
        ADDPAIR,
        /// <summary>
        /// The deletepair
        /// </summary>
        DELETEPAIR,
        /// <summary>
        /// The clearpairs
        /// </summary>
        CLEARPAIRS,
        /// <summary>
        /// The exit
        /// </summary>
        EXIT,
        /// <summary>
        /// The set
        /// </summary>
        SET,
        /// <summary>
        /// The logto
        /// </summary>
        LOGTO,
        /// <summary>
        /// The switch
        /// </summary>
        SWITCH,
        /// <summary>
        /// The listpairs
        /// </summary>
        LISTPAIRS,
        /// <summary>
        /// The listpairs
        /// </summary>
        LISTSETTINGS,
        /// <summary>
        /// The showpairdetail
        /// </summary>
        SHOWPAIRDETAIL,
        /// <summary>
        /// The help
        /// </summary>
        HELP,
    }
}