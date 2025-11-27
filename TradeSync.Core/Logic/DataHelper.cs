using System;

namespace TradeSync.Core.Logic
{
    public static class DataHelper
    {
        // Конвертація 1С Binary(16) -> C# Guid
        public static object ConvertToGuid(object value)
        {
            if (value == DBNull.Value || value == null)
                return DBNull.Value;

            try
            {
                // Якщо 1С повернула масив байтів
                if (value is byte[] bytes && bytes.Length == 16)
                {
                    return new Guid(bytes);
                }
            }
            catch { }

            return DBNull.Value;
        }

        // Конвертація 1С Binary(1)/Numeric -> C# bool
        public static object ConvertToBool(object value)
        {
            if (value == DBNull.Value || value == null)
                return false;

            // Варіант 1: Це масив байт (0x01 або 0x00)
            if (value is byte[] bytes && bytes.Length > 0)
            {
                return bytes[0] != 0;
            }

            // Варіант 2: Це число (1 або 0)
            try
            {
                return Convert.ToBoolean(value);
            }
            catch { return false; }
        }
    }
}