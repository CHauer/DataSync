namespace DataSync.UI.CommandHandling
{
    /// <summary>
    /// 
    /// </summary>
    public class Instruction
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public InstructionType Type { get; set; }

        /// <summary>
        /// Gets or sets the plain instruction.
        /// </summary>
        /// <value>
        /// The plain instruction.
        /// </value>
        public string PlainInstruction { get; set; }

        public List<Parameter> Parameters
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    }
}
