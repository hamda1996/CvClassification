using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ClassLibraryCV.Prediction;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace ClassLibraryCV
{
    public class CvService
    {        
        public CvAnalysisResult TextFromWord(string filePath, Stream stream)
        {            
            string errorFile = "";
            List<TechnoDuree> listTechnoDuree = new List<TechnoDuree>();
            List<CvQuality> listCvQuality = new List<CvQuality>();
            var labelService = new TechnologyLabelService();
            Scoring scoring = new Scoring();
            var nameFile = Path.GetFileName(filePath);

            try
            {
                using (var wdDoc = WordprocessingDocument.Open(stream, false))
                {
                    var elements = wdDoc.MainDocumentPart.Document.Body.Elements().ToList();
                   
                    AnalyzeCv(listTechnoDuree, listCvQuality, labelService, nameFile, elements);

                    foreach (var tech in listTechnoDuree)
                    {
                        tech.UpdateListIntervalle();
                        tech.UpdateDuree();
                        foreach (var techDate in tech.ListIntervalle)
                        {
                            tech.Score += scoring.ScoringCalculate(tech.Duree, tech.Technologie, techDate.DateEnd);
                        }
                    }
                }
            }
            catch (Exception)
            {
                errorFile = "Fichier non reconnu";
            }

            return new CvAnalysisResult(listCvQuality, listTechnoDuree, errorFile);
        }

        private static void AnalyzeCv(List<TechnoDuree> listTechnoDuree, List<CvQuality> listCvQuality, TechnologyLabelService labelService, string nameFile, List<DocumentFormat.OpenXml.OpenXmlElement> elements)
        {
            string[] keywords = new[] { "EXPERIENCES", "Expériences" };
            foreach (var keyword in keywords)
            {
                var elementsAfterExperience = elements.SkipWhile(e => !(e is Paragraph p && p.InnerText.Contains(keyword, StringComparison.CurrentCultureIgnoreCase))).Skip(1).Where(e => e is Table).ToList();
                for (int k = 0; k < elementsAfterExperience.Count; k++)
                {
                    if (elementsAfterExperience[k] is Table table)
                    {
                        foreach (TableRow child in table?.ChildElements.OfType<TableRow>() ?? Enumerable.Empty<TableRow>())
                        {
                            DateTime fromDate = DateTime.Now;
                            DateTime toDate = DateTime.Now;
                            var children = child.ChildElements.OfType<TableCell>().Where(x => !string.IsNullOrWhiteSpace(x.InnerText)).ToList();

                             string dateString = "";
                            try
                            {
                                if (children.Count < 2)
                                {
                                    break;
                                }

                                dateString = children[1].InnerText;
                                bool isDateWithoutEnd = false;
                                bool isDateWithMiddle = false;
                                string[] starts = new[] { "depuis le", "Depuis" };

                                foreach (var start in starts)
                                {
                                    if (children[1].InnerText.Contains(start, StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        fromDate = DateTime.Parse(children[1].InnerText.Remove(0, start.Length).Trim());
                                        toDate = DateTime.Now;
                                        
                                        isDateWithoutEnd = true;
                                        break;
                                    }
                                }
                               
                                if (!isDateWithoutEnd)
                                {
                                    string[] middles = new[] { "à", "au", "-", "–"};   

                                    foreach (var middle in middles)
                                    {
                                        if (children[1].InnerText.Contains(middle, StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            string[] parts = children[1].InnerText.Split(new[] { middle }, StringSplitOptions.RemoveEmptyEntries);
                                            if (parts[0].TrimEnd(' ').Length == 4)  //lorsqu'on a une seule année '2010 à 2011'
                                            {                                                
                                                int year = Int32.Parse(parts[0]);
                                                parts[0] = "01/01/" + year;                                                
                                            }

                                            fromDate = DateTime.Parse(parts[0]);                                            
                                                                                        
                                            if (parts[1].Contains("aujourd’hui", StringComparison.CurrentCultureIgnoreCase))
                                            {
                                                toDate = DateTime.Now;
                                            }
                                            else if (parts[1].Contains("aujourd'hui", StringComparison.CurrentCultureIgnoreCase))
                                            {
                                                toDate = DateTime.Now;
                                            }
                                            else if (parts[1].Contains("ce jour", StringComparison.CurrentCultureIgnoreCase))
                                            {
                                                toDate = DateTime.Now;
                                            }
                                            else 
                                            {
                                                if (parts[1].TrimStart(' ').Length == 4)//lorsqu'on a '2010 à 2011
                                                {
                                                    int year = Int32.Parse(parts[1]);
                                                    parts[1] = "31/12/" + year;
                                                }
                                                toDate = DateTime.Parse(parts[1]);                                                
                                            }
                                            isDateWithMiddle = true;
                                            break;
                                        }
                                    }                                                                    
                                    
                                }
                                if (children[1].InnerText.Length == 4)// lorsqu'on a une seule année comme 2007
                                {
                                    int year = Int32.Parse(children[1].InnerText);
                                    fromDate = DateTime.Parse("01/01/" + year);
                                    toDate = DateTime.Parse("31/12/" + year);
                                }

                                if (!isDateWithoutEnd && !isDateWithMiddle)
                                {
                                    fromDate = DateTime.Parse(children[1].InnerText);
                                    toDate = fromDate;
                                }
                               
                                if (elementsAfterExperience[k + 1] is Table nextTable)
                                {
                                    foreach (TableRow childnextTable in nextTable?.ChildElements.OfType<TableRow>() ?? Enumerable.Empty<TableRow>())
                                    {
                                        var nextTableChildren = childnextTable.ChildElements.OfType<TableCell>().Where(x => !string.IsNullOrWhiteSpace(x.InnerText)).ToList();
                                        if (nextTableChildren.Count >= 2 && nextTableChildren[0].InnerText.Contains("technologie", StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            string toto = nextTableChildren[1].InnerText;
                                                                                   
                                            if (nextTableChildren[1].InnerText.Contains("("))
                                            {                                                 
                                                string pattern = @"\(.+?\)";
                                                Regex rgx = new Regex(pattern); //utilisation des expressions regulières  
                                                toto = rgx.Replace(nextTableChildren[1].InnerText, "");   // remplace les elements entre parentheses par " " 
                                                MatchCollection matches = rgx.Matches(nextTableChildren[1].InnerText);
                                                foreach(Match match in matches)
                                                {                                                    
                                                    CvQuality quality = new CvQuality(nameFile, "erreur", "les éléments entre parenthèses n'ont pas été pris en compte", match.Value);
                                                    var defaultExistant = listCvQuality.FirstOrDefault(x => x.Details == quality.Details);
                                                    if(defaultExistant == null)     // si le defaut n'existe pas, l'ajouter a la listCvquality
                                                    {
                                                        listCvQuality.Add(quality);
                                                    }                                                   
                                                }                                                
                                            }                                           
                                            string[] technologies = toto.TrimEnd('.').Split(','); //suppression du '.' à la fin des technologies avec TrimEnd et split apres la virgule et le /  
                                            
                                            foreach (var tech in technologies)
                                            {
                                                string[] techsWithSlash = new[] { "PL/SQL" }; //tableau des technos qui s'écrivent normalement avec "/"

                                                if (tech.Contains('/') && !techsWithSlash.Contains(tech))
                                                {  
                                                   
                                                    CvQuality cvQuality = new CvQuality(nameFile, "erreur", "veuillez bien ecrire la technologie", tech);
                                                    listCvQuality.Add(cvQuality);                                                                                                                                                                 
                                                }
                                                else
                                                {
                                                    Intervalle listDateStartEnd = new Intervalle(fromDate, toDate);
                                                    string technologie = MakePrediction(labelService, tech);
                                                    TechnoDuree techno = new TechnoDuree(elements[0].InnerText, technologie.TrimStart(' ').TrimEnd(' '), listDateStartEnd);
                                                    var technoExistante = listTechnoDuree.FirstOrDefault(x => x.Technologie == techno.Technologie);
                                                    if (technoExistante == null)
                                                    {
                                                        listTechnoDuree.Add(techno);
                                                    }
                                                    else
                                                    {
                                                        technoExistante.ListIntervalle.Add(listDateStartEnd);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch (FormatException)
                            {
                                CvQuality cvQuality = new CvQuality(nameFile, "Erreur", "erreur date", dateString);
                                listCvQuality.Add(cvQuality);
                            }
                        }
                        k++;
                    }
                }
                if (elementsAfterExperience.Count >= 1)
                {
                    break;
                }
            }
            if (elements[0].InnerText.TrimStart(' ').Length > 3)
            {
                CvQuality quality = new CvQuality(nameFile, "Avertissement", "erreur nom", elements[0].InnerText );
                listCvQuality.Add(quality);
            }
        }

        private static string MakePrediction(TechnologyLabelService labelService, string description)
        {
            string prediction = labelService.PredictCategory(new TechnologyData
            {
                Description = description
            }, description);

            return prediction;
        }
        
    }
}