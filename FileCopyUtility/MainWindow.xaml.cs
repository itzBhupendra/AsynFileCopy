using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileCopyUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Basic Validation before we can start copy 
        /// </summary>
        /// <returns></returns>
        bool isValidForm()
        {
            bool isValid = true;
            if (string.IsNullOrEmpty(txtSource.Text) || string.IsNullOrEmpty(txtSource.Text))
            {
                lblMesage.Content = "Please select source and destination folders!! ";
                isValid = false;
            }
            if (isCopyingFiles)
            {
                MessageBox.Show("Another copy is in progress");
                isValid = false;
            }
            return isValid;

        }
        bool isCopyingFiles = false;
        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isValidForm())
                {
                    isCopyingFiles = true;
                    FileCopyLibrary.ICopyFiles copyUtility = new FileCopyLibrary.CopyFiles();
                    string destination = txtDestination.Text;
                    string source = txtSource.Text;
                    Task.Run(() =>
                    {
                        copyUtility.CopyFolder(source, destination, (fileName, progressPercentage) =>
                           progressBar.Dispatcher.BeginInvoke(
                            new Action(() =>
                            {
                                lblMesage.Content = fileName;
                                progressBar.Value = progressPercentage;
                                lblPercent.Content = progressPercentage.ToString() + "%";
                            })));
                    }).GetAwaiter().OnCompleted(() =>
                            progressBar.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                isCopyingFiles = false;
                                lblMesage.Content = "Copy Completed";
                                progressBar.Value = 100;
                                lblPercent.Content = "100%";
                                MessageBox.Show("You have successfully copied the file !", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                                lblMesage.Content = "";
                                progressBar.Value = 0;
                                lblPercent.Content = "";
                            })));
                }

            }
            catch (Exception ex)
            {
                lblMesage.Content = $"Exception while processing your request { ex.Message }";
            }
            finally
            {
                
            }
        }

        /// <summary>
        /// Select Source folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowseSource_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog openFileDlg = new System.Windows.Forms.FolderBrowserDialog();
            var result = openFileDlg.ShowDialog();
            if (result.ToString() != string.Empty)
            {
                txtSource.Text = openFileDlg.SelectedPath;
            }
        }

        /// <summary>
        /// Select Destination folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowseDestination_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog openFileDlg = new System.Windows.Forms.FolderBrowserDialog();
            var result = openFileDlg.ShowDialog();
            if (result.ToString() != string.Empty)
            {
                txtDestination.Text = openFileDlg.SelectedPath;
            }
        }
    }
}
