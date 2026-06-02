using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace CarSales
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <inheritdoc/>
        protected override void OnStartup(StartupEventArgs e)
        {
            var czechCulture = new CultureInfo("cs-CZ");

            CultureInfo.CurrentCulture = czechCulture;
            CultureInfo.CurrentUICulture = czechCulture;

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage("cs-CZ"))
            );

            base.OnStartup(e);
        }
    }

}
