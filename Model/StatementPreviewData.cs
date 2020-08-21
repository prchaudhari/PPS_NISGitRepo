
namespace nIS
{
    #region References
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    public class StatementPreviewData
    {
        public string FileContent { get; set; }
        public IList<FileData> SampleFiles { get; set; }
    }

    public class FileData
    {
        public string FileName { get; set; }
        public string FileUrl { get; set; }
    }
}
