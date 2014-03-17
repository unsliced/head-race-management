using System;
using System.Collections.Generic;
using Head.Common.Interfaces.Utils;
using Head.Common.Csv;
using Common.Logging;
using Head.Common.Domain;
using Head.Common.BritishRowing;
using Head.Common.Internal.Overrides;
using System.Linq;
using Head.Common.Internal.Categories;
using Head.Common.Interfaces.Enums;
using System.Text;
using Head.Common.Utils;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;

namespace Head.Common.Generate
{

	public class StartPositionGenerator
	{
		public static void Generate(IEnumerable<ICrew> ecrews)
		{
			ILog logger = LogManager.GetCurrentClassLogger ();

			IList<ICrew> crews = ecrews.ToList();
			if(crews.Any(cr => cr.StartNumber > 0))
			{
				logger.Info ("crews have start numbers. ");
				if(crews.Any(cr => cr.StartNumber <= 0))
					logger.Warn ("but some don't, that's not right - delete the start positions or fix thge JSON.");
				Dump (crews);
				return;
			}
			IList<string> startpositions = new List<string> ();

			foreach(var crew in 
				crews
					.OrderBy(cr => cr.Categories.First(cat => cat is EventCategory).Order)
					.ThenBy(cr => cr.PreviousYear.HasValue && cr.PreviousYear.Value <= 3 ? cr.PreviousYear.Value : 5)
					.ThenBy(cr => cr.CrewId.Reverse()))
			{
				logger.InfoFormat("{0}, {1}", crew.Name, crew.Categories.First(cat => cat is EventCategory).Name);
				startpositions.Add(String.Format("{{\"CrewId\":{0},\"StartNumber\":{1}}}", crew.CrewId, startpositions.Count+1));
			}
			logger.Info(startpositions.Delimited());
		}

		public static void Dump(IEnumerable<ICrew> crews)
		{
			ILog logger = LogManager.GetCurrentClassLogger ();

			string raceDetails = "Vets Head - 30 March 2014";

			using(var fs = new FileStream("Vets Head 2014 Draw.pdf", FileMode.Create)){
				using(Document document = new Document(PageSize.A4.Rotate())){

					// 					BaseFont bf = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
					Font font = new Font(Font.FontFamily.HELVETICA, 7f, Font.NORMAL);
					Font italic = new Font(Font.FontFamily.HELVETICA, 7f, Font.ITALIC);

					// step 2:
					// we create a writer that listens to the document and directs a PDF-stream to a file            
					PdfWriter.GetInstance(document, fs);

					// step 3: we open the document
					document.Open();

					// entitle the document 
					document.Add(new Paragraph(raceDetails));
					document.AddSubject(raceDetails);

					// grab the header and seed the table 

					float[] widths = new float[] { 1f, 8f, 3f, 3f, 1f, 4f };
					PdfPTable table = new PdfPTable(widths.Count()) 
					{
						TotalWidth = 800f,
						LockedWidth = true,                    
						HorizontalAlignment = 0,
						SpacingBefore = 20f,
						SpacingAfter = 30f,
					};
					table.SetWidths(widths);

					foreach(var h in new List<string> { "Start", "Crew", "Category", "Boating", "Paid","Other prizes" })
					{
						table.AddCell(new PdfPCell(new Phrase(h)) { Border = 1, HorizontalAlignment = 2, Rotation = 90 } );
					}
					foreach (var crew in crews.OrderBy(cr => cr.StartNumber)) 
					{
						ICategory primary;
						string extras = String.Empty;
						if(crew.Categories.Any (c => c is TimeOnlyCategory)) 
						{ 
							primary = crew.Categories.First (c => c is TimeOnlyCategory);
						}
						else 
						{
							primary = crew.Categories.First (c => c is EventCategory);
							extras = crew.Categories.Where (c => !(c is EventCategory) && !(c is OverallCategory) && c.Offered).Select (c => c.Name).Delimited ();
						}
						var objects = new List<Tuple<string, Font>> { 
							new Tuple<string, Font>(crew.StartNumber.ToString(), font),
							new Tuple<string, Font>(crew.Name, font),
							new Tuple<string, Font>(primary.Name, primary.Offered ? font : italic),
							new Tuple<string, Font>(crew.BoatingLocation.Name, font),
							new Tuple<string, Font>((crew.IsPaid ? String.Empty : "UNPAID") + " " + (crew.IsScratched ? "SCRATCHED" : String.Empty), font), 
							new Tuple<string, Font>(extras, font)
						};

						// TODO - actual category, for the purposes of adjustment 
						// chris - if multiple crews from the same club in the same category put the stroke's name - currently being overridden after manual observation 
						foreach(var l in objects)
							table.AddCell(new PdfPCell(new Phrase(l.Item1.TrimEnd(), l.Item2)) { Border = 0 } ); 
					}

					document.Add(table);
					document.AddTitle("Designed by vrc.org.uk");
					document.AddAuthor("Chris Harrison, VH Timing and Results");
					document.AddKeywords("Vets Head, 2014, Draw");

					document.Close();
				}
			}
		}

	}
}
