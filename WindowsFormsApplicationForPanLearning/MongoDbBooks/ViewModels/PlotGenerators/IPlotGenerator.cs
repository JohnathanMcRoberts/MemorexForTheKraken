using OxyPlot;

using MongoDbBooks.Models;

namespace MongoDbBooks.ViewModels.PlotGenerators
{
    public interface IPlotGenerator
    {
        PlotModel SetupPlot(MainBooksModel mainModel);
    }
}
