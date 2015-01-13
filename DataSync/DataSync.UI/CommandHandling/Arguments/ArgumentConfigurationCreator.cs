using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DataSync.Lib.Configuration;
using DataSync.Lib.Configuration.Data;

namespace DataSync.UI.CommandHandling.Arguments
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
        /// Initializes a new instance of the <see cref="ArgumentConfigurationCreator"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public ArgumentConfigurationCreator(string[] args)
        {
            this.args = args;
        }

        /// <summary>
        /// Occurs when [error occured].
        /// </summary>
        public event EventHandler<ArgumentErrorEventArgs> ErrorOccured;

        /// <summary>
        /// Loads the configuration.
        /// </summary>
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
        private void HandleArgument(string argument)
        {
            string parameter = null;

            if (string.IsNullOrWhiteSpace(argument))
            {
                //Ignore argument
                return;
            }

            if (argument.StartsWith("/"))
            {
                //remove leading /
                argument = argument.Remove(0, 1);

                //extract parameter
                if (argument.Contains(":"))
                {
                    string[] parts = argument.Split(':');

                    if (parts.Length == 2)
                    {
                        argument = parts[0];
                        parameter = parts[1];
                    }
                }

                MethodInfo handlerMethod = this.GetType().GetMethods()
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

                ConfigurationPair pair = parser.Parse();

                configuration.ConfigPairs.Add(pair);
            }
        }

        /// <summary>
        /// Handles the recursiv argument.
        /// </summary>
        [ArgumentHandler("r")]
        [ArgumentHandler("recursiv")]
        private void HandleRecursivArgument()
        {
            configuration.IsRecursiv = true;
        }

        /// <summary>
        /// Handles the log file argument.
        /// </summary>
        /// <param name="logfilename">The logfilename.</param>
        [ArgumentHandler("log")]
        private void HandleLogFileArgument(string logfilename)
        {
            if (string.IsNullOrWhiteSpace(logfilename))
            {

            }

            configuration.LogFileName = logfilename;
        }

        /// <summary>
        /// Handles the log size argument.
        /// </summary>
        /// <param name="logsize">The logsize.</param>
        [ArgumentHandler("logsize")]
        private void HandleLogSizeArgument(string logsize)
        {
            if (string.IsNullOrWhiteSpace(logsize))
            {

            }

            int value = ConvertParameter(logsize, "logsize");
            
            if (value != -1)
            {
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
                if (ErrorOccured != null)
                {
                    ErrorOccured(this, new ArgumentErrorEventArgs(
                        String.Format("Argument {0} error.", argumentname), ex));
                }
            }

            return -1;
        }

        [ArgumentHandler("blockcomparefilesize")]
        [ArgumentHandler("bcfs")]
        private void HandleBlockCompareArgument(string logsize)
        {

        }

        [ArgumentHandler("bs")]
        [ArgumentHandler("blocksize")]
        private void HandleBlockSizeArgument(string logsize)
        {

        }

        [ArgumentHandler("parallelsync")]
        [ArgumentHandler("ps")]
        private void HandleParallelSyncArgument(string logsize)
        {

        }

    }
}
