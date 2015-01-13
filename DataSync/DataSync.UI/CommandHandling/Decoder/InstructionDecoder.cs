using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataSync.UI.CommandHandling.Decoder
{
    public class InstructionDecoder
    {
        /// <summary>
        /// The instructions with out parameters
        /// </summary>
        private List<InstructionType> instructionsWithOutParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstructionDecoder"/> class.
        /// </summary>
        public InstructionDecoder()
        {
            Intialize();
        }

        /// <summary>
        /// Intializes this instance.
        /// </summary>
        private void Intialize()
        {
            instructionsWithOutParameters = new List<InstructionType>()
            {
                InstructionType.EXIT, 
                InstructionType.HELP, 
                InstructionType.CLEARPAIRS,
                InstructionType.LISTPAIRS 
            };
        }

        /// <summary>
        /// Decodes the instruction.
        /// </summary>
        /// <param name="undecoded">The undecoded.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        public Instruction DecodeInstruction(string undecoded)
        {
            string instructionPart;
            string[] parameters = new string[0];
            InstructionType irCode;

            if (undecoded.Contains(' '))
            {
                //Get instruction part and to upper
                instructionPart = undecoded.Substring(0, undecoded.IndexOf(' ')).ToUpper();

                //extract parameter part
                String parameterPart = undecoded.Substring(undecoded.IndexOf(' ') + 1).Trim();

                //combine to upper instruction part with parameterpart
                undecoded = String.Format("{0} {1}", instructionPart, parameterPart);

                //split parameters
                parameters = parameterPart.Split(new char[] { ' ' });
            }
            else
            {
                //instruction without parameters
                instructionPart = undecoded.ToUpper();
            }

            try
            {
                //get instruction type from enum
                irCode = (InstructionType)Enum.Parse(typeof(InstructionType), instructionPart);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(String.Format("Unknown Command {0}.", instructionPart));
            }

            //check if instruction is valid 
            if (!ValidateInstruction(irCode, parameters))
            {
                throw new InvalidOperationException(String.Format("The command {0} is invalid.", undecoded));
            }

            //Create Instruction object and set parameters
            var instruction = new Instruction()
            {
                PlainInstruction = undecoded,
                Type = irCode
            };

            //set parameters

            return instruction;
        }

        /// <summary>
        /// Validates the instruction.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        private bool ValidateInstruction(InstructionType type, string[] parameters)
        {
            List<IValidationToken> validationTokens;

            if (instructionsWithOutParameters.Contains(type))
            {
                return true;
            }

            switch (type)
            {
                case InstructionType.ADDPAIR:

                    break;
                case InstructionType.DELETEPAIR:
                case InstructionType.SHOWPAIRDETAIL:
                    validationTokens = new List<IValidationToken>()
                    {
                        new IdentifierToken(){Regex = "[A-Za-z0-9]+"}
                    };
                    break;
                case InstructionType.SWITCH:
                    validationTokens = new List<IValidationToken>()
                    {
                        new OptionToken(){
                            OptionList = new List<string>(){"RECURSIV", "PARALLELSYNC", "LOGVIEW", "JOBSVIEW"}
                        },
                        new SwitchToken()
                    };
                    break;
                case InstructionType.SET:
                    validationTokens = new List<IValidationToken>()
                    {
                        new OptionToken(){ 
                            OptionList = new List<string>(){"LOGSIZE", "BLOCKCOMPAREFILESIZE", "BLOCKSIZE"}
                        },
                        new ValueToken(){
                            TargetType = typeof(Int32) 
                        }
                    };
                    break;
                case InstructionType.LOGTO:
                    validationTokens = new List<IValidationToken>()
                    {
                        new IdentifierToken(){Regex = "[A-Za-z0-9.]+"}
                    };
                    break;
            }

            //TODO validate tokens

            return true;
        }

    }
}
