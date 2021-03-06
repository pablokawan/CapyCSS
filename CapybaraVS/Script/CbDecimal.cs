﻿using System;

namespace CapybaraVS.Script
{
    /// <summary>
    /// decimal 型
    /// </summary>
    public class CbDecimal : BaseCbValueClass<decimal>, ICbValueClass<decimal>
    {
        public override Type MyType => typeof(CbDecimal);

        public CbDecimal(decimal n = 0, string name = "")
        {
            Value = n;
            Name = name;
        }

        public override string ValueString 
        {
            get
            {
                if (IsError)
                    return ERROR_STR;
                return Value.ToString();
            }
            set
            {
                if (value != null)
                    Value = decimal.Parse(value);
            }
        }

        public static CbDecimal Create(string name)
        {
            var ret = new CbDecimal(0, name);
            return ret;
        }

        public static CbDecimal Create(decimal n = 0, string name = "")
        {
            var ret = new CbDecimal(n, name);
            return ret;
        }

        public static Func<ICbValue> TF = () => CbDecimal.Create();
        public static Func<string, ICbValue> NTF = (name) => CbDecimal.Create(name);
    }
}
