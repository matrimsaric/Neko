using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserDomain.Properties;
using ServerCommonModule.Attributes;
using ServerCommonModule.Database.Interfaces;
using System.Data;
using ServerCommonModule.Model;
using ServerCommonModule.Model.Interfaces;
using UserDomain.Model;
using UserDomain.ControlModule.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;
using TableAttribute = ServerCommonModule.Attributes.TableAttribute;

namespace UserDomain.model
{
    /// <summary>
    /// Prime user access class. Note: Image class is disaccociated with this
    /// due to overhead and specific  limited need requirements of the more complex
    /// image structure. If needed extend this class and include in collection but be aware
    /// of added overhead.
    /// </summary>
    [Table("usr_master")]
    [HasModifiedDate(true)]
    public class PrimeUser : ModelEntry
    {
        // note Id field (key,uuid) and Name field are inherited from ModelEntry and exist for all patterned tables
        [FieldName("usr_code"), FieldType(SqlDbType.VarChar), FieldUnique(true)]
        public string Code { get; set; }
        [FieldName("usr_tag"), FieldType(SqlDbType.VarChar), FieldUnique(true)]
        public string Tag { get; set; }
        [FieldName("usr_status"), FieldType(SqlDbType.Int)]
        public USER_STATUS Status { get; set; }
        [FieldName("usr_thumbnail"), FieldType(SqlDbType.VarChar)]
        public string ThumbnailUrl { get; set; }




        public PrimeUser()
        {
            Code = "DEF0000001";
            Tag = "DEFAULT_TAG";
            ThumbnailUrl = "DEFAULT_THUMB";


        }

        public override string ToString()
        {
            string res = @$"
Id: {Id}
Name: {Name}
Code: {Code}
Tag: {Tag}
Status: {Status}
ThumbnailUrl: {ThumbnailUrl},
";
            return res;
        }

        public override IModelEntry Clone()
        {
            PrimeUser clone = new PrimeUser
            {
                Id = this.Id,
                Name = this.Name,
                Code = this.Code,
                Tag = this.Tag,
                Status = this.Status,
                ThumbnailUrl = this.ThumbnailUrl

            };

            return clone;
        }





    }
}
