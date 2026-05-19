using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Enums {

  public enum WeDataType {
    None = -1,
    Reference = (int)WeItemType.CSharpClassType,
    StrAscii = (int)WeItemType.CSharpStringType,
    Boolean = (int)WeItemType.CSharpBoolType,
    Char = (int)WeItemType.CSharpCharType,  
    
    Int = (int)WeItemType.CSharpIntType,
    Long = (int)WeItemType.CSharpLongType,
    Short = (int)WeItemType.CSharpShortType,    

    Decimal = (int)WeItemType.CSharpDecimalType,
    Double = (int)WeItemType.CSharpDoubleType,          // Universal name for 'double'    
    Float = (int)WeItemType.CSharpFloatType,          // Universal name for 'float'

    Byte = (int)WeItemType.CSharpByteType,

    DateTime = (int)WeItemType.CSharpDateTimeType,
    Date = (int)WeItemType.CSharpDateType,
    Time = (int)WeItemType.CSharpTimeType,    
    DateTimeOffset = (int)WeItemType.CSharpDateTimeOffsetType,

    Binary = (int)WeItemType.CSharpByteArrayType,    
    Guid = (int)WeItemType.CSharpGuidType,
  }


  public enum WeSqlDataType {
    None = -1,
    Bit = (int)WeItemType.SqlBitType,    
    SmallInt = (int)WeItemType.SqlSmallIntType,
    Int = (int)WeItemType.SqlIntType,
    BigInt = (int)WeItemType.SqlBigIntType,
    UniqueIdentifier = (int)WeItemType.SqlGuidType,
    VarChar = (int)WeItemType.SqlVarcharType,
    NVarChar = (int)WeItemType.SqlNVarcharType,
    Float = (int)WeItemType.SqlFloatType,    
    Decimal = (int)WeItemType.SqlDecimalType,
    DateTime = (int)WeItemType.SqlDateTimeType,    
    Date = (int)WeItemType.SqlDateType,
    Time = (int)WeItemType.SqlTimeType,
    DateTimeOffset = (int)WeItemType.SqlDateTimeOffsetType,
    VarBinary = (int)WeItemType.SqlBinaryType
  }

  public enum WeCSharpDataType {
    None = -1,
    Class = (int)WeItemType.CSharpClassType,
    Record = (int)WeItemType.CSharpRecordType,
    Struct = (int)WeItemType.CSharpStructType,
    String = (int)WeItemType.CSharpStringType,

    Bool = (int)WeItemType.CSharpBoolType,
    Char = (int)WeItemType.CSharpCharType,    
    Int = (int)WeItemType.CSharpIntType,
    Long = (int)WeItemType.CSharpLongType,
    Short = (int)WeItemType.CSharpShortType,

    Decimal = (int)WeItemType.CSharpDecimalType,
    Double = (int)WeItemType.CSharpDoubleType,
    Float = (int)WeItemType.CSharpFloatType,

    Byte = (int)WeItemType.CSharpByteType,
    DateTime = (int)WeItemType.CSharpDateTimeType,
    Date = (int)WeItemType.CSharpDateType,
    Time = (int)WeItemType.CSharpTimeType,
    TimeSpan = (int)WeItemType.CSharpDateTimeOffsetType,
    ByteArray = (int)WeItemType.CSharpByteArrayType,
    Guid = (int)WeItemType.CSharpGuidType
  }
}
