namespace AddressProcessing.CSV
{
    using System.IO;

    public interface IFileOperations
    {
        StreamReader OpenText(string filePath);

        StreamWriter CreateText(string filePath);

        void WriteLine(StreamWriter streamWriter,string line);

        string ReadLine(StreamReader streamReader);
    }
}