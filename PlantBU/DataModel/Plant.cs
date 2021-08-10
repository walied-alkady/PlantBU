// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 04-12-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-09-2021
// ***********************************************************************
// <copyright file="Plant.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using MongoDB.Bson;
using PlantBU.DataModel.Annotations;
using Realms;
using Realms.Sync;
using Realms.Sync.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace PlantBU.DataModel
{
    public static class DBManager
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'DBManager'
    {
        #region Properties
        /// <summary>
        /// Gets or sets the realm.
        /// </summary>
        /// <value>The realm.</value>
        public static Realm realm { get; set; }
        /// <summary>Gets or sets the realm local.</summary>
        /// <value>The realm local.</value>
        public static Realm realmLocal { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is d bconnected.
        /// </summary>
        /// <value><c>true</c> if this instance is d bconnected; otherwise, <c>false</c>.</value>
        public static bool IsDBconnected { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is database loaded.
        /// </summary>
        /// <value><c>true</c> if this instance is database loaded; otherwise, <c>false</c>.</value>
        public static bool IsDBLoaded { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is inventory loaded.
        /// </summary>
        /// <value><c>true</c> if this instance is inventory loaded; otherwise, <c>false</c>.</value>
        public static bool IsInventoryLoaded { get { return realm.All<Spare>().Any(); } }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'DBManager.CurrentUser'
        public static Employee CurrentUser { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'DBManager.CurrentUser'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'DBManager.CurrentUserType'
        public static Usertyypes CurrentUserType { get; set; } = Usertyypes.User;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'DBManager.CurrentUserType'

        /// <summary>
        /// Gets a value indicating whether this instance is database loaded with data.
        /// </summary>
        /// <value><c>true</c> if this instance is database loaded with data; otherwise, <c>false</c>.</value>
        public static bool IsDBLoadedWithData
        {
            get
            {
                if (realm.All<Equipment>().Count() >0)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// Gets the session status.
        /// </summary>
        /// <value>The session status.</value>
        public static string SessionStatus
        {
            get
            {
                if (realm.GetSession().State == SessionState.Active)
                {
                    IsDBconnected = true;
                    return Properties.Resources.Active;
                }
                else
                    IsDBconnected = false;
                return Properties.Resources.InActive;
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'DBManager.IsDBReady'
        public static bool IsDBReady
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'DBManager.IsDBReady'
        {
            get
            {
                if (realm != null)
                    return true;
                else
                    return false;
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'DBManager.LogsChanged'
        public static IDisposable LogsChanged { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'DBManager.LogsChanged'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'DBManager.SchedulessChanged'
        public static IDisposable SchedulessChanged { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'DBManager.SchedulessChanged'
        #endregion

        /// <summary>
        /// Connects the local.
        /// </summary>
        public static void ConnectLocal()
        {
            IsDBconnected = false;
            var conf = new RealmConfiguration("Titan.realm"); // { SchemaVersion = 1 }
            conf.ShouldDeleteIfMigrationNeeded = true;
            realm = Realm.GetInstance(conf);
            IsDBconnected = true;
        }
        /// <summary>
        /// Connects the local defaults.
        /// </summary>
        public static void ConnectLocalDefaults()
        {
            IsDBconnected = false;
            realm.Dispose();
            var conf = new RealmConfiguration("Titan.realm");
            Realm.DeleteRealm(conf);
            var conf1 = new RealmConfiguration("Titan.realm");
            realm = Realm.GetInstance(conf1);
            IsDBconnected = true;
        }
       
        /// <summary>
        /// Backs up.
        /// </summary>
        public static void BackUp()
        {
            var confNew = new RealmConfiguration("Titan" + "_" +
                (DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString()) + "." +
                (DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString()) + "." +
                DateTime.Now.Year.ToString() + "." +
                (DateTime.Now.Hour < 10 ? "0" + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString()) + "." +
                (DateTime.Now.Minute < 10 ? "0" + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString()) + "." +
                "realm"); //
            realm.WriteCopy(confNew);
        }
        /// <summary>
        /// Restores the specified conf backup.
        /// </summary>
        /// <param name="confBak">The conf backup.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Restore(RealmConfiguration confBak)
        {

            if (!realm.IsInTransaction)
            {
                realm.Dispose();
                Realm.DeleteRealm(realm.Config);
                CopyBundledRealmFile(confBak.DatabasePath, realm.Config.DatabasePath);
                ConnectLocal();
                return true;
            }
            return false;

        }
        /// <summary>
        /// Copies the bundled realm file.
        /// </summary>
        /// <param name="oldFilePath">The old file path.</param>
        /// <param name="outFileName">Name of the out file.</param>
        /// <returns>System.String.</returns>
        private static string CopyBundledRealmFile(String oldFilePath, String outFileName)
        {
            byte[] byts = File.ReadAllBytes(oldFilePath);
            //var fs = new FileStream(oldFilePath, FileMode.Open);
            using (var fs1 = new FileStream(outFileName, FileMode.Create, FileAccess.Write))
            {
                fs1.Write(byts, 0, byts.Length);
                return outFileName;
            }
        }
        /// <summary>
        /// Connects the cloud.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>Task.</returns>
        public async static Task ConnectCloud(string filePath = "")
        {
            var app = Realms.Sync.App.Create("titanbu-tokvg");
            string apiKey = "oKsrKyZi7CwBkUDwGhfnLTZKppyO9L25vBbEE3Vp4WB0kMEbqUiyJgxoPGHtDkzw";
            var user = await app.LogInAsync(Credentials.ApiKey(apiKey));
            var config = new Realms.Sync.SyncConfiguration("Public", user);

            realm = Realm.GetInstance(config);
            await realm.GetSession().WaitForDownloadAsync();
            //var x = realm.GetSession().State;
            // var session = realm.GetSession();
            
            //DBInitialize();

            /*  var conf = new RealmConfiguration("TitanInventory.realm"); // { SchemaVersion = 1 }
              //conf.ShouldDeleteIfMigrationNeeded = true;
              realmLocal = Realm.GetInstance(conf);*/



            /* var conf = new RealmConfiguration("Titan.realm");
             //var confdef= new RealmConfiguration("default.realm");
             Realm.DeleteRealm(conf);
             //Realm.DeleteRealm(confdef);
             realm = Realm.GetInstance(conf);
             DBInitialize();
             //Get the db pathC
             string dbFullPath = realm.Config.DatabasePath;*/
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'DBManager.ConnectCloudTest(string)'
        public async static Task ConnectCloudTest(string filePath = "")
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'DBManager.ConnectCloudTest(string)'
        {
            /*var RealmApp = Realms.Sync.App.Create("tce - hnjyh");
            var user = await RealmApp.LogInAsync(Credentials.Anonymous());
            var userRealm = await Realm.GetInstanceAsync(syncConfig);*/
            //user = userRealm.Find<User>(RealmApp.CurrentUser.Id);
            try
            {

                var myRealmAppId = "application-0-qujfw";
                var appConfig = new AppConfiguration(myRealmAppId)
                {
                    //LogLevel = LogLevel.Debug,
                    DefaultRequestTimeout = TimeSpan.FromMilliseconds(1500)
                };
                var app = Realms.Sync.App.Create(appConfig);
                var user = await app.LogInAsync(Credentials.Anonymous());
                var config = new Realms.Sync.SyncConfiguration("Public", user);

                realm = Realm.GetInstance(config);
                await realm.GetSession().WaitForDownloadAsync();
                //var x = realm.GetSession().State;
                // var session = realm.GetSession();
                Session.Error += (session, errorArgs) =>
                {
                    var sessionException = (SessionException)errorArgs.Exception;
                    switch (sessionException.ErrorCode)
                    {
                        case ErrorCode.AccessTokenExpired:

                        case ErrorCode.BadUserAuthentication:
                            // Ask user for credentials
                            break;
                        case ErrorCode.PermissionDenied:
                            // Tell the user they don't have permissions to work with that Realm
                            break;
                        case ErrorCode.Unknown:
                            // Likely the app version is too old, prompt for update
                            break;
                            // ...
                    }
                };
                var session1 = realm.GetSession();
                var token = session1.GetProgressObservable(ProgressDirection.Upload, ProgressMode.ReportIndefinitely)
                    .Subscribe(progress =>
                    {
                        Console.WriteLine($"transferred bytes: {progress.TransferredBytes}");
                        Console.WriteLine($"transferable bytes: {progress.TransferableBytes}");
                    });
                realm.RealmChanged += (sender, eventArgs) =>
                {
                    realm.Refresh();
                    // sender is the realm that has changed.
                    // eventArgs is reserved for future use.
                    // ... update UI ...
                };
                // Observe collection notifications. Retain the token to keep observing.
                LogsChanged = realm.All<Log>().SubscribeForNotifications((sender1, changes, error) =>
                     {
                         LogChanges(changes);
                     });
                SchedulessChanged = realm.All<Schedule>().SubscribeForNotifications((sender1, changes, error) =>
                 {
                     ScheduleChanges(changes);
                 });
                /* realm.All<Schedule>()
                      .SubscribeForNotifications((sender1, changes, error) =>
                      {
                          if (changes != null)
                          {
                              int x = 0;
                              foreach (var i in changes.DeletedIndices)
                              {
                              }
                              foreach (var i in changes.InsertedIndices)
                              { Preferences.Set("NewSchedulesCount", x++); Preferences.Set("NewSchedule", 1); }

                              foreach (var i in changes.NewModifiedIndices)
                              { Preferences.Set("NewSchedulesCount", x++); Preferences.Set("NewSchedule", 1); }
                          }
                      });
  */

                // Later, when you no longer wish to receive notifications
                // token.Dispose();
                //DBInitialize();

                /*  var conf = new RealmConfiguration("TitanInventory.realm"); // { SchemaVersion = 1 }
                  //conf.ShouldDeleteIfMigrationNeeded = true;
                  realmLocal = Realm.GetInstance(conf);*/

            }
            catch (Exception ex)
            {
                throw new Exception("Connection Problem:" + ex.Message);
            }

            /* var conf = new RealmConfiguration("Titan.realm");
             //var confdef= new RealmConfiguration("default.realm");
             Realm.DeleteRealm(conf);
             //Realm.DeleteRealm(confdef);
             realm = Realm.GetInstance(conf);
             DBInitialize();
             //Get the db pathC
             string dbFullPath = realm.Config.DatabasePath;*/
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'DBManager.LogChanges(ChangeSet)'
        public static void LogChanges(ChangeSet changes)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'DBManager.LogChanges(ChangeSet)'
        {
            if (changes != null)
            {
                int x = 0;
                foreach (var i in changes.DeletedIndices)
                { }
                foreach (var i in changes.InsertedIndices)
                { x++; }
                foreach (var i in changes.NewModifiedIndices)
                { x++; }

                Preferences.Set("NewLogsCount", x + Preferences.Get("NewLogsCount", 0)); Preferences.Set("NewLog", 1);
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'DBManager.ScheduleChanges(ChangeSet)'
        public static void ScheduleChanges(ChangeSet changes)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'DBManager.ScheduleChanges(ChangeSet)'
        {
            if (changes != null)
            {
                int x = 0;
                foreach (var i in changes.DeletedIndices)
                { }
                foreach (var i in changes.InsertedIndices)
                { x++; }
                foreach (var i in changes.NewModifiedIndices)
                { x++; }

                Preferences.Set("NewSchedulesCount", x + Preferences.Get("NewSchedulesCount", 0)); Preferences.Set("NewSchedule", 1);
            }
        }
        /// <summary>
        /// Databases the defaults.
        /// </summary>
        /// <param name="progress">The progress.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
        public async static Task<int> DBDefaults(IProgress<int> progress)
        {
            CSVs ex = new CSVs();
            int totalCount; ;
            int processCount = await realm.WriteAsync<int>(realm =>
          {
              realm.RemoveAll();
              //Init BU
#pragma warning disable CS0168 // The variable 'bdget' is declared but never used
#pragma warning disable CS0168 // The variable 'exps' is declared but never used
              Plant BU; ProductionLine pl1, pl2; Inventory inv1; Budget bdget; Expenses exps;
#pragma warning restore CS0168 // The variable 'exps' is declared but never used
#pragma warning restore CS0168 // The variable 'bdget' is declared but never used
              if (realm.All<Plant>().Count() == 0)
              {
                  BU = new Plant() { Code = "TCE", Description = "Titan Egypt", Partition = "Public" };
                  realm.Add(BU, update: true);
              }
              else
                  BU = realm.All<Plant>().First();
                       //Init Prod lines
                       if (realm.All<Plant>().First().ProductionLines.Count == 0)
              {
                  pl1 = new ProductionLine() { Code = "BSF1", Description = "Line1", Partition = "Public" };
                  pl2 = new ProductionLine() { Code = "BSF2", Description = "Line2", Partition = "Public" };
                  BU.ProductionLines.Add(pl1);
                  BU.ProductionLines.Add(pl2);
              }
              else
              {
                  pl1 = BU.ProductionLines.Where(p => p.Code == "BSF1").First();
                  pl2 = BU.ProductionLines.Where(p => p.Code == "BSF2").First();
              }
              //Adding budget and expenses
                       if (realm.All<Plant>().First().Budget == null)
                  BU.Budget = new Budget();
              if (realm.All<Plant>().First().Expenses == null)
                  BU.Expenses = new Expenses();
              //Adding Lines list
                       realm.All<ProductionLine>().ToList()
              .Select(x => x.Code).Distinct()
              .ForEach(Ln => realm.Add(new ProductionLine() { Partition = "Public", LineNo = Ln }));
              //Adding Shops 
              List<Shop> ShopImports = CSVs.GetData<Shop>("Shops");
              foreach (Shop eq in ShopImports)
                {
                  realm.Add(eq);
                }
              //Adding Equipment
              List<Equipment> EQImports = CSVs.GetData<Equipment>("Equipments");
              List<Motor> MotorsImports = CSVs.GetData<Motor>("Motors");
              List<Sensor> SensorsImports = CSVs.GetData<Sensor>("Sensors");
              List<SparePart> SparePartsImports = CSVs.GetData<SparePart>("SpareParts");
                       //TODO: Component spares not implemented
                       totalCount = EQImports.Count + MotorsImports.Count + SparePartsImports.Count + SensorsImports.Count;
              int tempCount = 0;
              foreach (Equipment eq in EQImports)
              {
                  eq.Partition = "Public";
                           //Adding Motors
                           foreach (Motor mt in MotorsImports.Where(mm => mm.Code.Substring(0, 7) == eq.Code))
                  {
                               //Adding motor spares
                               foreach (SparePart sp in SparePartsImports.Where(mm => mm.ItemCode == mt.Code))
                      {
                          mt.SpareParts.Add(sp);
                      }
                      eq.Motors.Add(mt);
                      progress.Report((tempCount * 100 / totalCount));
                      tempCount++;
                  }
                           //Adding Sensors
                           foreach (Sensor mt in SensorsImports.Where(mm => mm.Code.Substring(0, 7) == eq.Code))
                  {
                      eq.Sensors.Add(mt);
                      progress.Report((tempCount * 100 / totalCount));
                      tempCount++;
                  }
                  if (eq.Extra == "1")
                  {
                      var eq1 = eq;
                      pl1.Equipments.Add(eq1);
                      progress.Report((tempCount * 100 / totalCount));
                      tempCount++;
                  }
                  else if (eq.Extra == "2")
                  {
                      var eq2 = eq;
                      pl2.Equipments.Add(eq2);
                      progress.Report((tempCount * 100 / totalCount));
                      tempCount++;
                  }
              }
                    /*   //Adding Shops list
                       var tests = realm.All<Equipment>().ToList()
              .Select(x => x.ShopExtra).Distinct().ToList();

              realm.All<Equipment>().ToList()
              .Select(x => x.ShopExtra).Distinct().ToList()
              .ForEach(sh => realm.Add(new Shop() { Partition = "Public", ShopName = sh }));
*/

                       //Init Inventory
                       if (realm.All<Plant>().First().Inventories.Count == 0)
              {
                  inv1 = new Inventory() { Code = "INV1", Description = "Inventory" };
                  BU.Inventories.Add(inv1);
              }
              return tempCount;

          });
            return processCount;
        }
        /// <summary>
        /// Databases the load inventory.
        /// </summary>
        /// <param name="progress">The progress.</param>
        /// <returns>Task&lt;System.Int32&gt;.</returns>
#pragma warning disable CS1573 // Parameter 'invCode' has no matching param tag in the XML comment for 'DBManager.DBLoadInventory(IProgress<int>, string)' (but other parameters do)
        public async static Task<int> DBLoadInventory(IProgress<int> progress, string invCode)
#pragma warning restore CS1573 // Parameter 'invCode' has no matching param tag in the XML comment for 'DBManager.DBLoadInventory(IProgress<int>, string)' (but other parameters do)
        {
            List<Spare> SpareImports = CSVs.GetData<Spare>("Inventory");
            int totalCount = SpareImports.Count;
            int processCount = await realm.WriteAsync<int>(realm =>
            {
                var inv1 = realm.All<Inventory>().Where(inv2=>inv2.Code == invCode).First();
                int tempCount = 0;
                foreach (Spare sps in SpareImports)
                {
                    Spare spadd = sps;
                    if (!inv1.Spares.Where(sp => sp.Code == spadd.Code).Any())
                        inv1.Spares.Add(spadd);
                    progress.Report((tempCount * 100 / totalCount));
                    tempCount++;
                }

                return tempCount;
            });
            return processCount;
        }
        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool AddItem<T>(T item)
        {
           
                var xx = typeof(T).Name;
            switch (xx)
            {
                case "Equipment":
                    realm.Write(() =>
                    {
                        realm.Add(item as Equipment, true);
                    });
                    return true;
                case "Motor":
                    realm.Write(() =>
                    {
                        realm.Add(item as Motor, true);
                    }); return true;
                case "Sensor":
                    realm.Write(() =>
                    {
                        realm.Add(item as Sensor, true);
                    }); return true;
                case "OtherComponent":
                    realm.Write(() =>
                    {
                        realm.Add(item as OtherComponent, true);
                    }); return true;
                case "Spare":
                    realm.Write(() =>
                    {
                        realm.Add(item as Spare, true);
                    }); return true;

                case "Schedule":
                    realm.Write(() =>
                    {
                        realm.Add(item as Schedule, true);
                    }); return true;
                case "Log":
                    realm.Write(() =>
                    {
                        realm.Add(item as Log, true);
                    }); return true;
                case "Employee":
                    realm.Write(() =>
                    {
                        realm.Add(item as Employee, true);
                    }); return true;
                case "ExpensItem":
                    realm.Write(() =>
                    {
                        if (DBManager.realm.All<Expenses>().Count() == 0)
                        {
                            var Bud = DBManager.realm.All<Plant>().First();
                            Expenses newExp = new Expenses();
                            Bud.Expenses = newExp;
                        }

                        var exp = DBManager.realm.All<Expenses>().First();
                        var expitem = item as ExpensItem;
                        exp.Items.Add(expitem);
                    }); return true;
                case "Budgetitem":
                    realm.Write(() =>
                    {
                        if (DBManager.realm.All<Budget>().Count() == 0)
                        {
                            var Bud1 = DBManager.realm.All<Plant>().First();
                            Budget newExp1 = new Budget();
                            Bud1.Budget = newExp1;
                        }
                        var Bud = DBManager.realm.All<Budget>().First();
                        var newExp = item as Budgetitem;
                        Bud.Items.Add(newExp);
                    }); return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool RemoveItem<T>(T item)
        {
            try
            {
                var xx = typeof(T).Name;
                switch (xx)
                {

                    case "Equipment":
                        var getEquipmentById = DBManager.realm.All<Equipment>().Where(x => x.Id == (item as Equipment).Id).First();
                        DBManager.realm.Write(() =>
                        {
                            realm.Remove(getEquipmentById);
                        });
                        return true;
                    case "Motor":
                        var getMotorById = DBManager.realm.All<Motor>().Where(x => x.Id == (item as Motor).Id).First();
                        DBManager.realm.Write(() =>
                        {
                            DBManager.realm.Remove(getMotorById);
                        });
                        return true;
                    case "Sensor":
                        var getSensorById = DBManager.realm.All<Sensor>().Where(x => x.Id == (item as Sensor).Id).First();
                        DBManager.realm.Write(() =>
                        {
                            DBManager.realm.Remove(getSensorById);
                        });
                        return true;
                    case "OtherComponent":
                        var getOtherComponentById = DBManager.realm.All<OtherComponent>().Where(x => x.Id == (item as OtherComponent).Id).First();
                        DBManager.realm.Write(() =>
                        {
                            DBManager.realm.Remove(getOtherComponentById);
                        });
                        return true;
                    case "Spare":
                        var getSpareById = DBManager.realm.All<Spare>().Where(x => x.Code == (item as Spare).Code).First();
                        DBManager.realm.Write(() =>
                        {
                            DBManager.realm.Remove(getSpareById);
                        });
                        return true;

                    case "Schedule":
                        var getScheduleById = DBManager.realm.All<Schedule>().Where(x => x.Id == (item as Schedule).Id).First();
                        DBManager.realm.Write(() =>
                        {
                            DBManager.realm.Remove(getScheduleById);
                        });
                        return true;
                    case "Log":
                        var getLogById = DBManager.realm.All<Log>().Where(x => x.Id == (item as Log).Id).First();
                        DBManager.realm.Write(() =>
                        {
                            DBManager.realm.Remove(getLogById);
                        });
                        return true;
                    case "Employee":
                        var getEmployeeById = DBManager.realm.All<Employee>().Where(x => x.Id == (item as Employee).Id).First();
                        DBManager.realm.Write(() =>
                        {
                            DBManager.realm.Remove(getEmployeeById);
                        });
                        return true;
                    case "ExpensItem":
                        var getExpensItemById = DBManager.realm.All<ExpensItem>().Where(x => x.Id == (item as ExpensItem).Id).First();
                        DBManager.realm.Write(() =>
                        {
                            DBManager.realm.Remove(getExpensItemById);
                        });
                        return true;
                    case "Budgetitem":
                        var getBudgetitemById = DBManager.realm.All<Budgetitem>().Where(x => x.Id == (item as Budgetitem).Id).First();
                        DBManager.realm.Write(() =>
                        {
                            DBManager.realm.Remove(getBudgetitemById);
                        });
                        return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'DBManager.Usertyypes'
        public enum Usertyypes
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'DBManager.Usertyypes'
        {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'DBManager.Usertyypes.Admin'
            Admin,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'DBManager.Usertyypes.Admin'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'DBManager.Usertyypes.User'
            User
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'DBManager.Usertyypes.User'
        }
    }
    public class Plant : RealmObject
    {
        #region Fields
        [PrimaryKey]
        [MapTo("_id")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.Id'
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.Id'
        [MapTo("partition")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.Partition'
        public string Partition { get; set; } = "Public";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.Partition'
        [MapTo("Code")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.Code'
        public string Code { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.Code'
        [MapTo("Budget")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.Budget'
        public Budget Budget { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.Budget'
        [MapTo("Expenses")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.Expenses'
        public Expenses Expenses { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.Expenses'

        [MapTo("Description")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.Description'
        public string Description { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.Description'
        [MapTo("Location")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.Location'
        public string Location { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.Location'
        [MapTo("ProductionLines")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.ProductionLines'
        public IList<ProductionLine> ProductionLines { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.ProductionLines'
        [MapTo("Inventories")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.Inventories'
        public IList<Inventory> Inventories { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.Inventories'
        [MapTo("Employees")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.Employees'
        public IList<Employee> Employees { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.Employees'
        [MapTo("Logs")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.Logs'
        public IList<Log> Logs { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.Logs'

        [MapTo("Schedules")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.Schedules'
        public IList<Schedule> Schedules { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.Schedules'
        [MapTo("Safety")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.Safety'
        public Safety Safety { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.Safety'


        #endregion
        #region Methods

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.Plant()'
        public Plant() { }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.Plant()'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.ProductionLineAdd(ProductionLine)'
        public void ProductionLineAdd(ProductionLine ProdLine)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.ProductionLineAdd(ProductionLine)'
        {
            try
            {
                if (ProductionLines.Where(x => x.Code == ProdLine.Code).Any())
                    throw new Exception("Item is repeated");
                DBManager.realm.Write(() =>
                {
                    ProductionLines.Add(ProdLine);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.ProductionLineAdd(string)'
        public void ProductionLineAdd(string Code)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.ProductionLineAdd(string)'
        {
            try
            {
                if (ProductionLines.Where(x => x.Code == Code).Any())
                    throw new Exception("Item is repeated");
                DBManager.realm.Write(() =>
                {
                    ProductionLines.Add(new ProductionLine() { Code = Code });
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.ProductionLineRemove(ProductionLine)'
        public void ProductionLineRemove(ProductionLine ProdLine)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.ProductionLineRemove(ProductionLine)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    DBManager.realm.Remove(ProdLine);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //TODO: Modify inventory
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.InventoryAdd(string, string)'
        public void InventoryAdd(string code, string description)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.InventoryAdd(string, string)'
        {
            try
            {
                if (Inventories.Where(x => x.Code == Code).Any())
                    throw new Exception("Item is repeated");
                Inventory inv = new Inventory();
                inv.Code = code;
                inv.Description = description;
                DBManager.realm.Write(() =>
                {
                    Inventories.Add(inv);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.InventoryDelete(Inventory)'
        public void InventoryDelete(Inventory inv)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.InventoryDelete(Inventory)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    DBManager.realm.Remove(inv);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.EmployeeAdd(string, string, string)'
        public void EmployeeAdd(string code, string firstname, string lastname)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.EmployeeAdd(string, string, string)'
        {
            try
            {
                if (Employees.Where(x => x.CompanyCode == Code).Any())
                    throw new Exception("Item is repeated");
                Employee emp = new Employee();
                emp.CompanyCode = code;
                emp.FirstName = firstname;
                emp.LastName = lastname;
                DBManager.realm.Write(() =>
                {
                    Employees.Add(emp);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.EmployeeAdd(Employee)'
        public void EmployeeAdd(Employee employee)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.EmployeeAdd(Employee)'
        {
            try
            {
                if (Employees.Where(x => x.FirstName == employee.FirstName && x.LastName == employee.LastName).Any())
                    throw new Exception("Item is repeated");
                DBManager.realm.Write(() =>
                {
                    Employees.Add(employee);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.EmployeeDelete(Employee)'
        public void EmployeeDelete(Employee employee)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.EmployeeDelete(Employee)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    DBManager.realm.Remove(employee);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.ExpenseAdd(ExpensItem)'
        public void ExpenseAdd(ExpensItem item)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.ExpenseAdd(ExpensItem)'
        {
            if (item.Value + Expenses.TotalValue > Budget.TotalValue)
                throw new Exception("Expenses exceed budget.");
            try
            {
                DBManager.realm.Write(() =>
                {
                    Expenses.Items.Add(item);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.ExpenseDelete(ExpensItem)'
        public void ExpenseDelete(ExpensItem item)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.ExpenseDelete(ExpensItem)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    DBManager.realm.Remove(item);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.BudgetAdd(Budgetitem)'
        public void BudgetAdd(Budgetitem item)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.BudgetAdd(Budgetitem)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    Budget.Items.Add(item);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.BudgetDelete(Budgetitem)'
        public void BudgetDelete(Budgetitem item)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.BudgetDelete(Budgetitem)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    DBManager.realm.Remove(item);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.ScheduleAdd(Schedule)'
        public void ScheduleAdd(Schedule item)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.ScheduleAdd(Schedule)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    Schedules.Add(item);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.ScheduleDelete(Schedule)'
        public void ScheduleDelete(Schedule item)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.ScheduleDelete(Schedule)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    DBManager.realm.Remove(item);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.LogAdd(Log)'
        public void LogAdd(Log item)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.LogAdd(Log)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    Logs.Add(item);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.LogDelete(Log)'
        public void LogDelete(Log item)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.LogDelete(Log)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    DBManager.realm.Remove(item);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.SafetyReportAdd(SafetyReport)'
        public void SafetyReportAdd(SafetyReport item)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.SafetyReportAdd(SafetyReport)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    Safety.SafetyReports.Add(item);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Plant.SafetyReportDelete(SafetyReport)'
        public void SafetyReportDelete(SafetyReport item)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Plant.SafetyReportDelete(SafetyReport)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    DBManager.realm.Remove(item);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
    #region production
    public class ProductionLine : RealmObject
    {
        #region Fields
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        /// <summary>
        /// Gets or sets the partition.
        /// </summary>
        /// <value>The partition.</value>
        [MapTo("partition")]
        public string Partition { get; set; } = "Public";
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = false, ErrorMessage = "Code cannot be empty!")]
        [MapTo("Code")]
        public string Code { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [MapTo("Description")]
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the line no.
        /// </summary>
        /// <value>The line no.</value>
        [StringLength(maximumLength: 12, MinimumLength = 1, ErrorMessage = "Line number must be set!")]
        public string LineNo { get; set; }
        /// <summary>
        /// Gets or sets the extra data.
        /// </summary>
        /// <value>The extra data.</value>
        public string ExtraData { get; set; }
        /// <summary>
        /// Gets the equipments.
        /// </summary>
        /// <value>The equipments.</value>
        public IList<Equipment> Equipments { get; }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ProductionLine.Shops'
        public IList<Shop> Shops { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ProductionLine.Shops'
        /// <summary>
        /// Gets the bussiness unit back reference.
        /// </summary>
        /// <value>The bussiness unit back reference.</value>
        [Backlink(nameof(Plant.ProductionLines))]
        public IQueryable<Plant> BussinessUnitBackRef { get; }

        #endregion

        #region Methods
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ProductionLine.ProductionLine()'
        public ProductionLine()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ProductionLine.ProductionLine()'
        {

        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ProductionLine.EquipmentAdd(Equipment)'
        public void EquipmentAdd(Equipment eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ProductionLine.EquipmentAdd(Equipment)'
        {
            try
            {
                if (Equipments.Where(x => x.Code == eq.Code).Any())
                    throw new Exception("Item is repeated");
                DBManager.realm.Write(() =>
                {
                    Equipments.Add(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ProductionLine.EquipmentDelete(Equipment)'
        public void EquipmentDelete(Equipment eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ProductionLine.EquipmentDelete(Equipment)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    DBManager.realm.Remove(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ProductionLine.EquipmentDeleteMany(List<Equipment>)'
        public void EquipmentDeleteMany(List<Equipment> eqs)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ProductionLine.EquipmentDeleteMany(List<Equipment>)'
        {
            foreach (Equipment eq in eqs)
                EquipmentDelete(eq);

        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ProductionLine.ShopAdd(Shop)'
        public void ShopAdd(Shop eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ProductionLine.ShopAdd(Shop)'
        {
            try
            {
                if (Shops.Where(x => x.ShopName == eq.ShopName).Any())
                    throw new Exception("Item is repeated");
                DBManager.realm.Write(() =>
                {
                    Shops.Add(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ProductionLine.ShopDelete(Shop)'
        public void ShopDelete(Shop eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ProductionLine.ShopDelete(Shop)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    DBManager.realm.Remove(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
    public class Shop : RealmObject
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        /// <summary>
        /// Gets or sets the partition.
        /// </summary>
        /// <value>The partition.</value>
        [MapTo("partition")]
        public string Partition { get; set; } = "Public";
        /// <summary>
        /// Gets or sets the name of the shop.
        /// </summary>
        /// <value>The name of the shop.</value>
        [StringLength(maximumLength: 50, MinimumLength = 1, ErrorMessage = "Shop name must be set?")]
        public string ShopName { get; set; }
        /// <summary>
        /// Gets or sets the shop description.
        /// </summary>
        /// <value>The shop description.</value>
        public string ShopDescription { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="Shop"/> class.
        /// </summary>
        [Backlink(nameof(ProductionLine.Shops))]
        public IQueryable<ProductionLine> ProductionLineBackRef { get; }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Shop.Shop()'
        public Shop() { }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Shop.Shop()'
    }
    public class Equipment : RealmObject
    {
        #region Fields
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        /// <summary>
        /// Gets or sets the partition.
        /// </summary>
        /// <value>The partition.</value>
        [MapTo("partition")]
        public string Partition { get; set; } = "Public";
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = false, ErrorMessage = "Code cannot be empty!")]
        public string Code { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Equipment.Type'
        public string Type { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Equipment.Type'

        /// <summary>
        /// Gets or sets the extra.
        /// </summary>
        /// <value>The extra.</value>
        public string Extra { get; set; }
        /// <summary>
        /// Gets or sets the shop extra.
        /// </summary>
        /// <value>The shop extra.</value>
        public string ShopExtra { get; set; }
        /// <summary>
        /// Gets or sets the shop.
        /// </summary>
        /// <value>The shop.</value>
        public string Shop { get; set; }
        /// <summary>
        /// Gets or sets the brand.
        /// </summary>
        /// <value>The brand.</value>
        public string Brand { get; set; }
        /// <summary>
        /// Gets or sets the type of the brand.
        /// </summary>
        /// <value>The type of the brand.</value>
        public string BrandType { get; set; }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Equipment.LocationLongitude'
        public double LocationLongitude { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Equipment.LocationLongitude'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Equipment.LocationLatitude'
        public double LocationLatitude { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Equipment.LocationLatitude'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Equipment.LocationAltitude'
        public double LocationAltitude { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Equipment.LocationAltitude'

        /// <summary>
        /// Gets the motors.
        /// </summary>
        /// <value>The motors.</value>
        public IList<Motor> Motors { get; }
        /// <summary>
        /// Gets the sensors.
        /// </summary>
        /// <value>The sensors.</value>
        public IList<Sensor> Sensors { get; }
        /// <summary>
        /// Gets the other components.
        /// </summary>
        /// <value>The other components.</value>
        public IList<OtherComponent> OtherComponents { get; }
        /// <summary>
        /// Gets the logs.
        /// </summary>
        /// <value>The logs.</value>
        public IList<Log> Logs { get; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Equipment.Schedules'
        public IList<Schedule> Schedules { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Equipment.Schedules'


        [Backlink(nameof(ProductionLine.Equipments))]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Equipment.ProductionLineBackRef'
        public IQueryable<ProductionLine> ProductionLineBackRef { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Equipment.ProductionLineBackRef'

        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="Equipment"/> class.
        /// </summary>
        public Equipment() { }
        
        /// <summary>
        /// Components the add modify.
        /// </summary>
        /// <param name="component">The component.</param>
        public void ComponentAdd<T>(T component)
        {

            Type ComponentType = component.GetType();
            if (!ComponentType.Equals(typeof(Motor)) && !ComponentType.Equals(typeof(Sensor)) &&
                !ComponentType.Equals(typeof(OtherComponent)))
                throw new Exception("Componenet Type is not registered!");
            try
            {

                DBManager.realm.Write(() =>
                {
                    if (ComponentType.Equals(typeof(Motor)))
                    {
                        if (Motors.Where(x => x.Code == (component as Motor).Code).Any())
                            throw new Exception("Item is repeated");
                        Motors?.Add(component as Motor);
                    }
                    else if (ComponentType.Equals(typeof(Sensor)))
                    {
                        if (Sensors.Where(x => x.Code == (component as Sensor).Code).Any())
                            throw new Exception("Item is repeated");
                        Sensors?.Add(component as Sensor);
                    }
                    else if (ComponentType.Equals(typeof(OtherComponent)))
                    {
                        if (OtherComponents.Where(x => x.Code == (component as OtherComponent).Code).Any())
                            throw new Exception("Item is repeated");
                        OtherComponents?.Add(component as OtherComponent);
                    }
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }



        }
        /// <summary>
        /// Components the delete.
        /// </summary>
        /// <param name="component">The component.</param>
        public void ComponentDelete<T>(T component)
        {
            Type ComponentType = component.GetType();
            if (!ComponentType.Equals(typeof(Motor)) || !ComponentType.Equals(typeof(Sensor)) ||
                !ComponentType.Equals(typeof(OtherComponent)))
                throw new Exception("Componenet Type is not registered!");
            try
            {
                DBManager.realm.Write(() =>
                {
                    if (ComponentType.Equals(typeof(Motor)))
                    {
                        var motor = (component as Motor);
                        Motors?.Remove(motor);
                    }
                    else if (ComponentType.Equals(typeof(Sensor)))
                    {
                        var sensor = (component as Sensor);
                        Sensors?.Remove(sensor);
                    }
                    else if (ComponentType.Equals(typeof(OtherComponent)))
                    {
                        var otherComponent = (component as OtherComponent);
                        OtherComponents?.Remove(otherComponent);
                    }
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }
        
#pragma warning disable CS1572 // XML comment has a param tag for 'mt', but there is no parameter by that name
/// <summary>
        /// Gets the motor latest grease date.
        /// </summary>
        /// <param name="mt">The mt.</param>
        /// <returns>DateTimeOffset.</returns>

#pragma warning disable CS1573 // Parameter 'Code' has no matching param tag in the XML comment for 'Equipment.ComponentGet(string)' (but other parameters do)
        public dynamic ComponentGet(string Code)
#pragma warning restore CS1573 // Parameter 'Code' has no matching param tag in the XML comment for 'Equipment.ComponentGet(string)' (but other parameters do)
#pragma warning restore CS1572 // XML comment has a param tag for 'mt', but there is no parameter by that name
        {
            
           /* Type ComponentType = component.GetType();
            if (!ComponentType.Equals(typeof(Motor)) || !ComponentType.Equals(typeof(Sensor)) ||
                !ComponentType.Equals(typeof(OtherComponent)))
                throw new Exception("Componenet Type is not registered!");*/
            try
            {
                var xxM = Motors?.Where(x => x.Code == Code)?.FirstOrDefault();
                var xxS = Sensors?.Where(x => x.Code == Code)?.FirstOrDefault();
                var xxO = OtherComponents?.Where(x => x.Code == Code)?.FirstOrDefault();

                if (xxM!=null)
                    {
                    return xxM;
                    }
                else if (xxS != null)
                {
                    return xxS;
                }
                else if (xxO != null)
                {
                    return xxO;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Equipment.getMotorLatestGreaseDate(Motor)'
        public DateTimeOffset getMotorLatestGreaseDate(Motor mt)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Equipment.getMotorLatestGreaseDate(Motor)'
        {
            if (Logs != null)
            {
                List<DateTimeOffset> result = Logs.Where(lg =>
                                        lg.Repair == MotorMaintainenanceDetails.GREASE &&
                                        lg.ItemCode == mt.Code).Select(d => d.DateLog).ToList();

                if (result.Count > 0)
                {
                    DateTimeOffset maxDate = result.Max();
                    return maxDate;
                }
                else
                    return DateTimeOffset.Now.Date;
            }
            else
                return DateTimeOffset.Now.Date;
        }
        /// <summary>
        /// Gets the motor new grease date.
        /// </summary>
        /// <param name="motor">The motor.</param>
        /// <returns>DateTimeOffset.</returns>
        public DateTimeOffset getMotorNewGreaseDate(Motor motor)
        {
            DateTimeOffset dt = getMotorLatestGreaseDate(motor);
            return dt.AddDays((double)motor?.IntervalDays).Date;
        }
        
#pragma warning disable CS1587 // XML comment is not placed on a valid language element
/// <summary>
        /// Updates the motors grease schedule.
        /// </summary>
        #endregion
    }
    public class Motor : RealmObject
    {

        #region Fields
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        /// <summary>
        /// Gets or sets the partition.
        /// </summary>
        /// <value>The partition.</value>
        [MapTo("partition")]
        public string Partition { get; set; } = "Public";
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = false, ErrorMessage = "Code cannot be empty!")]
        [RegularExpression(@"^([0-9])([a-z A-Z 0-9])([a-z A-Z 0-9])([a-z A-Z]){2}([0-9]){2}([a-z A-Z]){2}([a-z A-Z 0-9]){1,3}$", ErrorMessage = "Code formation is not valid 000AA00AA00")]
        public string Code { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the brand.
        /// </summary>
        /// <value>The brand.</value>
        public string Brand { get; set; }
        /// <summary>
        /// Gets or sets the type of the brand.
        /// </summary>
        /// <value>The type of the brand.</value>
        public string BrandType { get; set; }
        /// <summary>
        /// Gets or sets the extra data.
        /// </summary>
        /// <value>The extra data.</value>
        public string ExtraData { get; set; }
        //Motor specific
        /// <summary>
        /// Gets or sets the supply panel.
        /// </summary>
        /// <value>The supply panel.</value>
        public string SupplyPanel { get; set; }
        /// <summary>
        /// Gets or sets the supply panel loc.
        /// </summary>
        /// <value>The supply panel loc.</value>
        public string supplyPanelLoc { get; set; }
        /// <summary>
        /// Gets or sets the power.
        /// </summary>
        /// <value>The power.</value>
        public double Power { get; set; }
        /// <summary>
        /// Gets or sets the frequency.
        /// </summary>
        /// <value>The frequency.</value>
        [IntRange(50, 60)]
        public double Frequency { get; set; }
        /// <summary>
        /// Gets or sets the poles.
        /// </summary>
        /// <value>The poles.</value>
        [IntRange(2, 4, 6, 8)]
        public int Poles { get; set; }
        /// <summary>
        /// Gets or sets the volt.
        /// </summary>
        /// <value>The volt.</value>
        [IntRange(220, 230, 380, 400, 690, 11000)]
        public double Volt { get; set; }
        /// <summary>
        /// Gets or sets the current.
        /// </summary>
        /// <value>The current.</value>
        public double Current { get; set; }
        /// <summary>
        /// Gets or sets the RPM.
        /// </summary>
        /// <value>The RPM.</value>
        [Range(700, 3000, ErrorMessage = "Speed value is incorrect!")]
        public double RPM { get; set; }
        /// <summary>
        /// Gets or sets the cos phi.
        /// </summary>
        /// <value>The cos phi.</value>
        [Range(0, 1, ErrorMessage = "Cos phi value is incorrect!")]
        public double CosPhi { get; set; }
        /// <summary>
        /// Gets or sets the effeciency.
        /// </summary>
        /// <value>The effeciency.</value>
        [Range(0, 100, ErrorMessage = "Effeciency value is incorrect!")]
        public double Effeciency { get; set; }
        /// <summary>
        /// Gets or sets the mounting.
        /// </summary>
        /// <value>The mounting.</value>
        public string Mounting { get; set; }
        /// <summary>
        /// Gets or sets the size of the frame.
        /// </summary>
        /// <value>The size of the frame.</value>
        public string FrameSize { get; set; }
        /// <summary>
        /// Gets or sets the cooling.
        /// </summary>
        /// <value>The cooling.</value>
        public string Cooling { get; set; }
        /// <summary>
        /// Gets or sets the duty.
        /// </summary>
        /// <value>The duty.</value>
        [StringRange(AllowableValues = new[] { "S1", "S2", "S3", "S4", "S5", "S6", "S7", "S8" })]
        public string Duty { get; set; }
        /// <summary>
        /// Gets or sets the thermal class.
        /// </summary>
        /// <value>The thermal class.</value>
        [StringRange(AllowableValues = new[] { "A", "B", "F", "H" })]
        public string ThermalClass { get; set; }
        /// <summary>
        /// Gets or sets the ip.
        /// </summary>
        /// <value>The ip.</value>
        [Range(0, 67)]
        public int IP { get; set; }
        /// <summary>
        /// Gets or sets the bearing de.
        /// </summary>
        /// <value>The bearing de.</value>
        public string BearingDE { get; set; }
        /// <summary>
        /// Gets or sets the bearing nde.
        /// </summary>
        /// <value>The bearing nde.</value>
        public string BearingNDE { get; set; }
        /// <summary>
        /// Gets or sets the grease brand.
        /// </summary>
        /// <value>The grease brand.</value>
        public string GreaseBrand { get; set; }
        /// <summary>
        /// Gets or sets the type of the grease brand.
        /// </summary>
        /// <value>The type of the grease brand.</value>
        public string GreaseBrandType { get; set; }
        /// <summary>
        /// Gets or sets the interval days.
        /// </summary>
        /// <value>The interval days.</value>
        public double IntervalDays { get; set; }
        /// <summary>
        /// Gets or sets the qty de.
        /// </summary>
        /// <value>The qty de.</value>
        public double QtyDE { get; set; }
        /// <summary>
        /// Gets or sets the qty nde.
        /// </summary>
        /// <value>The qty nde.</value>
        public double QtyNDE { get; set; }
        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        /// <value>The weight.</value>
        public double Weight { get; set; }

        /// <summary>
        /// Gets the spares inv.
        /// </summary>
        /// <value>The spares inv.</value>
        public IList<SparePart> SpareParts { get; }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Motor.Logs'
        public IList<Log> Logs { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Motor.Logs'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Motor.Schedules'
        public IList<Schedule> Schedules { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Motor.Schedules'

        /// <summary>
        /// Gets the equipment.
        /// </summary>
        /// <value>The equipment.</value>
        [Backlink(nameof(Equipment.Motors))]
        public IQueryable<Equipment> equipment { get; }
        #endregion

        #region Methods
        /* public void MotorModify(Motor mt) {
             Code = mt.Code;
             Description = mt.Description;
             Brand = mt.Brand;
             BrandType = mt.BrandType;
             ExtraData = mt.ExtraData;
             SupplyPanel = mt.SupplyPanel;
             supplyPanelLoc = mt.supplyPanelLoc;
             Power = mt.Power;
             Volt = mt.Volt;
             Current = mt.Current;
             RPM = mt.RPM;
             BearingDE = mt.BearingDE;
             BearingNDE = mt.BearingNDE;
             GreaseBrand = mt.GreaseBrand;
             GreaseBrandType = mt.GreaseBrandType;
             IntervalDays = mt.IntervalDays;
             QtyDE = mt.QtyDE;
             QtyNDE = mt.QtyNDE;
             Effeciency = mt.Effeciency;
         }*/
        /// <summary>
        /// Initializes a new instance of the <see cref="Motor"/> class.
        /// </summary>
        public Motor() { }
        /// <summary>
        /// Maintains the part details.
        /// </summary>
        /// <param name="maintenanceStrategy">The maintenance strategy.</param>
        /// <param name="motorpart">The motorpart.</param>
        /// <param name="ExtraDetails">The extra details.</param>
        /// <returns>System.String.</returns>
        public string MaintainPartDetails(MaintenanceStrategy maintenanceStrategy
                                           , MotorParts motorpart, string ExtraDetails)
        {

            switch (maintenanceStrategy)
            {
                case MaintenanceStrategy.CORRECTIVE:
                    switch (motorpart)
                    {
                        case MotorParts.BEARINGDE:
                            return MotorMaintainenanceDetails.REPLACEBEARINGDE + "(" + BearingDE == null ? BearingDE : "" + ")" + ExtraDetails;
                        case MotorParts.BEARINGNDE:
                            return MotorMaintainenanceDetails.REPLACEBEARINGNDE + "(" + BearingNDE == null ? BearingNDE : "" + ")" + ExtraDetails;
                        case MotorParts.BEARINGS:
                            return MotorMaintainenanceDetails.REPLACEBEARINGS +
                        " (" + BearingDE == null ? BearingDE : "" + BearingNDE == null ? BearingNDE : "" + ")" + ExtraDetails;
                        case MotorParts.ENDSHIELDDE:
                            return MotorMaintainenanceDetails.FIXENDSHIELDDE + " (" + BearingDE == null ? BearingDE : "" + ")" + ExtraDetails;
                        case MotorParts.ENDSHIELDNDE:
                            return MotorMaintainenanceDetails.FIXENDSHIELDNDE + " (" + BearingNDE == null ? BearingNDE : "" + ")" + ExtraDetails;
                        case MotorParts.ENDSHIELDS:
                            return MotorMaintainenanceDetails.FIXENDSHIELDS + " (" + MotorParts.ENDSHIELDS == null ? BearingDE : "" +
                                                                                 BearingNDE == null ? BearingNDE : "" + ")" + ExtraDetails;
                        case MotorParts.FAN: return MotorMaintainenanceDetails.REPLACEFAN + ExtraDetails;
                        case MotorParts.FANCOVER: return MotorMaintainenanceDetails.FIXFANCOVER + ExtraDetails;
                        case MotorParts.OILSEAL: return MotorMaintainenanceDetails.REPLACESEAL + ExtraDetails;
                        case MotorParts.STATORASSEMBLY: return MotorMaintainenanceDetails.REWIND + ExtraDetails;
                        case MotorParts.ROTORASSEMBLY: return MotorMaintainenanceDetails.ROTORBALANCE + ExtraDetails;
                        case MotorParts.BRUSHES: return MotorMaintainenanceDetails.REPLACEBRUSHES + ExtraDetails;
                    }
                    break;
                case MaintenanceStrategy.PREVENTIVE:
                    switch (motorpart)
                    {
                        case MotorParts.BEARINGDE:
                            return MotorMaintainenanceDetails.REPLACEBEARINGDE + "(" + BearingDE == null ? BearingDE : "" + ")" + ExtraDetails;
                        case MotorParts.BEARINGNDE:
                            return MotorMaintainenanceDetails.REPLACEBEARINGNDE + "(" + BearingNDE == null ? BearingNDE : "" + ")" + ExtraDetails;
                        case MotorParts.BEARINGS:
                            return MotorMaintainenanceDetails.REPLACEBEARINGS + " (" + BearingDE == null ? BearingDE : "" +
                                                                                  BearingNDE == null ? BearingNDE : "" + ")" + ExtraDetails;
                        case MotorParts.OILSEAL: return MotorMaintainenanceDetails.REPLACESEAL + ExtraDetails;
                        case MotorParts.BRUSHES: return MotorMaintainenanceDetails.REPLACEBRUSHES + ExtraDetails;
                        case MotorParts.BEARINGSGREASE: return MotorMaintainenanceDetails.GREASE + ExtraDetails;
                        case MotorParts.BEARINGSGREASEDE: return MotorMaintainenanceDetails.GREASEDE + ExtraDetails;
                        case MotorParts.BEARINGSGREASENDE: return MotorMaintainenanceDetails.GREASENDE + ExtraDetails;

                    }
                    break;
                case MaintenanceStrategy.PREDICTIVE:
                    switch (motorpart)
                    {
                        case MotorParts.BEARINGDE: return MotorMaintainenanceDetails.CHECKBEARING + ExtraDetails;
                        case MotorParts.BEARINGNDE: return MotorMaintainenanceDetails.REPLACEBEARINGNDE + ExtraDetails;
                        case MotorParts.BEARINGS: return MotorMaintainenanceDetails.CHECKVIBRATION + ExtraDetails;
                        case MotorParts.OILSEAL: return MotorMaintainenanceDetails.REPLACESEAL + ExtraDetails;
                        case MotorParts.STATORASSEMBLY: return MotorMaintainenanceDetails.STATORINSULATIONTEST + ExtraDetails;
                        case MotorParts.ROTORASSEMBLY: return MotorMaintainenanceDetails.ROTORINSULATIONTEST + ExtraDetails;
                        case MotorParts.BRUSHES: return MotorMaintainenanceDetails.CHECKBRUSHES + ExtraDetails;
                    }
                    break;
            }

            return "";
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Motor.SparePartAdd(SparePart)'
        public void SparePartAdd(SparePart eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Motor.SparePartAdd(SparePart)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    SpareParts.Add(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Motor.SparePartDelete(SparePart)'
        public void SparePartDelete(SparePart eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Motor.SparePartDelete(SparePart)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    DBManager.realm.Remove(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Motor.LogAdd(Log)'
        public void LogAdd(Log eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Motor.LogAdd(Log)'
        {
            try
            {
                DBManager.realm.Write(() =>
                 {
                     Logs.Add(eq);
                 });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Motor.LogDelete(Log)'
        public void LogDelete(Log eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Motor.LogDelete(Log)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    DBManager.realm.Remove(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Motor.ScheduleAdd(Schedule)'
        public void ScheduleAdd(Schedule eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Motor.ScheduleAdd(Schedule)'
        {
            try
            {
                DBManager.realm.Write(() =>
                 {
                     Schedules.Add(eq);
                 });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Motor.ScheduleDelete(Schedule)'
        public void ScheduleDelete(Schedule eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Motor.ScheduleDelete(Schedule)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    DBManager.realm.Remove(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

    }
    public class Sensor : RealmObject
    {
        #region Fields 
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        /// <summary>
        /// Gets or sets the partition.
        /// </summary>
        /// <value>The partition.</value>
        [MapTo("partition")]
        public string Partition { get; set; } = "Public";
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Code cannot be empty!")]
        [RegularExpression(@"^([0-9])([a-z A-Z 0-9])([a-z A-Z 0-9])([a-z A-Z]){2}([0-9]){2}([a-z A-Z]){2}([a-z A-Z 0-9]){1,3}$", ErrorMessage = "Code formation is not valid 000AA00AA00")]
        public string Code { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the brand.
        /// </summary>
        /// <value>The brand.</value>
        public string Brand { get; set; }
        /// <summary>
        /// Gets or sets the type of the brand.
        /// </summary>
        /// <value>The type of the brand.</value>
        public string BrandType { get; set; }
        /// <summary>Gets or sets the type of the instrument.</summary>
        /// <value>The type of the instrument.</value>
        [StringRange(AllowableValues = new[] {"Thermometer","Pressure Transmitter","Flowmeter","Weigh Sensor","Vibro Sensor",
            "Positioner","Level Sensor","Speed/RPM","Other" })]
        public string InstrumentType { get; set; }
        [StringRange(AllowableValues = new[] { "0…10V", "2…10V", "0…20mA", "4…20mA", "Other" })]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Sensor.SignalType'
        public string SignalType { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Sensor.SignalType'
        [StringRange(AllowableValues = new[] { "24V", "48V", "110V", "230V", "380V", "400V", "Other" })]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Sensor.SupplyVoltage'
        public string SupplyVoltage { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Sensor.SupplyVoltage'
        [StringRange(AllowableValues = new[] { "2-wire", "4-wire", "Current Loop", "Other" })]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Sensor.Wiring'
        public string Wiring { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Sensor.Wiring'
        [StringRange(AllowableValues = new[] { "ProfiBUS", "Ethernet/ProfiNET", "Can-BUS", "ModBUS", "RS232", "RS422", "RS485", "Other" })]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Sensor.Communication'
        public string Communication { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Sensor.Communication'
        [StringRange(AllowableValues = new[] { "Internal", "External", "No" })]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Sensor.GalvanicIsolation'
        public string GalvanicIsolation { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Sensor.GalvanicIsolation'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Sensor.OperationalUnit'
        public string OperationalUnit { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Sensor.OperationalUnit'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Sensor.ScaleRange'
        public string ScaleRange { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Sensor.ScaleRange'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Sensor.LimitHH'
        public double LimitHH { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Sensor.LimitHH'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Sensor.LimitH'
        public double LimitH { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Sensor.LimitH'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Sensor.LimitL'
        public double LimitL { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Sensor.LimitL'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Sensor.LimitLL'
        public double LimitLL { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Sensor.LimitLL'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Sensor.Softawre'
        public string Softawre { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Sensor.Softawre'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Sensor.ExtraData'
        public string ExtraData { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Sensor.ExtraData'

        /// <summary>
        /// Gets the spares inv.
        /// </summary>
        /// <value>The spares inv.</value>
        public IList<SparePart> SpareParts { get; }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Sensor.Logs'
        public IList<Log> Logs { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Sensor.Logs'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Sensor.Schedules'
        public IList<Schedule> Schedules { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Sensor.Schedules'

        /// <summary>
        /// Gets the equipment back reference.
        /// </summary>
        /// <value>The equipment back reference.</value>
        [Backlink(nameof(Equipment.Sensors))]
        public IQueryable<Equipment> EquipmentBackRef { get; }

        #endregion
        /// <summary>
        /// Initializes a new instance of the <see cref="Sensor"/> class.
        /// </summary>
        public Sensor() { }

        public void SparePartAdd(SparePart eq)
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    SpareParts.Add(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Sensor.SparePartDelete(SparePart)'
        public void SparePartDelete(SparePart eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Sensor.SparePartDelete(SparePart)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    DBManager.realm.Remove(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Sensor.LogAdd(Log)'
        public void LogAdd(Log eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Sensor.LogAdd(Log)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    Logs.Add(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Sensor.LogDelete(Log)'
        public void LogDelete(Log eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Sensor.LogDelete(Log)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    DBManager.realm.Remove(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Sensor.ScheduleAdd(Schedule)'
        public void ScheduleAdd(Schedule eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Sensor.ScheduleAdd(Schedule)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    Schedules.Add(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Sensor.ScheduleDelete(Schedule)'
        public void ScheduleDelete(Schedule eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Sensor.ScheduleDelete(Schedule)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    DBManager.realm.Remove(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
    public class OtherComponent : RealmObject
    {
        #region Fields 
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        /// <summary>
        /// Gets or sets the partition.
        /// </summary>
        /// <value>The partition.</value>
        [MapTo("partition")]
        public string Partition { get; set; } = "Public";
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Code cannot be empty!")]
        [RegularExpression(@"^([0-9])([a-z A-Z 0-9])([a-z A-Z 0-9])([a-z A-Z]){2}([0-9]){2}([a-z A-Z]){2}([a-z A-Z 0-9]){1,3}$", ErrorMessage = "Code formation is not valid 000AA00AA00")]
        public string Code { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the brand.
        /// </summary>
        /// <value>The brand.</value>
        public string Brand { get; set; }
        /// <summary>
        /// Gets or sets the type of the brand.
        /// </summary>
        /// <value>The type of the brand.</value>
        public string BrandType { get; set; }
        /// <summary>
        /// Gets or sets the extra data.
        /// </summary>
        /// <value>The extra data.</value>
        public string ExtraData { get; set; }
        /// <summary>
        /// Gets the spares.
        /// </summary>
        /// <value>The spares.</value>
        public IList<Log> Logs { get; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'OtherComponent.Schedules'
        public IList<Schedule> Schedules { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'OtherComponent.Schedules'

        /// <summary>
        /// Gets the spares inv.
        /// </summary>
        /// <value>The spares inv.</value>
        public IList<SparePart> SpareParts { get; }
        /// <summary>
        /// Gets the equipment back reference.
        /// </summary>
        /// <value>The equipment back reference.</value>
        [Backlink(nameof(Equipment.OtherComponents))]
        public IQueryable<Equipment> EquipmentBackRef { get; }

        #endregion
        /// <summary>
        /// Initializes a new instance of the <see cref="OtherComponent"/> class.
        /// </summary>
        public OtherComponent() { }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'OtherComponent.SparePartAdd(SparePart)'
        public void SparePartAdd(SparePart eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'OtherComponent.SparePartAdd(SparePart)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    SpareParts.Add(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'OtherComponent.SparePartDelete(SparePart)'
        public void SparePartDelete(SparePart eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'OtherComponent.SparePartDelete(SparePart)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    DBManager.realm.Remove(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'OtherComponent.LogAdd(Log)'
        public void LogAdd(Log eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'OtherComponent.LogAdd(Log)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    Logs.Add(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'OtherComponent.LogDelete(Log)'
        public void LogDelete(Log eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'OtherComponent.LogDelete(Log)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    DBManager.realm.Remove(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'OtherComponent.ScheduleAdd(Schedule)'
        public void ScheduleAdd(Schedule eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'OtherComponent.ScheduleAdd(Schedule)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    Schedules.Add(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'OtherComponent.ScheduleDelete(Schedule)'
        public void ScheduleDelete(Schedule eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'OtherComponent.ScheduleDelete(Schedule)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    DBManager.realm.Remove(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
    public class SparePart : RealmObject
    {
        [PrimaryKey]
        [MapTo("_id")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SparePart.Id'
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SparePart.Id'
        [MapTo("partition")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SparePart.Partition'
        public string Partition { get; set; } = "Public";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SparePart.Partition'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SparePart.ItemCode'
        public string ItemCode { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SparePart.ItemCode'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SparePart.InventoryCode'
        public string InventoryCode { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SparePart.InventoryCode'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SparePart.Description1'
        public string Description1 { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SparePart.Description1'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SparePart.Description2'
        public string Description2 { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SparePart.Description2'

        [Range(0, double.MaxValue)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SparePart.QtyRequired'
        public double QtyRequired { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SparePart.QtyRequired'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SparePart.SparePart()'
        public SparePart() { }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SparePart.SparePart()'
    }
    public static class MaintenanceStrategyDetails
    {
        /// <summary>
        /// The corrective
        /// </summary>
        public const string CORRECTIVE = "Corrective Maintenance";
        /// <summary>
        /// The preventive
        /// </summary>
        public const string PREVENTIVE = "Preventive Maintenance";
        /// <summary>
        /// The predictive
        /// </summary>
        public const string PREDICTIVE = "Predictive Maintenance";
    }
    public class Log : RealmObject, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        /// <summary>
        /// Gets or sets the partition.
        /// </summary>
        /// <value>The partition.</value>
        [MapTo("partition")]
        public string Partition { get; set; } = "Public";
        /// <summary>
        /// Gets or sets the item code.
        /// </summary>
        /// <value>The item code.</value>
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Item Code cannot be empty!")]
        [RegularExpression(@"^([0-9])([a-z A-Z 0-9])([a-z A-Z 0-9])([a-z A-Z]){2}([0-9]){2}([a-z A-Z]){2}([a-z A-Z 0-9]){1,3}$", ErrorMessage = "Code formation is not valid 000AA00AA00")]
        public string ItemCode { get; set; }
        /// <summary>
        /// Gets or sets the item description.
        /// </summary>
        /// <value>The item description.</value>
        public string ItemDescription { get; set; }
        /// <summary>
        /// Gets or sets the repair.
        /// </summary>
        /// <value>The repair.</value>
        public string Repair { get; set; }
        /// <summary>
        /// Gets or sets the repairdetails.
        /// </summary>
        /// <value>The repairdetails.</value>
        public string Repairdetails { get; set; }
        [Range(0, double.MaxValue)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Log.Cost'
        public double Cost { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Log.Cost'

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        /// <value>The notes.</value>
        public string Notes { get; set; }
        /// <summary>
        /// Gets or sets the assignee.
        /// </summary>
        /// <value>The assignee.</value>
        public string AssigneeCompanyCode { get; set; }

        /// <summary>
        /// Gets or sets the date log.
        /// </summary>
        /// <value>The date log.</value>
        public DateTimeOffset DateLog { get; set; }

        [Backlink(nameof(Plant.Logs))]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Log.BussinessUnitBackRef'
        public IQueryable<Plant> BussinessUnitBackRef { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Log.BussinessUnitBackRef'

        [Backlink(nameof(Equipment.Logs))]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Log.EquipmentBackRef'
        public IQueryable<Equipment> EquipmentBackRef { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Log.EquipmentBackRef'

        [Backlink(nameof(Motor.Logs))]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Log.MotorBackRef'
        public IQueryable<Motor> MotorBackRef { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Log.MotorBackRef'

        [Backlink(nameof(Sensor.Logs))]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Log.SensorBackRef'
        public IQueryable<Sensor> SensorBackRef { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Log.SensorBackRef'
        [Backlink(nameof(OtherComponent.Logs))]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Log.OtherComponentBackRef'
        public IQueryable<OtherComponent> OtherComponentBackRef { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Log.OtherComponentBackRef'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Log.Log()'
        public Log() { }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Log.Log()'

    }
    public class Schedule : RealmObject
    {

        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [MapTo("partition")]
        public string Partition { get; set; } = "Public";
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Item Code cannot be empty!")]
        [RegularExpression(@"^([0-9])([a-z A-Z 0-9])([a-z A-Z 0-9])([a-z A-Z]){2}([0-9]){2}([a-z A-Z]){2}([a-z A-Z 0-9]){1,3}$", ErrorMessage = "Code formation is not valid 000AA00AA00")]
        public string ItemCode { get; set; }

        public string ItemDescription { get; set; }

        public string Area { get; set; }
        public string Repair { get; set; }

        public string Repairdetails { get; set; }
        [Range(0, double.MaxValue)]
        public double RepairCost { get; set; }
        [Range(0, double.MaxValue)]
        [Ignored]
        public double SparesCost { 
            get {
                /* object tempObject = EvaluateAlias(nameof(Amount));
               if (tempObject != null)
               {
                   return (double)tempObject;
               }
               else
               {
                   return 0;
               }*/
                return SpareParts.Sum(ep => ep.Value * ep.QtyRequired);
            } 
            }
        public string Notes { get; set; }
        public DateTimeOffset SetDate { get; set; }
        public DateTimeOffset DateScheduleFrom { get; set; }
        [DateGreaterThan("DateScheduleFrom")]
        public DateTimeOffset DateScheduleTo { get; set; }
        public string AssigneeCompanyCode { get; set; }
        public string AssigneeDoneCompanyCode { get; set; }
       
        public bool StatusSchedule { get; set; }
        public IList<ScheduleSparePart> SpareParts { get; }

        [Backlink(nameof(Plant.Schedules))]
        public IQueryable<Plant> PlantBackRef { get; }

        [Backlink(nameof(Equipment.Schedules))]
        public IQueryable<Equipment> EquipmentBackRef { get; }

        [Backlink(nameof(Motor.Schedules))]
        public IQueryable<Motor> MotorBackRef { get; }

        [Backlink(nameof(Sensor.Schedules))]
        public IQueryable<Sensor> SensorBackRef { get; }

        [Backlink(nameof(OtherComponent.Schedules))]
        public IQueryable<OtherComponent> OtherComponentBackRef { get; }
        public Schedule() { }
        public void SparePartAdd(ScheduleSparePart eq)
        {
            try
            {
              
                DBManager.realm.Write(() =>
                {
                    SpareParts.Add(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public void SparePartDelete(ScheduleSparePart eq)
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    DBManager.realm.Remove(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePart'
    public class ScheduleSparePart : RealmObject
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePart'
    {
        [PrimaryKey]
        [MapTo("_id")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePart.Id'
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePart.Id'
        [MapTo("partition")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePart.Partition'
        public string Partition { get; set; } = "Public";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePart.Partition'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePart.InventoryCode'
        public string InventoryCode { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePart.InventoryCode'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePart.Description1'
        public string Description1 { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePart.Description1'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePart.Description2'
        public string Description2 { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePart.Description2'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePart.Value'
        public double Value { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePart.Value'

        [Range(0, double.MaxValue)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePart.QtyRequired'
        public double QtyRequired { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePart.QtyRequired'
        [Backlink(nameof(Schedule.SpareParts))]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePart.SparePartsBackRef'
        public IQueryable<Schedule> SparePartsBackRef { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePart.SparePartsBackRef'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePart.ScheduleSparePart()'
        public ScheduleSparePart() { }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePart.ScheduleSparePart()'
        
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePartImport'
    public class ScheduleSparePartImport
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePartImport'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePartImport.CodeItem'
        public string CodeItem { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePartImport.CodeItem'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePartImport.InventoryCode'
        public string InventoryCode { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePartImport.InventoryCode'
        [Range(0, double.MaxValue)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePartImport.QtyRequired'
        public double QtyRequired { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePartImport.QtyRequired'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePartImport.ScheduleSparePartImport()'
        public ScheduleSparePartImport() { }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ScheduleSparePartImport.ScheduleSparePartImport()'

    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Safety'
    public class Safety : RealmObject
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Safety'
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        /// <summary>
        /// Gets or sets the partition.
        /// </summary>
        /// <value>The partition.</value>
        [MapTo("partition")]
        public string Partition { get; set; } = "Public";
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Safety.SafetyReports'
        public IList<SafetyReport> SafetyReports { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Safety.SafetyReports'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Safety.Safety()'
        public Safety() { }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Safety.Safety()'

    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport'
    public class SafetyReport : RealmObject
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport'
    {
        [PrimaryKey]
        [MapTo("_id")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.Id'
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.Id'
        /// <summary>
        /// Gets or sets the partition.
        /// </summary>
        /// <value>The partition.</value>
        [MapTo("partition")]
        public string Partition { get; set; } = "Public";
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.IssueDate'
        public DateTimeOffset IssueDate { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.IssueDate'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.SourceOfIssue'
        public string SourceOfIssue { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.SourceOfIssue'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.IssueOnCompany'
        public string IssueOnCompany { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.IssueOnCompany'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.IssueOnCompanyName'
        public string IssueOnCompanyName { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.IssueOnCompanyName'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ReporterDetails'
        public string ReporterDetails { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ReporterDetails'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ReporterDepartment'
        public string ReporterDepartment { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ReporterDepartment'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ReporterName'
        public string ReporterName { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ReporterName'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ReportDetailsArea'
        public string ReportDetailsArea { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ReportDetailsArea'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ReportDetailsLine'
        public string ReportDetailsLine { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ReportDetailsLine'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ReportDetailsObservation'
        public string ReportDetailsObservation { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ReportDetailsObservation'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ReportDetailsType'
        public string ReportDetailsType { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ReportDetailsType'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ReportDetailsViolationType'
        public string ReportDetailsViolationType { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ReportDetailsViolationType'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ReportDetailsRisk'
        public string ReportDetailsRisk { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ReportDetailsRisk'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ReportDetailscorrectiveActions'
        public string ReportDetailscorrectiveActions { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ReportDetailscorrectiveActions'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ResponsibilityDepartment'
        public string ResponsibilityDepartment { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ResponsibilityDepartment'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ResponsibilityPerson'
        public string ResponsibilityPerson { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ResponsibilityPerson'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.DueDate'
        public DateTimeOffset DueDate { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.DueDate'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.CloseDate'
        public DateTimeOffset CloseDate { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.CloseDate'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.Status'
        public string Status { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.Status'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ViolationCompany'
        public string ViolationCompany { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ViolationCompany'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ViolationDuring'
        public string ViolationDuring { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ViolationDuring'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ViolationCompanyName'
        public string ViolationCompanyName { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ViolationCompanyName'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ViolationDepartment'
        public string ViolationDepartment { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ViolationDepartment'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ViolationName'
        public string ViolationName { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.ViolationName'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.Feedback'
        public string Feedback { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.Feedback'
        [Backlink(nameof(Safety.SafetyReports))]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.SafetyBackRef'
        public IQueryable<Safety> SafetyBackRef { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.SafetyBackRef'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.SafetyReport()'
        public SafetyReport() { }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReport.SafetyReport()'

    }
    /// <summary>
    /// Enums
    /// </summary>
    public enum MaintenanceStrategy
    {
        /// <summary>
        /// The corrective
        /// </summary>
        CORRECTIVE,
        /// <summary>
        /// The preventive
        /// </summary>
        PREVENTIVE,
        /// <summary>
        /// The predictive
        /// </summary>
        PREDICTIVE
    }
    /// <summary>
    /// Extensions
    /// </summary>
    static class Extension
    {
        /// <summary>
        /// Withes the specified a.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="a">a.</param>
        public static void With<T>(this T obj, Action<T> a)
        {
            a(obj);
        }
        // a map function
        /// <summary>
        /// Fors the each.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enum">The enum.</param>
        /// <param name="mapFunction">The map function.</param>
        public static void ForEach<T>(this IEnumerable<T> @enum, Action<T> mapFunction)
        {
            foreach (var item in @enum) mapFunction(item);
        }
        /// <summary>
        /// Adds the modify.
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="rlm">The RLM.</param>
        /// <param name="T">The t.</param>
        public static void AddModify<R>(this R rlm, RealmObject T) where R : Realm
        {
            rlm.Write(() =>
            {
                if (rlm.Config.ObjectClasses.Contains(T.ObjectSchema.GetType()))
                    rlm.Add(T);
            });
        }
    }
    #region Maintenance
    /// <summary>
    /// Enum MotorParts
    /// </summary>
    public enum MotorParts //modify to struct
    {
        /// <summary>
        /// The bearingcirclip
        /// </summary>
        BEARINGCIRCLIP,
        /// <summary>
        /// The bearingde
        /// </summary>
        BEARINGDE,
        /// <summary>
        /// The bearingnde
        /// </summary>
        BEARINGNDE,
        /// <summary>
        /// The bearings
        /// </summary>
        BEARINGS,
        /// <summary>
        /// The bearingsgrease
        /// </summary>
        BEARINGSGREASE,
        /// <summary>
        /// The bearingsgreasede
        /// </summary>
        BEARINGSGREASEDE,
        /// <summary>
        /// The bearingsgreasende
        /// </summary>
        BEARINGSGREASENDE,
        /// <summary>
        /// The bolt
        /// </summary>
        BOLT,
        /// <summary>
        /// The endshieldde
        /// </summary>
        ENDSHIELDDE,
        /// <summary>
        /// The endshields
        /// </summary>
        ENDSHIELDS,
        /// <summary>
        /// The endshieldflange
        /// </summary>
        ENDSHIELDFLANGE,
        /// <summary>
        /// The endshieldnde
        /// </summary>
        ENDSHIELDNDE,
        /// <summary>
        /// The fan
        /// </summary>
        FAN,
        /// <summary>
        /// The fancirclip
        /// </summary>
        FANCIRCLIP,
        /// <summary>
        /// The fancover
        /// </summary>
        FANCOVER,
        /// <summary>
        /// The feet
        /// </summary>
        FEET,
        /// <summary>
        /// The flinger
        /// </summary>
        FLINGER,
        /// <summary>
        /// The oilseal
        /// </summary>
        OILSEAL,
        /// <summary>
        /// The rotorassembly
        /// </summary>
        ROTORASSEMBLY,
        /// <summary>
        /// The screw
        /// </summary>
        SCREW,
        /// <summary>
        /// The statorassembly
        /// </summary>
        STATORASSEMBLY,
        /// <summary>
        /// The terminalboard
        /// </summary>
        TERMINALBOARD,
        /// <summary>
        /// The terminalbox
        /// </summary>
        TERMINALBOX,
        /// <summary>
        /// The terminalboxlid
        /// </summary>
        TERMINALBOXLID,
        /// <summary>
        /// The terminalboxlidgasket
        /// </summary>
        TERMINALBOXLIDGASKET,
        /// <summary>
        /// The washer
        /// </summary>
        WASHER,
        /// <summary>
        /// The greasenibble
        /// </summary>
        GREASENIBBLE,
        /// <summary>
        /// The greasedrain
        /// </summary>
        GREASEDRAIN,
        /// <summary>
        /// The heater
        /// </summary>
        HEATER,
        /// <summary>
        /// The slipring
        /// </summary>
        SLIPRING,
        /// <summary>
        /// The commutator
        /// </summary>
        COMMUTATOR,
        /// <summary>
        /// The brushes
        /// </summary>
        BRUSHES,
        /// <summary>
        /// The brushholder
        /// </summary>
        BRUSHHOLDER,
        /// <summary>
        /// The brushcompartment
        /// </summary>
        BRUSHCOMPARTMENT,
        /// <summary>
        /// The light
        /// </summary>
        LIGHT,
        /// <summary>
        /// The switch
        /// </summary>
        SWITCH,
        /// <summary>
        /// The coolerstator
        /// </summary>
        COOLERSTATOR,
        /// <summary>
        /// The coolerslipring
        /// </summary>
        COOLERSLIPRING,
        /// <summary>
        /// The nameplate
        /// </summary>
        NAMEPLATE

    }
    /// <summary>
    /// Class MotorPartsDetails.
    /// </summary>
    public static class MotorPartsDetails
    {
        /// <summary>
        /// The bearingcirclip
        /// </summary>
        public const string BEARINGCIRCLIP = "Bearing Circlip";
        /// <summary>
        /// The bearingde
        /// </summary>
        public const string BEARINGDE = "Bearing DE";
        /// <summary>
        /// The bearingnde
        /// </summary>
        public const string BEARINGNDE = "Bearing NDE";
        /// <summary>
        /// The bearings
        /// </summary>
        public const string BEARINGS = "Bearing NDE & NDE";
        /// <summary>
        /// The bearingsgrease
        /// </summary>
        public const string BEARINGSGREASE = "Grease Bearings NDE & NDE";
        /// <summary>
        /// The bolt
        /// </summary>
        public const string BOLT = "Bolt";
        /// <summary>
        /// The endshieldde
        /// </summary>
        public const string ENDSHIELDDE = "Endshield DE";
        /// <summary>
        /// The endshields
        /// </summary>
        public const string ENDSHIELDS = "Endshields";
        /// <summary>
        /// The endshieldflange
        /// </summary>
        public const string ENDSHIELDFLANGE = "Endshield Flange";
        /// <summary>
        /// The endshieldnde
        /// </summary>
        public const string ENDSHIELDNDE = "Endshield NDE";
        /// <summary>
        /// The fan
        /// </summary>
        public const string FAN = "Fan";
        /// <summary>
        /// The fancirclip
        /// </summary>
        public const string FANCIRCLIP = "Fan Circlip";
        /// <summary>
        /// The fancover
        /// </summary>
        public const string FANCOVER = "Fan Cover";
        /// <summary>
        /// The feet
        /// </summary>
        public const string FEET = "Feet";
        /// <summary>
        /// The flinger
        /// </summary>
        public const string FLINGER = "Flinger";
        /// <summary>
        /// The oilseal
        /// </summary>
        public const string OILSEAL = "Oil seal";
        /// <summary>
        /// The rotorassembly
        /// </summary>
        public const string ROTORASSEMBLY = "Rotor Assembly";
        /// <summary>
        /// The screw
        /// </summary>
        public const string SCREW = "Screw";
        /// <summary>
        /// The statorassembly
        /// </summary>
        public const string STATORASSEMBLY = "Stator Assembly";
        /// <summary>
        /// The terminalboard
        /// </summary>
        public const string TERMINALBOARD = "Terminal Board";
        /// <summary>
        /// The terminalbox
        /// </summary>
        public const string TERMINALBOX = "Terminal Box";
        /// <summary>
        /// The terminalboxlid
        /// </summary>
        public const string TERMINALBOXLID = "Terminal Box Lid";
        /// <summary>
        /// The terminalboxlidgasket
        /// </summary>
        public const string TERMINALBOXLIDGASKET = "Terminal Box Lid Gasket";
        /// <summary>
        /// The washer
        /// </summary>
        public const string WASHER = "Washer";
        /// <summary>
        /// The greasenibble
        /// </summary>
        public const string GREASENIBBLE = "Grease Nibble";
        /// <summary>
        /// The greasedrain
        /// </summary>
        public const string GREASEDRAIN = "Grease Drain";
        /// <summary>
        /// The heater
        /// </summary>
        public const string HEATER = "Heater";
        /// <summary>
        /// The slipring
        /// </summary>
        public const string SLIPRING = "Slipring";
        /// <summary>
        /// The commutator
        /// </summary>
        public const string COMMUTATOR = "Commutator";
        /// <summary>
        /// The brushes
        /// </summary>
        public const string BRUSHES = "Brushes";
        /// <summary>
        /// The brushholder
        /// </summary>
        public const string BRUSHHOLDER = "Brushholder";
        /// <summary>
        /// The brushcompartment
        /// </summary>
        public const string BRUSHCOMPARTMENT = "Brush Compartment";
        /// <summary>
        /// The light
        /// </summary>
        public const string LIGHT = "Light";
        /// <summary>
        /// The switch
        /// </summary>
        public const string SWITCH = "Switch";
        /// <summary>
        /// The coolerstator
        /// </summary>
        public const string COOLERSTATOR = "Cooler Stator";
        /// <summary>
        /// The coolerslipring
        /// </summary>
        public const string COOLERSLIPRING = "Cooler Slipring";
        /// <summary>
        /// The nameplate
        /// </summary>
        public const string NAMEPLATE = "Nameplate";

    }
    /// <summary>
    /// Enum MotorMaintainenance
    /// </summary>
    public enum MotorMaintainenance
    {
        /// <summary>
        /// The rewind
        /// </summary>
        REWIND,
        /// <summary>
        /// The replacebearingde
        /// </summary>
        REPLACEBEARINGDE,
        /// <summary>
        /// The replacebearingnde
        /// </summary>
        REPLACEBEARINGNDE,
        /// <summary>
        /// The replacebearings
        /// </summary>
        REPLACEBEARINGS,
        /// <summary>
        /// The rotorbalance
        /// </summary>
        ROTORBALANCE,
        /// <summary>
        /// The replacerotor
        /// </summary>
        REPLACEROTOR,
        /// <summary>
        /// The overhaul
        /// </summary>
        OVERHAUL,
        /// <summary>
        /// The cleanmotor
        /// </summary>
        CLEANMOTOR,
        /// <summary>
        /// The replacefan
        /// </summary>
        REPLACEFAN,
        /// <summary>
        /// The replaceseal
        /// </summary>
        REPLACESEAL,
        /// <summary>
        /// The fixfeet
        /// </summary>
        FIXFEET,
        /// <summary>
        /// The fixendshield
        /// </summary>
        FIXENDSHIELD,
        /// <summary>
        /// The fixendshields
        /// </summary>
        FIXENDSHIELDS,
        /// <summary>
        /// The fixfancover
        /// </summary>
        FIXFANCOVER,
        /// <summary>
        /// The replacecirclip
        /// </summary>
        REPLACECIRCLIP,
        /// <summary>
        /// The fixterminalbox
        /// </summary>
        FIXTERMINALBOX,
        /// <summary>
        /// The fixslipring
        /// </summary>
        FIXSLIPRING,
        /// <summary>
        /// The fixcommutator
        /// </summary>
        FIXCOMMUTATOR,
        /// <summary>
        /// The replacebrushes
        /// </summary>
        REPLACEBRUSHES,
        /// <summary>
        /// The fixbrushholder
        /// </summary>
        FIXBRUSHHOLDER,
        /// <summary>
        /// The statorinsulationtest
        /// </summary>
        STATORINSULATIONTEST,
        /// <summary>
        /// The rotorinsulationtest
        /// </summary>
        ROTORINSULATIONTEST,
        /// <summary>
        /// The checkconnections
        /// </summary>
        CHECKCONNECTIONS,
        /// <summary>
        /// The checkbearing
        /// </summary>
        CHECKBEARING,
        /// <summary>
        /// The checkvibration
        /// </summary>
        CHECKVIBRATION,
        /// <summary>
        /// The checkbrushes
        /// </summary>
        CHECKBRUSHES,
        /// <summary>
        /// The checkoverheat
        /// </summary>
        CHECKOVERHEAT,
        /// <summary>
        /// The checkbrushholder
        /// </summary>
        CHECKBRUSHHOLDER,
        /// <summary>
        /// The grease
        /// </summary>
        GREASE,
        /// <summary>
        /// The checkcurrent
        /// </summary>
        CHECKCURRENT,
        /// <summary>
        /// The checkvoltage
        /// </summary>
        CHECKVOLTAGE,
        /// <summary>
        /// The checkspeed
        /// </summary>
        CHECKSPEED
    }
    /// <summary>
    /// Struct MotorMaintainenanceDetails
    /// </summary>
    public struct MotorMaintainenanceDetails
    {

        /// <summary>
        /// The rewind
        /// </summary>
        public const string REWIND = "Rewind Stator";
        /// <summary>
        /// The replacebearingde
        /// </summary>
        public const string REPLACEBEARINGDE = "Replace motor bearing DE";
        /// <summary>
        /// The replacebearingnde
        /// </summary>
        public const string REPLACEBEARINGNDE = "Replace motor bearing NDE";
        /// <summary>
        /// The replacebearings
        /// </summary>
        public const string REPLACEBEARINGS = "Replace motor bearings";
        /// <summary>
        /// The rotorbalance
        /// </summary>
        public const string ROTORBALANCE = "Balance rotor";
        /// <summary>
        /// The replacerotor
        /// </summary>
        public const string REPLACEROTOR = "Replace rotor";
        /// <summary>
        /// The overhaul
        /// </summary>
        public const string OVERHAUL = "Overhaul Motor";
        /// <summary>
        /// The cleanmotor
        /// </summary>
        public const string CLEANMOTOR = "Clean Motor";
        /// <summary>
        /// The replacefan
        /// </summary>
        public const string REPLACEFAN = "Replace fan";
        /// <summary>
        /// The replaceseal
        /// </summary>
        public const string REPLACESEAL = "Replace seal";
        /// <summary>
        /// The fixfeet
        /// </summary>
        public const string FIXFEET = "Fix feet";
        /// <summary>
        /// The fixendshieldde
        /// </summary>
        public const string FIXENDSHIELDDE = "Fix Endshield DE";
        /// <summary>
        /// The fixendshieldnde
        /// </summary>
        public const string FIXENDSHIELDNDE = "Fix Endshield NDE";
        /// <summary>
        /// The fixendshields
        /// </summary>
        public const string FIXENDSHIELDS = "Fix Endshields";
        /// <summary>
        /// The fixfancover
        /// </summary>
        public const string FIXFANCOVER = "Fix fan Cover";
        /// <summary>
        /// The replacecirclip
        /// </summary>
        public const string REPLACECIRCLIP = "Replace Circlip";
        /// <summary>
        /// The fixterminalbox
        /// </summary>
        public const string FIXTERMINALBOX = "Fix Terminal Box";
        /// <summary>
        /// The fixslipring
        /// </summary>
        public const string FIXSLIPRING = "Fix Slipring";
        /// <summary>
        /// The fixcommutator
        /// </summary>
        public const string FIXCOMMUTATOR = "Fix Commutator";
        /// <summary>
        /// The replacebrushes
        /// </summary>
        public const string REPLACEBRUSHES = "Replace Brushes";
        /// <summary>
        /// The fixbrushholder
        /// </summary>
        public const string FIXBRUSHHOLDER = "Fix Brush Holder";
        /// <summary>
        /// The statorinsulationtest
        /// </summary>
        public const string STATORINSULATIONTEST = "Stator Insulation Test";
        /// <summary>
        /// The rotorinsulationtest
        /// </summary>
        public const string ROTORINSULATIONTEST = "Rotor Insulation Test";
        /// <summary>
        /// The checkconnections
        /// </summary>
        public const string CHECKCONNECTIONS = "Check Connections";
        /// <summary>
        /// The checkbearing
        /// </summary>
        public const string CHECKBEARING = "Check Bearing";
        /// <summary>
        /// The checkvibration
        /// </summary>
        public const string CHECKVIBRATION = "Check Vibration";
        /// <summary>
        /// The checkbrushes
        /// </summary>
        public const string CHECKBRUSHES = "Check Brushes";
        /// <summary>
        /// The checkoverheat
        /// </summary>
        public const string CHECKOVERHEAT = "Check Overheat";
        /// <summary>
        /// The checkbrushholder
        /// </summary>
        public const string CHECKBRUSHHOLDER = "Check Brushholder";
        /// <summary>
        /// The grease
        /// </summary>
        public const string GREASE = "Grease";
        /// <summary>
        /// The greasede
        /// </summary>
        public const string GREASEDE = "Grease Drive End";
        /// <summary>
        /// The greasende
        /// </summary>
        public const string GREASENDE = "Grease Non Drive End";
        /// <summary>
        /// The checkcurrent
        /// </summary>
        public const string CHECKCURRENT = "Check Current";
        /// <summary>
        /// The checkvoltage
        /// </summary>
        public const string CHECKVOLTAGE = "Check Voltage";
        /// <summary>
        /// The checkspeed
        /// </summary>
        public const string CHECKSPEED = "Check Speed";
        /*public static string MaintenanceDetails(Maintainenance maint)
        {
            var values = typeof(Motor.MaintainenanceDetails).GetFields(BindingFlags.Static | BindingFlags.Public)
                                   .Where(x => x.IsLiteral && !x.IsInitOnly)
                                   .Select(x => x.GetValue(null)).Cast<string>();
            if (values.Contains(maint.ToString()))
                return values.First();
            else
                return string.Empty;
        }*/

    }
    #endregion
    #endregion
    #region Logistics
    /// <summary>
    /// Class Inventory.
    /// Implements the <see cref="Realms.RealmObject" />
    /// </summary>
    /// <seealso cref="Realms.RealmObject" />
    public class Inventory : RealmObject
    {
        #region Fields

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public string Code { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets the inventory level.
        /// </summary>
        /// <value>The inventory level.</value>
        [Ignored]
        public double InventoryLevel
        {
            get
            {
                /* object tempObject = EvaluateAlias(nameof(Amount));
                 if (tempObject != null)
                 {
                     return (double)tempObject;
                 }
                 else
                 {
                     return 0;
                 }*/
                return (from x in Spares select x.Value).Sum();
            }
        }

        /// <summary>
        /// Gets the spares.
        /// </summary>
        /// <value>The spares.</value>
        public IList<Spare> Spares { get; }

        /// <summary>
        /// Gets the bussiness unit back reference.
        /// </summary>
        /// <value>The bussiness unit back reference.</value>
        [Backlink(nameof(Plant.Inventories))]
        public IQueryable<Plant> BussinessUnitBackRef { get; }

        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="Inventory"/> class.
        /// </summary>
        public Inventory() { }
        /// <summary>
        /// Adds a spare part.
        /// </summary>
        /// <param name="code">inventory code (mandatory).</param>
        /// <param name="Desc1">The desc1.</param>
        /// <param name="description2">The description2.</param>
        /// <param name="brand">The brand.</param>
        /// <param name="brandtype">The brandtype.</param>
        /// <param name="onhand">The onhand.</param>
        /// <param name="unit">The unit.</param>
        /// <param name="valueEGP">The value egp.</param>
        /// <param name="traked">if set to <c>true</c> [traked].</param>
        /// <param name="minqty">The minqty.</param>
        /// <param name="extradata">default is empty</param>
        /// <returns>True if updates without problems.</returns>
        /// <exception cref="System.Exception">Thrown but not handled
        /// .</exception>
        /// See <see cref="Realm.Add(RealmObject, bool)" /> to add realm object.
        public void SpareAdd(string code, string Desc1, string description2 = "",
                                    string brand = "", string brandtype = "", double onhand = 0,
                                    string unit = "", double valueEGP = 0, bool traked = false, double minqty = 0, string extradata = "")
        {
            try
            {
                if (Spares.Where(x => x.Code == code).Any())
                    throw new Exception("Item is repeated");
                Spare sp = new Spare()
                {
                    Code = code,
                    Description1 = Desc1,
                    Brand = brand,
                    BrandType = brandtype,
                    OnHand = onhand,
                    Unit = unit,
                    Value = valueEGP,
                    TrackItem = traked,
                    MinQty = minqty,
                    ExtraData = extradata
                };

                DBManager.realm.Write(() =>
                {
                    Spares.Add(sp);
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Inventory.SpareAdd(Spare)'
        public void SpareAdd(Spare spare)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Inventory.SpareAdd(Spare)'
        {
            try
            {
                if (Spares.Where(x => x.Code == spare.Code).Any())
                    throw new Exception("Item is repeated");
                Spare sp = new Spare()
                {
                    Code = spare.Code,
                    Description1 = spare.Description1,
                    Description2 = spare.Description2,
                    Brand = spare.Brand,
                    BrandType = spare.BrandType,
                    OnHand = spare.OnHand,
                    Unit = spare.Unit,
                    Value = spare.Value,
                    TrackItem = spare.TrackItem,
                    MinQty = spare.MinQty,
                    ExtraData = spare.ExtraData
                };

                DBManager.realm.Write(() =>
                {
                    Spares.Add(sp);
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Spares the remove.
        /// </summary>
        /// <param name="spare">The spare.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public void SpareRemove(Spare spare)
        {

            try
            {
                if (Spares.Where(x => x.Code == spare.Code).Any())
                    throw new Exception("Item is repeated");
                DBManager.realm.Write(() =>
                {
                    DBManager.realm.Remove(spare);
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Spares the remove.
        /// </summary>
        /// <param name="spareCode">The spare code.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public void SpareRemove(string spareCode)
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    if (Spares.Where(s => s.Code == spareCode).Any())
                        DBManager.realm.Remove(Spares.Where(s => s.Code == spareCode).First());
                    else
                        throw new Exception("Spare cannot be found in inventory!");
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Spares the remove many.
        /// </summary>
        /// <param name="sparesList">The spares list.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public void SpareRemoveMany(List<Spare> sparesList)
        {
            foreach (Spare sp in sparesList)
                SpareRemove(sp);
        }
        /// <summary>
        /// Spares the remove many.
        /// </summary>
        /// <param name="spareCodes">The spare codes.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public void SpareRemoveMany(string[] spareCodes)
        {
            foreach (string sp in spareCodes)
                SpareRemove(sp);
        }
        /// <summary>
        /// Spares the remove all.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public void SpareRemoveAll()
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    DBManager.realm.RemoveAll<Spare>();
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Spares the withdraw.
        /// </summary>
        /// <param name="spare">The spare.</param>
        /// <param name="qty">The qty.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public void SpareWithdraw(Spare spare, double qty)
        {
            try
            {
                double onhand = Spares.Where(s => s.Code == spare.Code).First().OnHand;

                if (Spares.Where(s => s.Code == spare.Code).Any())
                    throw new Exception("Cannot find spare in inventory!");
                else if (onhand < qty)
                    throw new Exception("On hand is less than needed quantity!");
                else
                {
                    var x = from sp in Spares
                            where sp.Code.Equals(spare)
                            select sp;
                    if (spare != null)
                    {
                        DBManager.realm.Write(() =>
                        {
                            var spareMod = Spares.Where(s => s.Code == spare.Code).First();
                            spareMod.OnHand = onhand - qty; ;
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
    /// <summary>
    /// Class Spare.
    /// Implements the <see cref="Realms.RealmObject" />
    /// </summary>
    /// <seealso cref="Realms.RealmObject" />
    public class Spare : RealmObject
    {
        #region Fields

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = false, ErrorMessage = "Code cannot be empty!")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Inventory Code is not valid, should be 10 numbers!")]
        public string Code { get; set; }
        /// <summary>
        /// Gets or sets the description1.
        /// </summary>
        /// <value>The description1.</value>
        public string Description1 { get; set; }
        /// <summary>
        /// Gets or sets the description2.
        /// </summary>
        /// <value>The description2.</value>
        public string Description2 { get; set; }
        /// <summary>
        /// Gets or sets the brand.
        /// </summary>
        /// <value>The brand.</value>
        public string Brand { get; set; }
        /// <summary>
        /// Gets or sets the type of the brand.
        /// </summary>
        /// <value>The type of the brand.</value>
        public string BrandType { get; set; }
        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public string Location { get; set; }
        /// <summary>
        /// Gets or sets the on hand.
        /// </summary>
        /// <value>The on hand.</value>
        [Range(0, double.MaxValue)]
        public double OnHand { get; set; }
        /// <summary>
        /// Gets or sets the unit.
        /// </summary>
        /// <value>The unit.</value>
        public string Unit { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [Range(0, double.MaxValue)]
        public double Value { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [track item].
        /// </summary>
        /// <value><c>true</c> if [track item]; otherwise, <c>false</c>.</value>
        public bool TrackItem { get; set; }
        /// <summary>
        /// Gets or sets the minimum qty.
        /// </summary>
        /// <value>The minimum qty.</value>
        [Range(0, double.MaxValue)]
        public double MinQty { get; set; }
        /// <summary>
        /// Gets or sets the extra data.
        /// </summary>
        /// <value>The extra data.</value>
        public string ExtraData { get; set; }

        /// <summary>
        /// Gets the inventory back reference.
        /// </summary>
        /// <value>The inventory back reference.</value>
        [Backlink(nameof(Inventory.Spares))]
        public IQueryable<Inventory> InventoryBackRef { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Spare"/> class.
        /// </summary>
        public Spare() { }
        #endregion
    }

    #endregion
    #region HR
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Department'
    public class Department : RealmObject
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Department'
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        /// <summary>
        /// Gets or sets the partition.
        /// </summary>
        /// <value>The partition.</value>
        [MapTo("partition")]
        public string Partition { get; set; } = "Public";
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public string Description { get; set; }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Department.Department()'
        public Department() { }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Department.Department()'
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'DepartmentSection'
    public class DepartmentSection : RealmObject
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'DepartmentSection'
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        /// <summary>
        /// Gets or sets the partition.
        /// </summary>
        /// <value>The partition.</value>
        [MapTo("partition")]
        public string Partition { get; set; } = "Public";
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public string Description { get; set; }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'DepartmentSection.DepartmentSection()'
        public DepartmentSection()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'DepartmentSection.DepartmentSection()'
        {

        }
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee'
    public class Employee : RealmObject
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee'
    {
        #region Fields
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        /// <summary>
        /// Gets or sets the partition.
        /// </summary>
        /// <value>The partition.</value>
        [MapTo("partition")]
        public string Partition { get; set; } = "Public";
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public string Company { get; set; }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.CompanyCode'
        public string CompanyCode { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.CompanyCode'
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        public string Department { get; set; }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.DepartmentSection'
        public string DepartmentSection { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.DepartmentSection'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.FirstName'
        public string FirstName { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.FirstName'
        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        public string LastName { get; set; }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.FullNameEN'
        public string FullNameEN { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.FullNameEN'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.FullNameAr'
        public string FullNameAr { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.FullNameAr'

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [StringRange(AllowableValues = new[] { "Manager", "Engineer", "Supervisor", "Technician", "Labor" })]
        public DateTimeOffset BirthDate { get; set; }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.Title'
        public string Title { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.Title'
        /// <summary>
        /// Gets the public holidays.
        /// </summary>
        /// <value>The public holidays.</value>
        [MapTo("UserName")]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = false, ErrorMessage = "User Name cannot be empty!")]
        [StringLength(int.MaxValue, MinimumLength = 1, ErrorMessage = "User Name letters are very few!")]
        public string UserName { get; set; }
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        [MapTo("Email")]
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                            ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        [MapTo("Password")]
        [StringLength(maximumLength: 8, MinimumLength = 8, ErrorMessage = "Password Length must be 8 characters.")]
        [PasswordPropertyText]
        public string Password { get; set; } = "12345678";
        /// <summary>
        /// Gets or sets the photo of the employee.
        /// </summary>
        /// <value>The face.</value>
        [MapTo("Face")]
        public string Face { get; set; }
        [StringRange(AllowableValues = new[] { "FullTime", "PartTime" })]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.ContractType'
        public string ContractType { get; set; } = "FullTime";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.ContractType'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.HrsPerDay'
        public double HrsPerDay { get; set; } = 8;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.HrsPerDay'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.HoursPerMonth'
        public double HoursPerMonth { get; set; } = 210; //hours
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.HoursPerMonth'
        [Range(0, double.MaxValue)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.PayNetContractedFixed'
        public double PayNetContractedFixed { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.PayNetContractedFixed'
        [Range(0, double.MaxValue)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.PayNetContractedVariable'
        public double PayNetContractedVariable { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.PayNetContractedVariable'
        [Range(0, 52)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.LeaveContracted'
        public double LeaveContracted { get; set; } = 21;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.LeaveContracted'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.LeaveBalance'
        public double LeaveBalance { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.LeaveBalance'
        #endregion
        #region Methods
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.Employee()'
        public Employee()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.Employee()'
        {

        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.Attendances'
        public IList<Attendance> Attendances { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.Attendances'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.AttendanceAdd(Attendance)'
        public void AttendanceAdd(Attendance eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.AttendanceAdd(Attendance)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    Attendances.Add(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.AttendanceDelete(Attendance)'
        public void AttendanceDelete(Attendance eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.AttendanceDelete(Attendance)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    DBManager.realm.Remove(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.WorkDaysCount(int, int)'
        public double WorkDaysCount(int year, int mnth)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.WorkDaysCount(int, int)'
        {
            // Make Arabic (Egypt) the current culture
            // and Umm al-Qura calendar the current calendar.
            CultureInfo arEG = CultureInfo.CreateSpecificCulture("ar-EG");
            Calendar cal = new UmAlQuraCalendar();
            arEG.DateTimeFormat.Calendar = cal;
            Thread.CurrentThread.CurrentCulture = arEG;

            return 0;
            /*var ret = Attendances.Where(x => x.InDate.Year == year && x.InDate.Month == mnth).ToList();
            var x2 =  (from k in ret select k.OutDate - k.InDate).Sum(i => i.Days);
            return x2;*/
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.DaysShouldAttendPerMonth()'
        public double DaysShouldAttendPerMonth()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.DaysShouldAttendPerMonth()'
        {
            if (HrsPerDay <= 0)
                throw new Exception("Hours per day must be greater than zero!");
            return HoursPerMonth / HrsPerDay;
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.HoursPerMonthExHolidays()'
        public double HoursPerMonthExHolidays()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.HoursPerMonthExHolidays()'
        {

            var days = 0; ;
            foreach (var x in EGYPublicHolidays())
            {
                if (int.Parse(x.Key.Substring(1, 2)) == DateTime.Now.Month)
                    days++;
            }
            return 210 - days;
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.EGYPublicHolidays()'
        public Dictionary<string, string> EGYPublicHolidays()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.EGYPublicHolidays()'
        {

            return new Dictionary<string, string>()
                {
                    { "0107", "Coptic Christmas Day" },
                    { "0125", "Revolution Day January 25" },
                    { "0425", "Sinai Liberation Day" },
                    { "0501", "Labour Day" },
                    { "0502", "Coptic Easter" },
                    { "0503", "Sham el Nessim" },
                    { "0513", "Eid Al Fitr" },
                    { "0720", "Eid Al Adha" },
                    { "0723", "Revolution Day July 23" },
                    { "0810", "Islamic New Year" },
                    { "1006", "Armed Forces Day" },
                    { "1019", "The Prophet's Birthday" }
                };
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.AttendanceMonthDays()'
        public double AttendanceMonthDays()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.AttendanceMonthDays()'
        {
            int xadd = 0;
            foreach (var x in Attendances)
            {
                if ((x.OutDate - x.InDate).Days > 0)
                    xadd = xadd + ((x.OutDate - x.InDate).Days);
            }
            return xadd;
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.AttendanceMonthHours()'
        public double AttendanceMonthHours()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.AttendanceMonthHours()'
        {

            int xadd = 0;
            foreach (var x in Attendances)
            {
                if ((x.OutDate - x.InDate).Hours > 0)
                    xadd = xadd + ((x.OutDate - x.InDate).Hours);
            }
            return xadd;
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.AttendanceMonthHoursExOverTime()'
        public double AttendanceMonthHoursExOverTime()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.AttendanceMonthHoursExOverTime()'
        {
            int xadd = 0;
            foreach (var x in Attendances)
            {
                if ((x.OutDate - x.InDate).Hours > 0 && (x.OutDate - x.InDate).Hours <= 8
                    && !(x.InDate.DayOfWeek == DayOfWeek.Saturday || x.InDate.DayOfWeek == DayOfWeek.Friday))
                    xadd = xadd + ((x.OutDate - x.InDate).Hours);
                else if ((x.OutDate - x.InDate).Hours > 0 && (x.OutDate - x.InDate).Hours > 8
                    && !(x.InDate.DayOfWeek == DayOfWeek.Saturday || x.InDate.DayOfWeek == DayOfWeek.Friday))
                    xadd = xadd + 8;
            }
            return xadd;
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.AttendanceOverTimeHoursExWeekEnd()'
        public double AttendanceOverTimeHoursExWeekEnd()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.AttendanceOverTimeHoursExWeekEnd()'
        {

            int xadd = 0;
            foreach (var x in Attendances)
            {
                if ((x.OutDate - x.InDate).Hours - 8 > 0 && !(x.InDate.DayOfWeek == DayOfWeek.Saturday || x.InDate.DayOfWeek == DayOfWeek.Friday))
                    xadd = xadd + ((x.OutDate - x.InDate).Hours - 8);
            }
            return xadd;
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.AttendanceOverTimeHoursWeekEnd()'
        public double AttendanceOverTimeHoursWeekEnd()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.AttendanceOverTimeHoursWeekEnd()'
        {
            int xadd = 0;
            foreach (var x in Attendances)
            {
                if ((x.OutDate - x.InDate).Hours - 8 > 0 && (x.InDate.DayOfWeek == DayOfWeek.Saturday || x.InDate.DayOfWeek == DayOfWeek.Friday))
                    xadd = xadd + ((x.OutDate - x.InDate).Hours);
            }
            return xadd;
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.PayNetAttended()'
        public double PayNetAttended()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.PayNetAttended()'
        {

            return (AttendanceMonthHours() / HoursPerMonth) * (PayNetContractedFixed + PayNetContractedVariable);
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.PayGross()'
        public double PayGross()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.PayGross()'
        {
            return 0;
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.PayRaise(double, double)'
        public void PayRaise(double fixedRaisePercent, double VariableRaisePercent)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.PayRaise(double, double)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    PayNetContractedFixed = PayNetContractedFixed * fixedRaisePercent + PayNetContractedFixed;
                    VariableRaisePercent = VariableRaisePercent * fixedRaisePercent + VariableRaisePercent;
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        #endregion
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.Months'
        public enum Months
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.Months'
        {
            /// <summary>
            /// The jan
            /// </summary>
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.Months.Dec'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.Months.Oct'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.Months.Apr'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.Months.Aug'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.Months.Jun'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.Months.Feb'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.Months.May'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.Months.Jul'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.Months.Mar'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.Months.Sep'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Employee.Months.Nov'
            Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.Months.Nov'
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.Months.Sep'
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.Months.Mar'
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.Months.Jul'
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.Months.May'
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.Months.Feb'
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.Months.Jun'
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.Months.Aug'
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.Months.Apr'
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.Months.Oct'
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Employee.Months.Dec'
        }
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Attendance'
    public class Attendance : RealmObject
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Attendance'
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        /// <summary>
        /// Gets or sets the partition.
        /// </summary>
        /// <value>The partition.</value>
        [MapTo("partition")]
        public string Partition { get; set; } = "Public";
        /// <summary>
        /// Gets or sets the in date.
        /// </summary>
        /// <value>The in date.</value>
        public DateTimeOffset InDate { get; set; }
        /// <summary>
        /// Gets or sets the out date.
        /// </summary>
        /// <value>The out date.</value>
        [DateGreaterThan("InDate")]
        public DateTimeOffset OutDate { get; set; }
        [Ignored]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Attendance.IsPresent'
        public bool IsPresent
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Attendance.IsPresent'
        {
            get
            {
                if (AttendanceState != string.Empty && AttendanceState == "Present")
                    return true;
                else
                    return false;
            }
        }
        [StringRange(AllowableValues = new[] { "Absent", "BussinessNeed" , "Compensation", "Deduct1", "Deduct3",
            "Exam", "Haj", "Military", "Normal", "Normal" , "Present", "Sick", "Unpaid", "Vacation" ,"Voting","WeekEnd"})]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Attendance.AttendanceState'
        public string AttendanceState { get; set; } = "Present";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Attendance.AttendanceState'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Attendance.Attendance()'
        public Attendance() { }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Attendance.Attendance()'
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Taxes'
    public class Taxes : RealmObject
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Taxes'
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        /// <summary>
        /// Gets or sets the partition.
        /// </summary>
        /// <value>The partition.</value>
        [MapTo("partition")]
        public string Partition { get; set; } = "Public";
        /// <summary>
        /// Gets or sets the employee personal taxes free.
        /// </summary>
        /// <value>The employee personal taxes free.</value>
        public double EmployeePersonalTaxesFree { get; set; } = 9000;
        /// <summary>
        /// Gets the limits.
        /// </summary>
        /// <value>The limits.</value>
        public IList<double> Limits { get; }
        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        public IList<double> Values { get; }

        /// <summary>
        /// Gets or sets the income.
        /// </summary>
        /// <value>The income.</value>
        double Income { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="Taxes"/> class.
        /// </summary>
        public Taxes() { }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Taxes.GetValues(List<double>, List<double>, double)'
        public void GetValues(List<double> limits = null, List<double> values = null, double _EmployeePersonalTaxesFree = 0)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Taxes.GetValues(List<double>, List<double>, double)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    EmployeePersonalTaxesFree = _EmployeePersonalTaxesFree;
                    if (Limits == null || Limits.Count == 0 && limits == null)
                    {
                        Limits.Add(15000); Limits.Add(30000); Limits.Add(45000); Limits.Add(60000); Limits.Add(200000); Limits.Add(400000);
                        Values.Add(0.025); Values.Add(0.1); Values.Add(.15); Values.Add(0.2); Values.Add(0.22); Values.Add(0.25);
                    }
                    else if (Limits == null && limits != null)
                    {
                        foreach (var lmt in limits)
                            Limits.Add(lmt);
                        foreach (var vlas in values)
                            Values.Add(vlas);
                    }
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Gets the taxes.
        /// </summary>
        /// <returns>System.Double.</returns>
        public double GetTaxes()
        {
            //int[] numbers = Enumerable.Range(0, 100).ToArray();
            var AnnualIncome = Income * 12;

            for (int x = 0; x < Limits.Count; x++)
            {
                if (AnnualIncome < Limits[x])
                {
                    return (AnnualIncome - EmployeePersonalTaxesFree) * Values[x];
                }
            }
            return 0;
        }

    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SocialInsurance'
    public class SocialInsurance : RealmObject
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SocialInsurance'
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        /// <summary>
        /// Gets or sets the partition.
        /// </summary>
        /// <value>The partition.</value>
        [MapTo("partition")]
        public string Partition { get; set; } = "Public";
        /// <summary>
        /// Gets or sets the fixed maximum.
        /// </summary>
        /// <value>The fixed maximum.</value>
        public int FixedMax { get; set; } = 1510;
        /// <summary>
        /// Gets or sets the fixed minimum.
        /// </summary>
        /// <value>The fixed minimum.</value>
        public int FixedMin { get; set; } = 220;
        /// <summary>
        /// Gets or sets the variable maximum.
        /// </summary>
        /// <value>The variable maximum.</value>
        public int VariableMax { get; set; } = 3360;
        /// <summary>
        /// Gets or sets the fixed salary.
        /// </summary>
        /// <value>The fixed salary.</value>
        public double FixedSalary { get; set; }
        /// <summary>
        /// Gets or sets the variable salary.
        /// </summary>
        /// <value>The variable salary.</value>
        public double VariableSalary { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="SocialInsurance"/> class.
        /// </summary>
        public SocialInsurance() { }
        /// <summary>
        /// Gets the fixed person insurance.
        /// </summary>
        /// <returns>System.Double.</returns>
        public double GetFixedPersonInsurance()
        {
            switch (FixedSalary)
            {
                case double n when n >= FixedMax:
                    return FixedMax * 0.14;
                case double n when n < FixedMax:
                    return FixedSalary * 0.14;
            }
            return 0;
        }
        /// <summary>
        /// Gets the fixed company insurance.
        /// </summary>
        /// <returns>System.Double.</returns>
        public double GetFixedCompanyInsurance()
        {
            switch (FixedSalary)
            {
                case double n when n >= FixedMax:
                    return FixedMax * 0.26;
                case double n when n < FixedMax:
                    return FixedSalary * 0.26;
            }

            return 0;
        }
        /// <summary>
        /// Gets the variable person insurance.
        /// </summary>
        /// <returns>System.Double.</returns>
        public double GetVariablePersonInsurance()
        {
            switch (VariableSalary)
            {
                case double n when n >= VariableMax:
                    return VariableMax * 0.14;
                case double n when n < VariableMax:
                    return VariableSalary * 0.14;
            }
            return 0;

        }
        /// <summary>
        /// Gets the variable company insurance.
        /// </summary>
        /// <returns>System.Double.</returns>
        public double GetVariableCompanyInsurance()
        {
            switch (VariableSalary)
            {
                case double n when n >= VariableMax:
                    return VariableMax * 0.26;
                case double n when n < VariableMax:
                    return VariableSalary * 0.26;
            }
            return 0;

        }
    }

    #endregion
    #region Finance
    /// <summary>
    /// Class Budget.
    /// Implements the <see cref="Realms.RealmObject" />
    /// </summary>
    /// <seealso cref="Realms.RealmObject" />
    public class Budget : RealmObject
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        /// <summary>
        /// Gets or sets the partition.
        /// </summary>
        /// <value>The partition.</value>
        [MapTo("partition")]
        public string Partition { get; set; } = "Public";
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        /// <value>The maximum value.</value>
        [Ignored]
        public double MaxValue { get; set; }
        /// <summary>
        /// Gets the total value.
        /// </summary>
        /// <value>The total value.</value>
        [Ignored]
        public double TotalValue
        {
            get
            {
                return (from x in Items select x.Value).Sum();
            }
        }
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public IList<Budgetitem> Items { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Budget"/> class.
        /// </summary>
        public Budget() { }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Budget.BudgetitemAdd(Budgetitem)'
        public void BudgetitemAdd(Budgetitem eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Budget.BudgetitemAdd(Budgetitem)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    Items.Add(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Budget.BudgetitemDelete(Budgetitem)'
        public void BudgetitemDelete(Budgetitem eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Budget.BudgetitemDelete(Budgetitem)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    DBManager.realm.Remove(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
    /// <summary>
    /// The <c>Budgetitem</c> class.
    /// To add budget items.
    /// <list type="bullet"><item><term>Add</term><description>Addition Operation</description></item></list>
    /// Implements the <see cref="Realms.RealmObject" />
    /// </summary>
    /// <seealso cref="Realms.RealmObject" />
    /// <remarks>This class can add, subtract, multiply and divide.</remarks>
    public class Budgetitem : RealmObject
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        /// <summary>
        /// Gets or sets the partition.
        /// </summary>
        /// <value>The partition.</value>
        [MapTo("partition")]
        public string Partition { get; set; } = "Public";
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Budgetitem.Title'
        public string Title { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Budgetitem.Title'
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
        /// <summary>
        /// Budget item types  Service or Material.
        /// </summary>
        /// <value>The type of the item.</value>
        [StringRange(AllowableValues = new[] { "Material", "Spare" }, ErrorMessage = "Gender must be either 'Material' or 'Spare'.")]
        [StringLength(1, MinimumLength = 1, ErrorMessage = "The Gender must be 1 characters.")]
        public string ItemType { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [RequiredIf("ItemType',''Material'", ErrorMessage = "Part code is required as item type is material!")]
        public string PartCode { get; set; }
        [Range(1, double.MaxValue, ErrorMessage = "Please enter a value greater than {1}")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Budgetitem.Value'
        public double Value { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Budgetitem.Value'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Budgetitem.DateBudgetItem'
        public DateTimeOffset DateBudgetItem { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Budgetitem.DateBudgetItem'

        /// <summary>
        /// Gets the budget back reference.
        /// </summary>
        /// <value>The budget back reference.</value>
        [Backlink(nameof(Budget.Items))]
        public IQueryable<Budget> BudgetBackRef { get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="Budgetitem"/> class.
        /// </summary>
        public Budgetitem() { }
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Expenses'
    public class Expenses : RealmObject
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Expenses'
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        /// <summary>
        /// Gets or sets the partition.
        /// </summary>
        /// <value>The partition.</value>
        [MapTo("partition")]
        public string Partition { get; set; } = "Public";
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets the total value.
        /// </summary>
        /// <value>The total value.</value>
        [Ignored]
        public double TotalValue
        {
            get
            {
                return (from x in Items select x.Value).Sum();
            }
        }
        [Ignored]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Expenses.MaxValue'
        public double MaxValue
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Expenses.MaxValue'
        {
            get
            {
                return (from x in Items select x.Value).Max();
            }
        }
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public IList<ExpensItem> Items { get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="Budget"/> class.
        /// </summary>
        public Expenses() { }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Expenses.BudgetitemAdd(ExpensItem)'
        public void BudgetitemAdd(ExpensItem eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Expenses.BudgetitemAdd(ExpensItem)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    Items.Add(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Expenses.BudgetitemDelete(ExpensItem)'
        public void BudgetitemDelete(ExpensItem eq)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Expenses.BudgetitemDelete(ExpensItem)'
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    DBManager.realm.Remove(eq);
                });

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ExpensItem'
    public class ExpensItem : RealmObject
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ExpensItem'
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        /// <summary>
        /// Gets or sets the partition.
        /// </summary>
        /// <value>The partition.</value>
        [MapTo("partition")]
        public string Partition { get; set; } = "Public";
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ExpensItem.Title'
        public string Title { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ExpensItem.Title'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ExpensItem.Description'
        public string Description { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ExpensItem.Description'
        /// <summary>
        /// Budget item types  Service or Material.
        /// </summary>
        /// <value>The type of the item.</value>
        [StringRange(AllowableValues = new[] { "Material", "Spare" }, ErrorMessage = "Gender must be either 'Material' or 'Spare'.")]
        [StringLength(1, MinimumLength = 1, ErrorMessage = "The Gender must be 1 characters.")]
        public string ItemType { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [RequiredIf("ItemType',''Material'", ErrorMessage = "Part code is required as item type is material!")]
        public string PartCode { get; set; }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ExpensItem.DateExpense'
        public DateTimeOffset DateExpense { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ExpensItem.DateExpense'
        [Range(1, double.MaxValue, ErrorMessage = "Please enter a value greater than {1}")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ExpensItem.Value'
        public double Value { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ExpensItem.Value'
        /// <summary>
        /// Gets the budget back reference.
        /// </summary>
        /// <value>The budget back reference.</value>
        [Backlink(nameof(Expenses.Items))]
        public IQueryable<Expenses> ExpenssesBackRef { get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="Budgetitem"/> class.
        /// </summary>
        public ExpensItem() { }
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Contract'
    public class Contract : RealmObject
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Contract'
    {
        [PrimaryKey]
        [MapTo("_id")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Contract.Id'
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Contract.Id'
        [MapTo("partition")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Contract.Partition'
        public string Partition { get; set; } = "Public";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Contract.Partition'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Contract.Description'
        public string Description { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Contract.Description'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Contract.VendorCode'
        public string VendorCode { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Contract.VendorCode'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Contract.VendorName'
        public string VendorName { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Contract.VendorName'
        [StringRange(AllowableValues = new[] { "Service Provider", "Parts Supplier", "Manufacturer", "Distributor", "Contractor", "Consultant", "Miscellaneous" })]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Contract.VendorType'
        public string VendorType { get; set; } = "Miscellaneous";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Contract.VendorType'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Contract.VendorDescription'
        public string VendorDescription { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Contract.VendorDescription'

        [Ignored]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Contract.TotalValue'
        public double TotalValue
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Contract.TotalValue'
        {
            get
            {
                return Items.ToList().Select(x => x.EGPValue).Sum();
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Contract.Items'
        public IList<ContractItem> Items { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Contract.Items'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Contract.Contract()'
        public Contract() { }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Contract.Contract()'
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ContractItem'
    public class ContractItem : RealmObject
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ContractItem'
    {
        [PrimaryKey]
        [MapTo("_id")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ContractItem.Id'
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ContractItem.Id'

        [MapTo("partition")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ContractItem.Partition'
        public string Partition { get; set; } = "Public";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ContractItem.Partition'
        [StringRange(AllowableValues = new[] { "Material", "Service" })]
        [StringLength(1, MinimumLength = 1, ErrorMessage = "The Gender must be 1 characters.")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ContractItem.ItemType'
        public string ItemType { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ContractItem.ItemType'

        [RequiredIf("ItemType',''Material'", ErrorMessage = "Part code is required as item type is material!")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ContractItem.PartCode'
        public string PartCode { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ContractItem.PartCode'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ContractItem.Description1'
        public string Description1 { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ContractItem.Description1'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ContractItem.Description2'
        public string Description2 { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ContractItem.Description2'
        [Range(0, double.MaxValue)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ContractItem.EGPValue'
        public double EGPValue { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ContractItem.EGPValue'
        [Range(0, double.MaxValue)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ContractItem.QtyContracted'
        public double QtyContracted { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ContractItem.QtyContracted'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ContractItem.QtyUsed'
        public double QtyUsed { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ContractItem.QtyUsed'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ContractItem.Unit'
        public string Unit { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ContractItem.Unit'

        [Backlink(nameof(Contract.Items))]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ContractItem.ContractBackRef'
        public IQueryable<Contract> ContractBackRef { get; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ContractItem.ContractBackRef'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ContractItem.ContractItem()'
        public ContractItem() { }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ContractItem.ContractItem()'
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Vendor'
    public class Vendor
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Vendor'
    {
        [PrimaryKey]
        [MapTo("_id")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Vendor.Id'
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Vendor.Id'
        [MapTo("partition")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Vendor.Partition'
        public string Partition { get; set; } = "Public";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Vendor.Partition'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Vendor.Code'
        public string Code { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Vendor.Code'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Vendor.Name'
        public string Name { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Vendor.Name'
        [StringRange(AllowableValues = new[] { "Service Provider", "Parts Supplier", "Manufacturer", "Distributor", "Contractor", "Consultant", "Miscellaneous" })]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Vendor.Type'
        public string Type { get; set; } = "Miscellaneous";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Vendor.Type'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Vendor.Description'
        public string Description { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Vendor.Description'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Vendor.Vendor()'
        public Vendor() { }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Vendor.Vendor()'
    }
    #endregion

}