using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Logic.Domain
{
	/*
    public class RaceResult : IRaceResult
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(RaceResult));

        readonly IRace _race;
        readonly IList<ICrewResult> _crewResults;

        public RaceResult(IRace race, IList<ICrewResult> crewResults)
        {
            _race = race;
            _crewResults = crewResults;
        }

        #region IRaceResult implementation
        public void Dump()
        {
            StringBuilder sb = new StringBuilder();

			using(var fs = new FileStream(_race.Name + ".pdf", FileMode.Create)){
				using(Document document = new Document(PageSize.A4_LANDSCAPE)){
		            BaseFont bf = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
		            Font font = new Font(Font.FontFamily.HELVETICA, 7f, Font.NORMAL);

		            // step 2:
		            // we create a writer that listens to the document and directs a PDF-stream to a file            
					PdfWriter.GetInstance(document, fs);
					// PdfCopy writer = new PdfCopy(document, new FileOutputStream(OUTPUTFILE));

		            // step 3: we open the document
		            document.Open();

		            // entitle the document 
		            document.Add(new Paragraph(_race.ToString()));
		            sb.AppendLine(_race.ToString());
		            document.AddSubject(_race.ToString());

		            // grab the header and seed the table 
		            var header = CrewResult.HeaderRow;
		            sb.AppendLine(header.Aggregate((h, t) => h + ", " + t));

					float[] widths = new float[] { 2f, 1f, 6f, 6f, 4f, 2f, 1f, 1f,5f };
		            PdfPTable table = new PdfPTable(header.Count) 
		                {
		                    TotalWidth = 500f,
		                    LockedWidth = true,                    
		                    HorizontalAlignment = 0,
		                    SpacingBefore = 20f,
		                    SpacingAfter = 30f,
		            };
		            table.SetWidths(widths);

		            foreach(var h in header)
		            {
		                table.AddCell(new PdfPCell(new Phrase(h)) { Border = 1, HorizontalAlignment = 2, Rotation = 90 } );
		            }

		            foreach(var result in _crewResults.OrderBy(r => ((int)r.FinishType *10000) + (r.Elapsed.HasValue ? r.Elapsed.Value.TotalSeconds : 0)))
		            {
		                var d = result.Dump;
		                sb.AppendLine(d.Aggregate((h, t) => h + ", " + t));
		                if(result.FinishType == FinishType.DNS)
		                    continue;
		                foreach(var l in d)
		                    table.AddCell(new PdfPCell(new Phrase(l.TrimEnd(), font)) { Border = 0 } ); 
		            }
		            Logger.Info(sb.ToString());

		            document.Add(table);
		            document.AddTitle("Designed by vrc.org.uk");
		            document.AddAuthor("Chris Harrison, SH Timing and Results");
		            document.AddKeywords("Scullers Head, 2013, Results");

		            document.Close();
				}
			}
        }

        #endregion



    }
    */
}
