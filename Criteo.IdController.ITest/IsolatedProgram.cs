using System.Collections.Generic;
using System.Linq;
using Criteo.Testing.Isolation;
using Criteo.Testing.Isolation.AspNetCore;
using Sdk.ProductionResources.Consul;

namespace Criteo.IdController.ITest
{
    class IsolatedProgram
    {
        public static IEnumerable<SqlDatabaseService> dbsRequiredToStart = Enumerable.Empty<SqlDatabaseService>();

        private static void Main(string[] args)
        {
            using (var inProcessTestHelper = new InProcessTestHelper(nameof(IsolatedProgram)))
            {
                inProcessTestHelper.Init(dbsRequiredToStart);
                var isolatedHostBuilder = inProcessTestHelper.CreateAspNetCoreBuilder(Program.ConfigureApp);

                var program = new IsolatedAspNetCoreProgram(args, isolatedHostBuilder);
                program.RunHost();
            }
        }
    }
}
