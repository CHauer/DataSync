// -----------------------------------------------------------------------
// <copyright file="InputInstructionHandler.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - InputInstructionHandler.cs</summary>
// -----------------------------------------------------------------------

 // ReSharper disable UnusedMember.Local

namespace DataSync.UI.CommandHandling
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using DataSync.Lib.Configuration;
    using DataSync.Lib.Log;
    using DataSync.Lib.Log.Messages;
    using DataSync.Lib.Sync;
    using DataSync.UI.CommandHandling.Decoder;
    using DataSync.UI.CommandHandling.Instructions;
    using DataSync.UI.Monitor;

    /// <summary>
    /// The input instruction handler class.
    /// </summary>
    public class InputInstructionHandler
    {
        /// <summary>
        /// The prompt.
        /// </summary>
        private const string Prompt = ">";

        /// <summary>
        /// The decoder.
        /// </summary>
        private InstructionDecoder decoder;

        /// <summary>
        /// The input.
        /// </summary>
        private TextReader input;

        /// <summary>
        /// The is running.
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// The output.
        /// </summary>
        private TextWriter output;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputInstructionHandler"/> class.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <param name="output">
        /// The output.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Input or output.
        /// </exception>
        public InputInstructionHandler(TextReader input, TextWriter output)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            if (output == null)
            {
                throw new ArgumentNullException("output");
            }

            this.output = output;
            this.input = input;

            this.Initialize();
        }

        /// <summary>
        /// Occurs after output was written.
        /// </summary>
        public event EventHandler<OutputEventArgs> AfterOutput;

        /// <summary>
        /// Occurs before a output is written.
        /// </summary>
        public event EventHandler<OutputEventArgs> BeforeOutput;

        /// <summary>
        /// Occurs when exit instruction.
        /// </summary>
        public event EventHandler ExitOccured;

        /// <summary>
        /// Occurs when help instruction.
        /// </summary>
        public event EventHandler HelpInstructionOccured;

        /// <summary>
        /// Occurs when an instruction is received.
        /// </summary>
        public event EventHandler<Instruction> InstructionOccured;

        /// <summary>
        /// Occurs when help instruction.
        /// </summary>
        public event EventHandler<LogFilePropertiesChangedEventArgs> LogFileChangeOccured;

        /// <summary>
        /// Occurs when a monitor change instruction.
        /// </summary>
        public event EventHandler<MonitorChangeEventArgs> MonitorChangeOccured;

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILog Logger { get; set; }

        /// <summary>
        /// Gets or sets the synchronize manager.
        /// </summary>
        /// <value>
        /// The synchronize manager.
        /// </value>
        public SyncManager SyncManager { get; set; }

        /// <summary>
        /// Executes the add synchronize pair instruction.
        /// </summary>
        /// <param name="instruction">
        /// The instruction.
        /// </param>
        [InstructionExecute(InstructionType.ADDPAIR)]
        public void ExecuteAddSyncPairInstruction(Instruction instruction)
        {
            if (instruction == null || this.SyncManager == null)
            {
                return;
            }

            var pair = this.HandleAddPairInputs();

            if (instruction.Parameters.Count > 0)
            {
                pair.Name = instruction.Parameters[0].Content.ToString();
            }
            else
            {
                pair.Name = string.Format("Sync Pair {0}", this.SyncManager.Configuration.ConfigPairs.Count + 1);
            }

            if (this.SyncManager.AddSyncPair(pair))
            {
                this.WriteConfirm(string.Format("SyncPair {0} was added.", pair.Name));
            }
        }

        /// <summary>
        /// Executes the change log file instruction.
        /// </summary>
        /// <param name="instruction">
        /// The instruction.
        /// </param>
        [InstructionExecute(InstructionType.LOGTO)]
        public void ExecuteChangeLogFileInstruction(Instruction instruction)
        {
            if (instruction == null || this.SyncManager == null)
            {
                return;
            }

            this.SyncManager.Configuration.LogFileName = instruction.Parameters[0].Content.ToString();
            this.OnLogFileChangeOccured(instruction.Parameters[0].Content.ToString());
        }

        /// <summary>
        /// Executes the exit instruction.
        /// </summary>
        /// <param name="instruction">
        /// The instruction.
        /// </param>
        [InstructionExecute(InstructionType.EXIT)]
        public void ExecuteExitInstruction(Instruction instruction)
        {
            this.isRunning = false;

            if (this.ExitOccured != null)
            {
                this.ExitOccured(this, new EventArgs());
            }
        }

        /// <summary>
        /// Executes the help instruction.
        /// </summary>
        /// <param name="instruction">
        /// The instruction.
        /// </param>
        [InstructionExecute(InstructionType.HELP)]
        public void ExecuteHelpInstruction(Instruction instruction)
        {
            if (this.HelpInstructionOccured != null)
            {
                this.HelpInstructionOccured(this, new EventArgs());
            }
        }

        /// <summary>
        /// Executes the pair detail instruction.
        /// </summary>
        /// <param name="instruction">
        /// The instruction.
        /// </param>
        [InstructionExecute(InstructionType.LISTSETTINGS)]
        public void ExecuteListSettingsInstruction(Instruction instruction)
        {
            if (instruction == null || this.SyncManager == null || this.SyncManager.Configuration == null)
            {
                return;
            }

            this.WriteMessage(this.SyncManager.Configuration.ToString(), ConsoleColor.DarkCyan);
        }

        /// <summary>
        /// Executes the pair detail instruction.
        /// </summary>
        /// <param name="instruction">
        /// The instruction.
        /// </param>
        [InstructionExecute(InstructionType.LISTPAIRS)]
        [InstructionExecute(InstructionType.SHOWPAIRDETAIL)]
        public void ExecutePairDetailInstruction(Instruction instruction)
        {
            if (instruction == null || this.SyncManager == null)
            {
                return;
            }

            if (instruction.Type == InstructionType.SHOWPAIRDETAIL)
            {
                var detailpair =
                    this.SyncManager.SyncPairs.FirstOrDefault(
                        sp => sp.ConfigurationPair.Name.Equals(instruction.Parameters[0].Content));

                if (detailpair != null)
                {
                    this.WriteMessage(detailpair.ToString(), ConsoleColor.DarkYellow);
                }
                else
                {
                    this.WriteError(string.Format("Pair Name {0} was not found!", instruction.Parameters[0].Content));
                }
            }
            else if (instruction.Type == InstructionType.LISTPAIRS)
            {
                this.WriteMessage(this.SyncManager.ToString(), ConsoleColor.DarkYellow);
            }
        }

        /// <summary>
        /// Executes the pair clear/delete instruction.
        /// </summary>
        /// <param name="instruction">
        /// The instruction.
        /// </param>
        [InstructionExecute(InstructionType.DELETEPAIR)]
        [InstructionExecute(InstructionType.CLEARPAIRS)]
        public void ExecutePairInstruction(Instruction instruction)
        {
            if (instruction == null || this.SyncManager == null)
            {
                return;
            }

            if (instruction.Type == InstructionType.DELETEPAIR)
            {
                var delpair =
                    this.SyncManager.SyncPairs.FirstOrDefault(
                        sp => sp.ConfigurationPair.Name.Equals(instruction.Parameters[0].Content));

                if (delpair != null)
                {
                    if (this.ConfirmMessage())
                    {
                        this.SyncManager.RemoveSyncPair(instruction.Parameters[0].Content.ToString());
                    }
                }
                else
                {
                    this.WriteError(string.Format("Pair Name {0} was not found!", instruction.Parameters[0].Content));
                }
            }
            else if (instruction.Type == InstructionType.CLEARPAIRS)
            {
                if (this.ConfirmMessage())
                {
                    this.SyncManager.ClearSyncPairs();
                }
            }
        }

        /// <summary>
        /// Executes the exit instruction.
        /// </summary>
        /// <param name="instruction">
        /// The instruction.
        /// </param>
        [InstructionExecute(InstructionType.SET)]
        public void ExecuteSetInstruction(Instruction instruction)
        {
            if (instruction == null || this.SyncManager == null)
            {
                return;
            }

            string parameter = instruction.Parameters[0].Content.ToString().ToUpper();
            int value;

            try
            {
                value = Convert.ToInt32(instruction.Parameters[1].Content);
            }
            catch (Exception ex)
            {
                this.LogMessage(new ErrorLogMessage(ex));
                return;
            }

            if (parameter.Equals("LOGSIZE"))
            {
                this.SyncManager.Configuration.LogFileSize = value;
                this.WriteConfirm(string.Format("{0} parameter was set to {1}.", parameter, value));
                this.OnLogFileChangeOccured(value);
            }
            else if (parameter.Equals("BLOCKCOMPAREFILESIZE"))
            {
                this.SyncManager.Configuration.BlockCompareFileSize = value;
                this.WriteConfirm(string.Format("{0} parameter was set to {1}.", parameter, value));
            }
            else if (parameter.Equals("BLOCKSIZE"))
            {
                this.SyncManager.Configuration.BlockSize = value;
                this.WriteConfirm(string.Format("{0} parameter was set to {1}.", parameter, value));
            }
        }

        /// <summary>
        /// Executes the exit instruction.
        /// </summary>
        /// <param name="instruction">
        /// The instruction.
        /// </param>
        [InstructionExecute(InstructionType.SWITCH)]
        public void ExecuteSwitchtInstruction(Instruction instruction)
        {
            if (instruction == null)
            {
                return;
            }

            string parameter = instruction.Parameters[0].Content.ToString().ToUpper();
            bool switcher = instruction.Parameters[1].Content.ToString().ToUpper().Equals("ON");

            if (parameter.Equals("LOGVIEW") || parameter.Equals("JOBSVIEW"))
            {
                if (this.MonitorChangeOccured != null)
                {
                    this.MonitorChangeOccured(
                        this, 
                        new MonitorChangeEventArgs(
                            parameter.Equals("LOGVIEW") ? MonitorType.Log : MonitorType.Screen, 
                            !switcher));
                }

                this.WriteConfirm(
                    string.Format("{0} flag was set to {1}.", parameter, instruction.Parameters[1].Content));
            }
            else if (parameter.Equals("RECURSIV"))
            {
                if (this.SyncManager != null)
                {
                    this.SyncManager.Configuration.IsRecursive = switcher;
                    this.WriteConfirm(
                        string.Format("{0} flag was set to {1}.", parameter, instruction.Parameters[1].Content));
                }
            }
            else if (parameter.Equals("PARALLELSYNC"))
            {
                if (this.SyncManager != null)
                {
                    this.SyncManager.Configuration.IsParallelSync = switcher;
                    this.WriteConfirm(
                        string.Format("{0} flag was set to {1}.", parameter, instruction.Parameters[1].Content));
                }
            }
        }

        /// <summary>
        /// Runs the handler.
        /// </summary>
        public void RunHandler()
        {
            while (this.isRunning)
            {
                this.output.Write(Prompt);

                string undecodedInstruction;

                try
                {
                    undecodedInstruction = this.input.ReadLine();
                }
                catch (ObjectDisposedException ex)
                {
                    // input disposed - end handler
                    Debug.WriteLine(ex.Message);
                    return;
                }
                catch (Exception ex)
                {
                    this.LogMessage(new ErrorLogMessage(ex));
                    undecodedInstruction = string.Empty;
                }

                Instruction currentInstruction = null;

                // instruction not empty - decode instruction
                if (!string.IsNullOrWhiteSpace(undecodedInstruction))
                {
                    try
                    {
                        currentInstruction = this.decoder.DecodeInstruction(undecodedInstruction);
                    }
                    catch (Exception ex)
                    {
                        this.WriteError(ex.Message);
                        currentInstruction = null;
                    }
                }

                // handle instruction
                if (currentInstruction != null)
                {
                    this.ExecuteInstruction(currentInstruction);
                }
            }
        }

        /// <summary>
        /// Called when a log file change happened.
        /// </summary>
        /// <param name="logfilename">
        /// The log file name.
        /// </param>
        protected virtual void OnLogFileChangeOccured(string logfilename)
        {
            var handler = this.LogFileChangeOccured;
            if (handler != null)
            {
                handler(
                    this, 
                    new LogFilePropertiesChangedEventArgs(logfilename, this.SyncManager.Configuration.LogFileSize));
            }
        }

        /// <summary>
        /// Called when a log file changes.
        /// </summary>
        /// <param name="logfilesize">
        /// The log file size.
        /// </param>
        protected virtual void OnLogFileChangeOccured(int logfilesize)
        {
            var handler = this.LogFileChangeOccured;
            if (handler != null)
            {
                handler(
                    this, 
                    new LogFilePropertiesChangedEventArgs(this.SyncManager.Configuration.LogFileName, logfilesize));
            }
        }

        /// <summary>
        /// Called when instruction received.
        /// </summary>
        /// <param name="e">The instruction.</param>
        protected virtual void OnInstructionOccured(Instruction e)
        {
            var handler = this.InstructionOccured;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Confirms the message.
        /// </summary>
        /// <returns>
        /// The confirmation status.
        /// </returns>
        private bool ConfirmMessage()
        {
            bool confirm = false;
            bool valid = false;

            while (!valid)
            {
                this.output.Write("Please confirm this process (OK/CANCEL):");
                string inputText = this.input.ReadLine();

                if (!string.IsNullOrWhiteSpace(inputText))
                {
                    if (inputText.Trim().ToUpper().Equals("OK"))
                    {
                        confirm = true;
                        valid = true;
                    }
                    else if (inputText.Trim().ToUpper().Equals("CANCEL"))
                    {
                        confirm = false;
                        valid = true;
                    }
                }
            }

            return confirm;
        }

        /// <summary>
        /// Handles the given instruction.
        /// </summary>
        /// <param name="instruction">
        /// The instruction.
        /// </param>
        private void ExecuteInstruction(Instruction instruction)
        {
            MethodInfo executerMethod =
                this.GetType()
                    .GetMethods()
                    .FirstOrDefault(
                        method =>
                        method.GetCustomAttributes(typeof(InstructionExecuteAttribute), false)
                            .Any(attr => ((InstructionExecuteAttribute)attr).Type == instruction.Type));

            // ReSharper disable once UseNullPropagation
            if (executerMethod != null)
            {
                executerMethod.Invoke(this, new object[] { instruction });
            }
        }

        /// <summary>
        /// Handles the add pair inputs.
        /// </summary>
        /// <returns>
        /// The <see cref="ConfigurationPair"/>.
        /// </returns>
        private ConfigurationPair HandleAddPairInputs()
        {
            const string OkString = "OK";
            const string CancelString = "CANCEL";
            string inputText;
            bool inputStatus = false;
            bool endInputGiven = false;

            string sourceFolder = string.Empty;
            List<string> targetFolders = new List<string>();
            List<string> exceptFolders = new List<string>();

            this.output.WriteLine("{0} to abort sync pair input:", CancelString);

            while (!inputStatus)
            {
                this.output.Write("Source Folder:");
                inputText = this.input.ReadLine();

                if (!string.IsNullOrWhiteSpace(inputText))
                {
                    if (inputText.ToUpper().Trim().Equals(CancelString))
                    {
                        return null;
                    }

                    if (this.ValidatePath(inputText, out inputText))
                    {
                        inputStatus = true;
                        sourceFolder = inputText;
                    }
                }
            }

            inputStatus = false;
            endInputGiven = false;

            while (!endInputGiven)
            {
                while (!inputStatus)
                {
                    if (targetFolders.Count >= 1)
                    {
                        this.output.Write("Target Folder (OK for next input stage):");
                    }
                    else
                    {
                        this.output.Write("Target Folder:");
                    }

                    inputText = this.input.ReadLine();

                    if (!string.IsNullOrWhiteSpace(inputText))
                    {
                        string checkInput = inputText.ToUpper().Trim();

                        if (checkInput.Equals(CancelString))
                        {
                            return null;
                        }

                        if (checkInput.Equals(OkString) && targetFolders.Count >= 1)
                        {
                            endInputGiven = true;
                            inputStatus = true;
                        }

                        if (!endInputGiven)
                        {
                            if (this.ValidatePath(inputText, out inputText))
                            {
                                inputStatus = true;
                                targetFolders.Add(inputText);
                            }
                        }
                    }
                }

                inputStatus = false;
            }

            inputStatus = false;
            endInputGiven = false;

            while (!endInputGiven)
            {
                while (!inputStatus)
                {
                    this.output.Write("Except Folder(OK for end input):");
                    inputText = this.input.ReadLine();

                    if (!string.IsNullOrWhiteSpace(inputText))
                    {
                        string checkInput = inputText.ToUpper().Trim();

                        if (checkInput.Equals(CancelString))
                        {
                            return null;
                        }

                        if (checkInput.Equals(OkString))
                        {
                            inputStatus = true;
                            endInputGiven = true;
                        }

                        if (!endInputGiven)
                        {
                            if (this.ValidatePath(inputText, out inputText))
                            {
                                inputStatus = true;
                                exceptFolders.Add(inputText);
                            }
                        }
                    }
                }

                inputStatus = false;
            }

            return new ConfigurationPair()
            {
                SoureFolder = sourceFolder, 
                TargetFolders = targetFolders, 
                ExceptFolders = exceptFolders, 
                Logger = this.Logger
            };
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            this.decoder = new InstructionDecoder();

            this.isRunning = true;
        }

        /// <summary>
        /// Adds the log message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        private void LogMessage(LogMessage message)
        {
            if (this.Logger != null)
            {
                this.Logger.AddLogMessage(message);
            }
        }

        /// <summary>
        /// Validates the path.
        /// </summary>
        /// <param name="inputPath">
        /// The input path.
        /// </param>
        /// <param name="fullpath">
        /// The full path.
        /// </param>
        /// <returns>
        /// The validation status.
        /// </returns>
        private bool ValidatePath(string inputPath, out string fullpath)
        {
            try
            {
                fullpath = Path.GetFullPath(inputPath);

                if (Directory.Exists(fullpath))
                {
                    return true;
                }

                this.WriteError(string.Format("{0} does not exist!", fullpath));
            }
            catch (Exception ex)
            {
                this.WriteError(ex.Message);
            }

            fullpath = string.Empty;
            return false;
        }

        /// <summary>
        /// Writes the confirm.
        /// </summary>
        /// <param name="confirmMessage">
        /// The confirm message.
        /// </param>
        private void WriteConfirm(string confirmMessage)
        {
            this.WriteMessage(confirmMessage, ConsoleColor.Green);
        }

        /// <summary>
        /// Writes the error.
        /// </summary>
        /// <param name="errorMessage">
        /// The error message.
        /// </param>
        private void WriteError(string errorMessage)
        {
            this.WriteMessage(errorMessage, ConsoleColor.Red);
        }

        /// <summary>
        /// Writes the error message to output.
        /// Fires before and after event.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        private void WriteMessage(string message, ConsoleColor color)
        {
            if (this.BeforeOutput != null)
            {
                this.BeforeOutput(this, new OutputEventArgs(color));
            }

            this.output.WriteLine(message);

            if (this.AfterOutput != null)
            {
                this.AfterOutput(this, new OutputEventArgs());
            }
        }
    }
}