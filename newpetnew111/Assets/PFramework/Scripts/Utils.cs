using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace PFramework
{
    public static class NumberUtil
    {
        public static string GetNumberWithDot(float number, bool showFloatValue = false)
        {
            if (number > Mathf.FloorToInt(number))
            {
                return String.Format("{0:N}", number);
            }
            else if (showFloatValue)
            {
                return string.Format("{0:N2}", number);
            }
            else
            {
                return string.Format("{0:N0}", Mathf.FloorToInt(number));
            }
        }
    }

    /// <summary>
    /// 反射工具类
    /// </summary>
    public static class ReflectionUtil
    {

    }

    /// <summary>
    /// 文件工具类
    /// </summary>
    public static class FileUtil
    {
        public static void ClearDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(path);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
        }

        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void WriteFile(string writePath,byte[] data)
        {
            var file = File.Create(writePath);
            file.Flush();
            file.Write(data, 0, data.Length);
            file.Close();
        }
    }

    /// <summary>
    /// 时间工具类
    /// </summary>
    public static class TimeUtil
    {
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = Epoch;
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dtDateTime;
        }

        public static int DateTimeToUnixTimeStamp(DateTime time)
        {
            DateTime dtDateTime = Epoch;
            return (int)(time.ToUniversalTime() - dtDateTime).TotalSeconds;
        }

        /// <summary>
        /// 获取已经过去的时间，为了防止程序错误，改时间出现bug，抛弃负的值
        /// </summary>
        /// <param name="later"></param>
        /// <param name="previous"></param>
        /// <returns></returns>
        public static TimeSpan TimePast(DateTime later, DateTime previous)
        {
            var result = later - previous;
            result = result.TotalSeconds >= 0 ? result : TimeSpan.Zero;
            return result;
        }

        public static int MonthsPast(DateTime later, DateTime previous)
        {
            if(later < previous)
            {
                return 0;
            }

            int year = later.Year - previous.Year;
            int months = 0;
            if(later.Month < previous.Month)
            {
                months += 12 + later.Month - previous.Month;
                year--;
            }
            months += year * 12;
            return months;
        }
    }

    /// <summary>
    /// 加密工具类
    /// </summary>
    public static class SecurityUtils
    {
        public static string Base64Encode(string s)
        {
            byte[] bytesToEncode = Encoding.UTF8.GetBytes(s);
            string encodedText = Convert.ToBase64String(bytesToEncode);
            return encodedText;
        }

        public static string Base64Dncode(string s)
        {
            byte[] decodedBytes = Convert.FromBase64String(s);
            string decodedText = Encoding.UTF8.GetString(decodedBytes);
            return decodedText;
        }

        public static string GetMd5Hash(String input)
        {
            if (input == null)
            {
                return null;
            }
            MD5 md5Hash = MD5.Create();

            // 将输入字符串转换为字节数组并计算哈希数据
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // 创建一个 Stringbuilder 来收集字节并创建字符串
            StringBuilder sBuilder = new StringBuilder();

            // 循环遍历哈希数据的每一个字节并格式化为十六进制字符串
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            // 返回十六进制字符串
            return sBuilder.ToString();
        }

        /// <summary>
        ///  AES 加密
        /// </summary>
        /// <param name="str">明文（待加密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public static string AesEncrypt(string str, string key)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            };
            Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);

            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rm.CreateEncryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="str">明文（待解密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public static string AesDecrypt(string str, string key)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            Byte[] toEncryptArray = Convert.FromBase64String(str);

            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(key),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rm.CreateDecryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }

        /// <summary>
        /// 随机生成密钥
        /// </summary>
        /// <returns></returns>
        public static string CreateAESCriptKey()
        {
            int keySize = 16;
            char[] arrChar = new char[]{
           'a','b','d','c','e','f','g','h','i','j','k','l','m','n','p','r','q','s','t','u','v','w','z','y','x',
           '0','1','2','3','4','5','6','7','8','9',
           'A','B','C','D','E','F','G','H','I','J','K','L','M','N','Q','P','R','T','S','V','U','W','X','Y','Z'
          };

            StringBuilder num = new StringBuilder();

            System.Random rnd = new System.Random(DateTime.Now.Millisecond);
            for (int i = 0; i < keySize; i++)
            {
                num.Append(arrChar[rnd.Next(0, arrChar.Length)].ToString());

            }

            return num.ToString();
        }
    }
}
