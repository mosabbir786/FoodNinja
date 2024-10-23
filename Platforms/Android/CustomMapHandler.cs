using Android.Gms.Maps.Model;
using Android.Gms.Maps;
using Android.Graphics;
using Microsoft.Maui.Maps.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodNinja.Custom;
using FoodNinja.Model;

namespace FoodNinja.Platforms.Android
{
    public class CustomMapHandler : MapHandler
    {
        private const int _iconSize = 60;

        private readonly Dictionary<string, BitmapDescriptor> _iconMap = [];

        public static new IPropertyMapper<MapEx, CustomMapHandler> Mapper = new PropertyMapper<MapEx, CustomMapHandler>(MapHandler.Mapper)
        {
            [nameof(MapEx.CustomPins)] = MapPins
        };
        public Dictionary<string, (Marker Marker, MapPin Pin)> MarkerMap { get; } = new();
        public CustomMapHandler() : base(Mapper)
        {
        }
        protected override void ConnectHandler(MapView platformView)
        {
            base.ConnectHandler(platformView);
            var mapReady = new MapCallbackHandler(this);
            PlatformView.GetMapAsync(mapReady);

        }
        private static new void MapPins(IMapHandler handler, Microsoft.Maui.Maps.IMap map)
        {
            if (handler.Map is null || handler.MauiContext is null)
            {
                return;
            }
            if (handler is CustomMapHandler mapHandler)
            {
                foreach (var marker in mapHandler.MarkerMap)
                {
                    marker.Value.Marker.Remove();
                }
                mapHandler.MarkerMap.Clear();
                mapHandler.AddPins();
            }
        }
        private BitmapDescriptor GetIcon(string icon)
        {
            try
            {
                if (_iconMap.TryGetValue(icon, out BitmapDescriptor? value))
                {
                    return value;
                }
                var drawable = Context.Resources.GetIdentifier(icon, "drawable", Context.PackageName);
                if (drawable == 0)
                {
                    Console.WriteLine($"Loading icon: {icon}, Drawable ID: {drawable}");
                    throw new InvalidOperationException($"Drawable resource '{icon}' not found.");
                }
                var bitmap = BitmapFactory.DecodeResource(Context.Resources, drawable);
                if (bitmap == null)
                {
                    Console.WriteLine($"Bitmap loaded: {bitmap != null}");
                    throw new InvalidOperationException($"Bitmap for '{icon}' could not be decoded.");
                }
                var mapEx = VirtualView as MapEx;
                double pinHeight = mapEx?.PinHeight ?? _iconSize;
                double pinWidth = mapEx?.PinWidth ?? _iconSize;

                //var scaled = Bitmap.CreateScaledBitmap(bitmap, _iconSize, _iconSize, false);
                var scaled = Bitmap.CreateScaledBitmap(bitmap, (int)pinWidth, (int)pinHeight, false);
                bitmap.Recycle();
                var descriptor = BitmapDescriptorFactory.FromBitmap(scaled);
                _iconMap[icon] = descriptor;
                return descriptor;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetIcon: {ex}");
                throw;
            }
        }
        private void AddPins()
        {
            if (VirtualView is MapEx mapEx && mapEx.CustomPins != null)
            {
                foreach (var pin in mapEx.CustomPins)
                {
                    var markerOption = new MarkerOptions();
                    markerOption.SetSnippet(pin.Address);
                    markerOption.SetTitle(pin.Label);
                    markerOption.SetIcon(GetIcon(pin.IconSrc));
                    markerOption.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
                    var marker = Map.AddMarker(markerOption);

                    MarkerMap.Add(marker.Id, (marker, pin));

                    var bitmap = BitmapFactory.DecodeResource(Context.Resources, Context.Resources.GetIdentifier
                                 (pin.IconSrc, "drawable", Context.PackageName));
                    //This Click Listener for Default Info Window 
                    Map.SetOnMarkerClickListener(new CustomMarkerClickListener(bitmap));
                }
            }
        }


        //This Marker Click for making custom info window 
        public void MarkerClick(object sender, GoogleMap.MarkerClickEventArgs args)
        {
            if (MarkerMap.TryGetValue(args.Marker.Id, out (Marker Marker, MapPin Pin) value))
            {
                value.Pin.ClickedCommand?.Execute(null);
            }
        }
    }
    public class CustomMarkerClickListener : Java.Lang.Object, GoogleMap.IOnMarkerClickListener
    {
        private readonly Bitmap? bitmap;
        private Marker? lastClickedMarker;

        public CustomMarkerClickListener(Bitmap? bitmap)
        {
            this.bitmap = bitmap;
        }
        public bool OnMarkerClick(Marker marker)
        {
            if (lastClickedMarker != null && lastClickedMarker.Equals(marker))
            {
                marker.HideInfoWindow();
                lastClickedMarker = null;
            }
            else
            {
                marker.HideInfoWindow();
                marker.ShowInfoWindow();
                lastClickedMarker = marker;
            }

            // This code are commented tO remove default marker icon 
            /*  marker.SetIcon(bitmap is null ? BitmapDescriptorFactory.DefaultMarker()
                  :BitmapDescriptorFactory.FromBitmap(bitmap));*/

            return true;
        }
    }
    public class MapCallbackHandler : Java.Lang.Object, IOnMapReadyCallback
    {
        private readonly CustomMapHandler mapHandler;
        public MapCallbackHandler(CustomMapHandler mapHandler)
        {
            this.mapHandler = mapHandler;
        }
        public void OnMapReady(GoogleMap googleMap)
        {
            mapHandler.UpdateValue(nameof(MapEx.CustomPins));
            googleMap.MarkerClick += mapHandler.MarkerClick;
            googleMap.UiSettings.ZoomControlsEnabled = false;
            googleMap.UiSettings.MyLocationButtonEnabled = false;

            var mapEx = mapHandler.VirtualView as MapEx;
            if (mapEx?.MapStyle != null)
            {
                ApplyMapStyle(googleMap, mapEx.MapStyle.JsonStyle);
            }
        }

        public async Task ApplyMapStyle(GoogleMap googleMap, string jsonStyle)
        {
            try
            {
                await Task.Delay(1);
                bool success = googleMap.SetMapStyle(new MapStyleOptions(jsonStyle));
                if (!success)
                {
                    Console.WriteLine("Failed to apply map style.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying map style: {ex.Message}");
            }
        }
    }
}
