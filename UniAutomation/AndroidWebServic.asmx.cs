using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace UniAutomation
{
    /// <summary>
    /// Summary description for AndroidWebServic
    /// </summary>
    [WebService(Namespace = "http://rasa-system.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class AndroidWebServic : System.Web.Services.WebService
    {

        [WebMethod]
        public string FoodProgram(string UserId)
        {
            Models.UniAutomationEntities p = new Models.UniAutomationEntities();
            var q = p.sp_tblFoodProgramingSelect("MoreFoodDate", "", 30).ToList();
            string data = "";
            for (int i = 0; i < q.Count; i++)
            {
                var rezerv = p.sp_tblRezervSelect("fldFoodProgramingID", q[i].fldID.ToString(), 0).Where(k => k.fldFoodCartID == Convert.ToInt32(UserId)).FirstOrDefault();
                int haveFood = 0;
                if (rezerv != null)
                    haveFood = 1;
                if (i < q.Count - 1)
                    data += q[i].fldSelfServiceID + ";" + q[i].fldNobat + ";" +
                        MyLib.Shamsi.Shamsi2miladiString(q[i].fldFoodDate) + ";" + q[i].fldFoodInfoID + ";" + haveFood + ";|";
                else
                    data += q[i].fldSelfServiceID + ";" + q[i].fldNobat + ";" +
                        MyLib.Shamsi.Shamsi2miladiString(q[i].fldFoodDate) + ";" + q[i].fldFoodInfoID + ";" + haveFood;
            }
            var sharj = p.sp_GetCharge(Convert.ToInt32(UserId)).FirstOrDefault();
            return data + "%" + sharj.Charge;
        }

        [WebMethod]
        public string Config()
        {
            UniAutomation.Areas.faces.Models.UniAutomationEntities1 p = new Areas.faces.Models.UniAutomationEntities1();
            var q = p.sp_tblFoodInfoSelect("", "", 0).ToList();
            string data = "";
            for (int i = 0; i < q.Count; i++)
            {
                data += q[i].fldID + ":" + q[i].fldFoodName + ";";
                if (i == q.Count - 1)
                    data += "|";
            }

            var q1 = p.sp_tblSelfServiceSelect("", "", 0).ToList();
            for (int i = 0; i < q1.Count; i++)
            {
                data += q1[i].fldID + ":" + q1[i].fldName + ":" + Convert.ToInt32(q1[i].fldJender) + ";";
                if (i == q1.Count - 1)
                    data += "|";
            }

            var q2 = p.sp_tblSettingSelect().FirstOrDefault();
            data += q2.fldReservDay;
            return data;
        }

        [WebMethod]
        public string Login(string UserName, string Pass)
        {
            Models.UniAutomationEntities p = new Models.UniAutomationEntities();
            var q = p.sp_CheckPersonPass(UserName, Class1.GenerateHash(Pass)).FirstOrDefault();

            if (q != null)
            {
                return q.fldID.ToString();
            }
            else
                return "0";
        }

        [WebMethod]
        public string Reserve(string Data, string UserId)
        {
            //Data=Id;Date;FoodId;SelfId;NobatId|
            Models.UniAutomationEntities p = new Models.UniAutomationEntities();
            var Ptype = p.sp_GetCartPersonType(Convert.ToInt32(UserId)).FirstOrDefault();
            var date = p.sp_GetDate().FirstOrDefault();
            var q = p.sp_tblTariffSelect("fldDate", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime),"","","","", 0).Where(h => h.fldRezervType == true && h.fldUserType == Ptype.PrsonType).FirstOrDefault();
            //var q = p.sp_tblTariffSelect("", "", 1).Where(h => h.fldRezervType == true).FirstOrDefault();
            var Info = Data.Split('|');
            string Result = "";
            for (int i = 0; i < Info.Count()-1; i++)
            {
                var RezervInfo = Info[i].Split(';');

                var sharj = p.sp_GetCharge(Convert.ToInt32(UserId)).FirstOrDefault();
                var food = p.sp_tblFoodProgramingSelect("fldFoodDate",MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(RezervInfo[1])), 0).Where(h => h.fldFoodInfoID == Convert.ToInt32(RezervInfo[2]) && h.fldSelfServiceID == Convert.ToInt32(RezervInfo[3]) && h.fldNobat == Convert.ToInt32(RezervInfo[4])).FirstOrDefault();
                if (sharj.Charge >= q.fldPrice && food != null)
                {
                    int count = 1;
                    if (count > 1)
                        count = 1;

                    var takhfif = p.sp_tblDarsadTakhfifSelect("ChekPersonTakhfif", UserId.ToString(), food.fldFoodDate, 0).FirstOrDefault();
                    var darsadtakhfif = 0;
                    var MablagheGhaza = q.fldPrice;
                    if (takhfif != null)
                        darsadtakhfif = takhfif.fldDarsadTakhfif;
                    MablagheGhaza = MablagheGhaza - (MablagheGhaza * darsadtakhfif/100);

                    p.sp_tblRezervInsert(Convert.ToInt32(UserId), food.fldID, count, MablagheGhaza, darsadtakhfif,"");
                    Result += RezervInfo[0] + ";1";
                    if (i < Info.Count() - 1)
                        Result += "|";
                }
                else
                {
                    Result += RezervInfo[0] + ";0";
                    if (i < Info.Count() - 1)
                        Result += "|";
                }
            }

            var sharj1 = p.sp_GetCharge(Convert.ToInt32(UserId)).FirstOrDefault();
            return Result + "%" + (sharj1.Charge);
        }

        [WebMethod]
        public string Cancel(string Data, string UserId)
        {
            //Data=Id;Date;FoodId;SelfId;NobatId|
            Models.UniAutomationEntities p = new Models.UniAutomationEntities();
            var Ptype = p.sp_GetCartPersonType(Convert.ToInt32(UserId)).FirstOrDefault();
            var date = p.sp_GetDate().FirstOrDefault();
            var q = p.sp_tblTariffSelect("fldDate", MyLib.Shamsi.Miladi2ShamsiString(date.fldDateTime),"","","","", 0).Where(h => h.fldRezervType == true && h.fldUserType == Ptype.PrsonType).FirstOrDefault();
            //var q = p.sp_tblTariffSelect("", "", 1).Where(h => h.fldRezervType == true).FirstOrDefault();
            var Info = Data.Split('|');
            string Result = "";
            for (int i = 0; i < Info.Count() - 1; i++)
            {
                var RezervInfo = Info[i].Split(';');
                var sharj = p.sp_GetCharge(Convert.ToInt32(UserId)).FirstOrDefault();
                var food = p.sp_tblFoodProgramingSelect("fldFoodDate", MyLib.Shamsi.Miladi2ShamsiString(Convert.ToDateTime(RezervInfo[1])), 0).Where(h => h.fldFoodInfoID == Convert.ToInt32(RezervInfo[2]) && h.fldSelfServiceID == Convert.ToInt32(RezervInfo[3]) && h.fldNobat == Convert.ToInt32(RezervInfo[4])).FirstOrDefault();

                var reserv = p.sp_tblRezervSelect("fldFoodProgramingID", food.fldID.ToString(), 0).Where(h => h.fldFoodCartID == Convert.ToInt32(UserId)).FirstOrDefault();
                if (reserv != null)
                {
                    var Currentdate = p.sp_GetDate().FirstOrDefault();
                    var day = p.sp_tblSettingSelect().FirstOrDefault();
                    if (MyLib.Shamsi.Shamsi2miladiDateTime(RezervInfo[1]) < Currentdate.fldDateTime.AddDays(Convert.ToInt32(day.fldReservDay) - 1))
                    {
                        Result += RezervInfo[0] + ";0";
                        if (i < Info.Count() - 1)
                            Result += "|";
                    }
                    else
                    {
                        p.sp_tblReservDelete(reserv.fldID);
                        var user = p.sp_GetCharge(Convert.ToInt32(UserId)).FirstOrDefault();
                        Result += RezervInfo[0] + ";1";
                        if (i < Info.Count() - 1)
                            Result += "|";
                    }
                }
                else
                {
                    Result += RezervInfo[0] + ";0";
                    if (i == Info.Count() - 1)
                        Result += "|";
                }
            }
            var sharj1 = p.sp_GetCharge(Convert.ToInt32(UserId)).FirstOrDefault();
            return Result + "%" + (sharj1.Charge);
        }
    }
}
