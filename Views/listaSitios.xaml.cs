using Microsoft.Maui.Controls;

namespace PM2E15044.Views;

public partial class listaSitios : ContentPage
{
    private Controllers.DBSitioMaps controller;
    private List<Models.sitioMaps> sitios;
    private Models.sitioMaps SelectedSitio { get; set; }
    private Grid selectedGrid = null;

    public listaSitios()
    {
        InitializeComponent();
        controller = new Controllers.DBSitioMaps();
    }

    //Metodo que permite mostrar la lista mientras la pagina se esta mostrando o cargando
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Obtiene la lista de personas de la base de datos
        sitios = await controller.getListSitio();

        // Coloca la lista en el collection view
        listUbicacion.ItemsSource = sitios;
    }

    private void btnSalir_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private async void btnEliminar_Clicked(object sender, EventArgs e)
    {
        if (SelectedSitio != null)
        {
            var confirmed = await DisplayAlert("Borrar Sitio Visitado", $"Esta seguro que desea borrar el sitio visitado: {SelectedSitio.descripcion}?", "Si", "No");

            if (confirmed)
            {
                // Borra el sitio seleccionado
                var result = await controller.deleteSitio(SelectedSitio.Id);

                if (result > 0)
                {
                    await DisplayAlert("Sitio Borrado", $"{SelectedSitio.descripcion} ha sido borrado exitosamente!", "OK");

                    // Actualiza la lista despues de borrar
                    sitios = await controller.getListSitio();
                    listUbicacion.ItemsSource = sitios;

                    // Clear the selected item
                    SelectedSitio = null;
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo borrar el sitio", "OK");
                }
            }
        }
        else
        {
            await DisplayAlert("Error", "Seleccione un sitio antes de eliminar", "OK");
        }
    }

    private async void btnActualizar_Clicked(object sender, EventArgs e)
    {
        if (SelectedSitio != null)
        {
            await Navigation.PushAsync(new Views.editSitio(SelectedSitio));
        }
    }

    private async void btnVerMaps_Clicked(object sender, EventArgs e)
    {
        if (SelectedSitio != null)
        {
            // Navigate to the VerMapaPage and pass the SelectedSitio
            await Navigation.PushAsync(new Views.verMapa(SelectedSitio));
        }
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (SelectedSitio != null)
        {
            VisualStateManager.GoToState(selectedGrid, "Normal");
        }


        // Handle the tap event
        selectedGrid = sender as Grid;
        SelectedSitio = selectedGrid?.BindingContext as Models.sitioMaps;



        if (SelectedSitio != null)
        {
            // Set the visual state to 'Selected'
            VisualStateManager.GoToState(selectedGrid, "Selected");

        }
    }

    private async void TapGestureFoto_Tapped(object sender, TappedEventArgs e)
    {
        if (SelectedSitio != null)
        {
            await Navigation.PushAsync(new Views.verFoto(SelectedSitio));
            SelectedSitio = null;
        }
    }
}