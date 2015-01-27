using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace DataSync.Lib.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SyncConfiguration : INotifyPropertyChanged
    {
        /// <summary>
        /// The block compare file size
        /// </summary>
        private int blockCompareFileSize;

        /// <summary>
        /// The block size
        /// </summary>
        private int blockSize;

        /// <summary>
        /// The isblock compare
        /// </summary>
        private bool isBlockCompare;

        /// <summary>
        /// The recursiv
        /// </summary>
        private bool isRecursiv;

        /// <summary>
        /// The isLogToFile
        /// </summary>
        private bool isLogToFile;

        /// <summary>
        /// The log file size
        /// </summary>
        private int logFileSize;

        /// <summary>
        /// The log file name
        /// </summary>
        private string logFileName;

        /// <summary>
        /// The parallell synchronize
        /// </summary>
        private bool isParallellSync;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncConfiguration"/> class.
        /// </summary>
        public SyncConfiguration()
        {
            InitializeStandardValue();
        }

        /// <summary>
        /// Gets or sets the size of the block compare file.
        /// </summary>
        /// <value>
        /// The size of the block compare file.
        /// </value>
        public int BlockCompareFileSize
        {
            get { return blockCompareFileSize; }
            set
            {
                blockCompareFileSize = value;
                RaisePropertyChanged(() => BlockCompareFileSize);
            }
        }

        /// <summary>
        /// Gets or sets the size of the block.
        /// </summary>
        /// <value>
        /// The size of the block.
        /// </value>
        public int BlockSize
        {
            get { return blockSize; }
            set
            {
                blockSize = value;
                RaisePropertyChanged(() => BlockSize);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [recursiv].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [recursiv]; otherwise, <c>false</c>.
        /// </value>
        public bool IsRecursiv
        {
            get { return isRecursiv; }
            set
            {
                isRecursiv = value;
                RaisePropertyChanged(() => IsRecursiv);
            }
        }

        ///// <summary>
        ///// Gets or sets a value indicating whether [is block compare].
        ///// </summary>
        ///// <value>
        /////   <c>true</c> if [is block compare]; otherwise, <c>false</c>.
        ///// </value>
        //public bool IsBlockCompare
        //{
        //    get { return isBlockCompare; }
        //    set
        //    {
        //        isBlockCompare = value;
        //        RaisePropertyChanged(() => IsBlockCompare);
        //    }
        //}

        /// <summary>
        /// Gets or sets a value indicating whether this instance is log to file.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is log to file; otherwise, <c>false</c>.
        /// </value>
        public bool IsLogToFile
        {
            get
            {
                return !string.IsNullOrEmpty(logFileName);
            }
        }

        /// <summary>
        /// Gets or sets the size of the log file.
        /// </summary>
        /// <value>
        /// The size of the log file.
        /// </value>
        public int LogFileSize
        {
            get { return logFileSize; }
            set
            {
                logFileSize = value;
                RaisePropertyChanged(() => LogFileSize);
            }
        }

        /// <summary>
        /// Gets the configuration pairs.
        /// </summary>
        /// <value>
        /// The configuration pairs.
        /// </value>
        public List<ConfigurationPair> ConfigPairs { get; private set; }

        /// <summary>
        /// Gets or sets the name of the log file.
        /// </summary>
        /// <value>
        /// The name of the log file.
        /// </value>
        public string LogFileName
        {
            get { return logFileName; }
            set
            {
                logFileName = value;
                RaisePropertyChanged(() => LogFileName);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [parrallel synchronize].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [parrallel synchronize]; otherwise, <c>false</c>.
        /// </value>
        public bool IsParrallelSync
        {
            get { return isParallellSync; }
            set
            {
                isParallellSync = value;
                RaisePropertyChanged(() => IsParrallelSync);
            }
        }

        /// <summary>
        /// Tritt ein, wenn sich ein Eigenschaftswert ändert.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes the standard value.
        /// </summary>
        private void InitializeStandardValue()
        {
            ConfigPairs = new List<ConfigurationPair>();

            //this.IsBlockCompare = false;
            this.IsParrallelSync = false;
            this.IsRecursiv = true;
            this.LogFileName = null;
            this.LogFileSize = 1024 * 1024 * 2; //2MB
            this.BlockSize = 1024;
            this.BlockCompareFileSize = 1024 * 500; //ca 500KB
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="expression">The expression.</param>
        protected void RaisePropertyChanged(Expression<Func<object>> expression)
        {
            MemberExpression memberExpression = expression.Body as MemberExpression;

            if (memberExpression != null)
            {
                RaisePropertyChanged(memberExpression.Member.Name);
            }
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="property">The property.</param>
        protected void RaisePropertyChanged(string property)
        {
            // ReSharper disable once UseNullPropagation
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            foreach (var property in GetType().GetProperties())
            {
                if (!property.Name.Equals("ConfigPairs"))
                {
                    builder.AppendLine(String.Format("{0} : {1}", property.Name, property.GetValue(this)));
                }
            }

            return builder.ToString();
        }
    }
}
