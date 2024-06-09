using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserDomain.user_image
{
    internal class ThumbnailFactory
    {
        internal IThumbnail GetThumbnailGenerator()
        {
            return new ThumbnailGenerator();
        }
    }
}
