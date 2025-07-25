using Microsoft.EntityFrameworkCore;
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
    public class UnitService
    {
        public static void addUnit(string unitCode, string unitDesignation,string unitSpecialization,string selfSufficiencyReserve, string preCastWallTarget, string benzine80Reserve, string summerDieselReserve)
        {
            using (var context = new ApplicationDbContext())
            {
                try
                {
                    int unitCodeInt;
                    bool unitCodeIsInt = int.TryParse(unitCode,out unitCodeInt);
                    int selfSufficienyReserveInt;
                    bool selfSufficienyReserveIsInt = int.TryParse(selfSufficiencyReserve,out selfSufficienyReserveInt);
                    int preCastWallTargetInt;
                    bool preCastWallTargetIsInt = int.TryParse(preCastWallTarget, out preCastWallTargetInt);
                    int benzine80ReserveInt;
                    bool benzine80ReserveIsInt = int.TryParse(benzine80Reserve, out benzine80ReserveInt);
                    int summerDieselReserveInt;
                    bool summerDieselReserveIsInt = int.TryParse(summerDieselReserve, out summerDieselReserveInt);
                    if (unitCodeIsInt == false || selfSufficienyReserveIsInt == false || preCastWallTargetIsInt == false || benzine80ReserveIsInt == false  || summerDieselReserveIsInt == false)
                    {
                        throw new ArgumentException("برجاء التأكد من صحة الأرقام المدخلة");
                    }
                    if (unitCodeInt <= 0)
                    {
                        throw new ArgumentOutOfRangeException("كود الوحدة يجب أن يكون أكبر من صفر");
                    }
                    if (selfSufficienyReserveInt < 0)
                    {
                        throw new ArgumentOutOfRangeException("إحتياطى وقود الوحدة يجب أن يكون أكبر من صفر");
                    }
                    if (preCastWallTargetInt < 0)
                    {
                        throw new ArgumentOutOfRangeException("طول سور السابق الصب المخصص للوحدة يجب أن يكون على الأقل صفر");
                    }
                    var unit = new Unit
                    {
                        unitDesignation       = unitDesignation,
                        unitCode              = unitCodeInt,
                        unitSpecialization    = unitSpecialization,
                        selfSufficienyReserve = selfSufficienyReserveInt,
                        preCastWallTarget     = preCastWallTargetInt,
                        summerDieselReserve   = summerDieselReserveInt,
                        benzine80Reserve      = benzine80ReserveInt,
                        isOperational         = true
                    };

                    context.units.Add(unit);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw;
                }                      
            }
        }

        public static List<Unit> retrieveUnits()
        {
            using (var context = new ApplicationDbContext())
            {
                var units = context.units.ToList();
                return units;
            }
        }
        public static List<Unit> retrieveOperationalUnits()
        {
            using (var context = new ApplicationDbContext())
            {
                var units = context.units.Where(x=>x.isOperational == true).ToList();
                return units;
            }
        }

        public static List<Unit> retrieveOperationalUnitsWithOperationalDepots()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.units.Join(context.depots, u => u.unitID, d => d.unitID, (u, d) =>
                new {
                    unitID                = u.unitID,
                    isUnitOperational     = u.isOperational,
                    isDepotOperational    = d.isOperational,
                    unitSpecialization    = u.unitSpecialization,
                    unitCode              = u.unitCode,
                    unitDesignation       = u.unitDesignation,
                    selfSufficienyReserve = u.selfSufficienyReserve,
                    preCastWallTarget     = u.preCastWallTarget
                }).Where(du => du.isDepotOperational == true && du.isUnitOperational == true).Select(du => new Unit
                {
                    unitID                = du.unitID,
                    isOperational         = du.isDepotOperational,
                    unitSpecialization    = du.unitSpecialization,
                    unitCode              = du.unitCode,
                    unitDesignation       = du.unitDesignation,
                    selfSufficienyReserve = du.selfSufficienyReserve,
                    preCastWallTarget     = du.preCastWallTarget
                }).ToList();
            }
        }

        public static List<string> retrieveUnitNames()
        {
            using (var context = new ApplicationDbContext())
            {
                List<string> unitNames = context.units.Select(x=>$"{x.unitDesignation} {x.unitCode} {x.unitSpecialization}").ToList();
                return unitNames;
            }
        }


        public static Unit? fetchUnit(string unitName)
        {
            if (unitName == null || unitName.Trim().Length == 0)
            {
                return null;
            }
            var unitInfo = unitName.Trim().Split();
            using (var context = new ApplicationDbContext())
            {
                
                if (unitInfo.Length == 3)
                {
                    return context.units.Where(x => x.unitDesignation == unitInfo[0] && x.unitCode == int.Parse(unitInfo[1]) && x.unitSpecialization == unitInfo[2]).FirstOrDefault();
                }
                else
                {
                    string unitSpecialization = $"{unitInfo[2]} {unitInfo[3]}";
                    return context.units.Where(x => x.unitDesignation == unitInfo[0] && x.unitCode == int.Parse(unitInfo[1]) && x.unitSpecialization == unitSpecialization).FirstOrDefault();
                }
            }
        }

        public static Unit? fetchUnit(int unitID)
        {

            using (var context = new ApplicationDbContext())
            {
                Unit? unit = context.units.Where(x => x.unitID == unitID).FirstOrDefault();
                return unit;
            }
        }

        public static List<Unit> getUnitsWithPreCastWallTarget()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.units.Where(x => x.preCastWallTarget > 0 && x.isOperational == true).ToList();
            }
        }
        public static void editUnit(string unitName, string unitDesignation, string unitSpecialization, string precastWallTarget, string unitCode, string selfSufficienyReserve, string selectedOperationality, string benzine80Reserve, string summerDieselReserve)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    if (string.IsNullOrWhiteSpace(unitName))
                    {
                        throw new Exception("لا توجد وحدة بهذا الاسم");
                    }
                    var unitInfo = unitName.Split();
                    var unit = context.units.Where(x => x.unitDesignation == unitInfo[0] && x.unitCode == int.Parse(unitInfo[1]) && x.unitSpecialization == unitInfo[2]).FirstOrDefault();
                    if (unit == null)
                    {
                        throw new Exception("لا توجد وحدة بهذا الاسم");
                    }
                    else
                    {
                        int selfSufficiencyReserveInt;
                        int preCastWallTargetInt;
                        int unitCodeInt;
                        int benzine80ReserveInt;
                        int summerDieselReserveInt;
                        bool selfSufficiencyReserveIsInt;
                        bool preCastWallTargetIsInt;
                        bool unitCodeIsInt;
                        bool isOperational;
                        bool benzine80ReserveIsInt;
                        bool summerDieselReserveIsInt;
                        benzine80ReserveIsInt       = int.TryParse(benzine80Reserve, out benzine80ReserveInt);             
                        summerDieselReserveIsInt    = int.TryParse(summerDieselReserve, out summerDieselReserveInt);
                        selfSufficiencyReserveIsInt = int.TryParse(selfSufficienyReserve, out selfSufficiencyReserveInt);
                        preCastWallTargetIsInt      = int.TryParse(precastWallTarget, out preCastWallTargetInt);
                        unitCodeIsInt               = int.TryParse(unitCode, out unitCodeInt);
                        if (string.IsNullOrWhiteSpace(selectedOperationality))
                        {
                            throw new Exception("برجاء اختيار حالة الوحدة");
                        }
                        else
                        {
                            isOperational = selectedOperationality == "بالخدمة" ? true : false;
                        }

                        if (string.IsNullOrWhiteSpace(unitSpecialization))
                        {
                            throw new Exception("برجاء اختيار تخصص الوحدة");
                        }

                        if (string.IsNullOrWhiteSpace(unitDesignation))
                        {
                            throw new Exception("برجاء اختيار نوع الوحدة");
                        }
                  
                        bool isParsedSuccessfully = !new List<bool> { selfSufficiencyReserveIsInt, preCastWallTargetIsInt, unitCodeIsInt , benzine80ReserveIsInt, summerDieselReserveIsInt, summerDieselReserveIsInt }.Where(x=> x== false).Any();
                        if (!isParsedSuccessfully)
                        {
                            throw new Exception("برجاء التحقق من أن المدخلات الرقمية مكتوبة بشكل صحيح");
                        }
                        bool isBiggerThanZero     = !new List<int> { selfSufficiencyReserveInt, preCastWallTargetInt, unitCodeInt, benzine80ReserveInt , summerDieselReserveInt }.Where(x => x < 0).Any();
                        if (!isBiggerThanZero)
                        {
                            throw new Exception("برجاء التحقق من أن المدخلات الرقمية المكتوبة أكبر من صفر");
                        }
                        bool isUnique = !context.units.Where(x => x.unitSpecialization == unitSpecialization.Trim() && x.unitDesignation == unitDesignation.Trim() && x.unitCode == unitCodeInt && x.unitID != unit.unitID).Any();
                        if (!isUnique)
                        {
                            throw new Exception("يجب أن يكون اسم الوحدة الجديد مميز");
                        }
                        unit.unitCode              = unitCodeInt;
                        unit.unitDesignation       = unitDesignation.Trim();
                        unit.unitSpecialization    = unitSpecialization.Trim();
                        unit.preCastWallTarget     = preCastWallTargetInt;
                        unit.selfSufficienyReserve = selfSufficiencyReserveInt;
                        unit.summerDieselReserve   = summerDieselReserveInt;
                        unit.benzine80Reserve      = benzine80ReserveInt;
                        unit.isOperational         = isOperational;
                        context.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
            
        }


    }
}
