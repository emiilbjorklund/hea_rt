using System;
using System.Collections.Generic;
using System.Text;

namespace Basestation.Common.gRPC
{
    public abstract class BufferedSubscriber<T> : DataSubscriber<T>
    {
        private int _bufferSize;

        public BufferedSubscriber(string address, int bufferSize) : base(address)
        {
            _bufferSize = bufferSize;
        }

        public void WriteData(T data)
        {
            lock (Buffer)
            {
                if (Buffer != null)
                {
                    Buffer.Add(data);
                    while (Buffer.Count > _bufferSize)
                        Buffer.RemoveAt(0);
                }
            }
        }

        public List<T> Buffer { get; set; }
    }
}
