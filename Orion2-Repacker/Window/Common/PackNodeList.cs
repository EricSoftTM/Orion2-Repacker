using Orion.Crypto.Common;
using System.Collections.Generic;

namespace Orion.Window.Common
{
    public class PackNodeList
    {
        private readonly Dictionary<string, PackNodeList> mChildren;
        private readonly Dictionary<string, PackFileEntry> mEntries;

        public Dictionary<string, PackNodeList> Children { get { return mChildren; } }
        public Dictionary<string, PackFileEntry> Entries { get { return mEntries; } }

        public PackNodeList()
        {
            this.mChildren = new Dictionary<string, PackNodeList>();
            this.mEntries = new Dictionary<string, PackFileEntry>();
        }

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
