using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSync.Lib.Sync.Operations
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public class SupportedTypeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedTypeAttribute" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public SupportedTypeAttribute(Type type)
        {
            this.SupportedType = type;
        }

        /// <summary>
        /// Gets the type of the supported.
        /// </summary>
        /// <value>
        /// The type of the supported.
        /// </value>
        public Type SupportedType { get; private set; }
    }
}
