exports =async function() {
  const Logs      = await context.services.get('RealmService').db('TitanDB').collection('Log');
  const Motor     = await context.services.get('RealmService').db('TitanDB').collection('Motor');
  const schedules = await context.services.get('RealmService').db('TitanDB').collection('Schedule');
  
  const schedulesCount =await schedules.count({});
  const LogsCount =await Logs.count({});

  var docs = Motor.find({ IntervalDays: { $gt: 0 } }).toArray().then((GreasableMotors) => {
        GreasableMotors.forEach(async (GreasableMotor) =>{
              ///Checking motor is not scheduled for grease
              var MotSch = await schedules.count({ItemCode:GreasableMotor.Code, Repair:"Grease"});
              if (MotSch==0){
                 ///getting motor logs count
                  var GreasedMotorCount = await Logs.count({ItemCode:GreasableMotor.Code, Repair:"Grease"});  
                  console.log("GreasedMotorCount:");  console.log(GreasedMotorCount);
                    ///getting latest grease date
                   var latestDate ;
                    await Logs.find({"ItemCode":GreasableMotor.Code, "Repair":"Grease"} ,{ "DateLog": 1 , "_id": 0}).sort({ DateLog: -1 }).toArray().then(docs =>docs.forEach((log) =>{ latestDate= log.DateLog;}));
                    console.log("latestDate"); console.log(latestDate);
                    if (GreasedMotorCount > 0)
                       //Motor is greased before?
                       {
                         console.log("Greased Before:");console.log(latestDate);
                         latestDate = new Date(latestDate);  console.log("Greased Before New Date:");console.log(latestDate);
                       }
                    else
                        {latestDate =  new Date();  console.log("Greased New:");console.log(latestDate);}//add new grease date
                        
              var latestNewDate = new Date(latestDate.getFullYear(),latestDate.getMonth(),latestDate.getDate()+GreasableMotor.IntervalDays);// add interval days to latest grease     
              var tempdateNow = new Date();
              if (latestNewDate > tempdateNow) // checking if grease date is due , then add to schedule.
                 {
                   console.log("latestNewDate:");console.log(latestNewDate);
                   schedules.insertOne( { partition:"public", ItemCode: GreasableMotor.Code,ItemDescription:GreasableMotor.Description, Repair: "Grease" ,Repairdetails:""} );
                   
                 }
              }
                
        });
    }).catch((error) => {
        console.log(error);
    });
};
