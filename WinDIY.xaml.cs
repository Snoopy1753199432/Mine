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

namespace Mine
{
    /// <summary>
    /// WinDIY.xaml 的交互逻辑
    /// </summary>
    public partial class WinDIY : Window
    {
        public WinDIY()
        {
            InitializeComponent();
        }
        private void BtnDIYOK_Click(object sender, RoutedEventArgs e)
        {
            if (IsInputResonable())
            {
                MainWindow.blockNRow = int.Parse(txtRow.Text);
                MainWindow.blockNCol = int.Parse(txtCol.Text);
                MainWindow.NumOfMine = int.Parse(txtAmountOfMine.Text);
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                e.Handled = true;
            }
        }
        private void BtnDIYCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void TxtRow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            IsKeyPressValid(e);
        }
        private void TxtCol_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            IsKeyPressValid(e);
        }
        private void TxtAmountOfMine_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            IsKeyPressValid(e);
        }

        //判断键入值是否合法
        private void IsKeyPressValid(KeyEventArgs e)
        {
            Boolean shiftKey = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;
            if (shiftKey)
            {
                e.Handled = true;
            }
            else
            {
                if (!((e.Key >= Key.D0 && e.Key <= Key.D9)
                    || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                    || e.Key == Key.Delete
                    || e.Key == Key.Back
                    || e.Key == Key.Tab
                    || e.Key == Key.Left
                    || e.Key == Key.Right))
                {
                    e.Handled = true;
                }
            }
        }

        //判断输入的数值是否合理
        private bool IsInputResonable()
        {
            //bool isInputResonable = false; ;
            if (txtRow.Text == "")
            {
                lblRowTip.Content = "请输入高度!";
                return false;
            }
            else
            {
                lblRowTip.Content = "";
            }
            if (txtCol.Text == "")
            {
                lblColTip.Content = "请输入宽度!";
                return false;
            }
            else
            {
                lblColTip.Content = "";
            }
            if (txtAmountOfMine.Text == "")
            {
                lblAmountOfMineTip.Content = "请输入地雷数量！";
                return false;
            }
            else
            {
                int row = int.Parse(txtRow.Text);
                int col = int.Parse(txtCol.Text);
                int mines = int.Parse(txtAmountOfMine.Text);
                if (row < 9)
                {
                    txtRow.Text = 9.ToString();
                    lblRowTip.Content = "行数至少为9！";
                }
                else if (row > 25)
                {
                    txtRow.Text = 25.ToString();
                    lblRowTip.Content = "行数最多为25！";
                }
                else if (col < 9)
                {
                    txtCol.Text = 9.ToString();
                    lblColTip.Content = "列数至少为9！";
                }
                else if (col > 30)
                {
                    txtCol.Text = 30.ToString();
                    lblColTip.Content = "列数最多为30！";
                }
                else if (mines < 10)
                {
                    txtAmountOfMine.Text = 10.ToString();
                    lblAmountOfMineTip.Content = "雷数至少为10！";
                }
                else if (mines > row * col / 2)
                {
                    txtAmountOfMine.Text = (row * col / 2).ToString();
                    lblAmountOfMineTip.Content = "该条件下雷数最多为" + (row * col / 2).ToString() + "!";
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
    }
}
