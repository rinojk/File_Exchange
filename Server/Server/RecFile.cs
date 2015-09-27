using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class RecFile
    {

        private byte[] _hash;
        private long _size;
        private string _path;

        public byte[] Hash
        {
            get { return _hash; }
        }

        public long Size
        {
            get { return _size; }
            set { _size = value; }
        }


        public int CompareTo(object obj)
        {
            
            throw new NotImplementedException();
        }

        private byte[] GetChecksum(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    return md5.ComputeHash(stream);
                }
            }
        }
    }
}
