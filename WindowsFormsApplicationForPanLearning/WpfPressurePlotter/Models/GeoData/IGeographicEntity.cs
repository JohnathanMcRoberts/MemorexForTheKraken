using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPressurePlotter.Models.GeoData
{
    public interface IGeographicEntity
    {
        string Name { get; }

        double CentroidLongitude { get; }

        double CentroidLatitude { get; }

        double MinLongitude { get; }

        double MinLatitude { get; }

        double MaxLongitude { get; }

        double MaxLatitude { get; }

        double TotalArea { get; }

        List<PolygonBoundary> LandBlocks { get; }
    }
}
