// -----------------------------------------------------------------------
// <copyright file="ArgumentConfigurationCreator.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - ArgumentConfigurationCreator.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.UI.Arguments
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using DataSync.Lib.Configuration;
    using DataSync.Lib.Configuration.Data;

    /// <summary>
    /// The argument configuration creator.
    /// </summary>
    public class ArgumentConfigurationCreator : IConfigurationLoader
    {
        /// <summary>
        /// The arguments.
        /// </summary>
        private string[] args;

        /// <summary>
        /// The argument pair counter.
        /// </summary>
        private int argumentPairCounter;

        /// <summary>
        /// The configuration.
        /// </summary>
        private SyncConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentConfigurationCreator"/> class.
        /// </summary>
        /// <param name="args">
        /// The arguments.
        /// </param>
        public ArgumentConfigurationCreator(string[] args)
        {
            this.argumentPairCounter = 0;
            this.args = args;
            this.IsErrorFound = false;
        }

        /// <summary>
        /// Occurs when error.
        /// </summary>
        public event EventHandler<ArgumentErrorEventArgs> ErrorOccured;

        /// <summary>
        /// Gets a value indicating whether a argument is invalid - error was detected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is error found; otherwise, <c>false</c>.
        /// </value>
        public bool IsErrorFound { get; private set; }

        /// <summary>
        /// Handles the block compare argument.
        /// </summary>
        /// <param name="filesize">
        /// The file size.
        /// </param>
        [ArgumentHandler("blockcomparefilesize")]
        [ArgumentHandler("bcfs")]
        public void HandleBlockCompareArgument(string filesize)
        {
            if (string.IsNullOrWhiteSpace(filesize))
            {
                this.OnErrorOccured(
                    new ArgumentErrorEventArgs("Argument blockcomparefilesize error - No file size given."));
                return;
            }

            int value = this.ConvertParameter(filesize, "blockcomparefilesize");

            if (value != -1)
            {
                if (value <= 0)
                {
                    this.OnErrorOccured(
                        new ArgumentErrorEventArgs(
                            "Argument blockcomparefilesize error - file size has to be greater than zero."));
                }

                this.configuration.BlockCompareFileSize = value;
            }
        }

        /// <summary>
        /// Handles the block size argument.
        /// </summary>
        /// <param name="blocksize">
        /// The block size.
        /// </param>
        [ArgumentHandler("bs")]
        [ArgumentHandler("blocksize")]
        public void HandleBlockSizeArgument(string blocksize)
        {
            if (string.IsNullOrWhiteSpace(blocksize))
            {
                this.OnErrorOccured(new ArgumentErrorEventArgs("Argument blocksize error - No blocksize given."));
                return;
            }

            int value = this.ConvertParameter(blocksize, "blocksize");

            if (value != -1)
            {
                if (value <= 0)
                {
                    this.OnErrorOccured(
                        new ArgumentErrorEventArgs("Argument blocksize error - block size has to be greater than zero."));
                }

                this.configuration.BlockSize = value;
            }
        }

        /// <summary>
        /// Handles the log file argument.
        /// </summary>
        /// <param name="logfilename">
        /// The log file name.
        /// </param>
        [ArgumentHandler("log")]
        public void HandleLogFileArgument(string logfilename)
        {
            if (string.IsNullOrEmpty(logfilename))
            {
                this.OnErrorOccured(new ArgumentErrorEventArgs("Argument log error - No Logfile name given."));
                return;
            }

            try
            {
                logfilename = Path.GetFullPath(logfilename);
            }
            catch (Exception ex)
            {
                this.OnErrorOccured(new ArgumentErrorEventArgs("Argument log error - Logfile name is a invalid", ex));
            }

            this.configuration.LogFileName = logfilename;
        }

        /// <summary>
        /// Handles the log size argument.
        /// </summary>
        /// <param name="logsize">
        /// The log size.
        /// </param>
        [ArgumentHandler("logsize")]
        public void HandleLogSizeArgument(string logsize)
        {
            if (string.IsNullOrWhiteSpace(logsize))
            {
                this.OnErrorOccured(new ArgumentErrorEventArgs("Argument logsize error - No logfilesize given."));
                return;
            }

            int value = this.ConvertParameter(logsize, "logsize");

            if (value != -1)
            {
                if (value <= 0)
                {
                    this.OnErrorOccured(
                        new ArgumentErrorEventArgs("Argument logsize error - logfilesize has to be greater than zero."));
                }

                this.configuration.LogFileSize = value;
            }
        }

        /// <summary>
        /// Handles the parallel synchronize argument.
        /// </summary>
        [ArgumentHandler("parallelsync")]
        [ArgumentHandler("ps")]
        public void HandleParallelSyncArgument()
        {
            this.configuration.IsParallelSync = true;
        }

        /// <summary>
        /// Handles the given argument.
        /// </summary>
        [ArgumentHandler("r")]
        [ArgumentHandler("recursiv")]
        public void HandleRecursivArgument()
        {
            this.configuration.IsRecursive = true;
        }

        /// <summary>
        /// Loads the configuration.
        /// </summary>
        /// <remarks>
        /// Exception handling in caller method required!.
        /// </remarks>
        /// <returns>
        /// The <see cref="SyncConfiguration"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// No given Arguments!.
        /// </exception>
        public SyncConfiguration LoadConfiguration()
        {
            if (this.args == null)
            {
                throw new ArgumentException("No given Arguments!");
            }

            // Standard configuration
            this.configuration = new SyncConfiguration();

            this.args.ToList().ForEach(this.HandleArgument);

            return this.configuration;
        }

        /// <summary>
        /// Raises the <see cref="E:ErrorOccureds"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="ArgumentErrorEventArgs"/> instance containing the event data.
        /// </param>
        protected virtual void OnErrorOccured(ArgumentErrorEventArgs e)
        {
            this.IsErrorFound = true;
            var handler = this.ErrorOccured;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Converts the parameter.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="argumentname">
        /// The argument name.
        /// </param>
        /// <returns>
        /// The converted parameter.
        /// </returns>
        private int ConvertParameter(string parameter, string argumentname)
        {
            try
            {
                return Convert.ToInt32(parameter);
            }
            catch (Exception ex)
            {
                this.OnErrorOccured(new ArgumentErrorEventArgs(string.Format("Argument {0} error.", argumentname), ex));
            }

            return -1;
        }

        /// <summary>
        /// Handles the argument.
        /// </summary>
        /// <param name="argument">
        /// The argument.
        /// </param>
        private void HandleArgument(string argument)
        {
            string parameter = null;

            if (this.IsErrorFound)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(argument))
            {
                // Ignore argument
                return;
            }

            argument = argument.Trim();

            if (argument.StartsWith("/"))
            {
                if (argument.Length == 1)
                {
                    return;
                }

                // remove leading /
                argument = argument.Remove(0, 1);

                // extract parameter
                if (argument.Contains(":"))
                {
                    string[] parts = argument.Split(':');

                    if (parts.Length == 2)
                    {
                        argument = parts[0].Trim();
                        parameter = parts[1].Trim();
                    }
                }

                MethodInfo handlerMethod =
                    this.GetType()
                        .GetMethods()
                        .First(
                            method =>
                            method.GetCustomAttributes(typeof(ArgumentHandlerAttribute), false)
                                .Any(attr => ((ArgumentHandlerAttribute)attr).Argument.Equals(argument)));

                // if argument has parameter give to method
                if (!string.IsNullOrEmpty(parameter))
                {
                    handlerMethod.Invoke(this, new object[] { parameter });
                }
                else
                {
                    handlerMethod.Invoke(this, null);
                }
            }
            else
            {
                // SyncPair
                SyncPairDefinitionParser parser = new SyncPairDefinitionParser(argument);

                try
                {
                    ConfigurationPair pair = parser.Parse();

                    pair.Name = string.Format("ArgumentPair{0}", ++this.argumentPairCounter);

                    this.configuration.ConfigPairs.Add(pair);
                }
                catch (Exception ex)
                {
                    this.OnErrorOccured(new ArgumentErrorEventArgs("Sync Pair Definition is invalid.", ex));
                }
            }
        }
    }
}