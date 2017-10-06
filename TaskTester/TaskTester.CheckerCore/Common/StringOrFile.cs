﻿using System;
using System.Collections.Generic;
using System.IO;    

namespace TaskTester.CheckerCore.Common
{
    public class StringOrFile
    {
        class Locker { public object locker = new object(); }
        static Locker staticLocker = new Locker();

        static Dictionary<string, StringOrFile> repo = new Dictionary<string, StringOrFile>();


        object thisLock = new object();
        bool fileIsTemporary = false;
        string text = null;
        string filePath = null;

        public string Str => GetText();
        public string FilePath => GetFilePath();

        public static StringOrFile FromText(string text) => new StringOrFile(text, false);
        public static StringOrFile FromFile(string path)
        {
            lock (staticLocker.locker)
            {
                if (repo.ContainsKey(path))
                {
                    return repo[path];
                }
                var rt = new StringOrFile(path, true);
                repo[path] = rt;
                return rt;
            }
        }
        public override string ToString() => GetText();

        private string GetText()
        {
            FetchText();
            return text;
        }

        private string GetFilePath()
        {
            FetchFile();
            return filePath;
        }

        private StringOrFile(string textOrPath, bool isFile)
        {
            if (textOrPath == null) throw new ArgumentNullException($"{nameof(textOrPath)} cannot be null!");

            if (isFile)
            {
                if (!File.Exists(textOrPath)) throw new ArgumentException($"File {textOrPath} doesn't exist!");
                filePath = textOrPath;
                fileIsTemporary = false;
            }
            else
            {
                text = textOrPath;
                fileIsTemporary = true;
            }
        }

        void FetchText()
        {
            if (text != null) return;
            lock (thisLock)
            {
                if (text == null) { text = File.ReadAllText(filePath); }
            }
        }

        void FetchFile()
        {
            if (filePath != null && File.Exists(filePath)) return;

            lock (thisLock)
            {
                if (filePath == null || !File.Exists(filePath))
                {
                    filePath = Path.GetTempFileName();
                    lock (staticLocker.locker)
                    {
                        repo[filePath] = this;
                        File.WriteAllText(filePath, text);
                    }
                }
            }
        }

        public void PersistFile() { fileIsTemporary = false; }

        ~StringOrFile()
        {
            if (fileIsTemporary && filePath != null && File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
