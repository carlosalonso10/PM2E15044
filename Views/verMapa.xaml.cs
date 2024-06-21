using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Text.Json;

namespace PM2E15044.Views;

public partial class verMapa : ContentPage
{
    private Models.sitioMaps SelectedSitio { get; }
    public verMapa(Models.sitioMaps selectedSitio)
    {
        InitializeComponent();
        SelectedSitio = selectedSitio;
        //labelSitio.Text = SelectedSitio.descripcion;
        this.Appearing += VerMapa_Appearing;
    }

    private async void VerMapa_Appearing(object sender, EventArgs e)
    {
        try
        {
            // Obtiene la ubicacion
            var location = await Geolocation.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Default,
                Timeout = TimeSpan.FromSeconds(10)
            });
        }
        catch (FeatureNotEnabledException)
        {
            try
            {
                await Application.Current.MainPage.DisplayAlert("Alerta", "El GPS se encuentra desactivado. Porfavor active su GPS y abra la aplicación de nuevo!", "Ok");
                Application.Current.Quit();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DisplayGpsNotEnabledAlert: {ex.Message}");
            }
        }


        await colocarSitio(SelectedSitio.latitud, SelectedSitio.longitud);
        MoveMapToRegion(SelectedSitio.latitud, SelectedSitio.longitud);

        this.Appearing -= VerMapa_Appearing;
    }

    public async Task colocarSitio(double latitude, double longitude)
    {
        //Variables para guardar datos del json
        string? adminDistrict = null;
        string? adminDistrict2 = null;
        string? countryRegion = null;

        string jsonResponse = await GetGeocodeDataAsync(latitude, longitude);

        if (jsonResponse != null)
        {
            // Parse the JSON response using JsonDocument
            JsonDocument jsonDocument = JsonDocument.Parse(jsonResponse);

            // Obtiene el campo adminDistrict del JSON
            adminDistrict = jsonDocument.RootElement
            .GetProperty("resourceSets")[0]
            .GetProperty("resources")[0]
            .GetProperty("address")
            .GetProperty("adminDistrict")
            .GetString();

            // Obtiene el campo adminDistrict2 del JSON
            adminDistrict2 = jsonDocument.RootElement
            .GetProperty("resourceSets")[0]
            .GetProperty("resources")[0]
            .GetProperty("address")
            .GetProperty("adminDistrict2")
            .GetString();

            // Obtiene el campo countryRegion del JSON
            countryRegion = jsonDocument.RootElement
            .GetProperty("resourceSets")[0]
            .GetProperty("resources")[0]
            .GetProperty("address")
            .GetProperty("countryRegion")
            .GetString();
        }

        // Agrega el Marcador en la ubicacion
        var marker = new Pin
        {
            Type = PinType.Place,
            Location = new Location(latitude, longitude),
            Label = SelectedSitio.descripcion,
            Address = adminDistrict2 + ", " + adminDistrict + ", " + countryRegion
        };

        labelSitio.Text = (adminDistrict2 + ", " + adminDistrict + ", " + countryRegion).ToString();
        mapa.Pins.Add(marker);
    }


    private async Task<string?> GetGeocodeDataAsync(double latitude, double longitude)
    {
        string BingMapsApiKey = "Au7sMtQzyQZRzuQ2krOIbZg8j2MGoHzzOJAmVym6vQjHq_BJ8a1YQGX3iCosFh8u";
        try
        {
            string apiUrl = $"https://dev.virtualearth.net/REST/v1/Locations/{latitude},{longitude}?o=json&key={BingMapsApiKey}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    // Lee la respuesta como String
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    return jsonResponse;
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return null;
        }
    }

    public void MoveMapToRegion(double latitude, double longitude)
    {
        // Se mueve a la ubicacion del pin
        Location location = new Location(latitude, longitude);
        MapSpan mapSpan = new MapSpan(location, 0.01, 0.01);
        mapa.MoveToRegion(mapSpan);
    }

    private void btnSalir_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private void btnShare_Clicked(object sender, EventArgs e)
    {
        string locationUrl = $"https://maps.google.com/?q={SelectedSitio.latitud},{SelectedSitio.longitud}";
        string message = $"¡Mira el lugar que visite!: {SelectedSitio.descripcion}";

        Share.RequestAsync(new ShareTextRequest
        {
            Text = $"{message}\n{locationUrl}",
            Title = "Comparte el Lugar"
        });
    }
}