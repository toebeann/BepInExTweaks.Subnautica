using BepInEx;
using BepInEx.Logging;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using ByteSizeLib;
using BepInEx.Configuration;
using Oculus.Platform.Models;

namespace Tobey.BepInExTweaks.Subnautica;
[Tweak, DisallowMultipleComponent]
public class FileTreeTweaks : MonoBehaviour
{
    private readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("File Tree");

    internal ConfigEntry<bool> LogFileTree { get; } =
            BepInExTweaks.Instance.Config.Bind(
                section: "General",
                key: "Log file tree",
                defaultValue: true,
                configDescription: new(
                    description: "When enabled, BepInEx Tweaks will log the file tree to the console.",
                    tags: new[] { new ConfigurationManagerAttributes { IsAdvanced = true } }
                )
            );

    private void Awake()
    {
        LogFileTree.SettingChanged += LogFileTree_SettingChanged;
        LogFileTree_SettingChanged(this, null);
    }

    private void LogFileTree_SettingChanged(object _, EventArgs __)
    {
        if (LogFileTree.Value)
        {
            enabled = true;
        }
        else
        {
            enabled = false;
        }
    }

    private void OnEnable() => ThreadingHelper.Instance.StartAsyncInvoke(() =>
    {
        var excludeDirs = Directory.GetDirectories(Paths.GameRootPath)
            .Select(dir => Path.GetFileName(dir).ToLowerInvariant())
            .Where(dir => !new[] { "bepinex", "qmods" }.Contains(dir));

        var root = new FileTreeNode(Paths.GameRootPath, excludeDirs);
        root.PrettyPrint(Logger.LogMessage);
        return null;
    });

    public class FileTreeNode
    {
        public readonly string Path;
        public readonly string Name;
        public readonly List<FileTreeNode> Children;
        public readonly FileTreeNode Parent;
        public bool IsRoot => Parent is null;
        public bool IsDirectory => Path is not null && File.GetAttributes(Path).HasFlag(FileAttributes.Directory);
        public bool IsFile => Path is not null && !File.GetAttributes(Path).HasFlag(FileAttributes.Directory);

        public FileTreeNode(string path, IEnumerable<string> exclude = null)
        {
            Path = path;
            Name = System.IO.Path.GetFileName(Path);
            Parent = null;

            exclude ??= Enumerable.Empty<string>();
            exclude = exclude.Select(x => x.ToLowerInvariant());

            Children = GetChildren(exclude);
        }

        private FileTreeNode(string path, FileTreeNode parent, IEnumerable<string> exclude = null)
        {
            Path = path;
            Name = System.IO.Path.GetFileName(Path);
            Parent = parent;

            exclude ??= Enumerable.Empty<string>();
            exclude = exclude.Select(x => x.ToLowerInvariant());

            Children = GetChildren(exclude);
        }

        private FileTreeNode(string name, FileTreeNode parent)
        {
            Path = null;
            Name = name;
            Parent = parent;
            Children = new();
        }

        private List<FileTreeNode> GetChildren(IEnumerable<string> exclude)
        {
            if (IsFile) return new();

            if (exclude.Contains(Name.ToLowerInvariant()))
                return new List<FileTreeNode> { new("(contents not shown)", this) };

            return Directory.GetDirectories(Path)
                .Concat(Directory.GetFiles(Path)
                    .Where(file => !exclude.Contains(System.IO.Path.GetFileName(file).ToLowerInvariant())))
                .Select(p => new FileTreeNode(p, this, exclude)).ToList();
        }

        public long? GetSize()
        {
            if (IsDirectory) return null;

            try
            {
                return new FileInfo(Path).Length;
            }
            catch
            {
                return null;
            }
        }

        public void PrettyPrint(Action<string> printer) => PrettyPrint(printer, "");

        public void PrettyPrint(Action<string> printer, string indent, bool last = false)
        {
            string output = indent;
            if (!IsRoot)
            {
                output += last
                    ? "\\-- "
                    : "|-- ";
            }

            printer.Invoke($"{output}{(IsRoot ? Path : Name)}{(GetSize() is long size ? $" [{ByteSize.FromBytes(size):0.##}]" : "")}");

            if (!IsRoot)
            {
                indent += last
                    ? "    "
                    : "|   ";
            }

            for (int i = 0; i < Children.Count; i++)
            {
                if (i == 0 || Children[i].IsDirectory || i == Children.IndexOf(Children.FirstOrDefault(node => node.IsFile) ?? this))
                {
                    printer.Invoke($"{indent}|");
                }

                Children[i].PrettyPrint(printer, indent, i == Children.Count - 1);
            }
        }
    }
}
