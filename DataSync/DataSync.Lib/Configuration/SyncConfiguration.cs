// -----------------------------------------------------------------------
// <copyright file="SyncConfiguration.cs" company="FH Wr.Neustadt">
//      Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>DataSync.Lib - SyncConfiguration.cs</summary>
// -----------------------------------------------------------------------
namespace DataSync.Lib.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Text;

    /// <summary>
    /// The sync configuration class.
    /// </summary>
    [Serializable]
    public class SyncConfiguration : INotifyPropertyChanged
    {
        /// <summary>
        /// The block compare file size.
        /// </summary>
        private int blockCompareFileSize;

        /// <summary>
        /// The block size.
        /// </summary>
        private int blockSize;

        /// <summary>
        /// The parallel synchronize.
        /// </summary>
        private bool isParallelSync;

        /// <summary>
        /// The recursive.
        /// </summary>
        private bool isRecursive;

        /// <summary>
        /// The log file name.
        /// </summary>
        private string logFileName;

        /// <summary>
        /// The log file size.
        /// </summary>
        private int logFileSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncConfiguration"/> class.
        /// </summary>
        public SyncConfiguration()
        {
            this.InitializeStandardValue();
        }

        /// <summary>
        /// Occurs when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the size of the block compare file.
        /// </summary>
        /// <value>
        /// The size of the block compare file.
        /// </value>
        public int BlockCompareFileSize
        {
            get
            {
                return this.blockCompareFileSize;
            }

            set
            {
                this.blockCompareFileSize = value;
                this.RaisePropertyChanged(() => this.BlockCompareFileSize);
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
            get
            {
                return this.blockSize;
            }

            set
            {
                this.blockSize = value;
                this.RaisePropertyChanged(() => this.BlockSize);
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
        /// Gets a value indicating whether this instance is log to file.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is log to file; otherwise, <c>false</c>.
        /// </value>
        public bool IsLogToFile
        {
            get
            {
                return !string.IsNullOrEmpty(this.logFileName);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether parallel synchronize is active..
        /// </summary>
        /// <value>
        /// <c>true</c> if parallel synchronize; otherwise, <c>false</c>.
        /// </value>
        public bool IsParallelSync
        {
            get
            {
                return this.isParallelSync;
            }

            set
            {
                this.isParallelSync = value;
                this.RaisePropertyChanged(() => this.IsParallelSync);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether recursive.
        /// </summary>
        /// <value>
        /// <c>true</c> if set; otherwise, <c>false</c>.
        /// </value>
        public bool IsRecursive
        {
            get
            {
                return this.isRecursive;
            }

            set
            {
                this.isRecursive = value;
                this.RaisePropertyChanged(() => this.IsRecursive);
            }
        }

        /// <summary>
        /// Gets or sets the name of the log file.
        /// </summary>
        /// <value>
        /// The name of the log file.
        /// </value>
        public string LogFileName
        {
            get
            {
                return this.logFileName;
            }

            set
            {
                this.logFileName = value;
                this.RaisePropertyChanged(() => this.LogFileName);
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
            get
            {
                return this.logFileSize;
            }

            set
            {
                this.logFileSize = value;
                this.RaisePropertyChanged(() => this.LogFileSize);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            foreach (var property in this.GetType().GetProperties())
            {
                if (!property.Name.Equals("ConfigPairs"))
                {
                    builder.AppendLine(string.Format("{0} : {1}", property.Name, property.GetValue(this)));
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        protected void RaisePropertyChanged(Expression<Func<object>> expression)
        {
            MemberExpression memberExpression = expression.Body as MemberExpression;

            if (memberExpression != null)
            {
                this.RaisePropertyChanged(memberExpression.Member.Name);
            }
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="property">
        /// The property.
        /// </param>
        protected void RaisePropertyChanged(string property)
        {
            // ReSharper disable once UseNullPropagation
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        /// Initializes the standard value.
        /// </summary>
        private void InitializeStandardValue()
        {
            this.ConfigPairs = new List<ConfigurationPair>();

            // this.IsBlockCompare = false;
            this.IsParallelSync = false;
            this.IsRecursive = true;
            this.LogFileName = null;
            this.LogFileSize = 1024 * 1024 * 2; // 2MB
            this.BlockSize = 1024;
            this.BlockCompareFileSize = 1024 * 500; // ca 500KB
        }
    }
}