namespace ConfigTools
{
    //字段类型
    public enum TableFieldType
    {
        TFT_Int = 0,
        TFT_Float = 1,
        TFT_String = 2,
        TFT_Bool = 3,
        TFT_IntList = 4,
        TFT_FloatList = 5,
        TFT_StringList = 6,
        TFT_Int2DList = 7, //二维整型列表
        TFT_Float2DList = 8, //二维浮点型列表
        TFT_String2DList = 9 //二维字符串列表
    }

    //导出配置类型
    public enum ExportCfgType
    {
        Client, //仅客户端
        Server //仅服务器
    }
}