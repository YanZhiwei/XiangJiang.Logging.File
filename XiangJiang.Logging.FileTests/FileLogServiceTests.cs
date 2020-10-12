using Microsoft.VisualStudio.TestTools.UnitTesting;
using XiangJiang.Logging.Abstractions;
using XiangJiang.Logging.File;

namespace XiangJiang.Logging.FileTests
{
    [TestClass]
    public class FileLogServiceTests
    {
        private ILogService _logService;

        [TestInitialize]
        public void Init()
        {
            _logService = new FileLogService();
        }

        [TestMethod]
        public void LogTest()
        {
            _logService.Debug("serilog debugTest");
            _logService.Info("serilog InfoTest");
            _logService.Error("serilog ErrorTest");
            _logService.Fatal("serilog FatalTest");
            _logService.Warn("serilog WarnTest");
        }

        [TestCleanup]
        public void CleanUp()
        {
            _logService.Dispose();
        }
    }
}