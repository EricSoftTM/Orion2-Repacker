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
using System;
using System.Collections.Generic;

namespace Orion.Window.Common
{
    [Serializable]
    public class PackNodeList
    {
        public const string DATA_FORMAT = "Pack.Node.FileList";

        private readonly Dictionary<string, PackNodeList> mChildren; // <directory, list of nodes>
        private readonly Dictionary<string, PackFileEntry> mEntries; // <file name, file entry>

        public Dictionary<string, PackNodeList> Children { get { return mChildren; } }
        public Dictionary<string, PackFileEntry> Entries { get { return mEntries; } }
        public string Directory { get; private set; }

        public PackNodeList(string sDir)
        {
            this.Directory = sDir;
            this.mChildren = new Dictionary<string, PackNodeList>();
            this.mEntries = new Dictionary<string, PackFileEntry>();
        }

        /*
         * Recursively clear all children/entries within this node list.
         * 
        */
        public void InternalRelease()
        {
            mEntries.Clear();

            foreach (PackNodeList pChild in mChildren.Values)
            {
                pChild.InternalRelease();
            }
            mChildren.Clear();
        }
    }
}
