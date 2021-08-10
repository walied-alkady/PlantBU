using PlantBU.DataModel;
using PlantBU.ViewModels;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Syncfusion.SfDataGrid.XForms.Renderers;
using Syncfusion.XForms.ComboBox;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlantBU.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SafetyReportPage : ContentPage
	{
        SafetyReport SafetyReport { get; set; }
        List<string> images { get; set; }
        DataTable EmployeesTable { get; set; }
        public SafetyReportPage (SafetyReport sf, bool IsNewReport = false, bool editable=false)
		{
			InitializeComponent ();
            try
            {
              
                SafetyReport = sf;
                (BindingContext as SafetyReportViewModel).page = this;
                (BindingContext as SafetyReportViewModel).Navigation = Navigation;

                FillViolationTypes();

                images = new List<string>();

                if (string.IsNullOrEmpty(SafetyReport.SourceOfIssue))
                    SourceOfIssuecomboBox.Text = "تقرير خطورة Safety Alert";
                if (string.IsNullOrEmpty(SafetyReport.ReporterName))
                {
                    ReporterNameComboBox.Text = DBManager.CurrentUser.FullNameAr;
                    if (DBManager.CurrentUserType == DBManager.Usertyypes.Admin)
                        ReporterNameComboBox.IsEnabled = true;
                    else
                        ReporterNameComboBox.IsEnabled = false;
                }
                //if ((BindingContext as SafetyReportViewModel).ElectricalEmployeesList.Contains(ReporterNameComboBox.Text))
                if (string.IsNullOrEmpty(SafetyReport.ReporterDepartment))
                    ReporterDepartmentcomboBox.Text = "Electrical";
                if (string.IsNullOrEmpty(SafetyReport.IssueOnCompany))
                    IssueOnCompanycomboBox.Text = "Titan";
                if (string.IsNullOrEmpty(SafetyReport.IssueOnCompanyName))
                    IssueOnCompanyNamecomboBox.Text = "Titan";
                if (string.IsNullOrEmpty(SafetyReport.ReportDetailsType))
                    TypecomboBox.Text = "Unsafe_Condition";
                if (string.IsNullOrEmpty(SafetyReport.ReportDetailsRisk))
                    ReportDetailsRiskcomboBox.Text = "Low";
                if (SafetyReport.ViolationCompany == "Titan")
                    ViolationCompanyNamecomboBox.Text = "Titan";
                
                if (DBManager.CurrentUserType == DBManager.Usertyypes.Admin)
                    ReporterNameComboBox.IsEnabled = true;
                else
                    ReporterNameComboBox.IsEnabled = false;

                ReporterNameComboBox.Text = SafetyReport?.ReporterName;
                ReportDetailsObservationEditor.Text = SafetyReport?.ReportDetailsObservation;
                ReportDetailscorrectiveActionsEditor.Text = SafetyReport?.ReportDetailscorrectiveActions;
                ViolationNameEditor.Text = SafetyReport?.ViolationName;
                SourceOfIssuecomboBox.Text = SafetyReport.SourceOfIssue;
                IssueOnCompanycomboBox.Text = SafetyReport.IssueOnCompany;
                IssueOnCompanyNamecomboBox.Text = SafetyReport.IssueOnCompanyName;
                ReporterDepartmentcomboBox.Text = SafetyReport.ReporterDepartment;
                ReportDetailsAreacomboBox.Text = SafetyReport.ReportDetailsArea;
                ReportDetailsLinecomboBox.Text = SafetyReport.ReportDetailsLine;
                TypecomboBox.Text = SafetyReport.ReportDetailsType;
                ReportDetailsViolationTypecomboBox.Text = SafetyReport.ReportDetailsViolationType;
                ReportDetailsRiskcomboBox.Text = SafetyReport.ReportDetailsRisk;
                ResponsibilityDepartmentcomboBox.Text = SafetyReport.ResponsibilityDepartment;
                ResponsibilityPersoncomboBox.Text = SafetyReport.ResponsibilityPerson;
                ViolationCompanycomboBox.Text = SafetyReport.ViolationCompany;
                ViolationDuringcomboBox.Text = SafetyReport.ViolationDuring;
                ViolationCompanyNamecomboBox.Text = SafetyReport.ViolationCompanyName;
                ViolationDepartmentcomboBox.Text = SafetyReport.ViolationDepartment;
                DueDatePicker.Date = SafetyReport.DueDate.DateTime.ToLocalTime();
                CloseDatePicker.Date = SafetyReport.CloseDate.DateTime.ToLocalTime();
            }
            catch (Exception ex)
            {
                DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            
        }
        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            try
            {
                DBManager.realm.Write(() =>
                {
                    IssuedatePicker.Date = DateTime.Now.ToLocalTime();
                });
                // string action = await page.DisplayActionSheet(Properties.Resources.SafetyAlert, Properties.Resources.Cancel, null, Properties.Resources.Details, Properties.Resources.Edit, Properties.Resources.Remove);               
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        private async void Editor_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    if (sender is Editor)
                        switch ((sender as Editor).StyleId)
                        {
                            
                            case "ReportDetailsObservationEditor":
                                //var sch = DBManager.realm.All<Schedule>().Where(x => x.Id == schedule.Id).First();
                                SafetyReport.ReportDetailsObservation = (sender as Editor).Text;
                                break;
                            case "ReportDetailscorrectiveActionsEditor":
                                //var sch = DBManager.realm.All<Schedule>().Where(x => x.Id == schedule.Id).First();
                                SafetyReport.ReportDetailscorrectiveActions = (sender as Editor).Text; 
                                break;
                            case "ViolationNameEditor":
                                //var sch = DBManager.realm.All<Schedule>().Where(x => x.Id == schedule.Id).First();
                                SafetyReport.ViolationName = (sender as Editor).Text; 
                                break;
                        }
                    else if (sender is SfComboBox)
                        switch ((sender as SfComboBox).StyleId)
                        {
                            case "ReporterNameComboBox":
                                SafetyReport.ReporterName = (sender as SfComboBox).Text;
                                break;
                            case "SourceOfIssuecomboBox":
                                SafetyReport.SourceOfIssue = (sender as SfComboBox).Text;
                                break;
                            case "IssueOnCompanycomboBox":
                                SafetyReport.IssueOnCompany = (sender as SfComboBox).Text;
                                if ((sender as SfComboBox).Text == "Titan")
                                    SafetyReport.IssueOnCompanyName = "Titan";
                                break;
                            case "IssueOnCompanyNamecomboBox":
                                SafetyReport.IssueOnCompany = (sender as SfComboBox).Text;
                                break;
                            case "ReporterDepartmentcomboBox":
                                SafetyReport.ReporterDepartment = (sender as SfComboBox).Text;
                                break;
                            case "ReportDetailsAreacomboBox":
                                SafetyReport.ReportDetailsArea = (sender as SfComboBox).Text;
                                break;
                            case "ReportDetailsLinecomboBox":
                                SafetyReport.ReportDetailsLine = (sender as SfComboBox).Text;
                                break;
                            case "TypecomboBox":
                                SafetyReport.ReportDetailsType = (sender as SfComboBox).Text;
                                FillViolationTypes();
                                break;
                            case "ReportDetailsViolationTypecomboBox":
                                SafetyReport.ReportDetailsViolationType = (sender as SfComboBox).Text;
                                break;
                            case "ReportDetailsRiskcomboBox":
                                SafetyReport.ReportDetailsRisk = (sender as SfComboBox).Text;
                                break;
                            case "ResponsibilityDepartmentcomboBox":
                                SafetyReport.ResponsibilityDepartment = (sender as SfComboBox).Text;
                                break;
                            case "ResponsibilityPersoncomboBox":
                                SafetyReport.ResponsibilityPerson = (sender as SfComboBox).Text;
                                break;
                            case "ViolationCompanycomboBox":
                                SafetyReport.ViolationCompany = (sender as SfComboBox).Text;
                                if (SafetyReport.ViolationCompany == "Titan")
                                    SafetyReport.ViolationCompanyName = "Titan";
                                break;
                            case "ViolationDuringcomboBox":
                                SafetyReport.ViolationDuring = (sender as SfComboBox).Text;
                                break;
                            case "ViolationCompanyNamecomboBox":
                                SafetyReport.ViolationCompany = (sender as SfComboBox).Text;
                                break;
                            case "ViolationDepartmentcomboBox":
                                SafetyReport.ViolationDepartment = (sender as SfComboBox).Text;
                                break;
                            
                        }
                    
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }

        }
        private async void datefromPicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    switch ((sender as SfDatePicker).StyleId)
                    {
                        case "DueDatePicker":
                            //var sch = DBManager.realm.All<Schedule>().Where(x => x.Id == schedule.Id).First();
                            SafetyReport.DueDate = e.NewDate.ToLocalTime();
                            break;
                        case "CloseDatePicker":
                            //var sch = DBManager.realm.All<Schedule>().Where(x => x.Id == schedule.Id).First();
                            SafetyReport.CloseDate = e.NewDate.ToLocalTime();
                            break;
                    }
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
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
                        case "SendMail":
                            ShareXlsData();
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        void FillViolationTypes()
        {
            if (TypecomboBox.Text == "Unsafe_Behavior")
                (BindingContext as SafetyReportViewModel).ViolationTypes = new List<string>()
                {
                                "Unsafe_Behavior",
"Unsafe_Condition",
"NM",
"FA",
"MTC",
"LTI",
"Fatality",
"NM",

                };
            else if (TypecomboBox.Text == "Unsafe_Condition")
                (BindingContext as SafetyReportViewModel).ViolationTypes = new List<string>()
                {
                                "Improper design.",
"Lack of maintenance.",
"Lack of inspection.",
"Improper work environment.",
"Improper Ergonomics.",
"Absence(Improper) of safety device.",
"Lack of resources.",
"Poor housekeeping.",
"Improper/missing of barricading  / guarding",
"Unnecessary scaf.",
"Unsafe storage",
"Fire hazard",
"Falling objects",
"Improper Tools/Equip.",
"Improper Cylinders",
                };
            else if (TypecomboBox.Text == "NM")
            {
                (BindingContext as SafetyReportViewModel).ViolationTypes = new List<string>() { "NM" };
                ReportDetailsViolationTypecomboBox.Text = "NM";
            }
            else if (TypecomboBox.Text == "FA")
            {
                (BindingContext as SafetyReportViewModel).ViolationTypes = new List<string>() { "FA" };
                ReportDetailsViolationTypecomboBox.Text = "FA";
            }
            else if (TypecomboBox.Text == "MTC")
            {
                (BindingContext as SafetyReportViewModel).ViolationTypes = new List<string>() { "MTC" };
                ReportDetailsViolationTypecomboBox.Text = "MTC";
            }
            else if (TypecomboBox.Text == "LTI")
            {
                (BindingContext as SafetyReportViewModel).ViolationTypes = new List<string>() { "LTI" };
                ReportDetailsViolationTypecomboBox.Text = "LTI";
            }
            else if (TypecomboBox.Text == "Fatality")
            {
                (BindingContext as SafetyReportViewModel).ViolationTypes = new List<string>() { "Fatality" };
                ReportDetailsViolationTypecomboBox.Text = "Fatality";
            }
        }
        async void ShareXlsData()
        {
            try { 
            (BindingContext as SafetyReportViewModel).IsBusy = true;
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2013;

                //"App" is the class of Portable project
                Assembly assembly = typeof(App).GetTypeInfo().Assembly;
                Stream inputStream = assembly.GetManifestResourceStream("PlantBU.ATR 2021-Blank.xlsx");
                application.DefaultVersion = ExcelVersion.Excel2013;
                IWorkbook workbook = application.Workbooks.Open(inputStream);
                IWorksheet sheet = workbook.Worksheets[0];

                //Access a range by specifying cell address 
                sheet.Range["B8"].Text = SafetyReport?.IssueDate.Day.ToString();
                sheet.Range["C8"].Text = SafetyReport?.IssueDate.Month.ToString();
                sheet.Range["D8"].Text = SafetyReport?.IssueDate.Year.ToString();
                sheet.Range["G8"].Text = SafetyReport?.SourceOfIssue;
                sheet.Range["H8"].Text = SafetyReport?.IssueOnCompany;
                sheet.Range["I8"].Text = SafetyReport?.IssueOnCompanyName;
                sheet.Range["J8"].Text = SafetyReport?.ReporterDepartment;
                sheet.Range["K8"].Text = SafetyReport?.ReporterName;
                sheet.Range["O8"].Text = SafetyReport?.ReportDetailsArea;
                sheet.Range["P8"].Text = SafetyReport?.ReportDetailsLine;
                sheet.Range["Q8"].Text = SafetyReport?.ReportDetailsObservation;
                sheet.Range["R8"].Text = SafetyReport?.ReportDetailsType;
                sheet.Range["S8"].Text = SafetyReport?.ReportDetailsViolationType;
                sheet.Range["T8"].Text = SafetyReport?.ReportDetailsRisk;
                sheet.Range["U8"].Text = SafetyReport?.ReportDetailscorrectiveActions;
                sheet.Range["V8"].Text = SafetyReport?.ResponsibilityDepartment;
                sheet.Range["W8"].Text = SafetyReport?.ResponsibilityPerson;
                sheet.Range["X8"].Text = SafetyReport?.DueDate.ToLocalTime().Day.ToString();
                sheet.Range["Y8"].Text = SafetyReport?.DueDate.ToLocalTime().Month.ToString();
                sheet.Range["Z8"].Text = SafetyReport?.DueDate.ToLocalTime().Year.ToString();
                sheet.Range["AA8"].Text = SafetyReport?.DueDate.ToLocalTime().ToString();
                sheet.Range["AB8"].Text = SafetyReport?.CloseDate.ToLocalTime().Day.ToString();
                sheet.Range["AC8"].Text = SafetyReport?.CloseDate.ToLocalTime().Month.ToString();
                sheet.Range["AD8"].Text = SafetyReport?.CloseDate.ToLocalTime().Year.ToString();
                sheet.Range["AE8"].Text = SafetyReport?.CloseDate.ToLocalTime().ToString();
                sheet.Range["AF8"].Text = SafetyReport?.Status;
                sheet.Range["AK8"].Text = SafetyReport?.ViolationCompany;
                sheet.Range["AL8"].Text = SafetyReport?.ViolationDuring;
                sheet.Range["AM8"].Text = SafetyReport?.ViolationCompanyName;
                sheet.Range["AN8"].Text = SafetyReport?.ViolationDepartment;
                sheet.Range["AO8"].Text = SafetyReport?.ViolationName;

                /* //Access a range by specifying cell row and column index
                 sheet.Range[9, 1].Text = "Accessing a Range by specify cell row and column index ";

                 //Access a Range by specifying using defined name
                 IName name = workbook.Names.Add("Name");
                 name.RefersToRange = sheet.Range["A11"];
                 sheet.Range["Name"].Text = "Accessing a Range by specifying using defined name";

                 //Accessing a Range of cells by specifying cells address
                 sheet.Range["A13:C13"].Text = "Accessing a Range of Cells (Method 1)";

                 //Accessing a Range of cells specifying cell row and column index
                 sheet.Range[15, 1, 15, 3].Text = "Accessing a Range of Cells (Method 2)";*/
                workbook.Version = ExcelVersion.Excel2013;
                //Saving the workbook as stream
                MemoryStream stream = new MemoryStream();
                workbook.SaveAs(stream);

                    //Save the stream into XLSX file
                    List<string> Paths = new List<string>();
                    string path = await Xamarin.Forms.DependencyService.Get<ISave>()
                        .SaveAndView("Safety Report_"+
                        DateTime.Now.ToLocalTime().Date.Day+
                        DateTime.Now.ToLocalTime().Date.Month +
                        DateTime.Now.ToLocalTime().Date.Year +
                        ".xlsx", "application/msexcel", stream);
                    Paths.Add(path);
                    Paths.AddRange(images);
                 List<string> toAddress = new List<string>();
                List<string> ccAddress = new List<string>();

                    var empl = DBManager.realm.All<Employee>().Where(x => x.FullNameAr == SafetyReport.ReporterDepartment).FirstOrDefault();
                    if (empl != null)
                    {
                        if(empl.Title == "Technician") { toAddress.Add("khaled.reyad@titan.com.eg"); ccAddress.Add("walid.elkady@titan.com.eg");  }
                        else if (empl.UserName == "Welkady") { toAddress.Add("khaled.reyad@titan.com.eg"); ccAddress.Add("Hazem.Hosny@titan.com.eg"); ccAddress.Add("Mohamed.Gomaa@titan.com.eg"); }
                        else if (empl.Title == "Engineer") { toAddress.Add("khaled.reyad@titan.com.eg");  }

                    }
                    else
                    {
                        toAddress.Add("khaled.reyad@titan.com.eg"); ccAddress.Add("walid.elkady@titan.com.eg");
                    }
                    await SendEmail(Paths, "("+SafetyReport.SourceOfIssue+") " + " from " + DBManager.CurrentUser.FirstName + " " + DBManager.CurrentUser.LastName, "Dear Sir, \n please find attached safety report. \n Thanks. \n "+ DBManager.CurrentUser.FirstName +" "+DBManager.CurrentUser.LastName,
                    toAddress,ccAddress);
                stream.Position = 0;
                workbook.Close();
                excelEngine.Dispose();
            }
            (BindingContext as SafetyReportViewModel).IsBusy = false;
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        public async Task SendEmail(List<string> paths  ,string subject, string body, List<string> recipients, List<string> Ccrecipients)
        {
            try
            {
                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    To = recipients,
                    Cc = Ccrecipients,
                    //Bcc = bccRecipients
                };
                /* var fn = "Attachment.txt";
                 var file = Path.Combine(FileSystem.CacheDirectory, fn);
                 File.WriteAllText(file, "Hello World");*/
                foreach (var img in paths)
                { 
                    message.Attachments.Add(new EmailAttachment(img)); 
                }
                await Email.ComposeAsync(message);
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        async void ShareFile(Xamarin.Forms.View element)
        {
            try
            {
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = Title //,
                   // File = new ShareFile(file)
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        
        private async void Photobtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                await CrossMedia.Current.Initialize();
                Image image = null;
                if ((sender as Button).StyleId == "LoadPhotobtn")
                {
                  
                    var files = await CrossMedia.Current.PickPhotosAsync(new Plugin.Media.Abstractions.PickMediaOptions
                    {
                        PhotoSize = PhotoSize.Custom,
                        CustomPhotoSize = 60 ,//Resize to 90% of original
                        CompressionQuality = 92
                    });
                    
                    if (files == null)
                        return;
                   foreach (var xxx in files)
                        images.Add(xxx.Path);
                    (BindingContext as SafetyReportViewModel).PhotosNum++;
                }
                if ((sender as Button).StyleId == "TakePhotobtn")
                {
                    if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                    {
                        await DisplayAlert("No Camera", ":( No camera available.", "OK");
                        return;
                    }

                    var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {
                        Directory = "ATR Photos",
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

                    //await DisplayAlert("File Location", file.Path, "OK");

                    /* image.Source = ImageSource.FromStream(() =>
                     {
                         var stream = file.GetStream();
                         return stream;
                     });*/
                    images.Add(file.Path);
                    (BindingContext as SafetyReportViewModel).PhotosNum++;
                }
                else if ((sender as Button).StyleId == "ClearPhotobtn")
                {
                    images.Clear();
                    (BindingContext as SafetyReportViewModel).PhotosNum = 0;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }

        private void CreateEmployeesTable()
        {
            // Create a new DataTable.    
            EmployeesTable = new DataTable("Employees");
            DataColumn dtColumn;
            DataRow myDataRow;

            // Create id column  
            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(Int32);
            dtColumn.ColumnName = "id";
            dtColumn.Caption = "Code";
            dtColumn.ReadOnly = false;
            dtColumn.Unique = true;
            // Add column to the DataColumnCollection.  
            EmployeesTable.Columns.Add(dtColumn);

            // Create Name column.    
            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(String);
            dtColumn.ColumnName = "Name";
            dtColumn.Caption = "Employee Name";
            dtColumn.AutoIncrement = false;
            dtColumn.ReadOnly = false;
            dtColumn.Unique = false;
            
#pragma warning disable CS1587 // XML comment is not placed on a valid language element
/// Add column to the DataColumnCollection.  
            EmployeesTable.Columns.Add(dtColumn);
#pragma warning restore CS1587 // XML comment is not placed on a valid language element
            
            // Create Arabic Name column.    
            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(String);
            dtColumn.ColumnName = "NameAr";
            dtColumn.Caption = "Employee Arabic Name";
            dtColumn.AutoIncrement = false;
            dtColumn.ReadOnly = false;
            dtColumn.Unique = false;
            
#pragma warning disable CS1587 // XML comment is not placed on a valid language element
/// Add column to the DataColumnCollection.  
            EmployeesTable.Columns.Add(dtColumn);
#pragma warning restore CS1587 // XML comment is not placed on a valid language element

            // Create Position column.    
            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(String);
            dtColumn.ColumnName = "Position";
            dtColumn.Caption = "Position";
            dtColumn.ReadOnly = false;
            dtColumn.Unique = false;
            // Add column to the DataColumnCollection.    
            EmployeesTable.Columns.Add(dtColumn);

            // Create Department column.    
            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(String);
            dtColumn.ColumnName = "Department";
            dtColumn.Caption = "Department";
            dtColumn.ReadOnly = false;
            dtColumn.Unique = false;
            // Add column to the DataColumnCollection.    
            EmployeesTable.Columns.Add(dtColumn);

            // Create Section column.    
            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(String);
            dtColumn.ColumnName = "Section";
            dtColumn.Caption = "Section";
            dtColumn.ReadOnly = false;
            dtColumn.Unique = false;
            // Add column to the DataColumnCollection.    
            EmployeesTable.Columns.Add(dtColumn);

            // Make id column the primary key column.    
            DataColumn[] PrimaryKeyColumns = new DataColumn[1];
            PrimaryKeyColumns[0] = EmployeesTable.Columns["id"];
            EmployeesTable.PrimaryKey = PrimaryKeyColumns;

            // Create a new DataSet  
            //DataSet dtSet = new DataSet();

            // Add EmployeesTable to the DataSet.    
            //dtSet.Tables.Add(EmployeesTable);

            // Add data rows to the EmployeesTable using NewRow method    
            // I add three customers with their addresses, names and ids   
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 19; myDataRow["Name"] = "Mohamed Hassan Ahmed Mobarak"; myDataRow["NameAr"] = "محمد حسن احمد مبارك"; myDataRow["Position"] = "Senior Store Keeper"; myDataRow["Department"] = "Finance"; myDataRow["Section"] = "Stores"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 55; myDataRow["Name"] = "Mahmoud Mekheimar Ibrahim Abdel Moneim"; myDataRow["NameAr"] = "محمود مخيمر ابراهيم عبد المنعم"; myDataRow["Position"] = "Senior Production Technician"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 59; myDataRow["Name"] = "Medhat Farouk Ahmed Aly"; myDataRow["NameAr"] = "مدحت فاروق احمد علي"; myDataRow["Position"] = "Crusher Operator"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Quarries"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 71; myDataRow["Name"] = "Hatem Soliman Amin Mohamed"; myDataRow["NameAr"] = "حاتم سليمان امين محمد"; myDataRow["Position"] = "Budget & Reporting Manager"; myDataRow["Department"] = "Finance"; myDataRow["Section"] = "Accounting"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 88; myDataRow["Name"] = "Omar Korany Tolba Khater"; myDataRow["NameAr"] = "عمر قرنى طلبة خاطر"; myDataRow["Position"] = "Packing Plant Mechanical Foreman"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 107; myDataRow["Name"] = "Sayed Zaky Essawy Ahmed"; myDataRow["NameAr"] = "سيد ذكى عيسوي احمد"; myDataRow["Position"] = "Electricity Senior Technician"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Electrical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 116; myDataRow["Name"] = "Hamad Maher Mohamed Sabry"; myDataRow["NameAr"] = "حمد ماهر محمد صبري"; myDataRow["Position"] = "Quality Control Senior Technician"; myDataRow["Department"] = "Process and Quality"; myDataRow["Section"] = "Quality Control"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 119; myDataRow["Name"] = "Alaa Fawzy Mohamed Atwa"; myDataRow["NameAr"] = "علاء فوزى محمد عطوة"; myDataRow["Position"] = "Senior Production Technician"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 122; myDataRow["Name"] = "Taha Mahmoud Hassan Abo Bakr"; myDataRow["NameAr"] = "طه محمود حسن ابو بكر"; myDataRow["Position"] = "Physical Lab Senior Technician"; myDataRow["Department"] = "Process and Quality"; myDataRow["Section"] = "Laboratory"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 123; myDataRow["Name"] = "Ahmed Khalil Saleh Khalil"; myDataRow["NameAr"] = "احمد خليل صالح خليل"; myDataRow["Position"] = "Physical Lab Foreman"; myDataRow["Department"] = "Process and Quality"; myDataRow["Section"] = "Laboratory"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 124; myDataRow["Name"] = "Ihab Ramadan Saad Mohamed"; myDataRow["NameAr"] = "ايهاب رمضان سعد محمد"; myDataRow["Position"] = "Quality Control Senior Technician"; myDataRow["Department"] = "Process and Quality"; myDataRow["Section"] = "Quality Control"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 130; myDataRow["Name"] = "Khalaf Aly Sayed Mohamed"; myDataRow["NameAr"] = "خلف علي سيد محمد"; myDataRow["Position"] = "Quality Control Senior Technician"; myDataRow["Department"] = "Process and Quality"; myDataRow["Section"] = "Quality Control"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 131; myDataRow["Name"] = "Mahmoud Mahfouz Abdallah Abdallah"; myDataRow["NameAr"] = "محمود محفوظ عبد الله"; myDataRow["Position"] = "Kiln 1 Foreman"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 159; myDataRow["Name"] = "Hassan Aly Ibrahim Badawy"; myDataRow["NameAr"] = "حسن علي ابراهيم بدوي"; myDataRow["Position"] = "Senior Production Technician"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 165; myDataRow["Name"] = "Hamdy Sayed Zeidan Sabra"; myDataRow["NameAr"] = "حمدى سيد زيدان صبرة"; myDataRow["Position"] = "Kiln Operator"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 166; myDataRow["Name"] = "Magued Moris Wahba Hanna"; myDataRow["NameAr"] = "ماجد موريس وهبة حنا"; myDataRow["Position"] = "Clay Crusher Operator"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Quarries"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 173; myDataRow["Name"] = "Alaa Eldin Abdel Rehim Mohamed Ahmed"; myDataRow["NameAr"] = "علاء الدين عبد الرحيم محمد احمد"; myDataRow["Position"] = "Kiln Operator"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 175; myDataRow["Name"] = "Ahmed Mohamed Aly Mohamed"; myDataRow["NameAr"] = "احمد محمد علي محمد"; myDataRow["Position"] = "Senior Production Technician"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 185; myDataRow["Name"] = "Abdel Rahman Abdel Moneim Ahmed"; myDataRow["NameAr"] = "عبد الرحمن عبد المنعم احمد"; myDataRow["Position"] = "Senior Fitter"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 186; myDataRow["Name"] = "Mohamed Hussein Abo Elkassem Gad Elmawla"; myDataRow["NameAr"] = "محمد حسين ابو القاسم جاد المولي"; myDataRow["Position"] = "Electricity Senior Technician"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Electrical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 199; myDataRow["Name"] = "Mohamed Abdel Salam Ahmed Hafez"; myDataRow["NameAr"] = "محمد عبد السلام احمد حافظ"; myDataRow["Position"] = "Raw Material Weighbridge Operator"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 202; myDataRow["Name"] = "Ashraf Maher Ahmed Mahmoud"; myDataRow["NameAr"] = "اشرف ماهر احمد محمود"; myDataRow["Position"] = "Raw Material Pre Blending Foreman"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 203; myDataRow["Name"] = "Maher Abdel Hamid Ahmed Gueneidy"; myDataRow["NameAr"] = "ماهر عبد الحميد احمد جنيدي"; myDataRow["Position"] = "Senior Production Technician"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 209; myDataRow["Name"] = "Fares Mahmoud Fares Mohamed"; myDataRow["NameAr"] = "فارس محمود فارس"; myDataRow["Position"] = "Senior Production Technician"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 211; myDataRow["Name"] = "Mohamed Shaaban Abdel Atty Shaaban"; myDataRow["NameAr"] = "محمد شعبان عبد العاطي شعبان"; myDataRow["Position"] = "Mills Operator"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 223; myDataRow["Name"] = "Abdel Moneim Abdel Azim Mohamed"; myDataRow["NameAr"] = "عبد المنعم عبد العظيم محمد"; myDataRow["Position"] = "Senior Mechanical Technician"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 225; myDataRow["Name"] = "Khalil Gaber Mohamed Mohamed Khalil"; myDataRow["NameAr"] = "خليل جابر محمد خليل"; myDataRow["Position"] = "Cement Mills Foreman"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 230; myDataRow["Name"] = "Abdel Hamid Mabrok Ibrahim Mohamed"; myDataRow["NameAr"] = "عبد الحميد مبروك ابراهيم محمد"; myDataRow["Position"] = "Kiln 2 Foreman"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 235; myDataRow["Name"] = "Mohamed Mohamed Ahmed Aly"; myDataRow["NameAr"] = "محمد محمد احمد علي"; myDataRow["Position"] = "Kiln Operator"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 243; myDataRow["Name"] = "Mohamed Shaaban Taha Hassan"; myDataRow["NameAr"] = "محمد شعبان طه حسن"; myDataRow["Position"] = "Kiln Operator"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 247; myDataRow["Name"] = "Mohamed Shaaban Abdel Gawad Shimy"; myDataRow["NameAr"] = "محمد شعبان عبد الجواد شيمي"; myDataRow["Position"] = "Mechanical Foreman"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 249; myDataRow["Name"] = "Mohamed Ismail Mahmoud Bahr"; myDataRow["NameAr"] = "محمد اسماعيل محمود بحر"; myDataRow["Position"] = "Senior Production Technician"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 251; myDataRow["Name"] = "Hussein Kamal Eldin Hassan Eissa"; myDataRow["NameAr"] = "حسين كمال الدين حسن عيسي"; myDataRow["Position"] = "Kiln Operator"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 254; myDataRow["Name"] = "Sayed Eweis Ahmed Korany"; myDataRow["NameAr"] = "سيد عويس احمد قرنى"; myDataRow["Position"] = "Quality Control Senior Technician"; myDataRow["Department"] = "Process and Quality"; myDataRow["Section"] = "Quality Control"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 255; myDataRow["Name"] = "Ashraf Mohamed Mostafa Moussa"; myDataRow["NameAr"] = "اشرف محمد مصطفى موسي"; myDataRow["Position"] = "X-Ray Lab Senior Technician"; myDataRow["Department"] = "Process and Quality"; myDataRow["Section"] = "Laboratory"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 261; myDataRow["Name"] = "Ahmed Mohamed Aly Darwish"; myDataRow["NameAr"] = "احمد محمد علي درويش"; myDataRow["Position"] = "Rigger"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 262; myDataRow["Name"] = "Rady Mohamed Ahmed Mohamed"; myDataRow["NameAr"] = "راضى محمد احمد محمد"; myDataRow["Position"] = "Electricity Senior Technician"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Electrical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 271; myDataRow["Name"] = "Khalil Ibrahim Abdel Samad Mohamed"; myDataRow["NameAr"] = "خليل ابراهيم عبد الصمد محمد"; myDataRow["Position"] = "Senior Instrument Technician"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Electrical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 272; myDataRow["Name"] = "Mohamed Rashid Ghallab Ayoub"; myDataRow["NameAr"] = "محمد رشيد غلاب ايوب"; myDataRow["Position"] = "Mechanical Foreman"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 273; myDataRow["Name"] = "Ashraf Abdel Atty Mehalhel Moawad"; myDataRow["NameAr"] = "اشرف عبد العاطي مهلهل معوض"; myDataRow["Position"] = "Electricity Senior Technician"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Electrical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 276; myDataRow["Name"] = "Mohamed Abdel Moneim Abdel Maksoud"; myDataRow["NameAr"] = "محمد عبد المنعم عبد المقصود"; myDataRow["Position"] = "Fitter"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 279; myDataRow["Name"] = "Abdel Azim Abdallah Hussein Selim"; myDataRow["NameAr"] = "عبد العظيم عبد الله حسين سليم"; myDataRow["Position"] = "Senior Welder"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 281; myDataRow["Name"] = "Ragab Eweis Mahmoud Ahmed"; myDataRow["NameAr"] = "رجب عويس محمود احمد"; myDataRow["Position"] = "Senior Electrician"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Electrical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 283; myDataRow["Name"] = "Abdel Salam Abdel Azim Mahmoud"; myDataRow["NameAr"] = "عبد السلام عبد العظيم محمود"; myDataRow["Position"] = "Senior Production Technician"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 286; myDataRow["Name"] = "Mahmoud Ateya Mohamed Ateya"; myDataRow["NameAr"] = "محمود عطية محمد عطية"; myDataRow["Position"] = "Mobile Equipment Driver"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 291; myDataRow["Name"] = "Abdel Baki Ahmed Abdel Baki Moawad"; myDataRow["NameAr"] = "عبد الباقى احمد عبد الباقى معوض"; myDataRow["Position"] = "Senior Fitter"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 295; myDataRow["Name"] = "Mohamed Fakhry Mohamed Diab"; myDataRow["NameAr"] = "محمد فخرى محمد دياب"; myDataRow["Position"] = "Senior Welder"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 314; myDataRow["Name"] = "Ahmed Gouda Moussa Mohamed"; myDataRow["NameAr"] = "احمد جودة موسي محمد"; myDataRow["Position"] = "Senior Mechanical Inspector"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Preventive Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 317; myDataRow["Name"] = "Hassan Mohamed Ahmed Hussein"; myDataRow["NameAr"] = "حسن محمد احمد حسين"; myDataRow["Position"] = "Senior Fitter"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 318; myDataRow["Name"] = "Adel Sayed Ahmed Aly"; myDataRow["NameAr"] = "عادل سيد احمد علي"; myDataRow["Position"] = "Senior Electrician"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Electrical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 319; myDataRow["Name"] = "Sayed Fahmy Sayed Gaballah"; myDataRow["NameAr"] = "سيد فهمى سيد جاب الله"; myDataRow["Position"] = "Senior Fitter"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 330; myDataRow["Name"] = "Abdel Aziz Mohamed Mohamed Abdel Aziz"; myDataRow["NameAr"] = "عبد العزيز محمد محمد عبد العزيز"; myDataRow["Position"] = "Dispatch Officer"; myDataRow["Department"] = "Commercial"; myDataRow["Section"] = "Dispatch & Freight"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 332; myDataRow["Name"] = "Said Abdel Tawab Amin Badawy"; myDataRow["NameAr"] = "سعيد عبد التواب امين بدوي"; myDataRow["Position"] = "Kiln Operator"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 336; myDataRow["Name"] = "Sayed Abdel Aal Kotb Abbas"; myDataRow["NameAr"] = "سيد عبد العال قطب عباس"; myDataRow["Position"] = "Senior Lubrication Technician"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Preventive Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 337; myDataRow["Name"] = "Mohamed Abdel Aziz Mohamed Nasr Eldin"; myDataRow["NameAr"] = "محمد عبد العزيز محمد نصر الدين"; myDataRow["Position"] = "Quality Control Manager"; myDataRow["Department"] = "Process and Quality"; myDataRow["Section"] = "Quality Control"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 338; myDataRow["Name"] = "Hamouda Nady Hamouda Mohamed"; myDataRow["NameAr"] = "حمودة نادى حمودة محمد"; myDataRow["Position"] = "Dispatch Officer"; myDataRow["Department"] = "Commercial"; myDataRow["Section"] = "Dispatch & Freight"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 342; myDataRow["Name"] = "Ramadan Hussein Mahmoud Mohamed"; myDataRow["NameAr"] = "رمضان حسين محمود محمد"; myDataRow["Position"] = "Packing Operator"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Packing"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 344; myDataRow["Name"] = "Mamdouh Kamal Mohamed Ibrahim"; myDataRow["NameAr"] = "ممدوح كمال محمد ابراهيم"; myDataRow["Position"] = "Weighbridge Operator"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 345; myDataRow["Name"] = "Magdy Abdel Hakim Mohamed Soliman"; myDataRow["NameAr"] = "مجدى عبد الحكيم محمد سليمان"; myDataRow["Position"] = "Senior Fitter"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 346; myDataRow["Name"] = "Khaled Tawfik Hamed Said"; myDataRow["NameAr"] = "خالد توفيق حامد سعيد"; myDataRow["Position"] = "Senior Fitter"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 350; myDataRow["Name"] = "Elkazzafy Mahmoud Mostafa Diab"; myDataRow["NameAr"] = "القذافى محمود مصطفى دياب"; myDataRow["Position"] = "Packing Senior Technician"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Packing"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 351; myDataRow["Name"] = "Yosry Rabie Metwaly Hassan"; myDataRow["NameAr"] = "يسرى ربيع متولى حسن"; myDataRow["Position"] = "Packing Senior Technician"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Packing"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 365; myDataRow["Name"] = "Talaat Adly Zaky Abo Elmakarem"; myDataRow["NameAr"] = "طلعت عدلى ذكى ابو المكارم"; myDataRow["Position"] = "Social Insurance Specialist"; myDataRow["Department"] = "Human Resources"; myDataRow["Section"] = "Transportation"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 366; myDataRow["Name"] = "Mahmoud Mohamed Sayed Ahmed"; myDataRow["NameAr"] = "محمود محمد سيد احمد"; myDataRow["Position"] = "Packing Foreman"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Packing"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 368; myDataRow["Name"] = "Mohamed Mohamed Aly Eweis"; myDataRow["NameAr"] = "محمد محمد علي عويس"; myDataRow["Position"] = "Dispatch Section Head"; myDataRow["Department"] = "Commercial"; myDataRow["Section"] = "Dispatch & Freight"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 369; myDataRow["Name"] = "Samir Mohamed Eid Gaber"; myDataRow["NameAr"] = "سمير محمد عيد جابر"; myDataRow["Position"] = "Senior Fitter"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 370; myDataRow["Name"] = "Khaled Abdel Samad Ismail Aly"; myDataRow["NameAr"] = "خالد عبد الصمد اسماعيل علي"; myDataRow["Position"] = "Mills Operator"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 388; myDataRow["Name"] = "Rady Mohamed Ahmed Afify"; myDataRow["NameAr"] = "راضى محمد احمد عفيفى"; myDataRow["Position"] = "Utilities Foreman"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 391; myDataRow["Name"] = "Nasser Fathy Mohamed Dessouky"; myDataRow["NameAr"] = "ناصر فتحى محمد دسوقي"; myDataRow["Position"] = "Fitter"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 396; myDataRow["Name"] = "Ragab Abdel Salam Hafez Zeidan"; myDataRow["NameAr"] = "رجب عبد السلام حافظ زيدان"; myDataRow["Position"] = "Senior Lubrication Technician"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Preventive Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 411; myDataRow["Name"] = "Samy Sayed Abdel Kawy Mohamed"; myDataRow["NameAr"] = "سامى سيد عبد القوي محمد"; myDataRow["Position"] = "Senior Production Technician"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 412; myDataRow["Name"] = "Heidar Abdel Halim Sawy Mohamed"; myDataRow["NameAr"] = "حيدر عبد الحليم صاوي محمد"; myDataRow["Position"] = "Electrical Inspection Foreman"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Preventive Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 417; myDataRow["Name"] = "Mokhtar Hassan Mohamed Hassan"; myDataRow["NameAr"] = "مختار حسن محمد حسن"; myDataRow["Position"] = "Senior Fitter"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 424; myDataRow["Name"] = "Abdallah Sayed Abdallah Mohamed"; myDataRow["NameAr"] = "عبد الله سيد عبد الله محمد"; myDataRow["Position"] = "Lubrication Foreman"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Preventive Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 437; myDataRow["Name"] = "Mohamed Salah Eldin Mohamed Hussein"; myDataRow["NameAr"] = "محمد صلاح الدين محمد حسين"; myDataRow["Position"] = "Dispatch Officer"; myDataRow["Department"] = "Commercial"; myDataRow["Section"] = "Dispatch & Freight"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 453; myDataRow["Name"] = "Khaled Mohamed Hussein Osman"; myDataRow["NameAr"] = "خالد محمد حسين عثمان"; myDataRow["Position"] = "AC Technician"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Electrical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 468; myDataRow["Name"] = "Eid Abdel Azim Mahmoud Mohamed"; myDataRow["NameAr"] = "عيد عبد العظيم محمود محمد"; myDataRow["Position"] = "Worker"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 473; myDataRow["Name"] = "Samir Kamal Mohamed Abdallah"; myDataRow["NameAr"] = "سمير كمال محمد عبد الله"; myDataRow["Position"] = "Worker"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 477; myDataRow["Name"] = "Abdel Halim Korany Aly"; myDataRow["NameAr"] = "عبد الحليم قرنى علي"; myDataRow["Position"] = "Senior Production Technician"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 485; myDataRow["Name"] = "Mohamed Sobhy Abdallah Hassaan"; myDataRow["NameAr"] = "محمد صبحى عبد الله حسان"; myDataRow["Position"] = "Packing Operator"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Packing"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 486; myDataRow["Name"] = "Hatem Gaber Mohamed Mohamed"; myDataRow["NameAr"] = "حاتم جابر محمد"; myDataRow["Position"] = "Kiln Operator"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 503; myDataRow["Name"] = "Ayman Hosny Abdel Halim"; myDataRow["NameAr"] = "ايمن حسنى عبد الحليم"; myDataRow["Position"] = "Chemical Lab Foreman"; myDataRow["Department"] = "Process and Quality"; myDataRow["Section"] = "Laboratory"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 504; myDataRow["Name"] = "Abdel Nasser Abdel Azim Ahmed"; myDataRow["NameAr"] = "عبد الناصر عبد العظيم احمد"; myDataRow["Position"] = "X-Ray Lab Senior Technician"; myDataRow["Department"] = "Process and Quality"; myDataRow["Section"] = "Laboratory"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 508; myDataRow["Name"] = "Shaaban Mohamed Aly Darwish"; myDataRow["NameAr"] = "شعبان محمد علي درويش"; myDataRow["Position"] = "Production Worker"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 512; myDataRow["Name"] = "Sayed Mohamed Sayed Abdel Hafeez"; myDataRow["NameAr"] = "سيد محمد سيد عبد الحفيظ"; myDataRow["Position"] = "Production Worker"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 517; myDataRow["Name"] = "Fathy Sayed Aly Abdel Salam"; myDataRow["NameAr"] = "فتحى سيد علي عبد السلام"; myDataRow["Position"] = "Production Worker"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 530; myDataRow["Name"] = "Ramadan Sayed Abdel Hamid"; myDataRow["NameAr"] = "رمضان سيد عبد الحميد"; myDataRow["Position"] = "Packing Senior Technician"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Packing"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 531; myDataRow["Name"] = "Abdel Gawad Abdel Rehim Elkinawy"; myDataRow["NameAr"] = "عبد الجواد عبد الرحيم القناوى"; myDataRow["Position"] = "Production Technician"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 558; myDataRow["Name"] = "Adel Mohamed Abdallah Ibrahim"; myDataRow["NameAr"] = "عادل محمد عبد الله ابراهيم"; myDataRow["Position"] = "Health & Safety Foreman"; myDataRow["Department"] = "Health, Safety & Environment"; myDataRow["Section"] = "Health, Safety & Environment"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 562; myDataRow["Name"] = "Abdel Nasser Ebeid Mohamed Selim"; myDataRow["NameAr"] = "عبد الناصر عبيد محمد سليم"; myDataRow["Position"] = "Senior Mechanical Inspector"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Preventive Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 571; myDataRow["Name"] = "Hamdy Ahmed Said Abo Seif"; myDataRow["NameAr"] = "حمدى احمد سعيد ابو سيف"; myDataRow["Position"] = "Senior Fitter"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 577; myDataRow["Name"] = "Gouda Hassan Mohamed Hassan"; myDataRow["NameAr"] = "جودة حسن محمد حسن"; myDataRow["Position"] = "Senior Fitter"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 590; myDataRow["Name"] = "Aly Mohamed Aly Mansour"; myDataRow["NameAr"] = "علي محمد علي منصور"; myDataRow["Position"] = "Production Worker"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 600; myDataRow["Name"] = "Bereik Aly Hassan Mohamed"; myDataRow["NameAr"] = "بريك علي حسن محمد"; myDataRow["Position"] = "Senior Store Keeper"; myDataRow["Department"] = "Finance"; myDataRow["Section"] = "Stores"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 611; myDataRow["Name"] = "Eid Mohamed Ahmed Embaby"; myDataRow["NameAr"] = "عيد محمد احمد امبابي"; myDataRow["Position"] = "Production Worker"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 644; myDataRow["Name"] = "Essam Mahmoud Elsayed Khamis"; myDataRow["NameAr"] = "عصام محمود السيد خميس"; myDataRow["Position"] = "Electrician"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Electrical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 645; myDataRow["Name"] = "Ibrahim Ahmed Mohamed Mohamed"; myDataRow["NameAr"] = "ابراهيم احمد محمد محمد"; myDataRow["Position"] = "Senior Electrician"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Electrical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 665; myDataRow["Name"] = "Emad Ahmed Aly Moawad"; myDataRow["NameAr"] = "عماد احمد علي معوض"; myDataRow["Position"] = "Senior Technician"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 668; myDataRow["Name"] = "Mohamed Mohamed Abdel Baky Elbehery"; myDataRow["NameAr"] = "محمد محمد عبد الباقي البحيرى"; myDataRow["Position"] = "Senior Production Technician"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 672; myDataRow["Name"] = "Tarek Mohamed Mohamed Basal"; myDataRow["NameAr"] = "طارق محمد محمد بصل"; myDataRow["Position"] = "Stores Supervisor"; myDataRow["Department"] = "Finance"; myDataRow["Section"] = "Stores"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 674; myDataRow["Name"] = "Mohamed Hanafy Abdel Samad Saleh"; myDataRow["NameAr"] = "محمد حنفى عبد الصمد صالح"; myDataRow["Position"] = "Financial Controller"; myDataRow["Department"] = "Finance"; myDataRow["Section"] = "control"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 678; myDataRow["Name"] = "Abdel Nasser Ghanem Abdel Wahab"; myDataRow["NameAr"] = "عبدالناصر غانم عبد الوهاب"; myDataRow["Position"] = "Fitter"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 679; myDataRow["Name"] = "Mohamed Zarea Abdel Moula"; myDataRow["NameAr"] = "محمد زارع عبد المولى"; myDataRow["Position"] = "Senior Fitter"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 701; myDataRow["Name"] = "Eweis Elkorany Abdel Atty Ahmed"; myDataRow["NameAr"] = "عويس القرنى عبد العاطى احمد"; myDataRow["Position"] = "X-Ray Lab Senior Technician"; myDataRow["Department"] = "Process and Quality"; myDataRow["Section"] = "Laboratory"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 702; myDataRow["Name"] = "Ayman Farouk Ftahy Amin"; myDataRow["NameAr"] = "ايمن فاروق فتحى امين"; myDataRow["Position"] = "X-Ray Lab Senior Technician"; myDataRow["Department"] = "Process and Quality"; myDataRow["Section"] = "Laboratory"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 706; myDataRow["Name"] = "Mahmoud Mahmoud Abdel Wahab"; myDataRow["NameAr"] = "محمود محمود عبد الوهاب"; myDataRow["Position"] = "Electrical Section Head"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Electrical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 712; myDataRow["Name"] = "Rabie Moawad Moawad Mahmoud"; myDataRow["NameAr"] = "ربيع معوض معوض محمود"; myDataRow["Position"] = "Packing Operator"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Packing"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 715; myDataRow["Name"] = "Ramadan Mohamed Mohamed Eissa"; myDataRow["NameAr"] = "رمضان محمد محمد عيسي"; myDataRow["Position"] = "Electricity Senior Technician"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Electrical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 716; myDataRow["Name"] = "Marzouk Kordy Aly Eweis"; myDataRow["NameAr"] = "مرزوق كردى علي عويس"; myDataRow["Position"] = "Senior Instrument Technician"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Electrical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 722; myDataRow["Name"] = "Ahmed Abdel Alym Abdel Latif"; myDataRow["NameAr"] = "احمد عبد العليم عبد اللطيف"; myDataRow["Position"] = "Senior Electrician"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Electrical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 739; myDataRow["Name"] = "Mohamed Kamal Eldin Abdel Alym"; myDataRow["NameAr"] = "محمد كمال الدين عبد العليم"; myDataRow["Position"] = "Fitter"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 747; myDataRow["Name"] = "Sayed Mohamed Sabra Abdel Gawad"; myDataRow["NameAr"] = "سيد محمد صبرة عبد الجواد"; myDataRow["Position"] = "Production Worker"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 750; myDataRow["Name"] = "Zein Elabedeen Gomaa Ahmed"; myDataRow["NameAr"] = "زين العابدين جمعة احمد"; myDataRow["Position"] = "Senior Store Keeper"; myDataRow["Department"] = "Finance"; myDataRow["Section"] = "Stores"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 752; myDataRow["Name"] = "Ramadan Hamed Saeed Abdel Gawad"; myDataRow["NameAr"] = "رمضان حامد سعيد عبد الجواد"; myDataRow["Position"] = "Material Handling Supervisor"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 754; myDataRow["Name"] = "Hossam Eldin Mostafa Taha"; myDataRow["NameAr"] = "حسام الدين مصطفى طه"; myDataRow["Position"] = "Plant Manager"; myDataRow["Department"] = "Plant Management"; myDataRow["Section"] = "Plant Management"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 757; myDataRow["Name"] = "Nabil Fathy Abdel Aziz Darwish"; myDataRow["NameAr"] = "نبيل فتحى عبد العزيز درويش"; myDataRow["Position"] = "Mechanical Manager"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 759; myDataRow["Name"] = "Nasser Kamal Mohamed Abdel Gawad"; myDataRow["NameAr"] = "ناصر كمال محمد عبد الجواد"; myDataRow["Position"] = "Mechanical Section Head Raw Mills & Crusher"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 760; myDataRow["Name"] = "Reda Abdel Ghany Mohamed Abo Taleb"; myDataRow["NameAr"] = "رضا عبد الغني محمد ابو طالب"; myDataRow["Position"] = "Senior Reporting Engineer"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 771; myDataRow["Name"] = "Mohamed Rabie Ahmed Ghaffery"; myDataRow["NameAr"] = "محمد ربيع احمد غفارى"; myDataRow["Position"] = "Production Services Section Head"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 774; myDataRow["Name"] = "Ahmed Rashad Mohamed Abdel Aleem"; myDataRow["NameAr"] = "احمد رشاد محمد عبد العليم"; myDataRow["Position"] = "Dispatch Officer"; myDataRow["Department"] = "Commercial"; myDataRow["Section"] = "Dispatch & Freight"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 779; myDataRow["Name"] = "Mohamed Said Mohamed Said"; myDataRow["NameAr"] = "محمد سعيد محمد سعيد"; myDataRow["Position"] = "Telephone Maintenance Technician"; myDataRow["Department"] = "Finance"; myDataRow["Section"] = "Information Technology"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 798; myDataRow["Name"] = "Ragab Mounir Abdel Kawy Amin"; myDataRow["NameAr"] = "رجب منير عبد القوي امين"; myDataRow["Position"] = "Senior Store Keeper"; myDataRow["Department"] = "Finance"; myDataRow["Section"] = "Stores"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 801; myDataRow["Name"] = "Touny Ahmed Osman Mohamed"; myDataRow["NameAr"] = "تونى احمد عثمان محمد"; myDataRow["Position"] = "BU Community, Employee Relations & Government Affairs Department Manager"; myDataRow["Department"] = "B.U."; myDataRow["Section"] = "Administration & Public Relati"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 841; myDataRow["Name"] = "Mohamed Abdel Azeem Hassan"; myDataRow["NameAr"] = "محمد عبد العظيم حسن"; myDataRow["Position"] = "Dispatch Officer"; myDataRow["Department"] = "Commercial"; myDataRow["Section"] = "Dispatch & Freight"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 847; myDataRow["Name"] = "Salah Eldin Aly Younes"; myDataRow["NameAr"] = "صلاح الدين علي يونس"; myDataRow["Position"] = "Security Manager"; myDataRow["Department"] = "Human Resources"; myDataRow["Section"] = "Security"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 853; myDataRow["Name"] = "Mohamed Hashim Abdel Gawad Khalifa"; myDataRow["NameAr"] = "محمد هاشم عبد الجواد خليفة"; myDataRow["Position"] = "Legal Affairs Clerk"; myDataRow["Department"] = "Legal"; myDataRow["Section"] = "Legal"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 854; myDataRow["Name"] = "Ramadan Sayed Arafat Hamed"; myDataRow["NameAr"] = "رمضان سيد عرفات حامد"; myDataRow["Position"] = "Administrative Clerk"; myDataRow["Department"] = "Human Resources"; myDataRow["Section"] = "Administration & Public Relati"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 855; myDataRow["Name"] = "Ramadan Taha Abdel Raouf Taha"; myDataRow["NameAr"] = "رمضان طه عبد الرؤوف طه"; myDataRow["Position"] = "Administrative Clerk"; myDataRow["Department"] = "Human Resources"; myDataRow["Section"] = "Administration & Public Relati"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 856; myDataRow["Name"] = "Sayed Saber Metwaly Mohamed"; myDataRow["NameAr"] = "سيد صابر متولى محمد"; myDataRow["Position"] = "Personnel Clerk"; myDataRow["Department"] = "Human Resources"; myDataRow["Section"] = "Personnel"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 857; myDataRow["Name"] = "Khaled Aly Ibrahim Abdel Kereem"; myDataRow["NameAr"] = "خالد علي ابراهيم عبد الكريم"; myDataRow["Position"] = "Safety Worker"; myDataRow["Department"] = "Health, Safety & Environment"; myDataRow["Section"] = "Health & Safety"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 858; myDataRow["Name"] = "Sayed Shaaban Kamel Abdel Kader"; myDataRow["NameAr"] = "سيد شعبان كامل عبد القادر"; myDataRow["Position"] = "Administrative Clerk"; myDataRow["Department"] = "Human Resources"; myDataRow["Section"] = "Administration & Public Relati"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 861; myDataRow["Name"] = "Ahmed Abo Zeid Ahmed Abo Zeid"; myDataRow["NameAr"] = "احمد ابو زيد احمد ابو زيد"; myDataRow["Position"] = "Administrative Clerk"; myDataRow["Department"] = "Human Resources"; myDataRow["Section"] = "Clinic"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 862; myDataRow["Name"] = "Abdel Azeem Taha Abdel Aleem Ammar"; myDataRow["NameAr"] = "عبد العظيم طه عبد العليم عمار"; myDataRow["Position"] = "Accounting Clerk"; myDataRow["Department"] = "Finance"; myDataRow["Section"] = "Accounting"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 863; myDataRow["Name"] = "Walid Hussein Abdel Sattar Mohamed"; myDataRow["NameAr"] = "وليد حسين عبد الستار محمد"; myDataRow["Position"] = "Administrative Clerk"; myDataRow["Department"] = "Human Resources"; myDataRow["Section"] = "Clinic"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 864; myDataRow["Name"] = "Mohamed Saad Abdel Ghani Shehata"; myDataRow["NameAr"] = "محمد سعد عبد الغني شحاتة"; myDataRow["Position"] = "Administrative Clerk"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 865; myDataRow["Name"] = "Alaa Ezzat Mamdouh Hassan"; myDataRow["NameAr"] = "علاء عزت ممدوح حسن"; myDataRow["Position"] = "Clerk of Process & Quality Management"; myDataRow["Department"] = "Process and Quality"; myDataRow["Section"] = "Laboratory"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 875; myDataRow["Name"] = "Ibrahim Ahmed Mahmoud Hassanein"; myDataRow["NameAr"] = "ابراهيم احمد محمود حسنين"; myDataRow["Position"] = "Raw Mills & Pre Blending Section Head"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 879; myDataRow["Name"] = "Nashwa Mohamed Mostafa Mohamed"; myDataRow["NameAr"] = "نشوى محمد مصطفى محمد"; myDataRow["Position"] = "Senior Secretary"; myDataRow["Department"] = "Plant Management"; myDataRow["Section"] = "Plant Management"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 882; myDataRow["Name"] = "Alaa Hamed Abdel Moaty Abdel Gawad"; myDataRow["NameAr"] = "علاء حامد عبد المعطي عبد الجواد"; myDataRow["Position"] = "Production Worker"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 884; myDataRow["Name"] = "Ahmed Mohamed Abdel Aleem"; myDataRow["NameAr"] = "احمد محمد عبد العليم"; myDataRow["Position"] = "Materials Warehouse Section Head"; myDataRow["Department"] = "Finance"; myDataRow["Section"] = "Stores"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 893; myDataRow["Name"] = "Sayed Saad Mohamed Salem"; myDataRow["NameAr"] = "سيد سعد محمد سالم"; myDataRow["Position"] = "Production Technician"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 896; myDataRow["Name"] = "Ahmed Talaat Ahmed Aly"; myDataRow["NameAr"] = "احمد طلعت احمد علي"; myDataRow["Position"] = "Lawyer"; myDataRow["Department"] = "Legal"; myDataRow["Section"] = "Legal"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 904; myDataRow["Name"] = "Hazem Hosny Abdel Azeem"; myDataRow["NameAr"] = "حازم حسني عبد العظيم"; myDataRow["Position"] = "Maintenance Department Manager"; myDataRow["Department"] = "Plant Management"; myDataRow["Section"] = "Plant Management"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 905; myDataRow["Name"] = "Ahmed Mostafa Sayed Mahmoud"; myDataRow["NameAr"] = "احمد مصطفى سيد محمود"; myDataRow["Position"] = "Senior Buyer"; myDataRow["Department"] = "Purchasing"; myDataRow["Section"] = "Purchasing"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 915; myDataRow["Name"] = "Mehrez Meabed Amin Khedr"; myDataRow["NameAr"] = "محرز معبد امين خضر"; myDataRow["Position"] = "Quarries Section Head"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Quarries"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 917; myDataRow["Name"] = "Mohamed Yehia Ahmed Aly"; myDataRow["NameAr"] = "محمد يحيى احمد علي"; myDataRow["Position"] = "Inspection Section Head"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Preventive Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 919; myDataRow["Name"] = "Mohamed Abdel Latif Abdel Bari Ahmed"; myDataRow["NameAr"] = "محمد عبد اللطيف عبد الباري احمد"; myDataRow["Position"] = "Senior Electrical Inspection Engineer"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Preventive Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 923; myDataRow["Name"] = "Ahmed Sayed Youssef Eweis"; myDataRow["NameAr"] = "احمد سيد يوسف عويس"; myDataRow["Position"] = "Cement Mills & Packing Plant Section Head"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 924; myDataRow["Name"] = "Amr Mohamed Saleh Faragallah"; myDataRow["NameAr"] = "عمرو محمد صالح فرج الله"; myDataRow["Position"] = "Methods Manager"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Preventive Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 933; myDataRow["Name"] = "Mohamed Gomaa Abdel Latif"; myDataRow["NameAr"] = "محمد جمعة عبد اللطيف"; myDataRow["Position"] = "Automation & Instrumentation Section Head"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Electrical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 941; myDataRow["Name"] = "Alaa Eldin Farag Abdel Hamid Abdel Mohsen"; myDataRow["NameAr"] = "علاء الدين فرج عبد الحميد عبد المحسن"; myDataRow["Position"] = "Shift Leader"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 951; myDataRow["Name"] = "Mohamed Ahmed Shawky Hassan"; myDataRow["NameAr"] = "محمد احمد شوقى حسن"; myDataRow["Position"] = "Production and Process Manager"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 954; myDataRow["Name"] = "Amr Mohamed Yassin"; myDataRow["NameAr"] = "عمرو محمد ياسين"; myDataRow["Position"] = "Kiln Section Head"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 957; myDataRow["Name"] = "Hossam Aly Abdel Hakim Abdallah"; myDataRow["NameAr"] = "حسام علي عبد الحكيم عبد الله"; myDataRow["Position"] = "Personnel Clerk"; myDataRow["Department"] = "Human Resources"; myDataRow["Section"] = "Personnel"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 958; myDataRow["Name"] = "Ahmed Nady Aly Desouky"; myDataRow["NameAr"] = "احمد نادى علي دسوقى"; myDataRow["Position"] = "Security Clerk"; myDataRow["Department"] = "Human Resources"; myDataRow["Section"] = "Security"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 959; myDataRow["Name"] = "Ahmed Mohamed Abdel Hamid Ismail"; myDataRow["NameAr"] = "احمد محمد عبد الحميد اسماعيل"; myDataRow["Position"] = "Driver"; myDataRow["Department"] = "Human Resources"; myDataRow["Section"] = "Transportation"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 960; myDataRow["Name"] = "Osama Hussein Abdel Samad Saleh"; myDataRow["NameAr"] = "اسامة حسين عبد الصمد صالح"; myDataRow["Position"] = "Security Clerk"; myDataRow["Department"] = "Human Resources"; myDataRow["Section"] = "Security"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 962; myDataRow["Name"] = "Ahmed Sayed Abdel Halim Morsy"; myDataRow["NameAr"] = "احمد سيد عبد الحليم مرسي"; myDataRow["Position"] = "Safety Worker"; myDataRow["Department"] = "Health, Safety & Environment"; myDataRow["Section"] = "Health & Safety"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 963; myDataRow["Name"] = "Mohamed Eweis Ahmed Osman"; myDataRow["NameAr"] = "محمد عويس احمد عثمان"; myDataRow["Position"] = "Telephone Maintenance Worker"; myDataRow["Department"] = "Finance"; myDataRow["Section"] = "Information Technology"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 964; myDataRow["Name"] = "Sherif Hassan Mohamed Elgeddawy"; myDataRow["NameAr"] = "شريف حسن محمد الجداوى"; myDataRow["Position"] = "Planning Section Head"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Preventive Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 966; myDataRow["Name"] = "Mohamed Ahmed Abdel Naeem"; myDataRow["NameAr"] = "محمد احمد عبد النعيم"; myDataRow["Position"] = "Automation & Instrumentation Section Head"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Electrical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 969; myDataRow["Name"] = "Elamir Abdallah Abdallah Abdallah"; myDataRow["NameAr"] = "الامير عبد الله عبد الله عبد الله"; myDataRow["Position"] = "Quality Control Section Head"; myDataRow["Department"] = "Process and Quality"; myDataRow["Section"] = "Laboratory"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 973; myDataRow["Name"] = "Ahmed Mahmoud Abdel Salam Abo Bakr"; myDataRow["NameAr"] = "احمد محمود عبد السلام ابو بكر"; myDataRow["Position"] = "Shift Leader"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 974; myDataRow["Name"] = "Hany Ragab Mohamed Ahmed"; myDataRow["NameAr"] = "هانى رجب محمد احمد"; myDataRow["Position"] = "Automation Section Head"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Electrical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 976; myDataRow["Name"] = "Walid Aly Zein Elabedeen Reyad"; myDataRow["NameAr"] = "وليد علي زين العابدين رياض"; myDataRow["Position"] = "Senior Electrical Engineer"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Electrical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 983; myDataRow["Name"] = "Ahmed Shabaan Hozayen Sayed"; myDataRow["NameAr"] = "احمد شعبان حوزين سيد"; myDataRow["Position"] = "Senior Mechanical Engineer"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 987; myDataRow["Name"] = "Ahmed Raouf Abdel Salam"; myDataRow["NameAr"] = "احمد رؤوف عبد السلام"; myDataRow["Position"] = "Shift Leader"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 995; myDataRow["Name"] = "Abdel Aleem Soliman Abdel Ghany Soliman"; myDataRow["NameAr"] = "عبد العليم سليمان عبد الغني سليمان"; myDataRow["Position"] = "Guard"; myDataRow["Department"] = "Human Resources"; myDataRow["Section"] = "Security"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 997; myDataRow["Name"] = "Ramadan Mohamed Taha Metwally"; myDataRow["NameAr"] = "رمضان محمد طه متولى"; myDataRow["Position"] = "Guard"; myDataRow["Department"] = "Human Resources"; myDataRow["Section"] = "Security"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 998; myDataRow["Name"] = "Mahmoud Khalil Ibrahim Nossier"; myDataRow["NameAr"] = "محمود خليل ابراهيم نصير"; myDataRow["Position"] = "Guard"; myDataRow["Department"] = "Human Resources"; myDataRow["Section"] = "Security"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 1000; myDataRow["Name"] = "Hanafy Mahmoud Mohamed Abdel Naby"; myDataRow["NameAr"] = "حنفى محمود محمد عبد النبي"; myDataRow["Position"] = "Guard"; myDataRow["Department"] = "Human Resources"; myDataRow["Section"] = "Security"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 1006; myDataRow["Name"] = "Ahmed Abdel Moaez Mahmoud Elhosseiny"; myDataRow["NameAr"] = "احمد عبد المعز محمود الحسينى"; myDataRow["Position"] = "Health & Safety Section Head"; myDataRow["Department"] = "Health, Safety & Environment"; myDataRow["Section"] = "Health & Safety"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 1007; myDataRow["Name"] = "Khairy Mohamed Mazhar Gomaa Aly"; myDataRow["NameAr"] = "خيرى محمد مظهر جمعة علي"; myDataRow["Position"] = "Senior Administration Specialist"; myDataRow["Department"] = "Human Resources"; myDataRow["Section"] = "Administration & Public Relati"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 1010; myDataRow["Name"] = "Islam Ibrahim Fahmy Ibrahim"; myDataRow["NameAr"] = "اسلام ابراهيم فهمى ابراهيم"; myDataRow["Position"] = "Utilities Section Head"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 1012; myDataRow["Name"] = "Yasser Abdallah Mahmoud Mohamed"; myDataRow["NameAr"] = "ياسر عبد الله محمود محمد"; myDataRow["Position"] = "Senior Electrical Engineer"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Electrical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 1013; myDataRow["Name"] = "Mohamed Meatemed Mohamed Elsayed"; myDataRow["NameAr"] = "محمد معتمد محمد السيد"; myDataRow["Position"] = "Shift Leader"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 1014; myDataRow["Name"] = "Mohamed Medhat Sediek Saad"; myDataRow["NameAr"] = "محمد مدحت صديق سعد"; myDataRow["Position"] = "Production Engineer"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 1018; myDataRow["Name"] = "Walid Sayed Hassan Mostafa"; myDataRow["NameAr"] = "وليد سيد حسن مصطفى"; myDataRow["Position"] = "Environment Section Head"; myDataRow["Department"] = "Health, Safety & Environment"; myDataRow["Section"] = "Environment"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 1019; myDataRow["Name"] = "Mahmoud Ahmed Badr Abdel Maksoud"; myDataRow["NameAr"] = "محمود احمد بدر عبد المقصود"; myDataRow["Position"] = "Mechanical Inspection Engineer"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Preventive Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 1020; myDataRow["Name"] = "Mohamed Magdy Saad Zaghloul"; myDataRow["NameAr"] = "محمد مجدى سعد زغلول"; myDataRow["Position"] = "Senior Mechanical Engineer"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 1022; myDataRow["Name"] = "Ahmed Taha Abo Serea Taha"; myDataRow["NameAr"] = "احمد طه ابو سريع طه"; myDataRow["Position"] = "Lubrication Engineer"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Preventive Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 1023; myDataRow["Name"] = "Sherif Ahmed Elhosseiny Naguib"; myDataRow["NameAr"] = "شريف احمد الحسينى نجيب"; myDataRow["Position"] = "Raw Mill Mechanical Maintenance Engineer"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 1028; myDataRow["Name"] = "Mohamed Nasser Mohamed Darwish"; myDataRow["NameAr"] = "محمد ناصر محمد درويش"; myDataRow["Position"] = "Sales Representative"; myDataRow["Department"] = "Sales"; myDataRow["Section"] = "Sales"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 1032; myDataRow["Name"] = "Assem Taha Abdel Ghany"; myDataRow["NameAr"] = "عاصم طه عبد الغني"; myDataRow["Position"] = "Production Engineer"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 1033; myDataRow["Name"] = "Sherif Tarek Mostafa Mohamed"; myDataRow["NameAr"] = "شريف طارق مصصطفى محمد"; myDataRow["Position"] = "Production Engineer"; myDataRow["Department"] = "Production"; myDataRow["Section"] = "Production"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 1034; myDataRow["Name"] = "Mohamed Kamal Darwish Aly"; myDataRow["NameAr"] = "محمد كمال درويش علي"; myDataRow["Position"] = "Lab Chemist"; myDataRow["Department"] = "Process and Quality"; myDataRow["Section"] = "Laboratory"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 1044; myDataRow["Name"] = "Ahmed Abdel Hakim Mohamed Hafez"; myDataRow["NameAr"] = "احمد عبد الحكيم محمد حافظ"; myDataRow["Position"] = "Accountant"; myDataRow["Department"] = "Finance"; myDataRow["Section"] = "Accounting"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 6152; myDataRow["Name"] = "Khaled Mohie El Din Khorshed"; myDataRow["NameAr"] = "خالد محيى الدين خورشيد"; myDataRow["Position"] = "HR Buisness Lead"; myDataRow["Department"] = "Human Resources"; myDataRow["Section"] = "Management"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 1049; myDataRow["Name"] = "Khaled  Mohsen Ahmed Riad"; myDataRow["NameAr"] = "خالد محسن احمد رياض"; myDataRow["Position"] = "Senior Safety Engineer"; myDataRow["Department"] = "Health, Safety & Environment"; myDataRow["Section"] = "Health & Safety"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 1050; myDataRow["Name"] = "Hassan Adel Hassan"; myDataRow["NameAr"] = "حسن عادل حسن"; myDataRow["Position"] = " Mechanical Engineer"; myDataRow["Department"] = "Maintenance"; myDataRow["Section"] = "Mechanical Maintenance"; EmployeesTable.Rows.Add(myDataRow);
            myDataRow = EmployeesTable.NewRow(); myDataRow["id"] = 630245; myDataRow["Name"] = "Ahmed Ghidan"; myDataRow["NameAr"] = "احمد محمد غيضان"; myDataRow["Position"] = "IT Supervisor"; myDataRow["Department"] = "IT"; myDataRow["Section"] = "IT"; EmployeesTable.Rows.Add(myDataRow);

        }
    }
}