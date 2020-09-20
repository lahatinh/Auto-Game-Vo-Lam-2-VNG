using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoClient
{
    class fontConvert
    {
        private static char[] tcvnchars = {
                                              'µ', '¸', '¶', '·', '¹',
                                              '¨', '»', '¾', '¼', '½', 'Æ',
                                              '©', 'Ç', 'Ê', 'È', 'É', 'Ë',
                                              '®', 'Ì', 'Ð', 'Î', 'Ï', 'Ñ',
                                              'ª', 'Ò', 'Õ', 'Ó', 'Ô', 'Ö',
                                              '×', 'Ý', 'Ø', 'Ü', 'Þ',
                                              'ß', 'ã', 'á', 'â', 'ä',
                                              '«', 'å', 'è', 'æ', 'ç', 'é',
                                              '¬', 'ê', 'í', 'ë', 'ì', 'î',
                                              'ï', 'ó', 'ñ', 'ò', 'ô',
                                              '­', 'õ', 'ø', 'ö', '÷', 'ù',
                                              'ú', 'ý', 'û', 'ü', 'þ',
                                              '¡', '¢', '§', '£', '¤', '¥', '¦'
                                          };

        private static char[] unichars = {
                                             'à', 'á', 'ả', 'ã', 'ạ',
                                             'ă', 'ằ', 'ắ', 'ẳ', 'ẵ', 'ặ',
                                             'â', 'ầ', 'ấ', 'ẩ', 'ẫ', 'ậ',
                                             'đ', 'è', 'é', 'ẻ', 'ẽ', 'ẹ',
                                             'ê', 'ề', 'ế', 'ể', 'ễ', 'ệ',
                                             'ì', 'í', 'ỉ', 'ĩ', 'ị',
                                             'ò', 'ó', 'ỏ', 'õ', 'ọ',
                                             'ô', 'ồ', 'ố', 'ổ', 'ỗ', 'ộ',
                                             'ơ', 'ờ', 'ớ', 'ở', 'ỡ', 'ợ',
                                             'ù', 'ú', 'ủ', 'ũ', 'ụ',
                                             'ư', 'ừ', 'ứ', 'ử', 'ữ', 'ự',
                                             'ỳ', 'ý', 'ỷ', 'ỹ', 'ỵ',
                                             'Ă', 'Â', 'Đ', 'Ê', 'Ô', 'Ơ', 'Ư'
                                         };

        private static char[] nomarkchars = {
                                             'a', 'a', 'a', 'a', 'a',
                                             'a', 'a', 'a', 'a', 'a', 'a',
                                             'a', 'a', 'a', 'a', 'a', 'a',
                                             'd', 'e', 'e', 'e', 'e', 'e',
                                             'e', 'e', 'e', 'e', 'e', 'e',
                                             'i', 'i', 'i', 'i', 'i',
                                             'o', 'o', 'o', 'o', 'o',
                                             'o', 'o', 'o', 'o', 'o', 'o',
                                             'o', 'o', 'o', 'o', 'o', 'o',
                                             'u', 'u', 'u', 'u', 'u',
                                             'u', 'u', 'u', 'u', 'u', 'u',
                                             'y', 'y', 'y', 'y', 'y',
                                             'A', 'A', 'D', 'E', 'O', 'O', 'U'
                                         };

        private static char[] convertTable;
        private static char[] convertNoMarkTable;

        private static char[] _tcvnchars;

        private static char[] _unichars;

        private static readonly char[] ConvertTableToUnicode;

        private static readonly char[] ConvertTableToTcvn3;



        public static string ConvertTcvn3ToUnicode(string value)
        {
            char[] array = value.ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] < 'Ā')
                {
                    array[i] = fontConvert.ConvertTableToUnicode[(int)array[i]];
                }
            }
            return new string(array);
        }

        public static string ConvertUnicodeToTcvn3(string value)
        {
            char[] array = value.ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = fontConvert.ConvertTableToTcvn3[(int)array[i]];
            }
            return new string(array);
        }



        static fontConvert()
        {
            convertTable = new char[256];
            for (int i = 0; i < 256; i++)
                convertTable[i] = (char)i;
            convertNoMarkTable = new char[256];
            for (int i = 0; i < 256; i++)
                convertNoMarkTable[i] = (char)i;
            for (int i = 0; i < tcvnchars.Length; i++)
                convertTable[tcvnchars[i]] = unichars[i];

            for (int i = 0; i < tcvnchars.Length; i++)
                convertNoMarkTable[tcvnchars[i]] = nomarkchars[i];
            fontConvert._tcvnchars = new char[]
			{
				'µ',
				'¸',
				'¶',
				'·',
				'¹',
				'¨',
				'»',
				'¾',
				'¼',
				'½',
				'Æ',
				'©',
				'Ç',
				'Ê',
				'È',
				'É',
				'Ë',
				'®',
				'Ì',
				'Ð',
				'Î',
				'Ï',
				'Ñ',
				'ª',
				'Ò',
				'Õ',
				'Ó',
				'Ô',
				'Ö',
				'×',
				'Ý',
				'Ø',
				'Ü',
				'Þ',
				'ß',
				'ã',
				'á',
				'â',
				'ä',
				'«',
				'å',
				'è',
				'æ',
				'ç',
				'é',
				'¬',
				'ê',
				'í',
				'ë',
				'ì',
				'î',
				'ï',
				'ó',
				'ñ',
				'ò',
				'ô',
				'­',
				'õ',
				'ø',
				'ö',
				'÷',
				'ù',
				'ú',
				'ý',
				'û',
				'ü',
				'þ',
				'¡',
				'¢',
				'§',
				'£',
				'¤',
				'¥',
				'¦'
			};
            fontConvert._unichars = new char[]
			{
				'à',
				'á',
				'ả',
				'ã',
				'ạ',
				'ă',
				'ằ',
				'ắ',
				'ẳ',
				'ẵ',
				'ặ',
				'â',
				'ầ',
				'ấ',
				'ẩ',
				'ẫ',
				'ậ',
				'đ',
				'è',
				'é',
				'ẻ',
				'ẽ',
				'ẹ',
				'ê',
				'ề',
				'ế',
				'ể',
				'ễ',
				'ệ',
				'ì',
				'í',
				'ỉ',
				'ĩ',
				'ị',
				'ò',
				'ó',
				'ỏ',
				'õ',
				'ọ',
				'ô',
				'ồ',
				'ố',
				'ổ',
				'ỗ',
				'ộ',
				'ơ',
				'ờ',
				'ớ',
				'ở',
				'ỡ',
				'ợ',
				'ù',
				'ú',
				'ủ',
				'ũ',
				'ụ',
				'ư',
				'ừ',
				'ứ',
				'ử',
				'ữ',
				'ự',
				'ỳ',
				'ý',
				'ỷ',
				'ỹ',
				'ỵ',
				'Ă',
				'Â',
				'Đ',
				'Ê',
				'Ô',
				'Ơ',
				'Ư'
			};
            fontConvert.ConvertTableToUnicode = new char[256];
            fontConvert.ConvertTableToTcvn3 = new char[65536];
            for (int i = 0; i < 65536; i++)
            {
                if (i < 256)
                {
                    fontConvert.ConvertTableToUnicode[i] = (char)i;
                }
                fontConvert.ConvertTableToTcvn3[i] = (char)i;
            }
            for (int j = 0; j < fontConvert._tcvnchars.Length; j++)
            {
                fontConvert.ConvertTableToUnicode[(int)fontConvert._tcvnchars[j]] = fontConvert._unichars[j];
                fontConvert.ConvertTableToTcvn3[(int)fontConvert._unichars[j]] = fontConvert._tcvnchars[j];
            }
        }

        public static string TCVN3ToUnicode(string value)
        {
            char[] chars = value.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
                if (chars[i] < (char)256)
                    chars[i] = convertTable[chars[i]];
            return new string(chars);
        }

        public static string TCVN3ToNoMark(string value)
        {
            char[] chars = value.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
                if (chars[i] < (char)256)
                    chars[i] = convertNoMarkTable[chars[i]];

            return new string(chars);
        }
        public static string convertUnicodeToNomark(string s)
        {
            string stFormD = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }
            sb = sb.Replace('Đ', 'D');
            sb = sb.Replace('đ', 'd');
            return (sb.ToString().Normalize(NormalizationForm.FormD));
        }


        public static string convertUnicodeToTCVN3(string s)
        {
            string stFormD = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }
            sb = sb.Replace('Đ', '§');
            sb = sb.Replace('đ', '®');
            return (sb.ToString().Normalize(NormalizationForm.FormD));
        }




        public static string convertNomarkToUnicode(string s)
        {
            string stFormD = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }
            sb = sb.Replace('D', 'Đ');
            sb = sb.Replace('d', 'đ');
            return (sb.ToString().Normalize(NormalizationForm.FormD));
        }
    }
}
