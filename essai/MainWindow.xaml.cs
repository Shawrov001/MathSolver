using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;




namespace essai {
    public partial class MainWindow : Window {
        

        public MainWindow() {
            InitializeComponent();
            MainFrame.Navigate(new pages.pageAccueil.Accueil());
        }

        private void TogglePanelVisibility(StackPanel targetPanel) {
            // Masque tous les panneaux
            if (targetPanel.Visibility == Visibility.Visible) {
                //ferme tous les panneaux
                //autres panel à ajouter obligatoirement ici
                MatricePanel.Visibility = Visibility.Collapsed;
                EquationsPanel.Visibility = Visibility.Collapsed;

            }
            else {
                //ferme tous les panneaux et affiche le target
                //autres panel à ajouter obligatoirement ici
                MatricePanel.Visibility = Visibility.Collapsed;
                EquationsPanel.Visibility = Visibility.Collapsed;

                targetPanel.Visibility = Visibility.Visible;
            }

        }

        private void HistoriqueButton_Click(object sender, RoutedEventArgs e) {
            //création d'un frame différent pour régler le problème de slider
            // Naviguer vers la page Historique
            MatricePanel.Visibility = Visibility.Collapsed;
            EquationsPanel.Visibility = Visibility.Collapsed;
            MainFrame.Visibility = Visibility.Collapsed;

            HistoriqueFrame.Visibility = Visibility.Visible;
            HistoriqueFrame.Navigate(new pages.pageAccueil.Historique());

        }

        private void linkedinButton_Click(object sender, RoutedEventArgs e) {
            string url = "https://www.linkedin.com/in/shawrov-das-330144242/";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        private void githubButton_Click(object sender, RoutedEventArgs e) {
            string url = "https://github.com/Shawrov001";
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }


        private void AccueilButton_Click(object sender, RoutedEventArgs e) {
            MatricePanel.Visibility = Visibility.Collapsed;
            EquationsPanel.Visibility = Visibility.Collapsed;
            HistoriqueFrame.Visibility = Visibility.Collapsed;
            MainFrame.Visibility = Visibility.Visible;
            MainFrame.Navigate(new pages.pageAccueil.Accueil());
        }

        private void MatriceButton_Click(object sender, RoutedEventArgs e) {
            TogglePanelVisibility(MatricePanel);
        }

        private void InverserMatriceButton_Click(object sender, RoutedEventArgs e) {
            // Naviguer vers la page InverserMatricePage
            MatricePanel.Visibility = Visibility.Collapsed;
            HistoriqueFrame.Visibility = Visibility.Collapsed;
            MainFrame.Visibility = Visibility.Visible;
            MainFrame.Navigate(new pages.pagesMatrice.InverserMatricePage());
        }

        private void DeterminantButton_Click(object sender, RoutedEventArgs e) {
            // Naviguer vers la page DeterminantPage
            HistoriqueFrame.Visibility = Visibility.Collapsed;
            MatricePanel.Visibility = Visibility.Collapsed;
            MainFrame.Visibility = Visibility.Visible;
            MainFrame.Navigate(new pages.pagesMatrice.DeterminantPage());


        }

        private void ComatriceButton_Click(object sender, RoutedEventArgs e) {
            // Naviguer vers la page ComatricePage
            MatricePanel.Visibility = Visibility.Collapsed;
            HistoriqueFrame.Visibility = Visibility.Collapsed;
            MainFrame.Visibility = Visibility.Visible;
            MainFrame.Navigate(new pages.pagesMatrice.ComatricePage());

        }

        private void AdjointButton_Click(object sender, RoutedEventArgs e) {
            // Naviguer vers la page ComatricePage
            MatricePanel.Visibility = Visibility.Collapsed;
            HistoriqueFrame.Visibility = Visibility.Collapsed;
            MainFrame.Visibility = Visibility.Visible;
            MainFrame.Navigate(new pages.pagesMatrice.AdjointPage());

        }


        private void EquationsButton_Click(object sender, RoutedEventArgs e) {
            TogglePanelVisibility(EquationsPanel);
        }

        private void EquationLineaireButton_Click(object sender, RoutedEventArgs e) {
            // Naviguer vers la page EquationLineairePage
            EquationsPanel.Visibility = Visibility.Collapsed;
            HistoriqueFrame.Visibility = Visibility.Collapsed;
            MainFrame.Visibility = Visibility.Visible;
            MainFrame.Navigate(new pages.pagesEquations.EquationLineairePage());
        }
    }
}

