using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wox.Plugin.Todos
{
    /// <summary>
    /// Interaction logic for FilePathSetting.xaml
    /// </summary>
    public partial class FilePathSetting
    {
        private Settings _setting;

        public FilePathSetting(Settings setting)
        {
            InitializeComponent();
            _setting = setting;
            Directory.Text = _setting.FolderPath;
        }

        private void ChooseBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                var path = dialog.SelectedPath;
                Directory.Text = path;
                _setting.FolderPath = path;
            }
        }
    }
}
