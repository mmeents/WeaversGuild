using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Enums {

  public enum WeDataType {
    None = -1,
    Boolean = 1,
    Int16 = 2,
    Int32 = 3,
    Int64 = 4,
    Guid = 5,
    StrAscii = 6,
    StrUnicode = 7,
    Float32 = 8,          // Universal name for 'float'
    Float64 = 9,          // Universal name for 'double'
    Decimal128 = 10,
    Date = 11,
    Time = 12,
    SmallDateTime = 13,
    DateTime = 14,
    DateTime2 = 15,
    DateTimeOffset = 16,
    Binary = 17,
    Reference = 18
  }


  public enum WeSqlDataType {
    None = -1,
    Bit = 11,    
    SmallInt = 12,
    Int = 13,
    BigInt = 14,
    UniqueIdentifier = 16,
    VarChar = 18,
    NVarChar = 20,
    Float = 21,    
    Decimal = 22,
    DateTime = 24,
    DateTime2 = 25,
    Date = 26,
    Time = 28,
    
    DateTimeOffset = 29,
    VarBinary = 30
  }

  public enum WeCSharpDataType {
    None = -1,
    Class = 52,
    Record = 54,
    Struct = 56,
    String = 58,

    Bool = 60,
    Char = 62,    
    Int = 64,
    Long = 66,
    Short = 68,
    Decimal = 70,
    Double = 72,
    Float = 74,
    Byte = 76,
    DateTime = 78,
    Date = 80,
    Time=82,
    TimeSpan = 84,
    ByteArray = 86,
    Guid = 88
  }
}
