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
using System.Windows.Shapes;

namespace Mine
{
    /// <summary>
    /// WinSuccess.xaml 的交互逻辑
    /// </summary>
    public partial class WinSuccess : Window
    {
        public WinSuccess()
        {
            InitializeComponent();
        }
        private List<int> Rank_List = new List<int>();
        private List<string> Player_List = new List<string>();
        private List<double> Time_List = new List<double>();
        private string Level,Player;
        private double Time;
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            Level = MainWindow.level;
            if (txtPlayer.Text == "")
            {
                Player = "";
            }
            else
            {
                Player = txtPlayer.Text;
            }
            Time = MainWindow.gameElapsedTime;
            ReadRecord();
            SortRecord();
            DeleteRecord();
            WriteRecord();
            WinHall winHall = new WinHall();
            winHall.ShowDialog();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //读入记录
        private void ReadRecord()
        {
            StreamReader sr;
            if (Level == "初级")
            {
                sr = File.OpenText(Properties.Resources.Records_Junior);
            }
            else if (Level == "中级")
            {
                sr = File.OpenText(path: Properties.Resources.Records_Middle);
            }
            else
            {
                sr = File.OpenText(path: Properties.Resources.Records_Senior);
            }
            string nextLine;
            int line = 0;
            while ((nextLine = sr.ReadLine()) != null)
            {
                string[] str_Array = nextLine.Split('\t');
                Player_List.Add(str_Array[0]);
                Rank_List.Add(int.Parse(str_Array[1]));
                Time_List.Add(int.Parse(str_Array[2]));
                line++;
            }
            sr.Close();
        }
        //进行排序
        private void SortRecord()
        {
            int index = Time_List.BinarySearch(Time);
            if (index < 0)
            {
                Player_List.Insert(~index, Player);
                Rank_List.Insert(~index, ~index + 1);
                Time_List.Insert(~index, Time);
            }
        }
        //删除记录
        private void DeleteRecord()
        {
            if (Player_List.Count > 10)
            {
                Player_List.RemoveRange(10, 1);
                Rank_List.RemoveRange(10, 1);
                Time_List.RemoveRange(10, 1);
            }
        }
        //写入数据
        private void WriteRecord()
        {
            StreamWriter sw;
            if (Level == "初级")
            {
                sw = File.CreateText(path: Properties.Resources.Records_Junior);
            }
            else if (Level == "中级")
            {
                sw = File.CreateText(path: Properties.Resources.Records_Middle);
            }
            else
            {
                sw = File.CreateText(path: Properties.Resources.Records_Senior);
            }
            foreach (var item in Time_List)
            {
                int index = Time_List.IndexOf(item);
                sw.WriteLine(Player_List.ElementAt(index) + "\t"
                    + Rank_List.ElementAt(index) + "\t"
                    + Time_List.ElementAt(index));
            }
            sw.Close();
        }
    }
}
