using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baro.CoreLibrary.Ray
{
    public interface IRayQuery<TResult>
    {
        TResult this[string index] { get; set; }

        IEnumerable<TResult> SelectAll();
        IEnumerable<TResult> StartsWith(string value);
        IEnumerable<TResult> Like(string value);
    }
}
