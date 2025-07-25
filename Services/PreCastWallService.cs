using DocumentFormat.OpenXml.InkML;
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
    public class PreCastWallService
    {
        public static void addRecord(List<PreCastWallRecord> records)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    int distinctUnitcount = records.Select(x => x.unitName).Distinct().ToList().Count;
                    int unitRecordsCount = records.Count;
                    if (distinctUnitcount < unitRecordsCount)
                    {
                        throw new Exception("لا يمكنك إدخال تمام لنفس الوحدة مرتين");
                    }
                    else
                    {
                        foreach (var wallRecord in records)
                        {
                            int previouslyAccomplishedInt;
                            int accomplishedTodayInt;
                            int toBeAccomplishedInt;
                            int transportedAmountTodayInt;
                            int previouslyTransportedInt;
                            int remaningOnSiteInt;
                            bool previouslyAccomplishedIsInt  = int.TryParse(wallRecord.previouslyAccomplished, out previouslyAccomplishedInt);
                            bool accomplishedTodayIsInt       = int.TryParse(wallRecord.accomplishedToday, out accomplishedTodayInt);
                            bool toBeAccomplishedIsInt        = int.TryParse(wallRecord.toBeAccomplished, out toBeAccomplishedInt);
                            bool transportedAmountTodayIsInt  = int.TryParse(wallRecord.transportedAmountToday, out transportedAmountTodayInt);
                            bool previouslyTransportedIsInt   = int.TryParse(wallRecord.previouslyTransported, out previouslyTransportedInt);
                            bool remaningOnSiteIsInt          = int.TryParse(wallRecord.remainingOnSite, out remaningOnSiteInt);
                            List<bool> variableListBool       = new List<bool> { previouslyAccomplishedIsInt, accomplishedTodayIsInt, toBeAccomplishedIsInt, transportedAmountTodayIsInt, previouslyTransportedIsInt, remaningOnSiteIsInt };
                            List<int> variableListInt         = new List<int> { previouslyAccomplishedInt, accomplishedTodayInt, toBeAccomplishedInt, transportedAmountTodayInt, previouslyTransportedInt, remaningOnSiteInt };
                            bool unitCheck                    = context.units.Where(x => x.unitID == wallRecord.unitID && x.preCastWallTarget > 0).Any();
                            if (unitCheck == false)
                            {
                                throw new Exception($"{records.IndexOf(wallRecord) + 1} لم يعد ممكناً إضافة تمام سور سابق الصب للوحدة بالمدخل رقم ");
                            }
                            if (variableListBool.Where(x => x == false).Any())
                            {
                                throw new Exception($"{records.IndexOf(wallRecord) + 1} تحقق من الأرقام المدخلة فالمدخل رقم");
                            }
                            if (variableListInt.Where(x=>x <0).Any())
                            {
                                throw new Exception($"{records.IndexOf(wallRecord) + 1} تحقق من الأرقام المدخلة فالمدخل رقم");
                            }   
                            var record = new PreCastWallProgressRecord
                            {
                                 recordDate = DateTime.Today.Date ,
                                 unitID = wallRecord.unitID ,
                                 previouslyAccomplished = previouslyAccomplishedInt,
                                 accomplishedToday  = accomplishedTodayInt,
                                 toBeAccomplished = toBeAccomplishedInt,
                                 transportedAmountToday = transportedAmountTodayInt,
                                 previouslyTransported  = previouslyTransportedInt,
                                 remaningOnSite  = remaningOnSiteInt,
                            };
                            context.preCastWallProgressRecords.Add(record);
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
        public static bool validateRecords(List<PreCastWallRecord> records)
        {
            int distinctUnitcount = records.Select(x => x.unitName).Distinct().ToList().Count;
            int unitRecordsCount  = records.Count;
            
            if (distinctUnitcount < unitRecordsCount)
            {
                return false;
            }
            else
            {
                foreach (var wallRecord in records)
                {
                    using (var context = new ApplicationDbContext())
                    {
                        bool doesUnitHasPreCastWallTarget = context.units.Where(x => x.unitID == wallRecord.unitID && x.preCastWallTarget > 0).Any();
                        if (!doesUnitHasPreCastWallTarget)
                        {
                            return false;
                        }
                    }
                    int previouslyAccomplishedInt;
                    int accomplishedTodayInt;
                    int toBeAccomplishedInt;
                    int transportedAmountTodayInt;
                    int previouslyTransportedInt;
                    int remaningOnSiteInt;
                    bool previouslyAccomplishedIsInt = int.TryParse(wallRecord.previouslyAccomplished, out previouslyAccomplishedInt);
                    bool accomplishedTodayIsInt = int.TryParse(wallRecord.accomplishedToday, out accomplishedTodayInt);
                    bool toBeAccomplishedIsInt = int.TryParse(wallRecord.toBeAccomplished, out toBeAccomplishedInt);
                    bool transportedAmountTodayIsInt = int.TryParse(wallRecord.transportedAmountToday, out transportedAmountTodayInt);
                    bool previouslyTransportedIsInt = int.TryParse(wallRecord.previouslyTransported, out previouslyTransportedInt);
                    bool remaningOnSiteIsInt = int.TryParse(wallRecord.remainingOnSite, out remaningOnSiteInt);
                    List<bool> variableListBool = new List<bool> { previouslyAccomplishedIsInt, accomplishedTodayIsInt, toBeAccomplishedIsInt, transportedAmountTodayIsInt, previouslyTransportedIsInt, remaningOnSiteIsInt };
                    List<int> variableListInt = new List<int> { previouslyAccomplishedInt, accomplishedTodayInt, toBeAccomplishedInt, transportedAmountTodayInt, previouslyTransportedInt, remaningOnSiteInt };
                    if (variableListBool.Where(x => x == false).Any())
                    {
                        return false;
                    }
                    if (variableListInt.Where(x => x < 0).Any())
                    {
                        return false;
                    }

                }
                return true;

            }
        }
        public static bool canAddRecords()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    bool checkValue = context.preCastWallProgressRecords.Where(x => x.recordDate == DateTime.Today.Date).Any();
                    return !checkValue;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static List<PreCastWallProgressRecord> retrieveRecords(DateTime date)
        {
            using (var context = new ApplicationDbContext())
            {
                List<PreCastWallProgressRecord> wallRecords = context.preCastWallProgressRecords.Where(x => x.recordDate.Date == date.Date).ToList();
                return wallRecords;
            }
        }

        public static List<PreCastWallProgressRecord> convertTentativeToProgressRecord(List<PreCastWallRecord> wallRecordsToBeConverted)
        {
            List<PreCastWallProgressRecord> wallRecords = wallRecordsToBeConverted.Select(g => new PreCastWallProgressRecord
            {
                recordDate                  = DateTime.Today.Date,
                unitID                      = g.unitID,
                previouslyAccomplished      = int.Parse(g.previouslyAccomplished),
                accomplishedToday           = int.Parse(g.accomplishedToday),
                toBeAccomplished            = int.Parse(g.toBeAccomplished),
                transportedAmountToday      = int.Parse(g.transportedAmountToday),
                previouslyTransported       = int.Parse(g.previouslyTransported),
                remaningOnSite              = int.Parse(g.remainingOnSite)
            }).ToList();
            return wallRecords;
        }
        public static List<PreCastWallRecord> FilterWallRecords(List<PreCastWallRecord> wallTentativeRecords)
        {
            List<int> operationalUnitIDs = UnitService.getUnitsWithPreCastWallTarget().Select(x => x.unitID).ToList();
            List<PreCastWallRecord> filteredRecords = new List<PreCastWallRecord>();
            foreach (var record in wallTentativeRecords)
            {
                if (operationalUnitIDs.Contains(record.unitID))
                {
                    filteredRecords.Add(record);
                }

            }
            return filteredRecords;
        }
    }
}
