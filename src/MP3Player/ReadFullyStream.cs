﻿using System;
using System.IO;

namespace MP3Player
{
    public class ReadFullyStream : Stream
    {
        private const int BufferSize = 65536;
        private readonly Stream _sourceStream;
        private long _pos; // psuedo-position
        private readonly byte[] _readAheadBuffer;
        private int _readAheadLength;
        private int _readAheadOffset;

        public ReadFullyStream(Stream sourceStream, long contentLength)
        {
            _sourceStream = sourceStream;
            Length = contentLength;
            _readAheadBuffer = new byte[BufferSize];
        }
        public override bool CanRead => _sourceStream.CanRead;

        public override bool CanSeek => _sourceStream.CanSeek;

        public override bool CanWrite => false;

        public override void Flush()
        {
            throw new InvalidOperationException();
        }

        public override long Length { get; }

        public override long Position
        {
            get => _pos;
            set => Seek(value, SeekOrigin.Begin);
        }
        
        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = 0;
            while (bytesRead < count)
            {
                int readAheadAvailableBytes = _readAheadLength - _readAheadOffset;
                int bytesRequired = count - bytesRead;
                if (readAheadAvailableBytes > 0)
                {
                    int toCopy = Math.Min(readAheadAvailableBytes, bytesRequired);
                    Array.Copy(_readAheadBuffer, _readAheadOffset, buffer, offset + bytesRead, toCopy);
                    bytesRead += toCopy;
                    _readAheadOffset += toCopy;
                }
                else
                {
                    _readAheadOffset = 0;
                    _readAheadLength = _sourceStream.Read(_readAheadBuffer, 0, _readAheadBuffer.Length);
                    if (_readAheadLength == 0)
                    {
                        break;
                    }
                }
            }
            _pos += bytesRead;
            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (CanSeek)
            {
                _readAheadLength = 0;
                _readAheadOffset = 0;
                return _pos = _sourceStream.Seek(offset, origin);
            }

            return 0;
        }

        public override void SetLength(long value)
        {
            _sourceStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException();
        }

        protected override void Dispose(bool disposing)
        {
            _sourceStream?.Dispose();
            base.Dispose(disposing);
        }
    }
}
