using Pinnacle.Entities;
using FastMember;
using Pinnacle.Helpers.JWT;
using Serilog;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Collections;


namespace Pinnacle.Helpers
{
    public class CommonLogic
    {
        PinnacleDbContext db = new PinnacleDbContext();
        private static readonly string EncryptionKey = "PinnacleApi2025";
        public static string Encrypt(string clearText)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public static string Decrypt(string cipherText)
        {
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }


        public static JwtStatus GetUserPermissions(string jwtToken)
        {
            try
            {
                if (jwtToken != null)
                {
                    string jwt = jwtToken.Replace("Bearer ", "");
                    var handler = new JwtSecurityTokenHandler();
                    var token = handler.ReadJwtToken(jwt);
                    var claims = token.Claims;
                    JwtStatus JwtStatus = new JwtStatus();
                    DataTable table = new DataTable();
                    using (var reader = ObjectReader.Create(claims))
                    {
                        table.Load(reader);
                    }
                    DataRow[] row = table.Select("Type='exp'");
                    JwtStatus.IsExpired = DateTimeOffset.UtcNow.ToUnixTimeSeconds() > Convert.ToInt32(row[0]["Value"]) ? true : false;

                    //row = table.Select("Type='userId'");
                    //JwtStatus.Id = Convert.ToInt32(row[0]["Value"].ToString());
                    row = table.Select("Type='Id'");
                    JwtStatus.Id = Convert.ToInt32(row[0]["Value"].ToString());
                    row = table.Select("Type='HospitalId'");
                    JwtStatus.HospitalId = Convert.ToInt32(row[0]["Value"].ToString());
                    row = table.Select("Type='userName'");
                    JwtStatus.UserName = row[0]["Value"].ToString();

                    return JwtStatus;
                }
                else
                {
                    JwtStatus JwtStatus = new JwtStatus();
                    JwtStatus.IsExpired = true;
                    return JwtStatus;
                }
            }
            catch (Exception ex)
            {
                //Log.Information("Save Facility at " + DateTime.Now.ToString() + " message " + (ex.Message));
                return new JwtStatus();
            }
        }

        public ConfigKeyInfo getConfigValues()
        {
            var configs = db.Config.ToList();
            ConfigKeyInfo config = new ConfigKeyInfo();
            foreach (var eachConfig in configs)
            {
                //config.{eachConfig.ConfigKey} = eachConfig.ConfigValue;
                if (eachConfig.ConfigKey == "SMTP_HOST")
                    config.SMTP_HOST = eachConfig.ConfigValue;
                if (eachConfig.ConfigKey == "SMTP_PORT")
                    config.SMTP_PORT = eachConfig.ConfigValue;
                if (eachConfig.ConfigKey == "SMTP_USERNAME")
                    config.SMTP_USERNAME = eachConfig.ConfigValue;
                if (eachConfig.ConfigKey == "SMTP_PASSWORD")
                    config.SMTP_PASSWORD = eachConfig.ConfigValue;
                if (eachConfig.ConfigKey == "HOME_URL")
                    config.HOME_URL = eachConfig.ConfigValue;

            }
            return config;
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public bool upload(IFormFile FacilityImage, string filename, bool isUpdate = false)
        {
            try
            {
                if (isUpdate)
                {
                    var facilityId = Path.GetFileNameWithoutExtension(filename);
                    DirectoryInfo dir = new DirectoryInfo(Path.GetFullPath(filename)).Parent;
                    FileInfo[] files = dir.GetFiles(facilityId + ".*");
                    if (files.Length > 0)
                    {
                        foreach (FileInfo file in files)
                        {
                            File.Delete(file.FullName);
                        }
                    }
                }
                using (var filestream = System.IO.File.Create(filename))
                {
                    FacilityImage.CopyTo(filestream);
                    filestream.Flush();
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Information("Delete Schedule at " + DateTime.Now.ToString() + " message " + (ex.Message));
                return false;
            }
        }

        public Ret SaveAuditLog(AuditLog auditLog)
        {
            try
            {
                db.AuditLog.Add(auditLog);
                db.SaveChanges();
                return new Ret { status = true, message = "Audit log saved successfully!" };
            }
            catch (Exception ex)
            {
                Log.Information("Failed to save audit log at" + DateTime.Now.ToString() + " message " + (ex.Message));
                return new Ret { status = false, message = "Something went wrong" };
            }
        }
        public string IntArrayToString(int[] physicianIds)
        {
            string physicianId = "ALL";
            if (physicianIds != null && physicianIds.Length > 0)
            {
                physicianId = string.Join(",", physicianIds);
            }
            return physicianId;
        }
        public string StringArrayToString(string[] appointmentStatus)
        {
            string appointmentStatusString = "ALL";
            if (appointmentStatus != null && appointmentStatus.Length > 0)
            {
                appointmentStatusString = string.Join(",", appointmentStatus.Select(item => "'" + item + "'"));
            }
            return appointmentStatusString;
        }
        public static dynamic GetJsonObject(DataSet ds)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            dynamic json = JsonConvert.SerializeObject(ds.Tables[0], jsonSerializerSettings);
            JsonArray finalArray = JsonArray.Parse(json);
            return finalArray;
        }
        public static dynamic GetSQLJsonObject(DataSet ds)
        {
            dynamic jObject = JsonConvert.DeserializeObject(ds.Tables[0].Rows[0][0].ToString(), typeof(ExpandoObject));

            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                //PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            var serialized = JsonConvert.SerializeObject(jObject, settings);
            JsonArray serializedArray = JsonArray.Parse(serialized);

            return serializedArray;

        }
        public static dynamic GetJsonArray(DataSet ds)
        {
            var finalArray = new JsonArray();
            JsonObject dataObject = new JsonObject();
            string[] sectionNames = { "OPConsultations", "Gender", "PatientType", "Doctors", "DateWise", "DoctorDateWise", "DateWisePatientType", "Age0To15", "Age16To25", "Age26To45", "Age46To60", "Age59To70", "AgeAbove70" };

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            for (int i = 0; i < ds.Tables.Count && i < sectionNames.Length; i++)
            {
                if (ds.Tables[i].Rows.Count > 0)
                {
                    var json = JsonConvert.SerializeObject(ds.Tables[i], jsonSerializerSettings);
                    JsonNode? parsedNode = JsonNode.Parse(json);
                    if (parsedNode is JsonArray jsonArray)
                    {
                        dataObject[sectionNames[i]] = jsonArray;
                    }

                }
            }
            return dataObject;
        }

        public static string TranslateText(string input, string traslateTo)
        {
            #region Issue with Entire Text Translation

            //// Set the language from/to in the url (or pass it into this function)
            //string url = String.Format
            //("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
            // "en", "ar", Uri.EscapeUriString(input));
            //HttpClient httpClient = new HttpClient();
            //string result = httpClient.GetStringAsync(url).Result;

            //// Get all json data
            ////var jsonData = new JavaScriptSerializer().Deserialize<List<dynamic>>(result);
            //var jsonData = JsonConvert.DeserializeObject<List<dynamic>>(result);

            //// Extract just the first array element (This is the only data we are interested in)
            //var translationItems = jsonData[0];

            //// Translation Data
            //string translation = "";

            //// Loop through the collection extracting the translated objects
            //foreach (object item in translationItems)
            //{
            //    // Convert the item array to IEnumerable
            //    IEnumerable translationLineObject = item as IEnumerable;

            //    // Convert the IEnumerable translationLineObject to a IEnumerator
            //    IEnumerator translationLineString = translationLineObject.GetEnumerator();

            //    // Get first object in IEnumerator
            //    translationLineString.MoveNext();

            //    // Save its value (translated text)
            //    translation += string.Format(" {0}", Convert.ToString(translationLineString.Current));
            //}

            //// Remove first blank character
            //if (translation.Length > 1) { translation = translation.Substring(1); };

            //// Return translation
            //return translation;
            #endregion

            // Set the language from/to in the URL
            string baseUrl = "https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}";
            string urlTemplate = String.Format(baseUrl, "en", traslateTo, "{0}");

            HttpClient httpClient = new HttpClient();
            List<string> translationParts = new List<string>();

            // Regex to identify URLs
            string urlPattern = @"(https?:\/\/[^\s]+)";
            var segments = System.Text.RegularExpressions.Regex.Split(input, urlPattern);

            foreach (string segment in segments)
            {
                // If the segment matches the URL pattern, skip translation
                if (System.Text.RegularExpressions.Regex.IsMatch(segment, urlPattern))
                {
                    translationParts.Add(segment); // Keep the URL as it is
                }
                else
                {
                    // Split the segment into smaller chunks (e.g., sentences)
                    string[] sentences = segment.Split(new[] { ". ", "! ", "? " }, StringSplitOptions.None);

                    foreach (string sentence in sentences)
                    {
                        // Skip empty sentences
                        if (string.IsNullOrWhiteSpace(sentence))
                            continue;

                        // Prepare URL with the encoded sentence
                        string url = String.Format(urlTemplate, Uri.EscapeUriString(sentence));

                        // Call the translation API
                        string result = httpClient.GetStringAsync(url).Result;

                        // Deserialize the JSON data
                        var jsonData = JsonConvert.DeserializeObject<List<dynamic>>(result);

                        // Extract the translation
                        var translationItems = jsonData[0];
                        string translation = "";

                        foreach (object item in translationItems)
                        {
                            IEnumerable translationLineObject = item as IEnumerable;
                            IEnumerator translationLineString = translationLineObject.GetEnumerator();
                            translationLineString.MoveNext();
                            translation += Convert.ToString(translationLineString.Current);
                        }

                        // Add the translated sentence to the list
                        translationParts.Add(translation);
                    }
                }
            }

            // Join all translated parts into a single string
            return string.Join(" ", translationParts);



        }
    }



}
