using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using CarSales.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CarSales.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [INotifyPropertyChanged]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
        }

        [GeneratedRegex("[^0-9.,]+")]
        private static partial Regex DoubleNumericRegex();

        /// <summary>
        /// Validates textbox for numeric values on CTRL+V.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event args.</param>
        private void Txtnumeric_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                var text = (string)e.DataObject.GetData(DataFormats.Text);
                if (DoubleNumericRegex().IsMatch(text))
                    e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }

        /// <summary>
        /// Validates textbox for numeric values.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event args.</param>
        private void TxtNumeric_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = DoubleNumericRegex().IsMatch(e.Text);
        }
    }
}