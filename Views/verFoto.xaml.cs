namespace PM2E15044.Views;


public partial class verFoto : ContentPage
{
    private Models.sitioMaps SelectedSitio { get; }
    public verFoto(Models.sitioMaps selectedSitio)
    {
        InitializeComponent();
        SelectedSitio = selectedSitio;
        BindingContext = selectedSitio;
    }

    private void btnSalir_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
}