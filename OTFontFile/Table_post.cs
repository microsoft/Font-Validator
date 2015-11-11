using System;
using System.Collections;
using System.Text;

namespace OTFontFile
{
    public class StandardPostNames
    {
        private static string[] macNames = new string[258]
        {
            ".notdef",
            "null",
            "CR",
            "space",
            "exclam",
            "quotedbl",
            "numbersign",
            "dollar",
            "percent",
            "ampersand",
            "quotesingle",
            "parenleft",
            "parenright",
            "asterisk",
            "plus",
            "comma",
            "hyphen",
            "period",
            "slash",
            "zero",
            "one",
            "two",
            "three",
            "four",
            "five",
            "six",
            "seven",
            "eight",
            "nine",
            "colon",
            "semicolon",
            "less",
            "equal",
            "greater",
            "question",
            "at",
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z",
            "bracketleft",
            "backslash",
            "bracketright",
            "asciicircum",
            "underscore",
            "grave",
            "a",
            "b",
            "c",
            "d",
            "e",
            "f",
            "g",
            "h",
            "i",
            "j",
            "k",
            "l",
            "m",
            "n",
            "o",
            "p",
            "q",
            "r",
            "s",
            "t",
            "u",
            "v",
            "w",
            "x",
            "y",
            "z",
            "braceleft",
            "bar",
            "braceright",
            "asciitilde",
            "Adieresis",
            "Aring",
            "Ccedilla",
            "Eacute",
            "Ntilde",
            "Odieresis",
            "Udieresis",
            "aacute",
            "agrave",
            "acircumflex",
            "adieresis",
            "atilde",
            "aring",
            "ccedilla",
            "eacute",
            "egrave",
            "ecircumflex",
            "edieresis",
            "iacute",
            "igrave",
            "icircumflex",
            "idieresis",
            "ntilde",
            "oacute",
            "ograve",
            "ocircumflex",
            "odieresis",
            "otilde",
            "uacute",
            "ugrave",
            "ucircumflex",
            "udieresis",
            "dagger",
            "degree",
            "cent",
            "sterling",
            "section",
            "bullet",
            "paragraph",
            "germandbls",
            "registered",
            "copyright",
            "trademark",
            "acute",
            "dieresis",
            "notequal",
            "AE",
            "Oslash",
            "infinity",
            "plusminus",
            "lessequal",
            "greaterequal",
            "yen",
            "mu",
            "partialdiff",
            "summation",
            "product",
            "pi",
            "integral",
            "ordfeminine",
            "ordmasculine",
            "Omega",
            "ae",
            "oslash",
            "questiondown",
            "exclamdown",
            "logicalnot",
            "radical",
            "florin",
            "approxequal",
            "increment",
            "guillemotleft",
            "guillemotright",
            "elipsis",
            "nbspace",
            "Agrave",
            "Atilde",
            "Otilde",
            "OE",
            "oe",
            "endash",
            "emdash",
            "quotedblleft",
            "quotedblright",
            "quoteleft",
            "quoteright",
            "divide",
            "lozenge",
            "ydieresis",
            "Ydieresis",
            "fraction",
            "currency",
            "guilsinglleft",
            "guilsinglright",
            "fi",
            "fl",
            "vdaggerdbl",
            "middot",
            "quotesinglbase",
            "quotedblbase",
            "perthousand",
            "Acircumflex",
            "Ecircumflex",
            "Aacute",
            "Edieresis",
            "Egrave",
            "Iacute",
            "Icircumflex",
            "Idieresis",
            "Igrave",
            "Oacute",
            "Ocircumflex",
            "apple",
            "Ograve",
            "Uacute",
            "Ucircumflex",
            "Ugrave",
            "dotlessi",
            "circumflex",
            "tilde",
            "overscore",
            "breve",
            "dotaccent",
            "ring",
            "cidilla",
            "hungarumlaut",
            "ogonek",
            "caron",
            "Lslash",
            "lslash",
            "Scaron",
            "scaron",
            "Zcaron",
            "zcaron",
            "brokenbar",
            "Eth",
            "eth",
            "Yacute",
            "yacute",
            "Thorn",
            "thorn",
            "minus",
            "multiply",
            "onesuperior",
            "twosuperior",
            "threesuperior",
            "onehalf",
            "onequarter",
            "threequarters",
            "franc",
            "Gbreve",
            "gbreve",
            "Idot",
            "Scidilla",
            "scedilla",
            "Cacute",
            "cacute",
            "Ccaron",
            "ccaron",
            "dcroat"
        };
        
        private static Hashtable postNames;

        static StandardPostNames()
        {
            postNames = new Hashtable();

            postNames.Add(0x0000, ".notdef");
            postNames.Add(0x000D, "CR");
            postNames.Add(0x0020, "space");
            postNames.Add(0x0021, "exclam");
            postNames.Add(0x0022, "quotedbl");
            postNames.Add(0x0023, "numbersign");
            postNames.Add(0x0024, "dollar");
            postNames.Add(0x0025, "percent");
            postNames.Add(0x0026, "ampersand");
            postNames.Add(0x0027, "quotesingle");
            postNames.Add(0x0028, "parenleft");
            postNames.Add(0x0029, "parenright");
            postNames.Add(0x002a, "asterisk");
            postNames.Add(0x002b, "plus");
            postNames.Add(0x002c, "comma");
            postNames.Add(0x002d, "hyphen");
            postNames.Add(0x002e, "period");
            postNames.Add(0x002f, "slash");
            postNames.Add(0x0030, "zero");
            postNames.Add(0x0031, "one");
            postNames.Add(0x0032, "two");
            postNames.Add(0x0033, "three");
            postNames.Add(0x0034, "four");
            postNames.Add(0x0035, "five");
            postNames.Add(0x0036, "six");
            postNames.Add(0x0037, "seven");
            postNames.Add(0x0038, "eight");
            postNames.Add(0x0039, "nine");
            postNames.Add(0x003a, "colon");
            postNames.Add(0x003b, "semicolon");
            postNames.Add(0x003c, "less");
            postNames.Add(0x003d, "equal");
            postNames.Add(0x003e, "greater");
            postNames.Add(0x003f, "question");
            postNames.Add(0x0040, "at");
            postNames.Add(0x0041, "A");
            postNames.Add(0x0042, "B");
            postNames.Add(0x0043, "C");
            postNames.Add(0x0044, "D");
            postNames.Add(0x0045, "E");
            postNames.Add(0x0046, "F");
            postNames.Add(0x0047, "G");
            postNames.Add(0x0048, "H");
            postNames.Add(0x0049, "I");
            postNames.Add(0x004a, "J");
            postNames.Add(0x004b, "K");
            postNames.Add(0x004c, "L");
            postNames.Add(0x004d, "M");
            postNames.Add(0x004e, "N");
            postNames.Add(0x004f, "O");
            postNames.Add(0x0050, "P");
            postNames.Add(0x0051, "Q");
            postNames.Add(0x0052, "R");
            postNames.Add(0x0053, "S");
            postNames.Add(0x0054, "T");
            postNames.Add(0x0055, "U");
            postNames.Add(0x0056, "V");
            postNames.Add(0x0057, "W");
            postNames.Add(0x0058, "X");
            postNames.Add(0x0059, "Y");
            postNames.Add(0x005a, "Z");
            postNames.Add(0x005b, "bracketleft");
            postNames.Add(0x005c, "backslash");
            postNames.Add(0x005d, "bracketright");
            postNames.Add(0x005e, "asciicircum");
            postNames.Add(0x005f, "underscore");
            postNames.Add(0x0060, "grave");
            postNames.Add(0x0061, "a");
            postNames.Add(0x0062, "b");
            postNames.Add(0x0063, "c");
            postNames.Add(0x0064, "d");
            postNames.Add(0x0065, "e");
            postNames.Add(0x0066, "f");
            postNames.Add(0x0067, "g");
            postNames.Add(0x0068, "h");
            postNames.Add(0x0069, "i");
            postNames.Add(0x006a, "j");
            postNames.Add(0x006b, "k");
            postNames.Add(0x006c, "l");
            postNames.Add(0x006d, "m");
            postNames.Add(0x006e, "n");
            postNames.Add(0x006f, "o");
            postNames.Add(0x0070, "p");
            postNames.Add(0x0071, "q");
            postNames.Add(0x0072, "r");
            postNames.Add(0x0073, "s");
            postNames.Add(0x0074, "t");
            postNames.Add(0x0075, "u");
            postNames.Add(0x0076, "v");
            postNames.Add(0x0077, "w");
            postNames.Add(0x0078, "x");
            postNames.Add(0x0079, "y");
            postNames.Add(0x007a, "z");
            postNames.Add(0x007b, "braceleft");
            postNames.Add(0x007c, "bar");
            postNames.Add(0x007d, "braceright");
            postNames.Add(0x007e, "asciitilde");
            postNames.Add(0x00a0, "nbspace");
            postNames.Add(0x00a1, "exclamdown");
            postNames.Add(0x00a2, "cent");
            postNames.Add(0x00a3, "sterling");
            postNames.Add(0x00a4, "currency");
            postNames.Add(0x00a5, "yen");
            postNames.Add(0x00a6, "brokenbar");
            postNames.Add(0x00a7, "section");
            postNames.Add(0x00a8, "dieresis");
            postNames.Add(0x00a9, "copyright");
            postNames.Add(0x00aa, "ordfeminine");
            postNames.Add(0x00ab, "guillemotleft");
            postNames.Add(0x00ac, "logicalnot");
            postNames.Add(0x00ad, "sfthyphen");
            postNames.Add(0x00ae, "registered");
            postNames.Add(0x00af, "macron");
            postNames.Add(0x00b0, "degree");
            postNames.Add(0x00b1, "plusminus");
            postNames.Add(0x00b2, "twosuperior");
            postNames.Add(0x00b3, "threesuperior");
            postNames.Add(0x00b4, "acute");
            postNames.Add(0x00b5, "mu");
            postNames.Add(0x00b6, "paragraph");
            postNames.Add(0x00b7, "periodcentered");
            postNames.Add(0x00b8, "cedilla");
            postNames.Add(0x00b9, "onesuperior");
            postNames.Add(0x00ba, "ordmasculine");
            postNames.Add(0x00bb, "guillemotright");
            postNames.Add(0x00bc, "onequarter");
            postNames.Add(0x00bd, "onehalf");
            postNames.Add(0x00be, "threequarters");
            postNames.Add(0x00bf, "questiondown");
            postNames.Add(0x00c0, "Agrave");
            postNames.Add(0x00c1, "Aacute");
            postNames.Add(0x00c2, "Acircumflex");
            postNames.Add(0x00c3, "Atilde");
            postNames.Add(0x00c4, "Adieresis");
            postNames.Add(0x00c5, "Aring");
            postNames.Add(0x00c6, "AE");
            postNames.Add(0x00c7, "Ccedilla");
            postNames.Add(0x00c8, "Egrave");
            postNames.Add(0x00c9, "Eacute");
            postNames.Add(0x00ca, "Ecircumflex");
            postNames.Add(0x00cb, "Edieresis");
            postNames.Add(0x00cc, "Igrave");
            postNames.Add(0x00cd, "Iacute");
            postNames.Add(0x00ce, "Icircumflex");
            postNames.Add(0x00cf, "Idieresis");
            postNames.Add(0x00d0, "Eth");
            postNames.Add(0x00d1, "Ntilde");
            postNames.Add(0x00d2, "Ograve");
            postNames.Add(0x00d3, "Oacute");
            postNames.Add(0x00d4, "Ocircumflex");
            postNames.Add(0x00d5, "Otilde");
            postNames.Add(0x00d6, "Odieresis");
            postNames.Add(0x00d7, "multiply");
            postNames.Add(0x00d8, "Oslash");
            postNames.Add(0x00d9, "Ugrave");
            postNames.Add(0x00da, "Uacute");
            postNames.Add(0x00db, "Ucircumflex");
            postNames.Add(0x00dc, "Udieresis");
            postNames.Add(0x00dd, "Yacute");
            postNames.Add(0x00de, "Thorn");
            postNames.Add(0x00df, "germandbls");
            postNames.Add(0x00e0, "agrave");
            postNames.Add(0x00e1, "aacute");
            postNames.Add(0x00e2, "acircumflex");
            postNames.Add(0x00e3, "atilde");
            postNames.Add(0x00e4, "adieresis");
            postNames.Add(0x00e5, "aring");
            postNames.Add(0x00e6, "ae");
            postNames.Add(0x00e7, "ccedilla");
            postNames.Add(0x00e8, "egrave");
            postNames.Add(0x00e9, "eacute");
            postNames.Add(0x00ea, "ecircumflex");
            postNames.Add(0x00eb, "edieresis");
            postNames.Add(0x00ec, "igrave");
            postNames.Add(0x00ed, "iacute");
            postNames.Add(0x00ee, "icircumflex");
            postNames.Add(0x00ef, "idieresis");
            postNames.Add(0x00f0, "eth");
            postNames.Add(0x00f1, "ntilde");
            postNames.Add(0x00f2, "ograve");
            postNames.Add(0x00f3, "oacute");
            postNames.Add(0x00f4, "ocircumflex");
            postNames.Add(0x00f5, "otilde");
            postNames.Add(0x00f6, "odieresis");
            postNames.Add(0x00f7, "divide");
            postNames.Add(0x00f8, "oslash");
            postNames.Add(0x00f9, "ugrave");
            postNames.Add(0x00fa, "uacute");
            postNames.Add(0x00fb, "ucircumflex");
            postNames.Add(0x00fc, "udieresis");
            postNames.Add(0x00fd, "yacute");
            postNames.Add(0x00fe, "thorn");
            postNames.Add(0x00ff, "ydieresis");
            postNames.Add(0x0100, "Amacron");
            postNames.Add(0x0101, "amacron");
            postNames.Add(0x0102, "Abreve");
            postNames.Add(0x0103, "abreve");
            postNames.Add(0x0104, "Aogonek");
            postNames.Add(0x0105, "aogonek");
            postNames.Add(0x0106, "Cacute");
            postNames.Add(0x0107, "cacute");
            postNames.Add(0x0108, "Ccircumflex");
            postNames.Add(0x0109, "ccircumflex");
            postNames.Add(0x010a, "Cdotaccent");
            postNames.Add(0x010b, "cdotaccent");
            postNames.Add(0x010c, "Ccaron");
            postNames.Add(0x010d, "ccaron");
            postNames.Add(0x010e, "Dcaron");
            postNames.Add(0x010f, "dcaron");
            postNames.Add(0x0110, "Dcroat");
            postNames.Add(0x0111, "dcroat");
            postNames.Add(0x0112, "Emacron");
            postNames.Add(0x0113, "emacron");
            postNames.Add(0x0114, "Ebreve");
            postNames.Add(0x0115, "ebreve");
            postNames.Add(0x0116, "Edotaccent");
            postNames.Add(0x0117, "edotaccent");
            postNames.Add(0x0118, "Eogonek");
            postNames.Add(0x0119, "eogonek");
            postNames.Add(0x011a, "Ecaron");
            postNames.Add(0x011b, "ecaron");
            postNames.Add(0x011c, "Gcircumflex");
            postNames.Add(0x011d, "gcircumflex");
            postNames.Add(0x011e, "Gbreve");
            postNames.Add(0x011f, "gbreve");
            postNames.Add(0x0120, "Gdotaccent");
            postNames.Add(0x0121, "gdotaccent");
            postNames.Add(0x0122, "Gcommaaccent");
            postNames.Add(0x0123, "gcommaaccent");
            postNames.Add(0x0124, "Hcircumflex");
            postNames.Add(0x0125, "hcircumflex");
            postNames.Add(0x0126, "Hbar");
            postNames.Add(0x0127, "hbar");
            postNames.Add(0x0128, "Itilde");
            postNames.Add(0x0129, "itilde");
            postNames.Add(0x012a, "Imacron");
            postNames.Add(0x012b, "imacron");
            postNames.Add(0x012c, "Ibreve");
            postNames.Add(0x012d, "ibreve");
            postNames.Add(0x012e, "Iogonek");
            postNames.Add(0x012f, "iogonek");
            postNames.Add(0x0130, "Idotaccent");
            postNames.Add(0x0131, "dotlessi");
            postNames.Add(0x0132, "IJ");
            postNames.Add(0x0133, "ij");
            postNames.Add(0x0134, "Jcircumflex");
            postNames.Add(0x0135, "jcircumflex");
            postNames.Add(0x0136, "Kcommaaccent");
            postNames.Add(0x0137, "kcommaaccent");
            postNames.Add(0x0138, "kgreenlandic");
            postNames.Add(0x0139, "Lacute");
            postNames.Add(0x013a, "lacute");
            postNames.Add(0x013b, "Lcommaaccent");
            postNames.Add(0x013c, "lcommaaccent");
            postNames.Add(0x013d, "Lcaron");
            postNames.Add(0x013e, "lcaron");
            postNames.Add(0x013f, "Ldot");
            postNames.Add(0x0140, "ldot");
            postNames.Add(0x0141, "Lslash");
            postNames.Add(0x0142, "lslash");
            postNames.Add(0x0143, "Nacute");
            postNames.Add(0x0144, "nacute");
            postNames.Add(0x0145, "Ncommaaccent");
            postNames.Add(0x0146, "ncommaaccent");
            postNames.Add(0x0147, "Ncaron");
            postNames.Add(0x0148, "ncaron");
            postNames.Add(0x0149, "napostrophe");
            postNames.Add(0x014a, "Eng");
            postNames.Add(0x014b, "eng");
            postNames.Add(0x014c, "Omacron");
            postNames.Add(0x014d, "omacron");
            postNames.Add(0x014e, "Obreve");
            postNames.Add(0x014f, "obreve");
            postNames.Add(0x0150, "Ohungarumlaut");
            postNames.Add(0x0151, "ohungarumlaut");
            postNames.Add(0x0152, "OE");
            postNames.Add(0x0153, "oe");
            postNames.Add(0x0154, "Racute");
            postNames.Add(0x0155, "racute");
            postNames.Add(0x0156, "Rcommaaccent");
            postNames.Add(0x0157, "rcommaaccent");
            postNames.Add(0x0158, "Rcaron");
            postNames.Add(0x0159, "rcaron");
            postNames.Add(0x015a, "Sacute");
            postNames.Add(0x015b, "sacute");
            postNames.Add(0x015c, "Scircumflex");
            postNames.Add(0x015d, "scircumflex");
            postNames.Add(0x015e, "Scedilla");
            postNames.Add(0x015f, "scedilla");
            postNames.Add(0x0160, "Scaron");
            postNames.Add(0x0161, "scaron");
            postNames.Add(0x0162, "Tcommaaccent");
            postNames.Add(0x0163, "tcommaaccent");
            postNames.Add(0x0164, "Tcaron");
            postNames.Add(0x0165, "tcaron");
            postNames.Add(0x0166, "Tbar");
            postNames.Add(0x0167, "tbar");
            postNames.Add(0x0168, "Utilde");
            postNames.Add(0x0169, "utilde");
            postNames.Add(0x016a, "Umacron");
            postNames.Add(0x016b, "umacron");
            postNames.Add(0x016c, "Ubreve");
            postNames.Add(0x016d, "ubreve");
            postNames.Add(0x016e, "Uring");
            postNames.Add(0x016f, "uring");
            postNames.Add(0x0170, "Uhungarumlaut");
            postNames.Add(0x0171, "uhungarumlaut");
            postNames.Add(0x0172, "Uogonek");
            postNames.Add(0x0173, "uogonek");
            postNames.Add(0x0174, "Wcircumflex");
            postNames.Add(0x0175, "wcircumflex");
            postNames.Add(0x0176, "Ycircumflex");
            postNames.Add(0x0177, "ycircumflex");
            postNames.Add(0x0178, "Ydieresis");
            postNames.Add(0x0179, "Zacute");
            postNames.Add(0x017a, "zacute");
            postNames.Add(0x017b, "Zdotaccent");
            postNames.Add(0x017c, "zdotaccent");
            postNames.Add(0x017d, "Zcaron");
            postNames.Add(0x017e, "zcaron");
            postNames.Add(0x017F, "longs");
            postNames.Add(0x0192, "florin");
            postNames.Add(0x01fa, "Aringacute");
            postNames.Add(0x01fb, "aringacute");
            postNames.Add(0x01fc, "AEacute");
            postNames.Add(0x01fd, "aeacute");
            postNames.Add(0x01fe, "Oslashacute");
            postNames.Add(0x01ff, "oslashacute");
            postNames.Add(0x02c6, "circumflex");
            postNames.Add(0x02c7, "caron");
            postNames.Add(0x02c9, "overscore");
            postNames.Add(0x02d8, "breve");
            postNames.Add(0x02d9, "dotaccent");
            postNames.Add(0x02da, "ring");
            postNames.Add(0x02db, "ogonek");
            postNames.Add(0x02dc, "tilde");
            postNames.Add(0x02dd, "hungarumlaut");
            postNames.Add(0x0384, "tonos");
            postNames.Add(0x0385, "dieresistonos");
            postNames.Add(0x0386, "Alphatonos");
            postNames.Add(0x0387, "anoteleia");
            postNames.Add(0x0388, "Epsilontonos");
            postNames.Add(0x0389, "Etatonos");
            postNames.Add(0x038a, "Iotatonos");
            postNames.Add(0x038c, "Omicrontonos");
            postNames.Add(0x038e, "Upsilontonos");
            postNames.Add(0x038f, "Omegatonos");
            postNames.Add(0x0390, "iotadieresistonos");
            postNames.Add(0x0391, "Alpha");
            postNames.Add(0x0392, "Beta");
            postNames.Add(0x0393, "Gamma");
            postNames.Add(0x0394, "Deltagreek");
            postNames.Add(0x0395, "Epsilon");
            postNames.Add(0x0396, "Zeta");
            postNames.Add(0x0397, "Eta");
            postNames.Add(0x0398, "Theta");
            postNames.Add(0x0399, "Iota");
            postNames.Add(0x039a, "Kappa");
            postNames.Add(0x039b, "Lambda");
            postNames.Add(0x039c, "Mu");
            postNames.Add(0x039d, "Nu");
            postNames.Add(0x039e, "Xi");
            postNames.Add(0x039f, "Omicron");
            postNames.Add(0x03a0, "Pi");
            postNames.Add(0x03a1, "Rho");
            postNames.Add(0x03a3, "Sigma");
            postNames.Add(0x03a4, "Tau");
            postNames.Add(0x03a5, "Upsilon");
            postNames.Add(0x03a6, "Phi");
            postNames.Add(0x03a7, "Chi");
            postNames.Add(0x03a8, "Psi");
            postNames.Add(0x03a9, "Omegagreek");
            postNames.Add(0x03aa, "Iotadieresis");
            postNames.Add(0x03ab, "Upsilondieresis");
            postNames.Add(0x03ac, "alphatonos");
            postNames.Add(0x03ad, "epsilontonos");
            postNames.Add(0x03ae, "etatonos");
            postNames.Add(0x03af, "iotatonos");
            postNames.Add(0x03b0, "upsilondieresistonos");
            postNames.Add(0x03b1, "alpha");
            postNames.Add(0x03b2, "beta");
            postNames.Add(0x03b3, "gamma");
            postNames.Add(0x03b4, "delta");
            postNames.Add(0x03b5, "epsilon");
            postNames.Add(0x03b6, "zeta");
            postNames.Add(0x03b7, "eta");
            postNames.Add(0x03b8, "theta");
            postNames.Add(0x03b9, "iota");
            postNames.Add(0x03ba, "kappa");
            postNames.Add(0x03bb, "lambda");
            postNames.Add(0x03bc, "mugreek");
            postNames.Add(0x03bd, "nu");
            postNames.Add(0x03be, "xi");
            postNames.Add(0x03bf, "omicron");
            postNames.Add(0x03c0, "pi");
            postNames.Add(0x03c1, "rho");
            postNames.Add(0x03c2, "sigma1");
            postNames.Add(0x03c3, "sigma");
            postNames.Add(0x03c4, "tau");
            postNames.Add(0x03c5, "upsilon");
            postNames.Add(0x03c6, "phi");
            postNames.Add(0x03c7, "chi");
            postNames.Add(0x03c8, "psi");
            postNames.Add(0x03c9, "omega");
            postNames.Add(0x03ca, "iotadieresis");
            postNames.Add(0x03cb, "upsilondieresis");
            postNames.Add(0x03cc, "omicrontonos");
            postNames.Add(0x03cd, "upsilontonos");
            postNames.Add(0x03ce, "omegatonos");
            postNames.Add(0x0401, "Iocyrillic");
            postNames.Add(0x0402, "Djecyrillic");
            postNames.Add(0x0403, "Gjecyrillic");
            postNames.Add(0x0404, "Ecyrillic");
            postNames.Add(0x0405, "Dzecyrillic");
            postNames.Add(0x0406, "Icyrillic");
            postNames.Add(0x0407, "Yicyrillic");
            postNames.Add(0x0408, "Jecyrillic");
            postNames.Add(0x0409, "Ljecyrillic");
            postNames.Add(0x040a, "Njecyrillic");
            postNames.Add(0x040b, "Tshecyrillic");
            postNames.Add(0x040c, "Kjecyrillic");
            postNames.Add(0x040e, "Ushortcyrillic");
            postNames.Add(0x040f, "Dzhecyrillic");
            postNames.Add(0x0410, "Acyrillic");
            postNames.Add(0x0411, "Becyrillic");
            postNames.Add(0x0412, "Vecyrillic");
            postNames.Add(0x0413, "Gecyrillic");
            postNames.Add(0x0414, "Decyrillic");
            postNames.Add(0x0415, "Iecyrillic");
            postNames.Add(0x0416, "Zhecyrillic");
            postNames.Add(0x0417, "Zecyrillic");
            postNames.Add(0x0418, "Iicyrillic");
            postNames.Add(0x0419, "Iishortcyrillic");
            postNames.Add(0x041a, "Kacyrillic");
            postNames.Add(0x041b, "Elcyrillic");
            postNames.Add(0x041c, "Emcyrillic");
            postNames.Add(0x041d, "Encyrillic");
            postNames.Add(0x041e, "Ocyrillic");
            postNames.Add(0x041f, "Pecyrillic");
            postNames.Add(0x0420, "Ercyrillic");
            postNames.Add(0x0421, "Escyrillic");
            postNames.Add(0x0422, "Tecyrillic");
            postNames.Add(0x0423, "Ucyrillic");
            postNames.Add(0x0424, "Efcyrillic");
            postNames.Add(0x0425, "Khacyrillic");
            postNames.Add(0x0426, "Tsecyrillic");
            postNames.Add(0x0427, "Checyrillic");
            postNames.Add(0x0428, "Shacyrillic");
            postNames.Add(0x0429, "Shchacyrillic");
            postNames.Add(0x042a, "Hardsigncyrillic");
            postNames.Add(0x042b, "Yericyrillic");
            postNames.Add(0x042c, "Softsigncyrillic");
            postNames.Add(0x042d, "Ereversedcyrillic");
            postNames.Add(0x042e, "IUcyrillic");
            postNames.Add(0x042f, "IAcyrillic");
            postNames.Add(0x0430, "acyrillic");
            postNames.Add(0x0431, "becyrillic");
            postNames.Add(0x0432, "vecyrillic");
            postNames.Add(0x0433, "gecyrillic");
            postNames.Add(0x0434, "decyrillic");
            postNames.Add(0x0435, "iecyrillic");
            postNames.Add(0x0436, "zhecyrillic");
            postNames.Add(0x0437, "zecyrillic");
            postNames.Add(0x0438, "iicyrillic");
            postNames.Add(0x0439, "iishortcyrillic");
            postNames.Add(0x043a, "kacyrillic");
            postNames.Add(0x043b, "elcyrillic");
            postNames.Add(0x043c, "emcyrillic");
            postNames.Add(0x043d, "encyrillic");
            postNames.Add(0x043e, "ocyrillic");
            postNames.Add(0x043f, "pecyrillic");
            postNames.Add(0x0440, "ercyrillic");
            postNames.Add(0x0441, "escyrillic");
            postNames.Add(0x0442, "tecyrillic");
            postNames.Add(0x0443, "ucyrillic");
            postNames.Add(0x0444, "efcyrillic");
            postNames.Add(0x0445, "khacyrillic");
            postNames.Add(0x0446, "tsecyrillic");
            postNames.Add(0x0447, "checyrillic");
            postNames.Add(0x0448, "shacyrillic");
            postNames.Add(0x0449, "shchacyrillic");
            postNames.Add(0x044a, "hardsigncyrillic");
            postNames.Add(0x044b, "yericyrillic");
            postNames.Add(0x044c, "softsigncyrillic");
            postNames.Add(0x044d, "ereversedcyrillic");
            postNames.Add(0x044e, "iucyrillic");
            postNames.Add(0x044f, "iacyrillic");
            postNames.Add(0x0451, "iocyrillic");
            postNames.Add(0x0452, "djecyrillic");
            postNames.Add(0x0453, "gjecyrillic");
            postNames.Add(0x0454, "ecyrillic");
            postNames.Add(0x0455, "dzecyrillic");
            postNames.Add(0x0456, "icyrillic");
            postNames.Add(0x0457, "yicyrillic");
            postNames.Add(0x0458, "jecyrillic");
            postNames.Add(0x0459, "ljecyrillic");
            postNames.Add(0x045a, "njecyrillic");
            postNames.Add(0x045b, "tshecyrillic");
            postNames.Add(0x045c, "kjecyrillic");
            postNames.Add(0x045e, "ushortcyrillic");
            postNames.Add(0x045f, "dzhecyrillic");
            postNames.Add(0x0490, "gheupturncyrillic");
            postNames.Add(0x0491, "Ghestrokecyrillic");
            postNames.Add(0x1e80, "Wgrave");
            postNames.Add(0x1e81, "wgrave");
            postNames.Add(0x1e82, "Wacute");
            postNames.Add(0x1e83, "wacute");
            postNames.Add(0x1e84, "Wdieresis");
            postNames.Add(0x1e85, "wdieresis");
            postNames.Add(0x1ef2, "Ygrave");
            postNames.Add(0x1ef3, "ygrave");
            postNames.Add(0x2013, "endash");
            postNames.Add(0x2014, "emdash");
            postNames.Add(0x2015, "horizontalbar");
            postNames.Add(0x2017, "underscoredbl");
            postNames.Add(0x2018, "quoteleft");
            postNames.Add(0x2019, "quoteright");
            postNames.Add(0x201a, "quotesinglbase");
            postNames.Add(0x201b, "quotereversed");
            postNames.Add(0x201c, "quotedblleft");
            postNames.Add(0x201d, "quotedblright");
            postNames.Add(0x201e, "quotedblbase");
            postNames.Add(0x2020, "dagger");
            postNames.Add(0x2021, "daggerdbl");
            postNames.Add(0x2022, "bullet");
            postNames.Add(0x2026, "ellipsis");
            postNames.Add(0x2030, "perthousand");
            postNames.Add(0x2032, "minute");
            postNames.Add(0x2033, "second");
            postNames.Add(0x2039, "guilsinglleft");
            postNames.Add(0x203a, "guilsinglright");
            postNames.Add(0x203c, "exclamdbl");
            postNames.Add(0x203e, "overline");
            postNames.Add(0x2044, "fraction");
            postNames.Add(0x207f, "nsuperior");
            postNames.Add(0x20a3, "franc");
            postNames.Add(0x20a4, "lira");
            postNames.Add(0x20a7, "peseta");
            postNames.Add(0x20ac, "Euro");
            postNames.Add(0x2105, "careof");
            postNames.Add(0x2113, "lsquare");
            postNames.Add(0x2116, "numero");
            postNames.Add(0x2122, "trademark");
            postNames.Add(0x2126, "Omega");
            postNames.Add(0x212e, "estimated");
            postNames.Add(0x215b, "oneeighth");
            postNames.Add(0x215c, "threeeighths");
            postNames.Add(0x215d, "fiveeighths");
            postNames.Add(0x215e, "seveneighths");
            postNames.Add(0x2190, "arrowleft");
            postNames.Add(0x2191, "arrowup");
            postNames.Add(0x2192, "arrowright");
            postNames.Add(0x2193, "arrowdown");
            postNames.Add(0x2194, "arrowboth");
            postNames.Add(0x2195, "arrowupdn");
            postNames.Add(0x21a8, "arrowupdnbse");
            postNames.Add(0x2202, "partialdiff");
            postNames.Add(0x2206, "Delta");
            postNames.Add(0x220f, "product");
            postNames.Add(0x2211, "summation");
            postNames.Add(0x2212, "minus");
            postNames.Add(0x2215, "divisionslash");
            postNames.Add(0x2219, "bulletoperator");
            postNames.Add(0x221a, "radical");
            postNames.Add(0x221e, "infinity");
            postNames.Add(0x221f, "orthogonal");
            postNames.Add(0x2229, "intersection");
            postNames.Add(0x222b, "integral");
            postNames.Add(0x2248, "approxequal");
            postNames.Add(0x2260, "notequal");
            postNames.Add(0x2261, "equivalence");
            postNames.Add(0x2264, "lessequal");
            postNames.Add(0x2265, "greaterequal");
            postNames.Add(0x2302, "house");
            postNames.Add(0x2310, "revlogicalnot");
            postNames.Add(0x2320, "integraltp");
            postNames.Add(0x2321, "integralbt");
            postNames.Add(0x2500, "SF100000");
            postNames.Add(0x2502, "SF110000");
            postNames.Add(0x250c, "SF010000");
            postNames.Add(0x2510, "SF030000");
            postNames.Add(0x2514, "SF020000");
            postNames.Add(0x2518, "SF040000");
            postNames.Add(0x251c, "SF080000");
            postNames.Add(0x2524, "SF090000");
            postNames.Add(0x252c, "SF060000");
            postNames.Add(0x2534, "SF070000");
            postNames.Add(0x253c, "SF050000");
            postNames.Add(0x2550, "SF430000");
            postNames.Add(0x2551, "SF240000");
            postNames.Add(0x2552, "SF510000");
            postNames.Add(0x2553, "SF520000");
            postNames.Add(0x2554, "SF390000");
            postNames.Add(0x2555, "SF220000");
            postNames.Add(0x2556, "SF210000");
            postNames.Add(0x2557, "SF250000");
            postNames.Add(0x2558, "SF500000");
            postNames.Add(0x2559, "SF490000");
            postNames.Add(0x255a, "SF380000");
            postNames.Add(0x255b, "SF280000");
            postNames.Add(0x255c, "SF270000");
            postNames.Add(0x255d, "SF260000");
            postNames.Add(0x255e, "SF360000");
            postNames.Add(0x255f, "SF370000");
            postNames.Add(0x2560, "SF420000");
            postNames.Add(0x2561, "SF190000");
            postNames.Add(0x2562, "SF200000");
            postNames.Add(0x2563, "SF230000");
            postNames.Add(0x2564, "SF470000");
            postNames.Add(0x2565, "SF480000");
            postNames.Add(0x2566, "SF410000");
            postNames.Add(0x2567, "SF450000");
            postNames.Add(0x2568, "SF460000");
            postNames.Add(0x2569, "SF400000");
            postNames.Add(0x256a, "SF540000");
            postNames.Add(0x256b, "SF530000");
            postNames.Add(0x256c, "SF440000");
            postNames.Add(0x2580, "upblock");
            postNames.Add(0x2584, "dnblock");
            postNames.Add(0x2588, "block");
            postNames.Add(0x258c, "lfblock");
            postNames.Add(0x2590, "rtblock");
            postNames.Add(0x2591, "ltshade");
            postNames.Add(0x2592, "shade");
            postNames.Add(0x2593, "dkshade");
            postNames.Add(0x25a0, "filledbox");
            postNames.Add(0x25a1, "H22073");
            postNames.Add(0x25aa, "H18543");
            postNames.Add(0x25ab, "H18551");
            postNames.Add(0x25ac, "blackrectangle");
            postNames.Add(0x25b2, "triagup");
            postNames.Add(0x25ba, "triagrt");
            postNames.Add(0x25bc, "triagdn");
            postNames.Add(0x25c4, "triaglf");
            postNames.Add(0x25ca, "lozenge");
            postNames.Add(0x25cb, "circle");
            postNames.Add(0x25cf, "blackcircle");
            postNames.Add(0x25d8, "invbullet");
            postNames.Add(0x25d9, "invcircle");
            postNames.Add(0x25e6, "openbullet");
            postNames.Add(0x263a, "smileface");
            postNames.Add(0x263b, "invsmileface");
            postNames.Add(0x263c, "sun");
            postNames.Add(0x2640, "female");
            postNames.Add(0x2642, "male");
            postNames.Add(0x2660, "spade");
            postNames.Add(0x2663, "club");
            postNames.Add(0x2665, "heart");
            postNames.Add(0x2666, "diamond");
            postNames.Add(0x266a, "musicalnote");
            postNames.Add(0x266b, "musicalnotedbl");
            postNames.Add(0xfb01, "fi");
            postNames.Add(0xfb02, "fl");
        }

       
        public static string GetMacName(int index)
        {
            if (index < macNames.Length)
            {
                return (string)macNames[index];
            }

            return null;
        }
        
        public static int MacNameCount
        {
            get { return macNames.Length; }
        }

        public static int GetMacIndex(string name)
        {
            int nIndex = -1;

            for (int i = 0; i < macNames.Length; i++)
            {
                if (name == macNames[i])
                {
                    nIndex = i;
                    break;
                }
            }
            return nIndex;
        }

        public static string GetPostNameFromUnicode(ushort nVal)
        {
            if (postNames.ContainsKey((int)nVal))
            {
                return postNames[(int)nVal].ToString();
            }
            else
            {
                return "uni" + ((uint)nVal).ToString("X4");
            }
        }

    }

    /// <summary>
    /// Summary description for Table_post.
    /// </summary>
    public class Table_post : OTTable
    {
        /************************
         * constructors
         */
        
        
        public Table_post(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
            m_nameOffsets = null;

            

            if (Version.GetUint() == 0x00020000)
            {
                // make sure we have more than a header
                if (GetLength() < 34)
                    return;

                // count the number of strings
                m_nNumberOfStrings = 0;
                uint offset = (uint)FieldOffsetsVer2.glyphNameIndex + (uint)numberOfGlyphs*2;
                uint length = 0;
                while (offset< GetLength())
                {
                    m_nNumberOfStrings++;
                    length = m_bufTable.GetByte(offset);
                    offset += length + 1;
                }
                // store the offsets
                m_nameOffsets = new uint[m_nNumberOfStrings];
                offset = (uint)FieldOffsetsVer2.glyphNameIndex + (uint)numberOfGlyphs*2;
                for (uint i=0; i<m_nNumberOfStrings; i++)
                {
                    m_nameOffsets[i] = offset;
                    length = m_bufTable.GetByte(offset);
                    offset += length + 1;
                }
            }
        }

        /************************
         * field offset values
         */

        
        public enum FieldOffsets
        {
            Version            = 0, 
            italicAngle        = 4,
            underlinePosition  = 8,
            underlineThickness = 10,
            isFixedPitch       = 12,
            minMemType42       = 16,
            maxMemType42       = 20,
            minMemType1        = 24,
            maxMemType1        = 28
        }

        public enum FieldOffsetsVer2
        {
            numberOfGlyphs     = 32,
            glyphNameIndex     = 34
        }


        /************************
         * property accessors
         */

        public OTFixed Version
        {
            get {return m_bufTable.GetFixed((uint)FieldOffsets.Version);}
        }

        public OTFixed italicAngle
        {
            get {return m_bufTable.GetFixed((uint)FieldOffsets.italicAngle);}
        }

        public short underlinePosition
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.underlinePosition);}
        }

        public short underlineThickness
        {
            get {return m_bufTable.GetShort((uint)FieldOffsets.underlineThickness);}
        }

        public uint isFixedPitch
        {
            get {return m_bufTable.GetUint((uint)FieldOffsets.isFixedPitch);}
        }

        public uint minMemType42
        {
            get {return m_bufTable.GetUint((uint)FieldOffsets.minMemType42);}
        }

        public uint maxMemType42
        {
            get {return m_bufTable.GetUint((uint)FieldOffsets.maxMemType42);}
        }

        public uint minMemType1
        {
            get {return m_bufTable.GetUint((uint)FieldOffsets.minMemType1);}
        }

        public uint maxMemType1
        {
            get {return m_bufTable.GetUint((uint)FieldOffsets.maxMemType1);}
        }

        public ushort numberOfGlyphs
        {
            get
            {
                if (Version.GetUint() != 0x00020000)
                {
                    throw new InvalidOperationException();
                }
                return m_bufTable.GetUshort((uint)FieldOffsetsVer2.numberOfGlyphs);
            }
        }

        public ushort GetGlyphNameIndex(ushort iGlyph)
        {
            if (Version.GetUint() != 0x00020000)
            {
                throw new InvalidOperationException();
            }
            if (iGlyph >= numberOfGlyphs)
            {
                throw new ArgumentOutOfRangeException();
            }
            return m_bufTable.GetUshort((uint)FieldOffsetsVer2.glyphNameIndex + (uint)iGlyph*2);
        }

        public byte[] GetName(uint i)
        {
            if (Version.GetUint() != 0x00020000)
            {
                throw new InvalidOperationException();
            }
            uint length = m_bufTable.GetByte(m_nameOffsets[i]);
            byte [] buf = new byte[length+1];
            Buffer.BlockCopy(m_bufTable.GetBuffer(), (int)m_nameOffsets[i], buf, 0, (int)length+1);
            return buf;
        }

        public string GetNameString(uint i)
        {
            if (Version.GetUint() != 0x00020000)
            {
                throw new InvalidOperationException();
            }

            if (i >= m_nameOffsets.Length) return "";

            int length = m_bufTable.GetByte(m_nameOffsets[i]);
            
            if (length == 0) return "";
            
            StringBuilder sb = new StringBuilder(length,length);

            for (uint j = 1; j <= length; j++)
            {
                sb.Append((char)m_bufTable.GetByte(m_nameOffsets[i]+j));
            }

            return sb.ToString();
        }
        
        public string GetGlyphName(ushort glyphId)
        {
            ushort nameIndex = GetGlyphNameIndex(glyphId);
           
            if (nameIndex < StandardPostNames.MacNameCount)
            {
                return (string)StandardPostNames.GetMacName(nameIndex);
            }
            else 
            {
                return GetNameString((uint)(nameIndex - StandardPostNames.MacNameCount));
            }
        }

        public uint NumberOfStrings{ get{ return m_nNumberOfStrings; }}

        protected uint [] m_nameOffsets;
        protected uint m_nNumberOfStrings;


        /************************
         * DataCache class
         */

        public override DataCache GetCache()
        {
            if (m_cache == null)
            {
                m_cache = new post_cache(this);
            }

            return m_cache;
        }
        
        public class post_cache : DataCache
        {
            protected OTFixed m_Version;
            protected OTFixed m_italicAngle;
            protected short m_underlinePosition;
            protected short m_underlineThickness;
            protected uint m_isFixedPitch;
            protected uint m_minMemType42;
            protected uint m_maxMemType42;
            protected uint m_minMemType1;
            protected uint m_maxMemType1;
            protected ushort m_numberOfGlyphs; // v2.0, v2.5
            protected ArrayList m_glyphNameIndex; // v2.0 ushort[] array
            protected ArrayList m_names; // v2.0, string[] array, Store in strings but write out byte[] to the buffer
            protected char[] m_offset; // v2.5 NOTE: We may not need? so not supported yet

            // constructor
            public post_cache(Table_post OwnerTable)
            {
                m_Version = OwnerTable.Version;
                m_italicAngle = OwnerTable.italicAngle;
                m_underlinePosition = OwnerTable.underlinePosition;
                m_underlineThickness = OwnerTable.underlineThickness;
                m_isFixedPitch = OwnerTable.isFixedPitch;
                m_minMemType42 = OwnerTable.minMemType42;
                m_maxMemType42 = OwnerTable.maxMemType42;
                m_minMemType1 = OwnerTable.minMemType1;
                m_maxMemType1 = OwnerTable.maxMemType1;

                // NOTE: what about version 2.5 is that covered with this check?
                // NOTE: Are we not checking because it is deprecated?
                if( m_Version.GetUint() == 0x00020000 )
                {
                    m_numberOfGlyphs = OwnerTable.numberOfGlyphs;
                    m_glyphNameIndex = new ArrayList( m_numberOfGlyphs );

                    for( ushort i = 0; i < m_numberOfGlyphs; i++ )
                    {
                        m_glyphNameIndex.Add( OwnerTable.GetGlyphNameIndex( i ));
                    }

                    m_names = new ArrayList( (int)OwnerTable.NumberOfStrings );

                    // Get the gyph names
                    for( uint i = 0; i < OwnerTable.NumberOfStrings; i++ )
                    {
                        m_names.Add( OwnerTable.GetNameString( i ));
                    }
                }

            }

            // accessors for the cached data
            public void MakeDirty()
            {
                m_bDirty = true;
            }

            public OTFixed Version
            {
                get
                {
                    return m_Version;
                }
                set
                {
                    // NOTE: if the version is changed do we try to fix up the m_names?
                    // For now we will let the user handle this
                    if (value != m_Version)
                    {
                        m_Version = value;
                        m_bDirty = true;
                    }
                }
            }

            public OTFixed italicAngle
            {
                get
                {
                    return m_italicAngle;
                }
                set
                {
                    if (value != m_italicAngle)
                    {
                        m_italicAngle = value;
                        m_bDirty = true;
                    }
                }
            }


            public short underlinePosition
            {
                get
                {
                    return m_underlinePosition;
                }
                set
                {
                    if (value != m_underlinePosition)
                    {
                        m_underlinePosition = value;
                        m_bDirty = true;
                    }
                }
            }

            public short underlineThickness
            {
                get
                {
                    return m_underlineThickness;
                }
                set
                {
                    if (value != m_underlineThickness)
                    {
                        m_underlineThickness = value;
                        m_bDirty = true;
                    }
                }
            }

            public uint isFixedPitch
            {
                get
                {
                    return m_isFixedPitch;
                }
                set
                {
                    if (value != m_isFixedPitch)
                    {
                        m_isFixedPitch = value;
                        m_bDirty = true;
                    }
                }
            }


            public uint minMemType42
            {
                get
                {
                    return m_minMemType42;
                }
                set
                {
                    if (value != m_minMemType42)
                    {
                        m_minMemType42 = value;
                        m_bDirty = true;
                    }
                }
            }

            public uint maxMemType42
            {
                get
                {
                    return m_maxMemType42;
                }
                set
                {
                    if (value != m_maxMemType42)
                    {
                        m_maxMemType42 = value;
                        m_bDirty = true;
                    }
                }
            }

            public uint minMemType1
            {
                get
                {
                    return m_minMemType1;
                }
                set
                {
                    if (value != m_minMemType1)
                    {
                        m_minMemType1 = value;
                        m_bDirty = true;
                    }
                }
            }

            public uint maxMemType1
            {
                get
                {
                    return m_maxMemType1;
                }
                set
                {
                    if (value != m_maxMemType1)
                    {
                        m_maxMemType1 = value;
                        m_bDirty = true;
                    }
                }
            }

            // NOTE: I don't believe we want to allow this to be set here
            // since we automatically adjust this when we add index and name information
            public ushort numberOfGlyphs
            {                
                get
                {
                    if( m_Version.GetUint() != 0x00020000 )
                    {
                        throw new System.InvalidOperationException();
                    }
                    return m_numberOfGlyphs;
                }                
            }

            public ushort getGlyphNameIndex(ushort nGlyphIndex)
            {
                if( nGlyphIndex < m_glyphNameIndex.Count )
                {
                    return (ushort)m_glyphNameIndex[nGlyphIndex];
                }
                return 0;
            }

            public bool setGlyphNameIndex(ushort nGlyphIndex, ushort nValue)
            {
                if ( nGlyphIndex < m_glyphNameIndex.Count)
                {
                    m_glyphNameIndex[nGlyphIndex] = nValue;
                    return true;
                }
                else
                    return false;
            }

            public bool setGlyphNameIndexSize(ushort numGlyphs)
            {
                bool updated = false;
                ushort beginCount = (ushort)m_glyphNameIndex.Count;

                // expand the array size if we need to make it bigger
                for (ushort i = numGlyphs; i > beginCount; i--)
                {
                    // ser are simply adding an entry that is .notdef.
                    m_glyphNameIndex.Add((ushort)0);

                    // increase the number of gyphs since we just added one
                    m_numberOfGlyphs++;                    

                    updated = true;
                    m_bDirty = true;
                }
                return updated;
            }

            public void RebuildNameStringList()
            {
                m_names.Clear();
            }

            public string getNameString(ushort nNameIndex)
            {
                if (nNameIndex < m_names.Count)
                {
                    return m_names[nNameIndex].ToString();
                }
                else
                {
                    return "Out of range";
                }
            }
            
            public string GetGlyphName(ushort glyphId)
            {
                ushort nameIndex = getGlyphNameIndex(glyphId);
           
                if (nameIndex < StandardPostNames.MacNameCount)
                {
                    return (string)StandardPostNames.GetMacName(nameIndex);
                }
                else 
                {
                    return getNameString((ushort)(nameIndex - StandardPostNames.MacNameCount));
                }
            }

            public int addNameString(string sNameOfGlyph)
            {
                m_bDirty = true;
                return m_names.Add( sNameOfGlyph );
            }

            // If we remove a glyph this lets us remove the index and name
            public bool removeGlyphIndexAndName( ushort nGlyphIndex )
            {
                bool bResult = false;

                if( m_Version.GetUint() != 0x00020000 )
                {                    
                    throw new System.InvalidOperationException();
                }
                else if( (ushort)m_glyphNameIndex[nGlyphIndex] > 257 )
                {
                    // all nems under 258 are Macintosh order and are set so they are not stored here
                    m_names.RemoveAt( (ushort)m_glyphNameIndex[nGlyphIndex] - 258 );                    

                    // Now remove the index entry for this glyph
                    m_glyphNameIndex.RemoveAt( nGlyphIndex );
                    
                    // decrease the number of gyphs since we just removed one
                    m_numberOfGlyphs--;

                    // Fix up the indexes
                    for( int i = 0; i < m_numberOfGlyphs; i++ )
                    {
                        if( (ushort)m_glyphNameIndex[i] >= nGlyphIndex )
                        {
                            m_glyphNameIndex[i] = (ushort)m_glyphNameIndex[i] - 1;    
                        }
                    }

                    m_bDirty = true;
                    bResult = true;
                }

                return bResult;

            }

            // Use to add a brand new glyph entry
            public bool addNewGlyphIndexAndName( ushort nGlyphIndex, string sNameOfGlyph )
            {
                bool bResult = false;

                if( m_Version.GetUint() != 0x00020000 )
                {                    
                    throw new System.InvalidOperationException();
                }
                else if( (ushort)m_glyphNameIndex[nGlyphIndex] > 257 )
                {
                    // all nems under 258 are Macintosh order and are set so 
                    // they are not stored Add the new name to the end of the list
                    m_names.Add( sNameOfGlyph );                    

                    // Now insert the new index entry for this glyph
                    m_glyphNameIndex.Insert( nGlyphIndex - 258, m_names.Count - 1 );
                    
                    // increase the number of gyphs since we just added one
                    m_numberOfGlyphs++;                    

                    m_bDirty = true;
                    bResult = true;
                }

                return bResult;
            }

            // Use to add a band new glyph entry
            public bool changeGlyphName( ushort nGlyphIndex, string sNameOfGlyph )
            {
                bool bResult = false;

                if( m_Version.GetUint() != 0x00020000 )
                {                    
                    throw new System.InvalidOperationException();
                }
                else if( (ushort)m_glyphNameIndex[nGlyphIndex] > 258 )
                {
                    // all nems under 258 are Macintosh order and are set so 
                    // they are not stored Add the new name to the end of the list
                    m_names[(ushort)m_glyphNameIndex[nGlyphIndex] - 258] = sNameOfGlyph;    
                    
                    m_bDirty = true;
                    bResult = true;
                }

                return bResult;
            }

            public override OTTable GenerateTable()
            {
                // create a Motorola Byte Order buffer for the new table
                MBOBuffer newbuf;
                uint nSizeOfIndexAndNames = 0;                

                if( m_Version.GetUint() == 0x00020000 )
                {
                    uint nSizeOfByteArray = 0;
    
                    // Get what the size of the byte array will be
                    for(int i = 0; i < m_names.Count; i++ )
                    {
                        nSizeOfByteArray += (uint)(((string)m_names[i]).Length + 1); 
                    }                    

                    // Add 2 for the ushort numberOfGlyphs
                    nSizeOfIndexAndNames = (uint)(2 + (m_numberOfGlyphs * 2) + nSizeOfByteArray);                    

                }

                newbuf = new MBOBuffer(32 + nSizeOfIndexAndNames);

                newbuf.SetFixed( m_Version, (uint)Table_post.FieldOffsets.Version );
                newbuf.SetFixed( m_italicAngle, (uint)Table_post.FieldOffsets.italicAngle );
                newbuf.SetShort( m_underlinePosition, (uint)Table_post.FieldOffsets.underlinePosition );
                newbuf.SetShort( m_underlineThickness, (uint)Table_post.FieldOffsets.underlineThickness );
                newbuf.SetUint( m_isFixedPitch, (uint)Table_post.FieldOffsets.isFixedPitch );
                newbuf.SetUint( m_minMemType42, (uint)Table_post.FieldOffsets.minMemType42 );
                newbuf.SetUint( m_maxMemType42, (uint)Table_post.FieldOffsets.maxMemType42 );
                newbuf.SetUint( m_minMemType1, (uint)Table_post.FieldOffsets.minMemType1);
                newbuf.SetUint( m_maxMemType1, (uint)Table_post.FieldOffsets.maxMemType1);                


                if( m_Version.GetUint() == 0x00020000 )
                {
                    newbuf.SetUshort( m_numberOfGlyphs, (uint)Table_post.FieldOffsetsVer2.numberOfGlyphs);    

                    uint nOffset = (uint)Table_post.FieldOffsetsVer2.glyphNameIndex;
                    for( int i = 0; i < m_numberOfGlyphs; i++ )
                    {
                        newbuf.SetUshort( (ushort)m_glyphNameIndex[i], nOffset );
                        nOffset += 2;
                    }

                    // write out the names to the buffer in length followed by character bytes
                    for( int i = 0; i < m_names.Count; i++ )
                    {
                        string sName = (string)m_names[i];
                        newbuf.SetByte( (byte)sName.Length,      nOffset );
                        nOffset++;

                        for( int ii = 0; ii < sName.Length; ii++ )
                        {
                            newbuf.SetByte( (byte)sName[ii],      nOffset );
                            nOffset++;
                        }
                    }                
                }
                
                
                // put the buffer into a Table_maxp object and return it
                Table_post postTable = new Table_post("post", newbuf);

                return postTable;            
            }
        }

    }
}
