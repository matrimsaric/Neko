using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCommonModule.Attributes;
using ServerCommonModule.Database.Interfaces;
using System.Data;
using ServerCommonModule.Model;
using ServerCommonModule.Model.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;
using TableAttribute = ServerCommonModule.Attributes.TableAttribute;


namespace RankingDomain.Model
{
    [Table("usr_rating_history")]
    [HasModifiedDate(true)]
    public class ArchiveRank : ModelEntry
    {
        [FieldName("rating"), FieldType(SqlDbType.Decimal)]
        public decimal Rating { get; set; }
        [FieldName("deviation"), FieldType(SqlDbType.Decimal)]
        public decimal Deviation { get; set; }
        [FieldName("volatility"), FieldType(SqlDbType.Decimal)]
        public decimal Volatility { get; set; }

        [FieldName("link_id"), FieldType(SqlDbType.UniqueIdentifier)]
        public Guid UniqueIdentifier { get; set; }


        public ArchiveRank()
        {

        }

        public override string ToString()
        {
            return $@"
Id: {Id.ToString()}
Rating {Rating.ToString()} : {Deviation.ToString()} ({Volatility.ToString()})
";
        }

        public override IModelEntry Clone()
        {
            ArchiveRank clone = new ArchiveRank
            {
                Id = this.Id,
                Name = this.Name,
                Rating = this.Rating,
                Deviation = this.Deviation,
                Volatility = this.Volatility
            };
            return clone;
        }
    }
}
