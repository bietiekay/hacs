using xs1_data_logging.Handle_XS1_TSV;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using hacs.xs1;

namespace xs1_data_logging_tests
{
    
    
    /// <summary>
    ///This is a test class for HandleXS1_TSVTest and is intended
    ///to contain all HandleXS1_TSVTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HandleXS1_TSVTest
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
        ///A test for HandleValue
        ///</summary>
        [TestMethod()]
        public void HandleValueTest()
        {
            string DataLine = "1292690251	2010	12	18	Sat	17	37	31	+0100	S	1	Keller	temperature	14.8";
            XS1_DataObject expected = new XS1_DataObject("Keller", ObjectTypes.Sensor, "temperature", new DateTime(2010,12,18,17,37,31,0, System.Globalization.Calendar.CurrentEra), 1, 14.8);
            XS1_DataObject actual;

            actual = HandleXS1_TSV.HandleValue(DataLine);

            Assert.IsNotNull(actual);

            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Timecode.Ticks, actual.Timecode.Ticks);
            Assert.AreEqual(expected.Type, actual.Type);
            Assert.AreEqual(expected.TypeName, actual.TypeName);
            Assert.AreEqual(expected.Value, actual.Value);
            Assert.AreEqual(expected.XS1ObjectID, actual.XS1ObjectID);
            
        }

        /// <summary>
        ///A test for HandleValue error handling
        ///</summary>
        [TestMethod()]
        public void HandleValueErrorTest()
        {
            string DataLine = "12926902512010	12	18	Sat	17	37	31	+0100	S	1	Keller	temperature	14.8";
            XS1_DataObject actual;
            actual = HandleXS1_TSV.HandleValue(DataLine);
            Assert.AreEqual(null, actual);

            DataLine = "   sdfopsifpsifposifp";
            actual = HandleXS1_TSV.HandleValue(DataLine);
            Assert.AreEqual(null, actual);

            DataLine = "12926902512010	12	18	Sat	17	37	31	+0100	S	1	Keller	temperature	aaa";
            actual = HandleXS1_TSV.HandleValue(DataLine);
            Assert.AreEqual(null, actual);

            DataLine = "12926902512010	12	18	Sat	17	37	31	+0100	T	1	Keller	temperature	aaa";
            actual = HandleXS1_TSV.HandleValue(DataLine);
            Assert.AreEqual(null, actual);
        }

    }
}
