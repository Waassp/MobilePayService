using MobilePayService.Models;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace MobilePayService.Methods
{
    public class DBManager
    {

        public static SqlConnection GetConnection()
        {
            SqlConnection con;
            con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            return con;
        }

        public static void WithConnection(Action<SqlConnection> callback)
        {
            var conn = GetConnection();
            conn.Open();
            try
            {
                callback(conn);
            }
            finally
            {
                conn.Close();
            }
        }

        public static void InsertRecord(BCClientModel data)
        {
            WithConnection(conn =>
            {
                InsertRecord(data, conn);
            });
        }

        public static void InsertRecord(BCClientModel data, SqlConnection conn)
        {
            SqlCommand sqlCommand;
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            string query = "if exists(select 1 from MobilePayOnBoarding   where  [TenantIdFormatted] = '" + data.extractedTenantId + "' and [BCTenantId] = '" + data.BCTenantId + "') begin UPDATE MobilePayOnBoarding SET BCTenantId = '" + data.BCTenantId + "', State = '" + data.state + "' , CodeVerifier = '" + data.code_verifier + "', CodeChallenge = '" + data.code_challenge + "' where TenantIdFormatted = '" + data.extractedTenantId + "' and [BCTenantId] = '" + data.BCTenantId + "' end else begin  insert into MobilePayOnBoarding (BCTenantId,State,CodeVerifier,CodeChallenge,TenantIdFormatted) values('" + data.BCTenantId + "','" + data.state + "','" + data.code_verifier + "','" + data.code_challenge + "','" + data.extractedTenantId + "') end";
            sqlCommand = new SqlCommand(query, conn);
            sqlDataAdapter.InsertCommand = new SqlCommand(query, conn);
            sqlDataAdapter.InsertCommand.ExecuteNonQuery();
            sqlCommand.Dispose();
        }

        internal static string GetUrlByKey(int key)
        {
            string url = "";
            WithConnection(conn =>
            {
                url = GetUrlByKey(key, conn);
            });
            return url;
        }

        private static string GetUrlByKey(int key, SqlConnection conn)
        {
            string url = "";
            SqlCommand sqlCommand;
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            string query = "select *  from SessionUrl where id = " + key;
            sqlCommand = new SqlCommand(query, conn);
            using (SqlDataReader reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    url = reader.GetString(1);
                }
            }
            sqlCommand.Dispose();
            return url;
        }

        public static int InsertRecordSession(SessionUrl data)
        {
            int x = 0;
            WithConnection(conn =>
            {
                x = InsertRecordSession(data, conn);
            });
            return x;
        }

        public static int InsertRecordSession(SessionUrl data, SqlConnection conn)
        {
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            var query = "insert into SessionUrl (url,bctenantid) values('" + data.url + "','" + data.bcTenantId + "');  SELECT SCOPE_IDENTITY()";
            SqlCommand sqlCommand = new SqlCommand(query, conn);
            sqlDataAdapter.InsertCommand = new SqlCommand(query, conn);
            var val = sqlDataAdapter.InsertCommand.ExecuteScalar();
            sqlCommand.Dispose();
            return Convert.ToInt32(val);
        }

        public static void AddTokens(AccessTokenModel data)
        {
            WithConnection(conn =>
            {
                UpdateRecord(data, conn);
            });

        }

        public static void UpdateRecord(AccessTokenModel data, SqlConnection conn)
        {
            SqlCommand sqlCommand;
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            string query = "update MobilePayOnBoarding Set AuthCode='" + data.code + "',IdToken='" + data.id_token + "',AccessToken='" + data.access_token + "',RefreshToken='" + data.refresh_token + "' where State='" + data.state + "'";
            sqlCommand = new SqlCommand(query, conn);
            sqlDataAdapter.InsertCommand = new SqlCommand(query, conn);
            sqlDataAdapter.InsertCommand.ExecuteNonQuery();
            sqlCommand.Dispose();
        }

        public static void AddMerchantID(string merchantId, AccessTokenModel data)
        {
            WithConnection(conn =>
            {
                AddMerchantID(merchantId, data, conn);
            });

        }

        public static void AddMerchantID(string merchantId, AccessTokenModel data, SqlConnection conn)
        {
            SqlCommand sqlCommand;
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            string query = "update MobilePayOnBoarding Set MerchantId='" + merchantId + "' where AccessToken='" + data.access_token + "' and RefreshToken='" + data.refresh_token + "'";
            sqlCommand = new SqlCommand(query, conn);
            sqlDataAdapter.InsertCommand = new SqlCommand(query, conn);
            sqlDataAdapter.InsertCommand.ExecuteNonQuery();
            sqlCommand.Dispose();
        }


        public static string GetCodeVerifier(string state, Action<BCClientModel> callback)
        {
            string codeVerifier = "";
            WithConnection(conn =>
            {
                codeVerifier = GetCodeVerified(state, conn, callback);
            });
            return codeVerifier;
        }

        public static string GetCodeVerified(string state, SqlConnection conn, Action<BCClientModel> callback)
        {
            SqlCommand sqlCommand;
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            BCClientModel bCClient = new BCClientModel();
            string query = "select CodeVerifier,BCTenantIdfrom MobilePayOnBoarding where State='" + state + "'";
            string codeverified = "";
            sqlCommand = new SqlCommand(query, conn);
            using (SqlDataReader reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    codeverified = reader.GetString(0);
                    bCClient.BCTenantId = reader.GetString(1);
                }

            }
            sqlCommand.Dispose();
            callback(bCClient);
            return codeverified;
        }


        public static void VerifyTenantID(string query, Action<string> status)
        {
            WithConnection(conn =>
            {
                VerifyTenantID(query, conn, status);
            });
        }

        public static void VerifyTenantID(string query, SqlConnection conn, Action<string> status)
        {
            SqlCommand sqlCommand;
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            BCClientModel bCClient = new BCClientModel();
            int id = 0;
            sqlCommand = new SqlCommand(query, conn);
            using (SqlDataReader reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    id = reader.GetInt32(0);
                }

            }

            sqlCommand.Dispose();

            if (id > 0)
            {
                status("S");
            }
            else
            {
                status("E");
            }
            //return codeverified;
        }

        public static void VerifyInvoice(InvoiceModel Invoice, Action<BCClientModel> callback)
        {
            WithConnection(conn =>
            {
                VerifyInvoice(Invoice, conn, callback);
            });

        }

        public static void VerifyInvoice(InvoiceModel Invoice, SqlConnection conn, Action<BCClientModel> callback)
        {
            UpdateMobilePayDataInDB(Invoice.InvoiceId,Invoice.Date, conn);
            SqlCommand sqlCommand;
            string query = "select BCTenantURL,invoiceUrl from UserInvoice where InvoiceId='" + Invoice.InvoiceId + "'";
            string bcTenantId = "";
            sqlCommand = new SqlCommand(query, conn);
            using (SqlDataReader reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    bcTenantId = reader.GetString(0);
                    Invoice.InvoiceCallBackSoapURL = reader.GetString(1);

                }

            }
            sqlCommand.Dispose();
            if (!string.IsNullOrEmpty(bcTenantId))
            {
                UpdateInvoice(Invoice, conn);
                GetMerchantData(bcTenantId, conn, BcClientModel =>
                {
                    callback(BcClientModel);
                });
            }
            else
            {
                callback(null);
            }
        }

        private static void UpdateMobilePayDataInDB(string id, string date, SqlConnection conn)
        {
            SqlCommand sqlCommand;
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            string query = "update UserInvoice Set Date='" + date + "' where InvoiceId='" + id + "'";
            sqlCommand = new SqlCommand(query, conn);
            sqlDataAdapter.InsertCommand = new SqlCommand(query, conn);
            sqlDataAdapter.InsertCommand.ExecuteNonQuery();
            sqlCommand.Dispose();
        }

        private static void UpdateInvoice(InvoiceModel invoice, SqlConnection conn)
        {
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            var query = "Update UserInvoice Set status='" + invoice.Status + "', errorcode= '" + invoice.ErrorCode + "', errormessage= '" + invoice.ErrorMessage + "' where invoiceid='" + invoice.InvoiceId + "'";
            SqlCommand sqlCommand = new SqlCommand(query, conn);
            sqlDataAdapter.InsertCommand = new SqlCommand(query, conn);
            sqlDataAdapter.InsertCommand.ExecuteNonQuery();
            sqlCommand.Dispose();
        }

        public static void VerifyAgreement(string AgreementId, Action<BCClientModel> callback)
        {
            WithConnection(conn =>
            {
                VerifyAgreement(AgreementId, conn, callback);
            });
        }

        public static void VerifyAgreement(string AgreementId, SqlConnection conn, Action<BCClientModel> callback)
        {
            SqlCommand sqlCommand;
            string query = "select BCTenantId from UserAgreement where AgreementId='" + AgreementId + "'";
            string tenantId = "";
            sqlCommand = new SqlCommand(query, conn);
            using (SqlDataReader reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    tenantId = reader.GetString(0);
                }

            }
            sqlCommand.Dispose();
            if (!string.IsNullOrEmpty(tenantId))
            {
                GetMerchantData(tenantId, conn, BcClientModel =>
                {
                    callback(BcClientModel);
                });

            }
            else
            {
                callback(null);
            }


        }
        private static void GetMerchantData(string tenantId, SqlConnection conn, Action<BCClientModel> BcClientModel)
        {
            SqlCommand sqlCommand;
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            BCClientModel bCClient = new BCClientModel();
            string query = "select CodeVerifier,BCTenantId,AccessToken,RefreshToken from MobilePayOnBoarding where BCTenantId='" + tenantId + "'";
            sqlCommand = new SqlCommand(query, conn);
            using (SqlDataReader reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    bCClient.BCTenantId = reader.GetString(1);
                    if(!reader.IsDBNull(2))
                    bCClient.accessToken = string.IsNullOrEmpty(reader.GetString(2)) ? "" : reader.GetString(2);
                    if (!reader.IsDBNull(3))
                        bCClient.refreshToken = string.IsNullOrEmpty(reader.GetString(3)) ? "" : reader.GetString(3);                    
                }

            }
            sqlCommand.Dispose();
            BcClientModel(bCClient);
        }
        public static int GenerateInvoice(InvoiceModel invoice)
        {
            int x = 0;
            WithConnection(conn =>
            {
                x = GenerateInvoice(invoice, conn);
            });
            return x;
        }

        private static int GenerateInvoice(InvoiceModel invoice, SqlConnection conn)
        {

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            var query = "if exists(select 1 from UserInvoice   where  [InvoiceId] = '" + invoice.InvoiceId + "' ) Begin Select -1 End Else Begin insert into UserInvoice (invoiceId,invoiceUrl,BCTenantURL,Date) OUTPUT INSERTED.ID values('" + invoice.InvoiceId + "','" + invoice.InvoiceCallBackSoapURL + "','" + invoice.BCTenantURL + "','" + invoice.Date + "') End;";
            SqlCommand sqlCommand = new SqlCommand(query, conn);
            sqlDataAdapter.InsertCommand = new SqlCommand(query, conn);
            var val = sqlDataAdapter.InsertCommand.ExecuteScalar();
            sqlCommand.Dispose();
            return (Int32)val;
        }
        public static int InsertAgreement(AgreementModel agreement)
        {
            int x = 0;
            WithConnection(conn =>
            {
                x = InsertAgreement(agreement, conn);
            });
            return x;
        }

        private static int InsertAgreement(AgreementModel agreement, SqlConnection conn)
        {
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            var query = "if exists(select 1 from UserAgreement   where  [AgreementId] = '" + agreement.Agreement_Id + "' ) Begin Select -1 End Else Begin insert into UserAgreement (AgreementId,BCTenantId) OUTPUT INSERTED.ID values('" + agreement.Agreement_Id + "','" + agreement.BcTenantId + "') End;";
            SqlCommand sqlCommand = new SqlCommand(query, conn);
            sqlDataAdapter.InsertCommand = new SqlCommand(query, conn);
            var val = sqlDataAdapter.InsertCommand.ExecuteScalar();
            sqlCommand.Dispose();
            return (Int32)val;
        }

        public static void UpdateAgreement(AgreementModel agreement)
        {
            WithConnection(conn =>
            {
                UpdateAgreement(agreement, conn);
            });
        }

        private static void UpdateAgreement(AgreementModel agreement, SqlConnection conn)
        {
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            var query = "Update UserAgreement Set status='" + agreement.Status + "', StatusText= '" + agreement.Status_Text + "', StatusCode= '" + agreement.Status_Code + "', ExternalId= '" + agreement.External_Id + "' ,TimesStamp='" + agreement.Timestamp + "' where AgreementId='" + agreement.Agreement_Id + "'";
            SqlCommand sqlCommand = new SqlCommand(query, conn);
            sqlDataAdapter.InsertCommand = new SqlCommand(query, conn);
            sqlDataAdapter.InsertCommand.ExecuteNonQuery();
            sqlCommand.Dispose();
        }

        public static void GetTokens(string BcTenantId, Action<BCClientModel> callback)
        {
            WithConnection(conn =>
            {
                GetMerchantData(BcTenantId, conn, callback);
            });
        }

    }
}