using System;
using System.Buffers;
using System.IO;

namespace Excel.File.Service.Service.Extensions
{
    public static class StreamExtensions
    {
        public static byte[] ToByteArray(this FileStream stream)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);

                return memoryStream.ToArray();
            }
        }

        public static void WriteAsync(this FileStream stream, ReadOnlySpan<byte> buffer)
        {
            byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);

            try
            {
                buffer.CopyTo(sharedBuffer);

                stream.WriteAsync(sharedBuffer, 0, buffer.Length);
            }
            finally 
            { 
                ArrayPool<byte>.Shared.Return(sharedBuffer); 
            }
        }        
    }
}
