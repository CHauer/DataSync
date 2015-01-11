using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataSync.Lib.Configuration;
using DataSync.Lib.Configuration.Data;

namespace DataSync.UI.CommandHandling
{
    public class ArgumentConfigurationCreator : IConfigurationLoader
    {
        /// <summary>
        /// The arguments
        /// </summary>
        private string[] args;

        public ArgumentConfigurationCreator(string[] args)
        {
            this.args = args;
        }

        /// <summary>
        /// Occurs when [error occured].
        /// </summary>
        public event EventHandler<ArgumentErrorEventArgs> ErrorOccured;

        public SyncConfiguration LoadConfiguration()
        {
            if (args == null)
            {
                throw new ArgumentNullException("No given Arguments!");
            }

            return null;
        }
    }
}
