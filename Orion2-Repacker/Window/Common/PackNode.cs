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

using Orion.Crypto.Common;
using System.Text;
using System.Windows.Forms;

namespace Orion.Window.Common
{
    public class PackNode : TreeNode
    {
        private byte[] pData;

        public PackNode(object pItem, string sName) 
            : base()
        {
            this.Name = sName;
            this.Text = sName;
            this.Tag = pItem;
        }

        /* Generate the full current path of this node within the tree */
        public string Path
        {
            get
            {
                string[] aPath = new string[this.Level];

                TreeNode pNode = this;
                for (int i = 0; i < aPath.Length; i++)
                {
                    aPath[i] = pNode.Name;

                    pNode = pNode.Parent;
                    if (pNode == null)
                    {
                        break;
                    }
                }

                StringBuilder sPath = new StringBuilder();
                for (int i = aPath.Length - 1; i >= 0; i--)
                {
                    sPath.Append(aPath[i]);
                }

                return sPath.ToString();
            }
        }

        /* Return the decrypted data block from the entry */
        public byte[] Data
        {
            get
            {
                if (this.Tag is PackFileEntry)
                {
                    return (this.Tag as PackFileEntry).Data;
                } else
                {
                    return pData;
                }
            }
            set
            {
                if (this.Tag is PackFileEntry)
                {
                    (this.Tag as PackFileEntry).Data = value;
                } else
                {
                    this.pData = value;
                }
            }
        }
    }
}
