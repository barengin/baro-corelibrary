using System;
using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.Configuration
{
    public interface IConfigRepository: IDisposable, IEnumerable<IConfigSection>
    {
        bool AutoSave { get; set; }

        void Reload();
        void Save();
        void Clear();

        IConfigSection GetSection(string section);
        IConfigSection CreateOrGetSection(string section);
        IConfigSection CreateSection(string section);
        
        void RemoveSection(string section);
        void RemoveSection(IConfigSection section);
    }
}
