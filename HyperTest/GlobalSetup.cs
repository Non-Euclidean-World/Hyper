using NLog.Config;

namespace HyperTest
{
    [SetUpFixture]
    public class GlobalSetup
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            var config = new XmlLoggingConfiguration("NLog.Test.config");
            NLog.LogManager.Configuration = config;
        }
    }
}
