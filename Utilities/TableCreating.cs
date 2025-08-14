using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Vml.Office;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1;
using WpfApp2.Models;
using WpfApp2.Services;
using WpfApp2.ViewModels.Resources;

namespace WpfApp2.Utilities
{
    public class TableCreating
    {
        public static TableProperties tblProperties = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = BorderValues.ThinThickMediumGap, Size = 26 },
                    new BottomBorder { Val = BorderValues.ThickThinMediumGap, Size = 26 },
                    new LeftBorder { Val = BorderValues.ThinThickMediumGap, Size = 26 },
                    new RightBorder { Val = BorderValues.ThickThinMediumGap, Size = 26 },
                    new InsideHorizontalBorder { Val = BorderValues.Thick, Size = 20 },
                    new InsideVerticalBorder { Val = BorderValues.Thick, Size = 20 }
                ),
                new TableLayout() { Type = TableLayoutValues.Autofit },
                new TableJustification() { Val = TableRowAlignmentValues.Center }
        );

        public static TableRowProperties getTableRowProperties(double heightinInches = 0.45)
        {
            return new TableRowProperties(
                new TableRowHeight()
                {
                    Val = (UInt32Value)(uint)(1440 * heightinInches),
                    HeightType = HeightRuleValues.Exact // Can also use AtLeast
                });
        }
        public static string replaceArabicToIndianDigits(string input)
        {
            return input
                        .Replace("0", "٠")
                        .Replace("1", "١")
                        .Replace("2", "٢")
                        .Replace("3", "٣")
                        .Replace("4", "٤")
                        .Replace("5", "٥")
                        .Replace("6", "٦")
                        .Replace("7", "٧")
                        .Replace("8", "٨")
                        .Replace("9", "٩");
        }

        public static List<string> prepareFinalRowCementValues(List<CementDailyRecord> cementDailyRecords, List<Mixer> mixerList)
        {

            double sumImported = (double)cementDailyRecords.Sum(x => (decimal)x.importedCement);
            double sumConsumed = (double)cementDailyRecords.Sum(x => (decimal)x.consumedCement);
            double sumRemaining = 0;
            foreach (Mixer mixer in mixerList)
            {
                if (cementDailyRecords.Any(x => x.mixerID == mixer.mixerID))
                {
                    sumRemaining = (double)((decimal)sumRemaining + cementDailyRecords.Where(x => x.mixerID == mixer.mixerID).Select(x => (decimal)x.remaniningCement).FirstOrDefault());
                }
                else
                {
                    sumRemaining = (double)((decimal)sumRemaining + (decimal)mixer.currentCementLevel);
                }
            }
            double sumPreviouslyRemaining = (double)((decimal)sumRemaining + (decimal)sumConsumed - (decimal)sumImported);
            

            double epsilon = 1e-8;

            string sumImportedString = sumImported < epsilon && sumImported > -1 * epsilon ? "-" : sumImported.ToString();
            string sumConsumedString = sumConsumed < epsilon && sumConsumed > -1 * epsilon ? "-" : sumConsumed.ToString();
            string sumRemainingString = sumRemaining < epsilon && sumRemaining > -1 * epsilon ? "-" : sumRemaining.ToString();
            string sumPreviouslyRemainingString = sumPreviouslyRemaining < epsilon && sumPreviouslyRemaining > -1 * epsilon ? "-" : sumPreviouslyRemaining.ToString();

            return new List<string> { sumRemainingString, sumConsumedString, sumPreviouslyRemainingString, sumImportedString };
        }


        public static TableCell CreateCell(string text, double cellWidth, bool bold = false, string font = "Arial", string size = "24", bool center = true, string fillColor = "FFFFFF",string lineSpacing = "240")
        {
            text = replaceArabicToIndianDigits(text);
            // Create run properties (font, size, bold)
            var runProps = new RunProperties(       
                new RightToLeftText(),
                new RunFonts { Ascii = font, HighAnsi = font , ComplexScript = font },
                new FontSize { Val = size },
                new FontSizeComplexScript { Val = size },
                new Languages() { Bidi = "ar-SA" }
            );
            if (bold)
            {
                runProps.Append(new Bold());
                runProps.Append(new BoldComplexScript());
            }
                

            // Create paragraph properties (alignment)
            var paraProps = new ParagraphProperties();
            if (center)
                paraProps.Append(    
                    new Justification { Val = JustificationValues.Center });

            paraProps.Append(new BiDi());
            paraProps.Append(new SpacingBetweenLines
            {
                After = "0",
                Before = "0",
                AfterAutoSpacing = false,
                BeforeAutoSpacing= false,
                Line = lineSpacing,       
                LineRule = LineSpacingRuleValues.Auto
            });
            // Construct paragraph with run
            var paragraph = new Paragraph(
                paraProps
            );
            text = text ?? "";
            string[] texts = text.Split("\n");
            for (int i = 0; i < texts.Length; i++)
            {
                paragraph.Append(new Run(runProps.CloneNode(true),new Text(texts[i])));

                if (i < texts.Length - 1)
                    paragraph.Append(new Run(new Break())); // Add break between lines
            }


            // Construct and return the cell
            return new TableCell(
                new TableCellProperties(
                    new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center },
                    new TableCellWidth()
                    {
                        Type = TableWidthUnitValues.Dxa, // Twips
                        Width = $"{1440 * cellWidth}"
                    },
                    new Shading
                    {
                        Val = ShadingPatternValues.Clear,
                        Color = "auto",
                        Fill = fillColor
                    }
                ),
                paragraph
            );
        }

        public static void AddBorderToCell(TableCell cell, TableCellBorders borders)
        {
            // Ensure the cell has properties
            var tcp = cell.GetFirstChild<TableCellProperties>();
            if (tcp == null)
            {
                tcp = new TableCellProperties();
                cell.PrependChild(tcp);
            }

            // Add or replace the existing borders
            var existingBorders = tcp.GetFirstChild<TableCellBorders>();
            if (existingBorders != null)
                tcp.RemoveChild(existingBorders);

            tcp.Append(borders);
        }


        public static Table mergeTableCells(Table table)
        {
            var rows = table.Elements<TableRow>().ToList();
            int rowCount = rows.Count;

            // Build logical grid: each row is a list of (cell, text, span)
            var grid = new List<List<(TableCell cell, string text, int span)>>();
            foreach (var row in rows)
            {
                var line = new List<(TableCell, string, int)>();
                foreach (var cell in row.Elements<TableCell>())
                {
                    int span = cell.GetFirstChild<TableCellProperties>()?
                                  .GetFirstChild<GridSpan>()?.Val?.Value ?? 1;
                    string text = string.Concat(cell.Descendants<Text>().Select(t => t.Text)).Trim();
                    line.Add((cell, text, span));
                }
                grid.Add(line);
            }

            // Calculate max logical column index
            int colMax = grid.Max(row => row.Sum(c => c.span));
            int lastCol = colMax - 1;

            // For each logical column
            for (int col = 0; col < colMax; col++)
            {
                int r = 0;
                while (r < rowCount)
                {
                    var (cell0, text0, span0) = GetCellAtLogicalCol(grid[r], col);
                    var (last0, lastText0, lastSpan0) = GetCellAtLogicalCol(grid[r], lastCol);
                    if (cell0 == null || last0 == null) { r++; continue; }

                    int r2 = r + 1;
                    while (r2 < rowCount)
                    {
                        var (cellX, textX, spanX) = GetCellAtLogicalCol(grid[r2], col);
                        var (lastX, lastTextX, lastSpanX) = GetCellAtLogicalCol(grid[r2], lastCol);

                        if (cellX == null || lastX == null ||
                            textX != text0 || spanX != span0 ||
                            lastTextX != lastText0 || lastSpanX != lastSpan0)
                            break;

                        r2++;
                    }

                    if (r2 - r > 1)
                    {
                        for (int i = r; i < r2; i++)
                        {
                            var (cell, _, _) = GetCellAtLogicalCol(grid[i], col);
                            if (cell == null) continue;

                            var props = cell.GetFirstChild<TableCellProperties>()
                                        ?? cell.PrependChild(new TableCellProperties());

                            var existingMerge = props.GetFirstChild<VerticalMerge>();
                            if (existingMerge != null)
                                props.RemoveChild(existingMerge);

                            props.AppendChild(new VerticalMerge
                            {
                                Val = i == r ? MergedCellValues.Restart : MergedCellValues.Continue
                            });
                        }
                    }

                    r = r2;
                }
            }

            return table;

            // Helper: Get cell at logical column index
            static (TableCell, string, int) GetCellAtLogicalCol(
                List<(TableCell cell, string text, int span)> row, int target)
            {
                int pos = 0;
                foreach (var item in row)
                {
                    if (target >= pos && target < pos + item.span)
                        return item;
                    pos += item.span;
                }
                return (null, null, 0);
            }
        }



        public static Table centerTableCells(Table table)
        {
            foreach (var row in table.Elements<TableRow>())
            {
                foreach (var cell in row.Elements<TableCell>())
                {
                    foreach (var para in cell.Elements<Paragraph>())
                    {
                        // Set horizontal centering
                        para.ParagraphProperties ??= new ParagraphProperties();
                        para.ParagraphProperties.Justification = new Justification() { Val = JustificationValues.Center };
                    }

                    // Set vertical centering
                    cell.TableCellProperties ??= new TableCellProperties();
                    cell.TableCellProperties.TableCellVerticalAlignment =
                        new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
                }
            }
            return table;
        }

        public static Table createFuelTable(List<FuelConsumptionRecord> records,List<ConcreteProductionRecord>concreteRecords)
        {
            // Start table
            Table table = new Table();
            //table.AppendChild(tblProperties.CloneNode(true));

            // First Row
            TableRow row1 = new TableRow();
            List<string> items = new List<string> { "الخرسانة م3", "تاريخ أخر صرف", "المتبقى", "المستهلك اليوم", "السعة اللترية للخزانات", "المشروع / الوحدة", "م" };
            List<double> cellWidth = new List<double> { 1.18, 1.51, 0.76, 0.8, 0.83, 0.91, 1.43, 0.38 };

            ///////////// First Row Cell Borders
            List<TableCellBorders> cellBorders = new List<TableCellBorders> { 
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.None },
                    new LeftBorder   { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.None },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.None },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.None },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.None },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.ThinThickMediumGap, Size = 12 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.None },
                    new LeftBorder   { Val = BorderValues.ThinThickMediumGap, Size = 12 },
                    new RightBorder  { Val = BorderValues.ThinThickMediumGap, Size = 18 }
                    ),
            };
            //////////////////////////
            int j = 0;
            for (int i = 0; i < items.Count; i++)
            {

                if (items.ElementAt(i) == "تاريخ أخر صرف")
                {
                    var cell = CreateCell(items.ElementAt(i), cellWidth.ElementAt(i) + cellWidth.ElementAt(i + 1), bold: true, font: "PT Bold Heading", size:"24");
                    cell.TableCellProperties = new TableCellProperties(new GridSpan() { Val = 2 });
                    j++;
                    AddBorderToCell(cell,cellBorders.ElementAt(i));
                    row1.Append(cell);
                }
                else
                {
                    var cell = CreateCell(items.ElementAt(i), cellWidth.ElementAt(j), bold: true, font: "PT Bold Heading", size: "24");
                    cell.TableCellProperties = new TableCellProperties(new VerticalMerge { Val = MergedCellValues.Restart });
                    AddBorderToCell(cell, cellBorders.ElementAt(i));
                    row1.Append(cell);
                }
                j++;


            }
            table.Append(row1);

            // Second Row

            ///////////// Second Row Cell Borders
            cellBorders.Clear();
            cellBorders = new List<TableCellBorders> {
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.None},
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder   { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.None },
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.None },
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.None },
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.None },
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.ThinThickMediumGap, Size = 12 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.None },
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder   { Val = BorderValues.ThinThickMediumGap, Size = 12 },
                    new RightBorder  { Val = BorderValues.ThinThickMediumGap, Size = 18 }
                    ),
            };
            /////////////
            TableRow row2 = new TableRow();
            for (int i = 0; i < 8; i++)
            {
                var cell = new TableCell();
                if (i == 1)
                {
                    cell = CreateCell("التاريخ", cellWidth.ElementAt(i), bold: true, font: "PT Bold Heading", size: "24");
                }
                else if (i == 2)
                {
                    cell = CreateCell("الكمية", cellWidth.ElementAt(i), bold: true, font: "PT Bold Heading", size: "24");
                }
                else
                {
                    cell = CreateCell("", cellWidth.ElementAt(i), bold: true, font: "PT Bold Heading", size: "24");
                    cell.TableCellProperties = new TableCellProperties(new VerticalMerge());
                }
                AddBorderToCell(cell, cellBorders.ElementAt(i));
                row2.Append(cell);
            }
            table.Append(row2);


            ///////////// Rows Cell Borders
            cellBorders.Clear();
            cellBorders = new List<TableCellBorders> {
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4  },
                    new LeftBorder   { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.ThinThickMediumGap, Size = 12 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder   { Val = BorderValues.ThinThickMediumGap, Size = 12 },
                    new RightBorder  { Val = BorderValues.ThinThickMediumGap, Size = 18 }
                    ),
            };
            /////////////

            int rowCount = 1;
            /// Vals 
            List<FuelDepot> depots        = DepotService.fetchDepots();
            int depotCount                = depots.Count;
            double generalSumConcrete     = 0;
            int generalSumRemainingFuel   = 0;
            int generalSumConsumedFuel    = 0;
            int generalSumStorageCapacity = 0;
            int generalSumSelfSufficiency = 0;
            int generalSumBenzine80       = 0;
            int generalSumSummerDiesel    = 0;
            foreach (FuelDepot depot in depots)
            {
                var record = records.Where(x => x.depotID == depot.depotID).FirstOrDefault();
                double sumConcrete;
                using (var context = new ApplicationDbContext())
                {
                    sumConcrete = (double)concreteRecords.Join(context.mixers,
                        c => c.mixerID,
                        m => m.mixerID,
                        (c, m) => new
                        {
                            recordDate = c.recordDate,
                            depotID = m.depotID,
                            producedConcreteAmount = c.producedConcreteAmount
                        }).
                        Where(x => x.recordDate == DateTime.Today.Date && x.depotID == depot.depotID).Select(x => (decimal) x.producedConcreteAmount).Sum();
                    //MessageBox.Show($"{sumConcrete} {concreteRecords.Count}", "Wolf", MessageBoxButton.OK);
                }
                DateTime lastImportDate;
                string lastImportAmount;
                string consumedFuel;
                string remainingFuel;
                if (record != null)
                {
                    lastImportDate = record.importedAmount == 0 ? depot.LastConsignmentDate.Date : DateTime.Today.Date;
                    lastImportAmount = record.importedAmount == 0 ? depot.LastimportedFuelAmount.ToString("N0") : record.importedAmount.ToString("N0");
                    consumedFuel = record.consumedAmount == 0 ? "صفر" : record.consumedAmount.ToString("N0");
                    remainingFuel = record.fuelLevel == 0 ? "صفر" : record.fuelLevel.ToString("N0");
                    generalSumRemainingFuel += record.fuelLevel;

                }
                else
                {
                    lastImportDate = depot.LastConsignmentDate.Date;
                    lastImportAmount = depot.LastimportedFuelAmount.ToString("N0");
                    consumedFuel = "صفر";
                    remainingFuel = depot.currentReserve == 0 ? "صفر" : depot.currentReserve.ToString("N0");
                    generalSumRemainingFuel += depot.currentReserve;

                }
                double epsilon = 1e-8;
                bool shouldBeShaded = lastImportDate.Date == DateTime.Today.Date ? true : false;
                string lastImportDateString = $"{lastImportDate.Day.ToString("D2")}";
                lastImportDateString += $"";
                lastImportDateString += " ";
                lastImportDateString += "/";
                lastImportDateString += " ";
                lastImportDateString += $"{lastImportDate.Month.ToString("D2")}";
                lastImportDateString += " ";
                lastImportDateString += "/";
                lastImportDateString += " ";
                lastImportDateString += $"{lastImportDate.Year}";
                string sumConcreteString = sumConcrete < epsilon ? "-" : $"{sumConcrete.ToString("G29")}";
                if (sumConcreteString != "-")
                {
                    sumConcreteString += " ";
                    sumConcreteString += "م3 خرسانة";
                }              
                items = new List<string> { sumConcreteString, $"{lastImportDateString}", $"{lastImportAmount}", $"{remainingFuel}", $"{consumedFuel}", $"{depot.depotStorageCapacity.ToString("N0")}", $"{depot.depotName}", $"{rowCount}" };
                TableRow row = new TableRow();

                if (rowCount == 1)
                {
                    foreach(var cellBorder in cellBorders)
                    {
                        cellBorder.TopBorder = new TopBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 };
                    }
                }
                else if ( rowCount == depotCount)
                {
                    foreach (var cellBorder in cellBorders)
                    {                        
                        cellBorder.BottomBorder = new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 };
                    }
                }
                else
                {
                    foreach (var cellBorder in cellBorders)
                    {
                        cellBorder.TopBorder = new TopBorder { Val = BorderValues.Single, Size = 4 };
                    }
                }
                for (int i = 0; i < items.Count; i++)
                {
                    if (i == 0)
                    {
                        var cell = CreateCell(items.ElementAt(i), cellWidth.ElementAt(i), true, font: "PT Bold Heading" ,size: "20");
                        AddBorderToCell(cell, (TableCellBorders)cellBorders.ElementAt(i).CloneNode(true));
                        row.Append(cell);
                    }
                    else if(i == 1 || i == 2)
                    {
                        if (shouldBeShaded == true)
                        {
                            var cell = CreateCell(items.ElementAt(i), cellWidth.ElementAt(i), false,fillColor: "D9D9D9",size:"32");
                            AddBorderToCell(cell, (TableCellBorders)cellBorders.ElementAt(i).CloneNode(true));
                            row.Append(cell);
                        }
                        else
                        {
                            var cell = CreateCell(items.ElementAt(i), cellWidth.ElementAt(i), false, size: "32");
                            AddBorderToCell(cell, (TableCellBorders)cellBorders.ElementAt(i).CloneNode(true));
                            row.Append(cell);
                        }
                    }
                    else if (i == 6)
                    {
                        var cell = CreateCell(items.ElementAt(i), cellWidth.ElementAt(i), true, size: "32");
                        AddBorderToCell(cell, (TableCellBorders)cellBorders.ElementAt(i).CloneNode(true));
                        row.Append(cell);
                    }
                    else
                    {
                        var cell = CreateCell(items.ElementAt(i), cellWidth.ElementAt(i), false, size: "32");
                        AddBorderToCell(cell, (TableCellBorders)cellBorders.ElementAt(i).CloneNode(true));
                        row.Append(cell);
                    }
                    
                }
                row.AddChild(getTableRowProperties(0.47));
                table.Append(row);
                rowCount++;
                generalSumConcrete = (double)((decimal)generalSumConcrete + (decimal)sumConcrete);
                generalSumConsumedFuel += record == null ? 0 : record.consumedAmount;
                generalSumStorageCapacity += depot.depotStorageCapacity;
            }

            ///////////////////////

            cellBorders.Clear();
            cellBorders = new List<TableCellBorders> {
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18},
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder   { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.ThinThickMediumGap, Size = 18 }
                    )
            };
            /////////////

            // Third Row
            string generalSumConcreteString = generalSumConcrete == 0 ? "-" : generalSumConcrete.ToString("G29");
            if (generalSumConcreteString != "-")
            {
                generalSumConcreteString += " ";
                generalSumConcreteString += "م3 خرسانة";
            }           
            string generalSumRemainingFuelString = generalSumRemainingFuel == 0 ? "صفر" : generalSumRemainingFuel.ToString("N0");
            string generalSumConsumedFuelString = generalSumConsumedFuel == 0 ? "صفر" : generalSumConsumedFuel.ToString("N0");

            items = new List<string> { $"{generalSumConcreteString}", "-", "-", $"{generalSumRemainingFuelString}", $"{generalSumConsumedFuelString}", $"{generalSumStorageCapacity.ToString("N0")}" };
            TableRow row3 = new TableRow();
            for (int i = 0; i < items.Count; i++)
            {
                if (i == 0)
                {
                    var cell = CreateCell(items.ElementAt(i), cellWidth.ElementAt(i), true, font : "PT Bold Heading", size: "20");
                    AddBorderToCell(cell, cellBorders.ElementAt(i));
                    row3.Append(cell);
                }
                else
                {
                    var cell = CreateCell(items.ElementAt(i), cellWidth.ElementAt(i), false, size: "32");
                    AddBorderToCell(cell, cellBorders.ElementAt(i));
                    row3.Append(cell);
                }
                
            }
            var finalCell = CreateCell("الإجمالى", cellWidth.ElementAt(cellWidth.Count - 2) + cellWidth.ElementAt(cellWidth.Count - 1), true, size: "32");
            finalCell.TableCellProperties = new TableCellProperties(new GridSpan() { Val = 2 });
            AddBorderToCell(finalCell, cellBorders.Last());
            row3.Append(finalCell);
            row3.AddChild(getTableRowProperties(0.47));
            table.Append(row3);


            // Fourth Row

            cellBorders.Clear();
            cellBorders = new List<TableCellBorders> {
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18},
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder   { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.ThinThickMediumGap, Size = 18 }
                    )
            };
            /////////////

            items = new List<string> { $"-", $"سمر ديزل", $"بنزين 80", $"السولار", $"الإكتفاء الذاتى" };
            TableRow row4 = new TableRow();
            j = 0;
            for (int i = 0; i < items.Count; i++)
            {
                if (i != 1 && i != 2)
                {
                    var cell = CreateCell(items.ElementAt(i), cellWidth.ElementAt(j) + cellWidth.ElementAt(j + 1), true, size: "32");
                    cell.TableCellProperties = new TableCellProperties(new GridSpan() { Val = 2 });
                    AddBorderToCell(cell, cellBorders.ElementAt(i));
                    row4.Append(cell);
                    j++;
                }
                else
                {
                    var cell = CreateCell(items.ElementAt(i), cellWidth.ElementAt(j), true, size: "32");
                    AddBorderToCell(cell, cellBorders.ElementAt(i));
                    row4.Append(cell);
                }
                j++;

            }
            row4.AddChild(getTableRowProperties(0.47));
            table.Append(row4);

            //////////////////////////////////////
            cellBorders.Clear();
            cellBorders = new List<TableCellBorders> {
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.Single, Size = 4},
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder   { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.ThinThickMediumGap, Size = 12 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder   { Val = BorderValues.ThinThickMediumGap, Size = 12 },
                    new RightBorder  { Val = BorderValues.ThinThickMediumGap, Size = 18 }
                    )
            };
            /////////////

            List<Unit> units = UnitService.retrieveUnits();
            foreach (var unit in units)
            {
                j = 0;
                items = new List<string> { "-", $"{unit.summerDieselReserve.ToString("N0")}", $"{unit.benzine80Reserve.ToString("N0")}", $"{unit.selfSufficienyReserve.ToString("N0")}", $"{unit.unitDesignation.ElementAt(2)} {unit.unitCode}", $"{rowCount}" };
                TableRow row = new TableRow();
                for (int i = 0; i < items.Count; i++)
                {

                    if (i == 0 || i == 3)
                    {
                        var cell = CreateCell(items.ElementAt(i), cellWidth.ElementAt(j) + cellWidth.ElementAt(j + 1), true, size: "32");
                        cell.TableCellProperties = new TableCellProperties(new GridSpan() { Val = 2 });
                        j++;
                        AddBorderToCell(cell, (TableCellBorders)cellBorders.ElementAt(i).CloneNode(true));
                        row.Append(cell);
                    }
                    else
                    {
                        var cell = CreateCell(items.ElementAt(i), cellWidth.ElementAt(j), true, size: "32");
                        AddBorderToCell(cell, (TableCellBorders)cellBorders.ElementAt(i).CloneNode(true));
                        row.Append(cell);
                    }
                    j++;
                }
                row.AddChild(getTableRowProperties(0.47));
                table.Append(row);
                rowCount++;
                generalSumSelfSufficiency += unit.selfSufficienyReserve;
                generalSumBenzine80 += unit.benzine80Reserve;
                generalSumSummerDiesel += unit.summerDieselReserve;
            }
            ///


            // Fifth Row

            //////////////////////////////////////
            cellBorders.Clear();
            cellBorders = new List<TableCellBorders> {
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18},
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder   { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.ThinThickMediumGap, Size = 18 }
                    )
            };
            /////////////

            j = 0;
            items = new List<string> { $"-", $"{generalSumSummerDiesel.ToString("N0")}", $"{generalSumBenzine80.ToString("N0")}", $"{generalSumSelfSufficiency.ToString("N0")}", $"الإجمالى" };
            TableRow row5 = new TableRow();
            for (int i = 0; i < items.Count; i++)
            {
                if (i != 1 && i != 2)
                {
                    var cell = CreateCell(items.ElementAt(i), cellWidth.ElementAt(j) + cellWidth.ElementAt(j + 1), true, size: "32");
                    cell.TableCellProperties = new TableCellProperties(new GridSpan() { Val = 2 });
                    AddBorderToCell(cell, cellBorders.ElementAt(i));
                    row5.Append(cell);
                    j++;
                }
                else
                {
                    var cell = CreateCell(items.ElementAt(i), cellWidth.ElementAt(j), true, size: "32");
                    AddBorderToCell(cell, cellBorders.ElementAt(i));
                    row5.Append(cell);
                }
                j++;
            }
            row5.AddChild(getTableRowProperties(0.47));
            table.Append(row5);

            // Sixth Row
            TableRow row6 = new TableRow();
            var sixthRowCell = CreateCell("", cellWidth.Sum(), true, size: "32");
            TableCellBorders sixthRowCellBorders = new TableCellBorders(
                    new TopBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new RightBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 }
            );          
            sixthRowCell.TableCellProperties = new TableCellProperties(new GridSpan() { Val = 8 });
            AddBorderToCell(sixthRowCell, sixthRowCellBorders);
            row6.Append(sixthRowCell);
            row6.AddChild(getTableRowProperties(0.3));
            table.Append(row6);

            var additionalInto = File.ReadAllText(AdditionalDataViewModel.AdditionalDataPath);
            AdditionalInfo info = JsonConvert.DeserializeObject<AdditionalInfo>(additionalInto);


            // Seventh Row
            j = 0;
            //////////////////////////////////////
            cellBorders.Clear();
            cellBorders = new List<TableCellBorders> {
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18},
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder   { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.Single, Size = 4 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 4 },
                    new RightBorder  { Val = BorderValues.ThinThickMediumGap, Size = 12 }
                    ),
                new TableCellBorders (
                    new TopBorder    { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new BottomBorder { Val = BorderValues.ThinThickMediumGap, Size = 18 },
                    new LeftBorder   { Val = BorderValues.ThinThickMediumGap, Size = 12 },
                    new RightBorder  { Val = BorderValues.ThinThickMediumGap, Size = 18 }
                    )
            };
            /////////////
            int procurmentOfficeFuelAmount = FuelService.RetrieveProcurmenetOfficeFuelStorage();
            items = new List<string> { $"-", $"-", $"-", $"{procurmentOfficeFuelAmount.ToString("N0")}", $"مكتب الإتصال", $"{rowCount}" };
            TableRow row7 = new TableRow();
            for (int i = 0; i < items.Count; i++)
            {

                if (i == 0 || i == 3)
                {
                    var cell = CreateCell(items.ElementAt(i), cellWidth.ElementAt(j) + cellWidth.ElementAt(j + 1), true, size: "32");
                    cell.TableCellProperties = new TableCellProperties(new GridSpan() { Val = 2 });
                    AddBorderToCell(cell, cellBorders.ElementAt(i));
                    row7.Append(cell);
                    j++;
                }
                else
                {
                    var cell = CreateCell(items.ElementAt(i), cellWidth.ElementAt(j), true, size: "32");
                    AddBorderToCell(cell, cellBorders.ElementAt(i));
                    row7.Append(cell);
                }
                j++;
            }
            row7.AddChild(getTableRowProperties(0.47));
            table.Append(row7);

            ///////////////////////////////// Centering
            table = centerTableCells(table);

            ////////////////////////////////

            return table;
        }

        public static Table createColonelConcreteTable(List<ConcreteProductionRecord> concreteRecords)
        {
            // Start table
            Table table    = new Table();
            double epsilon = 1e-8;

            table.AppendChild(tblProperties.CloneNode(true));

            // Helper: Create a formatted cell
            List<string> tableItems = new List<string> { "ملاحظات", "الجهة المستفيدة", "العنصر", "الكمية المنتجة م3", "مج الكمية المنتجة م3", "منطقة تمركز الخلاطة", "الوحدة / الوحدة الفرعية", "م" };
            List<double> cellWidth = new List<double> { 0.59, 0.98, 0.98, 1.57, 1.2, 1.12, 1.28, 0.26 };
            //// Row 1
            TableRow HeaderRow = new TableRow();
            HeaderRow.AddChild(getTableRowProperties(0.6));
            for (int i = 0; i < tableItems.Count; i++)
            {
                var cell = CreateCell(tableItems.ElementAt(i), cellWidth.ElementAt(i), true, font: "PT Bold Heading", size: "20", fillColor: "D9D9D9");
                HeaderRow.Append(cell);
            }
            table.Append(HeaderRow);

            int rowCounter = 1;
            ////////////////////////////////
            List<int> mixerIDs = concreteRecords.Select(x => x.mixerID).Distinct().ToList();
            List<int> sortedMixerIDs = new List<int>();
            List<Unit> units = new List<Unit>();
            foreach (int mixerID in mixerIDs)
            {
                Unit unit = MixerService.getOperatingUnit(mixerID);
                units.Add(unit);

            }
            var unitMixerList = mixerIDs.Zip(units, (mixerID, unit) => new { mixerID = mixerID, unit = unit }).ToList();
            unitMixerList = unitMixerList.OrderBy(x => x.unit.unitCode).ToList();
            string temp = "";
            foreach (var entry in unitMixerList)
            {
                double sumConcrete =(double) concreteRecords.Where(x => x.mixerID == entry.mixerID && x.isInformal == false).Select(x => (decimal)x.producedConcreteAmount).Sum();
                Mixer mixer = MixerService.getMixer(entry.mixerID);
                string unitName = $"{entry.unit.unitDesignation} {entry.unit.unitCode} {entry.unit.unitSpecialization}";
                List<ConcreteProductionRecord> filteredRecords = concreteRecords.Where(x => x.mixerID == entry.mixerID).ToList();
                foreach (var record in filteredRecords)
                {
                    TableRow entryRow = new TableRow();
                    entryRow.AddChild(getTableRowProperties());
                    entryRow.Append(CreateCell("", cellWidth.ElementAt(0), true));
                    temp += $"{record.producedConcreteAmount.ToString("G29")}";
                    temp += " ";
                    temp += "م3";
                    temp += " ";
                    if (record.isReinforcedConcrete)
                    {
                        temp += "خرسانة مسلحة";
                    }
                    else
                    {
                        temp += "خرسانة عادية";
                    }
                    if (record.isInformal)
                    {
                        entryRow.Append(CreateCell($"{record.company}", cellWidth.ElementAt(1), true, font: "PT Bold Heading", size: "20", fillColor: "D9D9D9",lineSpacing : "170"));
                        entryRow.Append(CreateCell($"{record.project}", cellWidth.ElementAt(2), true, font: "PT Bold Heading", size: "20", fillColor: "D9D9D9", lineSpacing: "170"));                       
                        entryRow.Append(CreateCell(temp, cellWidth.ElementAt(3), true, font: "PT Bold Heading", size: "20", fillColor: "D9D9D9", lineSpacing: "170"));
                    }
                    else
                    {
                        entryRow.Append(CreateCell($"{record.company}", cellWidth.ElementAt(1), true, font: "PT Bold Heading", size: "20", lineSpacing: "170"));
                        entryRow.Append(CreateCell($"{record.project}", cellWidth.ElementAt(2), true, font: "PT Bold Heading", size: "20", lineSpacing: "170"));
                        entryRow.Append(CreateCell(temp, cellWidth.ElementAt(3), true, font: "PT Bold Heading", size: "20", lineSpacing: "170"));
                    }
                    if (sumConcrete == 0)
                    {
                        temp = "-";
                    }
                    else
                    {
                        temp = "";
                        temp += $"{sumConcrete.ToString("G29")}";
                        temp += " ";
                        temp += "م3 خرسانة";
                    }                   
                    entryRow.Append(CreateCell(temp, cellWidth.ElementAt(4), true, font: "PT Bold Heading", size: "20", lineSpacing: "170"));
                    entryRow.Append(CreateCell($"{mixer.mixerName}", cellWidth.ElementAt(5), true, font: "PT Bold Heading", size: "20", lineSpacing: "170"));
                    entryRow.Append(CreateCell($"{unitName}", cellWidth.ElementAt(6), true, font: "PT Bold Heading", size: "20", lineSpacing: "170"));
                    entryRow.Append(CreateCell($"{rowCounter}", cellWidth.ElementAt(7), true, font: "PT Bold Heading", size: "20", fillColor: "D9D9D9", lineSpacing: "170"));
                    entryRow.AddChild(getTableRowProperties(0.45));
                    table.Append(entryRow); 
                    temp = "";
                }
                rowCounter++;
            }
            temp = "";
            double totalSumConcrete = (double)concreteRecords.Where(x=>x.isInformal == false).Select(x => (decimal) x.producedConcreteAmount).Sum();
            double sumReinforcedConcrete = (double) concreteRecords.Where(x => x.isReinforcedConcrete == true && x.isInformal == false).Select(x => (decimal) x.producedConcreteAmount).Sum();
            double sumNormalConcrete = (double)concreteRecords.Where(x => x.isReinforcedConcrete == false && x.isInformal == false).Select(x => (decimal) x.producedConcreteAmount).Sum();
            string sumConcreteString = totalSumConcrete < epsilon ? "-" : $"{totalSumConcrete.ToString("G29")}";
            string concreteSummationInDetail = "";
            if (sumReinforcedConcrete > epsilon)
            {
                temp = $"{sumReinforcedConcrete.ToString("G29")}";
                temp += " ";
                temp += "م3";
                temp += " ";
                temp += "خرسانة مسلحة";
                concreteSummationInDetail += temp;
            }
            temp = "";
            if (sumNormalConcrete > epsilon)
            {
                string temp2 = "";
                temp2 = $"{sumNormalConcrete.ToString("G29")}";
                temp2 += " ";
                temp2 += "م3";
                temp2 += " ";
                temp2 += "خرسانة عادية";
                if (concreteSummationInDetail.Length > 0)

                {
                    temp += "\n";
                    temp += temp2;
                    concreteSummationInDetail += temp;
                }
                else
                {
                    temp += temp2;
                    concreteSummationInDetail += temp;
                }
            }
            if (concreteSummationInDetail.Length == 0)
            {
                concreteSummationInDetail = "-";
            }
            if (sumConcreteString == "-")
            {
                temp = "-";
            }
            else
            {
                temp = "";
                temp += $"{sumConcreteString}";
                temp += " ";
                temp += "م3 خرسانة";
            }          
            tableItems = new List<string> { "-", "-", "-", $"{concreteSummationInDetail}", $"{temp}", "-", "الإجمالى" };
            TableRow finalRow = new TableRow();
            finalRow.AddChild(getTableRowProperties(0.65));
            for (int i = 0; i < tableItems.Count; i++)
            {
                var cell = CreateCell($"{tableItems.ElementAt(i)}", cellWidth.ElementAt(i), true, font: "PT Bold Heading", size: "20", fillColor: "D9D9D9", lineSpacing: "170");
                if (i == tableItems.Count - 1)
                {
                    cell.TableCellProperties = new TableCellProperties(new GridSpan() { Val = 2 },
                        new Shading
                        {
                            Val = ShadingPatternValues.Clear,
                            Color = "auto",
                            Fill = "D9D9D9"
                        });
                }
                finalRow.Append(cell);
            }
            table.Append(finalRow);
            //////////////////////////////////

            table = mergeTableCells(table);

            ///////////////////////////////// Centering

            table = centerTableCells(table);
            //////////////////////////////////////

            return table;
        }
        public static List<Table> createWallTables(List<PreCastWallProgressRecord> wallRecords)
        {
            Table firstWallTable = new Table();
            Table secondWallTable = new Table();

            List<double> cellWidth = new List<double> { 3.11, 1.77, 1.31, 1.57, 1.35, 1.67, 1.48 };
            firstWallTable.AppendChild(tblProperties.CloneNode(true));
            secondWallTable.AppendChild(tblProperties.CloneNode(true));

            List<string> tableItems = new List<string> { "المتبقى", "اجمالى ما تم تنفيذه", "المنفذ اليوم", "السابق تنفيذه", "المخطط تنفيذه", "الكتيبة" };

            //// Row 1
            TableRow HeaderRow1 = new TableRow();
            HeaderRow1.AddChild(getTableRowProperties(0.5));
            for (int i = 0; i < tableItems.Count; i++)
            {
                var cell = CreateCell(tableItems.ElementAt(i), cellWidth.ElementAt(i), true, "Arial", "24");
                HeaderRow1.Append(cell);
            }
            firstWallTable.Append(HeaderRow1);


            tableItems = new List<string> { "ملاحظات", "المتبقى بالموقع", "اجمالى ما تم نقله", "ما تم نقله اليوم", "السابق نقله لمستودع المهمات", "الكتيبة" };
            //// Row 1
            TableRow HeaderRow2 = new TableRow();
            HeaderRow2.AddChild(getTableRowProperties(0.5));
            for (int i = 0; i < tableItems.Count; i++)
            {
                var cell = CreateCell(tableItems.ElementAt(i), cellWidth.ElementAt(i), true, "Arial", "24");
                HeaderRow2.Append(cell);
            }
            secondWallTable.Append(HeaderRow2);


            int columnCounter = 0;
            foreach (var record in wallRecords)
            {
                columnCounter = 0;
                Unit unit = UnitService.fetchUnit(record.unitID);
                string unitName = $"{unit.unitDesignation} {unit.unitCode}";
                TableRow row1 = new TableRow();
                TableRow row2 = new TableRow();
                row1.AddChild(getTableRowProperties(0.5));
                row2.AddChild(getTableRowProperties(0.5));
                string toBeAccomplished;
                if (record.toBeAccomplished == 0) {
                    toBeAccomplished = "-"; 
                }
                else
                {
                    toBeAccomplished = $"{record.toBeAccomplished}";
                    toBeAccomplished += " ";
                    toBeAccomplished += "متر طولى";
                }
                string totalAccomplished;
                if (unit.preCastWallTarget - record.toBeAccomplished == 0)
                {
                    totalAccomplished = "-";
                }
                else
                {
                    totalAccomplished = $"{unit.preCastWallTarget - record.toBeAccomplished}";
                    totalAccomplished += " ";
                    totalAccomplished += "متر طولى";
                }
                string accomplishedToday;
                if (record.accomplishedToday == 0)
                {
                    accomplishedToday = "-";
                }
                else
                {
                    accomplishedToday = $"{record.accomplishedToday}";
                    accomplishedToday += " ";
                    accomplishedToday += "متر طولى";
                }
                string previouslyAccomplished;
                if (record.previouslyAccomplished == 0)
                {
                    previouslyAccomplished = "-";
                }
                else
                {
                    previouslyAccomplished = $"{record.previouslyAccomplished}";
                    previouslyAccomplished += " ";
                    previouslyAccomplished += "متر طولى";
                }
                string target = $"{unit.preCastWallTarget}";
                target += " ";
                target += "متر طولى";
                string remainingOnSite;
                if (record.remaningOnSite == 0)
                {
                    remainingOnSite = "-";
                }
                else
                {
                    remainingOnSite = $"{record.remaningOnSite}";
                    remainingOnSite += " ";
                    remainingOnSite += "متر طولى";
                }
                string totalTransported;
                if (record.transportedAmountToday + record.previouslyTransported == 0)
                {
                    totalTransported = "-";
                }
                else
                {
                    totalTransported = $"{record.transportedAmountToday + record.previouslyTransported}";
                    totalTransported += " ";
                    totalTransported += "متر طولى";
                }
                string transportedToday;
                if (record.transportedAmountToday == 0)
                {
                    transportedToday = "-";
                }
                else
                {
                    transportedToday = $"{record.transportedAmountToday}";
                    transportedToday += " ";
                    transportedToday += "متر طولى";
                }
                string previouslyTransported;
                if (record.previouslyTransported == 0)
                {
                    previouslyTransported = "-";
                }
                else
                {
                    previouslyTransported = $"{record.previouslyTransported}";
                    previouslyTransported += " ";
                    previouslyTransported += "متر طولى";
                }


                row1.Append(CreateCell($"{toBeAccomplished}", cellWidth.ElementAt(columnCounter), true));
                columnCounter++;
                row1.Append(CreateCell($"{totalAccomplished}", cellWidth.ElementAt(columnCounter), true));
                columnCounter++;
                row1.Append(CreateCell($"{accomplishedToday}", cellWidth.ElementAt(columnCounter), true));
                columnCounter++;
                row1.Append(CreateCell($"{previouslyAccomplished}", cellWidth.ElementAt(columnCounter), true));
                columnCounter++;
                row1.Append(CreateCell($"{target}", cellWidth.ElementAt(columnCounter), true));
                columnCounter++;
                row1.Append(CreateCell($"{unitName}", cellWidth.ElementAt(columnCounter), true));

                columnCounter = 0;

                row2.Append(CreateCell($"-", cellWidth.ElementAt(columnCounter), true));
                columnCounter++;
                row2.Append(CreateCell($"{remainingOnSite}", cellWidth.ElementAt(columnCounter), true));
                columnCounter++;
                row2.Append(CreateCell($"{totalTransported}", cellWidth.ElementAt(columnCounter), true));
                columnCounter++;
                row2.Append(CreateCell($"{transportedToday}", cellWidth.ElementAt(columnCounter), true));
                columnCounter++;
                row2.Append(CreateCell($"{previouslyTransported}", cellWidth.ElementAt(columnCounter), true));
                columnCounter++;
                row2.Append(CreateCell($"{unitName}", cellWidth.ElementAt(columnCounter), true));
                columnCounter++;

                firstWallTable.Append(row1);
                secondWallTable.Append(row2);
            }
            return new List<Table> {firstWallTable,secondWallTable };
        }

        public static Table createHQConcreteTable(List<List<string>> rowEntries,List <string> finalRowItems ,bool isFinal)
        {
            // Start table
            Table table = new Table();
            table.AppendChild(tblProperties.CloneNode(true));

            // Helper: Create a formatted cell           
            List<string> tableItems = new List<string> { "كميات الأسمنت م3", "الجهه المستفيدة", "العنصر الذى تم صبه", "المشروع / الإستخدام", "الكمية المنتجة م3", "الطاقة الإنتاجية م3 / س", "الوحدة ", "الصلاحية الفنية", "نوع الخلاطة","اسم الخلاطة / منطقة تمركز الخلاطة", "م" };
            List<double> cellWidth = new List<double> { 0.67, 0.67, 0.67, 0.67, 0.89, 1.18, 1.48, 1.48, 0.52, 0.8, 0.56 , 0.56, 0.75, 0.90, 0.29 };
            TableRow row1 = new TableRow();
            TableRow row2 = new TableRow();
            row1.AddChild(getTableRowProperties(0.5));
            row2.AddChild(getTableRowProperties(0.5));
            int j = 0;
            for (int i = 0; i < tableItems.Count; i++)
            {
                if (i == 0)
                {
                    var cell = CreateCell(tableItems.ElementAt(i), cellWidth.ElementAt(j) + cellWidth.ElementAt(j + 1) + cellWidth.ElementAt(j + 2) + cellWidth.ElementAt(j + 3), true, "PT Bold Heading", "20");
                    cell.TableCellProperties = new TableCellProperties(new GridSpan() { Val = 4 });
                    row1.Append(cell);
                    j += 3;
                }
                else if (i == 7)
                {
                    var cell = CreateCell(tableItems.ElementAt(i), cellWidth.ElementAt(j) + cellWidth.ElementAt(j + 1), true, "PT Bold Heading", "20");
                    cell.TableCellProperties = new TableCellProperties(new GridSpan() { Val = 2 });
                    row1.Append(cell);
                    j++;
                }
                else
                {
                    row1.Append(CreateCell(tableItems.ElementAt(i), cellWidth.ElementAt(j), true, "PT Bold Heading", "20"));
                }
                j++;             
            }
            tableItems.Clear();
            tableItems = new List<string> { "باقى", "المستهلك اليوم", "الباقى الأمس", "توريد", "الجهه المستفيدة", "العنصر الذى تم صبه", "المشروع / الإستخدام", "الكمية المنتجة م3", "الطاقة الإنتاجية م3 / س", "الوحدة ", "عاطل", "صالح", "نوع الخلاطة", "اسم الخلاطة / منطقة تمركز الخلاطة", "م" };
            for (int i = 0; i < tableItems.Count; i++)
            {
                row2.Append(CreateCell(tableItems.ElementAt(i), cellWidth.ElementAt(i), true, "PT Bold Heading", "18"));                
            }

            table.Append(row1);
            table.Append(row2);

            foreach (var entry in rowEntries)
            {
                TableRow row = new TableRow();
                row.AddChild(getTableRowProperties(0.35));
                int entryCount = entry.Count;
                for (int i = 0; i <entryCount; i++)
                {
                    row.Append(CreateCell(entry.ElementAt(i), cellWidth.ElementAt(i), true, "PT Bold Heading", "20", lineSpacing: "170"));
                }
                row.AddChild(getTableRowProperties(0.45));
                table.Append(row);
            }
            if (isFinal == true)
            {
                TableRow row = new TableRow();
                row.AddChild(getTableRowProperties(0.55));
                j = 0;
                int itemCount = finalRowItems.Count;
                for (int i = 0; i< itemCount; i++)
                {
                    if (i < itemCount - 1)
                    {
                        row.Append(CreateCell(finalRowItems.ElementAt(i), cellWidth.ElementAt(j),true, "PT Bold Heading", "19", lineSpacing: "170"));
                    }
                    else
                    {
                        var cell = CreateCell(finalRowItems.ElementAt(i), cellWidth.ElementAt(j) + cellWidth.ElementAt(j + 1), true, "PT Bold Heading", "20", lineSpacing: "170");
                        cell.TableCellProperties = new TableCellProperties(new GridSpan() { Val = 2 });
                        row.Append(cell);
                        j++;
                    }
                    j++;
                }
                table.Append(row);
            }

            //////////////////////////////////

            table = mergeTableCells(table);

            ///////////////////////////////// Centering

            table = centerTableCells(table);
            //////////////////////////////////////

            return table;
        }
        public static Table fuelReportSignatureTable()
        {
            Table table = new Table();
            TableProperties tableProps = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = BorderValues.None },
                    new BottomBorder { Val = BorderValues.None },
                    new LeftBorder { Val = BorderValues.None },
                    new RightBorder { Val = BorderValues.None },
                    new InsideHorizontalBorder { Val = BorderValues.None },
                    new InsideVerticalBorder { Val = BorderValues.None }
                )
            );

            TableRow row = new TableRow();
            var additionalRecordsJsonString = File.ReadAllText(AdditionalDataViewModel.AdditionalDataPath);
            AdditionalInfo infoData = JsonConvert.DeserializeObject<AdditionalInfo>(additionalRecordsJsonString);
            string signatureName = "";
            string SignatureFirstLine = "";
            SignatureFirstLine += "التوقيع";
            SignatureFirstLine += " ";
            SignatureFirstLine += "(                       )";
            signatureName += $"{infoData.automotivesOfficerRank}";
            signatureName += " ";
            signatureName += "/";
            signatureName += " ";
            signatureName += $"{infoData.automotivesOfficerName}";
            TableCell cell1 = new TableCell(new Paragraph(
                    new ParagraphProperties(
                        new Justification { Val = JustificationValues.Right }
                    ),
                     new Run(
                         new RunProperties( new RunFonts() { ComplexScript = "PT Bold Heading" },
                                            new FontSizeComplexScript { Val = "22" },
                                            new Languages() { Bidi = "ar-SA" },
                                            new Bold(),
                                            new BoldComplexScript(),
                                            new RightToLeftText()),
                         new Text(SignatureFirstLine),
                         new Break(),
                         new Text(signatureName),
                         new Break(),
                         new Text("رئيس شئون فنية معدات")                  
                         )));
            cell1.AppendChild(new TableCellProperties(
                new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center },
                new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = $"{3*1440}" }
            ));
            signatureName = "";
            signatureName += $"{infoData.administrativeAffairsOfficerRank}";
            signatureName += " ";
            signatureName += "/";
            signatureName += " ";
            signatureName += $"{infoData.administrativeAffairsOfficerName}";
            TableCell cell2 = new TableCell(new Paragraph(
                    new ParagraphProperties(
                        new Justification { Val = JustificationValues.Right }
                    ),
                     new Run(
                         new RunProperties(
                                            new RunFonts { Ascii = "PT Bold Heading", HighAnsi = "PT Bold Heading", ComplexScript = "PT Bold Heading" },
                                            new FontSize { Val = "22" },
                                            new FontSizeComplexScript { Val = "22" },
                                            new Languages() { Bidi = "ar-SA" },
                                            new Bold(),
                                            new BoldComplexScript(),
                                            new RightToLeftText()),                                 
                         new Text(SignatureFirstLine),
                         new Break(),
                         new Text(signatureName),
                         new Break(),
                         new Text("رئيس شئون إدارية")
                         )));
            cell2.AppendChild(new TableCellProperties(
                new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center },
                new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = $"{5.5*1440}" }
            ));
            row.Append(cell1);
            row.Append(cell2);
            table.Append(row);

            return table;

        }
        public static Table BarricadeReportTable()
        {
            Table table = new Table();
            table.AppendChild(tblProperties.CloneNode(true));
            TableRow row1 = new TableRow();
            TableRow row2 = new TableRow();
            TableRow row3 = new TableRow();
            TableRow row4 = new TableRow();
            row1.AddChild(getTableRowProperties(0.5));
            row2.AddChild(getTableRowProperties(0.5));
            row3.AddChild(getTableRowProperties(0.8));
            row4.AddChild(getTableRowProperties(0.8));
            List<string> tableItems = new List<string> { "الجهة المستفيدة", "إجمالي الموانع", "عدد الموانع المنتجة", "الكمية المنتجة م3", "نوع المانع", "الوحــدة", "الصلاحية الفنية", "نوع الخلاطة", "اسم الخلاطة / منطقة التمركز"};
            List<double> cellWidth    = new List<double> { 1.91, 1.28,1.09 ,1.67 ,1.1 ,0.69 ,0.45 ,0.45 ,0.65 ,0.87 };
            int columnCounter = 0;
            int columnCount   = tableItems.Count;
            for (int i = 0; i< columnCount; i++)
            {
                if (i != 6)
                {
                    row1.Append(CreateCell(tableItems.ElementAt(i), cellWidth.ElementAt(columnCounter), true, "Arial", "22"));
                }
                else
                {
                    var cell = CreateCell(tableItems.ElementAt(i), cellWidth.ElementAt(columnCounter) + cellWidth.ElementAt(columnCounter + 1), true, "Arial", "20");
                    cell.TableCellProperties = new TableCellProperties(new GridSpan() { Val = 2 });
                    row1.Append(cell);
                    columnCounter++;
                }
                columnCounter++;
            }
            tableItems = new List<string> { "الجهة المستفيدة", "إجمالي الموانع", "عدد الموانع المنتجة", "الكمية المنتجة م3", "نوع المانع", "الوحــدة", "عاطل", "صالح", "نوع الخلاطة", "اسم الخلاطة / منطقة التمركز" };
            for (int i = 0; i < tableItems.Count; i++)
            {
                row2.Append(CreateCell(tableItems.ElementAt(i), cellWidth.ElementAt(i), true, "Arial", "20"));
            }
            tableItems = new List<string> { "الخلاطة", "470", "-", "-", "موزة", "ك 32 إنش", "-", "√", "كاباج", "‫خلاطة NA‬" };
            for (int i = 0; i < tableItems.Count; i++)
            {
                row3.Append(CreateCell(tableItems.ElementAt(i), cellWidth.ElementAt(i), true, "Arial", "20"));
            }
            tableItems[1] = "20";
            tableItems[4] = "مكعبات خرسانية";
            for (int i = 0; i < tableItems.Count; i++)
            {
                row4.Append(CreateCell(tableItems.ElementAt(i), cellWidth.ElementAt(i), true, "Arial", "20"));
            }
            table.Append(row1);
            table.Append(row2);
            table.Append(row3);
            table.Append(row4);
            table = mergeTableCells(table);
            table = centerTableCells(table);
            return table;
        }

    }
}
