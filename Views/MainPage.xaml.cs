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

        public MainPage(Controllers.DBSitioMaps dbPath)
        {
            InitializeComponent();
            controller = dbPath;
            InitializePage();
        }

        private async void InitializePage()
        {
            try
            {
                // Revisa si el permiso de ubicacion ha sido concedido
                var locationPermissionStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                if (locationPermissionStatus == PermissionStatus.Granted)
                {
                    // Obtiene la ubicacion
                    var location = await Geolocation.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Default,
                        Timeout = TimeSpan.FromSeconds(10)
                    });

                    if (location != null)
                    {
                        // Coloca la latitude y longitud en los labels
                        labelLatitude.Text = $"{location.Latitude}";
                        labelLongitude.Text = $"{location.Longitude}";
                    }
                    else if (labelLatitude.Text.Equals("00.00") || labelLongitude.Text.Equals("00.00"))
                    {
                        // Cuando la ubicacion es nula
                        await DisplayAlert("Alerta", "El GPS se encuentra desactivado. Porfavor active su GPS y abra la aplicación de nuevo!", "Ok");
                    }
                }
                else
                {
                    // Cuando el permiso no es otorgado
                    await DisplayAlert("Error", "Permiso de Ubicación no otorgado. El Permiso es necesario para utilizar la aplicacion.", "OK");
                    Application.Current.Quit();
                }
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

        }

        public string? GetImg64()
        {
            if (photo != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    Stream stream = photo.OpenReadAsync().Result;
                    stream.CopyTo(ms);
                    byte[] data = ms.ToArray();

                    String Base64 = Convert.ToBase64String(data);

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

            if (photo != null)
            {
                if (labelLatitude.Text.Equals("00.00") || labelLongitude.Text.Equals("00.00"))
                {
                    await DisplayAlert("Error", "No hay datos de longitud y latitud", "OK");
                    return;
                }
                else if (string.IsNullOrEmpty(descripcion))
                {
                    await DisplayAlert("Error", "Porfavor ingrese una descripción", "OK");
                    return;
                }
            }
            else
            {
                await DisplayAlert("Error", "Porfavor tome una fotografía", "OK");
                return;
            }

            var sitio = new Models.sitioMaps
            {
                latitud = double.Parse(labelLatitude.Text),
                longitud = double.Parse(labelLongitude.Text),
                descripcion = entryDescripcion.Text,
                imagen = GetImg64()
            };

            try
            {
                if (controller != null)
                {
                    if (await controller.InsertMapaSitio(sitio) > 0)
                    {
                        await DisplayAlert("Aviso", "Registro Ingresado con Exito!", "OK");
                        labelLatitude.Text = "00.00";
                        labelLongitude.Text = "00.00";
                        InitializePage();
                        entryDescripcion.Text = null;
                        photo = null;
                        imgSitio.Source = "defaultsite.png";

                    }
                    else
                    {
                        await DisplayAlert("Error", "Ocurrio un Error", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrio un Error: {ex.Message}", "OK");
            }


        }

        private void btnListaSitios_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Views.listaSitios());
        }

        private void btnSalir_Clicked(object sender, EventArgs e)
        {
            Application.Current.Quit();
        }

        private async void btnTomarFoto_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Request camera permission
                var cameraPermissionStatus = await Permissions.RequestAsync<Permissions.Camera>();

                if (cameraPermissionStatus != PermissionStatus.Granted)
                {
                    await DisplayAlert("Error", "No se otorgó el permiso de cámara.", "OK");
                    return;
                }

                // Continue with capturing the photo after permission is granted
                photo = await MediaPicker.CapturePhotoAsync();

                if (photo != null)
                {
                    string photoPath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
                    using Stream sourcephoto = await photo.OpenReadAsync();
                    using FileStream streamlocal = File.OpenWrite(photoPath);

                    imgSitio.Source = ImageSource.FromStream(() => photo.OpenReadAsync().Result); // Display the image
                    await sourcephoto.CopyToAsync(streamlocal); // Save the image locally
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al capturar la foto: {ex.Message}", "OK");
            }
        }
    }
}