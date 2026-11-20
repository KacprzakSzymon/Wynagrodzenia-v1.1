using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BindingList<WynagrodzenieModel> rejestrList;

        public MainWindow()
        {
            InitializeComponent();
            rejestrList = new BindingList<WynagrodzenieModel>();
            dgvRejestr.ItemsSource = rejestrList;
        }

        private void BtnObliczZapisz_Click(object sender, RoutedEventArgs e)
        {
            
            if (!decimal.TryParse(txtBrutto.Text.Trim(), out decimal brutto) || brutto <= 0)
            {
                MessageBox.Show("Podaj poprawną, dodatnią kwotę wynagrodzenia brutto!", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool isUmowaPracy = rbUmowaPracy.IsChecked == true;
            bool ulga = chkUlga.IsChecked == true;

            
            var wyniki = ObliczWynagrodzenie(brutto, isUmowaPracy, ulga);

            
            lblWynikNetto.Text = $"Wynagrodzenie netto: {wyniki.netto:N2} PLN";
            lblZus.Text = $"Składki ZUS (społeczne): {wyniki.zus:N2} PLN";
            lblZdrowotna.Text = $"Składka zdrowotna: {wyniki.zdrowotna:N2} PLN";
            lblPit.Text = $"Zaliczka na podatek PIT: {wyniki.pit:N2} PLN";

            
            var nowyWpis = new WynagrodzenieModel
            {
                Data = DateTime.Now,
                KwotaBrutto = brutto,
                TypUmowy = isUmowaPracy ? "O pracę" : "Zlecenie",
                Netto = wyniki.netto,
                Ulga = ulga ? "Tak" : "Nie"
            };

            rejestrList.Insert(0, nowyWpis); 
        }

        private (decimal netto, decimal zus, decimal zdrowotna, decimal pit) ObliczWynagrodzenie(decimal brutto, bool isUmowaPracy, bool ulgaDlaMlodych)
        {
            decimal zus = 0;
            decimal zdrowotna = 0;
            decimal pit = 0;

            if (isUmowaPracy)
            {
                
                zus = Math.Round(brutto * 0.1371m, 2);
                decimal podstawaOpodatkowania = brutto - zus;

                
                zdrowotna = Math.Round(podstawaOpodatkowania * 0.09m, 2);

                if (ulgaDlaMlodych)
                {
                    pit = 0;
                }
                else
                {
                    decimal kosztyUzyskania = 250m;
                    decimal podstawaPit = podstawaOpodatkowania - kosztyUzyskania;
                    if (podstawaPit < 0) podstawaPit = 0;
                    pit = Math.Round(podstawaPit * 0.12m - 300m, 2);
                    if (pit < 0) pit = 0;
                }
            }
            else
            {
                
                zus = Math.Round(brutto * 0.1371m, 2);
                decimal podstawaOpodatkowania = brutto - zus;
                zdrowotna = Math.Round(podstawaOpodatkowania * 0.09m, 2);

                if (ulgaDlaMlodych)
                {
                    pit = 0;
                }
                else
                {
                    decimal kosztyUzyskaniaZlec = podstawaOpodatkowania * 0.20m;
                    decimal podstawaPit = podstawaOpodatkowania - kosztyUzyskaniaZlec;
                    pit = Math.Round(podstawaPit * 0.12m, 2);
                }
            }

            decimal netto = brutto - zus - zdrowotna - pit;
            if (netto < 0) netto = 0;

            return (netto, zus, zdrowotna, pit);
        }
    }
}