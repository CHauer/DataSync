// -----------------------------------------------------------------------
// <copyright file="InputInstructionHandler.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - InputInstructionHandler.cs</summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
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

// ReSharper disable UnusedMember.Local

namespace DataSync.UI.CommandHandling
{
    /// <summary>
    /// 
    /// </summary>
    public class InputInstructionHandler
    {
        /// <summary>
        /// The output
        /// </summary>
        private TextWriter output;

        /// <summary>
        /// The input
        /// </summary>
        private TextReader input;

        /// <summary>
        /// The decoder
        /// </summary>
        private InstructionDecoder decoder;

        /// <summary>
        /// The is running
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// The prompt
        /// </summary>
        private const string Prompt = ">";

        /// <summary>
        /// Initializes a new instance of the <see cref="InputInstructionHandler" /> class.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        /// <exception cref="System.ArgumentNullException">input or output</exception>
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

            Initialize();
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
        /// Gets or sets the synchronize manager.
        /// </summary>
        /// <value>
        /// The synchronize manager.
        /// </value>
        public SyncManager SyncManager { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILog Logger { get; set; }

        /// <summary>
        /// Occurs before a output is written.
        /// </summary>
        public event EventHandler<OutputEventArgs> BeforeOutput;

        /// <summary>
        /// Occurs after output was written.
        /// </summary>
        public event EventHandler<OutputEventArgs> AfterOutput;

        /// <summary>
        /// Occurs when a monitor change instruction occured.
        /// </summary>
        public event EventHandler<MonitorChangeEventArgs> MonitorChangeOccured;

        /// <summary>
        /// Occurs when exit instruction occured.
        /// </summary>
        public event EventHandler ExitOccured;

        /// <summary>
        /// Occurs when help instruction occured.
        /// </summary>
        public event EventHandler HelpInstructionOccured;

        /// <summary>
        /// Occurs when help instruction occured.
        /// </summary>
        public event EventHandler<string> LogFileChangeOccured;

        /// <summary>
        /// Occurs when [instruction occured].
        /// </summary>
        public event EventHandler<Instruction> InstructionOccured;

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
                    //input disposed - end handler
                    return;
                }
                catch (Exception ex)
                {
                    //Todo log other exception
                    undecodedInstruction = string.Empty;
                }

                Instruction currentInstruction = null;

                //instruction not empty - decode instruction
                if (!String.IsNullOrWhiteSpace(undecodedInstruction))
                {
                    try
                    {
                        currentInstruction = this.decoder.DecodeInstruction(undecodedInstruction);
                    }
                    catch (Exception ex)
                    {
                        WriteError(ex.Message);
                        currentInstruction = null;
                    }
                }

                //handle instruction
                if (currentInstruction != null)
                {
                    ExecuteInstruction(currentInstruction);
                }
            }
        }

        /// <summary>
        /// Writes the error.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        private void WriteError(String errorMessage)
        {
            WriteMessage(errorMessage, ConsoleColor.Red);
        }

        /// <summary>
        /// Writes the confirm.
        /// </summary>
        /// <param name="confirmMessage">The confirm message.</param>
        private void WriteConfirm(String confirmMessage)
        {
            WriteMessage(confirmMessage, ConsoleColor.Green);
        }

        /// <summary>
        /// Writes the error message to output.
        /// Fires before and after event.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="color">The color.</param>
        private void WriteMessage(String message, ConsoleColor color)
        {
            if (BeforeOutput != null)
            {
                BeforeOutput(this, new OutputEventArgs(color));
            }

            this.output.WriteLine(message);

            if (AfterOutput != null)
            {
                AfterOutput(this, new OutputEventArgs());
            }
        }

        /// <summary>
        /// Instructions the handler_ instruction occured.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        private void ExecuteInstruction(Instruction instruction)
        {

            MethodInfo executerMethod = GetType().GetMethods()
                                        .FirstOrDefault(method => method.GetCustomAttributes(typeof(InstructionExecuteAttribute), false)
                                                                        .Any(attr => ((InstructionExecuteAttribute)attr).Type == instruction.Type));

            // ReSharper disable once UseNullPropagation
            if (executerMethod != null)
            {
                executerMethod.Invoke(this, new object[] { instruction });
            }
        }

        /// <summary>
        /// Executes the exit instruction.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        [InstructionExecute(InstructionType.EXIT)]
        public void ExecuteExitInstruction(Instruction instruction)
        {
            this.isRunning = false;

            if (ExitOccured != null)
            {
                ExitOccured(this, new EventArgs());
            }
        }

        /// <summary>
        /// Executes the help instruction.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        [InstructionExecute(InstructionType.HELP)]
        public void ExecuteHelpInstruction(Instruction instruction)
        {
            if (HelpInstructionOccured != null)
            {
                HelpInstructionOccured(this, new EventArgs());
            }
        }

        /// <summary>
        /// Executes the exit instruction.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        [InstructionExecute(InstructionType.SWITCH)]
        public void ExecuteSwitchtInstruction(Instruction instruction)
        {
            if (instruction == null) return;

            string parameter = instruction.Parameters[0].Content.ToString();
            bool switcher = instruction.Parameters[1].Content.Equals("ON");

            if (parameter.Equals("LOGVIEW") || parameter.Equals("JOBSVIEW"))
            {
                if (MonitorChangeOccured != null)
                {
                    MonitorChangeOccured(this,
                        new MonitorChangeEventArgs(parameter.Equals("LOGVIEW") ? MonitorType.Log : MonitorType.Screen,
                            switcher));
                }
            }
            else if (parameter.Equals("RECURSIV"))
            {
                if (this.SyncManager != null)
                {
                    this.SyncManager.Configuration.IsRecursiv = switcher;
                }
            }
            else if (parameter.Equals("PARALLELSYNC"))
            {
                if (this.SyncManager != null)
                {
                    this.SyncManager.Configuration.IsParrallelSync = switcher;
                }
            }
        }

        /// <summary>
        /// Executes the exit instruction.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        [InstructionExecute(InstructionType.SET)]
        public void ExecuteSetInstruction(Instruction instruction)
        {
            if (instruction == null || this.SyncManager == null) return;

            string parameter = instruction.Parameters[0].Content.ToString();
            int value;

            try
            {
                value = Convert.ToInt32(instruction.Parameters[1].Content);
            }
            catch (Exception ex)
            {
                LogMessage(new ErrorLogMessage(ex));
                return;
            }

            if (parameter.Equals("LOGSIZE"))
            {
                this.SyncManager.Configuration.LogFileSize = value;
            }
            else if (parameter.Equals("BLOCKCOMPAREFILESIZE"))
            {
                this.SyncManager.Configuration.BlockCompareFileSize = value;
            }
            else if (parameter.Equals("BLOCKSIZE"))
            {
                this.SyncManager.Configuration.BlockSize = value;
            }
        }

        /// <summary>
        /// Executes the add synchronize pair instruction.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        [InstructionExecute(InstructionType.ADDPAIR)]
        public void ExecuteAddSyncPairInstruction(Instruction instruction)
        {
            if (instruction == null || SyncManager == null) return;

            var pair = HandleAddPairInputs();

            if (pair != null)
            {
                SyncManager.Configuration.ConfigPairs.Add(pair);
            }
        }

        /// <summary>
        /// Handles the add pair inputs.
        /// </summary>
        private ConfigurationPair HandleAddPairInputs()
        {
            const string okString = "OK";
            const string cancelString = "CANCEL";
            string inputText = string.Empty;
            bool inputStatus = false;

            string sourceFolder = string.Empty;
            List<string> targetFolders = new List<string>();
            List<string> exceptFolders = new List<string>();

            this.output.WriteLine("{0} to abort sync pair input:", cancelString);
            this.output.Write("Source Folder:");

            while (!inputStatus)
            {
                inputText = this.input.ReadLine();

                if (inputText != null)
                {
                    if (inputText.ToUpper().Trim().Equals(cancelString))
                    {
                        return null;
                    }

                    if (ValidatePath(inputText))
                    {
                        sourceFolder = Path.GetFullPath(inputText);
                        inputStatus = true;
                    }
                }
            }

            inputStatus = false;

            while (!inputStatus)
            {
                this.output.Write("Target Folder:");

                while (string.IsNullOrEmpty(inputText))
                {
                    inputText = this.input.ReadLine();

                    if (inputText != null)
                    {
                        string checkInput = inputText.ToUpper().Trim();

                        if (checkInput.Equals(cancelString))
                        {
                            return null;
                        }

                        if (checkInput.Equals(okString) && targetFolders.Count >= 1)
                        {
                            inputStatus = true;
                        }

                        if (ValidatePath(inputText))
                        {
                            sourceFolder = Path.GetFullPath(inputText);
                            inputStatus = true;
                        }
                    }
                }

                this.output.Write("Target Folder (OK for next input stage):");
            }

            inputStatus = false;
            while (!inputStatus)
            {
                this.output.Write("Except Folder(OK for end input):");

                while (string.IsNullOrEmpty(inputText))
                {
                    inputText = this.input.ReadLine();

                    if (inputText != null)
                    {
                        string checkInput = inputText.ToUpper().Trim();

                        if (checkInput.Equals(cancelString))
                        {
                            return null;
                        }

                        if (checkInput.Equals(okString))
                        {
                            inputStatus = true;
                        }

                        if (ValidatePath(inputText))
                        {
                            sourceFolder = Path.GetFullPath(inputText);
                            inputStatus = true;
                        }
                    }
                }

                this.output.Write("Target Folder (OK for next input stage):");
            }

            return new ConfigurationPair()
            {
                SoureFolder = sourceFolder,
                TargetFolders = targetFolders,
                ExceptFolders = exceptFolders,
                Logger = Logger
            };
        }

        /// <summary>
        /// Validates the path.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private bool ValidatePath(string input)
        {
            string fullpath = string.Empty;

            try
            {
                fullpath = Path.GetFullPath(input);

                if (Directory.Exists(fullpath))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                WriteError(ex.Message);
            }

            return false;
        }


        /// <summary>
        /// Executes the pair clear/delete instruction.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        [InstructionExecute(InstructionType.DELETEPAIR)]
        [InstructionExecute(InstructionType.CLEARPAIRS)]
        public void ExecutePairInstruction(Instruction instruction)
        {
            if (instruction == null) { }
        }

        /// <summary>
        /// Executes the change log file instruction.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        [InstructionExecute(InstructionType.LOGTO)]
        public void ExecuteChangeLogFileInstruction(Instruction instruction)
        {
            if (instruction == null || SyncManager == null) return;


        }

        /// <summary>
        /// Executes the pair detail instruction.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        [InstructionExecute(InstructionType.LISTPAIRS)]
        [InstructionExecute(InstructionType.SHOWPAIRDETAIL)]
        public void ExecutePairDetailInstruction(Instruction instruction)
        {
            if (instruction == null) return;

            if (instruction.Type == InstructionType.SHOWPAIRDETAIL) { }
            else { }
        }

        /// <summary>
        /// Adds the log message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void LogMessage(LogMessage message)
        {
            if (Logger != null)
            {
                Logger.AddLogMessage(message);
            }
        }
    }
}