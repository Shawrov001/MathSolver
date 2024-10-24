using essai.codes;
using System.Windows.Controls;


namespace essai.pages.pagesMatrice
{
    /// <summary>
    /// Logique d'interaction pour AdjointPage.xaml
    /// </summary>
    public partial class AdjointPage : Page
    {
        public AdjointPage()
        {
            InitializeComponent();
            MatrixSize = 3;
        }

        private int _matrixSize;
        public int MatrixSize {
            get { return _matrixSize; }
            set {
                _matrixSize = value;
                MatrixUtils.UpdateMatrixGrid(MatrixGrid, MatrixSize);
            }
        }

        private void OnAdjointMatrixClick(object sender, EventArgs e) {
            List<List<float>> matrixData = new List<List<float>>();
            matrixData = MatrixUtils.GetMatrixData(MatrixGrid, MatrixSize);
            if (matrixData == null) {
                return;
            }
            Matrice matrice = new Matrice(matrixData);

            Matrice res = matrice.Adj();

            MatrixUtils.DisplayAdjMatrix(matrice, res, ref LatexMatrixDisplay);
        }
    }
}
