using MEAClosedLoop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for CCalcSE_N_Test and is intended
    ///to contain all CCalcSE_N_Test Unit Tests
    ///</summary>
  [TestClass()]
  public class CCalcSE_N_Test
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
    ///A test for CCalcSE_N Constructor
    ///</summary>
    [TestMethod()]
    public void CCalcSE_NConstructor_Test()
    {
      int n = 0; // TODO: Initialize to an appropriate value
      CCalcSE_N target = new CCalcSE_N(n);
      Assert.Inconclusive("TODO: Implement code to verify target");
    }

    /// <summary>
    ///A test for se
    ///</summary>
    [TestMethod()]
    public void se_Test()
    {
      int n = 0; // TODO: Initialize to an appropriate value
      CCalcSE_N target = new CCalcSE_N(n); // TODO: Initialize to an appropriate value
      ushort[] data = null; // TODO: Initialize to an appropriate value
      double[] expected = null; // TODO: Initialize to an appropriate value
      double[] actual;
      actual = target.SE(data);
      Assert.AreEqual(expected, actual);
      Assert.Inconclusive("Verify the correctness of this test method.");
    }
  }
}
