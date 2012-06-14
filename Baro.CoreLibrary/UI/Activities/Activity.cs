using System;
using System.Linq;
using System.Collections.Generic;
using Baro.CoreLibrary.UI.Controls;

namespace Baro.CoreLibrary.UI.Activities
{
    public abstract class Activity
    {
        public abstract void Create(UIForm form);

        public abstract void ExecuteOnce(UIForm form);
    }
}
