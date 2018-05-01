using System;
using System.Collections.Concurrent;
using System.IO;

namespace TaskTester.CheckerCore.Common
{
    public class StringOrFile
    {
        public static readonly StringOrFile Empty = StringOrFile.FromText(string.Empty);

        static ConcurrentDictionary<string, StringOrFile> Repository =
            new ConcurrentDictionary<string, StringOrFile>();

        private StringOrFile() { }

        object thisLock = new object();
        bool fileIsTemporary = false;
        string text = null;
        string filePath = null;

        public string StringValue => GetText();
        public string FilePath => GetFilePath();

        public static StringOrFile FromText(string text) => new StringOrFile(text, false);
        public static StringOrFile FromFile(string path)
        {
            return Repository.GetOrAdd(path, (x) => new StringOrFile(path, true));
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
                    Repository.AddOrUpdate(
                        FilePath,
                        (x) =>
                        {
                            File.WriteAllText(FilePath, text);
                            return this;
                        },
                        (x, old) =>
                        {
                            File.WriteAllText(FilePath, text);
                            return this;
                        }
                    );
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
