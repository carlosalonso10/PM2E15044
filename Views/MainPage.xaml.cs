using Microsoft.Maui.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;

namespace PM2E15044
{
    public partial class MainPage : ContentPage
    {
        Controllers.DBSitioMaps controller;
        FileResult photo;

        public MainPage()
        {
            InitializeComponent();
            controller = new Controllers.DBSitioMaps();
            InitializePage();
        }

        private async void InitializePage()
        {
            try
            {
                var locationPermissionStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                if (locationPermissionStatus == PermissionStatus.Granted)
                {
                    var location = await Geolocation.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Default,
                        Timeout = TimeSpan.FromSeconds(10)
                    });

                    if (location != null)
                    {
                        labelLatitude.Text = $"{location.Latitude}";
                        labelLongitude.Text = $"{location.Longitude}";
                    }
                    else
                    {
                        await DisplayAlert("Alerta", "No se pudo obtener la ubicaci�n actual.", "Ok");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Permiso de Ubicaci�n no otorgado. Es necesario para utilizar la aplicaci�n.", "OK");
                    // Implementar navegaci�n a una p�gina de configuraci�n o salida controlada
                    await Navigation.PopAsync();
                }
            }
            catch (FeatureNotEnabledException)
            {
                await DisplayAlert("Alerta", "El GPS se encuentra desactivado. Por favor active su GPS y abra la aplicaci�n de nuevo.", "Ok");
                // Implementar navegaci�n a una p�gina de configuraci�n o salida controlada
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en InitializePage: {ex.Message}");
                await DisplayAlert("Error", "Ocurri� un error al inicializar la p�gina.", "OK");
                // Implementar navegaci�n a una p�gina de configuraci�n o salida controlada
                await Navigation.PopAsync();
            }
        }

        public string GetImg64()
        {
            if (photo != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    Stream stream = photo.OpenReadAsync().Result;
                    stream.CopyTo(ms);
                    byte[] data = ms.ToArray();

                    string Base64 = Convert.ToBase64String(data);

                    return Base64;
                }
            }
            return null;
        }

        private async void btnAgregar_Clicked(object sender, EventArgs e)
        {
            string latitud = labelLatitude.Text;
            string longitud = labelLongitude.Text;
            string descripcion = entryDescripcion.Text;

            if (photo == null)
            {
                await DisplayAlert("Error", "Por favor tome una fotograf�a", "OK");
                return;
            }

            if (latitud == "00.00" || longitud == "00.00")
            {
                await DisplayAlert("Error", "No hay datos de longitud y latitud", "OK");
                return;
            }

            if (string.IsNullOrEmpty(descripcion))
            {
                await DisplayAlert("Error", "Por favor ingrese una descripci�n", "OK");
                return;
            }

            var sitio = new Models.sitioMaps
            {
                latitud = double.Parse(latitud),
                longitud = double.Parse(longitud),
                descripcion = descripcion,
                imagen = GetImg64()
            };

            try
            {
                if (controller != null)
                {
                    if (await controller.InsertMapaSitio(sitio) > 0)
                    {
                        await DisplayAlert("Aviso", "Registro ingresado con �xito!", "OK");
                        labelLatitude.Text = "00.00";
                        labelLongitude.Text = "00.00";
                        InitializePage();
                        entryDescripcion.Text = null;
                        photo = null;
                        imgSitio.Source = "defaultsite.png";
                    }
                    else
                    {
                        await DisplayAlert("Error", "Ocurri� un error al intentar insertar el registro.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurri� un error: {ex.Message}", "OK");
            }
        }

        private void btnListaSitios_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Views.listaSitios());
        }

        private async void btnSalir_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var result = await DisplayAlert("Confirmaci�n", "�Est�s seguro que quieres salir?", "S�", "No");

                if (result)
                {
                    await Navigation.PopAsync(); // Cerrar la p�gina actual
                    // Opcional: Para salir completamente de la aplicaci�n
                    // DependencyService.Get<IExitAppService>().ExitApp(); // Necesitas implementar IExitAppService en cada plataforma
                }
            });
        }
        private async void btnTomarFoto_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Verificar y solicitar permiso de c�mara en tiempo real
                var cameraPermissionStatus = await Permissions.RequestAsync<Permissions.Camera>();

                if (cameraPermissionStatus != PermissionStatus.Granted)
                {
                    await DisplayAlert("Error", "No se otorg� el permiso de c�mara.", "OK");
                    return;
                }

                // Continuar con la captura de la foto
                photo = await MediaPicker.CapturePhotoAsync();

                if (photo != null)
                {
                    string photoPath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
                    using Stream sourcephoto = await photo.OpenReadAsync();
                    using FileStream streamlocal = File.OpenWrite(photoPath);

                    imgSitio.Source = ImageSource.FromStream(() => photo.OpenReadAsync().Result); // Mostrar la imagen capturada

                    await sourcephoto.CopyToAsync(streamlocal); // Guardar la imagen localmente
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al capturar la foto: {ex.Message}", "OK");
            }
        }
    }
}