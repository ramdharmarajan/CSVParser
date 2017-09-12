using System;
using AddressProcessing.Address.v1;
using AddressProcessing.CSV;

namespace AddressProcessing.Address
{
    public class AddressFileProcessor
    {
        private readonly IMailShot _mailShot;

        public AddressFileProcessor(IMailShot mailShot)
        {
            if (mailShot == null) throw new ArgumentNullException("mailShot");
            _mailShot = mailShot;
        }

        public void Process(string inputFile)
        {
            // Dispose pattern to dispose the stream
            using (var reader = new CSVReaderWriter())
            {                
                reader.Open(inputFile, CSVReaderWriter.Mode.Read);

                string column1, column2;

                while (reader.Read(out column1, out column2))
                {
                    if (string.IsNullOrWhiteSpace(column1) || string.IsNullOrWhiteSpace(column2))
                    {
                        continue;
                    }

                    _mailShot.SendMailShot(column1, column2);
                }
            }                
        }
    }
}
