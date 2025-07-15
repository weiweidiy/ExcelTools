using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConfigTools
{
    public class CodeRegisterTableHelper
    {
        public static void GenCode(List<TableMeta> pTableMeta, string pPath, ExportCfgType pCfgType)
        {
            if (pCfgType == ExportCfgType.Client)
                GenCSCode(pTableMeta, pPath, pCfgType);
            else if (pCfgType == ExportCfgType.Server)
                GenGOCode(pTableMeta, pPath, pCfgType);
        }

        private static void GenCSCode(List<TableMeta> pTableMetas, string pPath, ExportCfgType pCfgType)
        {
            string registerName = "TiktokGenConfigManager";
            var _Path = Path.Combine(pPath, registerName + ".cs");
            using (var fs = new FileStream(_Path, FileMode.Create, FileAccess.Write))
            {
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    //if (!pTableMeta.CheckTypeIsMap())
                    //{
                    //    MessageBox.Show($"表 {pTableMeta.ClassName} 的第一列不是ID字段");
                    //    return;
                    //}
                    sw.WriteLine("/*");
                    sw.WriteLine("* 此类由ConfigTools自动生成. 不要手动修改!");
                    sw.WriteLine("*/");
                    sw.WriteLine("using System.Collections;");
                    sw.WriteLine("using System.Collections.Generic;");
                    sw.WriteLine("using JFramework.Game;");
                    sw.WriteLine("");
                    sw.WriteLine("namespace JFramework");
                    sw.WriteLine("{");
                    sw.WriteLine("    public partial class {0} : JConfigManager", registerName);
                    sw.WriteLine("    {");
                    sw.WriteLine("        public TiktokGenConfigManager(IConfigLoader loader, IDeserializer deserializer) : base(loader)");
                    //sw.WriteLine("        public static {0} GetData(int pID)", pTableMeta.DataName);
                    sw.WriteLine("        {");

                    foreach(var pTableMeta in pTableMetas)
                    {
                        sw.WriteLine("          RegisterTable<{0}, {1}>(nameof({2}), deserializer);", pTableMeta.ClassName, pTableMeta.DataName, pTableMeta.ClassName);
                    }
                    //sw.WriteLine("            return ConfigManager.Ins.m_{0}.AllData.TryGetValue(pID.ToString(), out var _data) ? _data : null;", pTableMeta.ClassName);
                    sw.WriteLine("        }");
                    sw.WriteLine("    }");
                    sw.WriteLine();
                    //sw.WriteLine("    public class {0} : IUnique", pTableMeta.DataName);
                    //sw.WriteLine("    {");
                    //foreach (var field in pTableMeta.Fields)
                    //{
                    //    if (!field.IsExportField(pCfgType))
                    //        continue;

                    //    if (!string.IsNullOrWhiteSpace(field.mSpostil))
                    //    {
                    //        sw.WriteLine("        /*");
                    //        sw.WriteLine("        " + field.mSpostil);
                    //        sw.WriteLine("        */");
                    //    }

                    //    sw.WriteLine("        " + field.mComment);
                    //    if (field.mFieldName == "Uid")
                    //        sw.WriteLine("        public {0} {1} ", field.GetFieldTypeName(ExportCfgType.Client), field.mFieldName + "{ get;set;}");
                    //    else
                    //        sw.WriteLine("        public {0} {1};", field.GetFieldTypeName(ExportCfgType.Client), field.mFieldName);

                    //    sw.WriteLine();
                    //}
                    //sw.WriteLine("    }");
                    sw.WriteLine("}");
                }
            }
        }

        private static void GenGOCode(List<TableMeta> pTableMetas, string pPath, ExportCfgType pCfgType)
        {
            var pTableMeta = pTableMetas[0]; 
            var _Path = Path.Combine(pPath, pTableMeta.ClassName + ".go");
            using (var fs = new FileStream(_Path, FileMode.Create, FileAccess.Write))
            {
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.WriteLine("/*");
                    sw.WriteLine("* 此类由ConfigTools自动生成. 不要手动修改!");
                    sw.WriteLine("*/");
                    sw.WriteLine();
                    sw.WriteLine("package Configs");
                    sw.WriteLine();
                    sw.WriteLine("import (");
                    sw.WriteLine("    \"encoding/json\"");
                    sw.WriteLine("    \"fmt\"");
                    sw.WriteLine("    \"os\"");
                    sw.WriteLine(")");
                    sw.WriteLine();
                    sw.WriteLine("var {0} {1}", pTableMeta.ClassName + "Data", pTableMeta.ClassName);
                    sw.WriteLine();

                    sw.WriteLine("type {0} struct {{", pTableMeta.ClassName);
                    sw.WriteLine("    AllData map[int32]*{0}", pTableMeta.DataName);
                    sw.WriteLine("}");
                    sw.WriteLine();

                    sw.WriteLine("type {0} struct {{", pTableMeta.DataName);
                    foreach (var field in pTableMeta.Fields)
                    {
                        if (!field.IsExportField(pCfgType))
                            continue;

                        if (!string.IsNullOrWhiteSpace(field.mSpostil))
                        {
                            sw.WriteLine("        /*");
                            sw.WriteLine("        " + field.mSpostil);
                            sw.WriteLine("        */");
                        }

                        sw.WriteLine("    {0} {1}  {2}", field.mFieldName, field.GetFieldTypeName(ExportCfgType.Server), field.mComment);
                    }
                    sw.WriteLine("}");
                    sw.WriteLine();

                    sw.WriteLine("func Load{0}(pDir string) {{", pTableMeta.ClassName);
                    sw.WriteLine("    file, err := os.ReadFile(pDir + `/{0}.json`)", pTableMeta.ClassName);
                    sw.WriteLine("    if err != nil {");
                    sw.WriteLine("        panic(fmt.Sprintf(\"Load [{0}] Failed. {{%s}}\", err))", pTableMeta.ClassName);
                    sw.WriteLine("    }");
                    sw.WriteLine();
                    sw.WriteLine("    err = json.Unmarshal(file, &{0})", pTableMeta.ClassName + "Data");
                    sw.WriteLine("    if err != nil {");
                    sw.WriteLine("        panic(fmt.Sprintf(\"Unmarshal [{0}] Failed. {{%s}}\", err))", pTableMeta.ClassName);
                    sw.WriteLine("        return");
                    sw.WriteLine("    }");
                    sw.WriteLine("}");
                    sw.WriteLine();

                    sw.WriteLine("func Get{0}(id int32) *{0} {{", pTableMeta.DataName);
                    sw.WriteLine("    data, have := {0}.AllData[id]", pTableMeta.ClassName + "Data");
                    sw.WriteLine("    if have {");
                    sw.WriteLine("        return data");
                    sw.WriteLine("    }");
                    sw.WriteLine("    return nil");
                    sw.WriteLine("}");
                }
            }
        }
    }
}