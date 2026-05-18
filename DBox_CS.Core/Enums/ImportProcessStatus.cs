using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBox_CS.Core.Enums
{
    public enum ImportProcessStatus
    {
        NotProcessed = 0,        // File has not been processed
        Downloading = 1,         // File is currently downloading
        Processed = 2,           // File has been processed
        HasErrors = 3,           // Errors occurred during processing
        SaveFailed = 4,          // Failed to save the file
        Saved = 5,               // File has been successfully saved
        Skipped = 6,             // File was skipped
        ReadyToProcess =7
    }
}
