using System;
using System.Collections.Generic;
using Common.Logging;
using Head.Common.Domain;
using System.Linq;
using Head.Common.Internal.Categories;
using Head.Common.Interfaces.Enums;
using System.Text;
using Head.Common.Utils;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;
using Head.Common.Internal.JsonObjects;
using System.Configuration;
using System.Diagnostics;

namespace Head.Common.Generate
{
    public class ResultsPrinter
	{
		public static void Dump(IEnumerable<ICrew> crews) 
		{
			DateTime racedate = DateTime.MinValue;
			if(!DateTime.TryParse(ConfigurationManager.AppSettings["racedate"].ToString(), out racedate))
				racedate = DateTime.MinValue;
			ILog logger = LogManager.GetCurrentClassLogger ();

			string raceDetails = string.Format("{0} - {1} - Final Results", ConfigurationManager.AppSettings["racenamelong"], racedate.ToLongDateString()); // todo - provisional / final results 
			string updated = "Updated: \t" + DateTime.Now.ToShortTimeString () + " " + DateTime.Now.ToShortDateString ();
			StringBuilder sb = new StringBuilder ();
			sb.AppendLine (updated);

			int cc = crews.Max(cr => cr.StartNumber)+1; // Count ()+2;
			ICategory[] primary = new ICategory[cc];
			int[] overallpos = new int[cc];
			int[] categorypos = new int[cc];
			int[] genderpos = new int[cc];
			int[] foreignpos = new int[cc];
			string[] extras = new string[cc];
		
			foreach (var crew in crews) { 
				StringBuilder sbe = new StringBuilder ();

				if (!string.IsNullOrEmpty (crew.QueryReason))
					sbe.Append (crew.QueryReason);

				if (!string.IsNullOrEmpty (crew.Citation))
					sbe.Append (crew.Citation);

				if (crew is UnidentifiedCrew)
					primary[crew.StartNumber] = new TimeOnlyCategory ();
				else if (crew.Categories.Any (c => c is TimeOnlyCategory)) { 
					primary[crew.StartNumber] = crew.Categories.First (c => c is TimeOnlyCategory);
				} else {
					try
					{
						primary[crew.StartNumber] = crew.Categories.First(c => c is EventCategory);
						if (crew.FinishType == FinishType.Finished)
						{
							overallpos[crew.StartNumber] = CategoryNotes(crew, c => c is OverallCategory, false, sbe);
							categorypos[crew.StartNumber] = primary[crew.StartNumber].Offered ? CategoryNotes(crew, c => c == primary[crew.StartNumber], false, sbe) : 0;
							genderpos[crew.StartNumber] = 0; // urgent CategoryNotes(crew, c => c.EventType == EventType.MastersHandicapped, false, sbe); 
							foreignpos[crew.StartNumber] = CategoryNotes(crew, c => c.EventType == EventType.Foreign, true, sbe);
						}
					}
					catch (Exception ex)
					{
						logger.DebugFormat("Problem with crew {0}", crew.StartNumber);
					}
				}
				try{
				extras[crew.StartNumber] = sbe.ToString();
				if (!string.IsNullOrEmpty (extras [crew.StartNumber])) {
					logger.InfoFormat ("{0}, {1}, {2}, {3}", 
						crew.Name, crew.CategoryName, crew.SubmittingEmail, extras [crew.StartNumber]);				
				}
					}
				catch (Exception ex)
				{
					logger.DebugFormat("Another problem with crew {0}", crew.StartNumber);
				}

			}
				
			var orders = new Dictionary<string, IOrderedEnumerable<ICrew>> {
				{string.Empty, crews.OrderBy(cr => cr.FinishType).ThenBy(cr => cr.Elapsed)},
				{" by category", crews.Where(cr => cr.EventCategory != null).OrderBy (cr => cr.EventCategory.Order).ThenBy(cr => cr.Adjusted)}, //  categorypos[cr.StartNumber])},
				{" by adjusted time", crews.OrderBy(cr => cr.FinishType).ThenBy(cr => cr.Adjusted)},
				{" by foreign", crews.Where(cr => foreignpos[cr.StartNumber] > 0).OrderBy(cr => cr.FinishType).ThenBy(cr => cr.Adjusted)},
				{" by pennants", crews.Where(cr => categorypos[cr.StartNumber] == 1 || foreignpos[cr.StartNumber] == 1|| genderpos[cr.StartNumber] == 1).OrderBy (cr => cr.EventCategory.Order)},

			};

			foreach(var kvp in orders)
			{
				using (var fs = new FileStream (string.Format ("{0} {1} Results{2}.pdf", ConfigurationManager.AppSettings ["racenamelong"], racedate.ToString ("yyyy"), kvp.Key), FileMode.Create)) {
					using (Document document = new Document (PageSize.A4.Rotate ())) {

						// 					BaseFont bf = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
						Font font = new Font (Font.FontFamily.HELVETICA, 7f, Font.NORMAL);
						Font italic = new Font (Font.FontFamily.HELVETICA, 7f, Font.ITALIC);
						Font bold = new Font (Font.FontFamily.HELVETICA, 7f, Font.BOLD);

						// step 2:
						// we create a writer that listens to the document and directs a PDF-stream to a file            
						PdfWriter.GetInstance (document, fs);

						// step 3: we open the document
						document.Open ();

						// entitle the document 
						document.Add (new Paragraph (raceDetails));
						document.AddSubject (raceDetails);

						// grab the header and seed the table 

						float[] widths = new float[] { 1f, 1f, 
							5f, 
							1f, 1f, 
							1f, 2f, 1f, 1f, 
							// 1f,
							3f
						};
						PdfPTable table = new PdfPTable (widths.Count ()) {
							TotalWidth = 800f,
							LockedWidth = true,                    
							HorizontalAlignment = 0,
							SpacingBefore = 20f,
							SpacingAfter = 30f,
						};
						table.SetWidths (widths);

						foreach (var h in new List<string> { "Overall",
                           "Start",
						"Crew", "Elapsed", 
						"Adjustment", "Adjusted", 
						"Category",  "Category Pos",  
						"Foreign Pos", 
						"Notes" }) {
							table.AddCell (new PdfPCell (new Phrase (h)) { Border = 1, HorizontalAlignment = 2, Rotation = 90 });
						}
						sb.AppendLine (new List<string> { "Overall", "StartNumber", 
							"Club", 
							"SequenceStart", "SequenceFinish", "Elapsed", 
							// "Adjustment", "Adjusted", 
							"Category", "CategoryPos", "FinishType"
						}.Delimited ('\t'));

                        // HACK: Hack!
                        int showAthlete = 1;
						foreach (var crew in kvp.Value) {

                            if (crew.FinishType == FinishType.DNS) continue; 
							string sequenceStart = string.Empty;
							string sequenceFinish = string.Empty;
							if (crew.FinishType == FinishType.Finished) {
								sequenceStart = crews.Count (c => !(c is UnidentifiedCrew) && c.StartTime <= crew.StartTime && c.StartTime > DateTime.MinValue).ToString ();
								sequenceFinish = crews.Count (c => !(c is UnidentifiedCrew) && c.FinishTime <= crew.FinishTime && c.FinishTime > DateTime.MinValue).ToString ();
							}
							string e = crew.Elapsed.ToString ();
							string elapsed = (crew.FinishType == FinishType.Finished || crew.FinishType == FinishType.TimeOnly) ? crew.Elapsed.ToString ().Substring (3).Substring (0, Math.Min (8, e.Length - 3)) : crew.FinishType.ToString ();
							string adjustment = crew.FinishType == FinishType.Finished ? crew.Adjusted.ToString ().Substring (3).Substring (0, Math.Min (8, e.Length - 3)) : string.Empty;
							string adjusted = (crew.FinishType == FinishType.Finished && crew.Adjustment > TimeSpan.Zero) ? crew.Adjustment.ToString ().Substring (3) : string.Empty;
							try
							{
								var objects = new List<Tuple<string, Font>> {
                                new Tuple<string, Font> (format0(overallpos[crew.StartNumber]), font),
								new Tuple<string, Font> (crew.StartNumber.ToString (), font),
                                new Tuple<string, Font> (crew.Name, font),
                                new Tuple<string, Font> (elapsed, font),
								new Tuple<string, Font> (adjustment, italic),
								new Tuple<string, Font> (adjusted, italic),
								new Tuple<string, Font> (crew.CategoryName, font), // primary.Name, primary.Offered ? font : italic),
                                new Tuple<string, Font> (format0(categorypos[crew.StartNumber]), font),

//							new Tuple<string, Font> (genderpos, font ),
								new Tuple<string, Font> (format0(foreignpos[crew.StartNumber]), font),
								new Tuple<string, Font> (extras[crew.StartNumber].ToString (), italic),
							};

								sb.AppendLine(new List<string> { overallpos[crew.StartNumber].ToString(), crew.StartNumber.ToString (), crew.AthleteName (1, true), crew.Name, sequenceStart, sequenceFinish, elapsed,
								adjustment, adjusted,
								primary[crew.StartNumber].Name, categorypos[crew.StartNumber].ToString(), crew.FinishType.ToString (),
                                extras[crew.StartNumber].ToString ()
                            }.Delimited('\t'));

								// TODO - actual category, for the purposes of adjustment 
								// todo - if multiple crews from the same club in the same category put the stroke's name - currently being overridden after manual observation 
								foreach (var l in objects)
								{
									table.AddCell(new PdfPCell(new Phrase(l.Item1.TrimEnd(), l.Item2)) { Border = 0 });
								}

								if (categorypos[crew.StartNumber] == 1)
									Debug.WriteLine("{3}", crew.StartNumber, crew.AthleteName(1, true), crew.Name, crew.SubmittingEmail);
							}
							catch (Exception ex)
							{
								logger.DebugFormat("Cannot output result for  crew {0}", crew.StartNumber);
							}

						}
						using (System.IO.StreamWriter file = new System.IO.StreamWriter (ConfigurationManager.AppSettings ["racecode"].ToString () + "-results.txt")) {
							file.Write (sb.ToString ());
						}

						document.Add (table);
						document.Add (new Paragraph ("Categories shown in italics have not attracted sufficient entries to qualify for prizes.", italic));
						document.Add (new Paragraph ("The adjusted and foreign prizes are open to all indicated crews and will be awarded based on adjusted times as calculated according to the tables in the Rules of Racing", font));
						document.Add (new Paragraph (updated, font));

						document.AddTitle ("Designed by www.vestarowing.co.uk");
						document.AddAuthor (string.Format ("Chris Harrison, {0} Timing and Results", ConfigurationManager.AppSettings ["racenamelong"]));
						document.AddKeywords (string.Format ("{0}, {1}, Draw", ConfigurationManager.AppSettings ["racenamelong"], racedate.Year));

						document.Close ();
					}
				}
			}
		}

		static string format0(int i)
		{
			if (i == 0)
				return string.Empty;
			return i.ToString ();
		}

		static int CategoryNotes(ICrew crew, Func<ICategory, bool> predicate, bool useDefault, StringBuilder stringBuilder)
		{
			ICategory cat = useDefault ? crew.Categories.FirstOrDefault (c => predicate (c)) : crew.Categories.First (c => predicate (c));
			if(cat == null) 
				return 0;

			int position = crew.CategoryPosition (cat);
			if (position == 1)
				stringBuilder.AppendFormat ("{0} winner. ", cat.Name);
			return position;
		}

	}
	
}
