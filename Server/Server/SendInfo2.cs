using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    [Serializable]
    class SendInfo2
    {
        private string _fileName;
        private byte[] _hash;
        private long _fileSize;

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public byte[] Hash
        {
            get { return _hash; }
            set { _hash = value; }
        }

        public long FileSize
        {
            get { return _fileSize; }
            set { _fileSize = value; }
        }
    }
}
