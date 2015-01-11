using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataSync.UI
{
    public interface IValidationToken
    {
        string ValidationErrorMessage
        {
            get;
            set;
        }

        bool Validate(string validationElement);
    }
}
