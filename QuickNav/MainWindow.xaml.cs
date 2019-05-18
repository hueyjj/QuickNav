using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace QuickNav
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern void SwitchToThisWindow(IntPtr hWnd);

        private StackPanel mainStackPanel;
        private TextBox searchTextBox;

        private List<Process> processes = new List<Process>();
        private List<Process> filteredProcesses = new List<Process>();

        private int tabIndex = 0;

        public MainWindow()
        {
            InitializeComponent();

            this.Width = 1000;

            mainStackPanel = new StackPanel();
            searchTextBox = new TextBox();
            Thickness searchTextBoxThickness = new Thickness();
            searchTextBoxThickness.Left = 10;
            searchTextBoxThickness.Right = 10;
            searchTextBox.Margin = searchTextBoxThickness;
            searchTextBox.KeyDown += OnSearchTextBoxKeyDown;
            mainStackPanel.Children.Add(searchTextBox);
            this.Content = mainStackPanel;


            UpdateProcessList();
        }

        private void OnSearchTextBoxKeyDown(object sender, KeyEventArgs e) {
            switch(e.Key) 
            {
                case Key.Return:
                    SwitchToThisWindow(filteredProcesses[tabIndex].MainWindowHandle);
                    Console.WriteLine("SwitchToWindow: " + filteredProcesses[tabIndex].MainWindowTitle);
                    Console.WriteLine("Return key down");
                    break;
                case Key.Tab:
                    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)) 
                    {
                        tabIndex = (tabIndex - 1 + filteredProcesses.Count) % filteredProcesses.Count;
                    }
                    else
                    {
                        tabIndex = (tabIndex + 1) % filteredProcesses.Count;
                    }
                    Console.WriteLine("Tab key down. tabIndex=" + tabIndex + " mainWindowTitle=" + filteredProcesses[tabIndex].MainWindowTitle);
                    break;
                default:
                    break;
            }
        }

        private void UpdateProcessList() {
            Process[] allProcesses = Process.GetProcesses();
            foreach (Process p in allProcesses)
            {
                if (p.MainWindowTitle != "")
                {
                    processes.Add(p);
                    filteredProcesses.Add(p);
                    //SetForegroundWindow(p.MainWindowHandle);
                    //SwitchToThisWindow(p.MainWindowHandle);
                    TextBlock processTextBlock = new TextBlock();
                    processTextBlock.Text = p.MainWindowTitle;
                    processTextBlock.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    mainStackPanel.Children.Add(processTextBlock);
                    Console.WriteLine("p.MainWindowTitle: " + p.MainWindowTitle);
                }
            }
        }
    }
}
