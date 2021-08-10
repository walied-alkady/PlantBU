// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 05-06-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-02-2021
// ***********************************************************************
// <copyright file="MotorPage.xaml.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using PlantBU.DataModel;
using PlantBU.ViewModels;
using Syncfusion.SfAutoComplete.XForms;
using Syncfusion.XForms.ComboBox;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace PlantBU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MotorPage : ContentPage
    {
        /// <summary>
        /// Gets or sets the motor.
        /// </summary>
        /// <value>The motor.</value>
        Motor motor { get; set; }
        ToolbarItem GetData;
        public MotorPage(Motor mtr = null, bool IsEnabled = false)
        {
            InitializeComponent();
            try
            {
                (BindingContext as MotorViewModel).page = this;
                (BindingContext as MotorViewModel).Navigation = Navigation;
                (BindingContext as MotorViewModel).Motor = mtr;
                (BindingContext as MotorViewModel).IsEnabled = IsEnabled;
                if (DBManager.CurrentUserType == DBManager.Usertyypes.Admin)
                {

                    

                }
                GetData = new ToolbarItem()
                {
                    Text = Properties.Resources.GetSiteData,
                    Order = ToolbarItemOrder.Secondary,
                    Priority = 2,
                    StyleId = "GetData"
                };
                GetData.Clicked += ToolbarItem_Clicked;
                this.ToolbarItems.Add(GetData);
            }
            catch (Exception ex)
            {
                DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }

        private async void Editor_TextChanged(object sender, EventArgs e)
        {
            try
            {
                motor = (BindingContext as MotorViewModel).Motor;
                DBManager.realm.Write(() =>
               {
                   if (sender is SfAutoComplete && !string.IsNullOrEmpty((sender as SfAutoComplete).Text))
                   {
                       switch ((sender as SfAutoComplete).StyleId)
                       {
                           case "CodeAutoComplete":
                               motor.Code = (sender as SfAutoComplete).Text;
                               break;
                           case "DescriptionAutoComplete":
                               motor.Description = (sender as SfAutoComplete).Text;
                               break;
                           case "BrandAutoComplete":
                               motor.Brand = (sender as SfAutoComplete).Text;
                               break;
                           case "BrandTypeAutoComplete":
                               motor.BrandType = (sender as SfAutoComplete).Text;
                               break;
                           case "DEBearingAutoComplete":
                               motor.BearingDE = (sender as SfAutoComplete).Text;
                               break;
                           case "NDEBearingAutoComplete":
                               motor.BearingNDE = (sender as SfAutoComplete).Text;
                               break;
                           case "GreaseBrandAutoComplete":
                               motor.GreaseBrand = (sender as SfAutoComplete).Text;
                               break;
                           case "GreaseBrandTypeAutoComplete":
                               motor.GreaseBrandType = (sender as SfAutoComplete).Text;
                               break;
                       }
                   }
                   else if (sender is SfComboBox && !string.IsNullOrEmpty((sender as SfComboBox).Text))
                   {
                       switch ((sender as SfComboBox).StyleId)
                       {
                           case "VoltagecomboBox":
                               
                               double v = 0; double.TryParse((sender as SfComboBox).Text, out v);
                               motor.Volt = v;
                               break;
                           case "FrequencycomboBox":
                               double f = 0; double.TryParse((sender as SfComboBox).Text, out f);
                               motor.Frequency = f;
                               break;
                           case "PolesAutoComplete":
                               int pol = 0; int.TryParse((sender as SfComboBox).Text, out pol);
                               motor.Poles = pol;
                               break;
                       }
                   }
                   else if (sender is Editor && !string.IsNullOrEmpty((sender as Editor).Text))
                   {
                           switch ((sender as Editor).StyleId)
                           {
                           case "ExtraDataEditor":
                               motor.ExtraData = (sender as Editor).Text;
                               break;
                       }
                   }
                   else if (sender is Entry && !string.IsNullOrEmpty((sender as Entry).Text))
                   {
                       switch ((sender as Entry).StyleId)
                       {
                           case "PowerEntry":
                               double p = 0; double.TryParse((sender as Entry).Text, out p);
                               motor.Power = p;
                               break;
                           case "CurrentEntry":
                               double c = 0; double.TryParse((sender as Entry).Text, out c);
                               motor.Current = c;
                               break;
                           case "CosPhiEntry":
                               double x = 0; double.TryParse((sender as Entry).Text, out x);
                               motor.CosPhi = x;
                               break;
                           case "RPMEEntry":
                               double rpm = 0; double.TryParse((sender as Entry).Text, out rpm);
                               motor.RPM = rpm;
                               break;
                           case "IntervalDaysEntry":
                               double interval = 0; double.TryParse((sender as Entry).Text, out interval);
                               motor.IntervalDays = interval;
                               break;
                           case "QTYDEEntry":
                               double qtde = 0; double.TryParse((sender as Entry).Text, out qtde);
                               motor.QtyDE = qtde;
                               break;
                           case "QTYNDEEntry":
                               double qtnde = 0; double.TryParse((sender as Entry).Text, out qtnde);
                               motor.QtyNDE = qtnde;
                               break;
                           case "FrameSizeEntry":
                               motor.FrameSize = (sender as Entry).Text;
                               break;
                           case "MountingEntry":
                               motor.Mounting = (sender as Entry).Text;
                               break;
                           case "WeightEntry":
                               double weight = 0; double.TryParse((sender as Entry).Text, out weight);
                               motor.Weight = weight;
                               break;
                           case "SupplyPanelEntry":
                               motor.SupplyPanel = (sender as Entry).Text;
                               break;
                           case "SupplyPanelLocationEntry":
                               motor.supplyPanelLoc = (sender as Entry).Text;
                               break;
                       }






                   }
               });
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }

        //reference get bearing data
        private void EquipmentDetailsvgrd_MouseDown(object sender)
        {
           /* try
            {

                Plant.Equipment.Bearings xx = null;
                if (e.Button == MouseButtons.Right)
                {
                    DevExpress.XtraVerticalGrid.VGridHitInfo hitInfo = EquipmentDetailsvgrd.CalcHitInfo(e.Location);
                    if (hitInfo.CellIndex != -1 && (hitInfo.Row.GetRowProperties(hitInfo.CellIndex).FieldName == "M_Bearing_DE" ||
                        hitInfo.Row.GetRowProperties(hitInfo.CellIndex).FieldName == "M_Bearing_NDE"))
                    {
                        Plant.Equipment.EquipmentData mm = (Plant.Equipment.EquipmentData)
                            EquipmentDataCollection.Lookup(EquipmentComponentmainsgrdv.GetRowCellValue(EquipmentComponentmainsgrdv.FocusedRowHandle, "ID"));
                        if (hitInfo.Row.GetRowProperties(hitInfo.CellIndex).FieldName == "M_Bearing_DE")
                        {
                            string lkup = mm.M_Bearing_DE.TrimStart('U', 'N', 'J');

                            if (lkup.StartsWith("6")) { lkup = lkup.Substring(0, 4); }
                            if (lkup.StartsWith("2") || lkup.StartsWith("3")) { lkup = lkup.Substring(0, 3); }

                            xx = (Plant.Equipment.Bearings)MotorsBearingsxpCollection.Lookup(int.Parse(lkup));
                            if (xx == null)
                                throw new Exception("Bearing cannot be found");
                        }
                        if (hitInfo.Row.GetRowProperties(hitInfo.CellIndex).FieldName == "M_Bearing_NDE")
                        {
                            string lkup = mm.M_Bearing_NDE.TrimStart('U', 'N', 'J');
                            if (lkup.StartsWith("6")) { lkup = lkup.Substring(0, 4); }
                            if (lkup.StartsWith("2") || lkup.StartsWith("3")) { lkup = lkup.Substring(0, 3); }

                            xx = (Plant.Equipment.Bearings)MotorsBearingsxpCollection.Lookup(int.Parse(lkup));
                            if (xx == null)
                                throw new Exception("Bearing cannot be found");
                        }
                        if (xx != null)
                        {
                            labelControl1.Text = xx.Bearing.ToString();
                            textEdit1.EditValue = xx.bearingborediameter.ToString();
                            textEdit2.EditValue = xx.shaftdiametermax.ToString();
                            textEdit3.EditValue = xx.shaftdiametermin.ToString();
                            textEdit4.EditValue = xx.bearingoutsidediameter.ToString();
                            textEdit5.EditValue = xx.housingdiametermax.ToString();
                            textEdit6.EditValue = xx.housingdiametermin.ToString();
                            flyoutPanel1.Options.Location = e.Location;
                            flyoutPanel1.ShowPopup();
                        }
                        else
                            throw new Exception("Bearing cannot be found");
                    }
                }

            }
            catch (Exception ex)
            {
                Alertcntrl.Show(this, "Error", ex.Message);
            }*/
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            motor = (BindingContext as MotorViewModel).Motor;
            if (InventoryAutoComplete.SelectedItem != null)
                DBManager.realm.Write(() =>
                {
                    motor.SpareParts.Add(new SparePart()
                    {
                        InventoryCode = (InventoryAutoComplete.SelectedItem as Spareitem).code,
                        Description1 = (InventoryAutoComplete.SelectedItem as Spareitem).description1,
                        Description2 = (InventoryAutoComplete.SelectedItem as Spareitem).description2
                    });
                    // Spare xpare = DBManager.realm.All<Spare>().Where(x => x.Code == (_sfAuto.SelectedItem as Spareitem).code).First();
                });
        }
        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                string action = "";
                if (sender is ToolbarItem)
                {
                    switch ((sender as ToolbarItem).StyleId)
                    {

                        case "GetData":
                           /* await CrossMedia.Current.Initialize();
                            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                            {
                                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                                return;
                            }

                            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                            {
                                Directory = "OCR Photos",
                                Name = "Photo_"
                                + DateTime.Now.ToLocalTime().Day.ToString()
                                + DateTime.Now.ToLocalTime().Month.ToString()
                                + DateTime.Now.ToLocalTime().Year.ToString()
                                + "_"
                                + DateTime.Now.ToLocalTime().Hour.ToString()
                                + DateTime.Now.ToLocalTime().Minute.ToString()
                                + DateTime.Now.ToLocalTime().Second.ToString()
                                + ".jpg",
                                PhotoSize = PhotoSize.Custom,
                                CustomPhotoSize = 60,//Resize to 90% of original
                                CompressionQuality = 92
                            });

                            if (file == null)
                                return;

                           // var Result = new IronTesseract().Read(file.Path).Text;
                            var Ocr = new IronTesseract();
                            Ocr.Language = OcrLanguage.English;
                            //Ocr.AddSecondaryLanguage(OcrLanguage.Arabic);
                            using (var Input = new OcrInput(file.Path))
                            {
                                //     Input.WithTitle("My Document");
                                //   Input.Binarize();
                                //  Input.Contrast();
                                Input.Deskew();
                                Input.DeNoise();
                                //   Input.Dilate();
                                //Input.EnhanceResolution(300);
                                // Input.Invert();
                                //  Input.Rotate(90);
                                //  Input.Scale(150); // or Input.Scale(3000, 2000);
                                // Input.Sharpen();
                                // Input.ToGrayScale();
                                // you don't need all of them
                                // most users only need Deskew() and occasionally DeNoise() 
                                var Result = Ocr.Read(Input);
                                await DisplayAlert(Properties.Resources.Error, Result.Text, Properties.Resources.Ok);
                            }*/
                            break;
                           
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
    }

}