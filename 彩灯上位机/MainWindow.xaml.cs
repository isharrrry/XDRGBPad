using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using System.IO.Ports;
using System.Management;

namespace 彩灯上位机
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        SerialPort com = new SerialPort();
        string[] btStatus = new string[] { "Open", "Close" };

        public MainWindow()
        {
            InitializeComponent();
            btNew_Click(null, null);
            changeColorUseH(0);


        }
        string[] DevName;
        string[] DeviceID;

        private void btNew_Click(object sender, RoutedEventArgs e)
        {
            lsCom.Items.Clear();
            //通过WMI获取COM端口
            //DevName = MulGetHardwareInfo(HardwareEnum.Win32_SerialPort, "Name");
            //DeviceID = MulGetHardwareInfo(HardwareEnum.Win32_SerialPort, "DeviceID");//SerialPort.GetPortNames()
            foreach (string item in SerialPort.GetPortNames())
            {
                lsCom.Items.Add(item);
            }
            lsCom.SelectedIndex = 0;

            int[] boud = new int[] { 9600, 38400, 115200 };
            lsBoud.Items.Clear();
            foreach (int item in boud)
            {
                lsBoud.Items.Add(item);
            }
            lsBoud.SelectedIndex = 2;

        }

        private void btOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (btOpen.Content.ToString() == btStatus[0])
                {
                    //open
                    btOpen.Content = btStatus[1];
                    com = new SerialPort(lsCom.SelectedItem.ToString(), int.Parse(lsBoud.SelectedItem.ToString()));//lsCom.SelectedItem.ToString()  DeviceID[lsCom.SelectedIndex]
                    //com.DtrEnable = true;
                    com.Open();
                    com.DataReceived += com_DataReceived;
                }
                else
                {
                    btOpen.Content = btStatus[0];
                    com.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Select serial post that could not be open！\n", ex.Message.ToString());
            }
        }

        void com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //int r = com.ReadBufferSize;
            int dateLengh = com.BytesToRead;


            byte[] DataTemp = new byte[dateLengh];
            for (int i = 0; i < dateLengh; i++)
            {
                DataTemp[i] = (byte)com.ReadByte();

            }

            string word = Encoding.UTF8.GetString(DataTemp, 0, DataTemp.Length);
            this.Dispatcher.Invoke(new Action(() =>
            {
                txtLog.Text += word;
            }));
            //byte[] dataCache = new byte[2];
            //for (int i = 0; i < dateLengh; i++)
            //{

            //    if (dataCache[0] != 0)
            //    {
            //        dataCache[1] = DataTemp[i];
            //        //转
            //        string word = Encoding.Default.GetString(dataCache, 0, dataCache.Length);
            //        this.Dispatcher.Invoke(new Action(() =>
            //        {
            //            txtLog.Text += word;
            //        }));
            //        //richTextBox1.Text += ']';
            //        dataCache[0] = 0;
            //    }
            //    else
            //    {
            //        dataCache[0] = DataTemp[i];
            //        //this.Dispatcher.Invoke(new Action(() =>
            //        //{
            //        //    txtLog.Text += '`';
            //        //}));
            //    }
            //}

            //this.Dispatcher.Invoke(new Action(() =>
            //    {
            //        // txtLog.Text += com.ReadLine();
            //        //for (int i = 0; i < indata.Length; i++)
            //        //{
            //        //    char ch = (char)indata[i];
            //        //    txtLog.Text += (ch >> 4).ToString("X");
            //        //    txtLog.Text += (ch & 0x0f).ToString("X");
            //        //    txtLog.Text += ' ';
            //        //}
            //    }));
        }

        private void btWrite_Click(object sender, RoutedEventArgs e)
        {
            if (!com.IsOpen)
            {

                MessageBox.Show("Please connect device firstly");
                return;
            }
            com.Write(txtCmd.Text);
            //com.Write(Encoding.Default.GetBytes(txtCmd.Text));
            txtLog.Text = "";
        }

        private void btRead_Click(object sender, RoutedEventArgs e)
        {
            sendCmd("run=0");
        }
        private void sendCmd(string cmd)
        {
            if (!com.IsOpen)
                return;
            com.Write(cmd + "\r\n");
            this.Dispatcher.Invoke(new Action(() =>
            {
                txtLog.Text += cmd + "\r\n";
                txtLog.ScrollToEnd();
            }));
        }
        private void btSet_Click(object sender, RoutedEventArgs e)
        {

            cb1.IsChecked = true;
            cb2.IsChecked = true;
            cb3.IsChecked = true;
            cb4.IsChecked = true;
            cb5.IsChecked = true;
            cb6.IsChecked = true;
            cb7.IsChecked = true;
            cb8.IsChecked = true;
            cb9.IsChecked = true;
            cb10.IsChecked = true;
            cb11.IsChecked = true;
            cb12.IsChecked = true;
            cb13.IsChecked = true;
            cb14.IsChecked = true;
            cb15.IsChecked = true;
            cb16.IsChecked = true;
            sendCmd("on=0");
        }

        private void btSetVal_Click(object sender, RoutedEventArgs e)
        {
            //sendCmd("run=" + txtAddr.Text);
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            txtLog.Text = "";
            txtCmd.Text = "";
        }


        private void BtSet2_Click(object sender, RoutedEventArgs e)
        {
            cb1.IsChecked = false;
            cb2.IsChecked = false;
            cb3.IsChecked = false;
            cb4.IsChecked = false;
            cb5.IsChecked = false;
            cb6.IsChecked = false;
            cb7.IsChecked = false;
            cb8.IsChecked = false;
            cb9.IsChecked = false;
            cb10.IsChecked = false;
            cb11.IsChecked = false;
            cb12.IsChecked = false;
            cb13.IsChecked = false;
            cb14.IsChecked = false;
            cb15.IsChecked = false;
            cb16.IsChecked = false;
            sendCmd("off=0");
        }


        private void trunLight(bool isOn, String no)
        {
            if (isOn)
                sendCmd("on=" + no);
            else
                sendCmd("off=" + no);
        }
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            trunLight((bool)((System.Windows.Controls.Primitives.ToggleButton)sender).IsChecked, ((System.Windows.Controls.ContentControl)sender).Content.ToString());

        }

        private void CheckBox_Click_1(object sender, RoutedEventArgs e)
        {

            trunLight((bool)((System.Windows.Controls.Primitives.ToggleButton)sender).IsChecked, ((System.Windows.Controls.ContentControl)sender).Content.ToString());

        }

        private void CheckBox_Click_2(object sender, RoutedEventArgs e)
        {
            trunLight((bool)((System.Windows.Controls.Primitives.ToggleButton)sender).IsChecked, ((System.Windows.Controls.ContentControl)sender).Content.ToString());

        }

        private void CheckBox_Click_3(object sender, RoutedEventArgs e)
        {
            trunLight((bool)((System.Windows.Controls.Primitives.ToggleButton)sender).IsChecked, ((System.Windows.Controls.ContentControl)sender).Content.ToString());

        }

        private void CheckBox_Click_4(object sender, RoutedEventArgs e)
        {
            trunLight((bool)((System.Windows.Controls.Primitives.ToggleButton)sender).IsChecked, ((System.Windows.Controls.ContentControl)sender).Content.ToString());

        }

        private void CheckBox_Click_5(object sender, RoutedEventArgs e)
        {
            trunLight((bool)((System.Windows.Controls.Primitives.ToggleButton)sender).IsChecked, ((System.Windows.Controls.ContentControl)sender).Content.ToString());

        }

        private void CheckBox_Click_6(object sender, RoutedEventArgs e)
        {
            trunLight((bool)((System.Windows.Controls.Primitives.ToggleButton)sender).IsChecked, ((System.Windows.Controls.ContentControl)sender).Content.ToString());

        }

        private void CheckBox_Click_7(object sender, RoutedEventArgs e)
        {
            trunLight((bool)((System.Windows.Controls.Primitives.ToggleButton)sender).IsChecked, ((System.Windows.Controls.ContentControl)sender).Content.ToString());

        }

        private void CheckBox_Click_8(object sender, RoutedEventArgs e)
        {
            trunLight((bool)((System.Windows.Controls.Primitives.ToggleButton)sender).IsChecked, ((System.Windows.Controls.ContentControl)sender).Content.ToString());

        }

        private void CheckBox_Click_9(object sender, RoutedEventArgs e)
        {
            trunLight((bool)((System.Windows.Controls.Primitives.ToggleButton)sender).IsChecked, ((System.Windows.Controls.ContentControl)sender).Content.ToString());

        }

        private void CheckBox_Click_10(object sender, RoutedEventArgs e)
        {
            trunLight((bool)((System.Windows.Controls.Primitives.ToggleButton)sender).IsChecked, ((System.Windows.Controls.ContentControl)sender).Content.ToString());

        }

        private void CheckBox_Click_11(object sender, RoutedEventArgs e)
        {
            trunLight((bool)((System.Windows.Controls.Primitives.ToggleButton)sender).IsChecked, ((System.Windows.Controls.ContentControl)sender).Content.ToString());

        }

        private void CheckBox_Click_12(object sender, RoutedEventArgs e)
        {
            trunLight((bool)((System.Windows.Controls.Primitives.ToggleButton)sender).IsChecked, ((System.Windows.Controls.ContentControl)sender).Content.ToString());

        }

        private void CheckBox_Click_13(object sender, RoutedEventArgs e)
        {
            trunLight((bool)((System.Windows.Controls.Primitives.ToggleButton)sender).IsChecked, ((System.Windows.Controls.ContentControl)sender).Content.ToString());

        }

        private void CheckBox_Click_14(object sender, RoutedEventArgs e)
        {
            trunLight((bool)((System.Windows.Controls.Primitives.ToggleButton)sender).IsChecked, ((System.Windows.Controls.ContentControl)sender).Content.ToString());

        }

        private void CheckBox_Click_15(object sender, RoutedEventArgs e)
        {
            trunLight((bool)((System.Windows.Controls.Primitives.ToggleButton)sender).IsChecked, ((System.Windows.Controls.ContentControl)sender).Content.ToString());

        }



        private void BtSet3_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btReadBuf_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void SdrH_LostMouseCapture(object sender, MouseEventArgs e)
        {
            int val = (int)((System.Windows.Controls.Primitives.RangeBase)sender).Value;
            sendCmd("hsbh=" + val.ToString());

            color_HSB_H = (float)val / 100;
            showColor();
        }
        float color_HSB_H = 0;
        float color_HSB_S = 1;
        float color_HSB_B = 1;

        private void changeColorUseH(float val)
        {
            //Color color =(Color) ColorConverter.ConvertFromString("#f00");
            Color color = RgbColorFromHSB(val / 100, 1, 1);
            cv_show.Background = new SolidColorBrush(color);
        }
        private void showColor()
        {
            //Color color =(Color) ColorConverter.ConvertFromString("#f00");
            Color color = RgbColorFromHSB(color_HSB_H, color_HSB_S, color_HSB_B);
            cv_show.Background = new SolidColorBrush(color);
        }

        private Color RgbColorFromHSB(float colorH, float colorS, float colorB)
        {
            float r;
            float g;
            float b;

            float h = colorH;
            float s = colorS;
            float v = colorB;

            if (colorS == 0.0f)
            {
                r = g = b = v; // achromatic or black
            }
            else
            {
                if (h < 0.0f)
                {
                    h += 1.0f;
                }
                else if (h >= 1.0f)
                {
                    h -= 1.0f;
                }
                h *= 6.0f;
                int i = (int)h;
                float f = h - i;
                float q = v * (1.0f - s * f);
                float p = v * (1.0f - s);
                float t = v * (1.0f - s * (1.0f - f));
                switch (i)
                {
                    case 0:
                        r = v;
                        g = t;
                        b = p;
                        break;
                    case 1:
                        r = q;
                        g = v;
                        b = p;
                        break;
                    case 2:
                        r = p;
                        g = v;
                        b = t;
                        break;
                    case 3:
                        r = p;
                        g = q;
                        b = v;
                        break;
                    case 4:
                        r = t;
                        g = p;
                        b = v;
                        break;
                    default:
                        r = v;
                        g = p;
                        b = q;
                        break;
                }
            }

            byte R = (byte)(r * 255.0f);
            byte G = (byte)(g * 255.0f);
            byte B = (byte)(b * 255.0f);
            return Color.FromRgb(R, G, B);
        }

        private void SdrS_LostMouseCapture(object sender, MouseEventArgs e)
        {
            int val = (int)((System.Windows.Controls.Primitives.RangeBase)sender).Value;
            sendCmd("hsbs=" + val.ToString());

            color_HSB_S = (float)val / 100;
            showColor();
        }

        private void SdrB_LostMouseCapture(object sender, MouseEventArgs e)
        {
            int val = (int)((System.Windows.Controls.Primitives.RangeBase)sender).Value;
            sendCmd("hsbb=" + val.ToString());
        }
        int runCount = 0;
        int runMin = -1;
        int runMax = 3;
        private void BtSetVal2_Click(object sender, RoutedEventArgs e)
        {
            runCount++;
            if (runCount == 0)
                runCount++;
            if (runCount > runMax)
                runCount = runMin;
            sendCmd("run=" + runCount);
        }

        private void BtSetVal3_Click(object sender, RoutedEventArgs e)
        {
            runCount--;
            if (runCount == 0)
                runCount--;
            if (runCount < runMin)
                runCount = runMax;
            sendCmd("run=" + runCount);
        }


        /// <summary>
        /// 枚举win32 api
        /// </summary>
        public enum HardwareEnum
        {
            // 硬件
            Win32_Processor, // CPU 处理器
            Win32_PhysicalMemory, // 物理内存条
            Win32_Keyboard, // 键盘
            Win32_PointingDevice, // 点输入设备，包括鼠标。
            Win32_FloppyDrive, // 软盘驱动器
            Win32_DiskDrive, // 硬盘驱动器
            Win32_CDROMDrive, // 光盘驱动器
            Win32_BaseBoard, // 主板
            Win32_BIOS, // BIOS 芯片
            Win32_ParallelPort, // 并口
            Win32_SerialPort, // 串口
            Win32_SerialPortConfiguration, // 串口配置
            Win32_SoundDevice, // 多媒体设置，一般指声卡。
            Win32_SystemSlot, // 主板插槽 (ISA & PCI & AGP)
            Win32_USBController, // USB 控制器
            Win32_NetworkAdapter, // 网络适配器
            Win32_NetworkAdapterConfiguration, // 网络适配器设置
            Win32_Printer, // 打印机
            Win32_PrinterConfiguration, // 打印机设置
            Win32_PrintJob, // 打印机任务
            Win32_TCPIPPrinterPort, // 打印机端口
            Win32_POTSModem, // MODEM
            Win32_POTSModemToSerialPort, // MODEM 端口
            Win32_DesktopMonitor, // 显示器
            Win32_DisplayConfiguration, // 显卡
            Win32_DisplayControllerConfiguration, // 显卡设置
            Win32_VideoController, // 显卡细节。
            Win32_VideoSettings, // 显卡支持的显示模式。

            // 操作系统
            Win32_TimeZone, // 时区
            Win32_SystemDriver, // 驱动程序
            Win32_DiskPartition, // 磁盘分区
            Win32_LogicalDisk, // 逻辑磁盘
            Win32_LogicalDiskToPartition, // 逻辑磁盘所在分区及始末位置。
            Win32_LogicalMemoryConfiguration, // 逻辑内存配置
            Win32_PageFile, // 系统页文件信息
            Win32_PageFileSetting, // 页文件设置
            Win32_BootConfiguration, // 系统启动配置
            Win32_ComputerSystem, // 计算机信息简要
            Win32_OperatingSystem, // 操作系统信息
            Win32_StartupCommand, // 系统自动启动程序
            Win32_Service, // 系统安装的服务
            Win32_Group, // 系统管理组
            Win32_GroupUser, // 系统组帐号
            Win32_UserAccount, // 用户帐号
            Win32_Process, // 系统进程
            Win32_Thread, // 系统线程
            Win32_Share, // 共享
            Win32_NetworkClient, // 已安装的网络客户端
            Win32_NetworkProtocol, // 已安装的网络协议
            Win32_PnPEntity,//all device
        }
        /// <summary>
        /// WMI取硬件信息
        /// </summary>
        /// <param name="hardType"></param>
        /// <param name="propKey"></param>
        /// <returns></returns>
        public static string[] MulGetHardwareInfo(HardwareEnum hardType, string propKey)
        {

            List<string> strs = new List<string>();
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from " + hardType))
                {
                    var hardInfos = searcher.Get();
                    foreach (var hardInfo in hardInfos)
                    {
                        if (hardInfo.Properties[propKey].Value.ToString().Contains("COM"))
                        {
                            //foreach (var item in hardInfo.Properties)
                            //{
                            //    try
                            //    {

                            //        String n = item.Name.ToString();
                            //        Console.WriteLine(n);
                            //        Console.WriteLine(item.Value.ToString());
                            //    }
                            //    catch (Exception e)
                            //    {
                            //    }
                            //}
                            strs.Add(hardInfo.Properties[propKey].Value.ToString());
                        }

                    }
                    searcher.Dispose();
                }
                return strs.ToArray();
            }
            catch
            {
                return null;
            }
            finally
            { strs = null; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("\nPlease contact us if that has translation errors \n——XD Artistic Group  xiudi@outlook.com\n\n\ndesigned by harrrry ", "XD Artistic Group");
        }
    }
}
