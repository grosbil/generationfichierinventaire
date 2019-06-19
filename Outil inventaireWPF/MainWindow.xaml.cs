using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows ;
using System.IO;
using WinForms = System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Outil_inventaireWPF
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //string path = "C:\\inventaireSage\\";               // dev
        string path = "M:\\CONFIDENTIEL CSI\\Sandrine\\Traitement Inventaire\\";               // prod
        public MainWindow()
        {
            InitializeComponent();
            Lb_Result.Content = "";
            Lb_inventaireSage.Content = "";
            Lb_inventaireDepot.Content = "";
            Bt_generate.Visibility = Visibility.Hidden;
            Bt_openFolder.Visibility = Visibility.Hidden;

        }

        private void Bt_fSage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            WinForms.OpenFileDialog fileDialog = new WinForms.OpenFileDialog();
            fileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            WinForms.DialogResult result = fileDialog.ShowDialog();
            if (result == WinForms.DialogResult.OK)
            {
                string sPath = fileDialog.FileName;
                //inventaireSage.Text = sPath;
                Lb_inventaireSage.Content = sPath;
                Bt_generate_activate();
            }
        }

        private void Bt_fDepot_Click(object sender, RoutedEventArgs e)
        {
            WinForms.OpenFileDialog fileDialog = new WinForms.OpenFileDialog();
            fileDialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            WinForms.DialogResult result = fileDialog.ShowDialog();
            if (result == WinForms.DialogResult.OK)
            {
                string sPath = fileDialog.FileName;
                Lb_inventaireDepot.Content =  sPath;
                Bt_generate_activate();
            }

        }

        private void Bt_generate_activate()
        {
            if (Lb_inventaireDepot.Content.ToString() != "" & Lb_inventaireSage.Content.ToString() != "")
            { 
                Bt_generate.Visibility = Visibility.Visible;
            } 
        }


        private void Bt_generate_Click(object sender, RoutedEventArgs e)
        { 
          
        string PathExport = Lb_inventaireSage.Content.ToString();   // Inventaire issu de SAGE
        string PathImport = Lb_inventaireDepot.Content.ToString();   // Inventaire issu du dépot
        string PathSortie = path + "SortieInventaire.txt"; // fichier à intégrer dans SAGE
            try
            {
                string[] tabinventaire = File.ReadAllLines(PathExport, Encoding.UTF8);
        string[] tabNewinventaire = File.ReadAllLines(PathImport, Encoding.UTF8);


        string[][] newInventaire = new string[tabNewinventaire.Length][];
        string[][] inventaire = new string[tabinventaire.Length][];
        string[] inventaireFinal = new string[tabinventaire.Length];   // tableau contenant toutes les valeurs à jour de l'inventaire


        List<string> newlines = new List<string>();

                if (tabinventaire[1] != "#VER 6" || tabinventaire[3] != "#INV")
                {
                    MessageBox.Show("La version du fichier d'export ne semble pas valide. Appuyez sur une touche pour quitter.");
                    return;

                }



                for (int i = 0; i<tabinventaire.Length; i++)
                {
                    inventaire[i] = tabinventaire[i].Split('\t');
}

                for (int i = 0; i<tabNewinventaire.Length; i++)
                {
                    newInventaire[i] = tabNewinventaire[i].Split(';');
                }

                    for (int i = 0; i<newInventaire.Length; i++)
                {
                    for (int j = 0; j<inventaire.Length; j++)
                    {
                        if (inventaire[j][0] == newInventaire[i][0] && inventaire[j][1] == newInventaire[i][1] && inventaire[j][2] == newInventaire[i][2])
                        {
                            inventaire[j][4] = newInventaire[i][3];
                            inventaire[j][5] = newInventaire[i][4];
                        }
                    }
                }

                for (int i = 0; i<inventaire.Length; i++)
                {
                    for (int j = 0; j<inventaire[i].Length; j++)
                    {
                        inventaireFinal[i] += inventaire[i][j];

                        if (i > 5 && j<inventaire[i].Length - 1)
                        {
                            inventaireFinal[i] += '\t';
                        }
                    }
                    newlines.Add(inventaireFinal[i]);

                }
                File.WriteAllLines(@PathSortie, newlines);
                Lb_Result.Content = " L'export du fichier est terminé. Le fichier est disponible ici : \n " + @PathSortie;
                Bt_openFolder.Visibility = Visibility.Visible;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void Bt_openFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", @path);
        }
    }
}
