// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 04-12-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 05-09-2021
// ***********************************************************************
// <copyright file="ExcelUtil.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using CsvHelper;
using CsvHelper.Configuration;
using PlantBU.Utilities;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PlantBU.DataModel
{
    /// <summary>
    /// Class ExcelUtil.
    /// </summary>
    public class CSVs
    {
        /// <summary>
        /// CSVs the get data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename">The filename.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public static List<T> GetData<T>(string filename)
        {
            var xx = typeof(T).Name;

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "PlantBU.CSVs." + filename + ".csv";
            var x = assembly.GetManifestResourceNames();
            var config = new CsvConfiguration(new CultureInfo("en"))
            {
                HasHeaderRecord = true,
            };
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                if (reader != null)
                {
                    using (var csv = new CsvReader(reader, config))
                    {
                        switch (xx)
                        {
                            case "Plant":
                                csv.Context.RegisterClassMap<PlantMap>();
                                break;
                            case "ProductionLine":
                                csv.Context.RegisterClassMap<ProductionLineMap>();
                                break;
                            case "Shop":
                                csv.Context.RegisterClassMap<ShopMap>();
                                break;
                            case "Equipment":
                                csv.Context.RegisterClassMap<EquipmentMap>();
                                break;
                            case "Motor":
                                csv.Context.RegisterClassMap<MotorMap>();
                                break;
                            case "Sensor":
                                csv.Context.RegisterClassMap<SensorMap>();
                                break;
                            case "OtherComponent":
                                csv.Context.RegisterClassMap<OtherComponentMap>();
                                break;
                            case "Spare":
                                csv.Context.RegisterClassMap<InventorySpareMap>();
                                break;
                            case "Schedule":
                                csv.Context.RegisterClassMap<ScheduleMap>();
                                break;
                            case "SparePart":
                                csv.Context.RegisterClassMap<SparePartMap>();
                                break;
                            default:
                                return null;
                        }
                        return csv.GetRecords<T>().ToList();
                    }
                }
            }
            return null;
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'CSVs.GetStreamData<T>(Stream)'
        public static List<T> GetStreamData<T>(Stream filestream)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'CSVs.GetStreamData<T>(Stream)'
        {
            var xx = typeof(T).Name;
            var config = new CsvConfiguration(new CultureInfo("en"))
            {
                HasHeaderRecord = true,
            };
            using (StreamReader reader = new StreamReader(filestream))
            {
                if (reader != null)
                {
                    using (var csv = new CsvReader(reader, config))
                    {
                        switch (xx)
                        {
                            case "Plant":
                                csv.Context.RegisterClassMap<PlantMap>();
                                break;
                            case "ProductionLine":
                                csv.Context.RegisterClassMap<ProductionLineMap>();
                                break;
                            case "Shop":
                                csv.Context.RegisterClassMap<ShopMap>();
                                break;
                            case "Equipment":
                                csv.Context.RegisterClassMap<EquipmentMap>();
                                break;
                            case "Motor":
                                csv.Context.RegisterClassMap<MotorMap>();
                                break;
                            case "Sensor":
                                csv.Context.RegisterClassMap<SensorMap>();
                                break;
                            case "OtherComponent":
                                csv.Context.RegisterClassMap<OtherComponentMap>();
                                break;
                            case "Spare":
                                csv.Context.RegisterClassMap<InventorySpareMap>();
                                break;
                            case "Schedule":
                                csv.Context.RegisterClassMap<ScheduleMap>();
                                break;
                            case "SparePart":
                                csv.Context.RegisterClassMap<SparePartMap>();
                                break;
                            case "ScheduleSparePart":
                                csv.Context.RegisterClassMap<ScheduleSparePartMap>();
                                break;
                            case "ScheduleSparePartImport":
                                csv.Context.RegisterClassMap<ScheduleSparePartMap>();
                                break;
                            default:
                                return null;
                        }
                        return csv.GetRecords<T>().ToList();
                    }
                }
            }
            return null;
        }
    }

    #region Class maps
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'PlantMap'
    public class PlantMap : ClassMap<Plant>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'PlantMap'
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentMap"/> class.
        /// </summary>
        public PlantMap()
        {
            Map(l => l.Id).Name("Id").Ignore();
            Map(l => l.Partition).Name("Partition").Default("Public");
            Map(l => l.Code).Name("Code");
            Map(l => l.Description).Name("Description");
            Map(l => l.Location).Name("Location");
        }
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ProductionLineMap'
    public class ProductionLineMap : ClassMap<ProductionLine>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ProductionLineMap'
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentMap"/> class.
        /// </summary>
        public ProductionLineMap()
        {
            Map(l => l.Id).Name("Id").Ignore();
            Map(l => l.Partition).Name("Partition").Default("Public");
            Map(l => l.Code).Name("Code");
            Map(l => l.Description).Name("Description");
            Map(l => l.LineNo).Name("LineNo");
            Map(l => l.ExtraData).Name("ExtraData");
        }
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'EquipmentMap'
    public class EquipmentMap : ClassMap<Equipment>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'EquipmentMap'
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentMap"/> class.
        /// </summary>
        public EquipmentMap()
        {
            Map(l => l.Id).Name("Id").Ignore();
            Map(l => l.Partition).Name("Partition").Default("Public");
            Map(l => l.Code).Name("Code").Validate(field => RegexLib.IsEquipmentCode(field.Field));
            Map(l => l.Description).Name("Description");
            Map(l => l.Type).Name("Type");
            Map(l => l.Extra).Name("Extra");
            Map(l => l.Shop).Name("Shop");
            Map(l => l.ShopExtra).Name("ShopExtra");
            Map(l => l.Brand).Name("Brand");
            Map(l => l.BrandType).Name("BrandType");
        }
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ShopMap'
    public class ShopMap : ClassMap<Shop>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ShopMap'
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentMap"/> class.
        /// </summary>
        public ShopMap()
        {
            Map(l => l.Id).Name("Id").Ignore();
            Map(l => l.Partition).Name("Partition").Default("Public");
            Map(l => l.ShopName).Name("ShopName");
            Map(l => l.ShopDescription).Name("ShopDescription");
        }
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'MotorMap'
    public class MotorMap : ClassMap<Motor>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'MotorMap'
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MotorMap"/> class.
        /// </summary>
        public MotorMap()
        {
            Map(l => l.Id).Name("Id").Ignore();
            Map(l => l.Partition).Name("Partition").Default("Public");
            Map(l => l.Code).Name("Code").Validate(field => RegexLib.IsComponentCode(field.Field));
            Map(l => l.Description).Name("Description");
            Map(l => l.Brand).Name("Brand");
            Map(l => l.BrandType).Name("BrandType");
            Map(l => l.ExtraData).Name("ExtraData");
            Map(l => l.SupplyPanel).Name("SupplyPanel");
            Map(l => l.supplyPanelLoc).Name("supplyPanelLoc");
            Map(l => l.Power).Name("Power").Default(0).Validate(field => RegexLib.IsFloat(field.Field));
            Map(l => l.Frequency).Name("Frequency").Default(0).Validate(field => RegexLib.IsFloat(field.Field));
            Map(l => l.Poles).Name("Poles").Default(0).Validate(field => RegexLib.IsInteger(field.Field));
            Map(l => l.Volt).Name("Volt").Default(0).Validate(field => RegexLib.IsFloat(field.Field));
            Map(l => l.Current).Name("Current").Default(0).Validate(field => RegexLib.IsFloat(field.Field));
            Map(l => l.RPM).Name("RPM").Default(0).Validate(field => RegexLib.IsFloat(field.Field));
            Map(l => l.CosPhi).Name("CosPhi").Default(0).Validate(field => RegexLib.IsFloat(field.Field));
            Map(l => l.Effeciency).Name("Effeciency").Default(0).Validate(field => RegexLib.IsFloat(field.Field));
            Map(l => l.Mounting).Name("Mounting");
            Map(l => l.FrameSize).Name("FrameSize");
            Map(l => l.Cooling).Name("Cooling");
            Map(l => l.Duty).Name("Duty");
            Map(l => l.ThermalClass).Name("ThermalClass");
            Map(l => l.IP).Name("IP").Default(0).Validate(field => RegexLib.IsFloat(field.Field));
            Map(l => l.BearingDE).Name("BearingDE");
            Map(l => l.BearingNDE).Name("BearingNDE");
            Map(l => l.GreaseBrand).Name("GreaseBrand");
            Map(l => l.GreaseBrandType).Name("GreaseBrandType");
            Map(l => l.IntervalDays).Name("IntervalDays").Default(0).Validate(field => RegexLib.IsFloat(field.Field));
            Map(l => l.QtyDE).Name("QtyDE").Default(0).Validate(field => RegexLib.IsFloat(field.Field));
            Map(l => l.QtyNDE).Name("QtyNDE").Default(0).Validate(field => RegexLib.IsFloat(field.Field));
            Map(l => l.Weight).Name("Weight").Default(0).Validate(field => RegexLib.IsFloat(field.Field));

        }
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SensorMap'
    public class SensorMap : ClassMap<Sensor>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SensorMap'
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SensorMap"/> class.
        /// </summary>
        public SensorMap()
        {
            Map(l => l.Id).Name("Id").Ignore();
            Map(l => l.Partition).Name("Partition").Default("Public");
            Map(l => l.Code).Name("Code").Validate(field => RegexLib.IsComponentCode(field.Field));
            Map(l => l.Description).Name("Description");
            Map(l => l.Brand).Name("Brand");
            Map(l => l.BrandType).Name("BrandType");
            Map(l => l.InstrumentType).Name("InstrumentType");
            Map(l => l.SignalType).Name("SignalType");
            Map(l => l.SupplyVoltage).Name("SupplyVoltage");
            Map(l => l.Wiring).Name("Wiring");
            Map(l => l.Communication).Name("Communication");
            Map(l => l.GalvanicIsolation).Name("GalvanicIsolation");
            Map(l => l.OperationalUnit).Name("OperationalUnit");
            Map(l => l.ScaleRange).Name("ScaleRange");
            Map(l => l.LimitHH).Name("LimitHH");
            Map(l => l.LimitH).Name("LimitH");
            Map(l => l.LimitL).Name("LimitL");
            Map(l => l.LimitLL).Name("LimitLL");
            Map(l => l.Softawre).Name("Softawre");
            Map(l => l.ExtraData).Name("ExtraData");
        }
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'OtherComponentMap'
    public class OtherComponentMap : ClassMap<OtherComponent>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'OtherComponentMap'
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OtherComponentMap"/> class.
        /// </summary>
        public OtherComponentMap()
        {
            Map(l => l.Id).Name("Id").Ignore();
            Map(l => l.Partition).Name("Partition").Default("Public");
            Map(l => l.Code).Name("Code").Validate(field => RegexLib.IsComponentCode(field.Field));
            Map(l => l.Description).Name("Description");
            Map(l => l.Brand).Name("Brand");
            Map(l => l.BrandType).Name("BrandType");
            Map(l => l.ExtraData).Name("ExtraData");
        }
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SparePartMap'
    public class SparePartMap : ClassMap<SparePart>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SparePartMap'
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SensorMap"/> class.
        /// </summary>
        public SparePartMap()
        {
            Map(l => l.Id).Name("Id").Ignore();
            Map(l => l.Partition).Name("Partition").Default("Public");
            Map(l => l.ItemCode).Name("CodeItem");
            Map(l => l.InventoryCode).Name("CodeInventory");
            Map(l => l.Description1).Name("Description1");
            Map(l => l.Description2).Name("Description2");
            Map(l => l.QtyRequired).Name("Qty");

        }
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'InventorySpareMap'
    public class InventorySpareMap : ClassMap<Spare>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'InventorySpareMap'
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InventorySpareMap"/> class.
        /// </summary>
        public InventorySpareMap()
        {
            Map(l => l.Code).Name("Code").Validate(field => RegexLib.IsInventoryCode(field.Field));
            Map(l => l.Description1).Name("Description1");
            Map(l => l.Description2).Name("Description2");
            Map(l => l.Brand).Name("Brand");
            Map(l => l.BrandType).Name("BrandType");
            Map(l => l.Location).Name("Location");
            Map(l => l.OnHand).Name("OnHand").Default(0).Validate(field => RegexLib.IsFloat(field.Field));
            Map(l => l.Unit).Name("Unit");
            Map(l => l.Value).Name("Value").Default(0).Validate(field => RegexLib.IsFloat(field.Field));
            Map(l => l.TrackItem).Name("TrackItem").Default(false).Validate(field => RegexLib.IsBool(field.Field));
            Map(l => l.MinQty).Name("MinQty").Default(0).Validate(field => RegexLib.IsFloat(field.Field));
            Map(l => l.ExtraData).Name("ExtraData");
        }

    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ScheduleMap'
    public class ScheduleMap : ClassMap<Schedule>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ScheduleMap'
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleMap"/> class.
        /// </summary>
        public ScheduleMap()
        {
            Map(l => l.Id).Name("Id").Ignore();
            Map(l => l.Partition).Name("Partition").Default("Public");
            Map(l => l.Area).Name("Area").Default("Other");
            Map(l => l.ItemCode).Name("ItemCode").Validate(field => RegexLib.IsComponentCode(field.Field));
            Map(l => l.ItemDescription).Name("ItemDescription");
            Map(l => l.Repair).Name("Repair");
            Map(l => l.Repairdetails).Name("Repairdetails");
            Map(l => l.RepairCost).Name("RepairCost");
            Map(l => l.Notes).Name("Notes");
            Map(l => l.SetDate).Name("SetDate").Validate(field => RegexLib.IsDate(field.Field));
            Map(l => l.DateScheduleFrom).Name("DateScheduleFrom").Validate(field => RegexLib.IsDate(field.Field));
            Map(l => l.DateScheduleTo).Name("DateScheduleTo").Validate(field => RegexLib.IsDate(field.Field));
            Map(l => l.StatusSchedule).Name("StatusSchedule").Validate(field => RegexLib.IsBool(field.Field));
            Map(l => l.AssigneeCompanyCode).Name("AssigneeCompanyCode");

        }
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePartMap'
    public class ScheduleSparePartMap : ClassMap<ScheduleSparePartImport>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePartMap'
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SensorMap"/> class.
        /// </summary>
        public ScheduleSparePartMap()
        {
            Map(l => l.CodeItem).Name("CodeItem");
            Map(l => l.InventoryCode).Name("CodeInventory");
            Map(l => l.QtyRequired).Name("QtyRequired");
        }
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePartImportMap'
    public class ScheduleSparePartImportMap : ClassMap<ScheduleSparePartImport>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePartImportMap'
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SensorMap"/> class.
        /// </summary>
        public ScheduleSparePartImportMap()
        {
            Map(l => l.CodeItem).Name("CodeItem");
            Map(l => l.InventoryCode).Name("CodeInventory");
            Map(l => l.QtyRequired).Name("QtyRequired");
        }
    }

    #endregion
}