﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using System.Dynamic;

namespace Docfx.Plugins;

public sealed class FileModel
{
    private ImmutableArray<UidDefinition> _uids = ImmutableArray<UidDefinition>.Empty;

    public FileModel(FileAndType ft, object content, FileAndType original = null)
        : this(ft, content, original, null)
    {
    }

    public FileModel(FileAndType ft, object content, FileAndType original, string key)
    {
        OriginalFileAndType = original ?? ft;
        Key = key;
        if (Key == null)
        {
            if (OriginalFileAndType.File.StartsWith("~/", StringComparison.Ordinal))
            {
                Key = OriginalFileAndType.File;
            }
            else
            {
                Key = "~/" + OriginalFileAndType.File;
            }
        }

        FileAndType = ft;
        Content = content;
    }

    public FileAndType FileAndType { get; private set; }

    public FileAndType OriginalFileAndType { get; private set; }

    public object Content { get; set; }

    public string BaseDir
    {
        get
        {
            return FileAndType.BaseDir;
        }
        set
        {
            if (value != BaseDir)
            {
                FileAndType = FileAndType.ChangeBaseDir(value);
                OnFileOrBaseDirChanged();
            }
        }
    }

    public string File
    {
        get
        {
            return FileAndType.File;
        }
        set
        {
            if (value != File)
            {
                FileAndType = FileAndType.ChangeFile(value);
                OnFileOrBaseDirChanged();
            }
        }
    }

    public DocumentType Type => FileAndType.Type;

    public string Key { get; }

    public ImmutableHashSet<string> LinkToFiles { get; set; } = ImmutableHashSet<string>.Empty;

    public ImmutableHashSet<string> LinkToUids { get; set; } = ImmutableHashSet<string>.Empty;

    public ImmutableDictionary<string, ImmutableList<LinkSourceInfo>> UidLinkSources { get; set; } = ImmutableDictionary<string, ImmutableList<LinkSourceInfo>>.Empty;

    public ImmutableDictionary<string, ImmutableList<LinkSourceInfo>> FileLinkSources { get; set; } = ImmutableDictionary<string, ImmutableList<LinkSourceInfo>>.Empty;

    public dynamic Properties { get; } = new ExpandoObject();

    public dynamic ManifestProperties { get; } = new ExpandoObject();

    public FileModel MarkdownFragmentsModel { get; set; }

    public string LocalPathFromRoot { get; set; }

    public string DocumentType { get; set; }

    public ImmutableArray<UidDefinition> Uids
    {
        get { return _uids; }
        set
        {
            var original = _uids;
            _uids = value;
            OnUidsChanged(nameof(Uids), original, value);
        }
    }

    public event EventHandler<PropertyChangedEventArgs<ImmutableArray<UidDefinition>>> UidsChanged;

    public event EventHandler FileOrBaseDirChanged;

    private void OnUidsChanged(string propertyName, ImmutableArray<UidDefinition> original, ImmutableArray<UidDefinition> current)
    {
        UidsChanged?.Invoke(this, new PropertyChangedEventArgs<ImmutableArray<UidDefinition>>(propertyName, original, current));
    }

    private void OnFileOrBaseDirChanged()
    {
        FileOrBaseDirChanged?.Invoke(this, EventArgs.Empty);
    }
}