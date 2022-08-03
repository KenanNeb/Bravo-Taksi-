using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.Geocoding;
using Esri.ArcGISRuntime.UI;
using System.Drawing;
using Esri.ArcGISRuntime.Tasks.NetworkAnalysis;
using System.Threading;
using System.Windows;
using Bravo_Taksi.View;

namespace Bravo_Taksi
{
    internal class MapViewModel : INotifyPropertyChanged
    {
        enum RouteBuilderStatus
        {
            NotStarted,
            SelectedStart,
            SelectedStartAndEnd
        }
        private Graphic _startGraphic;
        private Graphic _endGraphic;
        private Graphic _routeGraphic;
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private Map _map;
        public Map Map
        {
            get { return _map; }
            set
            {
                _map = value;
                OnPropertyChanged();
            }
        }
        private GraphicsOverlayCollection _graphicsOverlayCollection;
        public GraphicsOverlayCollection GraphicsOverlays
        {
            get { return _graphicsOverlayCollection; }
            set
            {
                _graphicsOverlayCollection = value;
                OnPropertyChanged();
            }
        }




        private List<string> _directions;
        public List<string> Directions
        {
            get { return _directions; }
            set
            {
                _directions = value;
                OnPropertyChanged();
            }
        }

        private RouteBuilderStatus _currentState;

        private void SetupMap()
        {


            Map = new Map(BasemapStyle.ArcGISTopographic);

            GraphicsOverlay overlay = new GraphicsOverlay();



            GraphicsOverlay routeAndStopsOverlay = new GraphicsOverlay();
            this.GraphicsOverlays = new GraphicsOverlayCollection
            {
                routeAndStopsOverlay
            };
            var startOutlineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.Green, 2);
            _startGraphic = new Graphic(null, new SimpleMarkerSymbol
            {
                Style = SimpleMarkerSymbolStyle.Diamond,
                Color = System.Drawing.Color.Orange,
                Size = 8,
                Outline = startOutlineSymbol
            }
            );

            var endOutlineSymbol = new SimpleLineSymbol(style: SimpleLineSymbolStyle.Solid, color: System.Drawing.Color.Red, width: 2);
            _endGraphic = new Graphic(null, new SimpleMarkerSymbol
            {
                Style = SimpleMarkerSymbolStyle.Square,
                Color = System.Drawing.Color.Green,
                Size = 8,
                Outline = endOutlineSymbol
            }
            );

            _routeGraphic = new Graphic(null, new SimpleLineSymbol(
                style: SimpleLineSymbolStyle.Solid,
                color: System.Drawing.Color.Green,
                width: 4
            ));
            routeAndStopsOverlay.Graphics.AddRange(new[] { _startGraphic, _endGraphic, _routeGraphic });

        }


        private void ResetState()
        {
            _startGraphic.Geometry = null;
            _endGraphic.Geometry = null;
            _routeGraphic.Geometry = null;
            Directions = null;
            _currentState = RouteBuilderStatus.NotStarted;
        }

        public async Task FindRoute()
        {

            var stops = new[] { _startGraphic, _endGraphic }.Select(graphic => new Stop(graphic.Geometry as MapPoint));
            var routeTask = await RouteTask.CreateAsync(new Uri("https://route-api.arcgis.com/arcgis/rest/services/World/Route/NAServer/Route_World"));
            RouteParameters parameters = await routeTask.CreateDefaultParametersAsync();
            parameters.SetStops(stops);
            parameters.ReturnDirections = true;
            parameters.ReturnRoutes = true;
            var result = await routeTask.SolveRouteAsync(parameters);

            if (result?.Routes?.FirstOrDefault() is Route routeResult)
            {
                _routeGraphic.Geometry = routeResult.RouteGeometry;
                Directions = routeResult.DirectionManeuvers.Select(maneuver => maneuver.DirectionText).ToList();
                _currentState = RouteBuilderStatus.NotStarted;
            }
            else
            {
                ResetState();
                throw new Exception("Route not found");
            }

        }









        public async Task HandleTap(MapPoint tappedPoint)
        {
            MessageBox.Show(tappedPoint.ToString());
            switch (_currentState)
            {
                case RouteBuilderStatus.NotStarted:
                    ResetState();
                    _startGraphic.Geometry = tappedPoint;
                    _currentState = RouteBuilderStatus.SelectedStart;
                    break;
                case RouteBuilderStatus.SelectedStart:
                    _endGraphic.Geometry = tappedPoint;
                    _currentState = RouteBuilderStatus.SelectedStartAndEnd;
                    await FindRoute();
                    break;
                case RouteBuilderStatus.SelectedStartAndEnd:
                    break;
            }

        }



        public async Task<MapPoint> RAddress(string address, SpatialReference spatialReference, bool isok = true)
        {
            MapPoint addressLocation = null;
            GraphicsOverlay graphicsOverlay = this.GraphicsOverlays.FirstOrDefault();
            graphicsOverlay.Graphics.Clear();
            LocatorTask locatorTask = new LocatorTask(new Uri("https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer"));
            GeocodeParameters geocodeParameters = new GeocodeParameters();
            geocodeParameters.ResultAttributeNames.Add("*");
            geocodeParameters.MaxResults = 1;
            geocodeParameters.OutputSpatialReference = spatialReference;
            IReadOnlyList<GeocodeResult> results = await locatorTask.GeocodeAsync(address, geocodeParameters);
            GeocodeResult geocodeResult = results.FirstOrDefault();
            addressLocation = geocodeResult.DisplayLocation;

            if (isok)
            {
                RouteMap.x1 = addressLocation.X;
                RouteMap.y1 = addressLocation.Y;

            }
            else
            {
                RouteMap.x2 = addressLocation.X;
                RouteMap.y2 = addressLocation.Y;
            }

            return addressLocation;
        }
        public async Task<MapPoint> SearchAddress(string address, SpatialReference spatialReference)
        {
            MapPoint addressLocation = null;
            try
            {
                GraphicsOverlay graphicsOverlay = this.GraphicsOverlays.FirstOrDefault();
                graphicsOverlay.Graphics.Clear();
                LocatorTask locatorTask = new LocatorTask(new Uri("https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer"));
                GeocodeParameters geocodeParameters = new GeocodeParameters();
                geocodeParameters.ResultAttributeNames.Add("*");
                geocodeParameters.MaxResults = 1;
                geocodeParameters.OutputSpatialReference = spatialReference;
                IReadOnlyList<GeocodeResult> results = await locatorTask.GeocodeAsync(address, geocodeParameters);
                GeocodeResult geocodeResult = results.FirstOrDefault();
                if (geocodeResult == null) { throw new Exception("No matches found."); }
                SimpleMarkerSymbol markerSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, Color.LightGreen, 12);

                Graphic markerGraphic = new Graphic(geocodeResult.DisplayLocation, geocodeResult.Attributes, markerSymbol);
                graphicsOverlay.Graphics.Add(markerGraphic);
                addressLocation = geocodeResult.DisplayLocation;

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Couldn't find address: " + ex.Message);
            }

            // Return the location of the geocode result.
            return addressLocation;
        }

        private RouteMap _mainWindow;
        public MapViewModel()
        {

            SetupMap();
        }
    }
}
