using Microsoft.EntityFrameworkCore;
using OxyPlot.Axes;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1;
using WpfApp2.Models;
using WpfApp2.Models.Items;
using Newtonsoft.Json;
using WpfApp2.Views.Resources;
using System.IO;
using WpfApp2.ViewModels.Resources;
using System.Collections.ObjectModel;
using WpfApp2.ViewModels.Fuel;

namespace WpfApp2.Services
{
    public class FuelService
    {
        public static List<FetchFuelRecord> records;
        public static void AddFuelRecords(List<FuelRecord> fuelRecords)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    int distinctDepotcount = fuelRecords.Select(x=>x.DepotName).Distinct().ToList().Count;
                    int fuelRecordsCount   = fuelRecords.Count;
                    if (distinctDepotcount < fuelRecordsCount)
                    {
                        throw new Exception("لا يمكنك إدخال تمام لنفس مستودع الوقود مرتين");
                    }
                    else
                    {
                        foreach (var fuelRecord in fuelRecords)
                        {
                            int consumedAmountInt ;
                            int importedAmountInt ;
                            int remainingAmountInt;
                            bool consumedAmountIsInt = int.TryParse(fuelRecord.consumedFuel,out consumedAmountInt);
                            bool importedAmountIsInt = int.TryParse(fuelRecord.importedFuel, out importedAmountInt);
                            bool remainingAmountIsInt = int.TryParse(fuelRecord.remainingFuel, out remainingAmountInt);
                            if (consumedAmountIsInt == false || importedAmountIsInt == false || remainingAmountIsInt == false)
                            {
                                throw new Exception($"{fuelRecords.IndexOf(fuelRecord) + 1} تحقق من الأرقام المدخلة فالمدخل رقم");
                            }
                            if (consumedAmountInt < 0 || importedAmountInt < 0 || remainingAmountInt < 0)
                            {
                                throw new Exception($"{fuelRecords.IndexOf(fuelRecord) + 1} تحقق من الأرقام المدخلة فالمدخل رقم");
                            }
                            var record = new FuelConsumptionRecord
                            {
                                recordDate     = DateTime.Now.Date,
                                depotID        = fuelRecord.depotID ,
                                consumedAmount = consumedAmountInt,
                                importedAmount = importedAmountInt,
                                fuelLevel      = remainingAmountInt
                            };
                            context.fuelConsumptionRecords.Add(record);
                            var depot = context.depots.Where(x => x.depotID == fuelRecord.depotID).FirstOrDefault();
                            depot.currentReserve = remainingAmountInt;
                            if (importedAmountInt > 0)
                            {
                                depot.LastimportedFuelAmount = importedAmountInt;
                                depot.LastConsignmentDate    = DateTime.Today.Date;
                            }
                        }
                        context.SaveChanges();
                    }
                }
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public static bool ValidateFuelRecords(List<FuelRecord> fuelRecords)
        {
            try
            {
                int distinctDepotcount = fuelRecords.Select(x => x.DepotName).Distinct().ToList().Count;
                int fuelRecordsCount = fuelRecords.Count;
                if (distinctDepotcount < fuelRecordsCount)
                {
                    return false;
                }
                else
                {
                    foreach (var fuelRecord in fuelRecords)
                    {
                        int consumedAmountInt;
                        int importedAmountInt;
                        int remainingAmountInt;
                        bool consumedAmountIsInt = int.TryParse(fuelRecord.consumedFuel, out consumedAmountInt);
                        bool importedAmountIsInt = int.TryParse(fuelRecord.importedFuel, out importedAmountInt);
                        bool remainingAmountIsInt = int.TryParse(fuelRecord.remainingFuel, out remainingAmountInt);
                        if (consumedAmountIsInt == false || importedAmountIsInt == false || remainingAmountIsInt == false)
                        {
                            return false;
                        }
                        if (consumedAmountInt < 0 || importedAmountInt < 0 || remainingAmountInt < 0)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public static void initializeFuelRecords()
        {
            using (var context = new ApplicationDbContext())
            {
                records = context.fuelConsumptionRecords.Join(context.depots,
                        f => f.depotID,
                        d => d.depotID,
                        (f, d) => new
                        {
                            depotName = d.depotName,
                            recordID = f.recordID,
                            recordDate = f.recordDate,
                            consumedAmount = f.consumedAmount,
                            importedAmount = f.importedAmount,
                            fuelLevel = f.fuelLevel
                        }).Select(g => new FetchFuelRecord
                        {
                            depotname = g.depotName,
                            recordDate = g.recordDate,
                            consumedAmount = g.consumedAmount,
                            importedAmount = g.importedAmount,
                            remainingAmount = g.fuelLevel
                        }
                        ).OrderByDescending(x => x.recordDate).ToList();
            }
        }
        public static List<FetchFuelRecord> fetchfuelRecords(string depotName, DateTime startDate, DateTime endDate)
        {
            if (depotName == "-")
            {
                return records.Where(x=>x.recordDate<= endDate && x.recordDate>= startDate).ToList();
            }
            else
            {
                return records.Where(x => x.depotname == depotName && x.recordDate <= endDate && x.recordDate >= startDate).ToList();

            }
        }


        public static List<List<DataPoint>> retrieveViewPoints(string depotName, DateTime startDate, DateTime endDate)
        {
            List<List<DataPoint>> dataPoints = new List<List<DataPoint>>();
            if (startDate > endDate)
            {
                dataPoints.Add(new List<DataPoint>());
                dataPoints.Add(new List<DataPoint>());
                dataPoints.Add(new List<DataPoint>());
                return dataPoints;
            }
            List<FetchFuelRecord> filteredRecords = records.Where(x => x.recordDate <= endDate && x.recordDate >= startDate && x.depotname == depotName).ToList();
            List<DataPoint> importedDataPoints = new List<DataPoint>();
            List<DataPoint> consumedDataPoints = new List<DataPoint>();
            List<DataPoint> remainingDataPoints = new List<DataPoint>();

            if (DateTime.Today.Date < endDate)
            {
                endDate = DateTime.Today.Date;
            }

            int dayCount = (endDate - startDate).Days;
            for (int i = 0; i <= dayCount; i++)
            {
                var record = filteredRecords.Where(x => x.recordDate == startDate.AddDays(i)).FirstOrDefault();
                if (record == null)
                {
                    var earliestRecord = records.Where(x => x.recordDate < startDate.AddDays(i) && x.depotname == depotName).OrderByDescending(r => r.recordDate).FirstOrDefault();
                    if (earliestRecord == null)
                    {
                        remainingDataPoints.Add(new DataPoint(DateTimeAxis.ToDouble(startDate.AddDays(i)), 0));
                    }
                    else
                    {
                        remainingDataPoints.Add(new DataPoint(DateTimeAxis.ToDouble(startDate.AddDays(i)), earliestRecord.remainingAmount));
                    }
                    importedDataPoints.Add(new DataPoint(DateTimeAxis.ToDouble(startDate.AddDays(i)), 0));
                    consumedDataPoints.Add(new DataPoint(DateTimeAxis.ToDouble(startDate.AddDays(i)), 0));
                }
                else
                {
                    importedDataPoints.Add(new DataPoint(DateTimeAxis.ToDouble(record.recordDate), record.importedAmount));
                    consumedDataPoints.Add(new DataPoint(DateTimeAxis.ToDouble(record.recordDate), record.consumedAmount));
                    remainingDataPoints.Add(new DataPoint(DateTimeAxis.ToDouble(record.recordDate), record.remainingAmount));
                }

            }

            dataPoints.Add(importedDataPoints);
            dataPoints.Add(consumedDataPoints);
            dataPoints.Add(remainingDataPoints);
            return dataPoints;
        }
        public static bool canAddRecords()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    bool checkValue = context.fuelConsumptionRecords.Where(x => x.recordDate.Date == DateTime.Today.Date).Any();
                    return !checkValue;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public static bool checkRecordExistence()
        {
            using (var context = new ApplicationDbContext())
            {
                bool checkValue = context.fuelConsumptionRecords.Where(x => x.recordDate.Date == DateTime.Today.Date).Any();
                return checkValue;
            }
        }

        public static List<FuelConsumptionRecord> retrieveFuelConsumptionRecords()
        {
            using (var context = new ApplicationDbContext())
            {
                List<FuelConsumptionRecord> consumptionRecords = context.fuelConsumptionRecords.Where(x => x.recordDate.Date == DateTime.Today.Date).ToList();
                return consumptionRecords;
            }
        }
        public static List<FuelConsumptionRecord> convertEntrytoConsumptionRecords(List<FuelRecord> tentativeRecords)
        {
            List<FuelConsumptionRecord> newRecords = tentativeRecords.Select(x=> new FuelConsumptionRecord
            {
                recordDate     = DateTime.Today.Date,
                depotID        = x.depotID,
                consumedAmount = int.Parse(x.consumedFuel),
                importedAmount = int.Parse(x.importedFuel),
                fuelLevel      = int.Parse(x.remainingFuel),
            }).ToList();
            return newRecords;
        }
        public static List<FuelRecord> FilterFuelRecords(List<FuelRecord> fuelTentativeRecords)
        {
            List<int> operationalDepotIDs = DepotService.fetchOperationalDepots().Select(x => x.depotID).ToList();
            List<FuelRecord> filteredRecords = new List<FuelRecord>();
            foreach (var record in fuelTentativeRecords)
            {
                if (operationalDepotIDs.Contains(record.depotID))
                {
                    filteredRecords.Add(record);
                }

            }
            return filteredRecords;
        }
        public static int RetrieveProcurmenetOfficeFuelStorage()
        {
            if (File.Exists(AdditionalDataViewModel.AdditionalDataPath))
            {
                AdditionalInfo infoData = new AdditionalInfo();
                var additionalRecordsJsonString = File.ReadAllText(AdditionalDataViewModel.AdditionalDataPath);
                infoData = JsonConvert.DeserializeObject<AdditionalInfo>(additionalRecordsJsonString);
                if (checkRecordExistence())
                {
                    return infoData.procurmentOfficeFuelAmount;
                }
                else
                {
                    int sumofImportedFuel = 0;
                    AddFuelRecordViewModel AddFuelRecordVM = new AddFuelRecordViewModel();
                    if (File.Exists(AddFuelRecordVM.fuelRecordsFilePath))
                    {
                        var fuelRecordsJsonString = File.ReadAllText(AddFuelRecordVM.fuelRecordsFilePath);
                        List<FuelRecord> filteredFuelRecords = FuelService.FilterFuelRecords(JsonConvert.DeserializeObject<List<FuelRecord>>(fuelRecordsJsonString));
                        sumofImportedFuel = filteredFuelRecords.Select(x => int.Parse(x.importedFuel)).Sum();
                    }
                    bool canImport = CanImportFuelFromProcurementOffice(sumofImportedFuel);
                    if (canImport)
                    {
                        return infoData.procurmentOfficeFuelAmount - sumofImportedFuel;
                    }
                    else
                    {
                        throw new Exception("لا يوجد مخزون وقود كافى بمكتب الإتصال");
                    }
                }
            }
            else
            {
                throw new Exception("برجاء تحديد مقدار مخزون الوقود بمكتب الإتصال من صفحة معلومات إضافية");
            }
        }
        public static void UpdateProcurementOfficeFuelStorage(int procuredAmount)
        {
            if (File.Exists(AdditionalDataViewModel.AdditionalDataPath))
            {
                AdditionalInfo infoData = new AdditionalInfo();
                var additionalRecordsJsonString = File.ReadAllText(AdditionalDataViewModel.AdditionalDataPath);
                infoData = JsonConvert.DeserializeObject<AdditionalInfo>(additionalRecordsJsonString);
                bool canImport = CanImportFuelFromProcurementOffice(procuredAmount);
                if (canImport)
                {
                    infoData.procurmentOfficeFuelAmount = infoData.procurmentOfficeFuelAmount - procuredAmount;
                    var json = JsonConvert.SerializeObject(infoData, Formatting.Indented);
                    File.WriteAllText(AdditionalDataViewModel.AdditionalDataPath, json);
                }
            }
            else
            {
                throw new Exception("برجاء تحديد مقدار مخزون الوقود بمكتب الإتصال من صفحة معلومات إضافية");
            }
        }
        public static bool CanImportFuelFromProcurementOffice(int procuredAmount)
        {
            if (File.Exists(AdditionalDataViewModel.AdditionalDataPath))
            {
                AdditionalInfo infoData = new AdditionalInfo();
                var additionalRecordsJsonString = File.ReadAllText(AdditionalDataViewModel.AdditionalDataPath);
                infoData = JsonConvert.DeserializeObject<AdditionalInfo>(additionalRecordsJsonString);
                if (infoData.procurmentOfficeFuelAmount >= procuredAmount)
                {
                    return true;
                }
                else
                {
                    throw new Exception("لا يوجد مخزون وقود كافى بمكتب الإتصال");
                }
            }
            else
            {
                throw new Exception("برجاء تحديد مقدار مخزون الوقود بمكتب الإتصال من صفحة معلومات إضافية");
            }

        }
    }
}
