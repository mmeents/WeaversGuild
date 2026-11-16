using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weavers.Core.Extensions {
  public sealed class ByteArrayComparer : IEqualityComparer<byte[]> {
    public static readonly ByteArrayComparer Instance = new();
    public bool Equals(byte[]? x, byte[]? y) => x.AsSpan().SequenceEqual(y);
    // SHA-256 output is uniformly distributed; the first 4 bytes make a fine hash code.
    public int GetHashCode(byte[] b) => b.Length >= 4 ? BinaryPrimitives.ReadInt32LittleEndian(b) : b.Length;
  }
}
