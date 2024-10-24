using System.Text;
using System.Windows.Controls;
using System.Windows;

using essai.codes;

namespace essai.pages.pagesMatrice {
    public partial class InverserMatricePage : Page {
        public InverserMatricePage() {
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

        private void OnInverseMatrixClick(object sender, RoutedEventArgs e) {
            List<List<float>> matrixData = new List<List<float>>();
            matrixData = MatrixUtils.GetMatrixData(MatrixGrid, MatrixSize);
            if (matrixData == null) {
                return;
            }
            Matrice matrice = new Matrice(matrixData);
            //on vérifie d'abord si la matrice est inversible avant de passer aux opérations
            if (matrice.estInversible()) {
                Matrice invertedMatrix = matrice.Inverse();
                MatrixUtils.DisplayInvertedMatrix(matrice, invertedMatrix, ref LatexMatrixDisplay);
            }
            else {
                StringBuilder latexBuilder = new StringBuilder();
                latexBuilder.Append(@"\text{La matrice n'est pas inversible}");
                LatexMatrixDisplay.Formula = latexBuilder.ToString();
            }

        }
    }
}
