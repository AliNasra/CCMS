using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WebApplication1;
using WpfApp2.Models;
using WpfApp2.Models.Items;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WpfApp2.Services
{
    public class ConcreteService
    {
        public static List<FetchConcreteRecord> records;
        public static List<string> retrieveCompanyNames()
        {
            using ( var context = new ApplicationDbContext()){
                return context.concreteProductionRecords.Select(x=>x.company).Distinct().ToList();
            }
        }
        public static void addConcreteRecord(List<ConcreteRecords> concreteRecords)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    int distinctRecordCount = concreteRecords.Select(x => new {x.mixerName, x.company, x.project,x.isReinforced}).Distinct().ToList().Count;
                    int concreteRecordsCount = concreteRecords.Count;
                    if (distinctRecordCount < concreteRecordsCount)
                    {
                        throw new Exception("برجاء عدم تكرار تمام الخرسانة");
                    }
                    else
                    {
                        foreach (var concreteRecord in concreteRecords)
                        {
                            double producedAmountDouble;
                            bool producedAmountIsDouble = double.TryParse(concreteRecord.concreteAmount, out producedAmountDouble);
                            bool companyCheck = string.IsNullOrWhiteSpace(concreteRecord.company);
                            bool projectCheck = string.IsNullOrWhiteSpace(concreteRecord.project);
                            bool concreteTypeCheck = string.IsNullOrWhiteSpace(concreteRecord.isReinforced);
                            bool mixerNameCheck = string.IsNullOrWhiteSpace(concreteRecord.mixerName);
                            if (producedAmountIsDouble == false || companyCheck == true || projectCheck == true || concreteTypeCheck == true || mixerNameCheck == true)
                            {
                                throw new Exception($"{concreteRecords.IndexOf(concreteRecord) + 1} تحقق من المعطيات المدخلة فالمدخل رقم");
                            }
                            if (producedAmountDouble < 0)
                            {
                                throw new Exception($"{concreteRecords.IndexOf(concreteRecord) + 1} تحقق من كمية الخرسانة المنتجة فالمدخل رقم");
                            }
                            var record = new ConcreteProductionRecord
                            {
                                recordDate             = DateTime.Now.Date,
                                mixerID                = concreteRecord.mixerID,
                                company                = concreteRecord.company.Trim(),
                                project                = concreteRecord.project.Trim(),
                                producedConcreteAmount = producedAmountDouble,
                                isReinforcedConcrete   = concreteRecord.isReinforced == "مسلحة"? true:false,
                                isInformal             = concreteRecord.isInformal
                            };
                            context.concreteProductionRecords.Add(record);
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


        public static bool validateConcreteRecord(List<ConcreteRecords> concreteRecords)
        {
            try
            {
                int distinctRecordCount = concreteRecords.Select(x => new { x.mixerName, x.company, x.project, x.isReinforced }).Distinct().ToList().Count;
                int concreteRecordsCount = concreteRecords.Count;
                if (distinctRecordCount < concreteRecordsCount)
                {
                    return false;
                }
                else
                {
                    foreach (var concreteRecord in concreteRecords)
                    {
                        double producedAmountDouble;
                        bool producedAmountIsDouble = double.TryParse(concreteRecord.concreteAmount, out producedAmountDouble);
                        bool companyCheck = string.IsNullOrWhiteSpace(concreteRecord.company);
                        bool projectCheck = string.IsNullOrWhiteSpace(concreteRecord.project);
                        bool concreteTypeCheck = string.IsNullOrWhiteSpace(concreteRecord.isReinforced);
                        bool mixerNameCheck = string.IsNullOrWhiteSpace(concreteRecord.mixerName);
                        if (producedAmountIsDouble == false || companyCheck == true || projectCheck == true || concreteTypeCheck == true || mixerNameCheck == true)
                        {
                            return false;
                        }
                        if (producedAmountDouble < 0)
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


        public static void initializeConcreteRecords()
        {
            using (var context = new ApplicationDbContext())
            {
                records = context.concreteProductionRecords.Join(
                    context.mixers,
                    r => r.mixerID,
                    m => m.mixerID,
                    (r, m) => new 
                    {
                        mixerName = m.mixerName,
                        recordDate = r.recordDate,
                        company = r.company,
                        project = r.project,
                        producedConcreteAmount = r.producedConcreteAmount,
                        isReinforcedConcrete   = r.isReinforcedConcrete == true ? "مسلحة" : "عادية",
                        isInformal             = r.isInformal,
                    }).Where(rm=>rm.isInformal == false).Select(
                    g => new FetchConcreteRecord
                    {
                        mixerName = g.mixerName,
                        recordDate = g.recordDate,
                        company = g.company,
                        project = g.project,
                        producedConcreteAmount = g.producedConcreteAmount,
                        isReinforcedConcrete = g.isReinforcedConcrete
                    }).OrderByDescending(x => x.recordDate).ToList();
            }
        }
        public static List<FetchConcreteRecord> filterConcreteRecords(string mixerName, string project, string company,string concreteType)
        {
            List<FetchConcreteRecord> filteredRecords = new List<FetchConcreteRecord>();
            filteredRecords.AddRange(records) ;
            if (mixerName != "-")
            {
                filteredRecords = records.Where(x => x.mixerName == mixerName).ToList();
            }
            if (!string.IsNullOrWhiteSpace(project))
            {
                filteredRecords = filteredRecords.Where(x => x.project.Contains(project.Trim())).ToList();
            }
            if (company != "-")
            {
                filteredRecords = filteredRecords.Where(x => x.company == company).ToList();
            }
            if (concreteType != "-")
            {
                filteredRecords = filteredRecords.Where(x => x.isReinforcedConcrete == concreteType).ToList();
            }
            return filteredRecords;           
        }
        public static List<DataPoint> retrieveViewPoints(string mixerName, DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                return new List<DataPoint>();
            }
            var list = records.Where(x=>x.mixerName == mixerName && x.recordDate >= startDate && x.recordDate <= endDate).
                GroupBy(x => x.recordDate).
                Select( g => new{
                       recordDate = g.Key,
                       producedAmount =(double) g.Sum(r=>(decimal)r.producedConcreteAmount)
                       }
                ).ToList();
            List<DataPoint> producedDataPoints = new List<DataPoint>();
            if (DateTime.Today.Date < endDate)
            {
                endDate = DateTime.Today.Date;
            }
            int dayCount = (endDate - startDate).Days;
            for (int i = 0; i <= dayCount; i++)
            {
                var record = list.Where(x => x.recordDate == startDate.AddDays(i)).FirstOrDefault();
                if (record == null)
                {
                    producedDataPoints.Add(new DataPoint(DateTimeAxis.ToDouble(startDate.AddDays(i)), 0));
                }
                else
                {
                    producedDataPoints.Add(new DataPoint(DateTimeAxis.ToDouble(record.recordDate), record.producedAmount));
                }
            }
            return producedDataPoints;

        }
        public static bool canAddRecords()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    bool checkValue = context.concreteProductionRecords.Where(x => x.recordDate.Date == DateTime.Today.Date).Any();
                    return !checkValue;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public static List<ConcreteProductionRecord> retrieveRecords(DateTime date,bool includeInformal)
        {
            using (var context = new ApplicationDbContext())
            {
                List<ConcreteProductionRecord> concreteRecords = new List<ConcreteProductionRecord>();
                if (includeInformal)
                {
                    concreteRecords = context.concreteProductionRecords.Where(x => x.recordDate.Date == date.Date).ToList();
                }
                else
                {
                    concreteRecords = context.concreteProductionRecords.Where(x => x.recordDate.Date == date.Date && x.isInformal == false).ToList();
                }
                
                return concreteRecords;
            }
        }


        public static List<ConcreteProductionRecord> convertTentativeToProductionRecord(List<ConcreteRecords> concreteRecordsToBeConverted)
        {
            List<ConcreteProductionRecord> concreteRecords = concreteRecordsToBeConverted.Select(g=> new ConcreteProductionRecord
            {
                 recordDate             = DateTime.Today.Date,
                 company                = g.company,
                 project                = g.project,
                 producedConcreteAmount = double.Parse(g.concreteAmount),
                 isReinforcedConcrete   = g.isReinforced == "مسلحة"? true : false,
                 mixerID                = g.mixerID,
                 isInformal             = g.isInformal
            }).ToList();
            return concreteRecords;
        }
        public static List<ConcreteRecords> FilterConcreteRecords(List<ConcreteRecords> concreteTentativeRecords)
        {
            List<int> operationalMixerIDs = MixerService.getOperationalMixers().Select(x => x.mixerID).ToList();
            List<ConcreteRecords> filteredRecords = new List<ConcreteRecords>();
            foreach (var record in concreteTentativeRecords)
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
