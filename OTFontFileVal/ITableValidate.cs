using System;

using OTFontFile;

namespace OTFontFileVal
{
    interface ITableValidate
    {
        bool Validate(Validator v, OTFontVal fontOwner);
    }
}
