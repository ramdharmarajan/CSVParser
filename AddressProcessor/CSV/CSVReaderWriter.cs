using System;
using System.IO;

namespace AddressProcessing.CSV
{
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    /*
        2) Refactor this class into clean, elegant, rock-solid & well performing code, without over-engineering.
           Assume this code is in production and backwards compatibility must be maintained.
    */

    public class CSVReaderWriter : IDisposable
    {
        private static string _filePath;

        public enum Mode
        {
            Read = 1,
            Write = 2
        };

        private readonly IFileOperations _fileOperations;
        private StreamReader _readerStream;
        private StreamWriter _writerStream;

        // For backward compatibility with production code
        public CSVReaderWriter() : this(new FileOperations())
        {
            
        }

        public CSVReaderWriter(IFileOperations fileOperations)
        {
            _fileOperations = fileOperations;
        }

        public void Open(string filePath, Mode mode)
        {
            _filePath = filePath;

            switch (mode)
            {
                case Mode.Read:
                    _readerStream = _fileOperations.OpenText(filePath);
                    break;
                case Mode.Write:
                    _writerStream = _fileOperations.CreateText(filePath);
                    break;
                default:
                    throw new Exception("Unknown file mode for " + filePath);
            }
        }

        public void Write(params string[] columns)
        {
            var output = BuildTabSeparatedString(columns);

            if (string.IsNullOrWhiteSpace(output))
            {
                return;
            }

            _fileOperations.WriteLine(_writerStream, output);
        }

        public virtual string BuildTabSeparatedString(params string[] columns)
        {
            var fileOutputBuilder = new StringBuilder();
            var columnCount = columns.Length;
            var count = 0;

            try
            {
                foreach (var column in columns)
                {
                    fileOutputBuilder.Append(column);

                    if (++count == columnCount)
                    {
                        break;
                    }

                    fileOutputBuilder.Append("\t");
                }

                var fileOutput = fileOutputBuilder.ToString();

                return string.IsNullOrWhiteSpace(fileOutput) ? null : fileOutput;
            }
            catch (Exception e)
            {
                Trace.WriteLine($"Failed to Build Tab Separated Data for column. {e.Message}");
            }

            return null;
        }        

        public bool Read(out string column1, out string column2)
        {            
            char[] separator = { '\t' };

            var line = _fileOperations.ReadLine(_readerStream);

            if (line == null)
            {
                column1 = null;
                column2 = null;

                return false;
            }

            var columns = line.Split(separator).ToList();            

            if (columns.Count == 0)
            {
                column1 = null;
                column2 = null;

                Trace.WriteLine($"Thie Line is Corrupted or Not in Correct Format. {line}");

                // You still want to continue when some lines in the file is corrupted. This makes the code more robust.
                return true;
            }

            var csvModel = new CSVModel(columns).Build();

            column1 = csvModel.Name;
            column2 = csvModel.Address;            

            return true;
        }        

        public void Dispose()
        {
            _readerStream?.Close();
            _writerStream?.Close();
        }
    }
}
