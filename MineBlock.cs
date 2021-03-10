using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Mine
{
    partial class MineBlock : Button
    {
        public Boolean HasMine { get; set; } 
        public int I { get; set; }
        public int J { get; set; }
        public Status Status;
        
    }
}
