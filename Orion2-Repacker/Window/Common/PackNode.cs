using Orion.Crypto.Common;
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
