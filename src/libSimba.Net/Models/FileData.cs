using System.IO;

namespace libSimba.Net.Models
{
    /// <summary>
    ///     File Data for encapsulating streams with file name and mime type for upload
    /// </summary>
    public class FileData
    {
        public string FileName;
        public string MimeType = "application/octet-stream";
        public Stream Stream;

        public FileData(string name, Stream stream)
        {
            FileName = name;
            Stream = stream;
        }

        public FileData(string name, string mimetype, Stream stream)
        {
            FileName = name;
            Stream = stream;
            MimeType = mimetype;
        }
    }
}