using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PFramework.Cfg
{
    public static class EEUtility
    {
        public static bool IsExcelFileSupported(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;
            var fileName = Path.GetFileName(filePath);
            if (fileName.Contains("~$"))// avoid temporary files
                return false;
            var lower = Path.GetExtension(filePath).ToLower();
            return lower == ".xlsx" || lower == ".xls" || lower == ".xlsm";
        }

        public static string GetFieldComment(Type classType, string fieldName)
        {
            try
            {
                var fld = classType.GetField(fieldName);
                var comment = fld.GetCustomAttributes(typeof(EECommentAttribute), true)[0] as EECommentAttribute;
                return comment != null ? comment.content : null;
            }
            catch
            {
                // ignored
            }

            return null;
        }

        public static PropertyInfo GetRowDataKeyProperty(Type rowDataType)
        {
            var properties = rowDataType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var keyProperty = (from propertyInfo in properties
                            let attrs = propertyInfo.GetCustomAttributes(typeof(EEKeyPropertyAttribute), false)
                            where attrs.Length > 0
                            select propertyInfo).FirstOrDefault();
            return keyProperty;
        }
    }
}
