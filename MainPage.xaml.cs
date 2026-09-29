using SQLite;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await UpdateCounter();
        }

        private async Task UpdateCounter()
        {
            try
            {
                string databasePath = System.IO.Path.Combine(
                    FileSystem.AppDataDirectory,
                    "database.db3");

                SQLiteAsyncConnection database =
                    new SQLiteAsyncConnection(databasePath);
                await database.CreateTableAsync<ScanRecord>();
                DateTime today = DateTime.Today;
                DateTime tomorrow = today.AddDays(1);
                int count = await database.Table<ScanRecord>()
                    .Where(x =>
                        x.ScanDate >= today &&
                        x.ScanDate < tomorrow)
                    .CountAsync();
                TodayCount.Text = count.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlert(
                    "Błąd",
                    "Nie udało się pobrać liczby logowań:\n" + ex.Message,
                    "OK");
            }
        }
        private async void OnHomeClicked(object sender, EventArgs e)
        {
           // await Shell.Current.GoToAsync("//MainPage");
        }
        private async void OnCalendarClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//Kalendarz");
        }

        private async void OnQrClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//QR");
        }

        private async void OnDatabaseClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//DB");
        }
    }
}