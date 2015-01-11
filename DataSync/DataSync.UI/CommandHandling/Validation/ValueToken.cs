using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataSync.UI
{
    public class ValueToken : IValidationToken
    {
        public string ValidationErrorMessage { get; set; }

        public Type TargetType { get; set; }

        public bool Validate(string validationElement)
        {
            throw new NotImplementedException();
        }
    }
}
