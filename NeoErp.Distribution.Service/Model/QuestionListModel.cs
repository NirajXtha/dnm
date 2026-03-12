using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
    public class QuestionListModel
    {
        public QuestionListModel()
        {
            General = new List<QuestionSetupModel>();
            Tabular = new List<TabularModel>();
        }
        public List<QuestionSetupModel> General { get; set; }
        public List<TabularModel> Tabular { get; set; }
    }

    public class QuestionSetModel
    {
        public int SET_CODE { get; set; }
        public string TITLE { get; set; }
        public string TYPE { get; set; }
    }
    public class SURVEY_COLUMN_MODEL
    {
        public string title { get; set; }
        public string field { get; set; }
    }

    public class SURVEY_REPORT_MODEL
    {
        public string SURVEY_EDESC { get; set; }
        public string DIST_OUTLET_NAME { get; set; }
        public string FULL_NAME { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string SUBTYPE_EDESC { get; set; }
        public string TYPE_EDESC { get; set; }
        public string Q1 { get; set; }
        public string Q2 { get; set; }
        public string Q3 { get; set; }
        public string Q4 { get; set; }
        public string Q5 { get; set; }
        public string Q6 { get; set; }
        public string Q7 { get; set; }
        public string Q8 { get; set; }
        public string Q9 { get; set; }
        public string Q10 { get; set; }
        public string Q11 { get; set; }
        public string Q12 { get; set; }
        public string Q13 { get; set; }
        public string Q14 { get; set; }
        public string Q15 { get; set; }
        public string Q16 { get; set; }
        public string Q17 { get; set; }
        public string Q18 { get; set; }
        public string Q19 { get; set; }
        public string Q20 { get; set; }

        public int? QN2_LNT { get; set; }
        public int? QN2_SANDESH { get; set; }
        public int? QN2_TIME_PASS { get; set; }
        public int? QN2_UPAKAR { get; set; }
        public int? QN2_ANY_OTHER { get; set; }

        public int? TASTE { get; set; }
        public int? LOW_PRICE { get; set; }
        public int? QUALITY { get; set; }

        public int? GOOD { get; set; }
        public int? AVERAGE { get; set; }
        public int? BAD { get; set; }

        public int? TASTY { get; set; }
        public int? CRUNCHY { get; set; }

        public int? AATA { get; set; }
        public int? RICE { get; set; }
        public int? SUJI { get; set; }
        public int? DAAL { get; set; }
        

        public int? LNT { get; set; }
        public int? SANDESH { get; set; }
        public int? TIME_PASS { get; set; }
        public int? UPAKAR { get; set; }
        public int? ANY_OTHER { get; set; }
    }

    public class SURVEY_REPORT_AATA_MODEL
    {
        public string SURVEY_EDESC { get; set; }
        public string DIST_OUTLET_NAME { get; set; }
        public string FULL_NAME { get; set; }
        public DateTime? CREATED_DATE { get; set; }

        public string Q1 { get; set; }
        public string Q2 { get; set; }
        public string Q3 { get; set; }
        public string Q4 { get; set; }
        public string Q5 { get; set; }
        public string Q6 { get; set; }
        public string Q7 { get; set; }
        public string Q8 { get; set; }
        public string Q9 { get; set; }
        public string Q10 { get; set; }
        public string Q11 { get; set; }
        public string Q12 { get; set; }
        public string Q13 { get; set; }

        public int? Q1_HULAS { get; set; } = 0;
        public int? Q1_GYAN { get; set; } = 0;
        public int? Q1_KALP { get; set; } = 0;
        public int? Q1_FORTUNE { get; set; } = 0;
        public int? Q1_OTHERS { get; set; } = 0;

        public int? Q2_HULAS { get; set; } = 0;
        public int? Q2_GYAN { get; set; } = 0;
        public int? Q2_KALP { get; set; } = 0;
        public int? Q2_FORTUNE { get; set; } = 0;
        public int? Q2_OTHERS { get; set; } = 0;

        public int? Q3_HULAS { get; set; } = 0;
        public int? Q3_GYAN { get; set; } = 0;
        public int? Q3_KALP { get; set; } = 0;
        public int? Q3_FORTUNE { get; set; } = 0;
        public int? Q3_OTHERS { get; set; } = 0;

        public int? Q4_HULAS { get; set; } = 0;
        public int? Q4_GYAN { get; set; } = 0;
        public int? Q4_KALP { get; set; } = 0;
        public int? Q4_FORTUNE { get; set; } = 0;
        public int? Q4_OTHERS { get; set; } = 0;

        public int? Q5_HULAS { get; set; } = 0;
        public int? Q5_GYAN { get; set; } = 0;
        public int? Q5_KALP { get; set; } = 0;
        public int? Q5_FORTUNE { get; set; } = 0;
        public int? Q5_OTHERS { get; set; } = 0;

        public int? Q7_HULAS { get; set; } = 0;
        public int? Q7_GYAN { get; set; } = 0;
        public int? Q7_KALP { get; set; } = 0;
        public int? Q7_FORTUNE { get; set; } = 0;
        public int? Q7_OTHERS { get; set; } = 0;

        public int? Q8_HULAS { get; set; } = 0;
        public int? Q8_GYAN { get; set; } = 0;
        public int? Q8_KALP { get; set; } = 0;
        public int? Q8_FORTUNE { get; set; } = 0;
        public int? Q8_OTHERS { get; set; } = 0;

        public int? Q9_HULAS { get; set; } = 0;
        public int? Q9_GYAN { get; set; } = 0;
        public int? Q9_KALP { get; set; } = 0;
        public int? Q9_FORTUNE { get; set; } = 0;
        public int? Q9_OTHERS { get; set; } = 0;

    }
    public class SURVEY_AATA_ANS_TABULAR_MODEL
    {
        public int SURVEY_CODE { get; set; }
        public string SURVEY_EDESC { get; set; }
        public string TABLE_TITLE { get; set; }
        public string DIST_OUTLET_NAME { get; set; }
        public string FULL_NAME { get; set; }
        public string ACTUAL_ANSWER { get; set; }
        public string ENTITY_CODE { get; set; }
        public DateTime? CREATED_DATE { get; set; }

    }
    public class SURVEY_AATA_ANS_MODEL
    {
        public int? HULAS { get; set; }
        public int? GYAN { get; set; }
        public int? KALP { get; set; }
        public int? FORTUNE { get; set; }
        public int? OTHERS { get; set; }
    }
    public class SURVEY_ANS_MODEL
    {
        public int? LNT { get; set; }
        public int? SANDESH { get; set; }
        public int? TIME_PASS { get; set; }
        public int? UPAKAR { get; set; }
        public int? ANY_OTHER { get; set; }
    }
    public class SurveyModel
    {
        public int SURVEY_CODE { get; set; }
        public string SURVEY_EDESC { get; set; }
        public List<string> SP_CODES { get; set; }
        public List<QuestionSetModel> QUESTIONS { get; set; }
        public DateTime EXPIRY_DATE { get; set; }
        public string SP_CODE_STR { get; set; }
        public string SET_INFO { get; set; }
        public string TITLE { get; set; }
        public string AREA_CODES { get; set; }
        public string GROUP_CODES { get; set; }
    }

    public class SURVEY_REPORT_BRANDING_MODEL
    {
        public string SURVEY_EDESC { get; set; }
        public string DIST_OUTLET_NAME { get; set; }
        public string FULL_NAME { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string SUBTYPE_EDESC { get; set; }
        public string TYPE_EDESC { get; set; }
        public string Q1 { get; set; }
        public string Q2 { get; set; }
        public string Q3 { get; set; }
        public string Q4 { get; set; }
        public string Q5 { get; set; }
        public string Q6 { get; set; }
        public string Q7 { get; set; }
        public string Q8 { get; set; }
        public string Q9 { get; set; }
        public string Q10 { get; set; }
        public string Q11 { get; set; }
        public string Q12 { get; set; }
        public string Q13 { get; set; }
        public string Q14 { get; set; }
        public string Q15 { get; set; }
        public string Q16 { get; set; }
        public string Q17 { get; set; }
        public string Q18 { get; set; }
        public string Q19 { get; set; }
        public string Q20 { get; set; }

        public int? Q2_TWOD { get; set; }
        public int? Q2_GSB { get; set; }
        public int? Q2_FLEX { get; set; }
        public int? Q2_DPS { get; set; }
        public int? Q2_VACUUM { get; set; }
        public int? Q2_Others { get; set; }

        public int? Q5_TWOD { get; set; }
        public int? Q5_GSB { get; set; }
        public int? Q5_FLEX { get; set; }
        public int? Q5_DPS { get; set; }
        public int? Q5_VACUUM { get; set; }
        public int? Q5_Others { get; set; }

       
        //public BRANDING_TYPE brd_type { get; set; }
        //public BRANDING_BRAND brd_brand { get; set; }
    }

    public class SurveyDDl
    {
        public int SURVEY_CODE { get; set; }
        public string SURVEY_EDESC { get; set; }
    }
    public class WEB_QUESTION_ASNWER
    {
        public string  QA_TYPE { get; set; }
        public string QUESTION { get; set; }
        public string ANSWERS { get; set; }
        public int SET_CODE { get; set; }
    }

    public class WEB_QUESTION_ASNWER_SAVE
    {
        public string TXT_QN { get; set; }
        public string BOL_QN { get; set; }
        public string MCR_QN { get; set; }
        public string MCC_QN { get; set; }
        public string TXT_ANS { get; set; }
        public string BOL_ANS { get; set; }
        public string MCR_ANS { get; set; }
        public string MCC_ANS { get; set; }
    }

    public class WEB_TEST
    {
        
        public string DATA { get; set; }

    }
    public class DDL_TEMPLATE
    {
        public string ID { get; set; }
        public string NAME { get; set; }
       
    }

    public class BRANDING_TYPE
    {
        public string TWOD { get;set;}
        public string GSB { get; set; }
        public string FLEX { get; set; }
        public string DPS { get; set; }
        public string VACUUM { get; set; }
       
    }
    public class BRANDING_BRAND
    {
        public int? MS { get; set; }
        public int? RV { get; set; }
        public int? GMR { get; set; }
        public int? HB { get; set; }
        public int? BD { get; set; }
        public int? WB { get; set; }
        public int? NB { get; set; }
        public int? RS { get; set; }
        public int? NP { get; set; }
        public int? MI { get; set; }
        public int? RT { get; set; }
        public int? BO { get; set; }
        public int? GO { get; set; }
    }

}

