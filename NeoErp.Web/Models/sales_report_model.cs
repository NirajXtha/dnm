using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Oracle.ManagedDataAccess.Client;



namespace NeoErp.Models
{
    public class sales_report_model
    {
        public string db_user_name;
        public string db_user_password;
        public string port;
        public string host_name;
        public string service_name;
        public string connectionString;
        OracleDataReader dr;
        //constructor
        public sales_report_model(String db_user_name, String db_user_password, String port, String host_name, String service_name)
        {

            this.connectionString = $"User Id={db_user_name};Password={db_user_password};Data Source={host_name}/{service_name}";
            //this.connectionString = "User Id=MARRIOTT_8081;Password=MARRIOTT8081;Data Source=PCAVINASH/SYN";
            // this.connectionString = "User Id=IMS7980;Password=IMS7980;Data Source=PCAVINASH/SYN";
            //db_user_name
            //db_user_password
            //port
            //host_name
            //service_name


        }
        public dynamic sales_register(string company_code, string branch_code, string from_date, string to_date)
        {
            string queryString = $" select * from ( select charge_code,charge_edesc,priority_index_no,SPECIFIC_CHARGE_FLAG from IP_CHARGE_CODE where company_code='01' and charge_code in " +
                $"(SELECT DISTINCT CHARGE_CODE FROM  CHARGE_SETUP WHERE  FORM_CODE IN  (select distinct form_code from  SA_SALES_INVOICE where company_code='01')) " +
                $"order by priority_index_no  )a left outer join   (select  charge_code,charge_type_flag from charge_setup where company_code='01' and " +
                $"FORM_CODE IN (select distinct form_code from  SA_SALES_INVOICE where company_code='01') group by charge_code,charge_type_flag)b on a.charge_code=b.charge_code ";
            OracleConnection connection = new OracleConnection(this.connectionString);
            OracleCommand command = new OracleCommand(queryString, connection);
            //Debug.WriteLine(queryString);
            command.Connection.Open();
            dr = command.ExecuteReader();
            string all_charges = "";
            string all_charges_for_pivot = "";
            var chargq = new Dictionary<string, Dictionary<string, string>>();
            List<string> charge_code = new List<string>();
            List<string> charge_edesc = new List<string>();
            int x = 0;
            while (dr.Read())
            {
                var cha = new Dictionary<string, string>();

                charge_code.Add(dr[0].ToString());
                charge_edesc.Add(dr[1].ToString());
                all_charges = all_charges + dr[0].ToString() + ",";

                cha["charge_code"] = dr[0].ToString();
                cha["charge_edesc"] = dr[1].ToString();
                cha["specific_charge_flag"] = dr[3].ToString();
                cha["charge_type_flag"] = dr[5].ToString();
                chargq[dr[2].ToString()] = cha;

                x++;
            }

            for (int i = 0; i < charge_code.Count; i++)
            {
                Debug.WriteLine(charge_code[i]);

                if (i == charge_code.Count - 1)
                {
                    Debug.WriteLine($"asbdhasvdb{charge_code[i]}");
                    all_charges_for_pivot = all_charges_for_pivot + $"'{charge_code[i]}'" + charge_code[i];
                }
                else
                {
                    all_charges_for_pivot = all_charges_for_pivot + $"'{charge_code[i]}'" + charge_code[i] + ",";
                }

            }





            Debug.WriteLine(all_charges_for_pivot);

            queryString = $"select trunc(sales_date),sales_no,customer_edesc,ref_no,qty,amt,{all_charges}'dummy' from " +
                $"(select * from  ( " +
                $"select sales_no,sales_date,customer_code,sum(quantity) qty,sum(total_price) amt,'1'  tim " +
                $"from SA_SALES_INVOICE " +
                $"where deleted_flag='N'  and company_code='01' " +
                $"group by sales_no,sales_date,customer_code)a, ( select * from  ( select reference_no,CHARGE_CODE,sum(CHARGE_AMOUNT) charge_amt  " +
                $" from CHARGE_TRANSACTION where company_code='01' " +
                $"and form_code in (select distinct form_code from  SA_SALES_INVOICE ) " +
                $"group by reference_no,CHARGE_CODE order by  reference_no)pivot (sum(charge_amt) for charge_code in ( {all_charges_for_pivot} )  ) )b, " +
                $"(select customer_edesc,customer_code  code from sa_customer_setup where company_code='01') c, " +
                $"(select voucher_no,reference_no ref_no from master_transaction where company_code='01') d " +
                $"where  a.sales_no=b.reference_no(+) " +
                $" AND a.sALES_DATE between '17-JUL-2022' and  '17-oct-2023'" +
                $"and a.customer_code=c.code(+) " +
                $"and a.sales_no=d.voucher_no(+) " +
                $")order by sales_date,sales_no ";
            //connection = new OracleConnection(this.connectionString);
            command = new OracleCommand(queryString, connection);

            //dr=command.ExecuteNonQuery();
            Debug.WriteLine(queryString);

            dr = command.ExecuteReader();

            var sales_rep = new Dictionary<string, List<string>>();







            List<string> sales_no = new List<string>();
            List<string> sales_date = new List<string>();
            List<string> customer_edesc = new List<string>();
            List<string> ref_no = new List<string>();
            List<string> qty = new List<string>();
            List<string> amt = new List<string>();
            List<string> c1 = new List<string>();
            List<string> c2 = new List<string>();
            List<string> c3 = new List<string>();
            List<string> c4 = new List<string>();
            List<string> c5 = new List<string>();
            List<string> c6 = new List<string>();
            List<string> c7 = new List<string>();
            List<string> c8 = new List<string>();
            List<string> c9 = new List<string>();
            List<string> c10 = new List<string>();

            while (dr.Read())
            {
                sales_date.Add(dr[0].ToString());
                sales_no.Add(dr[1].ToString());
                customer_edesc.Add(dr[2].ToString());
                ref_no.Add(dr[3].ToString());
                qty.Add(dr[4].ToString());
                amt.Add(dr[5].ToString());
                for (int i = 0; i < charge_code.Count; i++)
                {
                    if (i == 0)
                    {
                        c1.Add(dr[6].ToString());
                    }
                    if (i == 1)
                    {
                        c2.Add(dr[7].ToString());
                    }
                    if (i == 2)
                    {
                        c3.Add(dr[8].ToString());
                    }
                    if (i == 3)
                    {
                        c4.Add(dr[9].ToString());

                    }
                    if (i == 4)
                    {
                        c5.Add(dr[10].ToString());
                    }
                    if (i == 5)
                    {
                        c6.Add(dr[11].ToString());
                    }
                    if (i == 6)
                    {
                        c7.Add(dr[12].ToString());
                    }
                    if (i == 7)
                    {
                        c8.Add(dr[13].ToString());
                    }
                    if (i == 8)
                    {
                        c9.Add(dr[14].ToString());
                    }
                    if (i == 9)
                    {
                        c10.Add(dr[15].ToString());
                    }

                }
            }

            sales_rep["voucher_no"] = sales_no;
            sales_rep["date"] = sales_date;
            sales_rep["customer"] = customer_edesc;
            sales_rep["ref_no"] = ref_no;

            sales_rep["qty"] = qty;
            sales_rep["amt"] = amt;

            for (int i = 0; i < charge_code.Count; i++)
            {
                if (i == 0)
                {
                    sales_rep[charge_code[i]] = c1;

                }
                if (i == 1)
                {
                    sales_rep[charge_code[i]] = c2;
                }
                if (i == 2)
                {
                    sales_rep[charge_code[i]] = c3;
                }
                if (i == 3)
                {
                    sales_rep[charge_code[i]] = c4;

                }
                if (i == 4)
                {
                    sales_rep[charge_code[i]] = c5;
                }
                if (i == 5)
                {
                    sales_rep[charge_code[i]] = c6;
                }
                if (i == 6)
                {
                    sales_rep[charge_code[i]] = c7;
                }
                if (i == 7)
                {
                    sales_rep[charge_code[i]] = c8;
                }
                if (i == 8)
                {
                    sales_rep[charge_code[i]] = c9;
                }
                if (i == 9)
                {
                    sales_rep[charge_code[i]] = c10;
                }


            }
            dr.Close();

            var final_rep = new Dictionary<string, Dictionary<string, List<string>>>();

            final_rep["Detail"] = sales_rep;


            var obj = new
            {
                detail = sales_rep,
                charge_detail = chargq

            };
            return obj;
        }

    }
}