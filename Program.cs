using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public class Program
{
    // Discount Rate

    private const double DISCOUNT_RATE = 0.04; // 4%

    // Given the following two sets of cash flows (**Note: Dates are in American format):

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
    /* Info above this point is given information */
    /*Assumptions are written above every function and are specific to that function */

    public void CallFunctions()
    {
        //Console.WriteLine is only used to show the path of the log file
        Console.WriteLine("The path to the log file is :");
        Console.WriteLine(GetTempPath());

        /* Requirement 1. Write a method that returns and IEnumerable<KeyValuePair<DateTime, double>> of the combination of both sets ordered by
         DateTime that does not use LINQ and uses the least amount of memory possible. */

        /*Assumption: 

        The below method ConcatSortNoLinq returns IEnumerable<KeyValuePair<DateTime, double>> and the reason for using List is:
        -----Why not LinkedList?
        List in C# is not implemented internally as a linked list but as a generic strongly typed Array.
        Linked list is implemented as 

        // Pseudo-code for linked-list node:
     struct Node {
        // Pointer to next list item.
        Node* next;
        // Pointer to prev list item.
        Node* prev;
        // Pointer to next value.
        KeyValue<DateTime, double>* value;
         };

        There are two allocations: one for the KeyValue pair and one for the Node itself. 
        So in our current implementation we allocate a new set of Nodes just to sort the original set of values. We can do better.
        An ArrayList in C# should allow us to achieve the behaviour with just a single allocation. Hence the reason for using List. 

        -----Why not Dictionary?
        Dictionary was another choice and provides faster speed but utilizes more memory and since we needed to use the least amount of memory possible i decided to go with List.

        Aside: C++ allows you to transfer Nodes between Lists (http://en.cppreference.com/w/cpp/container/list/splice), but this is a destructive operation (it modifies the original), 
        and also allows you to embed the value in the Node in one contiguous block, which saves memory and improves spatial locality but I doubt C# supports an equivalent anyway. 

        Note: I also tested using various data structures and used the System.Diagnostics Stopwatch to check the time taken to come up with my choice of data structure.
        Although Dictionary was the fastest, memory consumption was higher.
        */
        List<KeyValuePair<DateTime, double>> sortedListNoLinq = (List<KeyValuePair<DateTime, double>>)ConcatSortNoLinq(cashFlowSetOne, cashFlowSetTwo);
        //This is a generic display function that takes any IEnumerable keyvalue pair and loops through it to display data
        LogMessageToFile("Displaying sorted data using NO Linq");
        Display(sortedListNoLinq);

        /* Requirement 2. Now write another method that does the same thing but uses LINQ */

        /* Assumption: same as above for Requirement 1. UNION was used and OrderBy was used , both being Linq functionalities */
        List<KeyValuePair<DateTime, double>> sortedListLinq = (List<KeyValuePair<DateTime, double>>)ConcatSortUsingLinq(cashFlowSetOne, cashFlowSetTwo);
        LogMessageToFile("Displaying sorted data USING Linq");
        Display(sortedListLinq);

        /* Requirement 3. Write a method that discounts all cash flows back to 5/15/2016 using the DISCOUNT_RATE above */

        /* Assumption: Here date is assumed as 05/15/2016 as per requirement. This can be made into a user input but im assuming there will be some front end
        and the user would be able to select a date from the calendar . Also, it would not matter where this is executed since the CultureInfo.InvariantCulture would 
        change the date format based on where it is run from. In the current context, it is en-GB */
        DateTime dateForCashFlowCalc = DateTime.ParseExact("05/15/2016", "MM/dd/yyyy", CultureInfo.InvariantCulture);
        double finalValue = CalculateDiscountedCashFlows(sortedListLinq, dateForCashFlowCalc);
        //Console.WriteLine is only used to show the final value

        Console.WriteLine(Math.Round(finalValue,2));
        LogMessageToFile((Math.Round(finalValue,2)).ToString());

        /* Requirement 4. Write unit tests that demonstrate your methods are correct */
        /* Please refer to ProgramTests.cs */
    }

    /*Requirement 1 function */
    public IEnumerable<KeyValuePair<DateTime, double>> ConcatSortNoLinq(List<KeyValuePair<DateTime, double>> cashFlowSetOne, List<KeyValuePair<DateTime, double>> cashFlowSetTwo)
    {
        /* Assumption: the list arguments provided must not be modified 
        Preallocate output list to required size */
        List<KeyValuePair<DateTime, double>> list = new List<KeyValuePair<DateTime, double>>(cashFlowSetOne.Count + cashFlowSetTwo.Count);
        try {
            list.AddRange(cashFlowSetOne);
            list.AddRange(cashFlowSetTwo);
            list.Sort(Compare);
        }
        catch (ArgumentNullException e)
        {
            LogMessageToFile(e.ToString());
        }
        catch(NullReferenceException e1)
        {
            LogMessageToFile(e1.ToString());
        }
        return list;
    }

    /*Requirement 2 function */
    public IEnumerable<KeyValuePair<DateTime, double>> ConcatSortUsingLinq(List<KeyValuePair<DateTime, double>> cashFlowSetOne, List<KeyValuePair<DateTime, double>> cashFlowSetTwo)
    {
        /* Assumption: the list arguments provided must not be modified 
        Preallocate output list to required size */
        List<KeyValuePair<DateTime, double>> listLinq = new List<KeyValuePair<DateTime, double>>(cashFlowSetOne.Count() + cashFlowSetTwo.Count());
        try
        {
            listLinq = cashFlowSetOne.Union(cashFlowSetTwo).OrderBy(e => e.Key).ToList<KeyValuePair<DateTime, double>>();
        }
        catch(ArgumentNullException e)
        {
            LogMessageToFile(e.ToString());
        }
        return listLinq;
    }

    /*Requirement 3 function */
    public double CalculateDiscountedCashFlows(IEnumerable<KeyValuePair<DateTime, double>> dataForCashFlowCalc, DateTime dateForCashFlowCalc)
    {
        /* Assumption:
        1. Continuous compounding is assumed. This can be easily changed a different type using enum as 
            public enum Compounding
            {
                Simple,
                Continuous,
                Compounded
            }
            Frequency can also be adapted as per below 
            public enum Freq
            {
                Annual = 1,
                SemiAnnual = 2,
                Quarterly = 4,
                Monthly = 12,
                Weekly = 52,
                Daily = 365
            };
        2. Each cashflow is discounted back to the date provided which in this case is 05/15/2016. If the cashflow for 03/30/2016 was provided
        we would be able to calculate all cashflows back to that date and then calculate the Accrued Interest from 03/30/2016 to 05/15/2016
        but since it was not provided, the assumption is that all the cashflows are to be calculated back to the date provided. 

        3. Formula used is CashFlow{number}CalculatedToDateProvided = CashFlow{number} * e^ (-discount rate * time)
            where time is calculated as (actual number of days between each cashflow and 05/15/2016) / 365 
            All CashFlow{number}CalculatedToDateProvided are then added to get the final value 

        4. DayCountConvention used is Actual/365. This can again easily be amended to accomodate and enum of all the types which can then be sent as argument to the function
        public enum Dc
        {
            _30_360 =1,
            _Act_360 =2,
            _Act_Act =3,
            _Act_365 =4,
            _30_Act =5,
            _30_365 =6
        }
        */
        List<double> storeCashFlows = new List<double>();

        /*check if the date that we want to discount back to is in the past. 
        if the date was in the future then the PV calculation would not work or return wrong results .
        In that case the assumption is that 0 will be returned since adding the cashflows would give wrong resuls. 
        If the date is the same then the cashflow value will be the same as the discounted value */

        foreach (var entry in dataForCashFlowCalc)
        {
            if (entry.Key < dateForCashFlowCalc)
            {
                LogMessageToFile("The cashflow date you want to discount back to is in the future");
                return 0;
            }
        }
        try {
            foreach (var entry in dataForCashFlowCalc)
            {
                storeCashFlows.Add(Math.Round(entry.Value * (Math.Exp(-DISCOUNT_RATE * (
                    (CalculateDayCountConventionDays(dateForCashFlowCalc, entry.Key))/365))), 2));                
            }
        }
        catch(Exception e)
        {
            LogMessageToFile(e.ToString());
        }
        LogMessageToFile("Individual cashflows discounted back to");
        LogMessageToFile(dateForCashFlowCalc.ToString());
        foreach (var cf in storeCashFlows)
        {
            LogMessageToFile(cf.ToString());
        }
        return storeCashFlows.Sum();
    }

    

    /*Some Utility functions have been written below. Ideally they should be in another class but for simplicity have added them to the same class file */

    /* CalculateNumber of days between 2 dates and use day count convention. An additional parameter can be DayCountConvention Type to choose 
    from different values as provided as enum in assumptions above */
    public double CalculateDayCountConventionDays(DateTime dateDiscountTo, DateTime dateCashFlow)
    {
        return (dateCashFlow - dateDiscountTo).TotalDays;
    }

    /* Compare function for the sort method to sort based on key */
    static int Compare(KeyValuePair<DateTime, double> a, KeyValuePair<DateTime, double> b)
    {
        return a.Key.CompareTo(b.Key);
    }

    /* Log file writing - get path - used in function LogMessageToFile*/
    public string GetTempPath()
    {
        string path = System.Environment.GetEnvironmentVariable("TEMP");
        if (!path.EndsWith("\\")) path += "\\";
        return path;
    }

    public void LogMessageToFile(string msg)
    {
        System.IO.StreamWriter sw = System.IO.File.AppendText(
            GetTempPath() + "LogFile.txt");
        try
        {
            string logLine = System.String.Format(
                "{0:G}: {1}.", System.DateTime.Now, msg);
            sw.WriteLine(logLine);
        }
        finally
        {
            sw.Close();
        }
    }

    /* Generic display function */
    public void Display(IEnumerable<KeyValuePair<DateTime, double>> data)
    {
        foreach (var entry in data)
        {
            LogMessageToFile(entry.ToString());
        }
    }

    static void Main(string[] args)
    {
        Program p = new Program();
        //calling all coding assignent functions in one method
        p.CallFunctions();
    }


}
