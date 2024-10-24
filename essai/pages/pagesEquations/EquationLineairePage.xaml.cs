using System.Text;
using System.Windows;
using System.Windows.Controls;
using WpfMath.Controls;
using essai.codes;

namespace essai.pages.pagesEquations
{
    /// <summary>
    /// Logique d'interaction pour EquationLineairePage.xaml
    /// </summary>
    public partial class EquationLineairePage : Page
    {
        public EquationLineairePage()
        {
            InitializeComponent();
            MatrixSize = 2;
        }

        private int _matrixSize;
        public int MatrixSize {
            get { return _matrixSize; }
            set {
                _matrixSize = value;
                UpdateEqLineaireGrid();
            }
        }

        char[] variables = { 'x', 'y', 'z', 'a', 'b', 'c', 'd' };


        private void UpdateEqLineaireGrid() {
            if (MatrixGrid != null) {
                // Met à jour le nombre de lignes dans la grille selon la taille de la matrice
                MatrixGrid.Rows = MatrixSize;

                // Calcule le nombre de colonnes, qui est fonction de la taille de la matrice
                MatrixGrid.Columns = 5 + 2 * (MatrixSize - 1);

                // Vide le contenu actuel de la grille
                MatrixGrid.Children.Clear();

                // Boucle pour chaque ligne de la grille
                for (int i = 0; i < MatrixGrid.Rows; i++) {
                    // Boucle pour chaque colonne de la grille
                    for (int j = 0; j < MatrixGrid.Columns; j++) {
                        // Vérifie si la colonne est une colonne paire
                        if (j % 2 == 0) {
                            TextBox textBox = new TextBox {
                                Width = 40, 
                                Height = 20,
                                Margin = new Thickness(0, 3, 0, 3), 
                                TextAlignment = TextAlignment.Center 
                            };
                            // Ajoute la TextBox à la grille
                            MatrixGrid.Children.Add(textBox);
                        }
                        else {
                            // Ajoute un TextBlock uniquement s'il ne s'agit pas de la dernière colonne
                            if (j < MatrixGrid.Columns - 1) {
                                // Vérifie si on est dans les colonnes de variables (avant les deux dernières colonnes)
                                if (j < MatrixGrid.Columns - 3) {
                                    StringBuilder latexBuilder = new StringBuilder();
                                    latexBuilder.Append($"{variables[(j - 1) / 2].ToString()} +"); // Ajoute la variable et le signe +

                                    // Création d'un contrôle FormulaControl pour afficher la formule
                                    FormulaControl forum = new FormulaControl {
                                        Formula = latexBuilder.ToString(), 
                                        VerticalAlignment = VerticalAlignment.Center, 
                                        HorizontalAlignment = HorizontalAlignment.Left 
                                    };
                                    MatrixGrid.Children.Add(forum);
                                }
                                else {
                                    // Si on est dans l'avant-dernière colonne, on ajoute le signe égal
                                    StringBuilder latexBuilder = new StringBuilder();
                                    latexBuilder.Append("="); // Ajoute le signe égal

                                    // Création d'un contrôle FormulaControl pour afficher le signe égal
                                    FormulaControl forum = new FormulaControl {
                                        Formula = latexBuilder.ToString(), 
                                        Width = 10, 
                                        VerticalAlignment = VerticalAlignment.Center, 
                                        HorizontalAlignment = HorizontalAlignment.Center 
                                    };
                                    MatrixGrid.Children.Add(forum);
                                }
                            }
                        }
                    }
                }
            }
        }


        private void OnEqLineaireClick(object sender, RoutedEventArgs e) {
            List<List<float>> matrixData = new List<List<float>>();
            List<float> vectorData = new List<float>();
            int index = 0;

            // Parcourir chaque ligne de la matrice
            for (int i = 0; i < MatrixSize; i++) {
                List<float> equationRow = new List<float>();

                // Parcourir chaque colonne de la matrice, en incluant seulement les TextBox
                for (int j = 0; j < MatrixGrid.Columns; j++) {
                    if (j % 2 == 0) { // On s'assure qu'on ne récupère que les valeurs des TextBox (qui sont aux index pairs)
                        TextBox textBox = (TextBox)MatrixGrid.Children[index];
                        if (float.TryParse(textBox.Text, out float value)) {
                            //vérifie si l'élément fait parti des 2 dernières colonnes
                            if (MatrixGrid.Columns - j < 4) {
                                //vérifie si l'élément fait parti de l'avant dernière colonne
                                if(MatrixGrid.Columns - j == 3) {
                                    vectorData.Add(-value);

                                }
                                else {
                                    vectorData[i] +=value;
                                }
                            }
                            else {
                                equationRow.Add(value);
                            }
                        }
                        else {
                            MessageBox.Show("Veuillez entrer des valeurs numériques valides.", "Erreur de saisie", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                    index++;
                    
                }
                matrixData.Add(equationRow);
            }


            Matrice A = new Matrice(matrixData);
            Vecteur B = new Vecteur(vectorData);

            Vecteur res = MathUtils.resoudre(A, B);

            DisplayEqLineaire(res);


        }
        private void DisplayEqLineaire(Vecteur res) {
            StringBuilder latexBuilder = new StringBuilder();

            for (int i = 0; i < res.n; i++) {

                latexBuilder.Append($"{variables[i]} = {MathUtils.FormatAsFraction(res.vec[i])}\\\\");
            }

            LatexMatrixDisplay.Formula = latexBuilder.ToString();
        }

    }
}
