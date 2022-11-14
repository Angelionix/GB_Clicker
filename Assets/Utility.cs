using System;
using UnityEngine;

public class Utility : MonoBehaviour
{
    public static double CalculationOfAFKTime(DateTime storedDT, DateTime currentDT)
    {
        return  (currentDT - storedDT).TotalSeconds;
    }
}
