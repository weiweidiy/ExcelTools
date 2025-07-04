﻿using System;
using System.Data;
using System.IO;
using System.Text;

namespace ConfigTools
{

    public class CfgArrayHelper
    {
        public static string JsonHead = "[";
        public static string JsonTail = "]";

        public static string JsonDataBegin = @"  {";
        public static string JsonDataEnd = @"   }";

        //public static string JsonDataName = "  \"AllData\":{"; //以Map形式保存

        public static void GenCfg(DataTable pDT, string pPath, TableMeta pTableMeta, ExportCfgType pCfgType)
        {
            UTF8Encoding utf8 = new UTF8Encoding(false);
            var _Path = Path.Combine(pPath, pTableMeta.ClassName + ".json");
            using (var fs = new FileStream(_Path, FileMode.Create, FileAccess.Write))
            {
                using (var sw = new StreamWriter(fs, utf8))
                {
                    sw.WriteLine(JsonHead);
                    //sw.WriteLine(JsonDataName);

                    for (var iRow = 5; iRow < pDT.Rows.Count; iRow++)
                    {
                        //跳过ID为空的行
                        var _tD = pDT.Rows[iRow].ItemArray[0].ToString();
                        if (string.IsNullOrWhiteSpace(_tD))
                        {
                            continue;
                        }

                        //sw.Write("  \"{0}\":", pDT.Rows[iRow].ItemArray[0]);
                        sw.WriteLine(JsonDataBegin);
                        for (var iCol = 0; iCol < pDT.Columns.Count; iCol++)
                        {
                            if (!pTableMeta.Fields[iCol].IsExportField(pCfgType))
                                continue;

                            if (iCol != 0)
                                sw.WriteLine(",");
                            sw.Write("    \"{0}\":", pTableMeta.Fields[iCol].mFieldName);
                            var _D = pDT.Rows[iRow].ItemArray[iCol].ToString();
                            if (iCol == 0 && string.IsNullOrWhiteSpace(_D))
                                throw new Exception("ID cant be null");
                            if (pTableMeta.Fields[iCol].mTypeName == "int")
                                sw.Write(ParseInt(_D));
                            else if (pTableMeta.Fields[iCol].mTypeName == "float")
                                sw.Write(ParseFloat(_D));
                            else if (pTableMeta.Fields[iCol].mTypeName == "string")
                                sw.Write(ParseString(_D));
                            else if (pTableMeta.Fields[iCol].mTypeName == "bool")
                                sw.Write(ParseBool(_D));
                            else if (pTableMeta.Fields[iCol].mTypeName == "int+")
                                sw.Write(ParseIntList(_D));
                            else if (pTableMeta.Fields[iCol].mTypeName == "float+")
                                sw.Write(ParseFloatList(_D));
                            else if (pTableMeta.Fields[iCol].mTypeName == "string+")
                                sw.Write(ParseStringList(_D));
                        }
                        sw.WriteLine();
                        sw.Write(JsonDataEnd);
                        //不是最后一行，加逗号
                        if (iRow != pDT.Rows.Count - 1 && !string.IsNullOrWhiteSpace(pDT.Rows[iRow+1].ItemArray[0].ToString()))
                            sw.WriteLine(",");
                        else
                            sw.WriteLine();
                    }

                    //sw.WriteLine("  }");
                    sw.WriteLine(JsonTail);
                }
            }

            //是否压缩
            if (Main.sNeedCom)
            {
                var _AllCon = File.ReadAllText(_Path);
                _AllCon = _AllCon.Trim().Replace(" ", "").Replace("\n", "").Replace("\r", "");
                File.WriteAllText(_Path, _AllCon);
            }
        }

        public static string ParseInt(string pData)
        {
            if (string.IsNullOrWhiteSpace(pData))
                return "0";
            return pData;
        }

        public static string ParseFloat(string pData)
        {
            if (string.IsNullOrWhiteSpace(pData))
                return "0.0";
            return pData;
        }

        public static string ParseString(string pData)
        {
            return $"\"{pData}\"";
        }

        public static string ParseBool(string pData)
        {
            if (string.IsNullOrWhiteSpace(pData))
                return "false";

            if (pData == "0")
                return "false";
            return "true";
        }

        public static string ParseIntList(string pData)
        {
            var strTemp = pData.ToString().Replace("\n", "");
            if (strTemp.Trim().Length <= 0)
                return "[]";

            var strData = "[\r\n";
            strData += "            ";
            var endwithsem = false;
            var lastdel = strTemp.LastIndexOf(';');
            if (lastdel == strTemp.Length - 1)
                endwithsem = true;

            for (var i = 0; i < strTemp.Length; ++i)
                if (strTemp[i] == ';')
                    if (endwithsem && (i == lastdel))
                    {
                    }
                    else
                    {
                        strData += ",\r\n";
                        strData += "            ";
                    }
                else
                    strData += strTemp[i];

            strData += "\r\n         ]";
            return strData;
        }

        public static string ParseFloatList(string pData)
        {
            return ParseIntList(pData);
        }

        public static string ParseStringList(string pData)
        {
            var strData = "[\r\n";
            var strTemp = pData.ToString().Replace("\n", "");
            if (strTemp.Trim().Length <= 0)
                return "[]";

            var strArray = strTemp.Split(';');
            for (var i = 0; i < strArray.Length; ++i)
                if (strArray[i].Trim().Length > 0)
                {
                    if (i != 0)
                        strData += ",\r\n";

                    strData += "            \"" + strArray[i] + "\"";
                }

            strData += "\r\n         ]";
            return strData;
        }
    }
}