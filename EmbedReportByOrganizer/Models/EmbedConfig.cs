
using Microsoft.PowerBI.Api.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EmbedReportByOrganizer.Models
{
    public class EmbedConfig
    {
        public EmbedToken EmbedToken { get; set; }
        public string EmbedUrl { get; set; }
        public string ReportId { get; set; }
    }
}