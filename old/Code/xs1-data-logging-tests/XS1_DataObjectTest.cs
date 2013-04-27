using hacs.xs1;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace xs1_data_logging_tests
{
    
    
    /// <summary>
    ///This is a test class for XS1_DataObjectTest and is intended
    ///to contain all XS1_DataObjectTest Unit Tests
    ///</summary>
    [TestClass()]
    public class XS1_DataObjectTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Serialize/Deserialize
        ///</summary>
        [TestMethod()]
        public void SerializeDeserializeTest()
        {
            XS1_DataObject start = new XS1_DataObject("server","Keller", ObjectTypes.Sensor, "temperature", new DateTime(2010, 12, 18, 17, 37, 31, 0, System.Globalization.Calendar.CurrentEra), 1, 14.8);
            byte[] Serialized;
            XS1_DataObject deserialized;

            Serialized = start.Serialize();

            deserialized = new XS1_DataObject();
            deserialized.Deserialize(Serialized);

            Assert.AreEqual(start.ServerName, deserialized.ServerName);
            Assert.AreEqual(start.Name,             deserialized.Name);
            Assert.AreEqual(start.Timecode.Ticks,   deserialized.Timecode.Ticks);
            Assert.AreEqual(start.Type,             deserialized.Type);
            Assert.AreEqual(start.TypeName,         deserialized.TypeName);
            Assert.AreEqual(start.Value,            deserialized.Value);
            Assert.AreEqual(start.XS1ObjectID,      deserialized.XS1ObjectID);

            start = new XS1_DataObject("server","Keller", ObjectTypes.Actor, "temperature", new DateTime(2010, 12, 18, 17, 37, 31, 0, System.Globalization.Calendar.CurrentEra), 1, 14.8);
            
            Serialized = start.Serialize();

            deserialized = new XS1_DataObject();
            deserialized.Deserialize(Serialized);

            Assert.AreEqual(start.ServerName, deserialized.ServerName);
            Assert.AreEqual(start.Name, deserialized.Name);
            Assert.AreEqual(start.Timecode.Ticks, deserialized.Timecode.Ticks);
            Assert.AreEqual(start.Type, deserialized.Type);
            Assert.AreEqual(start.TypeName, deserialized.TypeName);
            Assert.AreEqual(start.Value, deserialized.Value);
            Assert.AreEqual(start.XS1ObjectID, deserialized.XS1ObjectID);

        }

    }
}
