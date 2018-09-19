using System;
using System.Data;

namespace ZWT.DbLib
{
    /// <summary>
    /// Summary description for DBUtils.
    /// </summary>
    public class DBUtils
    {
        private DataRow _dataRow;

        public DBUtils(DataRow dr)
        {
            _dataRow = dr;
        }

        public System.Guid GetGuid(string fieldName)
        {
            if (_dataRow[fieldName] == DBNull.Value)
                return System.Guid.Empty;
            else
                return (System.Guid)_dataRow[fieldName];
        }

        public byte[] GetBytes(string fieldName)
        {
            if (_dataRow[fieldName] == DBNull.Value)
                return null;
            else
                return (byte[])_dataRow[fieldName];
        }

        public string GetValue(string fieldName, string defaultValue)
        {
            if (_dataRow[fieldName] == DBNull.Value)
                return defaultValue;
            else
                return _dataRow[fieldName].ToString();
        }


        public short GetValue(string fieldName, short defaultValue)
        {
            if (_dataRow[fieldName] == DBNull.Value)
                return defaultValue;
            else
                return Convert.ToInt16(_dataRow[fieldName]);
        }
        public Boolean GetValue(string fieldName, Boolean defaultValue)
        {
            if (_dataRow[fieldName] == DBNull.Value)
                return defaultValue;
            else
                return Convert.ToBoolean(_dataRow[fieldName]);
        }


        public Int32 GetValue(string fieldName, Int32 defaultValue)
        {
            if (_dataRow[fieldName] == DBNull.Value)
                return defaultValue;
            else
                return Convert.ToInt32(_dataRow[fieldName]);
        }


        public DateTime GetValue(string fieldName, DateTime defaultValue)
        {
            if (_dataRow[fieldName] == DBNull.Value)
                return defaultValue;
            else
                return Convert.ToDateTime(_dataRow[fieldName]);
        }


        public char GetValue(string fieldName, char defaultValue)
        {
            if (_dataRow[fieldName] == DBNull.Value)
                return defaultValue;
            else
                return Convert.ToChar(_dataRow[fieldName]);
        }


        public Decimal GetValue(string fieldName, Decimal defaultValue)
        {
            if (_dataRow[fieldName] == DBNull.Value)
                return defaultValue;
            else
                return Convert.ToDecimal(_dataRow[fieldName]);
        }
    }
}
