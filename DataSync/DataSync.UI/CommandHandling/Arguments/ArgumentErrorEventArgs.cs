using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSync.UI.CommandHandling
{
    public class ArgumentErrorEventArgs : EventArgs
    {   
        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentErrorEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public ArgumentErrorEventArgs(string message, Exception ex)
        {
            this.Exception = ex;
            this.ErrorMessage = message;
        }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (Exception != null)
            {
                return String.Format("{0}:{1}", Exception.GetType().Name, Exception.Message);
            }

            return ErrorMessage;
        }

    }
}
