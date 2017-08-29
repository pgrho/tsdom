using System.IO;
using System.Text;
using System.Threading.Tasks;

#if NETSTANDARD1_3

namespace System.CodeDom.Compiler
{
    public sealed class IndentedTextWriter : TextWriter
    {
        private readonly TextWriter _BaseWriter;
        private bool _LineWritten;

        public IndentedTextWriter(TextWriter baseWriter)
        {
            _BaseWriter = baseWriter;
        }

        public override Encoding Encoding
            => _BaseWriter.Encoding;

        public override IFormatProvider FormatProvider
            => _BaseWriter.FormatProvider;

        public override string NewLine
        {
            get => _BaseWriter.NewLine;
            set => _BaseWriter.NewLine = value;
        }

        public int Indent { get; set; }

        #region Flush

        public override void Flush()
            => _BaseWriter.Flush();

        public override Task FlushAsync()
            => _BaseWriter.FlushAsync();

        #endregion Flush

        #region Write

        public override void Write(bool value)
        {
            WriteIndentIfNeeded();
            _BaseWriter.Write(value);
            _LineWritten = true;
        }

        public override void Write(char value)
        {
            WriteIndentIfNeeded();
            _BaseWriter.Write(value);
            _LineWritten = true;
        }

        public override void Write(char[] buffer)
        {
            WriteIndentIfNeeded();
            _BaseWriter.Write(buffer);
            _LineWritten = true;
        }

        public override void Write(char[] buffer, int index, int count)
        {
            WriteIndentIfNeeded();
            _BaseWriter.Write(buffer, index, count);
            _LineWritten = true;
        }

        public override void Write(decimal value)
        {
            WriteIndentIfNeeded();
            _BaseWriter.Write(value);
            _LineWritten = true;
        }

        public override void Write(double value)
        {
            WriteIndentIfNeeded();
            _BaseWriter.Write(value);
            _LineWritten = true;
        }

        public override void Write(float value)
        {
            WriteIndentIfNeeded();
            _BaseWriter.Write(value);
            _LineWritten = true;
        }

        public override void Write(int value)
        {
            WriteIndentIfNeeded();
            _BaseWriter.Write(value);
            _LineWritten = true;
        }

        public override void Write(long value)
        {
            WriteIndentIfNeeded();
            _BaseWriter.Write(value);
            _LineWritten = true;
        }

        public override void Write(object value)
        {
            WriteIndentIfNeeded();
            _BaseWriter.Write(value);
            _LineWritten = true;
        }

        public override void Write(string format, object arg0)
        {
            WriteIndentIfNeeded();
            _BaseWriter.Write(format, arg0);
            _LineWritten = true;
        }

        public override void Write(string format, object arg0, object arg1)
        {
            WriteIndentIfNeeded();
            _BaseWriter.Write(format, arg0, arg1);
            _LineWritten = true;
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            WriteIndentIfNeeded();
            _BaseWriter.Write(format, arg0, arg1, arg2);
            _LineWritten = true;
        }

        public override void Write(string format, params object[] arg)
        {
            WriteIndentIfNeeded();
            _BaseWriter.Write(format, arg);
            _LineWritten = true;
        }

        public override void Write(string value)
        {
            WriteIndentIfNeeded();
            _BaseWriter.Write(value);
            _LineWritten = true;
        }

        public override void Write(uint value)
        {
            WriteIndentIfNeeded();
            _BaseWriter.Write(value);
            _LineWritten = true;
        }

        public override void Write(ulong value)
        {
            WriteIndentIfNeeded();
            _BaseWriter.Write(value);
            _LineWritten = true;
        }

        #endregion Write

        #region WriteAsync

        public override async Task WriteAsync(char value)
        {
            await WriteIndentIfNeededAsync().ConfigureAwait(false);
            await _BaseWriter.WriteAsync(value).ConfigureAwait(false);
            _LineWritten = true;
        }

        public override async Task WriteAsync(char[] buffer, int index, int count)
        {
            await WriteIndentIfNeededAsync().ConfigureAwait(false);
            await _BaseWriter.WriteAsync(buffer, index, count).ConfigureAwait(false);
            _LineWritten = true;
        }

        public override async Task WriteAsync(string value)
        {
            await WriteIndentIfNeededAsync().ConfigureAwait(false);
            await _BaseWriter.WriteAsync(value).ConfigureAwait(false);
            _LineWritten = true;
        }

        #endregion WriteAsync

        #region WriteLine

        public override void WriteLine()
        {
            WriteIndentIfNeeded();
            _BaseWriter.WriteLine();
            _LineWritten = false;
        }

        public override void WriteLine(bool value)
        {
            WriteIndentIfNeeded();
            _BaseWriter.WriteLine(value);
            _LineWritten = false;
        }

        public override void WriteLine(char value)
        {
            WriteIndentIfNeeded();
            _BaseWriter.WriteLine(value);
            _LineWritten = false;
        }

        public override void WriteLine(char[] buffer)
        {
            WriteIndentIfNeeded();
            _BaseWriter.WriteLine(buffer);
            _LineWritten = false;
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            WriteIndentIfNeeded();
            _BaseWriter.WriteLine(buffer, index, count);
            _LineWritten = false;
        }

        public override void WriteLine(decimal value)
        {
            WriteIndentIfNeeded();
            _BaseWriter.WriteLine(value);
            _LineWritten = false;
        }

        public override void WriteLine(double value)
        {
            WriteIndentIfNeeded();
            _BaseWriter.WriteLine(value);
            _LineWritten = false;
        }

        public override void WriteLine(float value)
        {
            WriteIndentIfNeeded();
            _BaseWriter.WriteLine(value);
            _LineWritten = false;
        }

        public override void WriteLine(int value)
        {
            WriteIndentIfNeeded();
            _BaseWriter.WriteLine(value);
            _LineWritten = false;
        }

        public override void WriteLine(long value)
        {
            WriteIndentIfNeeded();
            _BaseWriter.WriteLine(value);
            _LineWritten = false;
        }

        public override void WriteLine(object value)
        {
            WriteIndentIfNeeded();
            _BaseWriter.WriteLine(value);
            _LineWritten = false;
        }

        public override void WriteLine(string format, params object[] arg)
        {
            WriteIndentIfNeeded();
            _BaseWriter.WriteLine(format, arg);
        }

        public override void WriteLine(string format, object arg0)
        {
            WriteIndentIfNeeded();
            _BaseWriter.WriteLine(format, arg0);
            _LineWritten = false;
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            WriteIndentIfNeeded();
            _BaseWriter.WriteLine(format, arg0, arg1);
            _LineWritten = false;
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            WriteIndentIfNeeded();
            _BaseWriter.WriteLine(format, arg0, arg1, arg2);
            _LineWritten = false;
        }

        public override void WriteLine(string value)
        {
            WriteIndentIfNeeded();
            _BaseWriter.WriteLine(value);
            _LineWritten = false;
        }

        public override void WriteLine(uint value)
        {
            WriteIndentIfNeeded();
            _BaseWriter.WriteLine(value);
            _LineWritten = false;
        }

        public override void WriteLine(ulong value)
        {
            WriteIndentIfNeeded();
            _BaseWriter.WriteLine(value);
            _LineWritten = false;
        }

        #endregion WriteLine

        #region WriteLineAsync

        public override async Task WriteLineAsync()
        {
            await WriteIndentIfNeededAsync().ConfigureAwait(false);
            await _BaseWriter.WriteLineAsync();
            _LineWritten = true;
        }

        public override async Task WriteLineAsync(char value)
        {
            await WriteIndentIfNeededAsync().ConfigureAwait(false);
            await _BaseWriter.WriteLineAsync(value);
            _LineWritten = true;
        }

        public override async Task WriteLineAsync(char[] buffer, int index, int count)
        {
            await WriteIndentIfNeededAsync().ConfigureAwait(false);
            await _BaseWriter.WriteLineAsync(buffer, index, count);
            _LineWritten = true;
        }

        public override async Task WriteLineAsync(string value)
        {
            await WriteIndentIfNeededAsync().ConfigureAwait(false);
            await _BaseWriter.WriteLineAsync(value);
            _LineWritten = true;
        }

        #endregion WriteLineAsync

        private void WriteIndentIfNeeded()
        {
            if (!_LineWritten)
            {
                for (var i = 0; i < Indent; i++)
                {
                    for (var j = 0; j < 4; j++)
                    {
                        _BaseWriter.Write(' ');
                    }
                }
                _LineWritten = true;
            }
        }

        private async Task WriteIndentIfNeededAsync()
        {
            if (!_LineWritten)
            {
                await _BaseWriter.WriteAsync(new string(' ', Indent * 4)).ConfigureAwait(false);
                _LineWritten = true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _BaseWriter?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}

#endif