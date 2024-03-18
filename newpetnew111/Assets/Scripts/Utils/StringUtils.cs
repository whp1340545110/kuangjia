using Kkxx;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/**
 *
 *
 *
 */
namespace Kkxx
{

    public class StringUtils
    {
        private static Regex emailRegex = new Regex("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$");
        private static Regex phoneRegex = new Regex("^(\\d{3.4}-)\\d{7,8}$");
        //
        public static bool IsEmail(string emailStr)
        {
            if (emailRegex.IsMatch(emailStr))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //
        public static bool IsPhone(string phoneStr)
        {
            if (phoneRegex.IsMatch(phoneStr))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public static bool IsNullOrEmpty(string str)
        {
            if (str == null || str == string.Empty)
            {
                return true;
            }
            return false;
        }

        public static string TixianMoneyFormat(int money)
        {
            return "$" + (money/1.0f).ToString("0.00");
        }


    }
}
