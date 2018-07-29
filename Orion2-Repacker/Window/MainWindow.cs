using Orion.Crypto;
using Orion.Crypto.Common;
using Orion.Crypto.Stream;
using Orion.Crypto.Stream.DDS;
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
        private PackStreamVerBase pStream; //TODO: Do we need to store this into memory, or is it entirely useless after tree iteration?
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

            this.pStream = null;
            this.pNodeList = null;
            this.pDataMappedMemFile = null;
        }

        private void InitializeTree()
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
            if (this.pStream != null && this.pNodeList != null)
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

                using (BinaryReader pHeader = new BinaryReader(File.OpenRead(this.sHeaderUOL)))
                {
                    // Construct a new packed stream from the header data
                    this.pStream = PackVer.CreatePackVer(pHeader);

                    // Insert a collection containing the file list information [index,hash,name]
                    this.pStream.GetFileList().Clear();
                    this.pStream.GetFileList().AddRange(PackFileEntry.CreateFileList(Encoding.UTF8.GetString(CryptoMan.DecryptFileList(this.pStream, pHeader.BaseStream))));
                    // Make the collection of files sorted by their FileIndex for easy fetching
                    this.pStream.GetFileList().Sort();
                    
                    // Load the file allocation table and assign each file header to the entry within the list
                    byte[] pFileTable = CryptoMan.DecryptFileTable(this.pStream, pHeader.BaseStream);
                    using (MemoryStream pTableStream = new MemoryStream(pFileTable))
                    {
                        using (BinaryReader pReader = new BinaryReader(pTableStream))
                        {
                            PackFileHeaderVerBase pFileHeader;

                            switch (pStream.GetVer())
                            {
                                case PackVer.MS2F:
                                    for (ulong i = 0; i < this.pStream.GetFileListCount(); i++)
                                    {
                                        pFileHeader = PackFileHeaderVer1.ParseHeader(pReader);
                                        this.pStream.GetFileList()[pFileHeader.GetFileIndex() - 1].FileHeader = pFileHeader;
                                    }
                                    break;
                                case PackVer.NS2F:
                                    for (ulong i = 0; i < this.pStream.GetFileListCount(); i++)
                                    {
                                        pFileHeader = PackFileHeaderVer2.ParseHeader(pReader);
                                        this.pStream.GetFileList()[pFileHeader.GetFileIndex() - 1].FileHeader = pFileHeader;
                                    }
                                    break;
                                case PackVer.OS2F:
                                case PackVer.PS2F:
                                    for (ulong i = 0; i < this.pStream.GetFileListCount(); i++)
                                    {
                                        pFileHeader = PackFileHeaderVer3.ParseHeader(pReader, pStream.GetVer());
                                        this.pStream.GetFileList()[pFileHeader.GetFileIndex() - 1].FileHeader = pFileHeader;
                                    }
                                    break;
                            }
                        }
                    }
                }

                this.pDataMappedMemFile = MemoryMappedFile.CreateFromFile(this.sDataUOL);

                InitializeTree();
            }
        }

        private void OnReloadFile(object sender, EventArgs e)
        {
            if (this.pStream != null && this.pNodeList != null)
            {
                pTreeView.Nodes.Clear();
                pTreeView.Refresh();

                InitializeTree();

                this.pEntryValue.Text = "Packed Data File";
            } else
            {
                NotifyMessage("There is no package to be reloaded.", MessageBoxIcon.Warning);
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
            if (this.pStream != null && this.pNodeList != null)
            {
                pTreeView.Nodes.Clear();

                this.pNodeList.InternalRelease();
                this.pNodeList = null;

                this.sHeaderUOL = "";
                this.sDataUOL = "";

                this.pStream.GetFileList().Clear();
                this.pStream = null;

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

        private void UpdatePanel(string sExtension, byte[] pBuffer)
        {
            this.pEntryValue.Text = string.Format("{0} File", sExtension.ToUpper());

            this.pTextData.Visible = (sExtension.Equals("ini") || sExtension.Equals("xml"));
            this.panel1.Visible = (sExtension.Equals("png") || sExtension.Equals("dds"));

            if (sExtension.Equals("ini"))
            {
                string sOutput = Encoding.UTF8.GetString(pBuffer);

                this.pTextData.Text = sOutput;
            } else if (sExtension.Equals("xml"))
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

                // TODO: PNG, DDS, NIF, KF(?), NT(?), FLAT
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
