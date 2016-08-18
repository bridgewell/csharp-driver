﻿using System.Diagnostics;
using System.Globalization;
using System.Threading;
using BWCassandra.IntegrationTests.TestBase;
using BWCassandra.IntegrationTests.TestClusterManagement;
using NUnit.Framework;

namespace BWCassandra.IntegrationTests
{
    [SetUpFixture]
    public class CommonFixtureSetup : TestGlobals
    {
        [SetUp]
        public void SetupTestSuite()
        {
            Diagnostics.CassandraTraceSwitch.Level = TraceLevel.Info;
            Trace.TraceInformation("TestBase Setup Complete. Starting Test Run ...");
        }

        [TearDown]
        public void TearDownTestSuite()
        {
            // this method is executed once after all the fixtures have completed execution
            TestClusterManager.TryRemove();
        }
    }
}
