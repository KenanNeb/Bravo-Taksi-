using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.NetworkAnalysis;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Bravo_Taksi.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class RouteMap : Window
    {

        public RouteMap()
        {

            InitializeComponent();
            MainMapView.LocationDisplay.IsEnabled = true;
            MainMapView.LocationDisplay.AutoPanMode = Esri.ArcGISRuntime.UI.LocationDisplayAutoPanMode.Recenter;


        }




        private List<Stop> _routeStops;
        private GraphicsOverlay _routeGraphicsOverlay;

        // URI for the San Diego route service
        private Uri _sanDiegoRouteServiceUri = new Uri("https://sampleserver6.arcgisonline.com/arcgis/rest/services/NetworkAnalysis/SanDiego/NAServer/Route");

        // URIs for picture marker images
        private Uri _checkedFlagIconUri = new Uri("https://static.arcgis.com/images/Symbols/Transportation/CheckeredFlag.png");

        private Uri _carIconUri = new Uri("https://static.arcgis.com/images/Symbols/Transportation/CarRedFront.png");



        // Create the map, graphics overlay, and the 'from' and 'to' locations for the route


        public static double x1;
        public static double y1;
        public static double x2;
        public static double y2;

        private void Initialize()
        {


            //MessageBox.Show(x2.ToString(), y2.ToString());
            //MessageBox.Show(x1.ToString(),y1.ToString());


            MapPoint fromPoint = new MapPoint(x1, y2, MainMapView.SpatialReference);
            MapPoint toPoint = new MapPoint(x2, y2, MainMapView.SpatialReference);
            MessageBox.Show(toPoint.Y.ToString(), y2.ToString());
            // Create Stop objects with the points and add them to a list of stops
            Stop stop1 = new Stop(fromPoint);
            Stop stop2 = new Stop(toPoint);
            _routeStops = new List<Stop> { stop1, stop2 };

            // Picture marker symbols: from = car, to = checkered flag
            PictureMarkerSymbol carSymbol = new PictureMarkerSymbol(_carIconUri);
            PictureMarkerSymbol flagSymbol = new PictureMarkerSymbol(_checkedFlagIconUri);


            carSymbol.OffsetX = -carSymbol.Width / 2;
            carSymbol.OffsetY = -carSymbol.Height / 2;
            flagSymbol.OffsetX = -flagSymbol.Width / 2;
            flagSymbol.OffsetY = -flagSymbol.Height / 2;

            // Create graphics for the stops
            Graphic fromGraphic = new Graphic(fromPoint, carSymbol) { ZIndex = 1 };
            Graphic toGraphic = new Graphic(toPoint, flagSymbol) { ZIndex = 1 };

            // Create the graphics overlay and add the stop graphics
            _routeGraphicsOverlay = new GraphicsOverlay();
            _routeGraphicsOverlay.Graphics.Add(fromGraphic);
            _routeGraphicsOverlay.Graphics.Add(toGraphic);

            // Get an Envelope that covers the area of the stops (and a little more)
            Envelope routeStopsExtent = new Envelope(fromPoint, toPoint);
            EnvelopeBuilder envBuilder = new EnvelopeBuilder(routeStopsExtent);
            envBuilder.Expand(1.5);

            // Create a new viewpoint apply it to the map view when the spatial reference changes
            Viewpoint sanDiegoViewpoint = new Viewpoint(envBuilder.ToGeometry());
            //MainMapView.SpatialReferenceChanged += (s, e) => MainMapView.SetViewpoint(sanDiegoViewpoint);


            //MainMapView.Map = new Map(BasemapStyle.ArcGISStreets);
            MainMapView.GraphicsOverlays.Add(_routeGraphicsOverlay);
        }

        private async void SolveRouteClick(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                // Create a new route task using the San Diego route service URI
                RouteTask solveRouteTask = await RouteTask.CreateAsync(_sanDiegoRouteServiceUri);

                // Get the default parameters from the route task (defined with the service)
                RouteParameters routeParams = await solveRouteTask.CreateDefaultParametersAsync();

                // Make some changes to the default parameters
                routeParams.ReturnStops = true;
                routeParams.ReturnDirections = true;

                // Set the list of route stops that were defined at startup
                routeParams.SetStops(_routeStops);

                // Solve for the best route between the stops and store the result
                RouteResult solveRouteResult = await solveRouteTask.SolveRouteAsync(routeParams);

                // Get the first (should be only) route from the result
                Route firstRoute = solveRouteResult.Routes.First();

                // Get the route geometry (polyline)
                Polyline routePolyline = firstRoute.RouteGeometry;

                // Create a thick purple line symbol for the route
                SimpleLineSymbol routeSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, Color.Purple, 8.0);

                // Create a new graphic for the route geometry and add it to the graphics overlay
                Graphic routeGraphic = new Graphic(routePolyline, routeSymbol) { ZIndex = 0 };
                _routeGraphicsOverlay.Graphics.Add(routeGraphic);

                // Get a list of directions for the route and display it in the list box
                IReadOnlyList<DirectionManeuver> directionsList = firstRoute.DirectionManeuvers;
                DirectionsListBox.ItemsSource = directionsList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private void ResetClick(object sender, System.Windows.RoutedEventArgs e)
        {
            // Clear the list of directions
            DirectionsListBox.ItemsSource = null;

            // Remove the route graphic from the graphics overlay (only line graphic in the collection)
            int graphicsCount = _routeGraphicsOverlay.Graphics.Count;
            for (int i = graphicsCount; i > 0; i--)
            {
                // Get this graphic and see if it has line geometry
                Graphic g = _routeGraphicsOverlay.Graphics[i - 1];
                if (g.Geometry.GeometryType == GeometryType.Polyline)
                {
                    // Remove the graphic from the overlay
                    _routeGraphicsOverlay.Graphics.Remove(g);
                }
            }

        }













        public async void MainMapView_GeoViewTapped(object sender, GeoViewInputEventArgs e)
        {

            try
            {
                MapViewModel viewmodel = Resources["MapViewModel"] as MapViewModel;
                await viewmodel.HandleTap(e.Location);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", ex.Message);
            }
        }
        private bool isk = true;
        DispatcherTimer timer = new DispatcherTimer();
        MapViewModel ms = new MapViewModel();
        public void a()
        {
            ms.RAddress(AddressTextBox.Text, MainMapView.SpatialReference);
            InitializeComponent();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {

            if (isk)
            {
                if (x1 == 0 || y1 == 0)
                {
                    //MessageBox.Show(x1.ToString(), y1.ToString());
                    return;
                }
                else
                {

                    //MessageBox.Show(x1.ToString());
                    //MessageBox.Show(y1.ToString());

                    isk = false;
                    ms.RAddress(AddressTextBox2.Text, MainMapView.SpatialReference, false);

                }
            }
            else
            {
                if (x2 == 0 || y2 == 0)
                {
                    return;
                }
                else
                {

                    //MessageBox.Show(x2.ToString());
                    //MessageBox.Show(y2.ToString());

                    Initialize();
                    timer.Stop();
                    return;

                }
            }


        }

        private async void SearchAddressButton_Click(object sender, RoutedEventArgs e)
        {
            a();





            //MapViewModel viewModel = FindResource("MapViewModel") as MapViewModel;
            //Esri.ArcGISRuntime.Geometry.MapPoint addressPoint = await viewModel.SearchAddress(AddressTextBox.Text, MainMapView.SpatialReference);
            //Esri.ArcGISRuntime.Geometry.MapPoint addressPoint2 = await viewModel.SearchAddress(AddressTextBox.Text, MainMapView.SpatialReference);
            //if (addressPoint != null)
            //{
            //    await MainMapView.SetViewpointCenterAsync(addressPoint);
            //}
        }
    }
}
