using System;
using System.Collections.Generic;


namespace MongoDbBooks.ViewModels.Utilities
{
    public class LinearCurveFitter : ICurveFitter
    {
        public LinearCurveFitter(List<double> xVals, List<double> yVals)
        {
            // set up the slope parameters
            double rsquared;
            double yintercept;
            double slope;
            LinearRegression(xVals, yVals, out rsquared, out  yintercept, out  slope);

            // store the values
            _yintercept = yintercept;
            _slope = slope;
        }

        private double _yintercept;
        private double _slope;

        public double EvaluateYValueAtPoint(double xVal)
        {
            return _yintercept + (xVal * _slope);
        }


        /// <summary>
        /// Fits a line to a collection of (x,y) points.
        /// </summary>
        /// <param name="xVals">The x-axis values.</param>
        /// <param name="yVals">The y-axis values.</param>
        /// <param name="rsquared">The r^2 value of the line.</param>
        /// <param name="yintercept">The y-intercept value of the line (i.e. y = ax + b, yintercept is b).</param>
        /// <param name="slope">The slope of the line (i.e. y = ax + b, slope is a).</param>
        public static void LinearRegression(List<double> xVals, List<double> yVals,
                                            out double rsquared, out double yintercept,
                                            out double slope)
        {
            double sumOfX = 0;
            double sumOfY = 0;
            double sumOfXSq = 0;
            double sumOfYSq = 0;
            double ssX = 0;
            double ssY = 0;
            double sumCodeviates = 0;
            double sCo = 0;
            double count = xVals.Count;
            rsquared = yintercept = slope = 0.0;
            if (xVals.Count != yVals.Count || xVals.Count < 1) return;

            for (int ctr = 0; ctr < count; ctr++)
            {
                double x = xVals[ctr];
                double y = yVals[ctr];
                sumCodeviates += x * y;
                sumOfX += x;
                sumOfY += y;
                sumOfXSq += x * x;
                sumOfYSq += y * y;
            }
            ssX = sumOfXSq - ((sumOfX * sumOfX) / count);
            ssY = sumOfYSq - ((sumOfY * sumOfY) / count);
            double RNumerator = (count * sumCodeviates) - (sumOfX * sumOfY);
            double RDenom = (count * sumOfXSq - (sumOfX * sumOfX))
             * (count * sumOfYSq - (sumOfY * sumOfY));
            sCo = sumCodeviates - ((sumOfX * sumOfY) / count);

            double meanX = sumOfX / count;
            double meanY = sumOfY / count;
            double dblR = RNumerator / Math.Sqrt(RDenom);
            rsquared = dblR * dblR;
            yintercept = meanY - ((sCo / ssX) * meanX);
            slope = sCo / ssX;
        }

    }
}
