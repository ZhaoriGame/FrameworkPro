﻿using System.IO;

namespace Framework.Edit
{
    class ResJsonBuildCommand
    {
        string _resDir;

        public ResJsonBuildCommand(string resDir)
        {
            _resDir = resDir;
        }

        public void Execute()
        {
            var resDir = FileSystem.CombineDirs(false, _resDir, ZeroEditorUtil.PlatformDirName);
            if (false == Directory.Exists(resDir))
            {
                Directory.CreateDirectory(resDir);
            }

            var filePath = FileSystem.CombinePaths(resDir, "res.json");

            //首先删除旧的
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            ResVerVO res = new ResVerVOBuilder(resDir).Build();            
            string jsonStr = LitJson.JsonMapper.ToJson(res);
            File.WriteAllText(filePath, jsonStr);
        }
    }
}
