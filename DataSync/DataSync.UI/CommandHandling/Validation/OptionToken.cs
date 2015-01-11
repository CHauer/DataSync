using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataSync.UI
{
    public class OptionToken : IValidationToken
    {
        public string ValidationErrorMessage
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public List<string> OptionList
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public bool Validate(object validationElement)
        {
            throw new NotImplementedException();
        }
    }
}
