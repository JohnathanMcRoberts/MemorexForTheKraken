using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPressurePlotter.Models.GeoData
{
    public class PolygonBoundary
    {
        public List<PolygonPoint> Points { get; set; }


        public double MinLongitude { get; private set; }
        public double MinLatitude { get; private set; }

        public double MaxLongitude { get; private set; }
        public double MaxLatitude { get; private set; }   

        public PolygonBoundary()
        {
            MinLongitude = MinLatitude = Double.MaxValue;
            MaxLongitude = MaxLatitude = Double.MinValue;
            Points = new List<PolygonPoint>(); 
        }
        public PolygonBoundary(string coordinates) 
        {
            Points = new List<PolygonPoint>();
            MinLongitude = MinLatitude = Double.MaxValue;
            MaxLongitude = MaxLatitude = Double.MinValue;

            string[] latLongPairs = coordinates.Split(' ');
            foreach (var latLongPair in latLongPairs)
            {
                string trimmed = latLongPair.Trim();
                if (trimmed.Length < 3)
                    continue;

                bool isValid;
                PolygonPoint point = PolygonPoint.Create(trimmed, out isValid);

                if (!isValid)
                    continue;

                Points.Add(point);

                MinLongitude = Math.Min(MinLongitude, point.Longitude);
                MaxLongitude = Math.Max(MaxLongitude, point.Longitude);

                MinLatitude = Math.Min(MinLatitude, point.Latitude);
                MaxLatitude = Math.Max(MaxLatitude, point.Latitude);
            }
            SetupSignedArea();
            SetupCentroid();
        }

        public double SignedArea { get; private set; }

        public double TotalArea { get { return Math.Abs(SignedArea); } }

        public double CentroidLatitude { get; private set; }

        public double CentroidLongitude { get; private set; }

        #region Utility Functions

        private void SetupSignedArea()
        {
            double area = 0;
            for (int i = 0; i < Points.Count; ++i)
            {
                int iplusOne = (1 + i) % Points.Count;
                area += 
                    ((Points[i].Longitude * Points[iplusOne].Latitude) -
                    (Points[iplusOne].Longitude * Points[i].Latitude));
            }
            SignedArea = area / 2;
        }

        private void SetupCentroid()
        {
            double cx = 0;
            double cy = 0;
            double multiplier = 0;
            for (int i = 0; i < Points.Count; ++i)
            {
                int iplusOne = (1 + i) % Points.Count;
                multiplier = ((Points[i].Longitude * Points[iplusOne].Latitude) -
                    (Points[iplusOne].Longitude * Points[i].Latitude));
                cx += (Points[i].Longitude + Points[iplusOne].Longitude) * multiplier;
                cy += (Points[i].Latitude + Points[iplusOne].Latitude) * multiplier;
            }
            CentroidLongitude = cx / (6 * SignedArea);
            CentroidLatitude = cy / (6 * SignedArea);
        }

        #endregion
    }
}
