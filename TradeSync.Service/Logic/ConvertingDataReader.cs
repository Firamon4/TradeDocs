using System.Data;
using TradeSync.Core.Logic;
using TradeSync.Core.Models;

namespace TradeSync.Service.Logic
{
    /// <summary>
    /// Обгортка над IDataReader для конвертації типів "на льоту" без завантаження в пам'ять.
    /// </summary>
    public class ConvertingDataReader : IDataReader
    {
        private readonly IDataReader _source;
        private readonly List<ColumnInfo> _columns;

        public ConvertingDataReader(IDataReader source, TableSchema schema)
        {
            _source = source;
            _columns = schema.Columns;
        }

        // Головний метод: читає значення і конвертує його
        public object GetValue(int i)
        {
            var val = _source.GetValue(i);
            if (val == DBNull.Value || val == null) return DBNull.Value;

            // Отримуємо цільовий тип з нашої схеми JSON
            var targetTypeString = _columns[i].Type;

            try
            {
                return targetTypeString switch
                {
                    "Guid" => DataHelper.ConvertToGuid(val),
                    "Boolean" => DataHelper.ConvertToBool(val),
                    "Decimal" => Convert.ToDecimal(val),
                    "Int" => Convert.ToInt32(val),
                    "Binary" => (byte[])val,
                    "DateTime" => Convert.ToDateTime(val),
                    _ => val.ToString() // String
                };
            }
            catch
            {
                return DBNull.Value; // Якщо конвертація впала - повертаємо NULL
            }
        }

        // --- Перенаправлення інших методів до джерела ---
        public bool Read() => _source.Read();
        public void Close() => _source.Close();
        public void Dispose() => _source.Dispose();
        public int FieldCount => _source.FieldCount;
        public string GetName(int i) => _source.GetName(i);
        public int GetOrdinal(string name) => _source.GetOrdinal(name);
        public bool IsDBNull(int i) => _source.IsDBNull(i);

        // Решта методів інтерфейсу (рідко використовуються BulkCopy, але треба реалізувати)
        public int Depth => _source.Depth;
        public bool IsClosed => _source.IsClosed;
        public int RecordsAffected => _source.RecordsAffected;
        public object this[string name] => GetValue(GetOrdinal(name));
        public object this[int i] => GetValue(i);

        public bool GetBoolean(int i) => (bool)GetValue(i);
        public byte GetByte(int i) => (byte)GetValue(i);
        public long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length) => _source.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        public char GetChar(int i) => (char)GetValue(i);
        public long GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length) => _source.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        public IDataReader GetData(int i) => _source.GetData(i);
        public string GetDataTypeName(int i) => _source.GetDataTypeName(i);
        public DateTime GetDateTime(int i) => (DateTime)GetValue(i);
        public decimal GetDecimal(int i) => (decimal)GetValue(i);
        public double GetDouble(int i) => (double)GetValue(i);
        public Type GetFieldType(int i) => _source.GetFieldType(i);
        public float GetFloat(int i) => (float)GetValue(i);
        public Guid GetGuid(int i) => (Guid)GetValue(i);
        public short GetInt16(int i) => (short)GetValue(i);
        public int GetInt32(int i) => (int)GetValue(i);
        public long GetInt64(int i) => (long)GetValue(i);
        public string GetString(int i) => (string)GetValue(i);
        public int GetValues(object[] values)
        {
            int count = Math.Min(values.Length, FieldCount);
            for (int i = 0; i < count; i++) values[i] = GetValue(i);
            return count;
        }
        public DataTable? GetSchemaTable() => _source.GetSchemaTable();
        public bool NextResult() => _source.NextResult();
    }
}