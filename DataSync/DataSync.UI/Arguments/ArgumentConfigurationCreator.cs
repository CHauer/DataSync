using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using DataSync.Lib.Configuration;
using DataSync.Lib.Configuration.Data;

namespace DataSync.UI.Arguments
{
    public class ArgumentConfigurationCreator : IConfigurationLoader
    {
        /// <summary>
        /// The arguments
        /// </summary>
        private string[] args;

        /// <summary>
        /// The configuration
        /// </summary>
        private SyncConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentConfigurationCreator" /> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public ArgumentConfigurationCreator(string[] args)
        {
            this.args = args;
            this.IsErrorFound = false;
        }

        /// <summary>
        /// Gets a value indicating a argument is invalid - error was detected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is error found; otherwise, <c>false</c>.
        /// </value>
        public bool IsErrorFound { get; private set; }

        /// <summary>
        /// Occurs when [error occured].
        /// </summary>
        public event EventHandler<ArgumentErrorEventArgs> ErrorOccured;

        /// <summary>
        /// Loads the configuration.
        /// </summary>
        /// <remarks>Exception handling in caller method required!</remarks>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">No given Arguments!</exception>
        public SyncConfiguration LoadConfiguration()
        {
            if (args == null)
            {
                throw new ArgumentException("No given Arguments!");
            }

            //Standard configuration
            configuration = new SyncConfiguration();

            args.ToList().ForEach(HandleArgument);

            return configuration;
        }

        /// <summary>
        /// Handles the argument.
        /// </summary>
        /// <param name="argument">The argument.</param>
        private void HandleArgument(String argument)
        {
            string parameter = null;

            if (IsErrorFound)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(argument))
            {
                //Ignore argument
                return;
            }

            argument = argument.Trim();

            if (argument.StartsWith("/"))
            {
                if (argument.Length == 1)
                {
                    return;
                }

                //remove leading /
                argument = argument.Remove(0, 1);

                //extract parameter
                if (argument.Contains(":"))
                {
                    string[] parts = argument.Split(':');

                    if (parts.Length == 2)
                    {
                        argument = parts[0].Trim();
                        parameter = parts[1].Trim();
                    }
                }

                MethodInfo handlerMethod = GetType().GetMethods()
                                            .First(method => method.GetCustomAttributes(typeof(ArgumentHandlerAttribute), false)
                                                             .Any(attr => ((ArgumentHandlerAttribute)attr).Argument.Equals(argument)));

                //if argument has parameter give to method
                if (!String.IsNullOrEmpty(parameter))
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
                //SyncPair
                SyncPairDefinitionParser parser = new SyncPairDefinitionParser(argument);

                try
                {
                    ConfigurationPair pair = parser.Parse();

                    configuration.ConfigPairs.Add(pair);
                }
                catch (Exception ex)
                {
                    OnErrorOccured(new ArgumentErrorEventArgs("Sync Pair Definition is invalid.", ex));
                }
            }
        }

        /// <summary>
        /// Handles the recursiv argument.
        /// </summary>
        [ArgumentHandler("r")]
        [ArgumentHandler("recursiv")]
        public void HandleRecursivArgument()
        {
            configuration.IsRecursiv = true;
        }

        /// <summary>
        /// Handles the log file argument.
        /// </summary>
        /// <param name="logfilename">The logfilename.</param>
        [ArgumentHandler("log")]
        public void HandleLogFileArgument(string logfilename)
        {
            if (string.IsNullOrEmpty(logfilename))
            {
                OnErrorOccured(new ArgumentErrorEventArgs("Argument log error - No Logfile name given."));
                return;
            }

            try
            {
                logfilename = Path.GetFullPath(logfilename);
            }
            catch (Exception ex)
            {
                OnErrorOccured(new ArgumentErrorEventArgs("Argument log error - Logfile name is a invalid", ex));
            }
                
            configuration.LogFileName = logfilename;
        }

        /// <summary>
        /// Handles the log size argument.
        /// </summary>
        /// <param name="logsize">The logsize.</param>
        [ArgumentHandler("logsize")]
        public void HandleLogSizeArgument(string logsize)
        {
            if (string.IsNullOrWhiteSpace(logsize))
            {
                OnErrorOccured(new ArgumentErrorEventArgs("Argument logsize error - No logfilesize given."));
                return;
            }

            int value = ConvertParameter(logsize, "logsize");

            if (value != -1)
            {
                if (value <= 0)
                {
                    OnErrorOccured(new ArgumentErrorEventArgs("Argument logsize error - logfilesize has to be greater than zero."));
                }

                configuration.LogFileSize = value;
            }

        }

        /// <summary>
        /// Converts the parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="argumentname">The argumentname.</param>
        /// <returns></returns>
        private int ConvertParameter(string parameter, string argumentname)
        {
            try
            {
                return Convert.ToInt32(parameter);
            }
            catch (Exception ex)
            {
                OnErrorOccured(new ArgumentErrorEventArgs(
                         String.Format("Argument {0} error.", argumentname), ex));

            }

            return -1;
        }

        /// <summary>
        /// Handles the block compare argument.
        /// </summary>
        /// <param name="filesize">The filesize.</param>
        [ArgumentHandler("blockcomparefilesize")]
        [ArgumentHandler("bcfs")]
        public void HandleBlockCompareArgument(string filesize)
        {
            if (string.IsNullOrWhiteSpace(filesize))
            {
                OnErrorOccured(new ArgumentErrorEventArgs("Argument blockcomparefilesize error - No file size given."));
                return;
            }

            int value = ConvertParameter(filesize, "blockcomparefilesize");

            if (value != -1)
            {
                if (value <= 0)
                {
                    OnErrorOccured(new ArgumentErrorEventArgs("Argument blockcomparefilesize error - file size has to be greater than zero."));
                }
                configuration.BlockCompareFileSize = value;
            }
        }

        /// <summary>
        /// Handles the block size argument.
        /// </summary>
        /// <param name="blocksize">The blocksize.</param>
        [ArgumentHandler("bs")]
        [ArgumentHandler("blocksize")]
        public void HandleBlockSizeArgument(string blocksize)
        {
            if (string.IsNullOrWhiteSpace(blocksize))
            {
                OnErrorOccured(new ArgumentErrorEventArgs("Argument blocksize error - No blocksize given."));
                return;
            }

            int value = ConvertParameter(blocksize, "blocksize");

            if (value != -1)
            {
                if (value <= 0)
                {
                    OnErrorOccured(new ArgumentErrorEventArgs("Argument blocksize error - block size has to be greater than zero."));
                }
                configuration.BlockSize = value;
            }
        }

        /// <summary>
        /// Handles the parallel synchronize argument.
        /// </summary>
        [ArgumentHandler("parallelsync")]
        [ArgumentHandler("ps")]
        public void HandleParallelSyncArgument()
        {
            this.configuration.IsParrallelSync = true;
        }

        /// <summary>
        /// Raises the <see cref="E:ErrorOccureds" /> event.
        /// </summary>
        /// <param name="e">The <see cref="ArgumentErrorEventArgs"/> instance containing the event data.</param>
        protected virtual void OnErrorOccured(ArgumentErrorEventArgs e)
        {
            IsErrorFound = true;
            var handler = ErrorOccured;
            if (handler != null) handler(this, e);
        }
    }
}
