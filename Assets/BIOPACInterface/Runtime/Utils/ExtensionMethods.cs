using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ExtensionMethods
{
    public static double StandardDeviation(this IEnumerable<double> values)
    {
        double avg = values.Average();
        return Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));

    }

    public static double Median(this IEnumerable<double> values)
    {
        int numberCount = values.Count();
        int halfIndex = values.Count()/2;
        var sortedNumbers = values.OrderBy(n=>n);
        double median;
        if ((numberCount % 2) == 0)
        {
            int previous = halfIndex - 1;
            median = (sortedNumbers.ElementAt(halfIndex) + 
                        sortedNumbers.ElementAt(previous))/ 2;
        } else {
            median = sortedNumbers.ElementAt(halfIndex);
        }
        
        return median;
    }
}
