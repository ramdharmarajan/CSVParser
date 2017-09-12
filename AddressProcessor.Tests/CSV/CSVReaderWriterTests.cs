using NUnit.Framework;

namespace Csv.Tests
{
    using System.IO;
    using AddressProcessing.CSV;
    using FluentAssertions;
    using Moq;

    [TestFixture]
    public class CSVReaderWriterTests
    {
        private Mock<IFileOperations> _fileOperationsMock;

        [SetUp]
        public void SetUp()
        {
            _fileOperationsMock = new Mock<IFileOperations>();
        }

        [TestCase(CSVReaderWriter.Mode.Read)]
        [TestCase(CSVReaderWriter.Mode.Write)]
        public void DoesNotThrowException_WhenFileIsAccessedInPermissibleModes(CSVReaderWriter.Mode fileMode)
        {
            _fileOperationsMock.Setup(f => f.OpenText(It.IsAny<string>())).Returns(StreamReader.Null);
            Assert.DoesNotThrow(() => new CSVReaderWriter(_fileOperationsMock.Object).Open("test", fileMode));
        }

        [Test]
        public void ShouldGetTabSeparatedString_WhenColumnsAreSupplied()
        {           
            var expectedOutput = "this\tis\ta\ttest\tline";
            var actualOutput = new CSVReaderWriter(_fileOperationsMock.Object).BuildTabSeparatedString("this", "is", "a", "test", "line");

            actualOutput.Should().Be(expectedOutput);
        }

        [Test]
        public void ShouldCallWriteLine_IfTabSeparatedStringIsNotNull()
        {
            using (var csvReaderWriter = new CSVReaderWriter(_fileOperationsMock.Object))
            {
                csvReaderWriter.Write("this", "is", "a", "test", "line");

                _fileOperationsMock.Verify(o => o.WriteLine(It.IsAny<StreamWriter>(), It.IsAny<string>()), Times.Once);
            }   
        }

        [Test]
        public void ShouldReturnColumnValues_IfTabSeparatedStringIsCorrectFormat()
        {
            string column1, column2;
            var expectedLine = @"Shelby Macias	3027 Lorem St.|Kokomo|Hertfordshire|L9T 3D5|England	1 66 890 3865-9584	et@eratvolutpat.ca";

            using (var csvReaderWriter = new CSVReaderWriter(_fileOperationsMock.Object))
            {
                _fileOperationsMock.Setup(f => f.ReadLine(It.IsAny<StreamReader>())).Returns(expectedLine);
                csvReaderWriter.Open("test", CSVReaderWriter.Mode.Read);
                csvReaderWriter.Read(out column1, out column2);

                column1.Should().NotBeNullOrEmpty();
                column2.Should().NotBeNullOrEmpty();

                column1.Should().Be(@"Shelby Macias");
                column2.Should().Be(@"3027 Lorem St.|Kokomo|Hertfordshire|L9T 3D5|England");
            }    
        }

        [Test]
        public void ShouldNotReturnColumnValues_IfTabSeparatedStringIsWrongFormat()
        {
            string column1, column2;
            var expectedLine = @"xyz";

            using (var csvReaderWriter = new CSVReaderWriter(_fileOperationsMock.Object))
            {
                _fileOperationsMock.Setup(f => f.ReadLine(It.IsAny<StreamReader>())).Returns(expectedLine);
                csvReaderWriter.Open("test", CSVReaderWriter.Mode.Read);
                csvReaderWriter.Read(out column1, out column2);

                column1.Should().NotBeNullOrEmpty();
                column2.Should().BeNullOrEmpty();                
            }
        }
    }
}
