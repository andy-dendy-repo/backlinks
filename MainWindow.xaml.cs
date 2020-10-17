using Backlinks_LE.Models;
using Backlinks_LE.Services;
using Backlinks_LE.Services.Save;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
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

namespace Backlinks_LE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SearchManager _manager;

        private Settings _settings = Settings.Load();
        public MainWindow()
        {
            InitializeComponent();

            _settings.ConnectionString = "Server=127.0.0.1;port=3306;database=backlinks;uid=root;password=1111";

            if (_settings.ExtractMethod == DataMethods.File)
                rb_text.IsChecked = true;
            else
                rb_spreadsheet.IsChecked = true;

            if (_settings.SaveMethod == DataMethods.File)
                rb_save_text.IsChecked = true;
            else
                if (_settings.SaveMethod == DataMethods.GoogleDocs)
                rb_save_spreadsheet.IsChecked = true;
            else
                rb_save_excel.IsChecked = true;

            tb_input.Text=_settings.OpenDataString;

            tb_output.Text=_settings.SaveDataString;

            tb_blin.Text=_settings.BacklinkIndex.ToString();

            tb_dlin.Text=_settings.DomainIndex.ToString();

            tb_del.Text=_settings.Delimeter;

            _settings.SingleDomname = null;

            tb_pht.Text=_settings.PlainHttpThreads.ToString();

            tb_wdt.Text=_settings.WebDriverThreads.ToString();

            StartServ();
        }
        private void StartServ()
        {
            ServiceController sc = new ServiceController();
            sc.ServiceName = "MySQL80";

            if (sc.Status == ServiceControllerStatus.Running ||
                sc.Status == ServiceControllerStatus.StartPending)
            {
                Console.WriteLine("Service is already running");
            }
            else
            {
                try
                {
                    Console.Write("Start pending... ");
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 10));

                    if (sc.Status == ServiceControllerStatus.Running)
                    {
                        Console.WriteLine("Service started successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Service not started.");
                        Console.WriteLine("  Current State: {0}", sc.Status.ToString("f"));
                    }
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("Could not start the service.");
                }
            }
        }
        public void Info(string info) 
        {
            App.Current.Dispatcher.Invoke(()=> {
                lb_http_info_found.Content = info;
            });
        }
        private async void bt_start_Click(object sender, RoutedEventArgs e)
        {
            if (rb_text.IsChecked.Value)
            {
                string newdir = AppDomain.CurrentDomain.BaseDirectory + "/" + DateTime.Now.ToString("ddMMyyyyHHmmss");
                Directory.CreateDirectory(newdir);
                File.Copy(tb_input.Text, newdir + "source.txt");


                _settings.OpenDataString = newdir + "/source.txt";
            }

            _settings.Save();
             await _manager.Perform();
        }

        private async void bt_load_Click(object sender, RoutedEventArgs e)
        {
            _manager = new SearchManager(_settings);

            _settings.OpenDataString = tb_input.Text;

            if (rb_text.IsChecked.Value)
                _settings.ExtractMethod = DataMethods.File;
            if (rb_spreadsheet.IsChecked.Value)
                _settings.ExtractMethod = DataMethods.GoogleDocs;

            _settings.Save();

            lb_task.Content= "Task: "+await _manager.Open(Info);
        }

        private async void bt_save_Click(object sender, RoutedEventArgs e)
        {
            ISave save = null;

            switch (_settings.ExtractMethod)
            {
                case DataMethods.File:
                    {
                        save = new FileSave(
                            new DataBacklinkRowService(_settings.ConnectionString),
                            new DataListedInfoService(_settings.ConnectionString),
                            new DataTagAService(_settings.ConnectionString),
                            _settings);
                        break;
                    }
                case DataMethods.GoogleDocs:
                    {
                        save = new SpreadSheetSave(
                            new DataBacklinkRowService(_settings.ConnectionString),
                            new DataListedInfoService(_settings.ConnectionString),
                            new DataTagAService(_settings.ConnectionString),
                            _settings); 
                        break;
                    }
                case DataMethods.Excell:
                    {
                        save = new ExcelSave(
                            new DataBacklinkRowService(_settings.ConnectionString),
                            new DataListedInfoService(_settings.ConnectionString),
                            new DataTagAService(_settings.ConnectionString),
                            _settings);
                        break;
                    }
            }

            await save.Save();
        }

        private async void bt_perform_Click(object sender, RoutedEventArgs e)
        {
            if (rb_text.IsChecked.Value)
            {
                string newdir = AppDomain.CurrentDomain.BaseDirectory + "/" + DateTime.Now.ToString("ddMMyyyyHHmmss");
                Directory.CreateDirectory(newdir);
                File.Copy(tb_input.Text, newdir + "source.txt");
                _settings.OpenDataString = newdir + "/source.txt";
                _settings.ExtractMethod = DataMethods.File;
            }
            if (rb_spreadsheet.IsChecked.Value)
                _settings.ExtractMethod = DataMethods.GoogleDocs;

            if (rb_save_text.IsChecked.Value)
                _settings.SaveMethod = DataMethods.File;
            if (rb_save_spreadsheet.IsChecked.Value)
                _settings.SaveMethod = DataMethods.GoogleDocs;
            if (rb_save_excel.IsChecked.Value)
                _settings.SaveMethod = DataMethods.Excell;

            _settings.OpenDataString = tb_input.Text;

            _settings.SaveDataString = tb_output.Text;

            _settings.BacklinkIndex = int.Parse(tb_blin.Text);

            _settings.DomainIndex = int.Parse(tb_dlin.Text);

            _settings.Delimeter = tb_del.Text;

            if (!string.IsNullOrEmpty(tb_dom.Text))
                _settings.SingleDomname = tb_dom.Text;
            else
                _settings.SingleDomname = null;

            _settings.PlainHttpThreads = int.Parse(tb_pht.Text);

            _settings.WebDriverThreads = int.Parse(tb_wdt.Text);

            _settings.Save();

            SearchManager manager = new SearchManager(_settings);

            lb_task.Content = "Task: "+await manager.Open(Info);

            await manager.Perform();

            await manager.Close();
        }
    }
}
