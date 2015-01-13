using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataSync.UI.CommandHandling.Decoder;

namespace DataSync.UI.CommandHandling
{
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
        /// Initializes a new instance of the <see cref="InputInstructionHandler"/> class.
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
            decoder = new InstructionDecoder();
        }

        /// <summary>
        /// Occurs before an error output.
        /// </summary>
        public event EventHandler BeforeErrorOutput;

        /// <summary>
        /// Occurs after an error output.
        /// </summary>
        public event EventHandler AfterErrorOutput;

        public event EventHandler<OutputEventArgs> BeforeOutput;

        public event EventHandler<OutputEventArgs> AfterOutput;

        public event EventHandler<MonitorChangeEventArgs> MonitorChangeOccured;

        /// <summary>
        /// Occurs when exit instruction occured.
        /// </summary>
        public event EventHandler ExitOccured;

        /// <summary>
        /// Occurs when help instruction occured.
        /// </summary>
        public event EventHandler HelpInstructionOccured;

        public event EventHandler<Instruction> InstructionOccured;

        /// <summary>
        /// Runs the handler.
        /// </summary>
        public void RunHandler()
        {
            Instruction currentInstruction = null;
            string undecodedInstruction = string.Empty;

            isRunning = true;

            while (isRunning)
            {
                output.Write(Prompt);

                try
                {
                    undecodedInstruction = input.ReadLine();
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

                //instruction not empty - decode
                if (!String.IsNullOrWhiteSpace(undecodedInstruction))
                {

                    try
                    {
                        currentInstruction = decoder.DecodeInstruction(undecodedInstruction);
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
                    switch (currentInstruction.Type)
                    {
                        case InstructionType.EXIT:
                            isRunning = false;

                            if (ExitOccured != null)
                            {
                                ExitOccured(this, new EventArgs());
                            }
                            break;
                        case InstructionType.HELP:
                            if (HelpInstructionOccured != null)
                            {
                                HelpInstructionOccured(this, new EventArgs());
                            }
                            break;
                        case InstructionType.ADDPAIR:
                            HandleAddPairInputs();
                            break;
                        case InstructionType.SWITCH:
                            string parameter = currentInstruction.Parameters[0].Content;

                            if (parameter.Equals("LOGVIEW") || parameter.Equals("JOBSVIEW"))
                            {
                                break;
                            }

                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the add pair inputs.
        /// </summary>
        private void HandleAddPairInputs()
        {
            const string okString = "OK";
            const string cancelString = "CANCEL";
            string inputText = string.Empty;
            bool okInput = false;

            string sourceFolder = string.Empty;
            List<string> targetFolders = new List<string>();
            List<string> exceptFolders = new List<string>();


            output.WriteLine("{0} to abort sync pair input:", cancelString);
            output.Write("Source Folder:");

            while(string.IsNullOrEmpty(inputText))
            {
                inputText = input.ReadLine();

                if (inputText != null && inputText.ToUpper().Trim().Equals(cancelString))
                {
                    return;
                }
            }

            while (!okInput)
            {
                output.Write("Target Folder:");

                while (string.IsNullOrEmpty(inputText))
                {
                    inputText = input.ReadLine();

                    if (inputText != null)
                    {
                        string checkInput = inputText.ToUpper().Trim();
                        if (checkInput.Equals(cancelString))
                        {
                            return;
                        }
                        
                        if (checkInput.Equals(okString) && targetFolders.Count >= 1)
                        {
                            okInput = true;
                        }
                    }
                }

                output.Write("Target Folder (OK for next input stage):");
            }

            output.Write("Except Folder(OK for end input):");
        }

        /// <summary>
        /// Writes the error message to output.
        /// Fires before and after event.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        private void WriteError(String errorMessage)
        {
            if (BeforeErrorOutput != null)
            {
                BeforeErrorOutput(this, new EventArgs());
            }

            output.WriteLine(errorMessage);

            if (AfterErrorOutput != null)
            {
                AfterErrorOutput(this, new EventArgs());
            }
        }
    }
}
