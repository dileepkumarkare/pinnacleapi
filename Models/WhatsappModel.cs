using Pinnacle.Entities;
using Pinnacle.Helpers.JWT;
using Pinnacle.IServices;
using System.ServiceModel.Channels;
using Google.Cloud.Translation.V2;
using System;
using System.Collections.Generic;
using DevExpress.Data.Localization;
using Pinnacle.Helpers;
using System.Diagnostics.CodeAnalysis;

namespace Pinnacle.Models
{
    public class WhatsappModel
    {
        private readonly IWhatsappService _whatsappService;
        PinnacleDbContext db = new PinnacleDbContext();
        public WhatsappModel(IWhatsappService whatsappService)
        {
            _whatsappService = whatsappService;
        }

        public async Task SendUHIDNumberToWhatsapp(string PhoneNumber, string PatientName, string UMRNo, string Nationality, string AreaCode, JwtStatus jwtData)
        {
            HospitalEntity hospital = db.Hospital.Where(h => h.HospitalId == jwtData.HospitalId).FirstOrDefault();

            var _state = (from a in db.PincodeData
                          join b in db.District on a.DistrictId equals b.Id
                          where a.Id == Convert.ToInt32(AreaCode)
                          select new { b.StateId }).FirstOrDefault();

            string Message = "Dear *" + PatientName + ",* " +
                "We’re pleased to welcome you to Pinnacle Hospital!\r\n" +
                "Your Unique Hospital Identification Number (UHID) is: *" + UMRNo + "*\r\n" +
                "Please keep this number safe and use it during future consultations, diagnostics, pharmacy purchases, or online appointments.\r\n" +
                "At Pinnacle Hospital, we are committed to your health and well-being.\r\n" +
                "For any assistance or queries, feel free to contact us at *" + hospital.PrintContactNo + "* or visit \r\n *" + hospital.WebSiteLink + "*.\r\n" +
                "Wishing you good health,\r\n" +
                "*Team Pinnacle Hospital*\r\n\r\n";

            if (Nationality == "Indian")
            {
                if (_state is not null && _state.StateId != null && (_state.StateId == 2 || _state.StateId == 31))
                {
                    Message += "ప్రియమైన *" + CommonLogic.TranslateText(PatientName, "te") + "* గారు, " +
                    "\r\n*పిన్నాకిల్ హాస్పిటల్* కు స్వాగతం!\r\n" +
                    "మీ యూనిక్ హాస్పిటల్ ఐడెంటిఫికేషన్ నంబర్ (UHID): *" + UMRNo + "*\r\n\r\n" +
                    "దయచేసి ఈ నంబర్‌ను జాగ్రత్త గా ఉంచవలెను మరియు భవిష్యత్తులో నిర్వహించే కన్సల్టేషన్లు, డయగ్నోస్టిక్స్, ఫార్మసీ కొనుగోళ్ళు మరియు ఆన్లైన్ అపాయింట్మెంట్ల సమయంలో ఉపయోగించండి.\r\n" +
                    "*పిన్నాకిల్ హాస్పిటల్* మీ ఆరోగ్యాన్ని అత్యంత ప్రాధాన్యంగా తీసుకుంటుంది.\r\n" +
                    "ఏవైనా సందేహాలు లేదా సహాయం కావాలంటే, దయచేసి *" + hospital.PrintContactNo + "* కు కాల్ చేయండి లేదా \r\n *" + hospital.WebSiteLink + "* ని సందర్శించండి.\r\n" +
                    "మీరు ఎల్లప్పుడూ ఆరోగ్యంగా ఉండాలని ఆశిస్తున్నాము.\r\n*టీమ్ పిన్నాకిల్ హాస్పిటల్*\r\n\r\n";
                }
                else if (_state is not null && _state.StateId != null && _state.StateId == 25)
                {
                    Message += "ପ୍ରିୟ *" + CommonLogic.TranslateText(PatientName, "or") + ", ପିନାକଲ୍l* କୁ ସ୍ବାଗତ।\r\n" +
                    "ଆପଣଙ୍କର " + CommonLogic.TranslateText("Unique Hospital Identification Number", "or") + " (UHID) ହେଉଛି: *" + UMRNo + "*\r\n" +
                    "ଦୟାକରି ଏହି ନମ୍ବରକୁ ସୁରକ୍ଷିତ ରଖନ୍ତୁ ଏବଂ ଆଗାମୀ ଚିକିତ୍ସା, ଡାୟଗ୍ନୋଷ୍ଟିକ୍ସ, ଫାର୍ମାସୀ କିମ୍ବା ବା ଅନଲାଇନ୍ ଆପଏନ୍ଟମେଣ୍ଟ ସମୟରେ ବ୍ୟବହାର କରନ୍ତୁ।\r\n" +
                    "*ପିନାକଲ୍ ହସ୍ପିଟାଲ୍* ଆପଣଙ୍କ ଆରୋଗ୍ୟ ଓ ଭଲ ପାଇଁ ଦୃଢ଼ ପ୍ରତିଶ୍ରୁତ।\r\nଯେଉଁଠି ଆପଣଙ୍କୁ ଜରୁରୀ ଉପଦେଶ ବା ସହଯୋଗ ଆବଶ୍ୟକ, ଦୟାକରି  *" + hospital.PrintContactNo + "* କୁ କଲ୍ କରନ୍ତୁ ବା \r\n *" + hospital.WebSiteLink + "*. ଦେଖନ୍ତୁ।\r\n" +
                    "ଆପଣଙ୍କୁ ଶୁଭ ଆରୋଗ୍ୟ କାମନା।\r\n" +
                    "*ଟିମ୍ ପିନାକଲ୍ ହସ୍ପିଟାଲ୍*\r\n";
                }
                else
                {
                    Message += "प्रिय *" + CommonLogic.TranslateText(PatientName, "hi") + " जी, पिन्नाकल हॉस्पिटल* में आपका स्वागत है!\r\n" +
                        "आपका यूनिक हॉस्पिटल आइडेंटिफिकेशन नंबर (UHID) है: *" + UMRNo + "*\r\n" +
                        "कृपया इस नंबर को सुरक्षित रखें और भविष्य की परामर्श, डायग्नोस्टिक्स, फार्मेसी खरीद या ऑनलाइन अपॉइंटमेंट्स में इसका उपयोग करें।\r\n" +
                        "*पिन्नाकल हॉस्पिटल* आपकी सेहत और भलाई के लिए प्रतिबद्ध है।\r\n" +
                        "अगर आपको किसी सहायता या जानकारी की आवश्यकता हो, तो कृपया *" + hospital.PrintContactNo + "* पर संपर्क करें या \r\n *" + hospital.WebSiteLink + "* पर जाएँ।\r\n" +
                        "आपके अच्छे स्वास्थ्य की कामना करते हैं।\r\n" +
                        "*टीम पिन्नाकल हॉस्पिटल*\r\n";
                }
            }

            _whatsappService.SendMessageAsync(PhoneNumber, Message);

        }
        public static void TranslateText(string Text, string TargetLanguage)
        {
            List<string> _translateText = new List<string>() { Text };
            var client = TranslationClient.Create();

            foreach (var text in _translateText)
            {
                try
                {
                    // Translate the text
                    var response = client.TranslateText(text, TargetLanguage, "en"); // Translate from English to Telugu
                    Console.WriteLine($"Original: {text}, Translated: {response.TranslatedText}");

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Translation error: {ex.Message}");
                }

            }

        }
    }
}
