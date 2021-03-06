﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Baro.CoreLibrary.AAA2
{
    [DataContract]
    public struct UserData2
    {
        private string _key, _value;

        [DataMember]
        public string Value
        {
            set { _value = value; }
            get { return _value; }
        }

        [DataMember]
        public string Key
        {
            set { _key = value; }
            get { return _key; }
        }

        public UserData2(string key, string value)
        {
            _key = key;
            _value = value;
        }
    }
}
