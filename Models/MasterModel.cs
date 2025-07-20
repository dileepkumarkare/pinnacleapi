using Pinnacle.Helpers;
using Pinnacle.Helpers.JWT;
using Pinnacle.Entities;
using Microsoft.EntityFrameworkCore;

namespace Pinnacle.Models
{
    public class MasterModel
    {
        public string SaveSuccessMessage(int id, string type)
        {
            return id == 0 ? type + " Data saved successfully." : type + " Data updated successfully.";
        }
        public string FailedSaveMessage()
        {
            return "Failed to save data. Please try again!!";
        }
        public string AddSaveMessage(int id, string type)
        {
            return id == 0 ? type + " Data added successfully." : type + " Data updated successfully.";
        }

        public string RecordExist(string type)
        {
            return type + " already exist.";
        }
        public string NoDataMessage()
        {
            return "No data found.";
        }

        public string InvalidAccess()
        {
            return " Invalid access";
        }

        public string FetchMessage(object res, string type)
        {
            return res == null ? " No data found." : type + " data fetched successfully.";
        }
        public string UpdateStatusMessage(string type)
        {
            return type + " status updated successfully.";
        }
        public string MailSentMessage()
        {
            return "Mail sent successfully.";
        }

        public Ret CheckToken(string token)
        {
            try
            {
                if (token != null)
                {
                    JwtStatus tokenValues = CommonLogic.GetUserPermissions(token);
                    if (tokenValues.IsExpired)
                    {
                        return new Ret { status = false, IstokenExpired = true, message = "Token Expired" };
                    }
                    else
                    {
                        return new Ret { IstokenExpired = false, data = tokenValues };
                    }
                }
                else
                {
                    return new Ret { status = false, IstokenExpired = true, message = InvalidAccess() };
                }
            }
            catch (Exception ex)
            {
                return new Ret { status = "Error", message = "Failed to load data. Error: " + ex.InnerException };
            }
        }

        public Ret CheckAceess(bool condition)
        {
            try
            {
                if (condition)
                {
                    return new Ret { status = true, IstokenExpired = false, message = "Access granted" };
                }
                else
                {
                    return new Ret { status = false, IstokenExpired = false, message = InvalidAccess() };
                }
            }
            catch (Exception ex)
            {
                return new Ret { status = "Error", message = "Failed to load data. Error: " + ex.InnerException };
            }
        }

        public Ret CheckTokenTest(string token)
        {
            try
            {
                if (token != null)
                {
                    JwtStatus tokenValues = CommonLogic.GetUserPermissions(token);
                    if (tokenValues.IsExpired)
                    {
                        return new Ret { status = false, IstokenExpired = true, message = "Token Expired" };
                    }
                    else
                    {
                        return new Ret { IstokenExpired = false, data = tokenValues };
                    }
                }
                else
                {
                    return new Ret { status = false, IstokenExpired = true, message = InvalidAccess() };
                }
            }
            catch (Exception ex)
            {
                return new Ret { status = "Error", message = "Failed to load data. Error: " + ex.InnerException };
            }
        }
        public static List<T> PaginatedValues<T>(IQueryable<T> query, Pagination e)
        {
            if (e.PageNumber > 0 && e.PageSize > 0)
            {
                return query.Skip((e.PageNumber - 1) * e.PageSize).Take(e.PageSize).ToList();
            }
            query.ToQueryString();
            return query.ToList();
        }

        public static string AmountInWords(decimal Amount)
        {
            int _rupees = (int)Amount;
            int _paisa = (int)(Amount - _rupees) * 100;
            string Rupees = ConvertNumberToWords(_rupees);
            string Paisa = ConvertNumberToWords(_paisa);
            return _paisa > 0 ? Rupees + " rupees " + Paisa + " paisa only." : Rupees + " rupees only";
        }
        private static string ConvertNumberToWords(int number)
        {
            if (number == 0)
                return "Zero";

            if (number < 0)
                return "Minus " + ConvertNumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += ConvertNumberToWords(number / 1000000) + " Million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += ConvertNumberToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += ConvertNumberToWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                string[] unitsMap = { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                string[] tensMap = { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words.Trim();
        }
    }
}
