using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WebApplication1;
using WpfApp2.Models;

namespace WpfApp2.Services
{
    public class MixerService
    {
        public static void addMixer(string cabbageNo,string mixerName, string isOperational, string operationalCapacity,string currentCementLevel, string depotName)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    int cabbageNoInt;
                    int operationCapacityInt;
                    double currentCementLevelDouble;
                    bool CabbageNoIsInt = int.TryParse(cabbageNo,out cabbageNoInt);
                    bool operationCapacityIsInt = int.TryParse(operationalCapacity, out operationCapacityInt);
                    bool currentCementLevelIsDouble = Double.TryParse(currentCementLevel, out currentCementLevelDouble);
                    if (CabbageNoIsInt == false || operationCapacityIsInt == false || currentCementLevelIsDouble == false)
                    {
                        throw new Exception("برجاء التأكد من صحة الأرقام المدخلة");
                    }
                    if (string.IsNullOrEmpty(depotName))
                    {
                        throw new Exception("لا توجد وحدة بهذا الاسم");
                    }
                    if (cabbageNoInt < 0)
                    {
                        throw new ArgumentOutOfRangeException("رقم الكاباج لا يجب أن يكون أقل من صفر");
                    }
                    if (operationCapacityInt < 0)
                    {
                        throw new ArgumentOutOfRangeException("الطاقة الإنتاجية لا تجب أن تكون أقل من صفر");
                    }
                    if (currentCementLevelDouble < 0)
                    {
                        throw new ArgumentOutOfRangeException("مخزون الأسمنت لا يجب أن يكون أقل من صفر");
                    }
                    if (context.mixers.Where(x=>x.cabbageNo == cabbageNoInt).Any())
                    {
                        throw new ArgumentOutOfRangeException("رقم الكاباج يجب أن يكون مميزاً");
                    }
                    if (context.mixers.Where(x => x.mixerName == mixerName).Any())
                    {
                        throw new ArgumentOutOfRangeException("اسم الخلاطة يجب أن يكون مميزاً");
                    }

                    bool isOperationalBool = isOperational == "نعم" ? true : false;   
                    int depotID = DepotService.fetchDepot(depotName).depotID;
                    if (depotID == 0)
                    {
                        throw new Exception("يجب تعريف مخزون وقود للوحدة قبل تعريف خلاطة جديدة");
                    }
                    var mixer = new Mixer
                    {
                        depotID             = depotID,
                        isOperational       = isOperationalBool,
                        cabbageNo           = cabbageNoInt,
                        mixerName           = mixerName,
                        operationalCapacity = operationCapacityInt,
                        currentCementLevel  = currentCementLevelDouble
                    };
                    context.mixers.Add(mixer);
                    context.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public static List<Mixer> getOperationalMixers()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.mixers.Join(context.depots,m=>m.depotID,d=>d.depotID,
                    (m,d)=> new
                    {
                         mixerID             = m.mixerID,
                         unitID              = d.unitID,
                         cabbageNo           = m.cabbageNo ,
                         mixerName           = m.mixerName,
                         isMixerOperational  = m.isOperational,
                         isDepotOperational  = d.isOperational,
                         operationalCapacity = m.operationalCapacity,
                         currentCementLevel  = m.currentCementLevel,
                         depotID             = m.depotID
                    }
                    ).Where(md=>md.isMixerOperational == true && md.isDepotOperational == true).Join(context.units,md=>md.unitID, u=>u.unitID,
                    (md,u)=> new 
                    {
                        mixerID = md.mixerID,
                        depotID = md.depotID,
                        unitID  = md.unitID,
                        cabbageNo = md.cabbageNo,
                        mixerName = md.mixerName,
                        isMixerOperational = md.isMixerOperational,
                        isUnitOperational = u.isOperational,
                        operationalCapacity = md.operationalCapacity,
                        currentCementLevel = md.currentCementLevel,
                    }).Where(mdu=>mdu.isUnitOperational == true).Select(mdu=>new Mixer
                    {
                        mixerID = mdu.mixerID,
                        depotID = mdu.depotID,
                        cabbageNo = mdu.cabbageNo,
                        mixerName = mdu.mixerName,
                        isOperational = mdu.isMixerOperational,
                        operationalCapacity = mdu.operationalCapacity,
                        currentCementLevel = mdu.currentCementLevel,
                    }).ToList();
            }
        }

        public static List<Mixer> getAllMixers()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.mixers.ToList();
            }
        }

        public static Mixer getMixer(int mixerID)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.mixers.Where(x => x.mixerID == mixerID).FirstOrDefault();
            }
        }

        public static Mixer getMixer(string mixerName)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.mixers.Where(x => x.mixerName == mixerName).FirstOrDefault();
            }
        }

        public static Unit getOperatingUnit(int mixerID)
        {
            using (var context = new ApplicationDbContext())
            {
                int unitID = context.mixers.Join(context.depots,
                    m => m.depotID,
                    d => d.depotID,
                    (m, d) => new
                    {
                        unitID  = d.unitID,
                        mixerID = m.mixerID
                    }).Where(x=>x.mixerID == mixerID).Select(g => g.unitID).FirstOrDefault();
                return context.units.Where(x => x.unitID == unitID).FirstOrDefault();
            }
        }

        public static List<List<string>> prepareTableEntries(List<ConcreteProductionRecord>concreteRecords, List<CementDailyRecord> cementRecords,List<Mixer> mixerList, int startIndex)
        {
            List<List<string>> entryRows     = new List<List<string>>();
            int                mixerCounter  = startIndex;
            double             epsilon       = 1e-8;
            string             company;
            string             project;
            string             producedAmount;
            string             concreteSummationInDetail = "";
            foreach (var mixer in mixerList)
            {
                List<string> mixerEntryRows = new List<string>();
                List<ConcreteProductionRecord> mixerConcreteRecords = concreteRecords.Where(x=>x.mixerID == mixer.mixerID).ToList();
                CementDailyRecord cementRecord = cementRecords.Where(x => x.mixerID == mixer.mixerID).FirstOrDefault();
                string remainingCement;
                string consumedCement;
                string previouslyRemainingCement;
                string importedCement;
                string productionCapacity;
                string unitName;
                string isNotFunctional;
                string isFunctional;
                string cabbageNo;
                string mixerName;
                string counter;
                if (cementRecord != null)
                {
                    double previouslyRemaniningCement = (double)((decimal)cementRecord.remaniningCement + (decimal)cementRecord.consumedCement - (decimal)cementRecord.importedCement);
                    remainingCement = cementRecord.remaniningCement < epsilon        && cementRecord.remaniningCement > -1 * epsilon ? "-" : cementRecord.remaniningCement.ToString();
                    consumedCement = cementRecord.consumedCement < epsilon           && cementRecord.consumedCement   > -1 * epsilon ? "-" : cementRecord.consumedCement.ToString();
                    previouslyRemainingCement = previouslyRemaniningCement < epsilon && previouslyRemaniningCement    > -1 * epsilon ? "-" : previouslyRemaniningCement.ToString();
                    importedCement = cementRecord.importedCement < epsilon           && cementRecord.importedCement   > -1 * epsilon ? "-" : cementRecord.importedCement.ToString();
                }
                else
                {
                    remainingCement           = mixer.currentCementLevel < epsilon && mixer.currentCementLevel > -1 * epsilon ? "-" : mixer.currentCementLevel.ToString(); 
                    consumedCement            = "-";
                    previouslyRemainingCement = mixer.currentCementLevel < epsilon && mixer.currentCementLevel > -1 * epsilon ? "-" : mixer.currentCementLevel.ToString();
                    importedCement            = "-";
                }
                productionCapacity = mixer.operationalCapacity == 0 ? "-" : mixer.operationalCapacity.ToString();
                Unit managingUnit  = getOperatingUnit(mixer.mixerID);
                unitName           = $"{managingUnit.unitDesignation.ElementAt(2)} {managingUnit.unitCode} {managingUnit.unitSpecialization.Substring(0,3)}";
                isNotFunctional    = "-";
                isFunctional       = "✓";
                cabbageNo          = "كاباج";
                cabbageNo          += " ";
                cabbageNo          += "(";
                cabbageNo          += " ";
                cabbageNo          += $"{mixer.cabbageNo}";
                cabbageNo          += " ";
                cabbageNo          += $")"; 
                mixerName          = $"{mixer.mixerName}";
                counter            = mixerCounter.ToString();
                if (mixerConcreteRecords.Count == 0)
                {
                    company = "-";
                    project = "-";
                    producedAmount = "-";
                    concreteSummationInDetail = "-";
                    entryRows.Add(new List<string> { remainingCement, consumedCement, previouslyRemainingCement, importedCement, company, project, producedAmount, concreteSummationInDetail, productionCapacity, unitName, isNotFunctional, isFunctional, cabbageNo, mixerName, counter });
                }
                else
                {
                    double sumReinforcedConcrete = (double)concreteRecords.Where(x => x.isReinforcedConcrete == true && x.mixerID == mixer.mixerID).Select(x => (decimal)x.producedConcreteAmount).Sum();
                    double sumNormalConcrete     = (double)concreteRecords.Where(x => x.isReinforcedConcrete == false && x.mixerID == mixer.mixerID).Select(x => (decimal)x.producedConcreteAmount).Sum();
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
                    foreach (var concreteRecord in mixerConcreteRecords)
                    {
                        company = $"{concreteRecord.company}";
                        project = $"{concreteRecord.project}";
                        if (concreteRecord.isReinforcedConcrete)
                        {
                            producedAmount = "";
                            producedAmount += $"{concreteRecord.producedConcreteAmount.ToString("G29")}";
                            producedAmount += " ";
                            producedAmount += $"م3 خرسانة مسلحة";

                        }
                        else
                        {
                            producedAmount = "";
                            producedAmount += $"{concreteRecord.producedConcreteAmount.ToString("G29")}";
                            producedAmount += " ";
                            producedAmount += $"م3 خرسانة عادية";
                        }
                        entryRows.Add(new List<string> { remainingCement, consumedCement, previouslyRemainingCement, importedCement, company, project, producedAmount, concreteSummationInDetail, productionCapacity, unitName, isNotFunctional, isFunctional, cabbageNo, mixerName, counter });
                    }
                }
                mixerCounter++;
                concreteSummationInDetail = "";
            }
            return entryRows;
        }
        public static void editMixer(string SelectedMixerName, string CabbageNo, string MixerName, string IsOperational, string OperationalCapacity, string CurrentCementLevel, string depotName)
        {
            using (var context = new ApplicationDbContext())
            {
                if (string.IsNullOrEmpty(SelectedMixerName))
                {
                    throw new Exception("لا توجد خلاطة بهذا الاسم");
                }
                Mixer mixer = context.mixers.Where(x => x.mixerName == SelectedMixerName).FirstOrDefault();
                int depotID = DepotService.fetchDepot(depotName).depotID;
                if (depotID == 0)
                {
                    throw new Exception("يجب تعريف مخزون وقود للوحدة قبل إضافة خلاطة جديدة");
                }
                int cabbageNoInt;
                int operationCapacityInt;
                double currentCementLevelDouble;
                bool CabbageNoIsInt = int.TryParse(CabbageNo, out cabbageNoInt);
                bool operationCapacityIsInt = int.TryParse(OperationalCapacity, out operationCapacityInt);
                bool currentCementLevelIsDouble = Double.TryParse(CurrentCementLevel, out currentCementLevelDouble);
                if (CabbageNoIsInt == false || operationCapacityIsInt == false || currentCementLevelIsDouble == false)
                {
                    throw new Exception("برجاء التأكد من صحة الأرقام المدخلة");
                }
                if (string.IsNullOrEmpty(MixerName))
                {
                    throw new Exception("يجب إختيار اسم للخلاطة");
                }
                bool isNewNameUnique = !context.mixers.Where(x => x.mixerID != mixer.mixerID && (x.mixerName == MixerName || x.cabbageNo == cabbageNoInt )).Any();
                if (!isNewNameUnique)
                {
                    throw new Exception("يجب إختيار اسم مميز للخلاطة");
                }
                bool isOperationalBool    = IsOperational == "بالخدمة" ? true : false;
                mixer.isOperational       = isOperationalBool;
                mixer.depotID             = depotID;
                mixer.currentCementLevel  = currentCementLevelDouble;
                mixer.operationalCapacity = operationCapacityInt;
                mixer.cabbageNo           = cabbageNoInt;
                mixer.mixerName           = MixerName;
                context.SaveChanges();
            }
            
        }

    }
}
