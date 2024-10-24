using essai.codes;
using System.Windows;
using System.Windows.Controls;

namespace essai.pages.pageAccueil{
    /// <summary>
    /// Logique d'interaction pour Historique.xaml
    /// </summary>
    public partial class Historique : Page {

        
        

        public Historique() {
            InitializeComponent();
            InitializeDatabase();
            LoadHistorique();
        }

        private void InitializeDatabase() {
            DataBaseUtils.CreateDB();
        }

        private void LoadHistorique() {
            DataBaseUtils.LoadFromBD(HistoriqueStackPanel);
        }


        //private void DeleteDB() {
        //    DataBaseUtils.DeleteAllFromDB();
        //}

        private void ClearHistoryButton_Click(object sender, RoutedEventArgs e) {
            // Effacer l'historique de la base de données
            DataBaseUtils.DeleteAllFromDB();

            // Effacer le contenu affiché dans le StackPanel
            HistoriqueStackPanel.Children.Clear();
        }

    }

}
