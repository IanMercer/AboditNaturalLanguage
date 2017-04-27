using System;
using AboditNLP;
using log4net;

namespace NLPHelloWorld
{
    /// <summary>
    /// LogFacade maps from AboditNLP's logging interface to log4net
    /// </summary>
    /// <remarks>
    /// Use this class if you are using log4net and want logging from AboditNLP
    /// or create your own inheriting from ILogger to map Abodit NLP's calls to
    /// log to whatever logging framework you have chosen.
    /// 
    /// The ILogger interface keeps AboditNLP independent of logging framework.
    /// </remarks>
    internal class LogFacade : ILogger
    {
        private static readonly ILog log = LogManager.GetLogger("NLP");

        public void Debug(string message)
        {
            log.Debug(message);
        }

        public void Error(string message, Exception ex)
        {
            log.Error(message, ex);
        }

        public void Fatal(string message)
        {
            log.Fatal(message);
        }

        public void Info(string message)
        {
            log.Info(message);
        }

        public void Trace(string message)
        {
            log.Debug(message);
        }

        public void Warn(string message)
        {
            log.Warn(message);
        }
    }
}