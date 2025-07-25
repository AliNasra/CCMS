using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp2.Models;
using WpfApp2.Models.Items;
using WpfApp2.ViewModels.Fuel;
using System.IO;
using WpfApp2.ViewModels.Concrete;
using WpfApp2.ViewModels.Cement;

namespace WpfApp2.Services
{
    public class ValidationServices
    {
        public static List<ConcreteProductionRecord> verifyConcreteRecords(bool includeInformal)
        {
            bool areThereConcreteRecords = !ConcreteService.canAddRecords();
            if (areThereConcreteRecords)
            {
                return ConcreteService.retrieveRecords(DateTime.Today.Date, includeInformal);
            }
            else
            {
                if (File.Exists(AddConcreteRecordViewModel.concreteRecordsFilePath))
                {

                    var concreteRecordsJsonString = File.ReadAllText(AddConcreteRecordViewModel.concreteRecordsFilePath);
                    List<ConcreteRecords> tentativeConcreteRecords = JsonConvert.DeserializeObject<List<ConcreteRecords>>(concreteRecordsJsonString);
                    bool checkRecordValidity = ConcreteService.validateConcreteRecord(tentativeConcreteRecords);
                    if (checkRecordValidity)
                    {
                        List<ConcreteProductionRecord> records = ConcreteService.convertTentativeToProductionRecord(tentativeConcreteRecords);
                        if (includeInformal)
                        {
                            return records;
                        }
                        else
                        {
                            return records.Where(x=>x.isInformal == false).ToList();
                        }
                        
                    }
                    else
                    {
                        throw new Exception("برجاء التحقق من صحة تمام الخرسانة");
                    }

                }
                else
                {
                   return new List<ConcreteProductionRecord> { };
                }
            }
        }

        public static List<PreCastWallProgressRecord> verifyWallRecords()
        {
            bool areThereWallRecords = !PreCastWallService.canAddRecords();
            if (areThereWallRecords)
            {
                return PreCastWallService.retrieveRecords(DateTime.Today.Date);
            }
            else
            {
                if (File.Exists(AddWallRecordViewModel.wallRecordsFilePath))
                {
                    var wallRecordsJsonString = File.ReadAllText(AddWallRecordViewModel.wallRecordsFilePath);
                    List<PreCastWallRecord> tentativeWallRecords = JsonConvert.DeserializeObject<List<PreCastWallRecord>>(wallRecordsJsonString);
                    bool checkRecordValidity = PreCastWallService.validateRecords(tentativeWallRecords);
                    if (checkRecordValidity)
                    {
                        return PreCastWallService.convertTentativeToProgressRecord(tentativeWallRecords);
                    }
                    else
                    {
                        throw new Exception("برجاء التحقق من صحة تمام الحائط سابق الصب");
                    }

                }
                else
                {
                    return new List<PreCastWallProgressRecord> { };
                }
            }
        }

        public static List<CementDailyRecord> verifyCementRecords()
        {
            bool areThereCementRecords = !CementService.canAddRecords();
            if (areThereCementRecords)
            {
                return CementService.retrieveRecords(DateTime.Today.Date);
            }
            else
            {
                if (File.Exists(AddCementRecordViewModel.cementRecordsFilePath))
                {
                    var cementRecordsJsonString = File.ReadAllText(AddCementRecordViewModel.cementRecordsFilePath);
                    List<CementRecord> tentativeCementRecords = JsonConvert.DeserializeObject<List<CementRecord>>(cementRecordsJsonString);
                    bool checkRecordValidity = CementService.validateCementRecords(tentativeCementRecords);
                    if (checkRecordValidity)
                    {
                       return CementService.convertTentativeToDailyRecord(tentativeCementRecords);
                    }
                    else
                    {
                        throw new Exception("برجاء التحقق من صحة تمام الأسمنت");
                    }

                }
                else
                {
                    return new List<CementDailyRecord> { };
                }
            }
        }

        public static List<FuelConsumptionRecord> verifyFuelRecords()
        {
            bool areThereRecords = FuelService.checkRecordExistence();
            if (areThereRecords == true)
            {
                return FuelService.retrieveFuelConsumptionRecords();
            }
            else
            {
                var AddFuelRecordVM = new AddFuelRecordViewModel();
                if (File.Exists(AddFuelRecordVM.fuelRecordsFilePath))
                {
                    var fuelRecordsJsonString = File.ReadAllText(AddFuelRecordVM.fuelRecordsFilePath);
                    List<FuelRecord> FuelRecords = JsonConvert.DeserializeObject<List<FuelRecord>>(fuelRecordsJsonString);
                    if (FuelService.ValidateFuelRecords(FuelRecords))
                    {
                        List<FuelConsumptionRecord> fuelConsumptionRecords = FuelService.convertEntrytoConsumptionRecords(FuelRecords);
                        return fuelConsumptionRecords;
                    }
                    else
                    {
                        throw new Exception("برجاء التحقق من صحة التمام");
                    }

                }
                else
                {
                    return new List<FuelConsumptionRecord> { };
                }
            }
        }
        
    }
}
