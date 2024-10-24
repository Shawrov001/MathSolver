using System.Windows;
using System.Windows.Controls;
using essai.codes;


namespace essai.pages.pagesMatrice {
    /// <summary>
    /// Logique d'interaction pour DeterminantPage.xaml
    /// </summary>
    public partial class DeterminantPage : Page {
        public DeterminantPage() {

            InitializeComponent();
            MatrixSize = 3;
        }

        private int _matrixSize;
        public int MatrixSize {
            get { return _matrixSize; }
            set {
                _matrixSize = value;
                MatrixUtils.UpdateMatrixGrid(MatrixGrid,MatrixSize);
            }
        }


        private void OnDetMatrixClick(object sender, RoutedEventArgs e) {
            List<List<float>> matrixData = new List<List<float>>();
            matrixData = MatrixUtils.GetMatrixData(MatrixGrid, MatrixSize);

            if (matrixData == null) {
                return;
            }

            Matrice matrice = new Matrice(matrixData);

            float res = matrice.Detn();
            MatrixUtils.DisplayDetMatrix(matrice, res,ref LatexMatrixDisplay);
        }
        
    }
}
