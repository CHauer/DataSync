using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataSync.UI
{
    public class IdentifierToken : IValidationToken
    {
        public string ValidationErrorMessage { get; set; }

        public string Regex { get; set; }

        public bool Validate(string validationElement)
        {
            throw new NotImplementedException();
        }
    }
}
