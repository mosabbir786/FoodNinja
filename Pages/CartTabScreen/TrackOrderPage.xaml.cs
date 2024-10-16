using FoodNinja.Custom;
using FoodNinja.Model;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace FoodNinja.Pages.CartTabScreen;

public partial class TrackOrderPage : ContentPage
{
    private List<MapPin> _pins;
    public List<MapPin> Pins
    {
        get { return _pins; }
        set { _pins = value; OnPropertyChanged(); }
    }
    private OrderPlacedModel PlacedOrderItem;
    public TrackOrderPage(OrderPlacedModel placedOrderItem)
	{
		InitializeComponent();
        PlacedOrderItem = placedOrderItem;
        Application.Current.RequestedThemeChanged += OnRequestedThemeChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ApplyMapStyleBasedOnTheme();
        GetDirections();
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Application.Current.RequestedThemeChanged -= OnRequestedThemeChanged;
    }

    #region Clicked Event
    private async void backBtn_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
    private async void BackToRoot_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }

    private async void Message_Tapped(object sender, TappedEventArgs e)
    {
        if (sender is Border border)
        {
            await ApplyTapFadeAnimation(border);
            await Task.Delay(100);
            await ResetBorderFadeAnimation(border);
        }
    }
    #endregion

    private void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        ApplyMapStyleBasedOnTheme();
    }

    private void ApplyMapStyleBasedOnTheme()
    {
        var currentTheme = Application.Current.RequestedTheme;
        if (currentTheme == AppTheme.Dark)
        {
            AddMapStyle("FoodNinja.dark_map_style.json");
        }
        else
        {
            trackOrderMap.MapStyle = null;
        }
    }

    private async void AddMapStyle(string resourcePath)
    {
        try
        {
            var a = Assembly.GetExecutingAssembly();
            // using var stream = a.GetManifestResourceStream("FoodDeliveryMaui.dark_map_style.json");
            using var stream = a.GetManifestResourceStream("FoodNinja.darkmap.json");
            if (stream == null)
            {
                Console.WriteLine("Stream is null. Check the resource path.");
                return;
            }
            string jsonStyle = string.Empty;
            using (var reader = new StreamReader(stream!))
            {
                jsonStyle = reader.ReadToEnd();
            }
            if (string.IsNullOrEmpty(jsonStyle))
            {
                Console.WriteLine("JSON style is empty.");
                return;
            }
            if (trackOrderMap == null)
            {
                Console.WriteLine("trackOrderMap is null.");
                return;
            }
            trackOrderMap.MapStyle = MapStyle.FromJson(jsonStyle);
            if (trackOrderMap.MapStyle == null)
            {
                Console.WriteLine("Map style failed to load.");

            }
            else
            {
                Console.WriteLine("Map style applied successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading map style: {ex.Message}");
        }
    }
    private async void GetDirections()
    {
        string encodedPolyline = await GetRoutePolylineAsync(PlacedOrderItem.RestaurantLat, PlacedOrderItem.RestaurantLong,
                                 PlacedOrderItem.UserLatitude, PlacedOrderItem.UserLongitude);
        if (!string.IsNullOrEmpty(encodedPolyline))
        {
            var decodedPolyline = DecodePolyline(encodedPolyline);
            //  MoveMapToRoute(PlacedOrderItem.UserLatitude, PlacedOrderItem.UserLongitude);
            MoveMapToRoute(PlacedOrderItem.UserLatitude, PlacedOrderItem.UserLongitude, PlacedOrderItem.RestaurantLat, PlacedOrderItem.RestaurantLong);

            AddPinsToMap(PlacedOrderItem.RestaurantLat, PlacedOrderItem.RestaurantLong, PlacedOrderItem.UserLatitude, PlacedOrderItem.UserLongitude);

            var polyline = new Polyline
            {
                StrokeColor = Color.FromArgb("#53E88B"),
                StrokeWidth = 15
            };
            foreach (var location in decodedPolyline)
            {
                polyline.Geopath.Add(location);
            }
            trackOrderMap.MapElements.Add(polyline);

        }
    }
    private async Task<string> GetRoutePolylineAsync(double startLat, double startLng, double endLat, double endLng)
    {
        var apiKey = "AIzaSyDDH92KjDUYZhx-L6YctF7xLLXQAoYpCsc";
        string url = $"https://maps.googleapis.com/maps/api/directions/json?origin={startLat},{startLng}&destination={endLat},{endLng}&key={apiKey}";

        using (HttpClient httpClient = new HttpClient())
        {
            var response = await httpClient.GetStringAsync(url);
            var json = JObject.Parse(response);

            var status = json["status"]?.ToString();
            if (status != "OK")
            {
                Console.WriteLine($"Google API error: {status}");
                var errorMessage = json["error_message"]?.ToString();
                Console.WriteLine($"Error message: {errorMessage}");
                return null;
            }
            else
            {
                var polyline = json["routes"]?[0]?["overview_polyline"]?["points"]?.ToString();
                return polyline;
            }

        }
    }
    private List<Location> DecodePolyline(string encodedPoints)
    {
        if (string.IsNullOrEmpty(encodedPoints))
        {
            return null;
        }
        var polyline = new List<Location>();
        var index = 0;
        var currentLat = 0;
        var currentLng = 0;
        while (index < encodedPoints.Length)
        {
            // Decode latitude
            int sum = 0;
            int shift = 0;
            int b;
            do
            {
                b = encodedPoints[index++] - 63;
                sum |= (b & 0x1f) << shift;
                shift += 5;
            } while (b >= 0x20);
            var deltaLat = ((sum & 1) != 0 ? ~(sum >> 1) : (sum >> 1));
            currentLat += deltaLat;
            // Decode longitude
            sum = 0;
            shift = 0;
            do
            {
                b = encodedPoints[index++] - 63;
                sum |= (b & 0x1f) << shift;
                shift += 5;
            } while (b >= 0x20);
            var deltaLng = ((sum & 1) != 0 ? ~(sum >> 1) : (sum >> 1));
            currentLng += deltaLng;

            var lat = currentLat / 1E5;
            var lng = currentLng / 1E5;
            polyline.Add(new Location(lat, lng));
        }
        return polyline;
    }
    private void MoveMapToRoute(double userLat, double userLng, double restaurantLat, double restaurantLng)
    {
        // Calculate the center point between the user's location and the restaurant's location
        double centerLat = (userLat + restaurantLat) / 2;
        double centerLng = (userLng + restaurantLng) / 2;

        // Calculate the distance between the user's location and the restaurant's location
        double distance = Location.CalculateDistance(userLat, userLng, restaurantLat, restaurantLng, DistanceUnits.Kilometers);

        // Set the zoom level to include both points (adding some buffer to make sure both points are visible)
        double zoomLevel = distance * 0.7; // Adjust this factor as needed

        // Set the map view to the calculated region
        if (trackOrderMap.VisibleRegion != null)
        {
            trackOrderMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(centerLat, centerLng), Distance.FromKilometers(zoomLevel)));
        }
        else
        {
            trackOrderMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(centerLat, centerLng), Distance.FromKilometers(zoomLevel)));
        }
    }

    private void AddPinsToMap(double restaurantLat, double restaurantLong, double userLatitude, double userLongitude)
    {
        BindingContext = this;
        /* var restaurantPin = new Pin
         {
             Label = PlacedOrderItem.RestaurantName,
             Address = PlacedOrderItem.RestaurantAddress,
             Type = PinType.Place,
             Location = new Location(restaurantLat,restaurantLong)
         };
         var userPin = new Pin
         {
             Label = "MyLocation",
             Address = PlacedOrderItem.UserAddress,
             Type = PinType.Place,
             Location = new Location(userLatitude,userLongitude),

         };
         trackOrderMap.Pins.Clear();
         trackOrderMap.Pins.Add(restaurantPin);
         trackOrderMap.Pins.Add(userPin);*/

        try
        {
            Pins = new List<MapPin>()
            {
                new MapPin(MapPinClicked)
                {
                    Id = Guid.NewGuid().ToString(),
                    Label = PlacedOrderItem.RestaurantName,
                    Address = PlacedOrderItem.RestaurantAddress,
                    Position = new Location(restaurantLat, restaurantLong),
                    IconSrc = "bike",
                },
                new MapPin(MapPinClicked)
                {
                    Id = Guid.NewGuid().ToString(),
                    Label = PlacedOrderItem.UserName,
                    Address = PlacedOrderItem.UserAddress,
                    Position = new Location(userLatitude, userLongitude),
                    IconSrc = "mappin"
                },
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            throw;
        }
    }
    private async void MapPinClicked(MapPin pin)
    {

    }
    private async Task ApplyTapFadeAnimation(Border border)
    {
        border.BackgroundColor = Colors.Gray;
        for (double i = 1; i >= 0; i -= 0.1)
        {
            border.Opacity = i;
            await Task.Delay(20);
        }
    }
    private async Task ResetBorderFadeAnimation(Border border)
    {
        border.BackgroundColor = Color.FromArgb("#252525");
        for (double i = 0; i <= 1; i += 0.1)
        {
            border.Opacity = i;
            await Task.Delay(20);
        }
    }
}