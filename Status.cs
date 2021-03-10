namespace Mine
{
    enum Status
    {
        UnDetected = 1, //未触及到的方块
        SignedHasMine = 2, //玩家已确定存在地雷的方块
        SignedNotSure = 3, //玩家待定的方块
        Excluded = 4, //已排除的无数字提示的方块
        Hint = 5 //数字提示的方块
    }
}
