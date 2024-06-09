using ServerCommonModule.Attributes;
using ServerCommonModule.Model.Interfaces;
using ServerCommonModule.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UserDomain.Properties;
using UserDomain.model;
using UserDomain.ControlModule.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserDomain.Model
{
    [Table("usr_archive")]
    [HasModifiedDate(true)]
    public class ArchiveUser : ModelEntry
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


        public ArchiveUser()
        {
            Code = "DEF0000002";
            Tag = "DEFAULT_TAG2";
            ThumbnailUrl = "DEFAULT_THUMB2";
        }

        public ArchiveUser(PrimeUser archiveThisUser)
        {
            Id = archiveThisUser.Id;
            Code = archiveThisUser.Code;
            Tag = archiveThisUser.Tag;
            ThumbnailUrl = archiveThisUser.ThumbnailUrl;
            Status = archiveThisUser.Status;
            Name = archiveThisUser.Name;
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
            ArchiveUser clone = new ArchiveUser
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
