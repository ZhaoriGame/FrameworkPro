using System.IO;
using UnityEditor;

namespace Framework.Edit
{
    public class HotResPublishModel
    {
        const string CONFIG_NAME = "HotResCfg.json";

        const string HOT_RES_BACKUP_ROOT = "HotResBackup";

        public HotResConfigVO Cfg { get; private set; }

        /// <summary>
        /// DLL代码在Asset中的位置
        /// </summary>
        public string DllDirInAssets { get; }

        /// <summary>
        /// DLL代码在备份目录的位置
        /// </summary>
        public string DllDirInBackup { get; }

        public HotResPublishModel()
        {
            LoadConfig();
            DllDirInAssets = Cfg.ilScriptDir;
            DllDirInBackup = FileSystem.CombineDirs(false, HOT_RES_BACKUP_ROOT, Cfg.ilScriptDir);
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        public void LoadConfig()
        {
            Cfg = EditorConfigUtil.LoadConfig<HotResConfigVO>(CONFIG_NAME);
            if (null == Cfg)
            {
                Cfg = new HotResConfigVO();
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        public void SaveConfig()
        {
            EditorConfigUtil.SaveConfig(Cfg, CONFIG_NAME);
        }

        /// <summary>
        /// 构建热更AssetBundle资源
        /// </summary>
        public void BuildAssetBundle()
        {
            //标记目标目录
            new AssetBundleBuildCommand(Cfg.resDir, Cfg.abHotResDir, Cfg.isKeepManifest).Execute();
        }


        /// <summary>
        /// 构建热更DLL文件
        /// </summary>
        public void BuildDll()
        {
            if (Copy2DllProj())
            {
                var cmd = new DllBuildCommand(Cfg.resDir, Cfg.devenvPath, Cfg.ilProjCsprojPath);
                cmd.Execute();
            }
        }

        /// <summary>
        /// 构建res.json文件
        /// </summary>
        public void BuildResJsonFile()
        {
            new ResJsonBuildCommand(Cfg.resDir).Execute();
        }

        /// <summary>
        /// 拷贝代码到Proj项目
        /// </summary>
        public bool Copy2DllProj()
        {
            string projCodeDir = Path.Combine(Cfg.ilProjDir, "codes");

            if (Directory.Exists(Cfg.ilScriptDir))
            {
                if (Directory.Exists(projCodeDir))
                {
                    Directory.Delete(projCodeDir, true);
                }
                FileUtil.CopyFileOrDirectory(Cfg.ilScriptDir, projCodeDir);
                FileSystem.DeleteFilesByExt(projCodeDir, "meta");

                AssetDatabase.Refresh();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 排除DLL代码
        /// </summary>
        public void ExcludeDllCodes()
        {
            if (Directory.Exists(DllDirInAssets))
            {
                if (false == Directory.Exists(DllDirInBackup))
                {
                    Directory.CreateDirectory(DllDirInBackup);
                }
                FileUtil.ReplaceDirectory(DllDirInAssets, DllDirInBackup);
                FileUtil.DeleteFileOrDirectory(DllDirInAssets);
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// 导入DLL代码
        /// </summary>
        public void IncludeDllCodes()
        {
            //代码有备份，且Asset中没代码才还原
            if (Directory.Exists(DllDirInBackup) && false == Directory.Exists(DllDirInAssets))
            {
                if (false == Directory.Exists(DllDirInAssets))
                {
                    Directory.CreateDirectory(DllDirInAssets);
                }
                FileUtil.ReplaceDirectory(DllDirInBackup, DllDirInAssets);
                FileUtil.DeleteFileOrDirectory(DllDirInBackup);
                AssetDatabase.Refresh();
            }
        }
    }
}