using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using WpfMath.Controls;
using System.IO;
using System.Data.SQLite;
using System.ComponentModel.DataAnnotations.Schema;

namespace essai.codes
{
    public static class DataBaseUtils {


        public static string databasePath = "historique.db";

        //si la base de donnée n'existe pas, on la crée
        public static void CreateDB() {
            if (!File.Exists(databasePath)) {
                SQLiteConnection.CreateFile(databasePath);
            }

            using (var connexion = new SQLiteConnection($"Data Source={databasePath}")) {
                connexion.Open();
                string requete = @"CREATE TABLE IF NOT EXISTS Historique (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Texte TEXT NOT NULL,
                                    DateHeure TEXT NOT NULL
                                );";
                SQLiteCommand commande = new SQLiteCommand(requete, connexion);
                commande.ExecuteNonQuery();
            }
        }

        //on sauvegarde chaque équation latex dans la base de données
        public static void SaveInDB(string StringFormula) {
            if (!File.Exists(databasePath)) {
                CreateDB();
            }

            if (!string.IsNullOrWhiteSpace(StringFormula)) {
                string dateHeure = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                using (var connexion = new SQLiteConnection($"Data Source={databasePath}")) {
                    connexion.Open();
                    string requete = "INSERT INTO Historique (Texte, DateHeure) VALUES (@texte, @dateHeure)";
                    using (SQLiteCommand commande = new SQLiteCommand(requete, connexion)) {
                        commande.Parameters.AddWithValue("@texte", StringFormula);
                        commande.Parameters.AddWithValue("@dateHeure", dateHeure);
                        commande.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void LoadFromBD(StackPanel stackPanel) {
            using (var connexion = new SQLiteConnection($"Data Source={databasePath}")) {
                connexion.Open();
                string requete = "SELECT * FROM Historique ORDER BY DateHeure DESC";
                using (SQLiteCommand commande = new SQLiteCommand(requete, connexion)) {
                    using (SQLiteDataReader lecteur = commande.ExecuteReader()) {
                        while (lecteur.Read()) {
                            string text = lecteur["Texte"].ToString();
                            string dateHeure = lecteur["DateHeure"].ToString();

                            // Créer un contrôle FormulaControl pour chaque entrée
                            var formulaControl = new FormulaControl {
                                Margin = new Thickness(0, 10, 0, 10),
                                Formula = text,
                                Width = double.NaN, // Auto width
                                Height = double.NaN // Auto height
                            };
                            stackPanel.Children.Add(formulaControl);
                        }
                    }
                }
            }
        }

        public static void DeleteAllFromDB() {
            using (var connexion = new SQLiteConnection($"Data Source={databasePath}")) {
                connexion.Open();
                string requete = "DELETE FROM Historique"; // Supprime toutes les entrées de la table Historique
                using (SQLiteCommand commande = new SQLiteCommand(requete, connexion)) {
                    commande.ExecuteNonQuery();
                }
            }
        }


    }
    public static class MatrixUtils {

        //mets à jour la grille matricielle dans l'affichage
        public static void UpdateMatrixGrid(UniformGrid MatrixGrid, int MatrixSize) {
            if (MatrixGrid != null) {
                MatrixGrid.Rows = MatrixSize;
                MatrixGrid.Columns = MatrixSize;
                MatrixGrid.Children.Clear();

                for (int i = 0; i < MatrixSize * MatrixSize; i++) {
                    TextBox textBox = new TextBox {
                        Width = 40,
                        Height = 20,
                        Margin = new Thickness(3),
                        TextAlignment = TextAlignment.Center
                    };
                    MatrixGrid.Children.Add(textBox);
                }
            }
        }


        //récupère les valeurs saisis par l'utilisateur et renvoie la liste 2d de la matrice
        public static List<List<float>> GetMatrixData(UniformGrid matrixGrid, int matrixSize) {
            List<List<float>> matrixData = new List<List<float>>();
            int index = 0;

            for (int i = 0; i < matrixSize; i++) {
                List<float> row = new List<float>();
                for (int j = 0; j < matrixSize; j++) {
                    TextBox textBox = (TextBox)matrixGrid.Children[index];
                    if (float.TryParse(textBox.Text, out float value)) {
                        row.Add(value);
                    }
                    else {
                        MessageBox.Show("Veuillez entrer des valeurs numériques valides.", "Erreur de saisie", MessageBoxButton.OK, MessageBoxImage.Error);
                        return null;  // retourne null s'il y a une erreur
                    }
                    index++;
                }
                matrixData.Add(row);
            }

            return matrixData;
        }
        //fonction qui servait initialement à juste afficher la matrice dans le résultat, maintenant utile pour transférer le latex de la matrice dans les autres fonctions
        public static string DisplayMatrix(Matrice matrix) {
            StringBuilder latexBuilder = new StringBuilder();
            latexBuilder.Append(@"\pmatrix{");

            for (int i = 0; i < matrix.n; i++) {
                for (int j = 0; j < matrix.n; j++) {
                    latexBuilder.Append(MathUtils.FormatAsFraction(matrix.mat[i][j]));

                    if (j < matrix.n - 1) {
                        latexBuilder.Append("&");
                    }
                }
                if (i < matrix.n - 1) {
                    latexBuilder.Append(@"\\");
                }
            }

            latexBuilder.Append("}");

            return(latexBuilder.ToString());
        }

        public static void DisplayInvertedMatrix(Matrice matrix, Matrice InvertedMatrix, ref FormulaControl latexMatrixDisplay) {


            // mets à jour la formule dans l'affichage
            latexMatrixDisplay.Formula = DisplayMatrix(InvertedMatrix);

            //crée un stringBuilder pour ajouter dans la bdd l'équation complète
            StringBuilder historyLatexBuilder = new StringBuilder();
            historyLatexBuilder.Append(DisplayMatrix(matrix));
            historyLatexBuilder.Append("^{-1} =  ");
            historyLatexBuilder.Append(latexMatrixDisplay.Formula);

            // mets le latex dans la bdd
            DataBaseUtils.SaveInDB(historyLatexBuilder.ToString());
        }

        public static void DisplayDetMatrix(Matrice matrix, float res, ref FormulaControl latexMatrixDisplay) {
            
            //ici pas besoin de passer par 2 builder vu qu'on affiche dirctement la formule complète dans l'affichage
            StringBuilder historyLatexBuilder = new StringBuilder();
            historyLatexBuilder.Append(@"det");
            historyLatexBuilder.Append(DisplayMatrix(matrix));
            historyLatexBuilder.Append(" = ");
            historyLatexBuilder.Append(MathUtils.FormatAsFraction(res));
            latexMatrixDisplay.Formula = historyLatexBuilder.ToString();

            // mets le latex dans la bdd
            DataBaseUtils.SaveInDB(historyLatexBuilder.ToString());
        }

        public static void DisplayCofMatrix(Matrice matrix, Matrice ComMatrix, ref FormulaControl latexMatrixDisplay) {
            // mets à jour la formule dans l'affichage
            latexMatrixDisplay.Formula = DisplayMatrix(ComMatrix);

            //crée un stringBuilder pour ajouter dans la bdd l'équation complète
            StringBuilder historyLatexBuilder = new StringBuilder();
            historyLatexBuilder.Append(@"cof");
            historyLatexBuilder.Append(DisplayMatrix(matrix));
            historyLatexBuilder.Append(" = ");
            historyLatexBuilder.Append(DisplayMatrix(ComMatrix));

            // mets le latex dans la bdd
            DataBaseUtils.SaveInDB(historyLatexBuilder.ToString());
        }

        public static void DisplayAdjMatrix(Matrice matrix, Matrice AdjMatrix, ref FormulaControl latexMatrixDisplay) {
            // mets à jour la formule dans l'affichage
            latexMatrixDisplay.Formula = DisplayMatrix(AdjMatrix);

            //crée un stringBuilder pour ajouter dans la bdd l'équation complète
            StringBuilder historyLatexBuilder = new StringBuilder();
            historyLatexBuilder.Append(@"adj");
            historyLatexBuilder.Append(DisplayMatrix(matrix));
            historyLatexBuilder.Append(" = ");
            historyLatexBuilder.Append(DisplayMatrix(AdjMatrix));

            // mets le latex dans la bdd
            DataBaseUtils.SaveInDB(historyLatexBuilder.ToString());
        }

    }

    //utilitaire mathématique
    public static class MathUtils {

        //fonction résoudre pour résoudre le système linéaire
        public static Vecteur resoudre(Matrice matr, Vecteur vect) {
            return(vect.Multiply(matr.Inverse()));
        }

        //fonction pour transformer un float en fraction 
        public static string FormatAsFraction(float value) {
            if (Math.Abs(value % 1) < 0.00001) { // Check if the value is a whole number
                return ((int)value).ToString();
            }

            // Vérifie si la valeur a seulement deux chiffres après la virgule
            string decimalString = value.ToString();
            if (decimalString.Contains(',') && decimalString.Split(',')[1].Length == 1) {
                return decimalString; // Retourne la valeur décimale si elle a 2 chiffres après la virgule ou moins
            }

            string sign = Math.Sign(value)<0 ? "-":"";
            value = Math.Abs(value);

            int numerator = 1;
            int denominator = 1;
            float error = float.MaxValue;

            for (int d = 1; d <= 1000; d++) { // limite la recherche pour éviter les boucles infinis
                int n = (int)Math.Round(value * d);
                float currentError = Math.Abs(value - (float)n / d);
                if (currentError < error) {
                    numerator = n;
                    denominator = d;
                    error = currentError;
                    if (error < 0.0000001) break; // marge d'erreur acceptable
                }
            }
            return $"{sign}\\frac{{{ numerator}}}{{{denominator}}}";
        }

    }
    public class Matrice {
        public List<List<float>> mat;  // La matrice est stockée comme une liste de listes
        public int n;  // Taille de la matrice (n * n)

        // Constructeur qui accepte directement une matrice sous forme de liste de listes
        public Matrice(List<List<float>> matrice) {
            n = matrice.Count;  // On suppose que c'est une matrice carrée
            mat = matrice;
        }

        public Matrice Add(Matrice other) {
            Matrice res = new Matrice(new List<List<float>>(mat));
            for (int i = 0; i < n; i++) {
                for (int j = 0; j < n; j++) {
                    res.mat[i][j] += other.mat[i][j];
                }
            }
            return res;
        }

        public Matrice Multiply(float other) {
            Matrice res = new Matrice(new List<List<float>>(mat));
            for (int i = 0; i < n; i++) {
                for (int j = 0; j < n; j++) {
                    res.mat[i][j] = other * mat[i][j];
                }
            }
            return res;
        }

        public bool estInversible() {
            return Detn() != 0;
        }
        public Matrice Trans() {
            List<List<float>> res = new List<List<float>>(n);

            // initialise le matrice résultat avec la taille appropprié
            for (int i = 0; i < n; i++) {
                res.Add(new List<float>(new float[n]));
            }

            for (int i = 0; i < n; i++) {
                for (int j = 0; j < n; j++) {
                    res[i][j] = mat[j][i];
                }
            }

            return new Matrice(res);
        }


        // Fonction pour obtenir une matrice sans la première ligne et sans une colonne donnée
        public Matrice Suppressor(int col) {
            List<List<float>> res = new List<List<float>>(n - 1);
            for (int i = 0; i < n - 1; i++)
                res.Add(new List<float>(new float[n - 1]));

            for (int i = 1; i < n; i++)  // On commence à 1 pour sauter la première ligne
            {
                int b = 0;  // Colonne de la nouvelle matrice
                for (int j = 0; j < n; j++) {
                    if (j == col) continue;  // On saute la colonne spécifiée
                    res[i - 1][b] = mat[i][j];  // Remplissage de la nouvelle matrice
                    b++;
                }
            }

            return new Matrice(res);  // On retourne une nouvelle matrice de taille (n-1)x(n-1)
        }

        // Fonction pour calculer le déterminant de la matrice
        public float Detn() {
            if (n == 2) {
                // Cas de base : matrice 2x2
           
                return mat[0][0] * mat[1][1] - mat[0][1] * mat[1][0];
            }

            float res = 0;
            for (int i = 0; i < n; i++) {
                // Calcul du déterminant avec récursiviter
                res += (float)Math.Pow(-1, i) * mat[0][i] * Suppressor(i).Detn();
            }
            return res;
        }

        //fonction similaire à supressor utilisé pour calculer le déterminant, sauf qu'au lieu de s'arrêter aux colonnes, on fait colonnes et lignes
        public Matrice CoSuppressor(int col, int lign) {
            List<List<float>> res = new List<List<float>>(n - 1);
            for (int i = 0; i < n - 1; i++)
                res.Add(new List<float>(new float[n - 1]));

            for (int i = 0, a = 0; i < n; i++, a++) {
                if (i == lign) {
                    a--;
                    continue;
                }
                for (int j = 0, b = 0; j < n; j++, b++) {
                    if (j == col) {
                        b--;
                        continue;
                    }
                    res[a][b] = mat[i][j];
                }
            }

            return new Matrice(res);
        }

        //focntion renvoie la comatrice
        public Matrice Com() {
            List<List<float>> res = new List<List<float>>(n);
            for (int i = 0; i < n; i++)
                res.Add(new List<float>(new float[n]));

            if (n == 2) {
                res[0][0] = mat[1][1];
                res[1][1] = mat[0][0];
                res[1][0] = -mat[0][1];
                res[0][1] = -mat[1][0];
                return new Matrice(res);
            }

            for (int lign = 0; lign < n; lign++) {
                for (int col = 0; col < n; col++) {
                    res[col][lign] = (float)Math.Pow(-1, lign + col) * CoSuppressor(lign, col).Detn();
                }
            }

            return new Matrice(res);
        }

        public Matrice Adj() {
            return Com().Trans();
        }


        public Matrice Inverse() {
            float k = 1 / Detn();
            return Adj().Multiply(k);
        }
    }

    public class Vecteur {
        public List<float> vec;
        public int n;

        public Vecteur(List<float> vect) {
            n = vect.Count;
            vec = vect;
        }


        public Vecteur Multiply(Matrice other) {
            Vecteur res = new Vecteur(new List<float>(vec));
            for (int i = 0; i < n; i++) {
                float val = 0;
                for (int j = 0; j < n; j++) {
                    val += vec[j] * other.mat[i][j];
                }
                res.vec[i] = val;
            }
            return res;
        }
    }

}
