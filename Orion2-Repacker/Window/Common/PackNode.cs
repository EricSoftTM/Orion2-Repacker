using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Orion.Window.Common
{
    public class PackNode : TreeNode
    {
        public byte[] Data { get; set; }

        public PackNode(object pItem, string sName) 
            : base()
        {
            this.Name = sName;
            this.Text = sName;
            this.Tag = pItem;
        }
    }
}
