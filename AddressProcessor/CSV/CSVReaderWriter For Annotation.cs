using System;
using System.IO;

namespace AddressProcessing.CSV
{
    /*
        1) List three to five key concerns with this implementation that you would discuss with the junior developer. 

        Please leave the rest of this file as it is so we can discuss your concerns during the next stage of the interview process.
        
        1) The "Close" method in this class is called by the "Process" method in class "AddressFileProcessor". In case of any exception thrown in the "Process" Method,
           "Close" will never get called and hence the streams will never get closed. The "Process" method should have a try-catch-finally block or disposable pattern on 
           CSVReaderWriter class needs to be implemented.
        
        2) No Logging has been done in the application, so it is very hard to troubeshoot in production.

        3) The following methods in this class lack any exception handling; "Open", "Write" and "Read". This makes the code brittle and susceptible to any minor issues within the file.
           For a typical use case like this, you should be handling exceptions, logging them, but continue further with the processing of the file.

        4) The "Write" method has an issue. It is concatenating strings inside a loop. High chance of OutOfMemory Exception being thrown especially if it's a large file.
           Strings are immutable which means in each iteration you are creating a new instance of a string. Use StringBuilder instead since it's not immutable.

        5) The code isn't unit testable. There is no way to mock the File and FileInfo objects. File based operations can be setup as public methods and then mocked for testability.

        6) The file mode is either "read" or "write". Anything else, an exception is thrown. So, the "Mode" enum doesn't need to be decorated with a Flags attribute.

        7) What is the point of the "Read" method taking in 2 parameters when their input values are being anyhow overwritten?
    */

    public class CSVReaderWriterForAnnotation
    {
        private StreamReader _readerStream = null;
        private StreamWriter _writerStream = null;

        [Flags]
        public enum Mode { Read = 1, Write = 2 };

        public void Open(string fileName, Mode mode)
        {
            if (mode == Mode.Read)
            {
                _readerStream = File.OpenText(fileName);
            }
            else if (mode == Mode.Write)
            {
                FileInfo fileInfo = new FileInfo(fileName);
                _writerStream = fileInfo.CreateText();
            }
            else
            {
                throw new Exception("Unknown file mode for " + fileName);
            }
        }

        public void Write(params string[] columns)
        {
            string outPut = "";

            for (int i = 0; i < columns.Length; i++)
            {
                outPut += columns[i];
                if ((columns.Length - 1) != i)
                {
                    outPut += "\t";
                }
            }

            WriteLine(outPut);
        }

        public bool Read(string column1, string column2)
        {
            const int FIRST_COLUMN = 0;
            const int SECOND_COLUMN = 1;

            string line;
            string[] columns;

            char[] separator = { '\t' };

            line = ReadLine();
            columns = line.Split(separator);

            if (columns.Length == 0)
            {
                column1 = null;
                column2 = null;

                return false;
            }
            else
            {
                column1 = columns[FIRST_COLUMN];
                column2 = columns[SECOND_COLUMN];

                return true;
            }
        }

        public bool Read(out string column1, out string column2)
        {
            const int FIRST_COLUMN = 0;
            const int SECOND_COLUMN = 1;

            string line;
            string[] columns;

            char[] separator = { '\t' };

            line = ReadLine();

            if (line == null)
            {
                column1 = null;
                column2 = null;

                return false;
            }

            columns = line.Split(separator);

            if (columns.Length == 0)
            {
                column1 = null;
                column2 = null;

                return false;
            } 
            else
            {
                column1 = columns[FIRST_COLUMN];
                column2 = columns[SECOND_COLUMN];

                return true;
            }
        }

        private void WriteLine(string line)
        {
            _writerStream.WriteLine(line);
        }

        private string ReadLine()
        {
            return _readerStream.ReadLine();
        }

        public void Close()
        {
            if (_writerStream != null)
            {
                _writerStream.Close();
            }

            if (_readerStream != null)
            {
                _readerStream.Close();
            }
        }
    }
}
