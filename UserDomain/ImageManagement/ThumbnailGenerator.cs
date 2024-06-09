using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cave;
using Cave.Media;



namespace UserDomain.user_image
{
    /// <summary>
    /// default uses Gravatar, extend to use Liberator,DiceBEar or Robotash or other as 
    /// yet undefined. If you feel the need you can also convert the image to a bitmap
    /// and store if offline is important
    /// </summary>
    public class ThumbnailGenerator : IThumbnail
    {
        /// <summary>
        /// Generates URL to (in this base class Gravatar)
        /// to generate the URL of a unique avatar.
        /// We don't use enum for typeswitch in case overloading interface uses a different website and
        /// thus enum locked in and needs maintaining
        /// </summary>
        /// <param name="userTag">we use usertag as unique and not as long as the uuid user id</param>
        /// <param name="typeSwitch">defaults to MonsterId else 1 for IdentIcon, 2 for Retro and 3 for wavatar</param>
        /// <returns></returns>
        public string CreateNewThumbnail(string userTag)
        {
            GravatarType baseType = GravatarType.Wavatar;

            Avatar avatar = Gravatar.Create(userTag, 100, baseType);
            string? thumbnailUrl = avatar.Url.ToString();

            if (String.IsNullOrEmpty(thumbnailUrl))
            {
                return "https://www.gravatar.com/avatar/21232f297a57a5a743894a0e4a801fc3?d=wavatar&s=100";
            }


            return thumbnailUrl;
        }
    }
}
