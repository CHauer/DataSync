// -----------------------------------------------------------------------
// <copyright file="SwitchToken.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - SwitchToken.cs</summary>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace DataSync.UI.CommandHandling.Validation
{
    /// <summary>
    /// ValidationToken - OptionList - Fixed Values ON/OFF
    /// </summary>
    public class SwitchToken : OptionToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchToken" /> class.
        /// </summary>
        public SwitchToken()
        {
            base.OptionList = new List<string>()
            {
                "ON",
                "OFF"
            };
        }
    }
}