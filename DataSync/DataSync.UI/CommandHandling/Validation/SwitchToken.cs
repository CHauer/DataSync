// -----------------------------------------------------------------------
// <copyright file="SwitchToken.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - SwitchToken.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.UI.CommandHandling.Validation
{
    using System.Collections.Generic;

    /// <summary>
    /// ValidationToken - OptionList - Fixed Values ON/OFF.
    /// </summary>
    public class SwitchToken : OptionToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchToken"/> class.
        /// </summary>
        public SwitchToken()
        {
            this.OptionList = new List<string>() { "ON", "OFF" };
        }
    }
}