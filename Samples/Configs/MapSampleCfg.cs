/*
* 此类由ConfigTools自动生成. 不要手动修改!
*/
using System.Collections.Generic;
using JFramework;

namespace JFramework
{
    public partial class MapSampleCfg : BaseConfigTable<MapSampleData>
    {
    }

    public class MapSampleData
    {
        //表的ID,有此ID会生成Map结构
        public string Uid;

        /*
        这是批注, 暂时批注只能写在这一行上面
        */
        //文字描述
        public string Name;

        //整形数据
        public int IntData;

        //整型数组
        public List<int> IntArray;

        //浮点数据
        public float FloatData;

        //浮点数组
        public List<float> FloatArray;

        //字符串
        public string StringData;

        //字符串数组
        public List<string> StringList;

        //bool值
        public bool BoolData;

    }
}
