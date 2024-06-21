using PM2E15044.Extensions;
namespace PM2E15044.Views;

public partial class editSitio : ContentPage
{
    private Models.sitioMaps SelectedSitio { get; }
    private Models.sitioMaps editedSitio;

    public editSitio(Models.sitioMaps selectedSitio)
    {
        InitializeComponent();
        SelectedSitio = selectedSitio;
        BindingContext = editedSitio = selectedSitio;
    }

    private async void btnActualizar_Clicked(object sender, EventArgs e)
    {
        // Update the 'descripcion' field in the editedSitio object
        editedSitio.descripcion = entryDescripcion.Text;

        // Call the update method with the editedSitio object
        await App.DBSitioMapsController.updateSitios(editedSitio);

        // Display an alert or navigate back
        await DisplayAlert("Sitio Actualizado!", "La información del sitio ha sido actualizada!", "OK");
        await Navigation.PopAsync();
    }

    private void btnRegresar_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
}