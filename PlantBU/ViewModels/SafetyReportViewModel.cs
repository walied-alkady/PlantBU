// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 06-09-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-09-2021
// ***********************************************************************
// <copyright file="LogViewModel.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using PlantBU.DataModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PlantBU.ViewModels
{
    class SafetyReportViewModel : BaseViewModel
    {

        public SafetyReport SafetyReport { get { return _SafetyReport; } set { _SafetyReport = value; OnPropertyChanged("SafetyReport"); } }
        SafetyReport _SafetyReport;
        public List<string> SourceOfIssues { get { return _SourceOfIssues; } set { _SourceOfIssues = value; OnPropertyChanged("SourceOfIssues"); } }
        List<string> _SourceOfIssues;
        public List<string> IssueOnCompanies { get { return _IssueOnCompanies; } set { _IssueOnCompanies = value; OnPropertyChanged("IssueOnCompanies"); } }
        List<string> _IssueOnCompanies;
        public List<string> IssueOnCompanyNames { get { return _IssueOnCompanyNames; } set { _IssueOnCompanyNames = value; OnPropertyChanged("IssueOnCompanyNames"); } }
        List<string> _IssueOnCompanyNames;
        public List<string> Departments { get { return _Departments; } set { _Departments = value; OnPropertyChanged("Departments"); } }
        List<string> _Departments;
        public List<string> ElectricalEmployeesList { get { return _ElectricalEmployeesList; } set { _ElectricalEmployeesList = value; OnPropertyChanged("ElectricalEmployeesList"); } }
        List<string> _ElectricalEmployeesList;
        public List<string> Areas { get { return _Areas; } set { _Areas = value; OnPropertyChanged("Areas"); } }
        List<string> _Areas;
        public List<string> Types { get { return _Types; } set { _Types = value; OnPropertyChanged("Types"); } }
        List<string> _Types;
        public List<string> ViolationTypes { get { return _ViolationTypes; } set { _ViolationTypes = value; OnPropertyChanged("ViolationTypes"); } }
        List<string> _ViolationTypes;
        public List<string> RiskLevels { get { return _RiskLevels; } set { _RiskLevels = value; OnPropertyChanged("RiskLevels"); } }
        List<string> _RiskLevels;
        public List<string> During { get { return _During; } set { _During = value; OnPropertyChanged("During"); } }
        List<string> _During;
        public int PhotosNum { get { return _PhotosNum; } set { _PhotosNum = value; OnPropertyChanged("PhotosNum"); } }
        int _PhotosNum;
        public SafetyReportViewModel()
        {
            IsBusy = true;
            IsEnabled = true;
            SourceOfIssues = new List<string>(){
                "تقرير خطورة Safety Alert" ,
                "  حادثIncident" ,
                "  مراجعة داخليةInternal Audit" ,
                "  مراجعة ربع سنويةQuarter Audit" ,
                "  مراجعة 360360 Audit" ,
                "  لجنة السلامةSafety committ" ,
                "  زائرVisitor" ,
                "  خطة الطوارئEmergency plan",
                "  قياسات بيئيةEnv. Measurements",
                " تقييم مخاطرRisk Assessment",
                "  متطلب قانونيLegal Requirements" , 
                "  مراجعة ادارةManagement Review",
                "   جولة السلامةSafety Walk",
                "فحص روتينيRout. Insp."}; SourceOfIssues.Sort();
             IssueOnCompanies = new List<string>() {
               "Titan",
               "Contractor"
            }; IssueOnCompanies.Sort();
            IssueOnCompanyNames = new List<string>() {
                "Titan",
"Global",
"Shield",
"Egymint",
"Senosy",
"Igess",
"Nabil Abbas",
"Samaras",
"Family",
"Yathreb",
"IBS",
"EL-Saify",
"First Line",
"Orbit",
"Asic",
"Etfaa",
"EL-Gazaarya",
"Ezz",
"Future",
"EL-Maktab EL-Araby",
"East",
"Castle",
"Weld&Wear",
"Hi Force",
"cic",
"Hama",
"Horizon",
"Other",

                         }; IssueOnCompanyNames.Sort();
            Departments = new List<string>() {
              "Plant_Management",
"Safety",
"Environment",
"Production",
"Mechanical",
"Electrical",
"PM",
"Quality",
"Stores",
"HR",
"IT",
"Purchasing",
"Admin&Finance",
"Projects",
"Security",
"Sales",
"Legal",
"Other",


            }; Departments.Sort();
            Areas = new List<string>() {"Quarry",
"Main mazzot tank",
"Raw mill",
"Preheater/Kiln",
"Daily mazzot tank",
"Fuel station",
"Packing",
"Coal mill",
"DSS",
"Gypsum crusher",
"Clay crusher",
"Warehouse",
"Admin Building",
"CCR Building",
"Technical Building",
"Lime stone crusher",
"Mechanical workshop ",
"Electrical workshop",
"Ammonia tank",
"Bypass",
"Cooler",
"Pre blending area",
"Cement mills",
"Electric stations",
"External quarries",
"Utilities",
"Containers",
"Site",
"Coal storage",
"Samares Workshop",
"Mobile Equp.",
"Clinic&Ambulance",
"Mosque",
"Medical Admin",
"Electrical Tunnel",
"Palamatic",
"Water Stations",
"Other",
   }; Areas.Sort();
            Types = new List<string>()
            {
               "Unsafe_Behavior",
"Unsafe_Condition",
"NM",
"FA",
"MTC",
"LTI",
"Fatality",
"NM",

            }; Types.Sort();
            RiskLevels = new List<string>()
            {
                "Low",
"Medium",
"High",

            }; 
            During = new List<string>()
            {
             "Normal Operation",
"Shut Down",
            }; During.Sort();
            ElectricalEmployeesList = new List<string>()
            {
             "سيد ذكى عيسوي احمد",
"محمد حسين ابو القاسم جاد المولي",
"راضى محمد احمد محمد",
"خليل ابراهيم عبد الصمد محمد",
"اشرف عبد العاطي مهلهل معوض",
"رجب عويس محمود احمد",
"عادل سيد احمد علي",
"خالد محمد حسين عثمان",
"عصام محمود السيد خميس",
"ابراهيم احمد محمد محمد",
"محمود محمود عبد الوهاب",
"رمضان محمد محمد عيسي",
"مرزوق كردى علي عويس",
"احمد عبد العليم عبد اللطيف",
"محمد جمعة عبد اللطيف",
"محمد احمد عبد النعيم",
"هانى رجب محمد احمد",
"وليد علي زين العابدين رياض",
"ياسر عبد الله محمود محمد",

            }; ElectricalEmployeesList.Sort();
            /*SourceOfIssues = new List<string>(){
                Properties.Resources.SafetyAlert,
                Properties.Resources.Incident,
                Properties.Resources.InternalAudit,
                Properties.Resources.QuarterAudit,
                Properties.Resources._360Audit,
                Properties.Resources.SafetyCommitt,
                Properties.Resources.Visitor,
                Properties.Resources.Emergencyplan,
                Properties.Resources.EnvMeasurements,
                Properties.Resources.LegalRequirements,
                Properties.Resources.ManagementReview,
                Properties.Resources.SafetyWalk,
                Properties.Resources.RoutInsp
            };
            IssueOnCompanies = new List<string>() {
               Properties.Resources.Titan,
               Properties.Resources.Contractor
            };
            IssueOnCompanyNames = new List<string>() {
               Properties.Resources.Titan,
Properties.Resources.Global,
Properties.Resources.Shield,
Properties.Resources.Egymint,
Properties.Resources.Senosy,
Properties.Resources.Igess,
Properties.Resources.NabilAbbas,
Properties.Resources.Samaras,
Properties.Resources.Family,
Properties.Resources.Yathreb,
Properties.Resources.IBS,
Properties.Resources.ELSaify,
Properties.Resources.FirstLine,
Properties.Resources.Orbit,
Properties.Resources.Asic,
Properties.Resources.Etfaa,
Properties.Resources.ELGazaarya,
Properties.Resources.Ezz,
Properties.Resources.Future,
Properties.Resources.ELMaktabELAraby,
Properties.Resources.East,
Properties.Resources.Castle,
Properties.Resources.WeldWear,
Properties.Resources.HiForce,
Properties.Resources.cic,
Properties.Resources.Hama,
Properties.Resources.Horizon,
Properties.Resources.Other,            };
            Departments = new List<string>() {
               Properties.Resources.Plant_Management,
Properties.Resources.Safety,
Properties.Resources.Environment,
Properties.Resources.Production,
Properties.Resources.Mechanical,
Properties.Resources.Electrical,
Properties.Resources.PM,
Properties.Resources.Quality,
Properties.Resources.Stores,
Properties.Resources.HR,
Properties.Resources.IT,
Properties.Resources.Purchasing,
Properties.Resources.AdminFinance,
Properties.Resources.Projects,
Properties.Resources.Security,
Properties.Resources.Sales,
Properties.Resources.Legal,
Properties.Resources.Other

            };
            Areas = new List<string>() {
            Properties.Resources.Quarry,
Properties.Resources.MainMazzotTank,
Properties.Resources.RawMill,
Properties.Resources.PreheaterKiln,
Properties.Resources.DailyMazzotTank,
Properties.Resources.FuelStation,
Properties.Resources.Packing,
Properties.Resources.CoalMill,
Properties.Resources.DSS,
Properties.Resources.GypsumCrusher,
Properties.Resources.ClayCrusher,
Properties.Resources.Warehouse,
Properties.Resources.AdminBuilding,
Properties.Resources.CCRBuilding,
Properties.Resources.TechnicalBuilding,
Properties.Resources.LimeStoneCrusher,
Properties.Resources.MechanicalWorkshop ,
Properties.Resources.ElectricalWorkshop,
Properties.Resources.AmmoniaTank,
Properties.Resources.Bypass,
Properties.Resources.Cooler,
Properties.Resources.PreBlendingArea,
Properties.Resources.CementMills,
Properties.Resources.ElectricStations,
Properties.Resources.ExternalQuarries,
Properties.Resources.Utilities,
Properties.Resources.Containers,
Properties.Resources.Site,
Properties.Resources.CoalStorage,
Properties.Resources.SamaresWorkshop,
Properties.Resources.MobileEqup,
Properties.Resources.ClinicAmbulance,
Properties.Resources.Mosque,
Properties.Resources.MedicalAdmin,
Properties.Resources.ElectricalTunnel,
Properties.Resources.Palamatic,
Properties.Resources.WaterStations,
Properties.Resources.Other
            };
            Types = new List<string>()
            {
                Properties.Resources.UnsafeBehavior,
                Properties.Resources.UnsafeCondition,
                Properties.Resources.NM,
                Properties.Resources.FA,
                Properties.Resources.MTC,
                Properties.Resources.LTI,
                Properties.Resources.Fatality
            };
            ViolationTypes = new List<string>()
            {
                Properties.Resources.ImproperDesign,
Properties.Resources.LackOfMaintenance,
Properties.Resources.LackOfInspection,
Properties.Resources.ImproperWorkEnvironment,
Properties.Resources.ImproperErgonomics,
Properties.Resources.AbsenceImproperOfSafetyDevice,
Properties.Resources.LackOfResources,
Properties.Resources.PoorHousekeeping,
Properties.Resources.ImproperMissingOfBarricadingGuarding,
Properties.Resources.UnnecessaryScaf,
Properties.Resources.UnsafeStorage,
Properties.Resources.FireHazard,
Properties.Resources.FallingObjects

            };
            RiskLevels = new List<string>()
            {
                Properties.Resources.Low,
Properties.Resources.Medium,
Properties.Resources.High


            };
            During = new List<string>()
            {
             Properties.Resources.NormalOperation,
Properties.Resources.ShutDown,

            };*/

            IsBusy = false;
        }

    }
}