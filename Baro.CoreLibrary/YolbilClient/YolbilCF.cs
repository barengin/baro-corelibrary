using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Baro.CoreLibrary.YolbilClient
{
    public partial class YolbilCF : Component
    {
        public YolbilCF()
        {
            InitializeComponent();
        }

        public YolbilCF(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public bool Connected { get; set; }
    }
}
