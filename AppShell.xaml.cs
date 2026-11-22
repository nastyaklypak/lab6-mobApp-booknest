namespace BookNest;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        // Сторінка книжки відкривається поверх меню
        Routing.RegisterRoute("detail", typeof(DetailPage));
    }
}
