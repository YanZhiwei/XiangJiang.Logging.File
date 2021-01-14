using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
using Serilog;
using Serilog.Core;
using Serilog.Formatting.Display;
using XiangJiang.Common;
using XiangJiang.Logging.Abstractions;

namespace XiangJiang.Logging.File
{
    /// <summary>
    ///     基于Serilog的本地日志
    ///     First released
    /// </summary>
    public sealed class FileLogService : ILogService
    {
        #region Fields

        private readonly Logger _logger;

        #endregion Fields

        #region Constructors

        public FileLogService()
            : this(null)
        {
        }

        public FileLogService(LoggerConfiguration loggerConfiguration)
        {
            _logger = (loggerConfiguration ?? CreateDefaultConfiguration()).CreateLogger();
        }

        #endregion Constructors

        #region Methods

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Dispose()
        {
            _logger?.Dispose();
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(string message, Exception ex)
        {
            _logger.Error(ex, message);
        }

        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public void Fatal(string message, Exception ex)
        {
            _logger.Fatal(ex, message);
        }

        public void Info(string message)
        {
            _logger.Information(message);
        }

        public void Warn(string message)
        {
            _logger.Warning(message);
        }

        private LoggerConfiguration CreateDefaultConfiguration()
        {
            var loggerConfig = new LoggerConfiguration();
            SetMinimumLevel(loggerConfig);
            var retainedFileCountLimit = ConfigurationManager
                .AppSettings["serilog:write-to:File.retainedFileCountLimit"].ToInt32OrDefault(31);
            var fileSizeLimitBytes = ConfigurationManager.AppSettings["serilog:write-to:File.fileSizeLimitBytes"]
                .ToInt64OrDefault(10485760);
            var rollOnFileSizeLimit = ConfigurationManager.AppSettings["serilog:write-to:File.rollOnFileSizeLimit"]
                .ToBooleanOrDefault(true);
            var fileShared = ConfigurationManager.AppSettings["serilog:write-to:File.shared"].ToBooleanOrDefault();
            var logPath = ConfigurationManager.AppSettings["serilog:write-to:File.path"]
                .ToStringOrDefault(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log",
                    ".log"));
            var rollingInterval = ConfigurationManager.AppSettings["serilog:write-to:File.rollingInterval"]
                .ToStringOrDefault("Day");
            loggerConfig.WriteTo
                .File(new MessageTemplateTextFormatter(
                        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level} {Message:lj}{NewLine}{Exception}", CultureInfo.InvariantCulture)
                    , logPath
                    , retainedFileCountLimit: retainedFileCountLimit
                    , fileSizeLimitBytes: fileSizeLimitBytes
                    , rollOnFileSizeLimit: rollOnFileSizeLimit
                    , shared: fileShared
                    , rollingInterval: Enum.Parse<RollingInterval>(rollingInterval)
                    , encoding: Encoding.UTF8);
            return loggerConfig;
        }

        private void SetMinimumLevel(LoggerConfiguration loggerConfig)
        {
            var minimumLevel = ConfigurationManager.AppSettings["serilog:minimum-level"] ?? "Debug";
            var logLevel = EnumHelper.ContainEnumName<LogLevel>(minimumLevel)
                ? minimumLevel.ParseEnumName<LogLevel>()
                : LogLevel.Debug;
            switch (logLevel)
            {
                case LogLevel.Verbose:
                    loggerConfig.MinimumLevel.Verbose();
                    break;
                case LogLevel.Debug:
                    loggerConfig.MinimumLevel.Debug();
                    break;
                case LogLevel.Information:
                    loggerConfig.MinimumLevel.Information();
                    break;
                case LogLevel.Error:
                    loggerConfig.MinimumLevel.Error();
                    break;
                case LogLevel.Warning:
                    loggerConfig.MinimumLevel.Warning();
                    break;
                case LogLevel.Fatal:
                    loggerConfig.MinimumLevel.Fatal();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion Methods
    }
}