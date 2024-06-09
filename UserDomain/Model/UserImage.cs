using ServerCommonModule.Attributes;
using ServerCommonModule.Model;
using ServerCommonModule.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserDomain.model;
using UserDomain.Properties;
using TableAttribute = ServerCommonModule.Attributes.TableAttribute;

namespace UserDomain.Model
{

    [Table("usr_image")]
    [HasModifiedDate(false)]
    public class UserImage : ModelEntry
    {
        [FieldName("usr_img_type"), FieldType(SqlDbType.Int)]
        public IMAGE_TYPE ImageType { get; set; }
        [FieldName("usr_img_url"), FieldType(SqlDbType.VarChar)]
        public string ImageUrl { get; set; }


        public UserImage()
        {
            ImageType = 0;
            ImageUrl = "DEFAULT_IMAGE";
        }

        public override string ToString()
        {
            string res = @$"
Id: {Id}
ImageType: {ImageType}
ImageUrl: {ImageUrl}
";
            return res;
        }
        public override IModelEntry Clone()
        {
            UserImage clone = new UserImage
            {
                Id = this.Id,
                Name = this.Name,
                ImageType = this.ImageType,
                ImageUrl = this.ImageUrl
            };

            return clone;
        }
    }
}
