using Caliburn.Micro;
using System;

namespace RMD.Excercise.Logger
{
    /// <summary>
    /// Logger wrapper for NLog
    /// </summary>
    public class NLogLogger : ILog
    {
        #region Fields

        private readonly NLog.Logger _innerLogger;

        #endregion

        #region Constructors

        public NLogLogger(Type type)
        {
            _innerLogger = NLog.LogManager.GetLogger(type.Name);
        }

        #endregion

        #region ILog Members

        public void Error(Exception exception)
        {
            _innerLogger.Error(exception, exception.Message);
        }

        public void Warn(string format, params object[] args)
        {
            _innerLogger.Warn(format, args);
        }
        
        public void Info(string format, params object[] args)
        {
            _innerLogger.Info(format, args);
        }

        #endregion
    }
}
