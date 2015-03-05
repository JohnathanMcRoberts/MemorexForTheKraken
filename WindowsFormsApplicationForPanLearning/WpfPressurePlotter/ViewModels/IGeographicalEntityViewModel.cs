using System;
using System.Collections.Generic;
using System.Linq;
using WpfPressurePlotter.Models.GeoData;

namespace WpfPressurePlotter.ViewModels
{
    public interface IGeographicalEntityViewModel
    {
        string Name { get; }

        double CentralLongitude { get; }

        double CentralLatitude { get; }

        double MinLongitude { get; }

        double MinLatitude { get; }

        double MaxLongitude { get; }

        double MaxLatitude { get; }

        string CentralLongitudeDms { get; }

        string CentralLatitudeDms { get; }

        string MinLongitudeDms { get; }

        string MinLatitudeDms { get; }

        string MaxLongitudeDms { get; }

        string MaxLatitudeDms { get; }

        int NumberLandBlocks { get; }

        List<PolygonBoundary> LandBlocks { get; }

        
    }
}
