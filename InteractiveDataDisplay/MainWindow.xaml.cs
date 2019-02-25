using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace InteractiveDataDisplay
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private ObservableDataSource<Point> dataSource = new ObservableDataSource<Point>(); // 动态存储图表坐标点
        private PerformanceCounter performanceCounter = new PerformanceCounter(); //表示Windows NT的性能组件
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();//创建一个定时器
        private int currentSecond = 0;

        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void Window_Loaded_01(object sender, RoutedEventArgs e)
        {
            plotter.AddLineGraph(dataSource, Colors.Green, 2, "Percentage");
            //plotter.LegendVisible = false;
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += Timer_Tick_CPU;
            dispatcherTimer.IsEnabled = true;
            plotter.Viewport.FitToView();
        }
        
        private void Timer_Tick_CPU(object sender, EventArgs e)
        {
            performanceCounter.CategoryName = "Processor";
            performanceCounter.CounterName = "% Processor Time";
            performanceCounter.InstanceName = "_Total";

            double x = currentSecond;
            double y = performanceCounter.NextValue();

            Point point = new Point(x, y);
            dataSource.AppendAsync(base.Dispatcher, point);

            currentSecond++;
        }
    }
}
