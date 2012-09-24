using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Baro.CoreLibrary.AAA2
{
    [DataContract]
    public struct AAA2Credential
    {
        private string _usr, _pwd;

        [DataMember]
        public string Password
        {
            set { _pwd = value; }
            get { return _pwd; }
        }

        [DataMember]
        public string Username
        {
            set { _usr = value; }
            get { return _usr; }
        }

        public AAA2Credential(string usr, string pwd)
        {
            _usr = usr;
            _pwd = pwd;
        }
    }
}
