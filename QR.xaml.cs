namespace MauiApp1;
using System;
using System.Linq;
using SQLite;
using ZXing.Net.Maui;

public partial class QR : ContentPage
{
    private bool isSaving = false;

    public QR()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        cameraView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.OneDimensional | BarcodeFormats.TwoDimensional,
            AutoRotate = true,
            Multiple = true,
            TryHarder = true
        };

        cameraView.IsDetecting = true;
    }

    private void cameraView_BarcodesDetected(
    object sender,
    ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
        var result = e?.Results?.FirstOrDefault();

        if (result is null)
            return;

        string qrText = result.Value;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await DisplayAlert(
                "Odczytany QR",
                qrText,
                "OK");

            await SaveQrData(qrText);
        });
    }

    private async Task SaveQrData(string qrText)
    {
        if (isSaving)
            return;

        isSaving = true;

        cameraView.IsDetecting = false;

        try
        {
            string mail = "";
            string tablet = "";
            DateTime scanDate = DateTime.Now;

            string[] lines = qrText.Split(
                new[] { ';' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                string text = line.Trim();

                if (text.StartsWith("MAIL:", StringComparison.OrdinalIgnoreCase))
                {
                    mail = text.Substring(5).Trim();
                }
                else if (text.StartsWith("DATA:", StringComparison.OrdinalIgnoreCase))
                {
                    string dateText = text.Substring(5).Trim();

                    DateTime.TryParse(
                        dateText,
                        out scanDate);
                }
                else if (text.StartsWith("TOKEN:", StringComparison.OrdinalIgnoreCase))
                {
                    tablet = text.Substring(6).Trim();
                }
            }
            if(string.IsNullOrWhiteSpace(mail))            {
                await DisplayAlert(
                    "B³¹d",
                    "Kod QR nie zawiera adresu MAIL.",
                    "OK");

                return;
            }

            if (string.IsNullOrWhiteSpace(tablet))
            {
                await DisplayAlert(
                    "B³¹d",
                    "Kod QR nie zawiera numeru TABLETU.",
                    "OK");

                return;
            }

            string databasePath = System.IO.Path.Combine(
                FileSystem.AppDataDirectory,
                "database.db3");

            SQLiteAsyncConnection database =
                new SQLiteAsyncConnection(databasePath);
            await database.CreateTableAsync<ScanRecord>();

            ScanRecord record = new ScanRecord
            {
                Mail = mail,
                Tablet = tablet,
                ScanDate = scanDate
            };

            await database.InsertAsync(record);

            await DisplayAlert(
                "Sukces",
                "Kod QR zosta³ zapisany w bazie.",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "B³¹d",
                "Nie uda³o siê zapisaæ danych:\n" + ex.Message,
                "OK");
        }
        finally
        {
            isSaving = false;

            cameraView.IsDetecting = true;
        }
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
        //await Shell.Current.GoToAsync("//QR");
    }
    private async void OnDatabaseClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//DB");
    }
}