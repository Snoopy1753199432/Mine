using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace Mine
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //Icon = (Icon)MineResource.ResourceManager.GetObject("mine");
            MinHeight = 360;
            MinWidth = 530;
            lblElapsedTimeText.Visibility = Visibility.Collapsed;
            lblElapsedTime.Visibility = Visibility.Collapsed;
            lblRemainingMineText.Visibility = Visibility.Collapsed;
            lblRemainingMine.Visibility = Visibility.Collapsed;
            btnPause.Visibility = Visibility.Collapsed;
            btnReset.Visibility = Visibility.Collapsed;
            dpcElapsedTime.Tick += DpcElapsedTime_Tick;
            dpcElapsedTime.Interval = new TimeSpan(0, 0, 0, 0, 10);
        }

        public static int blockNRow; //方块行数
        public static int blockNCol; //方块列数
        public static int NumOfMine; //地雷个数
        public static string level; //游戏难度
        private int iniBlockLeft = 20; //第一个方块的Left
        private int iniBlockTop = 40; //第一个方块的Top
        private int blockHeight = 30; //方块的高度
        private int blockWidth = 30; //方块的宽度
        private MineBlock[,] mineBlocks; //方块数组
        private Stopwatch stopwatch = new Stopwatch(); //更精密的计时器
        public static double gameElapsedTime = 0; //游戏已过去时间
        private DispatcherTimer dpcElapsedTime = new DispatcherTimer();//创建一个计时器
        private int numOfRemainingMine; //玩家认为剩余的地雷个数
        private int numOfRealRemainingMine; //实际上剩余的地雷个数
        private List<MineBlock> misjudgedBlockList = new List<MineBlock>(); //错误判断的方块列表
        private bool hasClickedFirstBlock = false; //是否点击了第一个方块
        private bool hasClickedPauseButton = false; //是否暂停



        private void Rdo9multiple9_Checked(object sender, RoutedEventArgs e)
        {
            blockNRow = 9;
            blockNCol = 9;
            NumOfMine = 10;
            level = "初级";
        }

        private void Rdo16multiple16_Checked(object sender, RoutedEventArgs e)
        {
            blockNRow = 16;
            blockNCol = 16;
            NumOfMine = 35;
            level = "中级";
        }

        private void Rdo16multiple30_Checked(object sender, RoutedEventArgs e)
        {
            blockNRow = 16;
            blockNCol = 30;
            NumOfMine = 80;
            level = "高级";
        }

        //自定义
        private void BtnDIY_Click(object sender, RoutedEventArgs e)
        {
            WinDIY winDIY = new WinDIY();
            if ((Boolean)winDIY.ShowDialog())
            {
                rdo16multiple16.IsChecked = false;
                rdo9multiple9.IsChecked = false;
                rdo16multiple30.IsChecked = false;
                BtnStart_Click(btnStart, new RoutedEventArgs());
            }
        }
        //开始
        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            //创建新的方块区域的对象
            mineBlocks = new MineBlock[blockNRow, blockNCol];
            //创建方块实例
            CreateMineBlock(mineBlocks);
            //根据方块区域扩展窗体大小
            StretchWindow(blockNRow, blockNCol);
            //控制控件的显示与否
            rdo9multiple9.Visibility = Visibility.Collapsed;
            rdo16multiple16.Visibility = Visibility.Collapsed;
            rdo16multiple30.Visibility = Visibility.Collapsed;
            btnDIY.Visibility = Visibility.Collapsed;
            btnStart.Visibility = Visibility.Collapsed;
            //btnPause.Visibility = Visibility.Visible;
            btnReset.Visibility = Visibility.Visible;
            lblElapsedTimeText.Visibility = Visibility.Visible;
            lblElapsedTime.Visibility = Visibility.Visible;
            lblRemainingMineText.Visibility = Visibility.Visible;
            lblRemainingMine.Visibility = Visibility.Visible;

            numOfRemainingMine = NumOfMine;
            numOfRealRemainingMine = NumOfMine;
            lblElapsedTime.Content = "0 s";
            lblRemainingMine.Content = numOfRemainingMine;
        }
        //暂停
        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            if ((String)btnPause.Content == "暂停")
            {
                btnPause.Content = "继续";
                dpcElapsedTime.Stop();
                stopwatch.Stop();
                hasClickedPauseButton = true;
            }
            else
            {
                btnPause.Content = "暂停";
                dpcElapsedTime.Start();
                stopwatch.Start();
                hasClickedPauseButton = false;
            }
        }
        //重置
        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            btnPause.Visibility = Visibility.Collapsed;
            btnPause.Content = "暂停";
            btnReset.Visibility = Visibility.Collapsed;
            btnStart.Visibility = Visibility.Visible;
            btnDIY.Visibility = Visibility.Visible;
            lblElapsedTime.Visibility = Visibility.Collapsed;
            lblElapsedTime.Content = "";
            lblElapsedTimeText.Visibility = Visibility.Collapsed;
            lblRemainingMineText.Visibility = Visibility.Collapsed;
            lblRemainingMine.Visibility = Visibility.Collapsed;
            rdo16multiple16.Visibility = Visibility.Visible;
            rdo16multiple30.Visibility = Visibility.Visible;
            rdo9multiple9.Visibility = Visibility.Visible;
            CollapseMineBlock(mineBlocks);
            dpcElapsedTime.Stop();
            stopwatch.Reset();
            gameElapsedTime = 0;
            numOfRemainingMine = NumOfMine;
            hasClickedFirstBlock = false;
            hasClickedPauseButton = false;
        }

        //左键点击方块
        private void MineBlock_i_j_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (hasClickedPauseButton) return;
            MineBlock clickedMineBlock = (MineBlock)sender;
            if (!hasClickedFirstBlock)
            {
                //设置雷区
                SetMineBlock(blockNRow, blockNCol, NumOfMine, clickedMineBlock);
                //启动计时器开始计时
                dpcElapsedTime.Start();
                stopwatch.Start();

                btnPause.Visibility = Visibility.Visible;
                hasClickedFirstBlock = true;
            }
            Status status = clickedMineBlock.Status;
            if (status == Status.UnDetected)
            {
                if (clickedMineBlock.HasMine)
                {
                    clickedMineBlock.Content = SetContentOfMineBlock(new StackPanel(), Properties.Resources.mineandwrong);
                    ShowMisjudgedBlock(misjudgedBlockList);
                    ShowMine();
                    dpcElapsedTime.Stop();
                    stopwatch.Stop();
                    btnPause.Visibility = Visibility.Collapsed;
                    if (MessageBox.Show("你输了！是否再来一局？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                    {
                        BtnReset_Click(btnReset, e);
                        BtnStart_Click(btnStart, e);
                    }
                }
                else
                {
                    ShowNumOfMineAround(clickedMineBlock);
                }
            }
        }
        //右键点击方块
        private void MineBlock_i_j_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (hasClickedPauseButton) return;
            MineBlock mineBlock = (MineBlock)sender;
            Status status = mineBlock.Status;
            StackPanel stackPanel = new StackPanel();
            if (!(status == Status.Excluded || status == Status.Hint))
            {
                if (status == Status.UnDetected)
                {
                    mineBlock.Status = Status.SignedHasMine;
                    mineBlock.Content = SetContentOfMineBlock(stackPanel, Properties.Resources.flag);
                    numOfRemainingMine -= 1;
                    if (numOfRemainingMine < 0)
                    {
                        lblRemainingMine.Foreground = Brushes.Red;
                    }
                    lblRemainingMine.Content = numOfRemainingMine;
                    if (mineBlock.HasMine)
                    {
                        numOfRealRemainingMine--;
                        if (numOfRealRemainingMine == 0)
                        {
                            dpcElapsedTime.Stop();
                            stopwatch.Stop();
                            btnPause.Visibility = Visibility.Collapsed;
                            ShowMine();
                            //if (level != null)
                            //{
                            //    new WinSuccess().ShowDialog();
                            //}
                            MessageBox.Show("恭喜通关！");
                        }
                    }
                    else
                    {
                        misjudgedBlockList.Add(mineBlock);
                    }
                }
                else if (status == Status.SignedHasMine)
                {
                    mineBlock.Status = Status.SignedNotSure;
                    mineBlock.Content = SetContentOfMineBlock(stackPanel, Properties.Resources.question);
                    numOfRemainingMine += 1;
                    if (numOfRemainingMine > 0)
                    {
                        lblRemainingMine.Foreground = Brushes.Black;
                    }
                    lblRemainingMine.Content = numOfRemainingMine;
                    if (mineBlock.HasMine)
                    {
                        numOfRealRemainingMine++;
                    }
                    else
                    {
                        misjudgedBlockList.Remove(mineBlock);
                    }
                }
                else if (status == Status.SignedNotSure)
                {
                    mineBlock.Status = Status.UnDetected;
                    mineBlock.Content = "";
                    mineBlock.Background = new SolidColorBrush(Color.FromRgb(127, 171, 237));
                }
            }
        }
        //设置双键点击效应
        public void MineBlock_i_j_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (hasClickedPauseButton) return;
            bool mouse_left = false;
            bool mouse_right = false;
            MineBlock mineBlock = (MineBlock)sender;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                mouse_left = true;
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                mouse_right = true;
            }
            if (!mouse_left || !mouse_right) return;
            if (mineBlock.Status == Status.Hint)
            {
                int numOfSignedHasMine = GetNumOfSignedHasMineAround(mineBlock);
                if (numOfSignedHasMine == int.Parse((String)mineBlock.Content))
                {
                    if (GetNumOfBlockAround_Undetected(mineBlock) > 0)
                    {
                        if (HasMisjudgedBlockAround(mineBlock))
                        {
                            ShowMisjudgedBlock(misjudgedBlockList);
                            ShowMine();
                            dpcElapsedTime.Stop();
                            stopwatch.Stop();
                            btnPause.Visibility = Visibility.Collapsed;
                            if (MessageBox.Show("你输了！是否再来一局？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                            {
                                BtnReset_Click(btnReset, e);
                                BtnStart_Click(btnStart, e);
                            }
                        }
                        else
                        {
                            foreach (var mineBlockHasNoMineAround in GetMineBlocksHasNoMineAround(mineBlock))
                            {
                                MineBlock_i_j_PreviewMouseLeftButtonUp(mineBlockHasNoMineAround, e);
                            }
                        }
                    }
                }
            }
        }

        //计时器
        private void DpcElapsedTime_Tick(object sender, EventArgs e)
        {
            stopwatch.Stop();
            gameElapsedTime = stopwatch.ElapsedMilliseconds / 1000.0;
            lblElapsedTime.Content = gameElapsedTime.ToString("0") + " s";
            stopwatch.Start();
        }

        //创建所有方块
        private void CreateMineBlock(MineBlock[,] mineBlocks)
        {
            for (int i = 0; i < mineBlocks.GetLength(0); i++)
            {
                for (int j = 0; j < mineBlocks.GetLength(1); j++)
                {
                    int blockLeft_i_j = iniBlockLeft + j * blockWidth; //i,j 处方块的Left
                    int blockTop_i_j = iniBlockTop + i * blockHeight; //i,j 处方块的Top
                    mineBlocks[i, j] = CreateSingleMineBlock(blockLeft_i_j, blockTop_i_j, i, j);
                    mineBlocks[i, j].PreviewMouseLeftButtonUp += MineBlock_i_j_PreviewMouseLeftButtonUp;
                    mineBlocks[i, j].PreviewMouseRightButtonUp += MineBlock_i_j_PreviewMouseRightButtonUp;
                    mineBlocks[i, j].PreviewMouseDown += MineBlock_i_j_PreviewMouseDown;

                    gMainWin.Children.Add(mineBlocks[i, j]);
                }
            }
        }



        //创建第 i 行，第 j 列的方块
        private MineBlock CreateSingleMineBlock(int blockLeft, int blockTop, int i, int j)
        {
            Random rnd = new Random();
            rnd.Next(0, 1);
            MineBlock mineBlock = new MineBlock
            {
                Name = "btnMineBlock_" + i + "_" + j,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(blockLeft, blockTop, 0, 0),
                Height = blockHeight,
                Width = blockWidth,
                Background = new SolidColorBrush(Color.FromRgb(127, 171, 237)),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                BorderThickness = new Thickness(0.5, 0.5, 0.5, 0.5),
                Focusable = false,
                //----------------------------------------
                I = i,
                J = j,

                Status = Status.UnDetected


            };
            return (mineBlock);
        }

        //删除已创建好的方块
        private void CollapseMineBlock(MineBlock[,] mineBlocks)
        {
            for (int i = 0; i < mineBlocks.GetLength(0); i++)
            {
                for (int j = 0; j < mineBlocks.GetLength(1); j++)
                {
                    gMainWin.Children.Remove(mineBlocks[i, j]);
                }
            }
        }

        //根据雷区范围伸展窗口
        private void StretchWindow(int blockNRow, int blockNCol)
        {
            int allMineBlockHeight = blockNRow * blockHeight;
            int allMineBlockWidth = blockNCol * blockWidth;
            Height = allMineBlockHeight + iniBlockTop + 80;
            Width = allMineBlockWidth + iniBlockLeft + 40;
        }

        //根据点击的方块位置随机设置地雷
        private void SetMineBlock(int blockNRow, int blockNCol, int NumOfMine, MineBlock clickedMineBlock)
        {
            int minValue = 1;
            int maxValue = blockNRow * blockNCol;
            int excludeValue = clickedMineBlock.I * blockNCol + clickedMineBlock.J + 1;
            int[] location = GetRandomUnrepeatArray(minValue, maxValue, NumOfMine, excludeValue);
            for (int k = 0; k < location.Length; k++)
            {
                int location_k = location[k];
                int index_i, index_j;
                if (location_k < blockNCol)
                {
                    index_i = 0;
                }
                else
                {
                    index_i = (int)Math.Ceiling((double)location_k / blockNCol) - 1;
                }
                index_j = location_k - index_i * blockNCol - 1;
                mineBlocks[index_i, index_j].HasMine = true;
            }
        }

        //生成指定范围指定数量的不重复随机数(排除第一次点击的方块位置)
        private int[] GetRandomUnrepeatArray(int minValue, int maxValue, int count, int excludeValue)
        {
            Random rnd = new Random();
            int length = maxValue - minValue;
            byte[] keys = new byte[length];
            rnd.NextBytes(keys);
            int[] items = new int[length];
            for (int i = 0; i < length; i++)
            {
                int item = i + minValue;
                if (item >= excludeValue)
                {
                    item++;
                }
                items[i] = item;
            }
            Array.Sort(keys, items);
            int[] result = new int[count];
            Array.Copy(items, result, count);
            return result;
        }

        //显示所有地雷方块
        private void ShowMine()
        {
            for (int i = 0; i < mineBlocks.GetLength(0); i++)
            {
                for (int j = 0; j < mineBlocks.GetLength(1); j++)
                {
                    mineBlocks[i, j].Background = Brushes.WhiteSmoke;
                    mineBlocks[i, j].IsHitTestVisible = false;
                    if (mineBlocks[i, j].HasMine)
                    {
                        mineBlocks[i, j].Foreground = Brushes.Red;
                        var bmp = Properties.Resources.mine;
                        var imageSource = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                        mineBlocks[i, j].Background = new ImageBrush(imageSource);
                    }
                }
            }
        }

        //显示所有判断错误的方块
        private void ShowMisjudgedBlock(List<MineBlock> misjudgedBlockList)
        {
            foreach (var mineBlock in misjudgedBlockList)
            {

                StackPanel stackPanel = new StackPanel();
                mineBlock.FontSize = 22;
                mineBlock.Foreground = Brushes.Red;
                mineBlock.Content = SetContentOfMineBlock(stackPanel, Properties.Resources.wrong);
            }
        }

        //获取周围八个方块的对象
        private List<MineBlock> GetBlocksAround(MineBlock mineBlock)
        {
            List<MineBlock> mineBlocksAround = new List<MineBlock>();
            int location_i_lower = mineBlock.I - 1;
            int location_i_upper = mineBlock.I + 1;
            int location_j_lower = mineBlock.J - 1;
            int location_j_upper = mineBlock.J + 1;
            if (location_i_lower - 1 < 0)
            {
                location_i_lower = 0;
            }
            if (location_j_lower - 1 < 0)
            {
                location_j_lower = 0;
            }
            if (location_i_upper + 1 >= blockNRow)
            {
                location_i_upper = blockNRow - 1;
            }
            if (location_j_upper + 1 >= blockNCol)
            {
                location_j_upper = blockNCol - 1;
            }
            for (int i = location_i_lower; i <= location_i_upper; i++)
            {
                for (int j = location_j_lower; j <= location_j_upper; j++)
                {
                    if (mineBlocks[i, j] != mineBlock)
                    {
                        mineBlocksAround.Add(mineBlocks[i, j]);
                    }
                }
            }
            return mineBlocksAround;
        }

        //获取周围八个方块中未被探测到的方块对象
        private List<MineBlock> GetMineBlocksUnDetectedAround(MineBlock mineBlock)
        {
            List<MineBlock> mineBlocksUnDetectedAround = new List<MineBlock>();
            foreach (var mineBlockAround in GetBlocksAround(mineBlock))
            {
                if (mineBlockAround.Status == Status.UnDetected)
                {
                    mineBlocksUnDetectedAround.Add(mineBlockAround);
                }
            }
            return mineBlocksUnDetectedAround;
        }
        //获取周围八个方块中未被探测到的方块个数
        private int GetNumOfBlockAround_Undetected(MineBlock mineBlock)
        {
            return GetMineBlocksUnDetectedAround(mineBlock).Count;
        }

        //获取周围八个方块中没有地雷的方块的对象

        private List<MineBlock> GetMineBlocksHasNoMineAround(MineBlock mineBlock)
        {
            List<MineBlock> mineBlocksHasNoMineAround = new List<MineBlock>();
            foreach (var mineBlockAround in GetBlocksAround(mineBlock))
            {
                if (!mineBlockAround.HasMine)
                {
                    mineBlocksHasNoMineAround.Add(mineBlockAround);
                }
            }
            return mineBlocksHasNoMineAround;
        }

        //获取周围八个方块中Status为Excluded,Hint,HasMine之外的方块对象
        private List<MineBlock> GetMineBlocks_NotExcluded_NotHint_NotHasMine(MineBlock mineBlock)
        {
            List<MineBlock> mineBlocks_NotExcluded_NotHint_NotHasMine = new List<MineBlock>();
            foreach (var mineBlock_NotExcluded_NotHint_NotHasMine in GetBlocksAround(mineBlock))
            {
                bool condition = mineBlock_NotExcluded_NotHint_NotHasMine.Status != Status.Excluded && mineBlock_NotExcluded_NotHint_NotHasMine.Status != Status.Hint && mineBlock_NotExcluded_NotHint_NotHasMine.Status != Status.SignedHasMine;
                if (condition)
                {
                    mineBlocks_NotExcluded_NotHint_NotHasMine.Add(mineBlock_NotExcluded_NotHint_NotHasMine);
                }
            }
            return mineBlocks_NotExcluded_NotHint_NotHasMine;
        }
        //计算周围八个方块中标记为flag的方块个数
        private int GetNumOfSignedHasMineAround(MineBlock mineBlock)
        {
            List<MineBlock> MineBlocksAround = GetBlocksAround(mineBlock);
            int numOfSignedHasMine = 0;
            foreach (var mineBlockAround in MineBlocksAround)
            {
                if (mineBlockAround.Status == Status.SignedHasMine)
                {
                    numOfSignedHasMine++;
                }
            }
            return numOfSignedHasMine;
        }

        //计算周围八个方块的地雷个数
        private int GetNumOfMineAround(MineBlock mineBlock)
        {
            int numOfMineAround = 0;
            foreach (var mineBlockAround in GetBlocksAround(mineBlock))
            {
                if (mineBlockAround.HasMine)
                {
                    numOfMineAround++;
                }
            }
            return numOfMineAround;
        }

        //判断周围八个方块是否有玩家标记错误的方块
        private bool HasMisjudgedBlockAround(MineBlock mineBlock)
        {
            foreach (var mineBlockAround in GetBlocksAround(mineBlock))
            {
                if (misjudgedBlockList.Contains(mineBlockAround))
                {
                    return true;
                }
            }
            return false;
        }

        //在当前方块上显示周围八个方块的地雷个数
        private void ShowNumOfMineAround(MineBlock mineBlock)
        {
            int num = GetNumOfMineAround(mineBlock);
            if (num == 0)
            {
                mineBlock.Content = "";
                mineBlock.Background = Brushes.White;
                mineBlock.Status = Status.Excluded;
                mineBlock.IsHitTestVisible = false;
                foreach (MineBlock mineBlockAround in GetMineBlocks_NotExcluded_NotHint_NotHasMine(mineBlock))
                {
                    ShowNumOfMineAround(mineBlockAround);
                }
            }
            else
            {
                switch (num)
                {
                    case 1:
                        mineBlock.Content = "1";
                        mineBlock.Foreground = Brushes.DeepSkyBlue;
                        SetMineBlockOnlyLeftAndRightClick(mineBlock);
                        break;
                    case 2:
                        mineBlock.Content = "2";
                        mineBlock.Foreground = Brushes.DarkGreen;
                        SetMineBlockOnlyLeftAndRightClick(mineBlock);
                        break;
                    case 3:
                        mineBlock.Content = "3";
                        mineBlock.Foreground = Brushes.DarkRed;
                        SetMineBlockOnlyLeftAndRightClick(mineBlock);
                        break;
                    case 4:
                        mineBlock.Content = "4";
                        mineBlock.Foreground = Brushes.Orange;
                        SetMineBlockOnlyLeftAndRightClick(mineBlock);
                        break;
                    case 5:
                        mineBlock.Content = "5";
                        mineBlock.Foreground = Brushes.Fuchsia;
                        SetMineBlockOnlyLeftAndRightClick(mineBlock);
                        break;
                    case 6:
                        mineBlock.Content = "6";
                        mineBlock.Foreground = Brushes.Brown;
                        SetMineBlockOnlyLeftAndRightClick(mineBlock);
                        break;
                    case 7:
                        mineBlock.Content = "7";
                        mineBlock.Foreground = Brushes.SlateGray;
                        SetMineBlockOnlyLeftAndRightClick(mineBlock);
                        break;
                    case 8:
                        mineBlock.Content = "8";
                        mineBlock.Foreground = Brushes.Gray;
                        SetMineBlockOnlyLeftAndRightClick(mineBlock);
                        break;
                }
            }
        }

        //设置已排除的方块为Hint状态
        private void SetMineBlockOnlyLeftAndRightClick(MineBlock mineBlock)
        {
            mineBlock.Background = Brushes.White;
            mineBlock.Status = Status.Hint;
        }

        //设置方块状态图形
        private StackPanel SetContentOfMineBlock(StackPanel stackPanel, System.Drawing.Bitmap res_bmp)
        {
            stackPanel.VerticalAlignment = VerticalAlignment.Stretch;
            stackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            var bmp = res_bmp;
            var imageSource = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            Image image = new Image();
            image.Source = imageSource;
            stackPanel.Children.Add(image);
            return stackPanel;
        }
    }
}
