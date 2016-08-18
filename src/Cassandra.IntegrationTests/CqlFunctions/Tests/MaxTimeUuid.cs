﻿using System;
using System.Collections.Generic;
using System.Linq;
using BWCassandra.Data.Linq;
using BWCassandra.IntegrationTests.CqlFunctions.Structures;
using BWCassandra.IntegrationTests.TestBase;
using BWCassandra.Mapping;
using NUnit.Framework;
#pragma warning disable 612

namespace BWCassandra.IntegrationTests.CqlFunctions.Tests
{
    [Category("short")]
    public class MaxTimeUuid : SharedClusterTest
    {
        private ISession _session = null;
        private List<EntityWithTimeUuid> _expectedTimeUuidObjectList;
        private List<EntityWithNullableTimeUuid> _expectedNullableTimeUuidObjectList;
        private string _uniqueKsName = TestUtils.GetUniqueKeyspaceName();
        private Table<EntityWithTimeUuid> _tableEntityWithTimeUuid;
        private Table<EntityWithNullableTimeUuid> _tableEntityWithNullableTimeUuid;

        private DateTimeOffset _dateBefore;
        private DateTimeOffset _dateAfter;
        private const int DefaultRecordCount = 6;

        protected override void TestFixtureSetUp()
        {
            base.TestFixtureSetUp();
            _session = Session;
            _session.CreateKeyspace(_uniqueKsName);
            _session.ChangeKeyspace(_uniqueKsName);

            // Create necessary tables
            MappingConfiguration config1 = new MappingConfiguration();
            config1.MapperFactory.PocoDataFactory.AddDefinitionDefault(typeof(EntityWithTimeUuid),
                () => LinqAttributeBasedTypeDefinition.DetermineAttributes(typeof(EntityWithTimeUuid)));
            _tableEntityWithTimeUuid = new Table<EntityWithTimeUuid>(_session, config1);
            _tableEntityWithTimeUuid.Create();

            MappingConfiguration config2 = new MappingConfiguration();
            config2.MapperFactory.PocoDataFactory.AddDefinitionDefault(typeof(EntityWithNullableTimeUuid),
                () => LinqAttributeBasedTypeDefinition.DetermineAttributes(typeof(EntityWithNullableTimeUuid)));
            _tableEntityWithNullableTimeUuid = new Table<EntityWithNullableTimeUuid>(_session, config2);
            _tableEntityWithNullableTimeUuid.Create();

            _expectedTimeUuidObjectList = EntityWithTimeUuid.GetDefaultObjectList();
            _expectedNullableTimeUuidObjectList = EntityWithNullableTimeUuid.GetDefaultObjectList();

            _dateBefore = DateTimeOffset.Parse("2014-2-1");
            _dateAfter = DateTimeOffset.Parse("2014-4-1");
        }

        /// <summary>
        /// Validate that the LinqUtility function MaxTimeUuid, which corresponds to the CQL query maxTimeuuid
        /// functions as expected when using a 'greater than' comparison, comparing TimeUuid values
        /// </summary>
        [Test]
        public void MaxTimeUuid_GreaterThan_TimeUuidComparison()
        {
            EntityWithTimeUuid.SetupEntity(_tableEntityWithTimeUuid, _expectedTimeUuidObjectList);

            EntityWithTimeUuid defaultEntity = new EntityWithTimeUuid();
            var whereQuery = _tableEntityWithTimeUuid.Where(s => s.TimeUuidType > CqlFunction.MaxTimeUuid(_dateBefore) && s.StringType == defaultEntity.StringType);
            List<EntityWithTimeUuid> objectsReturned1 = whereQuery.ExecuteAsync().Result.ToList();
            Assert.AreEqual(_expectedTimeUuidObjectList.Count, objectsReturned1.Count);

            foreach (var actualObj in objectsReturned1)
                EntityWithTimeUuid.AssertListContains(_expectedTimeUuidObjectList, actualObj);

            whereQuery = _tableEntityWithTimeUuid.Where(s => s.TimeUuidType > CqlFunction.MaxTimeUuid(_dateAfter));
            List<EntityWithTimeUuid> objectsReturned2 = whereQuery.ExecuteAsync().Result.ToList();
            Assert.AreEqual(0, objectsReturned2.Count);
        }

        /// <summary>
        /// Validate that the LinqUtility function MaxTimeUuid, which corresponds to the CQL query maxTimeuuid
        /// functions as expected when using a 'greater than' comparison, comparing NullableTimeUuid values
        /// </summary>
        [Test]
        public void MaxTimeUuid_GreaterThanOrEqualTo_TimeUuidComparison()
        {
            EntityWithTimeUuid.SetupEntity(_tableEntityWithTimeUuid, _expectedTimeUuidObjectList);

            var whereQuery = _tableEntityWithTimeUuid.Where(s => s.TimeUuidType >= CqlFunction.MaxTimeUuid(_dateBefore));
            List<EntityWithTimeUuid> objectsReturned1 = whereQuery.ExecuteAsync().Result.ToList();
            Assert.AreEqual(_expectedTimeUuidObjectList.Count, objectsReturned1.Count);

            foreach (var actualObj in objectsReturned1)
                EntityWithTimeUuid.AssertListContains(_expectedTimeUuidObjectList, actualObj);

            whereQuery = _tableEntityWithTimeUuid.Where(s => s.TimeUuidType >= CqlFunction.MaxTimeUuid(_dateAfter));
            List<EntityWithTimeUuid> objectsReturned2 = whereQuery.ExecuteAsync().Result.ToList();
            Assert.AreEqual(0, objectsReturned2.Count);

        }

        /// <summary>
        /// Validate that the LinqUtility function MaxTimeUuid, which corresponds to the CQL query maxTimeuuid
        /// functions as expected when using a 'less than' comparison, comparing TimeUuid values
        /// </summary>
        [Test]
        public void MaxTimeUuid_LessThan_TimeUuidComparison()
        {
            EntityWithTimeUuid.SetupEntity(_tableEntityWithTimeUuid, _expectedTimeUuidObjectList);

            var whereQuery = _tableEntityWithTimeUuid.Where(s => s.TimeUuidType < CqlFunction.MaxTimeUuid(_dateAfter));
            List<EntityWithTimeUuid> objectsReturned1 = whereQuery.ExecuteAsync().Result.ToList();
            Assert.AreEqual(_expectedTimeUuidObjectList.Count, objectsReturned1.Count);

            foreach (var actualObj in objectsReturned1)
                EntityWithTimeUuid.AssertListContains(_expectedTimeUuidObjectList, actualObj);

            whereQuery = _tableEntityWithTimeUuid.Where(s => s.TimeUuidType < CqlFunction.MaxTimeUuid(_dateBefore));
            List<EntityWithTimeUuid> objectsReturned2 = whereQuery.ExecuteAsync().Result.ToList();
            Assert.AreEqual(0, objectsReturned2.Count);

        }

        /// <summary>
        /// Validate that the LinqUtility function MaxTimeUuid, which corresponds to the CQL query maxTimeuuid
        /// functions as expected when using a 'less than' comparison, comparing TimeUuid values
        /// </summary>
        [Test]
        public void MaxTimeUuid_LessThanOrEqualTo_TimeUuidComparison()
        {
            EntityWithTimeUuid.SetupEntity(_tableEntityWithTimeUuid, _expectedTimeUuidObjectList);

            var whereQuery = _tableEntityWithTimeUuid.Where(s => s.TimeUuidType <= CqlFunction.MaxTimeUuid(_dateAfter));
            List<EntityWithTimeUuid> objectsReturned1 = whereQuery.ExecuteAsync().Result.ToList();
            Assert.AreEqual(_expectedTimeUuidObjectList.Count, objectsReturned1.Count);

            foreach (var actualObj in objectsReturned1)
                EntityWithTimeUuid.AssertListContains(_expectedTimeUuidObjectList, actualObj);

            whereQuery = _tableEntityWithTimeUuid.Where(s => s.TimeUuidType <= CqlFunction.MaxTimeUuid(_dateBefore));
            List<EntityWithTimeUuid> objectsReturned2 = whereQuery.ExecuteAsync().Result.ToList();
            Assert.AreEqual(0, objectsReturned2.Count);

        }

        /////////////////////////////////////////
        /// NullableTimeUuid Comparisons
        /////////////////////////////////////////

        /// <summary>
        /// Validate that the LinqUtility function MaxTimeUuid, which corresponds to the CQL query maxTimeuuid
        /// functions as expected when using a 'greater than' comparison, comparing NullableTimeUuid values
        /// </summary>
        [Test]
        public void MaxTimeUuid_GreaterThan_NullableTimeUuidComparison()
        {
            EntityWithNullableTimeUuid.SetupEntity(_tableEntityWithNullableTimeUuid, _expectedNullableTimeUuidObjectList);

            EntityWithNullableTimeUuid defaultEntity = new EntityWithNullableTimeUuid();
            var whereQuery = _tableEntityWithNullableTimeUuid.Where(s => s.NullableTimeUuidType > CqlFunction.MaxTimeUuid(_dateBefore) && s.StringType == defaultEntity.StringType);
            List<EntityWithNullableTimeUuid> objectsReturned1 = whereQuery.ExecuteAsync().Result.ToList();
            Assert.AreEqual(_expectedTimeUuidObjectList.Count, objectsReturned1.Count);

            foreach (var actualObj in objectsReturned1)
                EntityWithNullableTimeUuid.AssertListContains(_expectedNullableTimeUuidObjectList, actualObj);

            whereQuery = _tableEntityWithNullableTimeUuid.Where(s => s.NullableTimeUuidType > CqlFunction.MaxTimeUuid(_dateAfter));
            List<EntityWithNullableTimeUuid> objectsReturned2 = whereQuery.ExecuteAsync().Result.ToList();
            Assert.AreEqual(0, objectsReturned2.Count);
        }

        /// <summary>
        /// Validate that the LinqUtility function MaxTimeUuid, which corresponds to the CQL query maxTimeuuid
        /// functions as expected when using a 'greater than or equal to' comparison, comparing NullableTimeUuid values
        /// </summary>
        [Test]
        public void MaxTimeUuid_GreaterThanOrEqualTo_NullableTimeUuidComparison()
        {
            EntityWithNullableTimeUuid.SetupEntity(_tableEntityWithNullableTimeUuid, _expectedNullableTimeUuidObjectList);

            EntityWithNullableTimeUuid defaultEntity = new EntityWithNullableTimeUuid();
            var whereQuery = _tableEntityWithNullableTimeUuid.Where(s => s.NullableTimeUuidType >= CqlFunction.MaxTimeUuid(_dateBefore) && s.StringType == defaultEntity.StringType);
            List<EntityWithNullableTimeUuid> objectsReturned1 = whereQuery.ExecuteAsync().Result.ToList();
            Assert.AreEqual(_expectedTimeUuidObjectList.Count, objectsReturned1.Count);

            foreach (var actualObj in objectsReturned1)
                EntityWithNullableTimeUuid.AssertListContains(_expectedNullableTimeUuidObjectList, actualObj);

            whereQuery = _tableEntityWithNullableTimeUuid.Where(s => s.NullableTimeUuidType >= CqlFunction.MaxTimeUuid(_dateAfter));
            List<EntityWithNullableTimeUuid> objectsReturned2 = whereQuery.ExecuteAsync().Result.ToList();
            Assert.AreEqual(0, objectsReturned2.Count);
        }

        /// <summary>
        /// Validate that the LinqUtility function MaxTimeUuid, which corresponds to the CQL query maxTimeuuid
        /// functions as expected when using a 'less than' comparison, comparing NullableTimeUuid values
        /// </summary>
        [Test]
        public void MaxTimeUuid_LessThan_NullableTimeUuidComparison()
        {
            EntityWithNullableTimeUuid.SetupEntity(_tableEntityWithNullableTimeUuid, _expectedNullableTimeUuidObjectList);

            var whereQuery = _tableEntityWithNullableTimeUuid.Where(s => s.NullableTimeUuidType < CqlFunction.MaxTimeUuid(_dateAfter));
            List<EntityWithNullableTimeUuid> objectsReturned1 = whereQuery.ExecuteAsync().Result.ToList();
            Assert.AreEqual(_expectedTimeUuidObjectList.Count, objectsReturned1.Count);

            foreach (var actualObj in objectsReturned1)
                EntityWithNullableTimeUuid.AssertListContains(_expectedNullableTimeUuidObjectList, actualObj);

            whereQuery = _tableEntityWithNullableTimeUuid.Where(s => s.NullableTimeUuidType < CqlFunction.MaxTimeUuid(_dateBefore));
            List<EntityWithNullableTimeUuid> objectsReturned2 = whereQuery.ExecuteAsync().Result.ToList();
            Assert.AreEqual(0, objectsReturned2.Count);
        }

        /// <summary>
        /// Validate that the LinqUtility function MaxTimeUuid, which corresponds to the CQL query maxTimeuuid
        /// functions as expected when using a 'less than or equal to' comparison, comparing NullableTimeUuid values
        /// </summary>
        [Test]
        public void MaxTimeUuid_LessThanOrEqualTo_NullableTimeUuidComparison()
        {
            EntityWithNullableTimeUuid.SetupEntity(_tableEntityWithNullableTimeUuid, _expectedNullableTimeUuidObjectList);

            var whereQuery = _tableEntityWithNullableTimeUuid.Where(s => s.NullableTimeUuidType <= CqlFunction.MaxTimeUuid(_dateAfter));
            List<EntityWithNullableTimeUuid> objectsReturned1 = whereQuery.ExecuteAsync().Result.ToList();
            Assert.AreEqual(_expectedTimeUuidObjectList.Count, objectsReturned1.Count);

            foreach (var actualObj in objectsReturned1)
                EntityWithNullableTimeUuid.AssertListContains(_expectedNullableTimeUuidObjectList, actualObj);

            whereQuery = _tableEntityWithNullableTimeUuid.Where(s => s.NullableTimeUuidType <= CqlFunction.MaxTimeUuid(_dateBefore));
            List<EntityWithNullableTimeUuid> objectsReturned2 = whereQuery.ExecuteAsync().Result.ToList();
            Assert.AreEqual(0, objectsReturned2.Count);
        }


    }


    

}
