using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using FFImageLoading.Maui;
using Firebase.Auth;
using Firebase.Database;
using Syncfusion.Maui.Core.Hosting;
using SimpleToolkit.Core;
using SimpleToolkit.SimpleShell;
using SkiaSharp.Views.Maui.Controls.Hosting;


#if IOS
using UIKit;
using Foundation;
#endif

#if ANDROID
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
#endif

namespace FoodNinja
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseFFImageLoading()
                .UseSimpleToolkit()
                .UseSimpleShell()
                .UseMauiMaps()
                .UseSkiaSharp()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("BentonSans_Bold.ttf", "BentonBold");
                    fonts.AddFont("BentonSans_Book.ttf", "BentonBook");
                    fonts.AddFont("BentonSans_Medium.ttf", "BentonMedium");
                    fonts.AddFont("BentonSans_Regular.ttf", "BentonRegular");
                })
                .ConfigureMauiHandlers(handlers =>
                {
#if ANDROID
			        handlers.AddHandler<Microsoft.Maui.Controls.Maps.Map, FoodNinja.Platforms.Android.CustomMapHandler>();
#elif IOS
                    handlers.AddHandler<Microsoft.Maui.Controls.Maps.Map, FoodNinja.Platforms.iOS.CustomMapHandler>();
#endif
                });

                builder.ConfigureSyncfusionCore();
                builder.Services.AddSingleton(new FirebaseConfig("AIzaSyD0ehwCKtxucZLyNcUGv-ZFKaNXmw_cmK8"));
                builder.Services.AddSingleton(new FirebaseClient("https://fir-maui-491c3-default-rtdb.firebaseio.com/"));

            //For Transparent the Statusbar in Android 
            /* builder.ConfigureLifecycleEvents(events =>
             {
#if ANDROID
                 events.AddAndroid(android => android.OnCreate((activity, bundle) => MakeStatusBarTranslucent(activity)));
                 static void MakeStatusBarTranslucent(Android.App.Activity activity)
                 {
                     activity.Window.SetFlags(Android.Views.WindowManagerFlags.LayoutNoLimits, Android.Views.WindowManagerFlags.LayoutNoLimits);
                     activity.Window.ClearFlags(Android.Views.WindowManagerFlags.TranslucentStatus);
                     activity.Window.SetStatusBarColor(Android.Graphics.Color.Transparent);
                 }
#endif
             });*/
#if DEBUG
            builder.Logging.AddDebug();
#endif

            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("SetUpEntry", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
                handler.PlatformView.TextCursorDrawable.SetTint(Android.Graphics.Color.ParseColor("#ff53e88b"));
#elif IOS || MACCATALYST
                handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
                handler.PlatformView.Layer.BorderWidth = 0;
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
                handler.PlatformView.TintColor = UIKit.UIColor.FromRGB(255, 99, 71);
#endif
            });

            Microsoft.Maui.Handlers.SearchBarHandler.Mapper.AppendToMapping("SearchBarBase", (handler, view) =>
            {
#if ANDROID
                Android.Widget.LinearLayout linearLayout = handler.PlatformView.GetChildAt(0) as Android.Widget.LinearLayout;
                linearLayout = linearLayout.GetChildAt(2) as Android.Widget.LinearLayout;
                linearLayout = linearLayout.GetChildAt(1) as Android.Widget.LinearLayout ;
                linearLayout.Background = null;
#endif
            });

            Microsoft.Maui.Handlers.DatePickerHandler.Mapper.AppendToMapping("Borderless", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.Background = null;
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToAndroid());
#elif IOS
                handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
                handler.PlatformView.Layer.BorderWidth = 0;
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
            });

            return builder.Build();
        }
    }
}