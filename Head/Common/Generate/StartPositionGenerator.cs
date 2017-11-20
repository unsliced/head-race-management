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
using Newtonsoft.Json;
using System.Configuration;

namespace Head.Common.Generate
{
    public class StartPositionGenerator
	{
		public static void Generate(IEnumerable<ICrew> ecrews)
		{
			ILog logger = LogManager.GetCurrentClassLogger ();

			IList<ICrew> crews = ecrews.ToList();
			int showAthlete = Int32.Parse(ConfigurationManager.AppSettings ["showcompetitor"].ToString ());
			if(crews.Any(cr => cr.StartNumber > 0))
			{
				logger.Info ("crews have start numbers. ");
				if(crews.Any(cr => cr.StartNumber <= 0))
					logger.Warn ("but some don't, that's not right - delete the start positions or fix the JSON.");
				Dump (crews, showAthlete);
				return;
			}
			IList<string> startpositions = new List<string> ();

			int lym = Int32.Parse (ConfigurationManager.AppSettings ["LastYearMen"].ToString ());
			int lyw = Int32.Parse(ConfigurationManager.AppSettings["LastYearWomen"].ToString());
			int lywo = Int32.Parse(ConfigurationManager.AppSettings["LastYearWomenOrder"].ToString());

			foreach(var crew in 
				crews
				.Where(cr => !cr.IsScratched) //  && cr.IsAccepted) 
				.OrderBy(cr => cr.EventCategory.Gender == Gender.Open && cr.PreviousYear.HasValue && cr.PreviousYear.Value <= lym ? cr.PreviousYear.Value : lym+1)
			        // if the cat order is before last year's women, observe it, otherwise part it - so that last year's women aren't directly behind last year's men. 
				.ThenBy(cr => cr.Categories.First(cat => cat is EventCategory).Order < lywo ? cr.Categories.First(cat => cat is EventCategory).Order : lywo)

				.ThenBy(cr => cr.EventCategory.Gender == Gender.Female && cr.PreviousYear.HasValue && cr.PreviousYear.Value <= lyw ? cr.PreviousYear.Value : lyw+1)
				.ThenBy(cr => cr.Categories.First(cat => cat is EventCategory).Order)
				.ThenBy(cr => cr.CrewId.Reverse()))
			// todo - put the vets head vs scullers head into config 
//			foreach(var crew in 
//				crews
//					.OrderBy(cr => cr.Categories.First(cat => cat is EventCategory).Order)
//					.ThenBy(cr => cr.PreviousYear.HasValue && cr.PreviousYear.Value <= 3 ? cr.PreviousYear.Value : 5)
//					.ThenBy(cr => cr.CrewId.Reverse()))
			{
				logger.InfoFormat("{0} [{2}], {1}, {3}, {4}", 
					crew.Name, crew.CategoryName, 
					crew.AthleteName(showAthlete, true), crew.CrewId, 
					crew.PreviousYear.HasValue ? crew.PreviousYear : -1);
				startpositions.Add(String.Format("{{\"CrewId\":{0},\"StartNumber\":{1}}}", crew.CrewId, startpositions.Count+1));
			}
			logger.Info(startpositions.Delimited());
		}

		static void Dump(IEnumerable<ICrew> crews, int showAthlete)
		{
			ILog logger = LogManager.GetCurrentClassLogger ();

			string json = JsonConvert.SerializeObject (crews.Select (cr => new { cr.StartNumber, cr.Name}).OrderBy(cr => cr.StartNumber));
			logger.InfoFormat ("JSON: {0}", json);

			DateTime racedate = DateTime.MinValue;
			if(!DateTime.TryParse(ConfigurationManager.AppSettings["racedate"].ToString(), out racedate))
				racedate = DateTime.MinValue;

			string raceDetails = string.Format("{0} - {1} - Draw", ConfigurationManager.AppSettings["racenamelong"], racedate.ToLongDateString());
			string updated = "Updated: \t" + DateTime.Now.ToShortTimeString () + " " + DateTime.Now.ToShortDateString ();
			StringBuilder sb = new StringBuilder ();
			sb.AppendLine (updated);
			StringBuilder sql = new StringBuilder ();

			var orders = new Dictionary<string, IOrderedEnumerable<ICrew>> {
				{string.Empty, crews.OrderBy (cr => cr.StartNumber)},
				{" by boating location", crews.OrderBy (cr => cr.BoatingLocation == null ? "unknown" : cr.BoatingLocation.Name).ThenBy (cr => cr.StartNumber)},
				{" by club name", crews.OrderBy (cr => cr.Name).ThenBy (cr => cr.StartNumber)},
			};

			foreach(var kvp in orders)
			{
				using (var fs = new FileStream (string.Format ("{0} {1} Draw{2}.pdf", ConfigurationManager.AppSettings ["racenamelong"], racedate.ToString ("yyyy"), kvp.Key), FileMode.Create)) {

					using (Document document = new Document (PageSize.A4.Rotate ())) {

						Font font = new Font (Font.FontFamily.HELVETICA, 7f, Font.NORMAL);
						Font italic = new Font (Font.FontFamily.HELVETICA, 7f, Font.ITALIC);
						Font bold = new Font (Font.FontFamily.HELVETICA, 7f, Font.BOLD);
						Font strike = new Font (Font.FontFamily.HELVETICA, 7f, Font.STRIKETHRU);

						// step 2:
						// we create a writer that listens to the document and directs a PDF-stream to a file            
						PdfWriter.GetInstance (document, fs);

						// step 3: we open the document
						document.Open ();

						// entitle the document 
						document.Add (new Paragraph (raceDetails));
						document.AddSubject (raceDetails);

						// grab the header and seed the table 
						// todo these need to be wider for the vets because of the composites 
                        // number - entry - composite - id - cat - boating - prize - notes 
						float[] widths = new float[] { 1f, 4f, 5f, 3f, 3f, 3f, 2f, 3f };
						PdfPTable table = new PdfPTable (widths.Count ()) {
							TotalWidth = 800f,
							LockedWidth = true,                    
							HorizontalAlignment = 0,
							SpacingBefore = 20f,
							SpacingAfter = 30f,
						};
						table.SetWidths (widths);

						foreach (var h in new List<string> { "Start", "BROE Entry", "Club Names", showAthlete == 1 ? "Sculler" : "BROE CrewID", "Category", "Boating", "Other prizes","Notes" }) {
							table.AddCell (new PdfPCell (new Phrase (h)) { Border = 1, HorizontalAlignment = 2, Rotation = 90 });
							sb.AppendFormat ("{0}\t", h);
						}
						sb.AppendLine ();

						string UNPAID = "UNPAID"; 
						foreach (var crew in kvp.Value) { //  crews.OrderBy(cr => cr.StartNumber)) 
							ICategory primary;
							string extras = String.Empty;
							// todo - transfer this into the crew itself 
							if (crew.Categories.Any (c => c is TimeOnlyCategory)) { 
								primary = crew.Categories.First (c => c is TimeOnlyCategory);
							} else {
								primary = crew.Categories.First (c => c is EventCategory);
								extras = crew.Categories.Where (c => !(c is EventCategory) && !(c is OverallCategory) && c.Offered).Select (c => c.Name).Delimited ();
							}
							var objects = new List<Tuple<string, Font>> { 
								new Tuple<string, Font> (crew.StartNumber.ToString (), font),
								new Tuple<string, Font> (crew.RawName, crew.IsScratched ? strike : font),
								new Tuple<string, Font> (crew.Name, crew.IsScratched ? strike : font),

								new Tuple<string, Font> (showAthlete  == 1 ? crew.AthleteName (showAthlete, false) : crew.CrewId.ToString(), crew.IsScratched ? strike : font),
								new Tuple<string, Font> (crew.CategoryName, font), //, primary.Offered ? font : italic),
								new Tuple<string, Font> (crew.BoatingLocation == null ? "unknown" : crew.BoatingLocation.Name, crew.BoatingLocation == null ? italic : font),
								new Tuple<string, Font> (extras, font), 
								new Tuple<string, Font> ((crew.IsScratched ? "SCRATCHED" : String.Empty) + " " 
									+ (crew.IsPaid || crew.IsAccepted ? String.Empty : UNPAID) + " " 
									+ crew.VoecNotes, bold), 
							};
							sql.AppendFormat ("connection.Execute(\"insert into Boats (_race, _number, _name) values (?, ?, ?)\", \"{0}\", {1}, \"[{2} / {3} / {4}]\");{5}", 
								ConfigurationManager.AppSettings ["racecode"].ToString (), crew.StartNumber, 
								crew.Name, crew.AthleteName (showAthlete, false), primary.Name, Environment.NewLine);
						
							sb.AppendFormat ("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}{8}", objects [0].Item1, objects [1].Item1, objects [2].Item1, objects [3].Item1, objects [4].Item1, objects [5].Item1, objects [6].Item1, crew.ShortName, Environment.NewLine);
							foreach (var l in objects)
								table.AddCell (new PdfPCell (new Phrase (l.Item1.TrimEnd (), l.Item2)) { Border = 0 }); 
						}

						if (string.IsNullOrEmpty (kvp.Key)) {
							using (StreamWriter file = new StreamWriter(ConfigurationManager.AppSettings ["racecode"].ToString () + ".txt")) {
								file.Write (sb.ToString ());
							}
							using (StreamWriter file = new StreamWriter(ConfigurationManager.AppSettings ["racecode"].ToString () + ".sql")) {
								file.Write (sql.ToString ());
							}
								
							// todo - need a crew index to be written out 
							// todo - this might well break with timeonly crews 
							json = JsonConvert.SerializeObject (crews
								.Select (cr => 
									new { cr.StartNumber, Name = cr.Name + " - " + cr.AthleteName (showAthlete, false), Category = cr.EventCategory.Name, Scratched = cr.IsScratched})
								.OrderBy (cr => cr.StartNumber));

							using (StreamWriter file = new StreamWriter(Path.Combine (ConfigurationManager.AppSettings ["dbpath"].ToString (), ConfigurationManager.AppSettings ["racecode"].ToString () + "-draw.json"))) {
								file.Write (json.ToString ());
							}
						}

						document.Add (table);
						document.Add (new Paragraph ("Any queries should be directed to scullers.head@vestarowing.co.uk", bold)); // todo - race-dependent email address 
					    document.Add (new Paragraph ("Unpaid crews will not be allowed to race. Crews that have scratched but remain unpaid run the risk of future sanction.", bold));
	                    // document.Add (new Paragraph ("Categories shown in italics have not attracted sufficient entries to qualify for a category prize.", italic));
						document.Add (new Paragraph ("Any adjusted prizes are open to all indicated crews and will be awarded based on adjusted times as calculated according to the tables in the Rules of Racing", font));
						document.Add (new Paragraph (updated, font));
						document.AddTitle ("Designed by www.vestarowing.co.uk");
						document.AddAuthor (string.Format ("Chris Harrison, {0} Timing and Results", ConfigurationManager.AppSettings ["racenamelong"]));
						document.AddKeywords (string.Format ("{0}, {1}, Draw", ConfigurationManager.AppSettings ["racenamelong"], racedate.Year));

						document.Close ();
					}
				}
			}
		}
	}
}
