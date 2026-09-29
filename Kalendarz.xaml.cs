using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using SQLite;

namespace MauiApp1;

public partial class Kalendarz : ContentPage
{
    private bool[] expandedDays = new bool[7];

    public Kalendarz()
    {
        InitializeComponent();

        Loaded += Kalendarz_Loaded;
    }

    private async void Kalendarz_Loaded(object? sender, EventArgs e)
    {
        Loaded -= Kalendarz_Loaded;

        await LoadCalendar();
    }
    private async Task LoadCalendar()
    {
        DaysLayout.Children.Clear();

        DateTime today = DateTime.Today;

        for (int i = 0; i < 7; i++)
        {
            DateTime date = today.AddDays(-i);

            int index = i;

            Border dayBorder = CreateDay(date, index);

            DaysLayout.Children.Add(dayBorder);
        }
    }
    private Border CreateDay(DateTime date, int index)
    {
        VerticalStackLayout mainLayout = new VerticalStackLayout
        {
            Spacing = 0
        };
        Grid header = new Grid
        {
            HeightRequest = 55,
            Padding = new Thickness(15, 0),
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            }
        };


        string dayName = GetDayName(date.DayOfWeek);

        Label dayLabel = new Label
        {
            Text = $"{dayName}  {date:dd.MM.yyyy}",
            FontSize = 18,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            VerticalOptions = LayoutOptions.Center
        };

        header.Add(dayLabel);
        Grid.SetColumn(dayLabel, 0);


        Label arrow = new Label
        {
            Text = "▼",
            FontSize = 18,
            TextColor = Colors.White,
            VerticalOptions = LayoutOptions.Center
        };

        header.Add(arrow);
        Grid.SetColumn(arrow, 1);

        VerticalStackLayout recordsLayout = new VerticalStackLayout
        {
            Spacing = 0,
            IsVisible = false
        };
        TapGestureRecognizer tap = new TapGestureRecognizer();

        tap.Tapped += async (s, e) =>
        {
            expandedDays[index] = !expandedDays[index];

            if (expandedDays[index])
            {
                arrow.Text = "▲";

                recordsLayout.Children.Clear();

                List<ScanRecord> records = await GetRecordsForDay(date);

                if (records.Count == 0)
                {
                    Label emptyLabel = new Label
                    {
                        Text = "Brak zapisów tego dnia.",
                        FontSize = 15,
                        TextColor = Colors.LightGray,
                        Padding = new Thickness(15, 12)
                    };

                    recordsLayout.Children.Add(emptyLabel);
                }
                else
                {
                    Grid tableHeader = new Grid
                    {
                        Padding = new Thickness(10, 8),
                        ColumnDefinitions =
                        {
                            new ColumnDefinition(GridLength.Star),
                            new ColumnDefinition(GridLength.Star),
                            new ColumnDefinition(GridLength.Star)
                        }
                    };

                    Label studentHeader = CreateTableLabel("UCZEŃ",true);
                    Label tabletHeader = CreateTableLabel("TABLET",true);
                    Label dateHeader = CreateTableLabel("DATA",true);

                    tableHeader.Add(studentHeader);
                    Grid.SetColumn(studentHeader, 0);

                    tableHeader.Add(tabletHeader);
                    Grid.SetColumn(tabletHeader, 1);

                    tableHeader.Add(dateHeader);
                    Grid.SetColumn(dateHeader, 2);


                    recordsLayout.Children.Add(tableHeader);

                    foreach (ScanRecord record in records)
                    {
                        Grid row = new Grid
                        {
                            Padding = new Thickness(10, 8),
                            ColumnDefinitions =
                            {
                                new ColumnDefinition(GridLength.Star),
                                new ColumnDefinition(GridLength.Star),
                                new ColumnDefinition(GridLength.Star)
                            }
                        };


                        Label student = CreateTableLabel(
                            record.Mail,
                            false);

                        Label tablet = CreateTableLabel(
                            record.Tablet,
                            false);

                        Label scanDate = CreateTableLabel(
                            record.ScanDate.ToString("dd.MM.yyyy HH:mm"),
                            false);

                        row.Add(student);
                        Grid.SetColumn(student, 0);

                        row.Add(tablet);
                        Grid.SetColumn(tablet, 1);
                        row.Add(scanDate);
                        Grid.SetColumn(scanDate, 2);


                        recordsLayout.Children.Add(row);
                        BoxView line = new BoxView
                        {
                            HeightRequest = 1,
                            BackgroundColor = Color.FromArgb("#71889C")
                        };

                        recordsLayout.Children.Add(line);
                    }
                }
            }
            else
            {
                arrow.Text = "▼";
            }
            recordsLayout.IsVisible = expandedDays[index];
        };

        header.GestureRecognizers.Add(tap);


        mainLayout.Children.Add(header);
        mainLayout.Children.Add(recordsLayout);

        Border border = new Border
        {
            BackgroundColor = Color.FromArgb("#607B90"),
            Stroke = Color.FromArgb("#8CA0B0"),
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = 8
            },
            Content = mainLayout
        };


        return border;
    }
    private async Task<List<ScanRecord>> GetRecordsForDay(DateTime date)
    {
        try
        {
            string databasePath = System.IO.Path.Combine(
                FileSystem.AppDataDirectory,
                "database.db3"
            );

            SQLiteAsyncConnection database = new SQLiteAsyncConnection(databasePath);

            DateTime start = date.Date;
            DateTime end = date.Date.AddDays(1);
            List<ScanRecord> records = await database.Table<ScanRecord>()
                .Where(x => x.ScanDate >= start &&
                            x.ScanDate < end)
                .OrderBy(x => x.ScanDate)
                .ToListAsync();
            return records;
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Błąd",
                "Nie udało się odczytać danych z bazy:\n" + ex.Message,
                "OK");

            return new List<ScanRecord>();
        }
    }

    private Label CreateTableLabel(string text, bool header)
    {
        return new Label
        {
            Text = text,
            FontSize = header ? 12 : 13,
            FontAttributes = header
                ? FontAttributes.Bold
                : FontAttributes.None,
            TextColor = Colors.White,
            VerticalOptions = LayoutOptions.Center,
            LineBreakMode = LineBreakMode.TailTruncation
        };
    }

    private string GetDayName(DayOfWeek day)
    {
        return day switch
        {
            DayOfWeek.Monday => "PON",
            DayOfWeek.Tuesday => "WT",
            DayOfWeek.Wednesday => "ŚR",
            DayOfWeek.Thursday => "CZW",
            DayOfWeek.Friday => "PT",
            DayOfWeek.Saturday => "SOB",
            DayOfWeek.Sunday => "NIE",
            _ => ""
        };
    }

    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
    private async void OnQrClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//QR");
    }
    private async void OnCalendarClicked(object sender, EventArgs e)
    {
        //await Shell.Current.GoToAsync("//Kalendarz");
    }

    private async void OnDatabaseClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//DB");
    }
}