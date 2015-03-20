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

using RotaMaker.ViewModels;

namespace RotaMaker.Views
{
    /// <summary>
    /// Interaction logic for AddNurseDialog.xaml
    /// </summary>
    public partial class AddNurseDialog : Window
    {
        public AddNurseDialog()
        {
            InitializeComponent();
        }
        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        public AddNurseDialog(NurseViewModel nurseVM)
        {
            InitializeComponent();
            DataContext = nurseVM;
        }
    }
}
