using Android.App;
using Android.Content.PM;
using Android.OS;

namespace MauiApp1
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

#if ANDROID
            // Ustawia kolor paska na taki sam jak tło Twojego menu (ciemnoszary)
            // Podmień #2E3A46 na dokładny kolor HEX swojego paska z ikonami
            Window.SetNavigationBarColor(Android.Graphics.Color.ParseColor("#526D82"));
#endif
        }
    }
}
