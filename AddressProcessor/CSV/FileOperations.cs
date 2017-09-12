namespace AddressProcessing.CSV
{
    using System.IO;

    public class FileOperations : IFileOperations
    {
        public StreamReader OpenText(string filePath)
        {
            return File.OpenText(filePath);
        }

        public StreamWriter CreateText(string filePath)
        {
            var fileInfo = new FileInfo(filePath);

            return fileInfo.CreateText();
        }

        public void WriteLine(StreamWriter streamWriter, string line)
        {
            streamWriter?.WriteLine(line);
        }

        public string ReadLine(StreamReader streamReader)
        {
            return streamReader?.ReadLine();
        }        
    }
}