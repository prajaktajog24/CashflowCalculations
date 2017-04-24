using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;

[TestClass()]
public class ProgramTests
{
    /* Note: We could have used something like Guava which we use in Java for preconditions testing which can test for null conditions etc. 
    I did read up a bit and i think CodeContracts in C# offers similar capabilities. Although, it is not done as part of this assignment as it would
    require additional installation */

    Program p = new Program();

    //Initializing data
    private const double DISCOUNT_RATE = 0.04; // 4%

    public List<KeyValuePair<DateTime, double>> cashFlowSetOne = new List<KeyValuePair<DateTime, double>>
        {
            new KeyValuePair<DateTime, double>(DateTime.ParseExact("06/30/2016", "MM/dd/yyyy", CultureInfo.InvariantCulture), 1500),
            new KeyValuePair<DateTime, double>(DateTime.ParseExact("12/30/2016", "MM/dd/yyyy", CultureInfo.InvariantCulture), 2500),
            new KeyValuePair<DateTime, double>(DateTime.ParseExact("06/30/2018", "MM/dd/yyyy", CultureInfo.InvariantCulture), 1300),
            new KeyValuePair<DateTime, double>(DateTime.ParseExact("12/30/2017", "MM/dd/yyyy", CultureInfo.InvariantCulture), 1600),
            new KeyValuePair<DateTime, double>(DateTime.ParseExact("06/30/2017", "MM/dd/yyyy", CultureInfo.InvariantCulture), 1900)
        };

    public List<KeyValuePair<DateTime, double>> cashFlowSetTwo = new List<KeyValuePair<DateTime, double>>
        {
            new KeyValuePair<DateTime, double>(DateTime.ParseExact("09/30/2016", "MM/dd/yyyy", CultureInfo.InvariantCulture), 1800),
            new KeyValuePair<DateTime, double>(DateTime.ParseExact("03/30/2018", "MM/dd/yyyy", CultureInfo.InvariantCulture), 3400),
            new KeyValuePair<DateTime, double>(DateTime.ParseExact("09/30/2017", "MM/dd/yyyy", CultureInfo.InvariantCulture), 1900),
            new KeyValuePair<DateTime, double>(DateTime.ParseExact("03/30/2017", "MM/dd/yyyy", CultureInfo.InvariantCulture), 1850),
            new KeyValuePair<DateTime, double>(DateTime.ParseExact("09/30/2018", "MM/dd/yyyy", CultureInfo.InvariantCulture), 1100)
        };

    public List<KeyValuePair<DateTime, double>> cashFlowSetTest = new List<KeyValuePair<DateTime, double>>
        {
            new KeyValuePair<DateTime, double>(DateTime.ParseExact("06/30/2016", "MM/dd/yyyy", CultureInfo.InvariantCulture), 1500),
        };

    [TestMethod()]
    [ExpectedException(typeof(System.NullReferenceException))]
    public void ConcatSortNoLinqTestNull()
    {
        p.ConcatSortNoLinq(null, cashFlowSetTwo);
    }

    [TestMethod()]
    [ExpectedException(typeof(System.ArgumentNullException))]
    public void ConcatSortUsingLinqTest()
    {
        p.ConcatSortUsingLinq(null, cashFlowSetTwo);
    }

    [TestMethod()]
    /* Testing that if the date is a forward date then the value returned will not be 
    correct due to the formula for PV being used e^-rt. Hence the assumption is that 
    if the date is ahead of the cashflow date then a 0 value will be returned. */
    public void CalculateDiscountedCashFlowsTestForwardDate()
    {
        double expectedValue = 0;
        DateTime dateForCashFlowCalc = DateTime.ParseExact("06/30/2020", "MM/dd/yyyy", CultureInfo.InvariantCulture);
        Assert.AreEqual(expectedValue, p.CalculateDiscountedCashFlows(cashFlowSetTest, dateForCashFlowCalc));
    }

    [TestMethod()]
    /* Testing for correct calculation of the value based on one cashflow 
   Value = 1500 * e^(0.04 * 46/365) = 1500 * 0.994971589
   So value is correctly 1492.46 */
    public void CalculateDiscountedCashFlowsTestBackDate()
    {
        double expectedValue = 1492.46;
        DateTime dateForCashFlowCalc = DateTime.ParseExact("05/15/2016", "MM/dd/yyyy", CultureInfo.InvariantCulture);
        Assert.AreEqual(expectedValue, p.CalculateDiscountedCashFlows(cashFlowSetTest, dateForCashFlowCalc));
    }

    [TestMethod()]
    /* If the cashflow is to be calculated back to same date then the value will be the same as the cashflow hence 1500 */
    public void CalculateDiscountedCashFlowsTestEqualDate()
    {
        double expectedValue = 1500;
        DateTime dateForCashFlowCalc = DateTime.ParseExact("06/30/2016", "MM/dd/yyyy", CultureInfo.InvariantCulture);
        Assert.AreEqual(expectedValue, p.CalculateDiscountedCashFlows(cashFlowSetTest, dateForCashFlowCalc));
    }

    [TestMethod()]
    /* The calculator used to test was http://www.investopedia.com/calculator/adaycount.aspx 
    Previous Coupon Payment: 05/15/2016
    Settlement Date: 06/30/2016 
    Actual Day Count: 46
    */
    public void CalculateDayCountConventionDaysTest()
    {
        DateTime cashFlowTestForDayCountConventionTest = DateTime.ParseExact("06/30/2016", "MM/dd/yyyy", CultureInfo.InvariantCulture);
        double expectedValue1 = 0;
        double expectedValue2 = 46;
        DateTime dateForCashFlowCalc1 = DateTime.ParseExact("06/30/2016", "MM/dd/yyyy", CultureInfo.InvariantCulture);
        DateTime dateForCashFlowCalc2 = DateTime.ParseExact("05/15/2016", "MM/dd/yyyy", CultureInfo.InvariantCulture);
        Assert.AreEqual(expectedValue1, p.CalculateDayCountConventionDays(dateForCashFlowCalc1, cashFlowTestForDayCountConventionTest));
        Assert.AreEqual(expectedValue2, p.CalculateDayCountConventionDays(dateForCashFlowCalc2, cashFlowTestForDayCountConventionTest));
    }
}

