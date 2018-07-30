using Orion.Crypto;
using Orion.Crypto.Common;
using Orion.Crypto.Stream;
using Orion.Crypto.Stream.DDS;
using Orion.Crypto.Stream.zlib;
using Orion.Window.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Windows.Forms;

namespace Orion.Window
{
    public partial class MainWindow : Form
    {
        private string sHeaderUOL;
        private string sDataUOL;
        private PackNodeList pNodeList;
        private MemoryMappedFile pDataMappedMemFile;

        public MainWindow()
        {
            InitializeComponent();

            this.panel1.AutoScroll = true;

            this.pImageData.BorderStyle = BorderStyle.None;
            this.pImageData.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right);

            this.pMenuStrip.Renderer = new MenuRenderer();

            this.pPrevSize = this.Size;
            this.OnChangeWindowSize(null, null);

            this.sHeaderUOL = "";
            this.sDataUOL = "";

            this.pNodeList = null;
            this.pDataMappedMemFile = null;
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
            this.pNodeList = new PackNodeList();

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
                            pCurList.Children.Add(sDir, new PackNodeList());
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

        private void OnChangeWindowSize(object sender, EventArgs e)
        {
            int nHeight = (this.Size.Height - this.pPrevSize.Height);
            int nWidth = (this.Size.Width - this.pPrevSize.Width);

            this.panel1.Size = new Size
            {
                Height = this.panel1.Height + nHeight,
                Width = this.panel1.Width + nWidth
            };

            RenderImageData();

            this.pTreeView.Size = new Size
            {
                Height = this.pTreeView.Height + nHeight,
                Width = this.pTreeView.Width
            };

            this.pPrevSize = this.Size;
        }

        private void OnCollapseNodes(object sender, EventArgs e)
        {
            this.pTreeView.CollapseAll();
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
            else if (pObj is PackFileEntry)
            {
                PackFileEntry pEntry = pObj as PackFileEntry;
                PackFileHeaderVerBase pFileHeader = pEntry.FileHeader;

                if (pFileHeader != null)
                {
                    byte[] pBuffer = CryptoMan.DecryptData(pFileHeader, this.pDataMappedMemFile);

                    UpdatePanel(pEntry.TreeName.Split('.')[1].ToLower(), pBuffer);
                }
            }
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void OnExpandNodes(object sender, EventArgs e)
        {
            // Should we ever expand all unselected nodes? It'd be awful to parse so many at once :/
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
                            Filter = string.Format("{0} File (*.{1})|*.{1}", sExtension.ToUpper(), sExtension)
                        };

                        if (pDialog.ShowDialog() == DialogResult.OK)
                        {
                            File.WriteAllBytes(pDialog.FileName, pData);
                        }
                    }
                }
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
                Filter = "All Extensions (*.m2h, *.m2d)|*.m2h;*.m2d|Header Files (*.m2h)|*.m2h|Data Files (*.m2d)|*.m2d",
                Multiselect = false
            };

            if (pDialog.ShowDialog() == DialogResult.OK)
            {
                string sUOL = Dir_BackSlashToSlash(pDialog.FileName);

                this.sHeaderUOL = sUOL.EndsWith(".m2h") ? sUOL : sUOL.Replace(".m2d", ".m2h");
                this.sDataUOL = sUOL.EndsWith(".m2h") ? sUOL.Replace(".m2h", ".m2d") : sUOL;

                PackStreamVerBase pStream;
                using (BinaryReader pHeader = new BinaryReader(File.OpenRead(this.sHeaderUOL)))
                {
                    // Construct a new packed stream from the header data
                    pStream = PackVer.CreatePackVer(pHeader);

                    // Insert a collection containing the file list information [index,hash,name]
                    pStream.GetFileList().Clear();
                    pStream.GetFileList().AddRange(PackFileEntry.CreateFileList(Encoding.UTF8.GetString(CryptoMan.DecryptFileList(pStream, pHeader.BaseStream))));
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
                                        pFileHeader = PackFileHeaderVer1.ParseHeader(pReader);
                                        pStream.GetFileList()[pFileHeader.GetFileIndex() - 1].FileHeader = pFileHeader;
                                    }
                                    break;
                                case PackVer.NS2F:
                                    for (ulong i = 0; i < pStream.GetFileListCount(); i++)
                                    {
                                        pFileHeader = PackFileHeaderVer2.ParseHeader(pReader);
                                        pStream.GetFileList()[pFileHeader.GetFileIndex() - 1].FileHeader = pFileHeader;
                                    }
                                    break;
                                case PackVer.OS2F:
                                case PackVer.PS2F:
                                    for (ulong i = 0; i < pStream.GetFileListCount(); i++)
                                    {
                                        pFileHeader = PackFileHeaderVer3.ParseHeader(pReader, pStream.GetVer());
                                        pStream.GetFileList()[pFileHeader.GetFileIndex() - 1].FileHeader = pFileHeader;
                                    }
                                    break;
                            }
                        }
                    }
                }

                this.pDataMappedMemFile = MemoryMappedFile.CreateFromFile(this.sDataUOL);

                InitializeTree(pStream);
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
                    UpdatePanel("", null);

                    this.pEntryValue.Text = "Empty";
                }
            } else
            {
                NotifyMessage("There is no package to be reloaded.", MessageBoxIcon.Warning);
            }
        }

        // TODO: Implement progress bar status, improve speed & efficiency if at all possible.
        private void OnSaveFile(object sender, EventArgs e)
        {
            PackNode pNode = this.pTreeView.SelectedNode as PackNode;

            if (pNode != null && pNode.Tag is PackStreamVerBase)
            {
                SaveFileDialog pDialog = new SaveFileDialog
                {
                    Title = "Select the destination to save the file",
                    Filter = "MS2 File|*.m2h;*.m2d"
                };

                if (pDialog.ShowDialog() == DialogResult.OK)
                {
                    string sPath = Dir_BackSlashToSlash(pDialog.FileName);
                    string sDir = sPath.Substring(0, sPath.LastIndexOf('/') + 1);
                    string sFileName = sPath.Replace(sDir, "").Split('.')[0];

                    PackStreamVerBase pStream = pNode.Tag as PackStreamVerBase;
                    if (pStream == null)
                    {
                        return;
                    }

                    // Re-calculate the file list in case of index removal
                    pStream.GetFileList().Sort();

                    // Save the data blocks to file and re-calculate all entries
                    SaveData(sDir + sFileName + ".m2d", pStream.GetFileList());

                    // Declare the new file count (header update)
                    uint dwFileCount = (uint)pStream.GetFileList().Count;

                    // Construct a raw string containing the new file list information
                    string sFileList = "";
                    foreach (PackFileEntry pEntry in pStream.GetFileList())
                    {
                        sFileList += pEntry.ToString();
                    }

                    // Encrypt the file list and output the new header sizes (header update)
                    uint uHeaderLen, uCompressedHeaderLen, uEncodedHeaderLen;
                    byte[] pHeader = CryptoMan.Encrypt(pStream.GetVer(), Encoding.UTF8.GetBytes(sFileList.ToCharArray()), true, out uHeaderLen, out uCompressedHeaderLen, out uEncodedHeaderLen);

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

                    // Encrypt the file table and output the new data sizes (header update)
                    uint uDataLen, uCompressedDataLen, uEncodedDataLen;
                    pFileTable = CryptoMan.Encrypt(pStream.GetVer(), pFileTable, true, out uDataLen, out uCompressedDataLen, out uEncodedDataLen);

                    // Update all header sizes to the new file list information
                    pStream.SetFileListCount(dwFileCount);
                    pStream.SetHeaderSize(uHeaderLen);
                    pStream.SetCompressedHeaderSize(uCompressedHeaderLen);
                    pStream.SetEncodedHeaderSize(uEncodedHeaderLen);
                    pStream.SetDataSize(uDataLen);
                    pStream.SetCompressedDataSize(uCompressedDataLen);
                    pStream.SetEncodedDataSize(uEncodedDataLen);

                    // Write the new header data to stream
                    using (BinaryWriter pWriter = new BinaryWriter(File.Create(sPath)))
                    {
                        // Encode the file version (MS2F,NS2F,etc)
                        pWriter.Write(pStream.GetVer());

                        // Encode the stream header information
                        pStream.Encode(pWriter);

                        // Encode the encrypted header and file table buffers
                        pWriter.Write(pHeader);
                        pWriter.Write(pFileTable);
                    }
                }
            } else
            {
                NotifyMessage("Please select the file to save.", MessageBoxIcon.Information);
            }
        }

        private void OnSelectNode(object sender, TreeViewEventArgs e)
        {
            PackNode pNode = this.pTreeView.SelectedNode as PackNode;

            if (pNode != null)
            {
                object pObj = pNode.Tag;

                if (pObj is PackNodeList)
                {
                    this.pEntryValue.Text = "Packed Directory";
                } else if (pObj is PackFileEntry)
                {
                    PackFileEntry pEntry = pObj as PackFileEntry;
                    PackFileHeaderVerBase pFileHeader = pEntry.FileHeader;

                    if (pFileHeader != null)
                    {
                        pNode.Data = CryptoMan.DecryptData(pFileHeader, this.pDataMappedMemFile);

                        UpdatePanel(pEntry.TreeName.Split('.')[1].ToLower(), pNode.Data);
                    }
                }
                else if (pObj is PackStreamVerBase)
                {
                    this.pEntryValue.Text = "Packed Data File";
                } else
                {
                    this.pEntryValue.Text = "Empty";
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
                this.sDataUOL = "";

                if (this.pDataMappedMemFile != null)
                {
                    this.pDataMappedMemFile.Dispose();
                    this.pDataMappedMemFile = null;
                }

                this.pTextData.Visible = false;
                this.pImageData.Visible = false;

                System.GC.Collect();
            } else
            {
                NotifyMessage("There is no package to be unloaded.", MessageBoxIcon.Warning);
            }
        }

        private void RenderImageData()
        {
            this.pImageData.Visible = this.panel1.Visible;

            if (this.pImageData.Visible)
            {
                this.pImageData.SizeMode = this.pImageData.Image.Size.Height > this.panel1.Height
                    || this.pImageData.Image.Size.Width > this.panel1.Width ? PictureBoxSizeMode.Zoom : PictureBoxSizeMode.Normal;
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
                    // If the entry was modified, or is new, ignore it and continue
                    if (pEntry.Changed)
                    {
                        aNewEntry.Add(pEntry);
                        aEntry.Remove(pEntry);
                    }
                    // If the entry is unchanged, parse the block from the original offsets
                    else
                    {
                        // Make sure the entry has a parsed file header from load
                        PackFileHeaderVerBase pHeader = pEntry.FileHeader;

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

                                if ((ulong)pBuffer.Read(pSrc, 0, (int)pHeader.GetEncodedFileSize()) == pHeader.GetEncodedFileSize())
                                {
                                    // Modify the header's file index to the updated offset after entry changes
                                    pHeader.SetFileIndex(nCurIndex);
                                    // Modify the header's offset to the updated offset after entry changes
                                    pHeader.SetOffset(uOffset);
                                    // Write the original (completely encrypted) block of data to file
                                    pWriter.Write(pSrc);

                                    nCurIndex++;
                                    uOffset += pHeader.GetEncodedFileSize();
                                }
                            }
                        }
                    }
                }

                // Now iterate all of the new or modified entries
                foreach (PackFileEntry pEntry in aNewEntry)
                {
                    PackFileHeaderVerBase pHeader = pEntry.FileHeader;

                    // If the header is null (new entry), then create one
                    if (pHeader == null)
                    {
                        // TODO: Real use of Compression Flags.. atm just compress everything yolo ;)
                        switch (uVer)
                        {
                            case PackVer.MS2F:
                                pHeader = PackFileHeaderVer1.CreateHeader(nCurIndex, 0xEE000009, uOffset, pEntry.Data);
                                break;
                            case PackVer.NS2F:
                                pHeader = PackFileHeaderVer2.CreateHeader(nCurIndex, 0xEE000009, uOffset, pEntry.Data);
                                break;
                            case PackVer.OS2F:
                            case PackVer.PS2F:
                                pHeader = PackFileHeaderVer3.CreateHeader(uVer, nCurIndex, 0xEE000009, uOffset, pEntry.Data);
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
                    uint uLen, uCompressed, uEncoded;
                    pWriter.Write(CryptoMan.Encrypt(uVer, pEntry.Data, pEntry.FileHeader.GetCompressionFlag() == 0xEE000009, out uLen, out uCompressed, out uEncoded));

                    // Apply the file size changes from the new buffer
                    pHeader.SetFileSize(uLen);
                    pHeader.SetCompressedFileSize(uCompressed);
                    pHeader.SetEncodedFileSize(uEncoded);

                    nCurIndex++;
                    uOffset += pHeader.GetEncodedFileSize();

                    // Add this new entry back to the file list.
                    aEntry.Add(pEntry);
                    aNewEntry.Remove(pEntry);
                }
            }
        }

        private void UpdatePanel(string sExtension, byte[] pBuffer)
        {
            this.pEntryValue.Text = string.Format("{0} File", sExtension.ToUpper());

            this.pTextData.Visible = (sExtension.Equals("ini") || sExtension.Equals("nt") || sExtension.Equals("lua")
                || sExtension.Equals("xml") || sExtension.Equals("flat") || sExtension.Equals("xblock") 
                || sExtension.Equals("diagram") || sExtension.Equals("preset") || sExtension.Equals("emtproj"));
            this.panel1.Visible = (sExtension.Equals("png") || sExtension.Equals("dds"));

            if (sExtension.Equals("ini") || sExtension.Equals("nt") || sExtension.Equals("lua"))
            {
                this.pTextData.Text = Encoding.UTF8.GetString(pBuffer);
            } else if (sExtension.Equals("xml") || sExtension.Equals("flat") || sExtension.Equals("xblock") 
                || sExtension.Equals("diagram") || sExtension.Equals("preset") || sExtension.Equals("emtproj"))
            {
                string sOutput = Encoding.UTF8.GetString(pBuffer);

                try
                {
                    this.pTextData.Text = System.Xml.Linq.XDocument.Parse(sOutput).ToString();
                } catch (Exception e)
                {
                    this.pTextData.Text = sOutput;
                }
            } else if (sExtension.Equals("png")) {
                Bitmap pImage;
                using (MemoryStream pStream = new MemoryStream(pBuffer))
                {
                    pImage = new Bitmap(pStream);
                }

                this.pImageData.Image = pImage;
            } else if (sExtension.Equals("dds")) {
                this.pImageData.Image = DDS.LoadImage(pBuffer);
            } else
            {
                this.pTextData.Visible = false;

                /*
                 * TODO:
                 * *.usm files (NOTE: unable to access raw data due to decryption error, fix bug with usm data)
                 * *.nif, *.kf, and *.kfm files
                 * Shaders/*.fxo - directx shader files?
                 * PrecomputedTerrain/*.tok - mesh3d files? token files?
                 * Gfx/*.gfx - graphics gen files?
                 * Precompiled/luapack.o - object files?
                */
            }

            RenderImageData();
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
