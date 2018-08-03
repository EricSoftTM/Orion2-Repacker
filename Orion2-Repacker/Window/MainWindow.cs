/*
 *      This file is part of Orion2, a MapleStory2 Packaging Library Project.
 *      Copyright (C) 2018 Eric Smith <notericsoft@gmail.com>
 * 
 *      This program is free software: you can redistribute it and/or modify
 *      it under the terms of the GNU General Public License as published by
 *      the Free Software Foundation, either version 3 of the License, or
 *      (at your option) any later version.
 * 
 *      This program is distributed in the hope that it will be useful,
 *      but WITHOUT ANY WARRANTY; without even the implied warranty of
 *      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *      GNU General Public License for more details.
 * 
 *      You should have received a copy of the GNU General Public License
 */

using Orion.Crypto;
using Orion.Crypto.Common;
using Orion.Crypto.Stream;
using Orion.Crypto.Stream.DDS;
using Orion.Window.Common;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Windows.Forms;
using static Orion.Crypto.CryptoMan;

namespace Orion.Window
{
    public partial class MainWindow : Form
    {
        private string sHeaderUOL;
        private PackNodeList pNodeList;
        private MemoryMappedFile pDataMappedMemFile;
        private ProgressWindow pProgress;

        public MainWindow()
        {
            InitializeComponent();

            this.pImagePanel.AutoScroll = true;

            this.pImageData.BorderStyle = BorderStyle.None;
            this.pImageData.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right);

            this.pMenuStrip.Renderer = new MenuRenderer();

            this.pPrevSize = this.Size;

            this.sHeaderUOL = "";

            this.pNodeList = null;
            this.pDataMappedMemFile = null;
            this.pProgress = null;

            this.UpdatePanel("Empty", null);
        }

        private void AddFileEntry(PackFileEntry pEntry)
        {
            PackNode pRoot = this.pTreeView.Nodes[0] as PackNode;

            if (pRoot != null)
            {
                PackStreamVerBase pStream = pRoot.Tag as PackStreamVerBase;

                if (pStream != null)
                {
                    pStream.GetFileList().Add(pEntry);
                }
            }
        }

        private void InitializeTree(PackStreamVerBase pStream)
        {
            // Insert the root node (file)
            string[] aPath = this.sHeaderUOL.Replace(".m2h", "").Split('/');
            this.pTreeView.Nodes.Add(new PackNode(pStream, aPath[aPath.Length - 1]));

            if (this.pNodeList != null)
            {
                this.pNodeList.InternalRelease();
            }
            this.pNodeList = new PackNodeList("/");

            foreach (PackFileEntry pEntry in pStream.GetFileList())
            {
                if (pEntry.Name.Contains("/"))
                {
                    string sPath = pEntry.Name;
                    PackNodeList pCurList = pNodeList;

                    while (sPath.Contains("/"))
                    {
                        string sDir = sPath.Substring(0, sPath.IndexOf('/') + 1);
                        if (!pCurList.Children.ContainsKey(sDir))
                        {
                            pCurList.Children.Add(sDir, new PackNodeList(sDir));
                            if (pCurList == pNodeList)
                            {
                                this.pTreeView.Nodes[0].Nodes.Add(new PackNode(pCurList.Children[sDir], sDir));
                            }
                        }
                        pCurList = pCurList.Children[sDir];

                        sPath = sPath.Substring(sPath.IndexOf('/') + 1);
                    }

                    pEntry.TreeName = sPath;
                    pCurList.Entries.Add(sPath, pEntry);
                } else
                {
                    pEntry.TreeName = pEntry.Name;

                    this.pNodeList.Entries.Add(pEntry.Name, pEntry);
                    this.pTreeView.Nodes[0].Nodes.Add(new PackNode(pEntry, pEntry.Name));
                }
            }

            // Sort all nodes
            this.pTreeView.Sort();
        }

        private void NotifyMessage(string sText, MessageBoxIcon eIcon = MessageBoxIcon.None)
        {
            MessageBox.Show(this, sText, this.Text, MessageBoxButtons.OK, eIcon);
        }

        private void OnAbout(object sender, EventArgs e)
        {
            About pAbout = new About
            {
                Owner = this
            };

            pAbout.ShowDialog();
        }

        private void OnChangeImage(object sender, EventArgs e)
        {
            if (!this.pChangeImageBtn.Visible)
            {
                return;
            }

            PackNode pNode = this.pTreeView.SelectedNode as PackNode;
            if (pNode != null && pNode.Data != null)
            {
                PackFileEntry pEntry = pNode.Tag as PackFileEntry;

                if (pEntry != null)
                {
                    string sExtension = pEntry.TreeName.Split('.')[1];

                    OpenFileDialog pDialog = new OpenFileDialog
                    {
                        Title = "Select the new image",
                        Filter = string.Format("{0} Image|*.{0}", sExtension.ToUpper()),
                        Multiselect = false
                    };

                    if (pDialog.ShowDialog() == DialogResult.OK)
                    {
                        byte[] pData = File.ReadAllBytes(pDialog.FileName);

                        if (pNode.Data != pData)
                        {
                            pEntry.Data = pData;
                            pEntry.Changed = true;

                            UpdatePanel(sExtension, pData);
                        }
                    }
                }
            }
        }

        private void OnChangeWindowSize(object sender, EventArgs e)
        {
            int nHeight = (this.Size.Height - this.pPrevSize.Height);
            int nWidth = (this.Size.Width - this.pPrevSize.Width);

            this.pTextData.Size = new Size
            {
                Height = this.pTextData.Height + nHeight,
                Width = this.pTextData.Width + nWidth
            };

            this.pImagePanel.Size = new Size
            {
                Height = this.pImagePanel.Height + nHeight,
                Width = this.pImagePanel.Width + nWidth
            };

            this.pTreeView.Size = new Size
            {
                Height = this.pTreeView.Height + nHeight,
                Width = this.pTreeView.Width
            };

            this.pEntryValue.Location = new Point
            {
                X = this.pEntryValue.Location.X + nWidth,
                Y = this.pEntryValue.Location.Y
            };

            this.pPrevSize = this.Size;
            this.pImageData.Size = this.pImagePanel.Size;

            RenderImageData(true);
        }

        private void OnCollapseNodes(object sender, EventArgs e)
        {
            this.pTreeView.CollapseAll();
        }

        private void OnCopyNode(object sender, EventArgs e)
        {
            PackNode pNode = this.pTreeView.SelectedNode as PackNode;

            if (pNode != null)
            {
                PackFileEntry pEntry = pNode.Tag as PackFileEntry;

                if (pEntry != null)
                {
                    // Clear any current data from clipboard.
                    Clipboard.Clear();
                    // Copy the new copied entry object to clipboard.
                    Clipboard.SetData(PackFileEntry.DATA_FORMAT, pEntry.CreateCopy());
                } else
                {
                    PackNodeList pList = pNode.Tag as PackNodeList;

                    if (pList != null)
                    {
                        // Currently, for memory effieciency's sake, we will only copy
                        // the entries of this directory, and not recursively copy all
                        // sub-directories and their own entries.

                        PackNodeList pListCopy = new PackNodeList(pList.Directory);
                        foreach (PackFileEntry pChild in pList.Entries.Values)
                        {
                            // Decrypt the data because we could potentially be moving
                            // the object across repacker instances.
                            byte[] pBlock = CryptoMan.DecryptData(pChild.FileHeader, this.pDataMappedMemFile);

                            // Add a completely cloned reference of this entry, with a
                            // new decrypted data block assigned and changed marked true.
                            pListCopy.Entries.Add(pChild.TreeName, pChild.CreateCopy(pBlock));
                        }

                        // Finally, copy the new node list to clipboard.
                        Clipboard.Clear();
                        Clipboard.SetData(PackNodeList.DATA_FORMAT, pListCopy);
                    }
                }
            } else
            {
                NotifyMessage("Please select the node you wish to copy.", MessageBoxIcon.Exclamation);
            }
        }

        private void OnDoubleClickNode(object sender, TreeNodeMouseClickEventArgs e)
        {
            PackNode pNode = pTreeView.SelectedNode as PackNode;
            if (pNode == null || pNode.Nodes.Count != 0)
                return;
            object pObj = pNode.Tag;

            if (pObj is PackNodeList)
            {
                PackNodeList pList = pObj as PackNodeList;

                // Iterate all further directories within the list
                foreach (KeyValuePair<string, PackNodeList> pChild in pList.Children)
                {
                    pNode.Nodes.Add(new PackNode(pChild.Value, pChild.Key));
                }

                // Iterate entries
                foreach (PackFileEntry pEntry in pList.Entries.Values)
                {
                    pNode.Nodes.Add(new PackNode(pEntry, pEntry.TreeName));
                }

                pNode.Expand();
            }
            /*else if (pObj is PackFileEntry)
            {
                PackFileEntry pEntry = pObj as PackFileEntry;
                PackFileHeaderVerBase pFileHeader = pEntry.FileHeader;

                if (pFileHeader != null)
                {
                    byte[] pBuffer = CryptoMan.DecryptData(pFileHeader, this.pDataMappedMemFile);

                    UpdatePanel(pEntry.TreeName.Split('.')[1].ToLower(), pBuffer);
                }
            }*/
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void OnExpandNodes(object sender, EventArgs e)
        {
            this.pTreeView.ExpandAll();
        }

        private void OnExport(object sender, EventArgs e)
        {
            PackNode pNode = this.pTreeView.SelectedNode as PackNode;

            if (pNode != null)
            {
                byte[] pData = pNode.Data;

                if (pData != null)
                {
                    PackFileEntry pEntry = pNode.Tag as PackFileEntry;
                    if (pEntry != null)
                    {
                        string sName = pEntry.TreeName.Split('.')[0];
                        string sExtension = pEntry.TreeName.Split('.')[1];

                        SaveFileDialog pDialog = new SaveFileDialog
                        {
                            Title = "Select the destination to export the file",
                            FileName = sName,
                            Filter = string.Format("{0} File|*.{1}", sExtension.ToUpper(), sExtension)
                        };

                        if (pDialog.ShowDialog() == DialogResult.OK)
                        {
                            File.WriteAllBytes(pDialog.FileName, pData);

                            NotifyMessage(string.Format("Successfully exported to {0}", pDialog.FileName), MessageBoxIcon.Information);
                        }
                    }
                }
            } else
            {
                NotifyMessage("Please select a file to export.", MessageBoxIcon.Asterisk);
            }
        }

        private void OnLoadFile(object sender, EventArgs e)
        {
            if (this.pNodeList != null)
            {
                NotifyMessage("Please unload the current file first.", MessageBoxIcon.Information);
                return;
            }

            OpenFileDialog pDialog = new OpenFileDialog()
            {
                Title = "Select the MS2 file to load",
                Filter = "MapleStory2 Files|*.m2d",
                Multiselect = false
            };

            if (pDialog.ShowDialog() == DialogResult.OK)
            {
                string sDataUOL = Dir_BackSlashToSlash(pDialog.FileName);
                this.sHeaderUOL = sDataUOL.Replace(".m2d", ".m2h");

                if (!File.Exists(this.sHeaderUOL))
                {
                    string sHeaderName = this.sHeaderUOL.Substring(this.sHeaderUOL.LastIndexOf('/') + 1);
                    NotifyMessage(string.Format("Unable to load the {0} file.\r\nPlease make sure it exists and is not being used.", sHeaderName), MessageBoxIcon.Error);
                    return;
                }

                PackStreamVerBase pStream;
                using (BinaryReader pHeader = new BinaryReader(File.OpenRead(this.sHeaderUOL)))
                {
                    // Construct a new packed stream from the header data
                    pStream = PackVer.CreatePackVer(pHeader);

                    // Insert a collection containing the file list information [index,hash,name]
                    pStream.GetFileList().Clear();
                    pStream.GetFileList().AddRange(PackFileEntry.CreateFileList(Encoding.UTF8.GetString(CryptoMan.DecryptFileString(pStream, pHeader.BaseStream))));
                    // Make the collection of files sorted by their FileIndex for easy fetching
                    pStream.GetFileList().Sort();
                    
                    // Load the file allocation table and assign each file header to the entry within the list
                    byte[] pFileTable = CryptoMan.DecryptFileTable(pStream, pHeader.BaseStream);
                    using (MemoryStream pTableStream = new MemoryStream(pFileTable))
                    {
                        using (BinaryReader pReader = new BinaryReader(pTableStream))
                        {
                            PackFileHeaderVerBase pFileHeader;

                            switch (pStream.GetVer())
                            {
                                case PackVer.MS2F:
                                    for (ulong i = 0; i < pStream.GetFileListCount(); i++)
                                    {
                                        pFileHeader = new PackFileHeaderVer1(pReader);
                                        pStream.GetFileList()[pFileHeader.GetFileIndex() - 1].FileHeader = pFileHeader;
                                    }
                                    break;
                                case PackVer.NS2F:
                                    for (ulong i = 0; i < pStream.GetFileListCount(); i++)
                                    {
                                        pFileHeader = new PackFileHeaderVer2(pReader);
                                        pStream.GetFileList()[pFileHeader.GetFileIndex() - 1].FileHeader = pFileHeader;
                                    }
                                    break;
                                case PackVer.OS2F:
                                case PackVer.PS2F:
                                    for (ulong i = 0; i < pStream.GetFileListCount(); i++)
                                    {
                                        pFileHeader = new PackFileHeaderVer3(pStream.GetVer(), pReader);
                                        pStream.GetFileList()[pFileHeader.GetFileIndex() - 1].FileHeader = pFileHeader;
                                    }
                                    break;
                            }
                        }
                    }
                }

                this.pDataMappedMemFile = MemoryMappedFile.CreateFromFile(sDataUOL);

                InitializeTree(pStream);
            }
        }

        private void OnPasteNode(object sender, EventArgs e)
        {
            IDataObject pData = Clipboard.GetDataObject();
            if (pData == null)
            {
                return;
            }

            PackNode pNode = this.pTreeView.SelectedNode as PackNode;
            if (pNode != null)
            {
                if (pNode.Tag is PackFileEntry) //wtf are they thinking?
                {
                    NotifyMessage("Please select a directory to paste into!", MessageBoxIcon.Exclamation);
                    return;
                }

                object pObj;
                if (pData.GetDataPresent(PackFileEntry.DATA_FORMAT))
                {
                    pObj = (PackFileEntry)pData.GetData(PackFileEntry.DATA_FORMAT);
                }
                else if (pData.GetDataPresent(PackNodeList.DATA_FORMAT))
                {
                    pObj = (PackNodeList)pData.GetData(PackNodeList.DATA_FORMAT);
                } else
                {
                    NotifyMessage("No files or directories are currently copied to clipboard.", MessageBoxIcon.Exclamation);
                    return;
                }

                PackNodeList pList;
                if (pNode.Level == 0)
                {
                    // If they're trying to add to the root of the file,
                    // then just use the root node list of this tree.
                    pList = this.pNodeList;
                } else
                {
                    pList = pNode.Tag as PackNodeList;
                }

                if (pList != null && pObj != null)
                {
                    if (pObj is PackFileEntry)
                    {
                        PackFileEntry pEntry = pObj as PackFileEntry;

                        AddFileEntry(pEntry);
                        pList.Entries.Add(pEntry.TreeName, pEntry);

                        PackNode pChild = new PackNode(pEntry, pEntry.TreeName);
                        pNode.Nodes.Add(pChild);

                        pEntry.Name = pChild.Path;
                    } else if (pObj is PackNodeList)
                    {
                        PackNodeList pChildList = pObj as PackNodeList;

                        PackNode pChild = new PackNode(pChildList, pChildList.Directory);
                        pList.Children.Add(pChildList.Directory, pChildList);
                        pNode.Nodes.Add(pChild);

                        foreach (PackFileEntry pEntry in pChildList.Entries.Values)
                        {
                            AddFileEntry(pEntry);
                            PackNode pListNode = new PackNode(pEntry, pEntry.TreeName);
                            pChild.Nodes.Add(pListNode);

                            pEntry.Name = pListNode.Path;
                        }
                    }
                }
            }
        }

        private void OnReloadFile(object sender, EventArgs e)
        {
            if (this.pNodeList != null)
            {
                PackStreamVerBase pStream;

                if (this.pTreeView.Nodes.Count > 0)
                {
                    pStream = this.pTreeView.Nodes[0].Tag as PackStreamVerBase;
                    if (pStream == null)
                    {
                        return;
                    }

                    this.pTreeView.Nodes.Clear();
                    this.pTreeView.Refresh();

                    InitializeTree(pStream);
                    UpdatePanel("Empty", null);
                }
            } else
            {
                NotifyMessage("There is no package to be reloaded.", MessageBoxIcon.Warning);
            }
        }

        private void OnRemoveFile(object sender, EventArgs e)
        {
            PackNode pNode = this.pTreeView.SelectedNode as PackNode;
            if (pNode != null)
            {
                PackNode pRoot = this.pTreeView.Nodes[0] as PackNode;
                if (pRoot != null && pNode != pRoot)
                {
                    PackStreamVerBase pStream = pRoot.Tag as PackStreamVerBase;
                    if (pStream != null)
                    {
                        PackFileEntry pEntry = pNode.Tag as PackFileEntry;
                        if (pEntry != null)
                        {
                            pStream.GetFileList().Remove(pEntry);

                            if (pNode.Parent == pRoot as TreeNode)
                            {
                                this.pNodeList.Entries.Remove(pEntry.TreeName);
                            } else
                            {
                                (pNode.Parent.Tag as PackNodeList).Entries.Remove(pEntry.TreeName);
                            }
                            pNode.Parent.Nodes.Remove(pNode);
                        } else if (pNode.Tag is PackNodeList)
                        {
                            string sWarning = "WARNING: You are about to delete an entire directory!"
                                    + "\r\nBy deleting this directory, all inner directories and entries will also be removed."
                                    + "\r\n\r\nAre you sure you want to continue?";
                            if (MessageBox.Show(this, sWarning, this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                            {
                                // Recursively remove all inner directories and entries.
                                RemoveDirectory(pNode, pStream);
                                // Remove the entire node list child from the parent list.
                                if (pNode.Parent == pRoot as TreeNode)
                                {
                                    this.pNodeList.Children.Remove(pNode.Name);
                                } else
                                {
                                    (pNode.Parent.Tag as PackNodeList).Children.Remove(pNode.Name);
                                }
                                // Remove the node and all of its children from tree.
                                pNode.Remove();
                            }
                        }
                    }
                }
            } else
            {
                NotifyMessage("Please select a file or directory to remove.", MessageBoxIcon.Exclamation);
            }
        }

        private void OnSaveBegin(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker pWorker = sender as BackgroundWorker;

            if (pWorker != null)
            {
                PackStreamVerBase pStream = this.pProgress.Stream;
                if (pStream == null)
                {
                    return;
                }
                // Start the elapsed time progress stopwatch
                this.pProgress.Start();

                // Re-calculate the file list in case of index removal
                pStream.GetFileList().Sort();

                // Save the data blocks to file and re-calculate all entries
                SaveData(this.pProgress.Path, pStream.GetFileList());

                // Declare the new file count (header update)
                uint dwFileCount = (uint)pStream.GetFileList().Count;

                // Construct a raw string containing the new file list information
                StringBuilder sFileString = new StringBuilder();
                foreach (PackFileEntry pEntry in pStream.GetFileList())
                {
                    sFileString.Append(pEntry.ToString());
                }
                this.pSaveWorkerThread.ReportProgress(96);

                // Encrypt the file list and output the new header sizes (header update)
                byte[] pFileString = Encoding.UTF8.GetBytes(sFileString.ToString().ToCharArray());
                byte[] pHeader = CryptoMan.Encrypt(pStream.GetVer(), pFileString, BufferManipulation.AES_ZLIB, out uint uHeaderLen, out uint uCompressedHeaderLen, out uint uEncodedHeaderLen);
                this.pSaveWorkerThread.ReportProgress(97);

                // Construct a new file allocation table
                byte[] pFileTable;
                using (MemoryStream pOutStream = new MemoryStream())
                {
                    using (BinaryWriter pWriter = new BinaryWriter(pOutStream))
                    {
                        foreach (PackFileEntry pEntry in pStream.GetFileList())
                        {
                            pEntry.FileHeader.Encode(pWriter);
                        }
                    }
                    pFileTable = pOutStream.ToArray();
                }
                this.pSaveWorkerThread.ReportProgress(98);

                // Encrypt the file table and output the new data sizes (header update)
                pFileTable = CryptoMan.Encrypt(pStream.GetVer(), pFileTable, BufferManipulation.AES_ZLIB, out uint uDataLen, out uint uCompressedDataLen, out uint uEncodedDataLen);
                this.pSaveWorkerThread.ReportProgress(99);

                // Update all header sizes to the new file list information
                pStream.SetFileListCount(dwFileCount);
                pStream.SetHeaderSize(uHeaderLen);
                pStream.SetCompressedHeaderSize(uCompressedHeaderLen);
                pStream.SetEncodedHeaderSize(uEncodedHeaderLen);
                pStream.SetDataSize(uDataLen);
                pStream.SetCompressedDataSize(uCompressedDataLen);
                pStream.SetEncodedDataSize(uEncodedDataLen);

                // Write the new header data to stream
                using (BinaryWriter pWriter = new BinaryWriter(File.Create(this.pProgress.Path.Replace(".m2d", ".m2h"))))
                {
                    // Encode the file version (MS2F,NS2F,etc)
                    pWriter.Write(pStream.GetVer());

                    // Encode the stream header information
                    pStream.Encode(pWriter);

                    // Encode the encrypted header and file table buffers
                    pWriter.Write(pHeader);
                    pWriter.Write(pFileTable);
                }
                this.pSaveWorkerThread.ReportProgress(100);
            }
        }

        private void OnSaveChanges(object sender, EventArgs e)
        {
            if (!this.pUpdateDataBtn.Visible)
            {
                return;
            }

            PackNode pNode = this.pTreeView.SelectedNode as PackNode;
            if (pNode != null && pNode.Data != null)
            {
                PackFileEntry pEntry = pNode.Tag as PackFileEntry;
                
                if (pEntry != null)
                {
                    string sData = this.pTextData.Text;
                    byte[] pData = Encoding.UTF8.GetBytes(sData.ToCharArray());

                    if (pNode.Data != pData)
                    {
                        pEntry.Data = pData;
                        pEntry.Changed = true;
                    }
                }
            }
        }

        private void OnSaveComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(this.pProgress, e.Error.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.pProgress.Finish();
            this.pProgress.Close();
            
            TimeSpan pInterval = TimeSpan.FromMilliseconds(this.pProgress.ElapsedTime);
            NotifyMessage(string.Format("Successfully saved in {0} minutes and {1} seconds!", pInterval.Minutes, pInterval.Seconds), MessageBoxIcon.Information);

            // Perform heavy cleanup
            System.GC.Collect();
        }

        private void OnSaveFile(object sender, EventArgs e)
        {
            PackNode pNode = this.pTreeView.SelectedNode as PackNode;

            if (pNode != null && pNode.Tag is PackStreamVerBase)
            {
                SaveFileDialog pDialog = new SaveFileDialog
                {
                    Title = "Select the destination to save the file",
                    Filter = "MapleStory2 Files|*.m2d"
                };

                if (pDialog.ShowDialog() == DialogResult.OK)
                {
                    string sPath = Dir_BackSlashToSlash(pDialog.FileName);

                    if (!pSaveWorkerThread.IsBusy)
                    {
                        this.pProgress = new ProgressWindow
                        {
                            Path = sPath,
                            Stream = (pNode.Tag as PackStreamVerBase)
                        };
                        this.pProgress.Show(this);
                        // Why do you make this so complicated C#? 
                        int x = this.DesktopBounds.Left + (this.Width - this.pProgress.Width) / 2;
                        int y = this.DesktopBounds.Top + (this.Height - this.pProgress.Height) / 2;
                        this.pProgress.SetDesktopLocation(x, y);

                        this.pSaveWorkerThread.RunWorkerAsync();
                    }
                }
            } else
            {
                NotifyMessage("Please select a Packed Data File file to save.", MessageBoxIcon.Information);
            }
        }

        private void OnSaveProgress(object sender, ProgressChangedEventArgs e)
        {
            this.pProgress.UpdateProgressBar(e.ProgressPercentage);
        }

        private void OnSearch(object sender, EventArgs e)
        {
            // TODO: Implement Scintilla SearchManager
            NotifyMessage("Coming Soon in a future update!", MessageBoxIcon.Information);
        }

        private void OnSelectNode(object sender, TreeViewEventArgs e)
        {
            PackNode pNode = this.pTreeView.SelectedNode as PackNode;

            if (pNode != null)
            {
                object pObj = pNode.Tag;

                this.pEntryName.Visible = true;
                this.pEntryName.Text = pNode.Name;

                if (pObj is PackNodeList)
                {
                    UpdatePanel("Packed Directory", null);
                } else if (pObj is PackFileEntry)
                {
                    PackFileEntry pEntry = pObj as PackFileEntry;
                    PackFileHeaderVerBase pFileHeader = pEntry.FileHeader;

                    if (pFileHeader != null)
                    {
                        if (pNode.Data == null)
                        {
                            // TODO: Improve memory efficiency here and dispose of the data if
                            // it's unchanged once they select a different node in the tree.
                            pNode.Data = CryptoMan.DecryptData(pFileHeader, this.pDataMappedMemFile);
                        }
                    }

                    UpdatePanel(pEntry.TreeName.Split('.')[1].ToLower(), pNode.Data);
                }
                else if (pObj is PackStreamVerBase)
                {
                    UpdatePanel("Packed Data File", null);
                } else
                {
                    UpdatePanel("Empty", null);
                }
            }
        }

        private void OnUnloadFile(object sender, EventArgs e)
        {
            if (this.pNodeList != null)
            {
                pTreeView.Nodes.Clear();

                this.pNodeList.InternalRelease();
                this.pNodeList = null;

                this.sHeaderUOL = "";

                if (this.pDataMappedMemFile != null)
                {
                    this.pDataMappedMemFile.Dispose();
                    this.pDataMappedMemFile = null;
                }

                this.UpdatePanel("Empty", null);

                System.GC.Collect();
            } else
            {
                NotifyMessage("There is no package to be unloaded.", MessageBoxIcon.Warning);
            }
        }

        private void OnWindowClosing(object sender, FormClosingEventArgs e)
        {
            // Only ask for confirmation when the user has files open.
            if (this.pTreeView.Nodes.Count > 0)
            {
                if (MessageBox.Show(this, "Are you sure you want to exit?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void RemoveDirectory(PackNode pNode, PackStreamVerBase pStream)
        {
            if (pNode.Nodes.Count == 0)
            {
                if (pNode.Tag is PackNodeList)
                {
                    PackNodeList pList = pNode.Tag as PackNodeList;

                    foreach (KeyValuePair<string, PackNodeList> pChild in pList.Children)
                    {
                        pNode.Nodes.Add(new PackNode(pChild.Value, pChild.Key));
                    }

                    foreach (PackFileEntry pEntry in pList.Entries.Values)
                    {
                        pNode.Nodes.Add(new PackNode(pEntry, pEntry.TreeName));
                    }

                    pList.Children.Clear();
                    pList.Entries.Clear();
                }
            }

            foreach (PackNode pChild in pNode.Nodes)
            {
                RemoveDirectory(pChild, pStream);
            }

            if (pNode.Tag is PackFileEntry)
            {
                pStream.GetFileList().Remove(pNode.Tag as PackFileEntry);
            }
        }

        private void RenderImageData(bool bChange)
        {
            this.pImageData.Visible = this.pImagePanel.Visible;

            if (this.pImageData.Visible)
            {
                // If the size of the bitmap image is bigger than the actual panel,
                // then we adjust the image sizing mode to zoom the image in order
                // to fit the full image within the current size of the panel.
                if (this.pImageData.Image.Size.Height > this.pImagePanel.Size.Height || this.pImageData.Image.Size.Width > this.pImagePanel.Size.Width)
                {
                    // If we went from selecting a small image to selecting a big image,
                    // then adjust the panel and data to fit the size of the new bitmap.
                    if (!bChange)
                    {
                        this.OnChangeWindowSize(null, null);
                    }

                    // Since the image is too big, scale it in zoom mode to fit it.
                    this.pImageData.SizeMode = PictureBoxSizeMode.Zoom;
                } else
                {
                    // Since the image is less than or equal to the size of the panel,
                    // we are able to render the image as-is with no additional scaling.
                    this.pImageData.SizeMode = PictureBoxSizeMode.Normal;
                }

                // Render the new size changes.
                this.pImageData.Update();
            }
        }

        private void SaveData(string sDataPath, List<PackFileEntry> aEntry)
        {
            List<PackFileEntry> aNewEntry = new List<PackFileEntry>();

            // Declare MS2F as the initial version until specified.
            uint uVer = PackVer.MS2F;
            // Re-calculate all file offsets from start to finish
            ulong uOffset = 0;
            // Re-calculate all file indexes from start to finish
            int nCurIndex = 1;

            using (BinaryWriter pWriter = new BinaryWriter(File.Create(sDataPath)))
            {
                // Iterate all file entries that exist
                foreach (PackFileEntry pEntry in aEntry)
                {
                    PackFileHeaderVerBase pHeader = pEntry.FileHeader;

                    // If the entry was modified, or is new, write the modified data block
                    if (pEntry.Changed)
                    {
                        // If the header is null (new entry), then create one
                        if (pHeader == null)
                        {
                            // Hacky way of doing this, but this follows Nexon's current conventions.
                            uint dwBufferFlag;
                            if (pEntry.Name.EndsWith(".usm"))
                            {
                                dwBufferFlag = BufferManipulation.XOR;
                            } else if (pEntry.Name.EndsWith(".png"))
                            {
                                dwBufferFlag = BufferManipulation.AES;
                            } else
                            {
                                dwBufferFlag = BufferManipulation.AES_ZLIB;
                            }

                            switch (uVer)
                            {
                                case PackVer.MS2F:
                                    pHeader = PackFileHeaderVer1.CreateHeader(nCurIndex, dwBufferFlag, uOffset, pEntry.Data);
                                    break;
                                case PackVer.NS2F:
                                    pHeader = PackFileHeaderVer2.CreateHeader(nCurIndex, dwBufferFlag, uOffset, pEntry.Data);
                                    break;
                                case PackVer.OS2F:
                                case PackVer.PS2F:
                                    pHeader = PackFileHeaderVer3.CreateHeader(uVer, nCurIndex, dwBufferFlag, uOffset, pEntry.Data);
                                    break;
                            }
                            // Update the entry's file header to the newly created one
                            pEntry.FileHeader = pHeader;
                        }
                        else
                        {
                            // If the header existed already, re-calculate the file index and offset.
                            pHeader.SetFileIndex(nCurIndex);
                            pHeader.SetOffset(uOffset);
                        }

                        // Encrypt the new data block and output the header size data
                        pWriter.Write(CryptoMan.Encrypt(uVer, pEntry.Data, pEntry.FileHeader.GetBufferFlag(), out uint uLen, out uint uCompressed, out uint uEncoded));

                        // Apply the file size changes from the new buffer
                        pHeader.SetFileSize(uLen);
                        pHeader.SetCompressedFileSize(uCompressed);
                        pHeader.SetEncodedFileSize(uEncoded);

                        // Update the Entry's index to the new current index
                        pEntry.Index = nCurIndex;

                        nCurIndex++;
                        uOffset += pHeader.GetEncodedFileSize();
                    }
                    // If the entry is unchanged, parse the block from the original offsets
                    else
                    {
                        // Make sure the entry has a parsed file header from load
                        if (pHeader != null)
                        {
                            // Update the initial versioning before any future crypto calls
                            if (pHeader.GetVer() != uVer)
                            {
                                uVer = pHeader.GetVer();
                            }

                            // Access the current encrypted block data from the memory map initially loaded
                            using (MemoryMappedViewStream pBuffer = this.pDataMappedMemFile.CreateViewStream((long)pHeader.GetOffset(), (long)pHeader.GetEncodedFileSize()))
                            {
                                byte[] pSrc = new byte[pHeader.GetEncodedFileSize()];

                                if (pBuffer.Read(pSrc, 0, (int)pHeader.GetEncodedFileSize()) == pHeader.GetEncodedFileSize())
                                {
                                    // Modify the header's file index to the updated offset after entry changes
                                    pHeader.SetFileIndex(nCurIndex);
                                    // Modify the header's offset to the updated offset after entry changes
                                    pHeader.SetOffset(uOffset);
                                    // Write the original (completely encrypted) block of data to file
                                    pWriter.Write(pSrc);

                                    // Update the Entry's index to the new current index
                                    pEntry.Index = nCurIndex;

                                    nCurIndex++;
                                    uOffset += pHeader.GetEncodedFileSize();
                                }
                            }
                        }
                    }
                    // Allow the remaining 5% for header file write progression
                    this.pSaveWorkerThread.ReportProgress((int)(((double)(nCurIndex - 1) / (double)aEntry.Count) * 95.0d));
                }
            }
        }

        private void UpdatePanel(string sExtension, byte[] pBuffer)
        {
            if (pBuffer == null)
            {
                this.pEntryValue.Text = sExtension;
                this.pEntryName.Visible = false;
            } else
            {
                this.pEntryValue.Text = string.Format("{0} File", sExtension.ToUpper());
                this.pEntryName.Visible = true;
            }

            this.pTextData.Visible = (sExtension.Equals("ini") || sExtension.Equals("nt") || sExtension.Equals("lua")
                || sExtension.Equals("xml") || sExtension.Equals("flat") || sExtension.Equals("xblock") 
                || sExtension.Equals("diagram") || sExtension.Equals("preset") || sExtension.Equals("emtproj"));
            this.pUpdateDataBtn.Visible = this.pTextData.Visible;

            this.pImagePanel.Visible = (sExtension.Equals("png") || sExtension.Equals("dds"));
            this.pChangeImageBtn.Visible = this.pImagePanel.Visible;

            if (this.pTextData.Visible)
            {
                this.pTextData.Text = Encoding.UTF8.GetString(pBuffer);
            } else if (this.pImagePanel.Visible)
            {
                Bitmap pImage;
                if (sExtension.Equals("png"))
                {
                    using (MemoryStream pStream = new MemoryStream(pBuffer))
                    {
                        pImage = new Bitmap(pStream);
                    }
                } else //if (sExtension.Equals("dds"))
                {
                    pImage = DDS.LoadImage(pBuffer);
                }

                this.pImageData.Image = pImage;
            }

            /*
             * TODO:
             * *.nif, *.kf, and *.kfm files
             * Shaders/*.fxo - directx shader files?
             * PrecomputedTerrain/*.tok - mesh3d files? token files?
             * Gfx/*.gfx - graphics gen files?
             * Precompiled/luapack.o - object files?
            */

            UpdateStyle(sExtension);
            RenderImageData(false);
        }

        private void UpdateStyle(string sExtension)
        {
            // Reset the styles
            this.pTextData.StyleResetDefault();
            this.pTextData.Styles[Style.Default].Font = "Consolas";
            this.pTextData.Styles[Style.Default].Size = 10;
            this.pTextData.Styles[Style.Default].BackColor = Color.FromArgb(0x282923);
            this.pTextData.StyleClearAll();

            if (sExtension.Equals("ini") || sExtension.Equals("nt"))
            {
                // Set the Styles to replicate Sublime
                this.pTextData.StyleResetDefault();
                this.pTextData.Styles[Style.Default].Font = "Consolas";
                this.pTextData.Styles[Style.Default].Size = 10;
                this.pTextData.Styles[Style.Default].BackColor = Color.FromArgb(0x282923);
                this.pTextData.Styles[Style.Default].ForeColor = Color.White;
                this.pTextData.StyleClearAll();
            } else if (sExtension.Equals("lua"))
            {
                // Extracted from the Lua Scintilla lexer and SciTE .properties file

                var alphaChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
                var numericChars = "0123456789";
                var accentedChars = "ŠšŒœŸÿÀàÁáÂâÃãÄäÅåÆæÇçÈèÉéÊêËëÌìÍíÎîÏïÐðÑñÒòÓóÔôÕõÖØøÙùÚúÛûÜüÝýÞþßö";

                // Configuring the default style with properties
                // we have common to every lexer style saves time.
                this.pTextData.StyleResetDefault();
                this.pTextData.Styles[Style.Default].Font = "Consolas";
                this.pTextData.Styles[Style.Default].Size = 10;
                this.pTextData.Styles[Style.Default].BackColor = Color.FromArgb(0x282923);
                this.pTextData.Styles[Style.Default].ForeColor = Color.White;
                this.pTextData.StyleClearAll();

                // Configure the Lua lexer styles
                this.pTextData.Styles[Style.Lua.Default].ForeColor = Color.Silver;
                this.pTextData.Styles[Style.Lua.Comment].ForeColor = Color.FromArgb(0xD8D8D8);
                this.pTextData.Styles[Style.Lua.CommentLine].ForeColor = Color.FromArgb(0xD8D8D8);
                this.pTextData.Styles[Style.Lua.Number].ForeColor = Color.FromArgb(0xC48CFF);
                this.pTextData.Styles[Style.Lua.Word].ForeColor = Color.FromArgb(0xFF007F);
                this.pTextData.Styles[Style.Lua.Word2].ForeColor = Color.BlueViolet;
                this.pTextData.Styles[Style.Lua.Word3].ForeColor = Color.FromArgb(0x52E3F6);
                this.pTextData.Styles[Style.Lua.Word4].ForeColor = Color.FromArgb(0x52E3F6);
                this.pTextData.Styles[Style.Lua.String].ForeColor = Color.FromArgb(0xECE47E);
                this.pTextData.Styles[Style.Lua.Character].ForeColor = Color.FromArgb(0xECE47E);
                this.pTextData.Styles[Style.Lua.LiteralString].ForeColor = Color.FromArgb(0xECE47E);
                this.pTextData.Styles[Style.Lua.StringEol].BackColor = Color.Pink;
                this.pTextData.Styles[Style.Lua.Operator].ForeColor = Color.White;
                this.pTextData.Styles[Style.Lua.Preprocessor].ForeColor = Color.Maroon;
                this.pTextData.Lexer = Lexer.Lua;
                this.pTextData.WordChars = alphaChars + numericChars + accentedChars;

                // Keywords
                this.pTextData.SetKeywords(0, "and break do else elseif end for function if in local nil not or repeat return then until while" + " false true" + " goto");
                // Basic Functions
                this.pTextData.SetKeywords(1, "assert collectgarbage dofile error _G getmetatable ipairs loadfile next pairs pcall print rawequal rawget rawset setmetatable tonumber tostring type _VERSION xpcall string table math coroutine io os debug" + " getfenv gcinfo load loadlib loadstring require select setfenv unpack _LOADED LUA_PATH _REQUIREDNAME package rawlen package bit32 utf8 _ENV");
                // String Manipulation & Mathematical
                this.pTextData.SetKeywords(2, "string.byte string.char string.dump string.find string.format string.gsub string.len string.lower string.rep string.sub string.upper table.concat table.insert table.remove table.sort math.abs math.acos math.asin math.atan math.atan2 math.ceil math.cos math.deg math.exp math.floor math.frexp math.ldexp math.log math.max math.min math.pi math.pow math.rad math.random math.randomseed math.sin math.sqrt math.tan" + " string.gfind string.gmatch string.match string.reverse string.pack string.packsize string.unpack table.foreach table.foreachi table.getn table.setn table.maxn table.pack table.unpack table.move math.cosh math.fmod math.huge math.log10 math.modf math.mod math.sinh math.tanh math.maxinteger math.mininteger math.tointeger math.type math.ult" + " bit32.arshift bit32.band bit32.bnot bit32.bor bit32.btest bit32.bxor bit32.extract bit32.replace bit32.lrotate bit32.lshift bit32.rrotate bit32.rshift" + " utf8.char utf8.charpattern utf8.codes utf8.codepoint utf8.len utf8.offset");
                // Input and Output Facilities and System Facilities
                this.pTextData.SetKeywords(3, "coroutine.create coroutine.resume coroutine.status coroutine.wrap coroutine.yield io.close io.flush io.input io.lines io.open io.output io.read io.tmpfile io.type io.write io.stdin io.stdout io.stderr os.clock os.date os.difftime os.execute os.exit os.getenv os.remove os.rename os.setlocale os.time os.tmpname" + " coroutine.isyieldable coroutine.running io.popen module package.loaders package.seeall package.config package.searchers package.searchpath" + " require package.cpath package.loaded package.loadlib package.path package.preload");

                // Instruct the lexer to calculate folding
                this.pTextData.SetProperty("fold", "1");
                this.pTextData.SetProperty("fold.compact", "1");

                // Configure a margin to display folding symbols
                this.pTextData.Margins[2].Type = MarginType.Symbol;
                this.pTextData.Margins[2].Mask = Marker.MaskFolders;
                this.pTextData.Margins[2].Sensitive = true;
                this.pTextData.Margins[2].Width = 20;

                // Set colors for all folding markers
                for (int i = 25; i <= 31; i++)
                {
                    this.pTextData.Markers[i].SetForeColor(SystemColors.ControlLightLight);
                    this.pTextData.Markers[i].SetBackColor(SystemColors.ControlDark);
                }

                // Configure folding markers with respective symbols
                this.pTextData.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
                this.pTextData.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
                this.pTextData.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
                this.pTextData.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
                this.pTextData.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
                this.pTextData.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
                this.pTextData.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

                // Enable automatic folding
                this.pTextData.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);
            } else
            {
                // Set the XML Lexer
                this.pTextData.Lexer = Lexer.Xml;

                // Show line numbers
                this.pTextData.Margins[0].Width = 20;

                // Enable folding
                this.pTextData.SetProperty("fold", "1");
                this.pTextData.SetProperty("fold.compact", "1");
                this.pTextData.SetProperty("fold.html", "1");

                // Use Margin 2 for fold markers
                this.pTextData.Margins[2].Type = MarginType.Symbol;
                this.pTextData.Margins[2].Mask = Marker.MaskFolders;
                this.pTextData.Margins[2].Sensitive = true;
                this.pTextData.Margins[2].Width = 20;

                // Reset folder markers
                for (int i = Marker.FolderEnd; i <= Marker.FolderOpen; i++)
                {
                    this.pTextData.Markers[i].SetForeColor(SystemColors.ControlLightLight);
                    this.pTextData.Markers[i].SetBackColor(SystemColors.ControlDark);
                }

                // Style the folder markers
                this.pTextData.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
                this.pTextData.Markers[Marker.Folder].SetBackColor(SystemColors.ControlText);
                this.pTextData.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
                this.pTextData.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
                this.pTextData.Markers[Marker.FolderEnd].SetBackColor(SystemColors.ControlText);
                this.pTextData.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
                this.pTextData.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
                this.pTextData.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
                this.pTextData.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

                // Enable automatic folding
                this.pTextData.AutomaticFold = AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change;

                // Set the Styles to replicate Sublime
                this.pTextData.StyleResetDefault();
                this.pTextData.Styles[Style.Default].Font = "Courier";
                this.pTextData.Styles[Style.Default].Size = 10;
                this.pTextData.Styles[Style.Default].BackColor = Color.FromArgb(0x282923);
                this.pTextData.StyleClearAll();
                this.pTextData.Styles[Style.Xml.XmlStart].ForeColor = Color.White;
                this.pTextData.Styles[Style.Xml.XmlEnd].ForeColor = Color.White;
                this.pTextData.Styles[Style.Xml.Other].ForeColor = Color.White;
                this.pTextData.Styles[Style.Xml.Attribute].ForeColor = Color.FromArgb(0xA7EC21);
                this.pTextData.Styles[Style.Xml.Entity].ForeColor = Color.FromArgb(0xA7EC21);
                this.pTextData.Styles[Style.Xml.Comment].ForeColor = Color.FromArgb(0xD8D8D8);
                this.pTextData.Styles[Style.Xml.Tag].ForeColor = Color.FromArgb(0xFF007F);
                this.pTextData.Styles[Style.Xml.TagEnd].ForeColor = Color.FromArgb(0xFF007F);
                this.pTextData.Styles[Style.Xml.DoubleString].ForeColor = Color.FromArgb(0xECE47E);
                this.pTextData.Styles[Style.Xml.SingleString].ForeColor = Color.FromArgb(0xECE47E);
            }
        }

        private static string Dir_BackSlashToSlash(string sDir)
        {
            while (sDir.Contains("\\"))
            {
                sDir = sDir.Replace("\\", "/");
            }
            return sDir;
        }
    }
}
