using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace Karaboss.Pages.ABCnotation
{
    public class StringExtensions
    {
        private enum ToLowerMode { INITIAL, FIRSTCHAR, NONFIRST, WHITESPACE };
        public static String ToTitleCase(String str)
        {//====================================================================           
            ToLowerMode mode = ToLowerMode.INITIAL;
            char[] ach = str.ToCharArray();
            for (int i = 0; i < ach.Length; i += 1)
            {
                switch (mode)
                {
                    default:
                        break;

                    case ToLowerMode.INITIAL:
                    case ToLowerMode.WHITESPACE:
                        if (Char.IsLetterOrDigit(ach[i])) { ach[i] = Char.ToUpper(ach[i]); mode = ToLowerMode.FIRSTCHAR; }
                        break;

                    case ToLowerMode.FIRSTCHAR:
                    case ToLowerMode.NONFIRST:
                        ach[i] = Char.ToLower(ach[i]);
                        mode = Char.IsWhiteSpace(ach[i]) ? ToLowerMode.WHITESPACE : ToLowerMode.NONFIRST;
                        break;

                }
            }
            return new String(ach);
        }

        public static String RightOf(String str, String strSep)
        {//====================================================================
            if (strSep.Length > 0) str = str.Substring(str.IndexOf(strSep) + strSep.Length).Trim();
            return str.Trim();
        }
        public static String ConcatList(String strOld, String strNew, String strSep)
        {//--------------------------------------------------------------------
            if (strOld.Length > 0) return strOld + ", " + RightOf(strNew, strSep);
            return RightOf(strNew, strSep);
        }

        public static String ConcatLines(String strOld, String strNew, String strSep)
        {//--------------------------------------------------------------------
            if (strOld.Length > 0) return strOld + "\n" + RightOf(strNew, strSep);
            return RightOf(strNew, strSep);
        }

        private enum ConvertState { CHARACTER, CR = 0x0d, LF = 0x0a };
        public static String ConvertNonDosFile(String str)
        {//====================================================================
            // No, this isn't very fast, is it?
            String strRet = String.Empty;

            ConvertState state = ConvertState.CHARACTER;
            foreach (char c in str.ToCharArray())
            {
                switch (c)
                {
                    default:
                        if (state == ConvertState.CR) strRet += (char)ConvertState.LF;  // If we added a CR but no LF, add the LF
                        strRet += c;
                        state = ConvertState.CHARACTER;
                        break;

                    case (char)ConvertState.CR:
                        strRet += (char)ConvertState.CR;
                        state = ConvertState.CR;
                        break;

                    case (char)ConvertState.LF:
                        if (state != ConvertState.CR) strRet += (char)ConvertState.CR;  // If we didn't just add a CR, insert it now
                        strRet += (char)ConvertState.LF;
                        state = ConvertState.LF;
                        break;
                }
            }
            return strRet;
        }

        public static string ToString(object o)
        {   //====================================================================
            // For testing serialization
            StringBuilder sb = new StringBuilder();
            XmlSerializer serializer = new XmlSerializer(o.GetType());
            XmlRootAttribute root = new XmlRootAttribute(o.GetType().Name);
            root.Namespace = "http://schemas.microsoft.com/crm/2006/WebServices";

            TextWriter writer = new StringWriter(sb);
            serializer.Serialize(writer, o);

            return sb.ToString();
        }

    }

    public class ObjectUtils
    {
        public static string GenerateKey(int nLen)
        {   //====================================================================

            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            byte[] data = new byte[nLen];
            crypto.GetNonZeroBytes(data);

            StringBuilder result = new StringBuilder(nLen * 2);
            foreach (byte b in data) result.Append(((int)b).ToString("X2"));
            return result.ToString();
        }

    }
}
