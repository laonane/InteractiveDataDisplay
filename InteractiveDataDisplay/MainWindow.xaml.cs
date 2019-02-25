using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System;
using System.Collections;
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

        bool rock = false;//标志是否滚屏
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void Window_Loaded_01(object sender, RoutedEventArgs e)
        {
            plotter.AddLineGraph(dataSource, Colors.Green, 3, "Percentage");
            plotter.LegendVisible = true;
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += Timer_Tick_CPU;
            dispatcherTimer.IsEnabled = true;
            plotter.Viewport.FitToView();
        }

        int xaxis = 0;
        int yaxis = 0;
        int group = 20;//默认组距

        Queue q = new Queue();
        private void Timer_Tick_CPU(object sender, EventArgs e)
        {
            performanceCounter.CategoryName = "Processor";
            performanceCounter.CounterName = "% Processor Time";
            performanceCounter.InstanceName = "_Total";

            double x = currentSecond;
            double y = performanceCounter.NextValue();

            Point point = new Point(x, y);
            dataSource.AppendAsync(base.Dispatcher, point);

            if (rock)
            {
                if (q.Count < group)
                {
                    q.Enqueue((int)y);//入队
                    yaxis = 0;
                    foreach (int c in q)
                        if (c > yaxis)
                            yaxis = c;
                }
                else
                {
                    q.Dequeue();//出队
                    q.Enqueue((int)y);//入队
                    yaxis = 0;
                    foreach (int c in q)
                        if (c > yaxis)
                            yaxis = c;
                }

                if (currentSecond - group > 0)
                    xaxis = currentSecond - group;
                else
                    xaxis = 0;

                Debug.Write("{0}\n", yaxis.ToString());
                plotter.Viewport.Visible = new System.Windows.Rect(xaxis, 0, group, yaxis);//主要注意这里一行
            }

            currentSecond++;
        }

        private void Btn_Rock_click_01(object sender, EventArgs e)
        {
            if (rock)
            {
                rock = false;
            }
            else
            {
                rock = true;
            }
        }
    }
}
