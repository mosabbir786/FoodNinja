using FoodNinja.Model;
using Microsoft.Maui.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.Custom
{
    public class MapEx : Microsoft.Maui.Controls.Maps.Map
    {
        public static readonly BindableProperty MapStyleProperty = BindableProperty.Create(
        nameof(MapStyle),
        typeof(MapStyle),
        typeof(MapEx),
        propertyChanged: OnMapStyleChanged);
        public MapStyle? MapStyle
        {
            get { return (MapStyle?)GetValue(MapStyleProperty); }
            set { SetValue(MapStyleProperty, value); }
        }
        public Microsoft.Maui.Maps.IMap? Map { get; set; }
        static async void OnMapStyleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (MapEx)bindable;
            var mapStyle = control.MapStyle;
            if (control.Handler?.PlatformView is null)
            {
                // Workaround for when this executes the Handler and PlatformView is null
                control.HandlerChanged += OnHandlerChanged;
                return;
            }
            if (mapStyle is not null)
            {
#if ANDROID || IOS || MACCATALYST
#else
                await Task.CompletedTask;
#endif
            }
            else
            {
#if ANDROID
#elif IOS
#endif
            }
            void OnHandlerChanged(object? s, EventArgs e)
            {
                OnMapStyleChanged(control, oldValue, newValue);
                control.HandlerChanged -= OnHandlerChanged;
            }
        }
        public List<MapPin> CustomPins
        {
            get { return (List<MapPin>)GetValue(CustomPinsProperty); }
            set { SetValue(CustomPinsProperty, value); }
        }
        public static readonly BindableProperty CustomPinsProperty = BindableProperty.Create(
            nameof(CustomPins),
            typeof(List<MapPin>),
            typeof(MapEx),
            null);
        public double PinHeight
        {
            get { return (double)GetValue(PinHeightProperty); }
            set { SetValue(PinHeightProperty, value); }
        }
        public static readonly BindableProperty PinHeightProperty = BindableProperty.Create(
           propertyName: nameof(PinHeight),
           returnType: typeof(double),
           declaringType: typeof(MapEx),
           defaultValue: 60.0);
        public double PinWidth
        {
            get { return (double)GetValue(PinWidthProperty); }
            set { SetValue(PinWidthProperty, value); }
        }
        public static readonly BindableProperty PinWidthProperty = BindableProperty.Create(
            propertyName: nameof(PinWidth),
            returnType: typeof(double),
            declaringType: typeof(MapEx),
            defaultValue: 60.0);
    }
    public sealed class MapStyle
    {
        public static MapStyle FromJson(string jsonStyle)
        {
            return new MapStyle(jsonStyle);
        }
        public string JsonStyle { get; }
        private MapStyle(string jsonStyle)
        {
            this.JsonStyle = jsonStyle;
        }
    }
}
