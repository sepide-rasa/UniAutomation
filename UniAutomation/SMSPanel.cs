using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;

namespace UniAutomation
{
    public class SMSPanel
    {
        private static readonly Timer _timer = new Timer(OnTimerElapsed);
        private static readonly SMSjobHost _jobHost = new SMSjobHost();

        public static void Start()
        {
            _timer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(120000));
        }

        private static void OnTimerElapsed(object sender)
        {
            _jobHost.DoWork(() =>
            {
                //SMS.Send sms = new SMS.Send();
                //string Username = "daneshkadeh";
                //string Pass = "3393304";
                //string SmsPanel = "10001101100110";
                //SMS.MessagesBL[] msg = sms.getMessages(Username, Pass, 1, "", 0, 5);
                //AndroidWebServic h = new AndroidWebServic();
                //string Result = "";
                //long[] rec = null;
                //byte[] status = null;
                //foreach (var item in msg)
                //{
                //    string Body = item.Body;
                //    var BodySplited = Body.Split('-');
                //    switch (BodySplited[0].ToString())
                //    {
                //        case "FP":
                //            Result = "FP-" + h.FoodProgram(BodySplited[1]);                            
                //            sms.SendSms(Username, Pass, new string[] { item.Sender }, SmsPanel, Result, false, "", ref rec, ref status);
                //            break;
                //        case "L":
                //            Result = "L-" + h.Login(BodySplited[1], BodySplited[2]);
                //            sms.SendSms(Username, Pass, new string[] { item.Sender }, SmsPanel, Result, false, "", ref rec, ref status);
                           
                //            break;
                //    }
                //}
                
                
        
            });
        }
    }
}