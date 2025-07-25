using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1;
using WpfApp2.Models;
using WpfApp2.Models.Items;

namespace WpfApp2.Services
{
    public class CementService
    {
        public static List<FetchCementRecord> records;
        public static void addCementRecords(List<CementRecord> cementRecords)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    int distinctMixercount = cementRecords.Select(x => x.MixerName).Distinct().ToList().Count;
                    int cementRecordsCount = cementRecords.Count;
                    if (distinctMixercount < cementRecordsCount)
                    {
                        throw new Exception("لا يمكنك إدخال تمام لنفس الخلاطة مرتين");
                    }
                    else
                    {
                        foreach (var cementRecord in cementRecords)
                        {
                            double consumedAmountDouble;
                            double importedAmountDouble;
                            double remainingAmountDouble;
                            bool consumedAmountIsDouble = double.TryParse(cementRecord.consumedCement, out consumedAmountDouble);
                            bool importedAmountIsDouble = double.TryParse(cementRecord.importedCement, out importedAmountDouble);
                            bool remainingAmountIsDouble = double.TryParse(cementRecord.remaniningCement, out remainingAmountDouble);
                            if (consumedAmountIsDouble == false || importedAmountIsDouble == false || remainingAmountIsDouble == false)
                            {
                                throw new Exception($"{cementRecords.IndexOf(cementRecord) + 1} تحقق من الأرقام المدخلة فالمدخل رقم");
                            }
                            if (consumedAmountDouble < 0 || importedAmountDouble < 0 || remainingAmountDouble < 0)
                            {
                                throw new Exception($"{cementRecords.IndexOf(cementRecord) + 1} تحقق من الأرقام المدخلة فالمدخل رقم");
                            }
                            var record = new CementDailyRecord
                            {
                                recordDate = DateTime.Now.Date,
                                mixerID = cementRecord.mixerID,
                                consumedCement = consumedAmountDouble,
                                importedCement = importedAmountDouble,
                                remaniningCement = remainingAmountDouble
                            };
                            context.cementDailyRecords.Add(record);
                            var mixer = context.mixers.Where(x => x.mixerID == cementRecord.mixerID).FirstOrDefault();
                            mixer.currentCementLevel = remainingAmountDouble;
                        }
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static bool validateCementRecords(List<CementRecord> cementRecords)
        {
            try
            {
                int distinctMixercount = cementRecords.Select(x => x.MixerName).Distinct().ToList().Count;
                int cementRecordsCount = cementRecords.Count;
                if (distinctMixercount < cementRecordsCount)
                {
                    return false;
                }
                else
                {
                    foreach (var cementRecord in cementRecords)
                    {
                        double consumedAmountDouble;
                        double importedAmountDouble;
                        double remainingAmountDouble;
                        bool consumedAmountIsDouble = double.TryParse(cementRecord.consumedCement, out consumedAmountDouble);
                        bool importedAmountIsDouble = double.TryParse(cementRecord.importedCement, out importedAmountDouble);
                        bool remainingAmountIsDouble = double.TryParse(cementRecord.remaniningCement, out remainingAmountDouble);
                        if (consumedAmountIsDouble == false || importedAmountIsDouble == false || remainingAmountIsDouble == false)
                        {
                            return false;
                        }
                        if (consumedAmountDouble < 0 || importedAmountDouble < 0 || remainingAmountDouble < 0)
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

        public static void initializeCementRecords()
        {
            using (var context = new ApplicationDbContext())
            {
                records = context.cementDailyRecords.Join(context.mixers,
                        c => c.mixerID,
                        m => m.mixerID,
                        (c, m) => new
                        {
                            mixerName = m.mixerName,
                            recordDate = c.recordDate,
                            consumedAmount = c.consumedCement,
                            importedAmount = c.importedCement,
                            remainingAmount = c.remaniningCement
                        }).Select(g => new FetchCementRecord
                        {
                            mixerName = g.mixerName,
                            recordDate = g.recordDate,
                            consumedAmount = g.consumedAmount,
                            importedAmount = g.importedAmount,
                            remainingAmount = g.remainingAmount
                        }
                        ).OrderByDescending(x=>x.recordDate).ToList();
            }
        }

        public static List<FetchCementRecord> fetchCementRecords(string mixerName, DateTime startDate, DateTime endDate)
        {
            if (mixerName == "-")
            {
                return records.Where(x => x.recordDate <= endDate && x.recordDate >= startDate).ToList();
            }
            else
            {
                return records.Where(x => x.mixerName == mixerName && x.recordDate <= endDate && x.recordDate >= startDate).ToList();
            }

        }
        public static List<List<DataPoint>> retrieveViewPoints(string mixerName, DateTime startDate, DateTime endDate)
        {
            List<List<DataPoint>> dataPoints = new List<List<DataPoint>>();
            if (startDate > endDate)
            {
                dataPoints.Add(new List<DataPoint>());
                dataPoints.Add(new List<DataPoint>());
                dataPoints.Add(new List<DataPoint>());
                return dataPoints;
            }
            List<FetchCementRecord> filteredRecords = records.Where(x => x.recordDate <= endDate && x.recordDate >= startDate && x.mixerName == mixerName).ToList();
            List<DataPoint> importedDataPoints  = new List<DataPoint>();
            List<DataPoint> consumedDataPoints  = new List<DataPoint>();
            List<DataPoint> remainingDataPoints = new List<DataPoint>();

            if(DateTime.Today.Date< endDate)
            {
                endDate = DateTime.Today.Date;
            }

            int dayCount = (endDate - startDate).Days;
            for (int i = 0; i <= dayCount; i++)
            {
                var record = filteredRecords.Where(x => x.recordDate == startDate.AddDays(i)).FirstOrDefault();
                if (record == null)
                {
                    var earliestRecord = records.Where(x => x.recordDate < startDate.AddDays(i) && x.mixerName == mixerName).OrderByDescending(r => r.recordDate).FirstOrDefault();
                    if (earliestRecord == null)
                    {
                        remainingDataPoints.Add(new DataPoint(DateTimeAxis.ToDouble(startDate.AddDays(i)), 0));
                    }
                    else
                    {
                        remainingDataPoints.Add(new DataPoint(DateTimeAxis.ToDouble(startDate.AddDays(i)), earliestRecord.remainingAmount));
                    }
                    importedDataPoints.Add(new DataPoint(DateTimeAxis.ToDouble(startDate.AddDays(i)),0));
                    consumedDataPoints.Add(new DataPoint(DateTimeAxis.ToDouble(startDate.AddDays(i)),0));
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
                    bool checkValue = context.cementDailyRecords.Where(x => x.recordDate.Date == DateTime.Today.Date).Any();
                    return !checkValue;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public static List<CementDailyRecord> retrieveRecords(DateTime date)
        {
            using (var context = new ApplicationDbContext())
            {
                List<CementDailyRecord> cementRecords = context.cementDailyRecords.Where(x => x.recordDate.Date == date.Date).ToList();
                return cementRecords;
            }
        }
        public static List<CementDailyRecord> convertTentativeToDailyRecord (List<CementRecord> tentativeCementRecords)
        {
            return tentativeCementRecords.Select(x => new CementDailyRecord
            {
                recordDate = DateTime.Today.Date,
                mixerID    = x.mixerID,
                consumedCement = double.Parse(x.consumedCement) ,
                importedCement = double.Parse(x.importedCement),
                remaniningCement = double.Parse(x.remaniningCement) 
            }).ToList();
        }
        public static List<CementRecord> FilterCementRecords(List<CementRecord> cementTentativeRecords)
        {
            List<int> operationalMixerIDs = MixerService.getOperationalMixers().Select(x => x.mixerID).ToList();
            List<CementRecord> filteredRecords = new List<CementRecord>();
            foreach(var record in cementTentativeRecords)
            {
                if (operationalMixerIDs.Contains(record.mixerID))
                {
                    filteredRecords.Add(record);
                }

            }
            return filteredRecords;
        }

    }
}
