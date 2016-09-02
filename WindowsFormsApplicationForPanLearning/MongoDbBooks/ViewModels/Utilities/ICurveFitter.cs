using System;

namespace MongoDbBooks.ViewModels.Utilities
{
    public interface ICurveFitter
    {
        double EvaluateYValueAtPoint(double xVal);
    }
}
