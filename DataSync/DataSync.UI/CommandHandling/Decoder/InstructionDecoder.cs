﻿// -----------------------------------------------------------------------
// <copyright file="InstructionDecoder.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.UI - InstructionDecoder.cs</summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using DataSync.UI.CommandHandling.Instructions;
using DataSync.UI.CommandHandling.Validation;

namespace DataSync.UI.CommandHandling.Decoder
{
    public class InstructionDecoder
    {
        /// <summary>
        /// The instructions with out parameters
        /// </summary>
        private List<InstructionType> instructionsWithOutParameters;

        /// <summary>
        /// The validation error message.
        /// </summary>
        private string validationErrorMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstructionDecoder"/> class.
        /// </summary>
        public InstructionDecoder()
        {
            Initialize();
        }

        /// <summary>
        /// Intializes this instance.
        /// </summary>
        private void Initialize()
        {
            this.instructionsWithOutParameters = new List<InstructionType>()
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

                if (!string.IsNullOrWhiteSpace(parameterPart))
                {
                    //split parameters
                    parameters = parameterPart.Split(new char[] { ' ' });
                }
            }
            else
            {
                //instruction without parameters
                instructionPart = undecoded.ToUpper();
            }

            try
            {
                //get instruction type from enum
                irCode = (InstructionType) Enum.Parse(typeof (InstructionType), instructionPart);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(String.Format("Unknown Command {0}.", instructionPart));
            }

            //check if instruction is valid 
            if (!ValidateInstruction(irCode, parameters))
            {
                throw new InvalidOperationException(String.Format("The command {0} is invalid.\nDetails: {1}", undecoded,
                    this.validationErrorMessage));
            }

            //Create Instruction object and set parameters
            var instruction = new Instruction()
            {
                PlainInstruction = undecoded,
                Type = irCode,
                Parameters = CreateParameters(irCode, parameters)
            };

            return instruction;
        }

        /// <summary>
        /// Creates the parameters.
        /// </summary>
        /// <param name="irCode">The ir code.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        private List<Parameter> CreateParameters(InstructionType irCode, string[] parameters)
        {
            List<Parameter> returnParams = new List<Parameter>();
            switch (irCode)
            {
                case InstructionType.ADDPAIR:
                case InstructionType.DELETEPAIR:
                case InstructionType.LOGTO:
                case InstructionType.SHOWPAIRDETAIL:

                    if (parameters[0].StartsWith("\"") || parameters[0].EndsWith("\""))
                    {
                        parameters[0] = parameters[0].Replace("\"", string.Empty);
                    }

                    if (parameters[0].StartsWith("\'") || parameters[0].EndsWith("\'"))
                    {
                        parameters[0] = parameters[0].Replace("\'", string.Empty);
                    }

                    returnParams.Add(new Parameter()
                    {
                        Content = parameters[0],
                        Type = ParameterType.StringValue
                    });

                    break;

                case InstructionType.SWITCH:
                    returnParams.Add(new Parameter()
                    {
                        Content = parameters[0],
                        Type = ParameterType.StringValue
                    });
                    returnParams.Add(new Parameter()
                    {
                        Content = parameters[1],
                        Type = ParameterType.StringValue
                    });

                    break;

                case InstructionType.SET:
                    returnParams.Add(new Parameter()
                    {
                        Content = parameters[0],
                        Type = ParameterType.StringValue
                    });
                    returnParams.Add(new Parameter()
                    {
                        Content = parameters[1],
                        Type = ParameterType.IntegerValue
                    });

                    break;
            }

            return returnParams;
        }

        /// <summary>
        /// Validates the instruction.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        private bool ValidateInstruction(InstructionType type, string[] parameters)
        {
            List<IValidationToken> validationTokens = null;

            if (this.instructionsWithOutParameters.Contains(type))
            {
                return true;
            }

            switch (type)
            {
                case InstructionType.ADDPAIR:
                    validationTokens = new List<IValidationToken>()
                    {
                        new IdentifierToken() {Regex = "[A-Za-z0-9]+"}
                    };
                    break;
                case InstructionType.DELETEPAIR:
                case InstructionType.SHOWPAIRDETAIL:
                    validationTokens = new List<IValidationToken>()
                    {
                        new IdentifierToken() {Regex = "[A-Za-z0-9]+"}
                    };
                    break;
                case InstructionType.SWITCH:
                    validationTokens = new List<IValidationToken>()
                    {
                        new OptionToken()
                        {
                            OptionList = new List<string>() {"RECURSIV", "PARALLELSYNC", "LOGVIEW", "JOBSVIEW"}
                        },
                        new SwitchToken()
                    };
                    break;
                case InstructionType.SET:
                    validationTokens = new List<IValidationToken>()
                    {
                        new OptionToken()
                        {
                            OptionList = new List<string>() {"LOGSIZE", "BLOCKCOMPAREFILESIZE", "BLOCKSIZE"}
                        },
                        new ValueToken()
                        {
                            TargetType = typeof (Int32)
                        }
                    };
                    break;
                case InstructionType.LOGTO:
                    validationTokens = new List<IValidationToken>()
                    {
                        //file path
                        new PathToken()
                    };
                    break;
            }

            //validate tokens
            if (validationTokens != null && parameters.Length > 0 && validationTokens.Count == parameters.Length)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (!validationTokens[i].Validate(parameters[i]))
                    {
                        this.validationErrorMessage = validationTokens[i].ValidationErrorMessage;
                        return false;
                    }
                }
            }
            else
            {
                this.validationErrorMessage = string.Format("Missing Parameters for {0}", type.ToString("g"));
                return false;
            }

            return true;
        }
    }
}