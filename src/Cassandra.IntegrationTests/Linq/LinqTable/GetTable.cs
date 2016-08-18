using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BWCassandra.Data.Linq;
using BWCassandra.IntegrationTests.Linq.Structures;
using BWCassandra.IntegrationTests.TestBase;
using BWCassandra.Mapping;
using NUnit.Framework;

namespace BWCassandra.IntegrationTests.Linq.LinqTable
{
    /// <summary>
    /// NOTE: The GetTable() method is deprecated.
    /// </summary>
    [Category("short")]
    public class GetTable : SharedClusterTest
    {
        string _uniqueKsName;

        protected override void TestFixtureSetUp()
        {
            base.TestFixtureSetUp();
            _uniqueKsName = TestUtils.GetUniqueKeyspaceName();
            Session.CreateKeyspace(_uniqueKsName);
            TestUtils.WaitForSchemaAgreement(Session.Cluster);
            Session.ChangeKeyspace(_uniqueKsName);
        }

        /// <summary>
        /// Get table using GetTable, validate that the resultant table object functions as expected
        /// </summary>
        [Test]
        public void LinqTable_GetTable()
        {
            // Test
            Table<AllDataTypesEntity> table = Session.GetTable<AllDataTypesEntity>();
            table.Create();
            AllDataTypesEntity.WriteReadValidate(table);
        }
    }
}
