using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1;
using WpfApp2.Models;

namespace WpfApp2.Services
{
    public class DepotService
    {
        public static void addDepot(string selectedUnit, string depotStorageCapacity, string depotName, string currentReserve,string lastImportedFuelAmount, DateTime lastConsignmentDate)
        {
            using (var context = new ApplicationDbContext())
            {
                try
                {
                    int currentReserveInt ;
                    int depotStorageCapacityInt;
                    int lastImportedFuelAmountInt;
                    bool currentReserveIsInt = int.TryParse(currentReserve, out currentReserveInt);
                    bool depotStorageCapacityIsInt = int.TryParse(depotStorageCapacity, out depotStorageCapacityInt);
                    bool lastImportedFuelAmountIsInt = int.TryParse(lastImportedFuelAmount, out lastImportedFuelAmountInt);
                    if (depotStorageCapacityIsInt == false || currentReserveIsInt == false || lastImportedFuelAmountIsInt == false)
                    {
                        throw new Exception("برجاء التأكد من صحة الأرقام المدخلة");
                    }
                    if ( currentReserveInt> depotStorageCapacityInt)
                    {
                        throw new Exception("لا يمكنك تخزين وقود أكثر من سعة المستودع");
                    }
                    Unit? unit = UnitService.fetchUnit(selectedUnit);
                    if (unit == null)
                    {
                        throw new Exception("! لا توجد وحدة بهذا الاسم");
                    }
                    if (currentReserveInt < 0)
                    {
                        throw new ArgumentOutOfRangeException("الكمية المخزنة للوقود لا يمكن أن تكون أقل من صفر");
                    }
                    if (depotStorageCapacityInt < 0)
                    {
                        throw new ArgumentOutOfRangeException("سعة المستودع التخزينية لا يجب أن تكون أقل من صفر");
                    }

                    if (currentReserveInt > depotStorageCapacityInt)
                    {
                        throw new ArgumentOutOfRangeException("الكمية المخزنة للوقود لا يمكن أن تتعدى سعة المستودع التخزينية");
                    }
                    if (lastImportedFuelAmountInt < 0)
                    {
                        throw new Exception("لا يمكنك توريد وقود أقل من صفر");
                    }
                    if (lastConsignmentDate.Date > DateTime.Today.Date)
                    {
                        throw new Exception("لا يمكنك توريد وقود فى تاريخ لاحق");
                    }
                    var depot = new FuelDepot
                    {
                        depotStorageCapacity   = depotStorageCapacityInt,
                        depotName              = depotName,
                        unitID                 = unit.unitID,
                        currentReserve         = currentReserveInt,
                        LastConsignmentDate    = lastConsignmentDate,
                        LastimportedFuelAmount = lastImportedFuelAmountInt,
                        isOperational          = true
                    };

                    context.depots.Add(depot);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public static int fetchDepotID(string unitName)
        {
            using (var context = new ApplicationDbContext())
            {
                var unitInfo           = unitName.Split();
                string unitDesignation = unitInfo[0];
                int unitCode           = int.Parse(unitInfo[1]);
                string unitSpecialization;
                if (unitInfo.Length == 3)
                {
                    unitSpecialization = unitInfo[2];
                }
                else
                {
                    unitSpecialization = $"{unitInfo[2]} {unitInfo[3]}";
                }
                int depot = context.depots.Join(
                    context.units,
                    d=>d.unitID,
                    u=>u.unitID,
                    (d, u) => new{
                        depotID              = d.depotID,
                        unitDesignation      = u.unitDesignation,
                        unitCode             = u.unitCode,
                        unitSpecialization   = u.unitSpecialization
                    }
                    ).Where(x => x.unitDesignation == unitDesignation && x.unitCode == unitCode && x.unitSpecialization == unitSpecialization).Select(g => g.depotID).FirstOrDefault();
                return depot;

            }               
        }
        public static List<FuelDepot> fetchDepots()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.depots.ToList();
            }
        }

        public static List<FuelDepot> fetchOperationalDepots()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.depots.Join(context.units, d => d.unitID, u => u.unitID, (d, u) =>
                new {
                    unitID                 = d.unitID,
                    isUnitOperational      = u.isOperational,
                    depotID                = d.depotID,
                    isDepotOperational     = d.isOperational,
                    depotStorageCapacity   = d.depotStorageCapacity,
                    depotName              = d.depotName,
                    currentReserve         = d.currentReserve,
                    LastimportedFuelAmount = d.LastimportedFuelAmount,
                    LastConsignmentDate    = d.LastConsignmentDate
                }).Where(du=>du.isDepotOperational == true && du.isUnitOperational == true).Select(du => new FuelDepot
                {
                    unitID                 = du.unitID,
                    isOperational          = du.isDepotOperational,
                    depotID                = du.depotID,
                    depotStorageCapacity   = du.depotStorageCapacity,
                    depotName              = du.depotName,
                    currentReserve         = du.currentReserve,
                    LastimportedFuelAmount = du.LastimportedFuelAmount,
                    LastConsignmentDate    = du.LastConsignmentDate
                }).ToList();
            }
        }

        public static FuelDepot fetchDepot(string depotName)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.depots.Where(x=>x.depotName == depotName).FirstOrDefault();
            }
        }

        public static FuelDepot fetchDepot(int depotID)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.depots.Where(x => x.depotID == depotID).FirstOrDefault();
            }
        }


        public static void editDepot(string selectedDepotName,string selectedUnit, string DepotStorageCapacity, string DepotName, string CurrentReserve, string LastImportedFuelAmount, DateTime LastConsignmentDate, string selectedOperationality)
        {
            using (var context = new ApplicationDbContext())
            {
                if (string.IsNullOrWhiteSpace(selectedDepotName))
                {
                    throw new Exception("برجاء إختيار مستودع لتعديله");
                }
                FuelDepot depot = context.depots.Where(x => x.depotName == selectedDepotName).FirstOrDefault();
                Unit unit       = UnitService.fetchUnit(selectedUnit);
                int depotStorageCapacityInt;
                int currentReserveInt;
                int LastImportedFuelAmountInt;
                bool depotStorageCapacityIsInt;
                bool currentReserveIsInt;
                bool LastImportedFuelAmountIsInt;
                bool isIntCheck;
                bool isBiggerThanZero;
                bool depotNameUniquenessCheck;
                bool isOperational;
                depotStorageCapacityIsInt = int.TryParse(DepotStorageCapacity,out depotStorageCapacityInt);
                currentReserveIsInt = int.TryParse(CurrentReserve, out currentReserveInt);
                LastImportedFuelAmountIsInt = int.TryParse(LastImportedFuelAmount, out LastImportedFuelAmountInt);
                if (string.IsNullOrWhiteSpace(DepotName))
                {
                    throw new Exception("برجاء تحديد اسم مستودع الوقود");
                }
                depotNameUniquenessCheck = !context.depots.Where(x => x.depotID != depot.depotID && x.depotName == DepotName.Trim()).Any();
                if (!depotNameUniquenessCheck)
                {
                    throw new Exception("برجاء اختيار مميز اسم ممير للمستودع");
                }
                if (string.IsNullOrWhiteSpace(selectedOperationality))
                {
                    throw new Exception("برجاء اختيار حالة الوحدة");
                }
                else
                {
                    isOperational = selectedOperationality == "بالخدمة" ? true : false;
                }
                isIntCheck = !new List<bool> { depotStorageCapacityIsInt, currentReserveIsInt, LastImportedFuelAmountIsInt }.Where(x=> x == false).Any();
                if (!isIntCheck)
                {
                    throw new Exception("برجاء التأكد من المدخلات الرقمية");
                }
                isBiggerThanZero = !new List<int> { depotStorageCapacityInt, currentReserveInt, LastImportedFuelAmountInt }.Where(x => x < 0).Any();
                if (!isBiggerThanZero)
                {
                    throw new Exception("برجاء التأكد من أن المدخلات الرقمية ليست أقل من صفر");
                }
                if (LastConsignmentDate.Date>DateTime.Today.Date)
                {
                    throw new Exception("برجاء التأكد من أن صحة تاريخ أخر توريد سولار");
                }
                depot.depotStorageCapacity = depotStorageCapacityInt;
                depot.depotName = DepotName.Trim();
                depot.unitID = unit.unitID;
                depot.currentReserve = currentReserveInt;
                depot.LastimportedFuelAmount = LastImportedFuelAmountInt;
                depot.isOperational = isOperational;
                depot.LastConsignmentDate = LastConsignmentDate;
                context.SaveChanges();
            }
            
        }



    }
}
