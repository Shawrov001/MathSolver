using System.Windows.Controls;
using essai.codes;


namespace essai.pages.pagesMatrice {
    /// <summary>
    /// Logique d'interaction pour TracePage.xaml
    /// </summary>
    public partial class ComatricePage : Page {
        public ComatricePage() {
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

        private void OnComatriceMatrixClick(object sender, EventArgs e) {
            List<List<float>> matrixData = new List<List<float>>();
            matrixData = MatrixUtils.GetMatrixData(MatrixGrid, MatrixSize);
            if (matrixData == null) {
                return;
            }
            Matrice matrice = new Matrice(matrixData);

            Matrice res = matrice.Com();

            MatrixUtils.DisplayCofMatrix(matrice, res, ref LatexMatrixDisplay);
        }
    }
}
