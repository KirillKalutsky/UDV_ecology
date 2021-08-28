using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSources.Models
{
    public class DataSource
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DataSourceField Field { get; set; }

        public List<Publication> Publications { get; set; }

        public Sources SourceType { get; set; }
    }
}
