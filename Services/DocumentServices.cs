using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1;
using WpfApp2.Models;
using WpfApp2.Models.Items;
using WpfApp2.ViewModels.Fuel;
using WpfApp2.ViewModels.Resources;
using WpfApp2.Views.Fuel;
using Newtonsoft.Json;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using WpfApp2.ViewModels.Concrete;
using WpfApp2.Utilities;
using WpfApp2.ViewModels.Cement;
using WpfApp2.Views.Resources;
using System.Windows;



namespace WpfApp2.Services
{
    public class DocumentServices
    {
        public static void createFuelDocument()
        {
            using (var context = new ApplicationDbContext())
            {
                if (!File.Exists(AdditionalDataViewModel.AdditionalDataPath))
                {
                    throw new Exception("برجاء إدخال بيانات الضباط المسئولين عن التمام");
                }
                try
                {
                    List<FuelConsumptionRecord> fuelRecords        = ValidationServices.verifyFuelRecords();
                    List<ConcreteProductionRecord> concreteRecords = ValidationServices.verifyConcreteRecords(false);
                    populateFuelReport(fuelRecords, concreteRecords);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public static void populateFuelReport(List<FuelConsumptionRecord> fuelRecords,List<ConcreteProductionRecord> concreteRecords)
        {

            string filePath = "تمام السولار.docx";

            using (var wordDoc = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = new Body();

                // Arabic Culture and Title
                DateTime date = DateTime.Now;
                CultureInfo arabicCulture = new CultureInfo("ar-EG");
                string dayName = date.ToString("dddd", arabicCulture);
                string dateString = "";
                dateString += $"{DateTime.Now.Day.ToString("D2")}";
                dateString += " ";
                dateString += "/";
                dateString += " ";
                dateString += $"{DateTime.Now.Month.ToString("D2")}";
                dateString += " ";
                dateString += "/";
                dateString += " ";
                dateString += $"{DateTime.Now.Year}";
                string paragraphTitleString = "بيان تمام السولار عن يوم";
                paragraphTitleString += $" ";
                paragraphTitleString += $"{dayName}";
                paragraphTitleString += $" ";
                paragraphTitleString += $"الموافق";
                paragraphTitleString += $" ";
                paragraphTitleString += dateString;
                // Title Paragraph
                Paragraph titlePara = new Paragraph(
                    new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center },
                        new BiDi(),
                        new SpacingBetweenLines() { After = "200" }
                    ),
                    new Run(
                        new RunProperties(
                            new Bold(),
                            new Underline { Val = UnderlineValues.Single },
                            new BoldComplexScript(),
                            new RightToLeftText(),
                            new RunFonts() { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" },                            
                            new FontSize() { Val = "28" },
                            new FontSizeComplexScript { Val = "28" }
                        ),
                        new Text(TableCreating.replaceArabicToIndianDigits(paragraphTitleString)))
                    
                );
                body.Append(titlePara);                
                Table table = TableCreating.createFuelTable(fuelRecords, concreteRecords);

                ////////////////////////////////
                
                Paragraph spacer = new Paragraph(
                    new ParagraphProperties(
                        new SpacingBetweenLines { Before = "360" } // 360 = 0.25 inch
                    )
                );
                
                Table signatureTable = TableCreating.fuelReportSignatureTable();

                ///////////////////////////////
                var sectionProperties = new SectionProperties(
                            new PageSize()
                            {
                                Width = 11906, // Portrait A4
                                Height = 16838,
                                Orient = PageOrientationValues.Portrait
                            },
                            new PageMargin()
                            {
                                Top = (int)(1440 * 0.3),
                                Bottom = (int)(1440 * 0.3),
                                Left = (int)(1440 * 0.16),
                                Right = (int)(1440 * 0.16),
                            }
                );
                ////////////////////////////////
                body.Append(table);
                body.Append(spacer);
                body.Append(signatureTable);
                body.Append(sectionProperties);
                mainPart.Document.Append(body);
                mainPart.Document.Save();
            }
        }

        public static void createColonelDocument()
        {
            using (var context = new ApplicationDbContext())
            {
                
                if (!File.Exists(AdditionalDataViewModel.AdditionalDataPath))
                {
                    throw new Exception("برجاء إدخال بيانات الضباط المسئولين عن التمام");
                }
                
                
                List<ConcreteProductionRecord>  concreteRecords          = new List<ConcreteProductionRecord>();
                List<PreCastWallProgressRecord> wallRecords              = new List<PreCastWallProgressRecord>();

                try
                {
                    concreteRecords = ValidationServices.verifyConcreteRecords(true);
                    wallRecords     = ValidationServices.verifyWallRecords();
                    populateColonelReport(concreteRecords, wallRecords);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
        }
        public static void populateColonelReport(List<ConcreteProductionRecord>concreteRecords,List<PreCastWallProgressRecord>wallRecords)
        {
            string filePath = "تمام العقيد.docx";

            using (var wordDoc = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = new Body();

                // Arabic Culture and Title
                DateTime date = DateTime.Now;
                CultureInfo arabicCulture = new CultureInfo("ar-EG");
                string dayName = date.ToString("dddd", arabicCulture);
                string dateString = "";
                dateString += $"{DateTime.Now.Day.ToString("D2")}";
                dateString += " ";
                dateString += "/";
                dateString += " ";
                dateString += $"{DateTime.Now.Month.ToString("D2")}";
                dateString += " ";
                dateString += "/";
                dateString += " ";
                dateString += $"{DateTime.Now.Year}";
                string paragraphTitleString = "بيان إنتاج الخرسانة للواء ٢٣ إنشاءات ووحداته الفرعية عن يوم";
                paragraphTitleString += $" ";
                paragraphTitleString += $"{dayName}";
                paragraphTitleString += $" ";
                paragraphTitleString += $"الموافق";
                paragraphTitleString += $" ";
                paragraphTitleString += dateString;
                // Title Paragraph
                Paragraph titlePara = new Paragraph(
                    new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center },
                        new BiDi(),
                        new SpacingBetweenLines() { After = "200" }
                    ),
                    new Run(
                        new RunProperties(
                            new Bold(),
                            new Underline { Val = UnderlineValues.Single },
                            new BoldComplexScript(),
                            new RightToLeftText(),
                            new RunFonts() { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" },
                            new FontSize() { Val = "28" },
                            new FontSizeComplexScript { Val = "28" }
                        ),
                        new Text(TableCreating.replaceArabicToIndianDigits(paragraphTitleString))
                    )
                );
                body.Append(titlePara);

                Table table = TableCreating.createColonelConcreteTable(concreteRecords);

                body.Append(table);

                /////////////////////////////
                // Assume table and intro paragraph were already added

                var endPortraitParagraph = new Paragraph(new Run(new Text("")))
                {
                    ParagraphProperties = new ParagraphProperties(
                        new SectionProperties(
                            new PageSize()
                            {
                                Width = 11906, // Portrait A4
                                Height = 16838,
                                Orient = PageOrientationValues.Portrait
                            },
                            new PageMargin()
                            {
                                Top = (int)(1440 * 0.39),
                                Bottom = (int)(1440 * 0.69),
                                Left = (int)(1440 * 0.13),
                                Right = (int)(1440 * 0.13),
                            }
                        )
                    )
                };
                body.Append(endPortraitParagraph);



                /////////////////////////////
                paragraphTitleString = "تمام انتاج السور الخرسانى سابق الصب بارتفاع 2.5 م بالمتر الطولى عن يوم";
                paragraphTitleString += $" ";
                paragraphTitleString += $"{dayName}";
                paragraphTitleString += $" ";
                paragraphTitleString += $"الموافق";
                paragraphTitleString += $" ";
                paragraphTitleString += dateString;

                // Title Paragraph
                Paragraph secondtitlePara = new Paragraph(
                    new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center },
                        new BiDi(),
                        new SpacingBetweenLines() { After = "200" }
                    ),
                    new Run(
                        new RunProperties(
                            new Bold(),
                            new Underline { Val = UnderlineValues.Single },
                            new BoldComplexScript(),
                            new RightToLeftText(),
                            new RunFonts() { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" },
                            new FontSize() { Val = "36" },
                            new FontSizeComplexScript { Val = "36" }
                        ),
                        new Text(TableCreating.replaceArabicToIndianDigits(paragraphTitleString))
                    )
                );

                Paragraph thirdtitlePara = new Paragraph(
                    new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Left },
                        new BiDi(),
                        new SpacingBetweenLines() { After = "200" }
                    ),
                    new Run(
                        new RunProperties(
                            new Bold(),
                            new Underline { Val = UnderlineValues.Single },
                            new BoldComplexScript(),
                            new RightToLeftText(),
                            new RunFonts() { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" },
                            new FontSize() { Val = "28" },
                            new FontSizeComplexScript { Val = "28" }
                        ),
                        new Text($"موقف النقل :")
                    )
                );

                List<Table> wallTables = TableCreating.createWallTables(wallRecords);


                body.Append(secondtitlePara);

                body.Append(wallTables.ElementAt(0)); 

                body.Append(thirdtitlePara);

                body.Append(wallTables.ElementAt(1));


                //////////////////

                var startLandscapeSection = new SectionProperties(
                    new PageSize()
                    {
                        Width = 16838, // Landscape A4
                        Height = 11906,
                        Orient = PageOrientationValues.Landscape
                    },
                    new PageMargin()
                    {
                        Top = (int)(1440 * 0.3),
                        Bottom = (int)(1440 * 1),
                        Left = (int)(1440 * 0.50),
                        Right = (int)(1440 * 0.50),
                    }
                );

                body.Append(startLandscapeSection);

                //////////////////

                mainPart.Document.Append(body);
                mainPart.Document.Save();
            }
        }
        public static void createHQDocument()
        {
            using (var context = new ApplicationDbContext())
            {

                if (!File.Exists(AdditionalDataViewModel.AdditionalDataPath))
                {
                    throw new Exception("برجاء إدخال بيانات الضباط المسئولين عن التمام");
                }
                bool areThereConcreteRecords                   = !ConcreteService.canAddRecords();
                bool areThereWallRecords                       = !PreCastWallService.canAddRecords();
                bool areThereCementRecords                     = !CementService.canAddRecords();
                List<ConcreteProductionRecord> concreteRecords = new List<ConcreteProductionRecord>();
                List<PreCastWallProgressRecord> wallRecords    = new List<PreCastWallProgressRecord>();
                List<CementDailyRecord> cementRecords          = new List<CementDailyRecord>();
                try
                {
                    concreteRecords = ValidationServices.verifyConcreteRecords(false);
                    cementRecords   = ValidationServices.verifyCementRecords();
                    wallRecords     = ValidationServices.verifyWallRecords();
                    populateHQReport(concreteRecords.Where(x=>x.isInformal == false).ToList(), cementRecords, wallRecords);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
        }
        public static void populateHQReport(List<ConcreteProductionRecord> concreteRecords, List<CementDailyRecord> cementDailyRecords, List<PreCastWallProgressRecord> wallRecords)
        {
            string filePath = "تمام الإدارة.docx";

            using (var wordDoc = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = new Body();


                //////////////////Declare a Header

                HeaderPart headerPart = mainPart.AddNewPart<HeaderPart>();
                string headerPartId = mainPart.GetIdOfPart(headerPart);

                Header header = new Header(
                    new Paragraph(
                        new ParagraphProperties(
                            new Justification() 
                            { Val = JustificationValues.Center }
                            ),
                        new Run(
                            new RunProperties(
                                new Bold(),
                                new Underline() { Val = UnderlineValues.Single }
                            ),           
                            new Text("ســـرى")
                            )
                    )
                );

                headerPart.Header = header;
                headerPart.Header.Save();

                HeaderReference headerReference = new HeaderReference()
                {
                    Type = HeaderFooterValues.Default,
                    Id = headerPartId
                };


                //////////////////////////////////

                // Arabic Culture and Title
                DateTime date = DateTime.Now;
                CultureInfo arabicCulture = new CultureInfo("ar-EG");
                string dayName = date.ToString("dddd", arabicCulture);
                string dateString = "";
                dateString += $"{DateTime.Now.Day.ToString("D2")}";
                dateString += " ";
                dateString += "/";
                dateString += " ";
                dateString += $"{DateTime.Now.Month.ToString("D2")}";
                dateString += " ";
                dateString += "/";
                dateString += " ";
                dateString += $"{DateTime.Now.Year}";
                string paragraphTitleString = "بيان إنتاج الخرسانة للواء ٢٣ إنشاءات ووحداته الفرعية عن يوم";
                paragraphTitleString += $" ";
                paragraphTitleString += $"{dayName}";
                paragraphTitleString += $" ";
                paragraphTitleString += $"الموافق";
                paragraphTitleString += $" ";
                paragraphTitleString += dateString;

                List<Mixer> mixerList = MixerService.getOperationalMixers();
                mixerList        = mixerList.OrderBy(x=>x.cabbageNo).ToList();
                int mixerPerPage = 4;
                int mixerCount   = mixerList.Count;
                int pageCount    = (mixerCount + mixerPerPage - 1) / mixerPerPage;
                double epsilon   = 1e-8;

                ///////////////////////// Sum Concrete for the final

                double sumReinforcedConcrete = (double)concreteRecords.Where(x => x.isReinforcedConcrete == true).Select(x =>(decimal) x.producedConcreteAmount).Sum();
                double sumNormalConcrete     = (double)concreteRecords.Where(x => x.isReinforcedConcrete == false).Select(x =>(decimal) x.producedConcreteAmount).Sum();
                string concreteSummationInDetail;
                concreteSummationInDetail = "";
                if (sumReinforcedConcrete > epsilon)
                {
                    concreteSummationInDetail += $"{sumReinforcedConcrete.ToString("G29")}";
                    concreteSummationInDetail += " ";
                    concreteSummationInDetail += "م3 خرسانة مسلحة";
                }
                if (sumNormalConcrete > epsilon)
                {
                    if (concreteSummationInDetail.Length > 0)
                    {
                        concreteSummationInDetail += "\n";
                        concreteSummationInDetail += $"{sumNormalConcrete.ToString("G29")}";
                        concreteSummationInDetail += " ";
                        concreteSummationInDetail += "م3 خرسانة عادية";
                    }
                    else
                    {
                        concreteSummationInDetail += $"{sumNormalConcrete.ToString("G29")}";
                        concreteSummationInDetail += " ";
                        concreteSummationInDetail += "م3 خرسانة عادية";
                    }
                }
                if (concreteSummationInDetail.Length == 0)
                {
                    concreteSummationInDetail = "-";
                }


                ///////////////////////Signature Paragraph
                var additionalRecordsJsonString = File.ReadAllText(AdditionalDataViewModel.AdditionalDataPath);
                AdditionalInfo infoData = JsonConvert.DeserializeObject<AdditionalInfo>(additionalRecordsJsonString);
                string signatureName = "";
                string SignatureFirstLine = "";
                SignatureFirstLine += " التوقيع(             )";
                signatureName += $"{infoData.deputyRank}";
                signatureName += " ";
                signatureName += "/";
                signatureName += " ";
                signatureName += $"{infoData.deputyName}";
                Paragraph Signature = new Paragraph(
                        new ParagraphProperties(
                            new Languages() { Bidi = "ar-SA" },
                            new Justification { Val = JustificationValues.Right },
                            new Indentation { Left = "0", Right = $"{1440 * 8}", Hanging = "0" },
                            new SpacingBetweenLines
                            {
                                Before = "360" // 360 twips = 0.25 inch
                            }
                        ),
                        new Run(new RunProperties(
                                new RightToLeftText(),
                                new Languages() { Bidi = "ar-SA" },
                                new Bold(),
                                new BoldComplexScript(),
                                new RunFonts { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" },
                                new FontSize { Val = "32" },
                                new FontSizeComplexScript { Val = "32" },
                                new Text($"{SignatureFirstLine}"),
                                new Break(),
                                new Text($"{signatureName}"),
                                new Break(),
                                new Text(TableCreating.replaceArabicToIndianDigits("قائد منوب اللواء 23 إنشاءات"))
                        )));
                /////////////////////// Title Paragraph
                
                Paragraph titlePara = new Paragraph(
                        new ParagraphProperties(
                            new Justification() { Val = JustificationValues.Center },
                            new BiDi(),
                            new SpacingBetweenLines() { After = "200" }
                        ),
                        new Run(
                            new RunProperties(
                                new Languages() { Bidi = "ar-SA" },
                                new Bold(),
                                new BoldComplexScript(),
                                new Underline { Val = UnderlineValues.Single },
                                new RunFonts() { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" },
                                new FontSize() { Val = "28" },
                                new FontSizeComplexScript { Val = "28" },
                                new RightToLeftText()
                            ),
                            new Text(TableCreating.replaceArabicToIndianDigits(paragraphTitleString))
                        )
                    );

                ////////////////////// Skip to a new Page
                var landscapeSection = new Paragraph(new Run(new Text("")))
                {
                    ParagraphProperties = new ParagraphProperties(
                        new SectionProperties(
                                        new HeaderReference()
                                        {
                                            Type = HeaderFooterValues.Default,
                                            Id = headerPartId
                                        },
                                       new PageSize()
                                       {
                                           Width = 16838, // Landscape A4
                                           Height = 11906,
                                           Orient = PageOrientationValues.Landscape
                                       },
                                       new PageMargin()
                                       {
                                           Header = (int)(1440 * 0.1),
                                           Top = (int)(1440 * 0.25),
                                           Bottom = (int)(1440 * 0.15),
                                           Left = (int)(1440 * 0.25),
                                           Right = (int)(1440 * 0.25),
                                       }
                                   ))
                };
                

                //////////////////////

                for (int pageCounter = 0; pageCounter < pageCount; pageCounter++)
                {

                    body.Append(titlePara.CloneNode(true));
                    List<Mixer> subMixerList                           = mixerList.GetRange(pageCounter*mixerPerPage, int.Min((pageCounter + 1) * mixerPerPage, mixerCount) - pageCounter * mixerPerPage);
                    List<int> subMixerIDList                           = subMixerList.Select(x => x.mixerID).ToList();
                    List < CementDailyRecord >     subCementRecord     = cementDailyRecords.Where(x=> subMixerIDList.Contains(x.mixerID)).ToList();
                    List<ConcreteProductionRecord> subConcreteRecords  = concreteRecords.Where(x => subMixerIDList.Contains(x.mixerID)).ToList();
                    List<List<string>>             entryLists          = MixerService.prepareTableEntries(subConcreteRecords, subCementRecord, subMixerList, (pageCounter * mixerPerPage)+1);
                    
                    /////////////////
                    if (pageCounter < pageCount - 1) 
                    {
                        Table table = TableCreating.createHQConcreteTable(entryLists, null,false);

                        body.Append(table);
                    }
                    else
                    {
                        List<string> cementDataFinalRow     = TableCreating.prepareFinalRowCementValues(cementDailyRecords, mixerList);
                        int biggestCabbage = mixerList.Max(x => x.cabbageNo);
                        List<string> finalRowEntries        = new List<string> { cementDataFinalRow.ElementAt(0), cementDataFinalRow.ElementAt(1), cementDataFinalRow.ElementAt(2), cementDataFinalRow.ElementAt(3), "-","-","-",concreteSummationInDetail,"-", "-", "-", "-", biggestCabbage.ToString(), "الإجمالى" };
                        Table table = TableCreating.createHQConcreteTable(entryLists, finalRowEntries, true);
                        body.Append(table);
                    }
                    ///////////// Add the signature
                    body.Append(Signature.CloneNode(true));
                    ///////////// Get to a new Page
                    body.Append(landscapeSection.CloneNode(true));

                }
                Table barricadesTable = TableCreating.BarricadeReportTable();
                body.Append(titlePara.CloneNode(true));
                body.Append(barricadesTable);
                body.Append(Signature.CloneNode(true));
                body.Append(landscapeSection.CloneNode(true));


                ///////////////////////////// PreCast Wall Section
                paragraphTitleString = "تمام انتاج السور الخرسانى سابق الصب بارتفاع 2.5 م بالمتر الطولى عن يوم";
                paragraphTitleString += $" ";
                paragraphTitleString += $"{dayName}";
                paragraphTitleString += $" ";
                paragraphTitleString += $"الموافق";
                paragraphTitleString += $" ";
                paragraphTitleString += dateString;

                // Title Paragraph
                Paragraph secondtitlePara = new Paragraph(
                    new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center },
                        new BiDi(),
                        new SpacingBetweenLines() { After = "200" }
                    ),
                    new Run(
                        new RunProperties(
                            new Bold(),
                            new BoldComplexScript(),
                            new Underline { Val = UnderlineValues.Single },
                            new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                            new FontSize() { Val = "36" },
                            new FontSizeComplexScript { Val = "36" },
                            new RightToLeftText()
                        ),
                        new Text(TableCreating.replaceArabicToIndianDigits(paragraphTitleString))
                    )
                );

                Paragraph thirdtitlePara = new Paragraph(
                    new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Left },
                        new BiDi(),
                        new SpacingBetweenLines() { After = "200" }
                    ),
                    new Run(
                        new RunProperties(
                            new Bold(),
                            new BoldComplexScript(),
                            new Underline { Val = UnderlineValues.Single },
                            new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" },
                            new FontSize() { Val = "28" },
                            new FontSizeComplexScript { Val = "28" },
                            new RightToLeftText()
                        ),
                        new Text($"موقف النقل :")
                    )
                );

                List<Table> wallTables = TableCreating.createWallTables(wallRecords);


                body.Append(secondtitlePara);

                body.Append(wallTables.ElementAt(0));

                body.Append(thirdtitlePara);

                body.Append(wallTables.ElementAt(1));
                body.Append(Signature.CloneNode(true));

                //////////////////

                var startLandscapeSection = new SectionProperties(
                    new HeaderReference()
                    {
                        Type = HeaderFooterValues.Default,
                        Id = headerPartId
                    },
                    new PageSize()
                    {
                        Width = 16838, // Landscape A4
                        Height = 11906,
                        Orient = PageOrientationValues.Landscape
                    },
                    new PageMargin()
                    {
                        Header = (int)(1440 * 0.1),
                        Top = (int)(1440 * 0.3),
                        Bottom = (int)(1440 * 0.3),
                        Left = (int)(1440 * 0.50),
                        Right = (int)(1440 * 0.50),
                    }
                );

                body.Append(startLandscapeSection);
                mainPart.Document.Append(body);
                mainPart.Document.Save();

            }
        }
    }
}
