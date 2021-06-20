using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace dll_reader
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        // выбор папки
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result != System.Windows.Forms.DialogResult.OK)
                {
                    MessageBox.Show("Пожалуйста, выберите папку!");
                    return;
                }

                string selectedPath = dialog.SelectedPath;

                path.Text = selectedPath;
            }
        }

        // прочитать выбранную папку
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path.Text);

            FileInfo[] dlls = directoryInfo.GetFiles("*.dll");

            treeView.Items.Clear();
            for (int i = 0; i < dlls.Length; i++)
            {
                Assembly a = Assembly.LoadFile(dlls[i].FullName);
                Type[] exportedClasses = a.GetExportedTypes();

                var outerTvItem = new TreeViewItem();
                outerTvItem.Header = dlls[i].Name;

                foreach (Type exportedClass in exportedClasses)
                {
                    var tvItem = new TreeViewItem();

                    tvItem.Header = exportedClass.Name;

                    if (exportedClass.IsClass)
                    {

                        var methods = exportedClass
                            .GetRuntimeMethods()
                            .Where(m =>
                                !typeof(object)
                                 .GetRuntimeMethods()
                                 .Select(me => me.Name)
                                 .Contains(m.Name)
                        );

                        foreach (MethodInfo mf in methods)
                        {
                            if (!(mf.IsPublic || mf.IsFamily)) continue;

                            var innerTvItem = new TreeViewItem();

                            innerTvItem.Header = "- " + mf.Name;
                            tvItem.Items.Add(innerTvItem);
                        }
                    }

                    outerTvItem.Items.Add(tvItem);
                }

                treeView.Items.Add(outerTvItem);
            }
        }
    }
}
