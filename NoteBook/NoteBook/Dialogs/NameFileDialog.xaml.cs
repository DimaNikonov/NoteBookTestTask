using NoteBook.Models;
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
using System.Windows.Shapes;

namespace NoteBook.Dialogs
{
    /// <summary>
    /// Interaction logic for NameFileDialog.xaml
    /// </summary>
    public partial class NameFileDialog : Window
    {
        public NameFileDialog()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
        }

        private void FialogButtonOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public string FileName => string.Format($"{this.InputBox.Text}{Constants.ExtentionTxt}");
    }
}
