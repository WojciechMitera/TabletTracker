using SQLite;
namespace MauiApp1;

public partial class DB : ContentPage
{
    public DB()
    {
        InitializeComponent();

        Loaded += DB_Loaded;
    }
    private async void DB_Loaded(object? sender, EventArgs e)
    {
        Loaded -= DB_Loaded;

        await LoadDatabase();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await LoadDatabase();
    }
    private async Task LoadDatabase()
    {
        DatabaseLayout.Children.Clear();
        try
        {
            string databasePath = System.IO.Path.Combine(
                FileSystem.AppDataDirectory,
                "database.db3");

            SQLiteAsyncConnection database =
                new SQLiteAsyncConnection(databasePath);

            await database.CreateTableAsync<ScanRecord>();

            List<ScanRecord> records =
                await database.Table<ScanRecord>()
                .OrderByDescending(x => x.ScanDate)
                .ToListAsync();

            foreach (ScanRecord record in records)
            {
                AddRecord(record);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "B³¹d",
                "Nie uda³o siê wczytaæ danych z bazy:\n" + ex.Message,
                "OK");
        }
    }
    private void AddRecord(ScanRecord record)
    {
        Grid row = new Grid
        {
            HeightRequest = 55,
            Padding = new Thickness(10, 0),

            ColumnDefinitions =
            {
                new ColumnDefinition
                {
                    Width = new GridLength(1.5, GridUnitType.Star)
                },

                new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                },

                new ColumnDefinition
                {
                    Width = new GridLength(1.5, GridUnitType.Star)
                }
            }
        };

        Label mail = new Label
        {
            Text = record.Mail,
            TextColor = Color.FromArgb("#FFFFFF"),
            FontSize = 15,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center
        };

        Label tablet = new Label
        {
            Text = record.Tablet,
            TextColor = Color.FromArgb("#FFFFFF"),
            FontSize = 15,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center
        };

        Label date = new Label
        {
            Text = record.ScanDate.ToString("dd.MM.yyyy HH:mm:ss"),
            TextColor = Color.FromArgb("#FFFFFF"),
            FontSize = 15,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center
        };

        Grid.SetColumn(mail, 0);
        Grid.SetColumn(tablet, 1);
        Grid.SetColumn(date, 2);

        row.Children.Add(mail);
        row.Children.Add(tablet);
        row.Children.Add(date);

        DatabaseLayout.Children.Add(row);

        BoxView separator = new BoxView
        {
            HeightRequest = 1,
            BackgroundColor = Color.FromArgb("#30475E")
        };

        DatabaseLayout.Children.Add(separator);
    }

    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
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
       // await Shell.Current.GoToAsync("//DB");
    }
}