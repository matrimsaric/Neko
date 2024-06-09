using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserDomain.user_image
{
    public interface IThumbnail
    {
        public string? CreateNewThumbnail(string userTag);
    }
}
