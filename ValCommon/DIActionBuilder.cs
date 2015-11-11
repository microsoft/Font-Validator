using System;

namespace NS_ValCommon
{
    public class DIActionBuilder
    {
        static public DIAction DIA(object obj, string nameDIA)
        {
            if ((obj==null) || (nameDIA==null))
                return null;
            return ((Delegate.CreateDelegate(typeof(DIAction),obj,nameDIA)) as DIAction);
        }
    }
}