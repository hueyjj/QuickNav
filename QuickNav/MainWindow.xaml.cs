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
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        private StackPanel mainStackPanel;
        private StackPanel searchStackPanel;
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
            searchTextBox.TextChanged += OnSearchTextBoxTextChange;
            mainStackPanel.Children.Add(searchTextBox);
            searchStackPanel = new StackPanel();
            mainStackPanel.Children.Add(searchStackPanel);
            this.Content = mainStackPanel;


            UpdateProcessList();
            UpdateSearchResults();
        }

        private void OnSearchTextBoxKeyDown(object sender, KeyEventArgs e) {
            Console.Write(e.Key + " ");
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
                    // Highlight selection (tabIndex)
                    foreach (object child in searchStackPanel.Children) {
                        if (child is TextBlock) {
                            TextBlock childTextBlock = (child as TextBlock);
                            String childText = childTextBlock.Text;
                            if (childText.Equals(filteredProcesses[tabIndex].MainWindowTitle)) {
                                childTextBlock.Background = new SolidColorBrush(Color.FromRgb(0, 0, 255));
                            } else {
                                childTextBlock.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                            }
                        }
                    }
                    Console.WriteLine("Tab key down. tabIndex=" + tabIndex + " mainWindowTitle=" + filteredProcesses[tabIndex].MainWindowTitle);
                    break;
                default:
                    break;
            }
        }

        private void OnSearchTextBoxTextChange(object sender, EventArgs e) {
            UpdateFilteredProcessList();
            UpdateSearchResults();
        }

        private void UpdateProcessList() {
            Process[] allProcesses = Process.GetProcesses();
            foreach (Process p in allProcesses)
            {
                if (!p.MainWindowTitle.Equals(""))
                {
                    processes.Add(p);
                    filteredProcesses.Add(p);
                    // TextBlock processTextBlock = new TextBlock();
                    // processTextBlock.Text = p.ProcessName + " " + p.MainWindowTitle;
                    // processTextBlock.Text = p.MainWindowTitle;
                    // processTextBlock.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    // searchStackPanel.Children.Add(processTextBlock);
                    Console.WriteLine("p.MainWindowTitle: " + p.MainWindowTitle);
                    // if (IsIconic(p.MainWindowHandle)) {
                    //     Console.WriteLine("p.MainWindowHandle={0} p.MainWindowTitle{1}", p.MainWindowHandle, p.MainWindowTitle);
                    // }
                } 
                else 
                {
                    // Console.WriteLine("?p.MainWindowTitle:" + p.ProcessName + " " + p.MainWindowTitle);
                }
            }
        }

        private void UpdateFilteredProcessList() {
            filteredProcesses.Clear();

            String term = searchTextBox.Text.ToLower();
            foreach (Process p in processes) {
                if (p.MainWindowTitle.ToLower().Contains(term)) {
                    filteredProcesses.Add(p);
                }
            }
        }
        
        private void UpdateSearchResults() {
            searchStackPanel.Children.Clear();

            for (int i = 0; i < filteredProcesses.Count; i++) {
                Process p = filteredProcesses[i];
                TextBlock processTextBlock = new TextBlock();
                processTextBlock.Text = p.MainWindowTitle;
                if (i == 0) {
                    processTextBlock.Background = new SolidColorBrush(Color.FromRgb(0, 0, 255));
                } else {
                    processTextBlock.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                }
                searchStackPanel.Children.Add(processTextBlock);
            }
        }
    }
}
