using DevExpress.XtraReports.UI;
using DevExpress.XtraRichEdit.API.Native;
using Pinnacle.Helpers;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;

namespace Pinnacle.Reports
{
    public partial class DPCardioMedicineList : DevExpress.XtraReports.UI.XtraReport
    {
        public DPCardioMedicineList()
        {
            InitializeComponent();
        }

        private void DPMedicineListSR_BeforePrint(object sender, CancelEventArgs e)
        {

        }
        private void xrPictureBox1_BeforePrint(object sender, CancelEventArgs e)
        {
            XRTableCell TC = this.FindControl("xrTableCell13", true) as XRTableCell;
            string text = TC.Text;

            XRPictureBox pictureBox = sender as XRPictureBox;
            if (!string.IsNullOrEmpty(text) && text != "-1")
            {

            }
            else
            {

            }

        }

        private void xrTableCellAfternoon_BeforePrint(object sender, CancelEventArgs e)
        {

        }
        private void xrRichText3_BeforePrint(object sender, CancelEventArgs e)
        {
            XRRichText xrRichText3 = sender as XRRichText;
            if (xrRichText3.Text.Split(':')[0].ToString() == "-2" && (xrRichText3.Text.Split(':')[1].ToString() == "2" || xrRichText3.Text.Split(':')[1].ToString() == "31"))
            {
                xrRichText3.Text = "Morning ( ఉదయం )";
            }
            else if (xrRichText3.Text.Split(':')[0].ToString() == "-2" && xrRichText3.Text.Split(':')[1].ToString() == "25")
            {
                xrRichText3.Text = "Morning ( ପ୍ରଭାତ )";
            }
            else if (xrRichText3.Text.Split(':')[0].ToString() == "-2" && !string.IsNullOrEmpty(xrRichText3.Text.Split(':')[1].ToString()))
            {
                xrRichText3.Text = "Morning ( सुबह )";
            }
            else if (xrRichText3.Text.Split(':')[0].ToString() == "-2" && string.IsNullOrEmpty(xrRichText3.Text.Split(':')[1].ToString()))
            {
                xrRichText3.Text = "Morning";
            }
            else
            {
                xrRichText3.Text = "";
            }
        }
        private void xrRichText5_BeforePrint(object sender, CancelEventArgs e)
        {
            XRRichText xrRichText5 = sender as XRRichText;
            if (xrRichText5.Text.Split(':')[0].ToString() == "-2" && xrRichText5.Text.Split(':')[1].ToString() == "2" || xrRichText5.Text.Split(':')[1].ToString() == "31")
            {
                xrRichText5.Text = "Afternoon ( మధ్యాహ్నం )";
            }
            else if (xrRichText5.Text.Split(':')[0].ToString() == "-2" && xrRichText5.Text.Split(':')[1].ToString() == "25")
            {
                xrRichText5.Text = "Afternoon ( ଅପରାହ୍ନ | )";
            }
            else if (xrRichText5.Text.Split(':')[0].ToString() == "-2" && !string.IsNullOrEmpty(xrRichText5.Text.Split(':')[1]))
            {
                xrRichText5.Text = "Afternoon ( दोपहर )";
            }
            else if (xrRichText5.Text.Split(':')[0].ToString() == "-2" && string.IsNullOrEmpty(xrRichText5.Text.Split(':')[1].ToString()))
            {
                xrRichText5.Text = "Afternoon";
            }
            else
            {
                xrRichText5.Text = "";
            }
        }
        //private void xrRichText7_BeforePrint(object sender, CancelEventArgs e)
        //{
        //    XRRichText xrRichTextBox7 = sender as XRRichText;
        //    if (xrRichText7.Text.Split(':')[0].ToString() == "-2" && xrRichText7.Text.Split(':')[1].ToString() == "2" || xrRichText7.Text.Split(':')[1].ToString() == "31")
        //    {
        //        xrRichTextBox7.Text = "Evening ( సాయంత్రం )";
        //    }
        //    else if (xrRichText7.Text.Split(':')[0].ToString() == "-2" && xrRichText7.Text.Split(':')[1].ToString() == "25")
        //    {
        //        xrRichTextBox7.Text = "Evening ( ସନ୍ଧ୍ୟା | )";
        //    }
        //    else if (xrRichText7.Text.Split(':')[0].ToString() == "-2" && !string.IsNullOrEmpty(xrRichText7.Text.Split(':')[1].ToString()))
        //    {
        //        xrRichTextBox7.Text = "Evening ( शाम )";
        //    }
        //    else if (xrRichText7.Text.Split(':')[0].ToString() == "-2" && string.IsNullOrEmpty(xrRichText7.Text.Split(':')[1].ToString()))
        //    {
        //        xrRichTextBox7.Text = "Evening";
        //    }
        //    else
        //    {
        //        xrRichTextBox7.Text = "";
        //    }
        //}
        //private void xrRichText9_BeforePrint(object sender, CancelEventArgs e)
        //{
        //    XRRichText xrRichTextBox9 = sender as XRRichText;
        //    if (xrRichText9.Text.Split(':')[0].ToString() == "-2" && xrRichText9.Text.Split(':')[1].ToString() == "2" || xrRichText9.Text.Split(':')[1].ToString() == "31")
        //    {
        //        xrRichTextBox9.Text = "Night ( రాత్రి )";
        //    }
        //    else if (xrRichText9.Text.Split(':')[0].ToString() == "-2" && xrRichText9.Text.Split(':')[1].ToString() == "25")
        //    {
        //        xrRichTextBox9.Text = "Night ( ରାତି )";
        //    }
        //    else if (xrRichText9.Text.Split(':')[0].ToString() == "-2" && !string.IsNullOrEmpty(xrRichText9.Text.Split('-')[1].ToString()))
        //    {
        //        xrRichTextBox9.Text = "Night ( रात )";
        //    }
        //    else if (xrRichText9.Text.Split(':')[0].ToString() == "-2" && string.IsNullOrEmpty(xrRichText9.Text.Split(':')[1].ToString()))
        //    {
        //        xrRichTextBox9.Text = "Night";
        //    }
        //    else
        //    {
        //        xrRichTextBox9.Text = "";
        //    }
        //}
        private void xrRichText11_BeforePrint(object sender, CancelEventArgs e)
        {
            XRRichText xrRichText11 = sender as XRRichText;
            string[] beforeFood = xrRichText11.Text.Split('-');
            if (beforeFood[1].ToString() == "2" || beforeFood[1].ToString() == "31")
            {
                xrRichText11.Text = beforeFood[0].ToString() + " ( " + (beforeFood[0].ToString() == "Before Food" ? "ఆహారం తినడానికి ముందు" : "ఆహారం తిన్న తర్వాత") + " )";
            }
            else if (beforeFood[1].ToString() == "25")
            {
                xrRichText11.Text = beforeFood[0].ToString() + " ( " + (beforeFood[0].ToString() == "Before Food" ? "ଖାଦ୍ୟ ଖାଇବା ପୂର୍ବରୁ" : "ଖାଦ୍ୟ ଖାଇବା ପରେ") + " )";

            }
            else if (!string.IsNullOrEmpty(beforeFood[1].ToString()))
            {
                xrRichText11.Text = beforeFood[0].ToString() + " ( " + (beforeFood[0].ToString() == "Before Food" ? "खाना खाने से पहले" : "खाना खाने के बाद") + " )";

            }
            else
            {
                xrRichText11.Text = beforeFood[0].ToString();
            }
        }
        private void xrPictureBox2_BeforePrint(object sender, CancelEventArgs e)
        {
            XRPictureBox pictureBox = sender as XRPictureBox;
            string _afternoon = Convert.ToString(GetCurrentColumnValue("Afternoon") as string);

            if (!string.IsNullOrEmpty(_afternoon) && _afternoon != "-1")
            {
                string imageUrl = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Reports", "Right.png");
                pictureBox.ImageUrl = imageUrl;
            }
            else
            {
                string imageUrl = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Reports", "Wrong.png");
                pictureBox.ImageUrl = imageUrl;
            }
        }
        private void xrPictureBox3_BeforePrint(object sender, CancelEventArgs e)
        {
            XRPictureBox pictureBox = sender as XRPictureBox;
            string _afternoon = this.GetCurrentColumnValue("Result3.Afternoon") as string;
            if (!string.IsNullOrEmpty(_afternoon) && _afternoon != "-1")
            {
                string imageUrl = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Reports", "Right.png");
                pictureBox.ImageUrl = imageUrl;
            }
            else
            {
                string imageUrl = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Reports", "Wrong.png");
                pictureBox.ImageUrl = imageUrl;
            }
        }
    }
}
